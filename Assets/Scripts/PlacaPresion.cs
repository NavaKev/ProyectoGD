using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlacaPresion : MonoBehaviour
{
    [Header("Configuración de Tiempo")]
    [SerializeField] private float tiempoParaDesactivar = 3f;

    [Header("Trampa de Pinchos Inferior")]
    [SerializeField] private GameObject trampaPinchos;
    [SerializeField] private bool trampaOcultaAlInicio = false;

    private Coroutine corrutinaTemporizador;
    private bool trampaActivada = false;

    private void Start()
    {
        if (trampaPinchos != null && trampaOcultaAlInicio)
        {
            trampaPinchos.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EvaluarContacto(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (corrutinaTemporizador == null && !trampaActivada)
        {
            EvaluarContacto(collision);
        }
    }

    private void EvaluarContacto(Collision2D collision)
    {
        if (trampaActivada) return;

        if (EsJugador(collision.gameObject))
        {
            // Validamos que el personaje esté sobre la placa
            if (collision.transform.position.y > transform.position.y)
            {
                if (corrutinaTemporizador == null)
                {
                    Debug.Log("Jugador sobre la placa. Iniciando cuenta");
                    corrutinaTemporizador = StartCoroutine(RutinaColapsoPlaca());
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (trampaActivada) return;

        if (EsJugador(collision.gameObject))
        {
            Debug.Log("Jugador salió antes de tiempo. Cuenta cancelada.");
            
            if (corrutinaTemporizador != null)
            {
                StopCoroutine(corrutinaTemporizador);
                corrutinaTemporizador = null;
            }
        }
    }

    private IEnumerator RutinaColapsoPlaca()
    {
        float tiempo = 0f;
        while (tiempo < tiempoParaDesactivar)
        {
            tiempo += Time.deltaTime;
            yield return null;
        }

        trampaActivada = true;
        Debug.Log("<Desactivando GameObject");

        // 1. Activa o revela los pinchos en el foso
        if (trampaPinchos != null)
        {
            trampaPinchos.SetActive(true);
        }

        // 2. Apaga completamente el GameObject de la placa (desactiva colisiones, render y animaciones al instante)
        gameObject.SetActive(false);
    }

    private bool EsJugador(GameObject obj)
    {
        return obj.CompareTag("Player") || obj.GetComponent<PlayerMov>() != null || obj.GetComponentInParent<PlayerMov>() != null;
    }
}