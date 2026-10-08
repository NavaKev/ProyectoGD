using UnityEngine;

public class Bala : MonoBehaviour
{
    public float velocidad = 10f;
    private Vector2 direccion = Vector2.right; // Dirección por defecto
    private Rigidbody2D rb;

    public int Dano { get; set; } = 2;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 5f); // Destruye la bala después de 5 segundos
    }

    void FixedUpdate()
    {
        // Aplica velocidad en la dirección X e Y recibida
        rb.linearVelocity = direccion * velocidad;
    }

    // Sobrecarga para mantener compatibilidad si se envía un float (izquierda / derecha)
    public void establecerDireccion(float direccionX)
    {
        EstablecerDireccion(new Vector2(direccionX, 0f));
    }

    // Método principal para asignar dirección en 2D (arriba, abajo, diagonales)
    public void EstablecerDireccion(Vector2 nuevaDireccion)
    {
        direccion = nuevaDireccion.normalized;

        // Calcula el ángulo en grados y rota el sprite de la bala sobre el eje Z
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo);
    }
}