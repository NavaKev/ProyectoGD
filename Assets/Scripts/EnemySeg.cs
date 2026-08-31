using System.Collections;
using UnityEngine;

public class EnemySeg : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform jugador;

    [Header("Configuración de Persecución")]
    [SerializeField] private float velocidad = 3.5f;
    [Tooltip("Altura fija a la que flotará sobre la posición del jugador")]
    [SerializeField] private float alturaSobreJugador = 4.0f;
    [Tooltip("Distancia horizontal mínima para empezar a moverse")]
    [SerializeField] private float distanciaMinima = 0.2f;

    [Header("Spawn de Piedras")]
    [SerializeField] private GameObject prefabPiedra;
    [Tooltip("Punto de salida debajo de la nube (si se deja vacío, usa la posición de la nube)")]
    [SerializeField] private Transform puntoSpawnPiedra;
    [SerializeField] private float intervaloSpawn = 1.5f;

    void Start()
    {
        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                jugador = playerObj.transform;
            }
            else
            {
                Debug.LogWarning($"[EnemySeg] No se encontró ningún objeto con el Tag 'Player' en la escena.", this);
            }
        }

        if (puntoSpawnPiedra == null)
        {
            puntoSpawnPiedra = transform;
        }

        // Inicia el bucle continuo de spawn de piedras
        StartCoroutine(RutinaSpawnPiedras());
    }

    void Update()
    {
        if (jugador == null) return;

        SeguirJugador();
    }

    private void SeguirJugador()
    {
        // Calculamos la posición destino (misma X que el jugador, pero a una altura fija en Y)
        Vector2 posicionObjetivo = new Vector2(jugador.position.x, jugador.position.y + alturaSobreJugador);

        // Solo se desplaza si no está ya justo encima del jugador
        if (Mathf.Abs(transform.position.x - jugador.position.x) > distanciaMinima)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                posicionObjetivo,
                velocidad * Time.deltaTime
            );
        }
        else
        {
            // Ajusta suavemente la altura vertical en caso de que el jugador salte o baje
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(transform.position.x, posicionObjetivo.y),
                velocidad * Time.deltaTime
            );
        }
    }

    private IEnumerator RutinaSpawnPiedras()
    {
        // Espera inicial de 1 segundo antes de soltar la primera piedra
        yield return new WaitForSeconds(1.0f);

        while (true)
        {
            if (prefabPiedra != null)
            {
                Instantiate(prefabPiedra, puntoSpawnPiedra.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(intervaloSpawn);
        }
    }
}