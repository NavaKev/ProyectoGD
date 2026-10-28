using System.Collections;
using UnityEngine;

[System.Serializable]
public class BloquePlataformas
{
    public GameObject[] plataformas;
}

[RequireComponent(typeof(Collider2D))]
public class ControladorArenaJefe : MonoBehaviour
{
    [Header("Configuración del Jefe")]
    [SerializeField] private GameObject objetoJefe;

    [Header("Cámara de la Arena")]
    [Tooltip("Arrastra aquí una Cinemachine Camera configurada para abarcar solo la arena.")]
    [SerializeField] private GameObject camaraBoss;

    [Header("Muros de la Arena")]
    [SerializeField] private GameObject[] muros;

    [Header("Canvas de la Arena")]
    [SerializeField] private GameObject canvasArena;

    [Header("Secuencia de Plataformas (Por Bloques)")]
    [SerializeField] private BloquePlataformas[] secuenciaDeBloques;
    [SerializeField] private float tiempoPorBloque = 2.5f;

    private bool batallaActiva = false;
    private Coroutine rutinaPlataformas;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true; 
    }

    private void Start()
    {
        // Estado inicial
        ActivarMuros(false);
        ApagarTodasLasPlataformas();

        if (objetoJefe != null) 
        {
            objetoJefe.SetActive(false); 
        }

        // Asegurarnos de que la cámara del boss esté apagada al inicio
        if (camaraBoss != null)
        {
            camaraBoss.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detecta al jugador
        if (!batallaActiva && (collision.CompareTag("Player") || collision.GetComponent<PlayerMov>() != null))
        {
            IniciarBatalla();
        }
    }

    private void IniciarBatalla()
    {
        batallaActiva = true;

        if (objetoJefe != null) objetoJefe.SetActive(true);
        ActivarMuros(true);

        // 1. Enciende la cámara del Boss para bloquear la vista
        if (camaraBoss != null) camaraBoss.SetActive(true);

        if (canvasArena != null) canvasArena.SetActive(true);

        // Inicia el bucle de bloques
        if (rutinaPlataformas != null) StopCoroutine(rutinaPlataformas);
        rutinaPlataformas = StartCoroutine(RutinaSecuenciaBloques());
    }

    private IEnumerator RutinaSecuenciaBloques()
    {
        while (batallaActiva)
        {
            // Si el jefe es derrotado, rompemos el ciclo
            if (objetoJefe == null || !objetoJefe.activeInHierarchy)
            {
                FinalizarBatalla();
                yield break;
            }

            // Recorre los bloques (ej. Bloque 0, Bloque 1, Bloque 2...)
            for (int i = 0; i < secuenciaDeBloques.Length; i++)
            {
                if (!batallaActiva || objetoJefe == null || !objetoJefe.activeInHierarchy) break;

                // Enciende todas las plataformas del bloque actual y apaga el resto
                ActivarSoloBloque(i);
                
                yield return new WaitForSeconds(tiempoPorBloque);
            }
        }
    }

    private void FinalizarBatalla()
    {
        batallaActiva = false;

        if (rutinaPlataformas != null)
        {
            StopCoroutine(rutinaPlataformas);
            rutinaPlataformas = null;
        }

        ActivarMuros(false);
        ApagarTodasLasPlataformas();
        
        // 2. Apaga la cámara del Boss para que la cámara del jugador vuelva a seguirlo
        if (camaraBoss != null) camaraBoss.SetActive(false);
        
        GetComponent<Collider2D>().enabled = false;
    }

    private void ActivarMuros(bool estado)
    {
        foreach (GameObject muro in muros)
        {
            if (muro != null) muro.SetActive(estado);
        }
    }

    private void ActivarSoloBloque(int indiceBloqueActivo)
    {
        ApagarTodasLasPlataformas();

        if (indiceBloqueActivo >= 0 && indiceBloqueActivo < secuenciaDeBloques.Length)
        {
            foreach (GameObject plat in secuenciaDeBloques[indiceBloqueActivo].plataformas)
            {
                if (plat != null) plat.SetActive(true);
            }
        }
    }

    private void ApagarTodasLasPlataformas()
    {
        foreach (BloquePlataformas bloque in secuenciaDeBloques)
        {
            if (bloque != null && bloque.plataformas != null)
            {
                foreach (GameObject plat in bloque.plataformas)
                {
                    if (plat != null) plat.SetActive(false);
                }
            }
        }
    }
}