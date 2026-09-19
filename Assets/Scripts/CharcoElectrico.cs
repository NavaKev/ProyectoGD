using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CharcoElectrico : MonoBehaviour
{
    [Header("Configuración de Daño")]
    [SerializeField] private int danoEntrada = 2;
    [SerializeField] private int danoZona = 4;
    [SerializeField] private float intervaloDano = 0.5f;

    [Header("Configuración de Ciclo")]
    [SerializeField] private float tiempoCiclo = 5f;

    [Header("Efectos Visuales")]
    [SerializeField] private ParticleSystem particulasElectricas;

    private Collider2D col2D;
    private bool estaActivo = false;

    // Propiedades públicas para que el DetectorDano las consulte
    public bool EstaActivo => estaActivo;
    public int DanoEntrada => danoEntrada;
    public int DanoZona => danoZona;
    public float IntervaloDano => intervaloDano;

    [SerializeField] private AudioSource audioSource;

    void Awake()
    {
        col2D = GetComponent<Collider2D>();
        col2D.isTrigger = true;

        // Busca automáticamente el ParticleSystem en este objeto o en los hijos si no se asignó
        if (particulasElectricas == null)
        {
            particulasElectricas = GetComponentInChildren<ParticleSystem>();
        }

        // Asegura que inicie apagado
        if (particulasElectricas != null)
        {
            particulasElectricas.Stop();
        }
    }

    void Start()
    {
        StartCoroutine(CicloElectrico());
    }

    private IEnumerator CicloElectrico()
    {
        while (true)
        {
            // Estado 1: Desactivado
            estaActivo = false;
            if (particulasElectricas != null && particulasElectricas.isPlaying)
            {
                particulasElectricas.Stop();
                audioSource?.Stop();
            }
            yield return new WaitForSeconds(tiempoCiclo);

            // Estado 2: Activado
            estaActivo = true;
            if (particulasElectricas != null && !particulasElectricas.isPlaying)
            {
                particulasElectricas.Play();
                audioSource?.Play();
            }
            yield return new WaitForSeconds(tiempoCiclo);
        }
    }
}