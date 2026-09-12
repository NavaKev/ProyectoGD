using UnityEngine;

public class VisionEnemigo : MonoBehaviour
{

    public float distanciaVision = 5f; // Distancia máxima de visión del enemigo
    public LayerMask capasVision; // Capa que representa los obstáculos

    public Transform posicionJugador; // Referencia al transform del jugador


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
    }

    void Update()
    {
        detectarJugador();
    }

    private void detectarJugador()
    {
        Vector2 direccion = this.transform.right; // Dirección hacia la derecha del enemigo
        RaycastHit2D impacto = Physics2D.Raycast(transform.position, direccion, distanciaVision, capasVision);

        Debug.DrawRay(transform.position, direccion * distanciaVision, Color.red); // Dibuja el rayo en la escena para depuración

        if (impacto.collider != null){
            if (impacto.collider.CompareTag("Player")){
                posicionJugador = impacto.collider.transform;  
            } else{
                posicionJugador = null;
                
            }
        }
    }

}

    
