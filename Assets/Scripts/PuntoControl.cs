using System.Collections;
using UnityEngine;

public class PuntoControl : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool estaActivo = true; 

    [Header("Efectos Visuales")]
    [SerializeField] private ParticleSystem particulasElectricas;

    
    void Awake()
    {
       
        PlayerPrefs.DeleteKey("UltimaPosX");
        PlayerPrefs.DeleteKey("UltimaPosY");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (!estaActivo) return;

        if (collision.CompareTag("Player"))
        {
            
            PlayerPrefs.SetFloat("UltimaPosX", transform.position.x);
            PlayerPrefs.SetFloat("UltimaPosY", transform.position.y);
            particulasElectricas?.Play(); 
            Debug.Log("Checkpoint activado en: " + transform.position);
        }
    }
}