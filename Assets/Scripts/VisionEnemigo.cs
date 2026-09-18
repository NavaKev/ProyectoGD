using UnityEngine;

public class VisionEnemigo : MonoBehaviour
{
    [Header("Configuración de Visión")]
    public float distanciaVision = 5f;
    public LayerMask capasVision;

    [Header("Detección")]
    public Transform posicionJugador;

    private Transform jugadorTransform;

    void Start()
    {
        // Busca al jugador por su Tag al iniciar
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            jugadorTransform = playerObj.transform;
        }
    }

    void Update()
    {
        DetectarJugador();
    }

    private void DetectarJugador()
    {
        if (jugadorTransform == null)
        {
            posicionJugador = null;
            return;
        }

        // 1. Comprueba si el jugador está dentro de los 5 metros de distancia
        float distanciaAlJugador = Vector2.Distance(transform.position, jugadorTransform.position);

        if (distanciaAlJugador <= distanciaVision)
        {
            // 2. Traza un rayo hacia el jugador para verificar si hay obstáculos en medio
            Vector2 direccion = (jugadorTransform.position - transform.position).normalized;
            RaycastHit2D impacto = Physics2D.Raycast(transform.position, direccion, distanciaVision, capasVision);

            Debug.DrawRay(transform.position, direccion * distanciaVision, Color.red);

            // Si el primer obstáculo impactado es el jugador, lo ve claramente
            if (impacto.collider != null && impacto.collider.CompareTag("Player"))
            {
                posicionJugador = jugadorTransform;
                return;
            }
        }

        // Si está a más de 5 metros o detrás de un muro, lo pierde de vista
        posicionJugador = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaVision);
    }
}