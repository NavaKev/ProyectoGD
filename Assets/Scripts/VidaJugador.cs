using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class VidaJugador : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private int vidaMaxima = 50;
    private int vidaActual;

    [Header("UI de Muerte y Reaparición")]
    [SerializeField] private GameObject textoRevivir; 

    public TextMeshProUGUI textVida; //Barra de vida por texto

    [SerializeField] private Image barraVida; //Barra de vida por imagen

    
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
        textVida.text = "Vida: " + vidaActual; //Barra de Vida por text
        barraVida.fillAmount = 1f;  // Barra de vida por imagen

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
        textVida.text = "Vida: " + vidaActual;
        barraVida.fillAmount = (float)vidaActual / vidaMaxima; // Actualiza la barra de vida por imagen
        Debug.Log("Vida actual: " + vidaActual);
    }

    public bool TieneVidaMaxima => vidaActual >= vidaMaxima;

    public void recibirDano(int cantidad)
    {
        if (estaMuerto) return;

        vidaActual -= cantidad;
        Debug.Log("Vida actual: " + vidaActual);

        if (vidaActual <= 0)
        {
            morir();
        }

        textVida.text = "Vida: " + vidaActual;
        barraVida.fillAmount = (float)vidaActual / vidaMaxima; // Actualiza la barra de vida por imagen
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
        textVida.text = "Vida: " + vidaActual;
        barraVida.fillAmount = 1f; // Actualiza la barra de vida por imagen

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