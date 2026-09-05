using UnityEngine;

public class EnemigoNavmesh : MonoBehaviour
{
    public Transform target; // El objetivo al que el enemigo seguirá
    public UnityEngine.AI.NavMeshAgent agent; // El agente de navegación del enemigo
    
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
        agent.SetDestination(target.position);
    }
}
