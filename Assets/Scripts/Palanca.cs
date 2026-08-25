using UnityEngine;

public class Palanca : MonoBehaviour
{
    [SerializeField] private GameObject puerta;
    [SerializeField] private float alturaApertura = 3f;

    private SpriteRenderer spriteRendererPalanca;
    private bool abierto = false;

    private Vector3 posicionCerrada;
    private Vector3 posicionAbierta;

    private void Awake()
    {
        spriteRendererPalanca = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (puerta != null)
        {
            // Guardamos las posiciones exactas en el mundo
            posicionCerrada = puerta.transform.position;
            posicionAbierta = posicionCerrada + (Vector3.up * alturaApertura);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            abierto = !abierto;

            if (abierto)
            {
               
                puerta.transform.position = posicionAbierta;
                //spriteRendererPalanca.color = Color.blue;
                transform.rotation = Quaternion.Euler(0, 0, -90);
            }
            else
            {
                // Cerrar: regresa la puerta a su posición inicial exacta
                puerta.transform.position = posicionCerrada;
                //spriteRendererPalanca.color = Color.yellow;
                transform.rotation = Quaternion.identity; // Vuelve a 0 grados
            }
        }
    }
}