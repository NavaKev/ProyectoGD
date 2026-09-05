using UnityEngine;

public class DetectarDanoEnemy : MonoBehaviour
{
    private VidaEnemigo vidaEnemigo; // Referencia al script de vida del enemigo
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bala"))
        {
            
            vidaEnemigo = GetComponentInParent<VidaEnemigo>();

            if (vidaEnemigo != null)
            {
                
                Bala bala = collision.GetComponent<Bala>();
                if (bala != null)
                {
                    int dano = bala.Dano; 
                    vidaEnemigo.RecibirDano(dano);
                }
            }

            
            Destroy(collision.gameObject);
        }
    }
}
