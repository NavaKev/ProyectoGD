using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CartelTrampaRoca : MonoBehaviour
{
    [Header("Spawn de Piedra")]
    [SerializeField] private GameObject prefabPiedra;

    [SerializeField] private Transform puntoSpawn;

    [Header("Configuración")]
    [SerializeField] private bool activarUnaSolaVez = true;

    [SerializeField] private float retrasoCaida = 0f;

    private Collider2D col2D;
    private bool yaSeActivo = false;

    void Awake()
    {
        col2D = GetComponent<Collider2D>();
        col2D.isTrigger = true; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (yaSeActivo && activarUnaSolaVez) return;

        // Detecta si lo que atravesó el cartel es el jugador
        if (EsJugador(collision))
        {
            yaSeActivo = true;

            if (retrasoCaida > 0f)
            {
                Invoke(nameof(GenerarPiedra), retrasoCaida);
            }
            else
            {
                GenerarPiedra();
            }
        }
    }

    private void GenerarPiedra()
    {
        if (prefabPiedra == null)
        {
            Debug.LogWarning($"No hay prefab de piedra asignado en {gameObject.name}.", this);
            return;
        }

        Vector3 posicion = puntoSpawn != null ? puntoSpawn.position : transform.position;
        Quaternion rotacion = puntoSpawn != null ? puntoSpawn.rotation : Quaternion.identity;

        Instantiate(prefabPiedra, posicion, rotacion);
    }

    private bool EsJugador(Collider2D col)
    {
        return col.CompareTag("Player") || 
               col.GetComponent<VidaJugador>() != null || 
               col.GetComponentInParent<VidaJugador>() != null || 
               col.GetComponent<DetectorDano>() != null;
    }
}