using UnityEngine;

public class PlatMovil : MonoBehaviour
{
    public Transform[] puntos;
    public int indiceDestino = 0;
    public float velocidad = 1f;

    [Header("Tiempo de Espera")]
    public float tiempoEspera = 2f;
    private float temporizadorEspera = 0f;
    private int direccion = 1;

    void Start()
    {
        if (puntos != null && puntos.Length > 0 && puntos[0] != null)
        {
            transform.position = puntos[0].position;
        }
    }

    void Update()
    {
        if (puntos == null || puntos.Length < 2) return;

        if (temporizadorEspera > 0f)
        {
            temporizadorEspera -= Time.deltaTime;
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, puntos[indiceDestino].position, velocidad * Time.deltaTime);

        if (Vector3.Distance(transform.position, puntos[indiceDestino].position) <= 0.05f)
        {
            if (indiceDestino >= puntos.Length - 1 && direccion == 1)
            {
                direccion = -1;
                temporizadorEspera = tiempoEspera;
            }
            else if (indiceDestino <= 0 && direccion == -1)
            {
                direccion = 1;
                temporizadorEspera = tiempoEspera;
            }

            indiceDestino += direccion;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Rigidbody2D rbPlayer = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rbPlayer != null && rbPlayer.linearVelocity.y <= 0.1f)
            {
                collision.transform.SetParent(this.transform);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (collision.transform.parent == this.transform)
            {
                collision.transform.SetParent(null);
            }
        }
    }
}