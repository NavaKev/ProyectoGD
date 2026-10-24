using UnityEngine;

public class DetectarDanoEnemy : MonoBehaviour
{
    private SistemaVida vidaEnemigo;

    void Awake()
    {
        vidaEnemigo = GetComponentInParent<SistemaVida>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (vidaEnemigo == null) return;

        // Proyectil (Bala del Pool)
        if (collision.CompareTag("Bala"))
        {
            Bala bala = collision.GetComponent<Bala>() ?? collision.GetComponentInParent<Bala>();
            if (bala != null)
            {
                vidaEnemigo.RecibirDano(bala.Dano);
                bala.DesactivarBala();
            }
            else
            {
                collision.gameObject.SetActive(false);
            }
        }
    }
}