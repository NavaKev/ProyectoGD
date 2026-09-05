using UnityEngine;

public class Bala : MonoBehaviour
{
    public float velocidad = 1f; 
    public float direccion = 1f; // 1 para derecha, -1 para izquierda
    private Rigidbody2D rb;

    public int Dano { get; set; } = 2; 

    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Destroy(gameObject, 10f); // Destruir la bala después de 10 segundos
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(direccion * velocidad,0);
    }

    public void establecerDireccion(float nuevaDireccion)
    {
        direccion = nuevaDireccion;

        if (direccion > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); // Voltear la bala horizontalmente
        }
        else if (direccion < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

    }
}
