using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Necesario para reiniciar la escena

public class VidaJugador : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private int vidaMaxima = 50;
    private int vidaActual;

    [Header("Configuración de Escudo")]
    [SerializeField] private bool tieneEscudo = true;
    [SerializeField] private GameObject objetoEscudoVisual;

    [Header("UI de Muerte y Reaparición")]
    [SerializeField] private GameObject panelPantallaMuerte; // Arrastra aquí la Image / Panel negro
    [SerializeField] private GameObject textoRevivir; 

    public TextMeshProUGUI textVida;
    [SerializeField] private Image barraVida;
    private Vector3 posicionInicial;
    private bool estaMuerto = false;
    private Collider2D col2D;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    [Header("Animación")]
    [SerializeField] private Animator anim;

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
        if (textVida != null) textVida.text = "Vida: " + vidaActual;
        if (barraVida != null) barraVida.fillAmount = 1f;

        if (objetoEscudoVisual != null)
        {
            objetoEscudoVisual.SetActive(tieneEscudo);
        }

        // Se asegura de que la pantalla en negro esté oculta al iniciar
        if (panelPantallaMuerte != null)
        {
            panelPantallaMuerte.SetActive(false);
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
        if (textVida != null) textVida.text = "Vida: " + vidaActual;
        if (barraVida != null) barraVida.fillAmount = (float)vidaActual / vidaMaxima;
    }

    public bool TieneVidaMaxima => vidaActual >= vidaMaxima;

    public void recibirDano(int cantidad)
    {
        if (estaMuerto) return;

        if (tieneEscudo)
        {
            tieneEscudo = false;

            if (objetoEscudoVisual != null)
            {
                objetoEscudoVisual.SetActive(false);
            }
            return;
        }

        vidaActual -= cantidad;
        if (textVida != null) textVida.text = "Vida: " + vidaActual;
        if (barraVida != null) barraVida.fillAmount = (float)vidaActual / vidaMaxima;

        if (vidaActual <= 0)
        {
            morir();
        }
    }

    public void morir()
    {
        estaMuerto = true;
        vidaActual = 0;
        if (anim != null) anim.SetTrigger("Muerto");

        // Activa la pantalla negra al morir
        if (panelPantallaMuerte != null)
        {
            panelPantallaMuerte.SetActive(true);
        }

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

        if (objetoEscudoVisual != null)
        {
            objetoEscudoVisual.SetActive(false);
        }
    }

    public void Revivir()
    {
        // Al recargar la escena, la vida, los enemigos eliminados y la interfaz vuelven automáticamente a su estado original
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}