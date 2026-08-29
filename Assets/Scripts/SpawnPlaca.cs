using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlacaPresionPiedra : MonoBehaviour
{
    [Header("Spawn de Piedra")]
    [SerializeField] private GameObject prefabPiedra;
    [SerializeField] private Transform puntoSpawn;

    [SerializeField] private SpriteRenderer spritePlaca;
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private Color colorPresionada = Color.gray;

    private Collider2D col2D;
    private bool estaPresionada = false;

    void Awake()
    {
        col2D = GetComponent<Collider2D>();
        col2D.isTrigger = true; 

        if (spritePlaca == null)
        {
            spritePlaca = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detecta si lo que pisó la placa es el jugador
        if (EsJugador(collision) && !estaPresionada)
        {
            estaPresionada = true;
            GenerarPiedra();

            if (spritePlaca != null)
            {
                spritePlaca.color = colorPresionada;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Cuando el jugador se baja de la placa, queda lista para volver a activarse
        if (EsJugador(collision))
        {
            estaPresionada = false;

            if (spritePlaca != null)
            {
                spritePlaca.color = colorNormal;
            }
        }
    }

    private void GenerarPiedra()
    {
        if (prefabPiedra == null)
        {
            Debug.LogWarning($"[PlacaPresion] No hay prefab de piedra asignado en {gameObject.name}.", this);
            return;
        }

        Vector3 posicion = puntoSpawn != null ? puntoSpawn.position : transform.position;
        Quaternion rotacion = puntoSpawn != null ? puntoSpawn.rotation : Quaternion.identity;

        Instantiate(prefabPiedra, posicion, rotacion);
    }

    private bool EsJugador(Collider2D col)
    {
        // Verifica por tag "Player" o por si tiene componentes de vida / detección
        return col.CompareTag("Player") || 
               col.GetComponent<VidaJugador>() != null || 
               col.GetComponentInParent<VidaJugador>() != null ||
               col.GetComponent<DetectorDano>() != null;
    }
}