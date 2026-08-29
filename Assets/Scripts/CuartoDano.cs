using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CuartoDano : MonoBehaviour
{
    [Header("Tiempos")]
    [SerializeField] private float tiempoInicial = 2f;

    [SerializeField] private float intervaloDano = 2f;

    [Header("Daño")]
    [Tooltip("Daño inicial del primer golpe")]
    [SerializeField] private int danoInicial = 2;

    [SerializeField] private int incrementoDano = 2;

    [SerializeField] private int danoMaximo = 50;

    private Coroutine corrutinaDano;
    private Collider2D col2D;

    void Awake()
    {
        col2D = GetComponent<Collider2D>();
        col2D.isTrigger = true; // Asegura que funcione como trigger
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Busca si el objeto que entró tiene el componente VidaJugador
        VidaJugador vida = collision.GetComponent<VidaJugador>() ?? collision.GetComponentInParent<VidaJugador>();

        if (vida != null)
        {
            // Inicia la rutina de daño para este jugador
            if (corrutinaDano != null)
            {
                StopCoroutine(corrutinaDano);
            }
            corrutinaDano = StartCoroutine(RutinaDanoProgresivo(vida));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        VidaJugador vida = collision.GetComponent<VidaJugador>() ?? collision.GetComponentInParent<VidaJugador>();

        if (vida != null)
        {
            // Detiene el daño si el jugador sale de la habitación
            if (corrutinaDano != null)
            {
                StopCoroutine(corrutinaDano);
                corrutinaDano = null;
            }
        }
    }

    private IEnumerator RutinaDanoProgresivo(VidaJugador jugador)
    {
        int danoActual = danoInicial;

        // 1. Tiempo de gracia (2 segundos sin recibir daño)
        yield return new WaitForSeconds(tiempoInicial);

        // 2. Bucle de daño progresivo
        while (jugador != null)
        {
            // Aplica el daño actual
            jugador.recibirDano(danoActual);

            // Incrementa el daño para el próximo intervalo
            danoActual += incrementoDano;
            if (danoMaximo > 0 && danoActual > danoMaximo)
            {
                danoActual = danoMaximo;
            }

            // Espera el intervalo antes del siguiente golpe (2 segundos)
            yield return new WaitForSeconds(intervaloDano);
        }
    }
}