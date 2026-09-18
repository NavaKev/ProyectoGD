using UnityEngine;

public class DanadorContactoEnemigo : MonoBehaviour
{
    [Header("Configuración de Daño")]
    [SerializeField] private int danoContacto = 5;

    [SerializeField] private float intervaloDano = 1.0f;

    private float proximoAtaque = 0f;

    // Por si el enemigo o el jugador usan colisión sólida (Collision2D)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        EvaluarDano(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        EvaluarDano(collision.gameObject);
    }

    // Por si usan colisión de tipo Trigger (Collider2D Trigger)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EvaluarDano(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        EvaluarDano(collision.gameObject);
    }

    private void EvaluarDano(GameObject objetivo)
    {
        // 1. Verifica si es el jugador (por Tag o componente de vida)
        if (objetivo.CompareTag("Player") || (objetivo.transform.root != null && objetivo.transform.root.CompareTag("Player")))
        {
            // 2. Control de tiempo entre golpes
            if (Time.time >= proximoAtaque)
            {
                VidaJugador vida = objetivo.GetComponent<VidaJugador>() ?? objetivo.GetComponentInParent<VidaJugador>();

                if (vida != null)
                {
                    vida.recibirDano(danoContacto);
                    proximoAtaque = Time.time + intervaloDano;
                }
            }
        }
    }
}