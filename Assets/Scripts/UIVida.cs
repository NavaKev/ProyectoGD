using UnityEngine;
using UnityEngine.UI;

public class UIVida : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Image imagenRelleno;
    [SerializeField] private SistemaVida sistemaVida;

    [Header("Opciones de Orientación")]
    [SerializeField] private bool orientarHaciaCamara = true;

    private Camera camaraPrincipal;

    private void Awake()
    {
        camaraPrincipal = Camera.main;

        
        if (sistemaVida == null)
        {
            sistemaVida = GetComponentInParent<SistemaVida>();
        }
    }

    private void OnEnable()
    {
        if (sistemaVida != null)
        {
            sistemaVida.OnVidaCambiada.AddListener(ActualizarBarra);
        }
    }

    private void OnDisable()
    {
        if (sistemaVida != null)
        {
            sistemaVida.OnVidaCambiada.RemoveListener(ActualizarBarra);
        }
    }

    private void LateUpdate()
    {
        
        if (orientarHaciaCamara && camaraPrincipal != null)
        {
            transform.rotation = camaraPrincipal.transform.rotation;
        }
    }

    public void ActualizarBarra(float vidaActual, float vidaMaxima)
    {
        if (imagenRelleno != null && vidaMaxima > 0)
        {
            imagenRelleno.fillAmount = vidaActual / vidaMaxima;
        }
    }
}