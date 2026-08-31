using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PalancaPlataformas: MonoBehaviour
{
    [Header("Secuencia de Plataformas")]
    [SerializeField] private GameObject[] plataformas;
    [SerializeField] private float intervaloSegundos = 2f;

    [Header("Comportamiento")]

    [SerializeField] private bool cicloInfinito = false;

    [Header("Animación de la Palanca")]

    [SerializeField] private Animator animPalanca;
    private readonly int activaHash = Animator.StringToHash("Activa");

    private Coroutine rutinaSecuencia;
    private bool secuenciaEnCurso = false;

    private void Awake()
    {
        if (animPalanca == null)
        {
            animPalanca = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        }

        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Start()
    {
        ApagarTodasLasPlataformas();

        if (animPalanca != null)
        {
            animPalanca.SetBool(activaHash, false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!secuenciaEnCurso)
            {
                rutinaSecuencia = StartCoroutine(SecuenciaIdaYVuelta());
            }
        }
    }

    private IEnumerator SecuenciaIdaYVuelta()
    {
        secuenciaEnCurso = true;

        // Activa la palanca en el Animator
        if (animPalanca != null)
        {
            animPalanca.SetBool(activaHash, true);
        }

        do
        {
            // 1. Recorrido de IDA (del primer elemento al último)
            for (int i = 0; i < plataformas.Length; i++)
            {
                ActivarSoloPlataforma(i);
                yield return new WaitForSeconds(intervaloSegundos);
            }

            // 2. Recorrido de VUELTA (desde el penúltimo hasta el segundo para no repetir extremos)
            for (int i = plataformas.Length - 2; i > 0; i--)
            {
                ActivarSoloPlataforma(i);
                yield return new WaitForSeconds(intervaloSegundos);
            }

        } while (cicloInfinito);

        // Al finalizar la vuelta completa (si no es infinito), apaga todo y restablece la palanca
        ApagarTodasLasPlataformas();

        if (animPalanca != null)
        {
            animPalanca.SetBool(activaHash, false);
        }

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
        if (plataformas == null) return;

        foreach (GameObject obj in plataformas)
        {
            if (obj != null) obj.SetActive(false);
        }
    }
}