using System.Collections;
using UnityEngine;

public class SpawnLimitado : MonoBehaviour
{
    [SerializeField] private GameObject prefabPiedra;

    [SerializeField] private Transform puntoSpawn;

    [Header("Generación de piedras")]
    [SerializeField] private int totalPiedras = 5;
    [SerializeField] private float intervaloSpawn = 1f;
    [SerializeField] private bool generarAlIniciar = true;

    void Start()
    {
        if (puntoSpawn == null)
        {
            puntoSpawn = transform;
        }

        if (generarAlIniciar)
        {
            IniciarSpawn();
        }
    }

    public void IniciarSpawn()
    {
        StartCoroutine(RutinaSpawnPiedras());
    }

    private IEnumerator RutinaSpawnPiedras()
    {
        if (prefabPiedra == null)
        {
            Debug.LogError($"[GeneradorPiedras] Falta asignar el prefab de la piedra en {gameObject.name}.", this);
            yield break;
        }

        for (int i = 0; i < totalPiedras; i++)
        {
            // Instancia la piedra en la posición y rotación del punto de spawn
            Instantiate(prefabPiedra, puntoSpawn.position, puntoSpawn.rotation);

            // Espera 0.5 segundos antes de soltar la siguiente (evita esperar después de la última)
            if (i < totalPiedras - 1)
            {
                yield return new WaitForSeconds(intervaloSpawn);
            }
        }

        // El bucle termina automáticamente tras la 5ta piedra y no genera más
    }
}