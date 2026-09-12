using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class VidaJugador : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private int vidaMaxima = 50;
    private int vidaActual;

    [Header("Configuración de Escudo")]
    [SerializeField] private bool tieneEscudo = true;
    [SerializeField] private GameObject objetoEscudoVisual;

    [Header("UI de Muerte y Reaparición")]
    [SerializeField] private GameObject textoRevivir; 

    public TextMeshProUGUI textVida; //Barra de vida por texto
    [SerializeField] private Image barraVida; //Barra de vida por imagen
    private Vector3 posicionInicial;
    private bool estaMuerto = false;
    private Collider2D col2D;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    //private Vector3 posicionInicial;
    //private bool estaMuerto = false;
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

        // Activa o muestra el escudo al comenzar la partida
        if (objetoEscudoVisual != null)
        {
            objetoEscudoVisual.SetActive(tieneEscudo);
        }

        if (textoRevivir != null)
        {
            textoRevivir.SetActive(false);
        }
    }

    void Update()
    {
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

        // 1. Mecánica de Escudo: Bloquea el primer golpe de daño
        if (tieneEscudo)
        {
            tieneEscudo = false;

            if (objetoEscudoVisual != null)
            {
                objetoEscudoVisual.SetActive(false); // Oculta/borra la imagen del escudo
            }

            Debug.Log("¡El escudo absorbió el golpe y se ha roto!");
            return; // Detiene la ejecución para no restar puntos de vida
        }

        // 2. Daño normal a la vida del jugador
        vidaActual -= cantidad;
        textVida.text = "Vida: " + vidaActual;
        barraVida.fillAmount = (float)vidaActual / vidaMaxima;
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

        // Si aún tenía escudo al morir, se oculta
        if (objetoEscudoVisual != null)
        {
            objetoEscudoVisual.SetActive(false);
        }
    }

    public void Revivir()
    {
        estaMuerto = false;
        vidaActual = vidaMaxima;
        textVida.text = "Vida: " + vidaActual;
        barraVida.fillAmount = 1f; // Actualiza la barra de vida por imagen

        // Opcional: restaurar el escudo al revivir
        tieneEscudo = true;
        if (objetoEscudoVisual != null)
        {
            objetoEscudoVisual.SetActive(true);
        }

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