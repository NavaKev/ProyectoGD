using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class VidaJugador : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private int vidaMaxima = 50;
    private int vidaActual;

    [Header("UI de Muerte y Reaparición")]
    [SerializeField] private GameObject textoRevivir; 

    [SerializeField] private GameObject textVida;
    private Collider2D col2D;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private Vector3 posicionInicial;
    private bool estaMuerto = false;

    void Awake()
    {
        col2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        posicionInicial = transform.position;
        vidaActual = vidaMaxima;
        textVida.GetComponent<TMP_Text>().text = "Vida: " + vidaActual;

        if (textoRevivir != null)
        {
            textoRevivir.SetActive(false);
        }
    }

    void Update()
    {
        // Detección con el nuevo Input System
        if (estaMuerto && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Revivir();
        }
    }

    public void Curar(int cantidad)
    {
        if (estaMuerto) return;

        vidaActual = Mathf.Min(vidaActual + cantidad, vidaMaxima);
        textVida.GetComponent<TMP_Text>().text = "Vida: " + vidaActual;
        Debug.Log("Vida actual: " + vidaActual);
    }

    public bool TieneVidaMaxima => vidaActual >= vidaMaxima;

    public void recibirDano(int cantidad)
    {
        if (estaMuerto) return;

        vidaActual -= cantidad;
        textVida.GetComponent<TMP_Text>().text = "Vida: " + vidaActual;
        Debug.Log("Vida actual: " + vidaActual);

        if (vidaActual <= 0)
        {
            morir();
        }
    }

    public void morir()
    {
        estaMuerto = true;
        vidaActual = 0;
        Debug.Log("El jugador ha muerto.");

        if (textoRevivir != null)
        {
            textoRevivir.SetActive(true);
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; 
            rb.simulated = false;
        }

        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (col2D != null) col2D.enabled = false;
    }

    public void Revivir()
    {
        estaMuerto = false;
        vidaActual = vidaMaxima;

        transform.position = posicionInicial;

        if (textoRevivir != null)
        {
            textoRevivir.SetActive(false);
        }

        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (col2D != null) col2D.enabled = true;

        Debug.Log("El jugador ha revivido con vida: " + vidaActual);
    }
}