using System.Collections;
using UnityEngine;

public class PalancaPlataformasSecuencia : MonoBehaviour
{
    [Header("Secuencia de Plataformas")]
    [Tooltip("Orden en el que se irán activando las plataformas")]
    [SerializeField] private GameObject[] plataformas;

    [Tooltip("Tiempo en segundos que permanece activa cada plataforma")]
    [SerializeField] private float intervaloSegundos = 3f;

    [Header("Comportamiento")]
    [Tooltip("Si es true, el ciclo se repite infinitamente tras tocar la palanca. Si es false, recorre la lista una vez y termina.")]
    [SerializeField] private bool cicloInfinito = false;

    [Header("Feedback Visual")]
    [SerializeField] private Color colorActivo = Color.green;
    [SerializeField] private Color colorInactivo = Color.red;

    private SpriteRenderer spriteRendererPalanca;
    private Coroutine rutinaSecuencia;
    private bool secuenciaEnCurso = false;

    private void Awake()
    {
        spriteRendererPalanca = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Apaga todas las plataformas al inicio
        ApagarTodasLasPlataformas();

        if (spriteRendererPalanca != null)
        {
            spriteRendererPalanca.color = colorInactivo;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Evita reiniciar o duplicar la corrutina si ya está corriendo
            if (!secuenciaEnCurso)
            {
                rutinaSecuencia = StartCoroutine(SecuenciaPlataformas());
            }
        }
    }

    private IEnumerator SecuenciaPlataformas()
    {
        secuenciaEnCurso = true;

        if (spriteRendererPalanca != null)
        {
            spriteRendererPalanca.color = colorActivo;
        }
        transform.rotation = Quaternion.Euler(0, 0, -45f);

        do
        {
            for (int i = 0; i < plataformas.Length; i++)
            {
                // Apaga todas y activa únicamente la plataforma del turno actual
                ActivarSoloPlataforma(i);

                // Espera los 3 segundos antes de pasar a la siguiente
                yield return new WaitForSeconds(intervaloSegundos);
            }
        } 
        while (cicloInfinito);

        // Al terminar el recorrido (si no es infinito), apaga todo y restablece la palanca
        ApagarTodasLasPlataformas();
        
        if (spriteRendererPalanca != null)
        {
            spriteRendererPalanca.color = colorInactivo;
        }
        transform.rotation = Quaternion.identity;

        secuenciaEnCurso = false;
        rutinaSecuencia = null;
    }

    private void ActivarSoloPlataforma(int indiceActivo)
    {
        for (int i = 0; i < plataformas.Length; i++)
        {
            if (plataformas[i] != null)
            {
                plataformas[i].SetActive(i == indiceActivo);
            }
        }
    }

    private void ApagarTodasLasPlataformas()
    {
        foreach (GameObject obj in plataformas)
        {
            if (obj != null) obj.SetActive(false);
        }
    }
}