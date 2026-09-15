using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FuenteVida : MonoBehaviour
{
    [Header("Curación")]
    [SerializeField] private int puntosCuracion = 1;

    [SerializeField] private float intervaloCuracion = 1f;

    [Header("Efectos Visuales")]
    [SerializeField] private ParticleSystem particulasCuracion;


    private Coroutine corrutinaCuracion;
    private Collider2D col2D;

    void Awake()
    {
        col2D = GetComponent<Collider2D>();
        col2D.isTrigger = true; 

        
        if (particulasCuracion == null)
        {
            particulasCuracion = GetComponentInChildren<ParticleSystem>();
        }

        
        if (particulasCuracion != null)
        {
            var main = particulasCuracion.main;
            particulasCuracion.Stop();
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
        // Activa las partículas
        if (particulasCuracion != null && !particulasCuracion.isPlaying)
        {
            particulasCuracion.Play();
        }

        // Bucle que corre mientras el jugador esté dentro
        while (jugador != null)
        {
            // Solo cura si aún no ha alcanzado la vida máxima
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

        // Detiene la emisión (las partículas que ya salieron se desvanecen solas)
        if (particulasCuracion != null && particulasCuracion.isPlaying)
        {
            particulasCuracion.Stop();
        }
    }
}