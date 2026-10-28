using System.Collections;
using UnityEngine;

public class BossCuervo : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform jugador;
    [SerializeField] private SpriteRenderer spriteBoss;

    [Header("Nodos de Teletransporte")]
    [Tooltip("Puntos vacíos en el aire donde el cuervo puede aparecer")]
    [SerializeField] private Transform[] nodosTeletransporte;
    
    [Header("Tiempos de Fase")]
    [Tooltip("Tiempo que se queda quieto atacando antes de huir")]
    [SerializeField] private float tiempoEsperaEnNodo = 3f;
    [Tooltip("Tiempo que permanece invisible moviéndose entre las sombras")]
    [SerializeField] private float tiempoDesaparecido = 0.5f;

    [Header("Ataque: Plumas de Esencia")]
    [Tooltip("Prefab del proyectil (bala) que lanzará el jefe")]
    [SerializeField] private GameObject prefabPluma;
    [Tooltip("De dónde sale la pluma (si está vacío, saldrá del centro del Boss)")]
    [SerializeField] private Transform puntoDisparo;
    [Tooltip("Velocidad de la pluma al volar")]
    [SerializeField] private float velocidadProyectil = 10f;
    [Tooltip("Cantidad de plumas por ráfaga")]
    [SerializeField] private int cantidadProyectilesPorRafaga = 3;
    [Tooltip("Tiempo de espera entre cada pluma de la ráfaga")]
    [SerializeField] private float cadenciaDisparo = 0.3f;

    private int indiceNodoActual = -1;
    private bool batallaActiva = false;
    private bool mirandoDerecha = true;

    private void Start()
    {
        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) jugador = playerObj.transform;
        }

        if (spriteBoss == null) spriteBoss = GetComponentInChildren<SpriteRenderer>();
        if (puntoDisparo == null) puntoDisparo = transform;
    }

    private void OnEnable()
    {
        // Al encenderse por el Controlador de la Arena, inicia su ciclo
        batallaActiva = true;
        StartCoroutine(RutinaTeletransporteYAtaque());
    }

    private void OnDisable()
    {
        batallaActiva = false;
        StopAllCoroutines();
    }

    private void Update()
    {
        // Mientras sea visible, el jefe siempre debe encarar al jugador
        if (spriteBoss != null && spriteBoss.enabled && jugador != null)
        {
            MirarHaciaJugador();
        }
    }

    private IEnumerator RutinaTeletransporteYAtaque()
    {
        MoverANodoAleatorio();

        while (batallaActiva)
        {
            // FASE DE ATAQUE: Dispara una ráfaga de plumas hacia Luck
            yield return StartCoroutine(RutinaDispararRáfaga());

            // Espera el tiempo restante de su fase en el nodo (o se escapa de inmediato si la ráfaga duró más)
            yield return new WaitForSeconds(tiempoEsperaEnNodo);

            // FASE DE EVASIÓN: Desaparece
            if (spriteBoss != null) spriteBoss.enabled = false;
            
            yield return new WaitForSeconds(tiempoDesaparecido);

            // FASE DE REAPARICIÓN: Se mueve a un nuevo nodo y se hace visible
            MoverANodoAleatorio();
            if (spriteBoss != null) spriteBoss.enabled = true;
        }
    }

    private IEnumerator RutinaDispararRáfaga()
    {
        // El cuervo dispara una ráfaga calculada
        for (int i = 0; i < cantidadProyectilesPorRafaga; i++)
        {
            if (jugador == null || prefabPluma == null) break;

            DispararPlumaDirigida();
            yield return new WaitForSeconds(cadenciaDisparo);
        }
    }

    private void DispararPlumaDirigida()
    {
        // 1. Calculamos la dirección del disparo hacia donde esté el jugador en ese instante
        Vector2 direccion = ((Vector2)jugador.position - (Vector2)puntoDisparo.position).normalized;

        // 2. Calculamos el ángulo para rotar la pluma hacia él
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        Quaternion rotacionDisparo = Quaternion.Euler(0f, 0f, angulo);

        // 3. Instanciamos la pluma
        GameObject pluma = Instantiate(prefabPluma, puntoDisparo.position, rotacionDisparo);

        // 4. Asignamos la velocidad en línea recta usando el Rigidbody de la pluma
        Rigidbody2D rbPluma = pluma.GetComponent<Rigidbody2D>();
        if (rbPluma != null)
        {
            rbPluma.linearVelocity = direccion * velocidadProyectil;
        }
    }

    // Funciones de movimiento
    private void MoverANodoAleatorio()
    {
        if (nodosTeletransporte == null || nodosTeletransporte.Length == 0) return;

        int nuevoIndice = indiceNodoActual;
        
        if (nodosTeletransporte.Length > 1)
        {
            while (nuevoIndice == indiceNodoActual)
            {
                nuevoIndice = Random.Range(0, nodosTeletransporte.Length);
            }
        }
        else
        {
            nuevoIndice = 0;
        }

        indiceNodoActual = nuevoIndice;
        transform.position = nodosTeletransporte[indiceNodoActual].position;
    }

    private void MirarHaciaJugador()
    {
        float diferenciaX = jugador.position.x - transform.position.x;

        if (diferenciaX > 0 && !mirandoDerecha)
        {
            Voltear();
        }
        else if (diferenciaX < 0 && mirandoDerecha)
        {
            Voltear();
        }
    }

    private void Voltear()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1; 
        transform.localScale = escala;
    }
}