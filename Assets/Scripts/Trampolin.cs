using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Trampolin : MonoBehaviour
{
    [Header("Configuración del Impulso")]
    [SerializeField] private float fuerzaSalto = 15f;

    [Header("Animación")]
    [SerializeField] private Animator animator;
    [SerializeField] private string parametroAnimacion = "Work";

    private readonly int animHash = Animator.StringToHash("Work");

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            foreach (ContactPoint2D punto in collision.contacts)
            {
                if (punto.normal.y < -0.5f)
                {
                    ImpulsarJugador(collision.gameObject);
                    break;
                }
            }
        }
    }

    private void ImpulsarJugador(GameObject jugador)
    {
        Rigidbody2D rbJugador = jugador.GetComponent<Rigidbody2D>();

        if (rbJugador != null)
        {
            // Resetea la inercia vertical previa
            rbJugador.linearVelocity = new Vector2(rbJugador.linearVelocity.x, 0f);

            // Aplica el salto
            rbJugador.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);

            // Activa la animación del trampolín
            ActivarAnimacionRebote();
        }
    }

    private void ActivarAnimacionRebote()
    {
        if (animator == null) return;
        animator.SetTrigger(animHash);

        
    }
}