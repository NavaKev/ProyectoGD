using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Palanca : MonoBehaviour
{
    [Header("Puerta y Movimiento")]
    [SerializeField] private Transform puerta;
    [SerializeField] private float alturaApertura = 3f;
    [SerializeField] private float velocidadPuerta = 3f;

    [Header("Comportamiento")]
    [SerializeField] private bool cierreAutomatico = false;
    [SerializeField] private float tiempoParaCerrar = 3f;

    [Header("Animación de la Palanca")]
    [SerializeField] private Animator animPalanca;
    private readonly int activaHash = Animator.StringToHash("Activa");

    private bool abierta = false;
    private Vector3 posicionCerrada;
    private Vector3 posicionAbierta;
    private Coroutine corrutinaMovimiento;
    private Coroutine corrutinaAutoCierre;

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
        if (puerta != null)
        {
            posicionCerrada = puerta.position;
            posicionAbierta = posicionCerrada + (Vector3.up * alturaApertura);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Si tiene cierre automático y ya está abierta, no permite interacción manual hasta cerrarse
            if (cierreAutomatico && abierta) return;

            AlternarEstado(!abierta);
        }
    }

    public void AlternarEstado(bool nuevoEstado)
    {
        abierta = nuevoEstado;

        // 1. Activa la animación en el Animator de la palanca
        if (animPalanca != null)
        {
            animPalanca.SetBool(activaHash, abierta);
        }

        // 2. Desplazamiento suave de la puerta
        if (puerta != null)
        {
            if (corrutinaMovimiento != null) StopCoroutine(corrutinaMovimiento);
            Vector3 destino = abierta ? posicionAbierta : posicionCerrada;
            corrutinaMovimiento = StartCoroutine(MoverPuerta(destino));
        }

        // 3. Gestión de cierre automático
        if (cierreAutomatico && abierta)
        {
            if (corrutinaAutoCierre != null) StopCoroutine(corrutinaAutoCierre);
            corrutinaAutoCierre = StartCoroutine(RutinaAutoCierre());
        }
    }

    private IEnumerator MoverPuerta(Vector3 destino)
    {
        while (Vector3.Distance(puerta.position, destino) > 0.01f)
        {
            puerta.position = Vector3.MoveTowards(puerta.position, destino, velocidadPuerta * Time.deltaTime);
            yield return null;
        }
        puerta.position = destino;
    }

    private IEnumerator RutinaAutoCierre()
    {
        yield return new WaitForSeconds(tiempoParaCerrar);
        AlternarEstado(false); // Cierra la puerta y regresa la palanca a su animación inicial
    }
}