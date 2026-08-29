using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CharcoElectrico : MonoBehaviour
{
    // Daño 
    [SerializeField] private int danoEntrada = 2;
    [SerializeField] private int danoZona = 4;
    [SerializeField] private float intervaloDano = 0.5f;

    //Configuración de ciclo eléctrico
    [SerializeField] private float tiempoCiclo = 5f;
    [SerializeField] private Color colorDesactivado = Color.white;
    [SerializeField] private Color colorActivado = Color.yellow;

    //SpriteRenderer para cambiar el color del charco eléctrico
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Collider2D col2D;
    private bool estaActivo = false;

    // Propiedades públicas para que el DetectorDano las consulte
    public bool EstaActivo => estaActivo;
    public int DanoEntrada => danoEntrada;
    public int DanoZona => danoZona;
    public float IntervaloDano => intervaloDano;

    void Awake()
    {
        
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();           
        }

        col2D = GetComponent<Collider2D>();
        col2D.isTrigger = true;

        if (spriteRenderer == null)
        {
            Debug.LogWarning($"[CharcoElectrico] No se encontró ningún SpriteRenderer en los hijos de {gameObject.name}.", this);
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
            if (spriteRenderer != null)
            {
                spriteRenderer.color = colorDesactivado;
            }
            yield return new WaitForSeconds(tiempoCiclo);

            // Estado 2: Activado
            estaActivo = true;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = colorActivado;
            }
            yield return new WaitForSeconds(tiempoCiclo);
        }
    }
}