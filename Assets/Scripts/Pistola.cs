using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Pistola : MonoBehaviour
{
    [Header("Configuración del Proyectil")]
    [SerializeField] private GameObject balaPrefab;
    [SerializeField] private Transform puntoDisparo; // Punto de disparo horizontal por defecto
    [SerializeField] private Transform puntoDisparoArriba; // Punto cuando apunta hacia arriba
    [SerializeField] private PlayerMov playerMov;
    [SerializeField] private PlayerInput playerInput;

    [Header("Object Pool")]
    [Tooltip("Cantidad de balas instanciadas de inicio en la reserva")]
    [SerializeField] private int tamanoPoolInicial = 10;
    private List<Bala> poolBalas;
    private Transform contenedorPool;

    [Header("Munición y Recarga")]
    [SerializeField] private int capacidadCargador = 5;
    [SerializeField] private float tiempoRecarga = 1.2f;
    [SerializeField] private int balasActuales;

    [Header("Interfaz de Balas")]
    [SerializeField] private GameObject[] iconosBalas;

    [Header("Tiempo entre disparos")]
    [SerializeField] private float tiempoEntreDisparos = 0.5f;
    private float ultimoDisparo;

    [Header("Animación")]
    [SerializeField] private Animator anim;

    private InputAction dispararAccion;
    private InputAction recargarAccion;
    private InputAction moverAccion;
    private bool estaRecargando = false;

    // Propiedades públicas
    public int BalasActuales => balasActuales;
    public int CapacidadCargador => capacidadCargador;
    public bool EstaRecargando => estaRecargando;

    private void Awake()
    {
        if (playerInput == null) playerInput = GetComponentInParent<PlayerInput>();
        if (playerMov == null) playerMov = GetComponentInParent<PlayerMov>();
        if (puntoDisparo == null) puntoDisparo = transform;
        if (anim == null) anim = GetComponent<Animator>();

        balasActuales = capacidadCargador;

        InicializarPool();
    }

    private void Start()
    {
        if (playerInput != null && playerInput.actions != null)
        {
            dispararAccion = playerInput.actions.FindAction("Disparar", false);
            recargarAccion = playerInput.actions.FindAction("Recargar", false);
            moverAccion = playerInput.actions.FindAction("Mover", false);

            dispararAccion?.Enable();
            recargarAccion?.Enable();
            moverAccion?.Enable();
        }

        ActualizarUIBalas();
    }

    private void OnDisable()
    {
        dispararAccion?.Disable();
        recargarAccion?.Disable();
        moverAccion?.Disable();
    }

    private void Update()
    {
        if (dispararAccion != null && dispararAccion.WasPressedThisFrame())
        {
            IntentarDisparar();
        }

        bool presionoR = (recargarAccion != null && recargarAccion.WasPressedThisFrame()) ||
                         (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame);

        if (presionoR && !estaRecargando && balasActuales < capacidadCargador)
        {
            StartCoroutine(RutinaRecarga());
        }
    }

    private void InicializarPool()
    {
        poolBalas = new List<Bala>();

        // Crea un objeto vacío en la escena para mantener limpio el árbol de la jerarquía
        GameObject poolObj = new GameObject("Pool_Balas");
        contenedorPool = poolObj.transform;

        for (int i = 0; i < tamanoPoolInicial; i++)
        {
            CrearNuevaBalaParaPool();
        }
    }

    private Bala CrearNuevaBalaParaPool()
    {
        if (balaPrefab == null) return null;

        GameObject nuevaBala = Instantiate(balaPrefab, contenedorPool);
        nuevaBala.SetActive(false);

        Bala scriptBala = nuevaBala.GetComponent<Bala>();
        poolBalas.Add(scriptBala);

        return scriptBala;
    }

    private Bala ObtenerBalaDelPool()
    {
        // 1. Busca si hay alguna bala apagada disponible
        for (int i = 0; i < poolBalas.Count; i++)
        {
            if (poolBalas[i] != null && !poolBalas[i].gameObject.activeInHierarchy)
            {
                return poolBalas[i];
            }
        }

        // 2. Si se agotaron todas las disponibles, crea una extra dinámicamente
        return CrearNuevaBalaParaPool();
    }

    private void IntentarDisparar()
    {
        if (estaRecargando)
        {
            Debug.Log("No se puede disparar: recargando...");
            return;
        }

        if (balasActuales <= 0)
        {
            Debug.Log("Cargador vacío. Presiona 'R' para recargar.");
            return;
        }

        if (Time.time - ultimoDisparo < tiempoEntreDisparos)
        {
            return;
        }

        ultimoDisparo = Time.time;
        DispararProyectil();
    }

    private void DispararProyectil()
    {
        if (balaPrefab == null) return;

        // 1. Determinar dirección de disparo
        Vector2 direccion = Vector2.right;
        bool apuntandoArriba = false;

        if (moverAccion != null)
        {
            Vector2 inputMov = moverAccion.ReadValue<Vector2>();
            if (inputMov.y > 0.5f)
            {
                apuntandoArriba = true;
            }
        }

        if (!apuntandoArriba && Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            {
                apuntandoArriba = true;
            }
        }

        if (apuntandoArriba)
        {
            direccion = Vector2.up;
        }
        else
        {
            float dirX = (playerMov != null && playerMov.mirandoDerecha) ? 1f : -1f;
            direccion = new Vector2(dirX, 0f);
        }

        // 2. Determinar punto de origen
        Transform origen = (apuntandoArriba && puntoDisparoArriba != null) ? puntoDisparoArriba : puntoDisparo;

        // 3. Activar parámetros en el Animator
        if (anim != null)
        {
            anim.SetBool("ApuntandoArriba", apuntandoArriba);
            anim.SetTrigger("Disparar");
        }

        balasActuales--;
        ActualizarUIBalas();

        // 4. Tomar la bala del pool en vez de instanciarla
        Bala bala = ObtenerBalaDelPool();
        if (bala != null)
        {
            bala.transform.position = origen.position;
            bala.gameObject.SetActive(true); // Se activa y reinicia su temporizador
            bala.EstablecerDireccion(direccion);
        }

        Debug.Log($"Disparo realizado hacia {direccion}. Balas restantes: {balasActuales}/{capacidadCargador}");
    }

    private IEnumerator RutinaRecarga()
    {
        estaRecargando = true;
        if (anim != null)
        {
            anim.SetTrigger("Recargando");
        }

        yield return new WaitForSeconds(tiempoRecarga);

        balasActuales = capacidadCargador;
        ActualizarUIBalas();
        estaRecargando = false;
    }

    private void ActualizarUIBalas()
    {
        if (iconosBalas == null || iconosBalas.Length == 0) return;

        for (int i = 0; i < iconosBalas.Length; i++)
        {
            if (iconosBalas[i] != null)
            {
                iconosBalas[i].SetActive(i < balasActuales);
            }
        }
    }
}