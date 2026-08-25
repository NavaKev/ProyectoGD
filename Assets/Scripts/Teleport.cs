using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform tpSalida;

    public static float siguieteTeletransporte = 0f; 

    public float tiempBloqueo = 1f; // Tiempo de espera antes de poder teletransportarse nuevamente. 

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))         
        {
            if (Time.time > siguieteTeletransporte)
            {
                collision.transform.position = tpSalida.position;
                siguieteTeletransporte = Time.time + tiempBloqueo;

            }
        }
    }
           
}
