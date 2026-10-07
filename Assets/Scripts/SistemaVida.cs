using UnityEngine;
using UnityEngine.Events;

public class SistemaVida : MonoBehaviour
{
    [Header("Configuración de Salud")]
    [SerializeField] private float vidaMaxima = 100f;
    private float vidaActual;

    [Header("Eventos")]
    public UnityEvent<float, float> OnVidaCambiada;
    public UnityEvent OnMuerte;

    public float VidaActual => vidaActual;
    public float VidaMaxima => vidaMaxima;

    private void Awake()
    {
        vidaActual = vidaMaxima;
    }

    private void Start()
    {
        OnVidaCambiada?.Invoke(vidaActual, vidaMaxima);
    }

    public void RecibirDano(float cantidad)
    {
        if (vidaActual <= 0) return;

        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        OnVidaCambiada?.Invoke(vidaActual, vidaMaxima);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    public void Curar(float cantidad)
    {
        if (vidaActual <= 0) return;

        vidaActual += cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        OnVidaCambiada?.Invoke(vidaActual, vidaMaxima);
    }

    private void Morir()
    {
        OnMuerte?.Invoke();
        Destroy(gameObject);
    }
}