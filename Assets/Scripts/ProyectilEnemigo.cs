using UnityEngine;

public class ProyectilEnemigo : MonoBehaviour
{
    [SerializeField] private int danoProyectil = 10;

    private void Start()
    {
        Destroy(gameObject, 5f); // Se destruye tras 5 segundos para limpiar memoria
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Si golpea al jugador, le quita vida y se destruye
        if (col.CompareTag("Player") || (col.transform.root != null && col.transform.root.CompareTag("Player")))
        {
            VidaJugador vida = col.GetComponent<VidaJugador>() ?? col.GetComponentInParent<VidaJugador>();
            if (vida != null)
            {
                vida.recibirDano(danoProyectil);
            }
            Destroy(gameObject);
            return;
        }

        // Si toca un muro o el piso (y no es el jefe), también se destruye
        /*if (!col.CompareTag("Enemy") && !col.isTrigger) 
        {
            Destroy(gameObject);
        }*/
    }
}