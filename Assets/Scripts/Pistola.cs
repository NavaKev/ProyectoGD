using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // Requerido para el Input System

public class Pistola : MonoBehaviour
{
    [Header("Configuración del Proyectil")]
    [SerializeField] private GameObject balaPrefab;
    [SerializeField] private Transform puntoDisparo; // Si es null, usa la posición de este GameObject
    [SerializeField] private PlayerMov playerMov;
    [SerializeField] private PlayerInput playerInput;

    [Header("Munición y Recarga")]
    [SerializeField] private int capacidadCargador = 3;
    [SerializeField] private float tiempoRecarga = 1.2f;
    [SerializeField] private int balasActuales;

    [Header("Tiempo entre disparos")]
    [SerializeField] private float tiempoEntreDisparos = 1f; // Tiempo mínimo entre disparos
    [SerializeField] private float ultimoDisparo;

    private InputAction dispararAccion;
    private InputAction recargarAccion;
    private bool estaRecargando = false;

    // Propiedades públicas para consultar estado o conectar a la UI
    public int BalasActuales => balasActuales;
    public int CapacidadCargador => capacidadCargador;
    public bool EstaRecargando => estaRecargando;

    private void Awake()
    {
        if (playerInput == null) playerInput = GetComponentInParent<PlayerInput>();
        if (playerMov == null) playerMov = GetComponentInParent<PlayerMov>();
        if (puntoDisparo == null) puntoDisparo = transform;

        balasActuales = capacidadCargador;
    }

    private void Start()
    {
        if (playerInput != null && playerInput.actions != null)
        {
            dispararAccion = playerInput.actions.FindAction("Disparar", false);
            recargarAccion = playerInput.actions.FindAction("Recargar", false);

            dispararAccion?.Enable();
            recargarAccion?.Enable();
        }
    }

    private void OnDisable()
    {
        dispararAccion?.Disable();
        recargarAccion?.Disable();
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

    private void IntentarDisparar()
    {
        if (estaRecargando)
        {
            Debug.Log("No se puede disparar: recargando...");
            return;
        }

        if (balasActuales <= 0)
        {
            Debug.Log("Cargador vacío.Presiona 'R' para recargar.");
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

        balasActuales--;

        GameObject bala = Instantiate(balaPrefab, puntoDisparo.position, puntoDisparo.rotation);

        Bala movProyectil = bala.GetComponent<Bala>();
        if (movProyectil != null && playerMov != null)
        {
            movProyectil.establecerDireccion(playerMov.mirandoDerecha ? 1f : -1f);
        }

        Debug.Log($"Disparo realizado. Balas restantes: {balasActuales}/{capacidadCargador}");
    }

    private IEnumerator RutinaRecarga()
    {
        estaRecargando = true;
        Debug.Log("Recargando...");

        yield return new WaitForSeconds(tiempoRecarga);

        balasActuales = capacidadCargador;
        estaRecargando = false;
        Debug.Log("Recarga completa. Cargador listo con 3 balas.");
    }
}