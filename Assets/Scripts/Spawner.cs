using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject objeto; // Objeto a spawnear
    public float tiempoSpawn = 1.5f; // Intervalo de tiempo entre spawns 
    public float tiempotranscurrido = 0f; // Tiempo transcurrido desde el último spawn

    public void Start()
    {
        tiempotranscurrido = Time.time + tiempoSpawn; // Inicializa el tiempo transcurrido para el primer spawn
    }


    void Update()
    {
        
        if(Time.time >= tiempotranscurrido + tiempoSpawn)
        {
            tiempotranscurrido = Time.time + tiempoSpawn;
            Instantiate(objeto, transform.position, transform.rotation);

        }

    }
}
