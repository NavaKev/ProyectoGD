using System.Collections;
using UnityEngine;

public class EnemySeg : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform jugador;

    [Header("Rango de Visión")]
    [Tooltip("Distancia en unidades a la que el enemigo detecta al jugador")]
    [SerializeField] private float rangoVision = 6.0f;

    [Header("Configuración de Persecución")]
    [SerializeField] private float velocidadPersecucion = 3.5f;
    [SerializeField] private float alturaSobreJugador = 4.0f;
    [SerializeField] private float distanciaMinima = 0.2f;

    [Header("Configuración de Patrullaje")]
    [SerializeField] private float velocidadPatrulla = 2.0f;
    [Tooltip("Distancia horizontal que recorrerá a izquierda y derecha desde su posición inicial")]
    [SerializeField] private float distanciaPatrulla = 4.0f;

    [Header("Spawn de Piedras")]
    [SerializeField] private GameObject prefabPiedra;
    [SerializeField] private Transform puntoSpawnPiedra;
    [SerializeField] private float intervaloSpawn = 1.5f;

    [Header("Disparo de Proyectil")]
    [SerializeField] private float velocidadProyectil = 8f;

    // Variables internas de control
    private Vector2 puntoInicial;
    private int direccionPatrulla = 1; // 1 = derecha, -1 = izquierda
    private bool jugadorEnRango = false;

    // Control de orientación (Flip)
    private bool mirandoDerecha = true;
    private float ultimaPosicionX;

    void Start()
    {
        puntoInicial = transform.position;
        ultimaPosicionX = transform.position.x;

        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                jugador = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("[EnemySeg] No se encontró ningún objeto con el Tag 'Player' en la escena.", this);
            }
        }

        if (puntoSpawnPiedra == null)
        {
            puntoSpawnPiedra = transform;
        }

        StartCoroutine(RutinaSpawnPiedras());
    }

    void Update()
    {
        EvaluarDeteccion();

        if (jugadorEnRango)
        {
            SeguirJugador();
        }
        else
        {
            Patrullar();
        }

        GestionarOrientacion();
    }

    private void EvaluarDeteccion()
    {
        if (jugador == null)
        {
            jugadorEnRango = false;
            return;
        }

        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);
        jugadorEnRango = (distanciaAlJugador <= rangoVision);
    }

    private void SeguirJugador()
    {
        Vector2 posicionObjetivo = new Vector2(jugador.position.x, jugador.position.y + alturaSobreJugador);

        if (Mathf.Abs(transform.position.x - jugador.position.x) > distanciaMinima)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                posicionObjetivo,
                velocidadPersecucion * Time.deltaTime
            );
        }
        else
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(transform.position.x, posicionObjetivo.y),
                velocidadPersecucion * Time.deltaTime
            );
        }
    }

    private void Patrullar()
    {
        float objetivoX = puntoInicial.x + (distanciaPatrulla * direccionPatrulla);
        Vector2 destinoPatrulla = new Vector2(objetivoX, puntoInicial.y);

        transform.position = Vector2.MoveTowards(
            transform.position,
            destinoPatrulla,
            velocidadPatrulla * Time.deltaTime
        );

        if (Mathf.Abs(transform.position.x - objetivoX) <= 0.05f)
        {
            direccionPatrulla *= -1;
        }
    }

    private void GestionarOrientacion()
    {
        // Calcula el desplazamiento horizontal ocurrido en este fotograma
        float deltaX = transform.position.x - ultimaPosicionX;

        // Umbral mínimo para descartar micro-movimientos por redondeo de físicas
        if (deltaX > 0.001f && !mirandoDerecha)
        {
            Voltear();
        }
        else if (deltaX < -0.001f && mirandoDerecha)
        {
            Voltear();
        }

        ultimaPosicionX = transform.position.x;
    }

    private void Voltear()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1; // Invierte el eje X del enemigo y de sus hijos (como el punto de spawn)
        transform.localScale = escala;
    }

    private IEnumerator RutinaSpawnPiedras()
    {
        yield return new WaitForSeconds(1.0f);

        while (true)
        {
            if (jugadorEnRango && prefabPiedra != null && jugador != null)
            {
                Vector2 direccion = ((Vector2)jugador.position - (Vector2)puntoSpawnPiedra.position).normalized;

                float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
                Quaternion rotacionDisparo = Quaternion.Euler(0f, 0f, angulo);

                GameObject piedra = Instantiate(prefabPiedra, puntoSpawnPiedra.position, rotacionDisparo);

                Rigidbody2D rbPiedra = piedra.GetComponent<Rigidbody2D>();
                if (rbPiedra != null)
                {
                    rbPiedra.linearVelocity = direccion * velocidadProyectil;
                }
            }

            yield return new WaitForSeconds(intervaloSpawn);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoVision);

        Vector2 centro = Application.isPlaying ? puntoInicial : (Vector2)transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(centro - Vector2.right * distanciaPatrulla, centro + Vector2.right * distanciaPatrulla);
    }
}