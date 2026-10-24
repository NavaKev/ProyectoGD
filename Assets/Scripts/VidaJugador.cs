using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VidaJugador : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private int vidaMaxima = 50;
    private int vidaActual;

    [Header("Inmunidad y Parpadeo")]
    [SerializeField] private float tiempoInmunidad = 2.0f;
    [SerializeField] private float intervaloParpadeo = 0.1f;
    private bool esInmune = false;

    [Header("Configuración de Escudo")]
    [SerializeField] private bool tieneEscudo = true;
    [SerializeField] private GameObject objetoEscudoVisual;

    [Header("UI de Muerte y Reaparición")]
    [SerializeField] private GameObject panelPantallaMuerte; 
    [SerializeField] private GameObject textoRevivir; 

    [Header("UI de Vida")]
    public TextMeshProUGUI textVida;
    [SerializeField] private Image barraVida;
    
    private Vector3 posicionInicial;
    private bool estaMuerto = false;
    private Collider2D col2D;
   [Header("Visuales")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    [Header("Animación")]
    [SerializeField] private Animator anim;

    void Awake()
    {
        col2D = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    void Start()
    {
        posicionInicial = transform.position;
        vidaActual = vidaMaxima;
        
        ActualizarUI();

        if (objetoEscudoVisual != null)
        {
            objetoEscudoVisual.SetActive(tieneEscudo);
        }

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
        ActualizarUI();
    }

    public bool TieneVidaMaxima => vidaActual >= vidaMaxima;

    public void recibirDano(int cantidad)
    {
        // 1. Ignorar el daño si está muerto o si el periodo de inmunidad está activo
        if (estaMuerto || esInmune) return;

        // 2. Mecánica de Escudo
        if (tieneEscudo)
        {
            tieneEscudo = false;

            if (objetoEscudoVisual != null)
            {
                objetoEscudoVisual.SetActive(false);
            }
            
            // Activa el parpadeo de inmunidad también al perder el escudo
            StartCoroutine(RutinaInmunidadYParpadeo());
            return;
        }

        // 3. Aplicar daño normal
        vidaActual = Mathf.Max(vidaActual - cantidad, 0);
        ActualizarUI();

        if (vidaActual <= 0)
        {
            morir();
        }
        else
        {
            // Inicia la inmunidad y el parpadeo tras recibir daño a la vida
            StartCoroutine(RutinaInmunidadYParpadeo());
        }
    }

    private void ActualizarUI()
    {
        if (textVida != null) textVida.text = "Vida: " + vidaActual;
        
        if (barraVida != null) 
        {
            float porcentaje = (float)vidaActual / vidaMaxima;
            barraVida.fillAmount = Mathf.Clamp01(porcentaje);
        }
    }

    private IEnumerator RutinaInmunidadYParpadeo()
    {
        esInmune = true;
        float tiempoPasado = 0f;

        // Bucle que enciende y apaga el sprite
        while (tiempoPasado < tiempoInmunidad)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }

            yield return new WaitForSeconds(intervaloParpadeo);
            tiempoPasado += intervaloParpadeo;
        }

        // Al finalizar, aseguramos que el sprite quede visible
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        esInmune = false;
    }

    public void morir()
    {
        estaMuerto = true;
        vidaActual = 0;
        ActualizarUI();
        
        // Detener la corrutina de parpadeo si el jugador muere por el daño
        StopAllCoroutines();
        esInmune = false;

        if (anim != null) anim.SetTrigger("Muerto");

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}