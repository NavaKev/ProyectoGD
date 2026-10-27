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

    [Header("Muros de la Arena")]
    
    [SerializeField] private GameObject[] muros;

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