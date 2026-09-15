using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CuartoDano : MonoBehaviour
{
    [Header("Efectos Visuales")]
    [SerializeField] private ParticleSystem particulasCuarto;

    [Header("Tiempos")]
    [SerializeField] private float tiempoInicial = 2f;
    [SerializeField] private float intervaloDano = 2f;

    [Header("Daño")]
    [SerializeField] private int danoInicial = 2;
    [SerializeField] private int incrementoDano = 2;
    [SerializeField] private int danoMaximo = 50;

    private Coroutine corrutinaDano;
    private Collider2D col2D;

    void Awake()
    {
        col2D = GetComponent<Collider2D>();
        col2D.isTrigger = true; 

        // Busca automáticamente el ParticleSystem si no fue asignado en el Inspector
        if (particulasCuarto == null)
        {
            particulasCuarto = GetComponentInChildren<ParticleSystem>();
        }

        // Se asegura de que no comience reproduciéndose por error
        if (particulasCuarto != null)
        {
            particulasCuarto.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Busca si el objeto que entró tiene el componente VidaJugador
        VidaJugador vida = collision.GetComponent<VidaJugador>() ?? collision.GetComponentInParent<VidaJugador>();
        if (vida != null)
        {
            // Activa las partículas al entrar a la zona
            if (particulasCuarto != null && !particulasCuarto.isPlaying)
            {
                particulasCuarto.Play();
            }

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
        VidaJugador vida = collision.GetComponent<VidaJugador>() ?? collision.GetComponentInParent<VidaJugador>(); //[cite: 2]

        if (vida != null)
        {
            // Detiene las partículas al salir (las partículas vivas se desvanecen con naturalidad)
            if (particulasCuarto != null && particulasCuarto.isPlaying)
            {
                particulasCuarto.Stop();
            }

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

        // 1. Tiempo de gracia (sin recibir daño)
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

            // Espera el intervalo antes del siguiente golpe
            yield return new WaitForSeconds(intervaloDano);
        }
    }
}