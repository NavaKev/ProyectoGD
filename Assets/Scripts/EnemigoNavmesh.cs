using UnityEngine;

public class EnemigoNavmesh : MonoBehaviour
{
    public Transform target; // El objetivo al que el enemigo seguirá
    public UnityEngine.AI.NavMeshAgent agent; // El agente de navegación del enemigo

    public VisionEnemigo visEnemigo; // Referencia al script VisionEnemigo
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false; // Desactivar la rotación automática del agente
        agent.updateUpAxis = false; // Desactivar la actualización del eje Y del agente
    }

    // Update is called once per frame
    void Update()
    {
        if (visEnemigo.posicionJugador != null)
        {
            // Si el jugador está dentro del rango de visión, el enemigo lo sigue
            agent.SetDestination(visEnemigo.posicionJugador.position);
        }
        else
        {
            // Si no hay jugador detectado, el enemigo puede quedarse en su posición actual o realizar otra acción
            agent.SetDestination(target.position); // Mantener la posición actual del enemigo
        }
    }
}
