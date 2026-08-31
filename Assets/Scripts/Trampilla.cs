using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Trampilla : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Animator animTrampilla;
    [SerializeField] private Collider2D colisionadorFisico;

    private readonly int abiertaHash = Animator.StringToHash("Abierta");
    private bool estaAbierta = false;

    private void Awake()
    {
        if (animTrampilla == null)
        {
            animTrampilla = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        }

        if (colisionadorFisico == null)
        {
            colisionadorFisico = GetComponent<Collider2D>();
        }
    }

    public void SetAbierta(bool abrir)
    {
        estaAbierta = abrir;

        // 1. Activa la animación visual de la compuerta abriéndose
        if (animTrampilla != null)
        {
            animTrampilla.SetBool(abiertaHash, estaAbierta);
        }

        // 2. Desactiva la colisión física para que el personaje caiga al vacío
        if (colisionadorFisico != null)
        {
            colisionadorFisico.enabled = !estaAbierta;
        }
    }
}