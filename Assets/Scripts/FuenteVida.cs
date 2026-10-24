using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class FuenteVida : MonoBehaviour
{
    [Header("Efectos Visuales")]
    [SerializeField] private ParticleSystem particulasCuracion;
    
    private SistemaCuracion sistemaCuracionEnZona;

    void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true; 

        if (particulasCuracion == null) particulasCuracion = GetComponentInChildren<ParticleSystem>();
        if (particulasCuracion != null) particulasCuracion.Stop();
    }

    void Update()
    {
        if (sistemaCuracionEnZona != null && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            sistemaCuracionEnZona.RecargarViales();

            if (particulasCuracion != null)
            {
                particulasCuracion.Play();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SistemaCuracion curacion = collision.GetComponent<SistemaCuracion>() ?? collision.GetComponentInParent<SistemaCuracion>();
        if (curacion != null)
        {
            sistemaCuracionEnZona = curacion;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        SistemaCuracion curacion = collision.GetComponent<SistemaCuracion>() ?? collision.GetComponentInParent<SistemaCuracion>();
        if (curacion != null)
        {
            sistemaCuracionEnZona = null;
        }
    }
}