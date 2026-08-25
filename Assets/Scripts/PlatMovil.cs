using UnityEngine;

public class PlatMovil : MonoBehaviour

{
    public Transform[] puntos; // Array de puntos de destino para el movimiento
    public int indiceDestino = 0; // Índice del punto de destino actual
    public float velocidad = 1f; // Velocidad de movimiento


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(this.transform.position, puntos[indiceDestino].position) <= 0.1f)
        {
            // Cambiar al siguiente punto de destino
            indiceDestino++;
            if (indiceDestino >= puntos.Length)
            {
                indiceDestino = 0; 
            }

            if (indiceDestino == 0)
            {
                
                System.Array.Reverse(puntos);
            }

        }
        this.transform.position = Vector3.MoveTowards(this.transform.position, puntos[indiceDestino].position, velocidad * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log("El jugador ha entrado en contacto con la plataforma móvil.");
            // Hacer que el jugador sea hijo de la plataforma para que se mueva con ella
            collision.transform.SetParent(this.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            // Quitar al jugador como hijo de la plataforma cuando salga de ella
            collision.transform.SetParent(null);
        }
    }
}
