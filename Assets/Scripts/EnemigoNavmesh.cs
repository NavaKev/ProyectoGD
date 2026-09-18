using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemigoNavmesh : MonoBehaviour
{
    [Header("Patrulla (Mínimo 2 puntos)")]
    [SerializeField] private Transform[] puntosPatrulla;
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
        agent.updateRotation = false; // Mantiene fijas las rotaciones 3D
        agent.updateUpAxis = false;   // Permite la orientación en plano 2D XY

        if (visEnemigo == null)
        {
            visEnemigo = GetComponent<VisionEnemigo>() ?? GetComponentInChildren<VisionEnemigo>();
        }
    }

    void Start()
    {
        if (puntosPatrulla != null && puntosPatrulla.Length > 0 && puntosPatrulla[0] != null)
        {
            agent.SetDestination(puntosPatrulla[0].position);
        }
    }

    void Update()
    {
        // 1. Voltea el sprite según la dirección en la que camina
        GestionarGiro();

        // 2. Persecución (si el jugador está dentro de los 5m y visible)
        if (visEnemigo != null && visEnemigo.posicionJugador != null)
        {
            agent.SetDestination(visEnemigo.posicionJugador.position);
            return;
        }

        // 3. Patrullaje
        Patrullar();
    }

    private void GestionarGiro()
    {
        // Si se mueve hacia la derecha y está mirando a la izquierda
        if (agent.velocity.x > 0.1f && !mirandoDerecha)
        {
            Voltear();
        }
        // Si se mueve hacia la izquierda y está mirando a la derecha
        else if (agent.velocity.x < -0.1f && mirandoDerecha)
        {
            Voltear();
        }
    }

    private void Voltear()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1; // Invierte el eje horizontal del sprite y de su visión
        transform.localScale = escala;
    }

    private void Patrullar()
    {
        if (puntosPatrulla == null || puntosPatrulla.Length < 2) return;

        Transform puntoObjetivo = puntosPatrulla[indiceActual];
        if (puntoObjetivo == null) return;

        agent.SetDestination(puntoObjetivo.position);

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