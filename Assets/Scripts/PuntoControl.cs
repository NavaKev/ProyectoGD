using System.Collections;
using UnityEngine;
public class PuntoControl : MonoBehaviour

{

    [Header("Efectos Visuales")]
    [SerializeField] private ParticleSystem particulasElectricas;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerPrefs.DeleteKey("UltimaPosX");
        PlayerPrefs.DeleteKey("UltimaPosY");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            // Guardar la posición del jugador en PlayerPrefs
            PlayerPrefs.SetFloat("UltimaPosX", transform.position.x);
            PlayerPrefs.SetFloat("UltimaPosY", transform.position.y);
            particulasElectricas?.Play(); // Reproduce las partículas al activar el checkpoint
            Debug.Log("Checkpoint activado en: " + transform.position);
        }
    
    }


}
