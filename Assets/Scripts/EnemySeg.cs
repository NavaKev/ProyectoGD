using System.Collections;
using UnityEngine;

public class EnemySeg : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform jugador;

    [Header("Configuración de Persecución")]
    [SerializeField] private float velocidad = 3.5f;
    [SerializeField] private float alturaSobreJugador = 4.0f;
    [SerializeField] private float distanciaMinima = 0.2f;

    [Header("Spawn de Piedras")]
    [SerializeField] private GameObject prefabPiedra;
    [SerializeField] private Transform puntoSpawnPiedra;
    [SerializeField] private float intervaloSpawn = 1.5f;

    [Header("Disparo de Proyectil")]
    [SerializeField] private float velocidadProyectil = 8f;

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

        StartCoroutine(RutinaSpawnPiedras());
    }

    void Update()
    {
        if (jugador == null) return;

        SeguirJugador();
    }

    private void SeguirJugador()
    {
        Vector2 posicionObjetivo = new Vector2(jugador.position.x, jugador.position.y + alturaSobreJugador);

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
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(transform.position.x, posicionObjetivo.y),
                velocidad * Time.deltaTime
            );
        }
    }

    private IEnumerator RutinaSpawnPiedras()
    {
        yield return new WaitForSeconds(1.0f);

        while (true)
        {
            if (prefabPiedra != null && jugador != null)
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
}