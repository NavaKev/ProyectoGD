using UnityEngine;

public class Trampolin : MonoBehaviour
{
    [Header("Configuración del Impulso")]
    [SerializeField] private float fuerzaSalto = 15f;

    [SerializeField] private bool usarAnimacionEscala = true;
    [SerializeField] private Vector3 escalaAlRebotar = new Vector3(1.2f, 0.6f, 1f);

    private Vector3 escalaOriginal;
    private Animator animator;

    private void Awake()
    {
        escalaOriginal = transform.localScale;
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica si el objeto que colisiona es el jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            // Opcional: Solo rebotar si el jugador cae desde arriba
            foreach (ContactPoint2D punto in collision.contacts)
            {
                if (punto.normal.y < -0.5f) // El impacto viene desde arriba hacia abajo
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
            // Resetea la velocidad en Y antes de aplicar la fuerza para que el impulso sea siempre igual
            rbJugador.linearVelocity = new Vector2(rbJugador.linearVelocity.x, 0f);

            // Aplica el impulso hacia arriba
            rbJugador.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);

            // Feedback visual: Si tienes trigger de animación o cambio de escala
            if (animator != null)
            {
                animator.SetTrigger("Rebotar");
            }
            else if (usarAnimacionEscala)
            {
                CancelInvoke(nameof(RestaurarEscala));
                transform.localScale = escalaAlRebotar;
                Invoke(nameof(RestaurarEscala), 0.15f);
            }
        }
    }

    private void RestaurarEscala()
    {
        transform.localScale = escalaOriginal;
    }
}   