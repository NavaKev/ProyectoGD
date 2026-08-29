using UnityEngine;

public class Piedra : MonoBehaviour
{

    public float tiempoDestruccion = 5f; // Tiempo en segundos antes de destruir la piedra
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, tiempoDestruccion);
    }


}
