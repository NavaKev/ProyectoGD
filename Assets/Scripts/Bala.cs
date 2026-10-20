using UnityEngine;

public class Bala : MonoBehaviour
{
    [Header("Configuración de Vuelo")]
    [SerializeField] private float velocidad = 10f;
    [SerializeField] private float tiempoVida = 3.0f; 
    [SerializeField] private int dano = 2;

    [Header("Filtros")]
    [SerializeField] private string tagEnemigo = "Enemy";
    [SerializeField] private string tagJugador = "Player";

    private Vector2 direccion = Vector2.right;
    private Rigidbody2D rb;
    private float temporizadorDesactivacion;

    public int Dano => dano;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        // Reinicia la cuenta regresiva de 3 segundos cada vez que la bala sale del pool
        temporizadorDesactivacion = tiempoVida;
    }

    private void Update()
    {
        // Cuenta los 3 segundos en el aire
        temporizadorDesactivacion -= Time.deltaTime;
        if (temporizadorDesactivacion <= 0f)
        {
            DesactivarBala(); // Se oculta y vuelve al pool tras 3 segundos
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = direccion * velocidad;
        }
    }

    public void EstablecerDireccion(Vector2 nuevaDireccion)
    {
        direccion = nuevaDireccion.normalized;

        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angulo);
    }

    public void establecerDireccion(float direccionX)
    {
        EstablecerDireccion(new Vector2(direccionX, 0f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EvaluarImpacto(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EvaluarImpacto(collision.collider);
    }

    private void EvaluarImpacto(Collider2D col)
    {
        // 1. Ignorar colisiones con el propio jugador
        if (col.CompareTag(tagJugador) || (col.transform.root != null && col.transform.root.CompareTag(tagJugador)))
        {
            return;
        }

        
        if (col.CompareTag(tagEnemigo) || (col.transform.root != null && col.transform.root.CompareTag(tagEnemigo)))
        {
            DesactivarBala();
            return;
        }

    }

    public void DesactivarBala()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        gameObject.SetActive(false); // Vuelve al pool
    }
}