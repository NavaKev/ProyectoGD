using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemigoNavmesh : MonoBehaviour
{
    [Header("Patrulla (Mínimo 2 puntos)")]
    [Tooltip("Puntos de ruta entre los que patrullará")]
    [SerializeField] private Transform[] puntosPatrulla;
    [Tooltip("Distancia mínima al punto para pasar al siguiente")]
    [SerializeField] private float distanciaLlegada = 0.5f;

    [Header("Referencias")]
    [SerializeField] private VisionEnemigo visEnemigo;
    private NavMeshAgent agent;

    private int indiceActual = 0;
    private int direccionPatrulla = 1; // 1 = adelante, -1 = atrás
    private bool mirandoDerecha = true;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // Configuración crítica para navegación 2D plana
        agent.updateRotation = false; // Mantiene fijas las rotaciones 3D[cite: 1]
        agent.updateUpAxis = false;   // Permite la orientación en plano 2D XY[cite: 1]

        // Asegura que el enemigo inicie en Z = 0
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        if (visEnemigo == null)
        {
            visEnemigo = GetComponent<VisionEnemigo>() ?? GetComponentInChildren<VisionEnemigo>();
        }
    }

    void Start()
    {
        if (puntosPatrulla != null && puntosPatrulla.Length > 0 && puntosPatrulla[0] != null)
        {
            FijarDestino(puntosPatrulla[0].position);
        }
    }

    void Update()
    {
        // Bloqueo continuo del eje Z en 0 para evitar hundimiento en profundidad
        if (transform.position.z != 0f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        }

        // 1. Voltea el sprite según la dirección horizontal
        GestionarGiro();

        // 2. Persecución (si el jugador está dentro de la visión del enemigo)
        if (visEnemigo != null && visEnemigo.posicionJugador != null)
        {
            FijarDestino(visEnemigo.posicionJugador.position);
            return;
        }

        // 3. Patrullaje de ida y vuelta
        Patrullar();
    }

    private void FijarDestino(Vector3 objetivo)
    {
        // Fija el destino en el plano 2D con Z = 0
        Vector3 destino2D = new Vector3(objetivo.x, objetivo.y, 0f);
        agent.SetDestination(destino2D);
    }

    private void GestionarGiro()
    {
        // Movimiento a la derecha mirando a la izquierda
        if (agent.velocity.x > 0.1f && !mirandoDerecha)
        {
            Voltear();
        }
        // Movimiento a la izquierda mirando a la derecha
        else if (agent.velocity.x < -0.1f && mirandoDerecha)
        {
            Voltear();
        }
    }

    private void Voltear()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1; // Invierte el eje horizontal del sprite y sus componentes hijos[cite: 1]
        transform.localScale = escala;
    }

    private void Patrullar()
    {
        if (puntosPatrulla == null || puntosPatrulla.Length < 2) return;

        Transform puntoObjetivo = puntosPatrulla[indiceActual];
        if (puntoObjetivo == null) return;

        FijarDestino(puntoObjetivo.position);

        if (!agent.pathPending && agent.remainingDistance <= distanciaLlegada)
        {
            CambiarPuntoPatrulla();
        }
    }

    private void CambiarPuntoPatrulla()
    {
        if (indiceActual == puntosPatrulla.Length - 1 && direccionPatrulla == 1)
        {
            direccionPatrulla = -1;
        }
        else if (indiceActual == 0 && direccionPatrulla == -1)
        {
            direccionPatrulla = 1;
        }

        indiceActual += direccionPatrulla;
    }
}