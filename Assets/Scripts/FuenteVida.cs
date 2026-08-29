using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FuenteVida : MonoBehaviour
{
    [Header("Curación")]

    [SerializeField] private int puntosCuracion = 1;

    [Tooltip("Tiempo en segundos entre cada curación")]
    [SerializeField] private float intervaloCuracion = 1f;

    [Header("Visuales")]
    [SerializeField] private SpriteRenderer spriteFuente;
    [SerializeField] private Color colorInactivo = Color.white;
    [SerializeField] private Color colorCurando = Color.green;

    private Coroutine corrutinaCuracion;
    private Collider2D col2D;

    void Awake()
    {
        col2D = GetComponent<Collider2D>();
        col2D.isTrigger = true; 

        if (spriteFuente == null)
        {
            spriteFuente = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        VidaJugador jugador = collision.GetComponent<VidaJugador>() ?? collision.GetComponentInParent<VidaJugador>();

        if (jugador != null)
        {
            if (corrutinaCuracion != null)
            {   
                StopCoroutine(corrutinaCuracion);
            }
            corrutinaCuracion = StartCoroutine(RutinaCuracion(jugador));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        VidaJugador jugador = collision.GetComponent<VidaJugador>() ?? collision.GetComponentInParent<VidaJugador>();

        if (jugador != null)
        {
            DetenerCuracion();
        }
    }

    private IEnumerator RutinaCuracion(VidaJugador jugador)
    {
        if (spriteFuente != null) spriteFuente.color = colorCurando;

        // Bucle que corre mientras el jugador esté dentro
        while (jugador != null)
        {
            // Solo cura si aún no ha alcanzado el límite máximo
            if (!jugador.TieneVidaMaxima)
            {
                jugador.Curar(puntosCuracion);
            }

            yield return new WaitForSeconds(intervaloCuracion);
        }

        DetenerCuracion();
    }

    private void DetenerCuracion()
    {
        if (corrutinaCuracion != null)
        {
            StopCoroutine(corrutinaCuracion);
            corrutinaCuracion = null;
        }

        if (spriteFuente != null)
        {
            spriteFuente.color = colorInactivo;
        }
    }
}