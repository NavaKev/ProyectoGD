using UnityEngine;

public class VidaEnemigo : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private int vidaMaxima = 5;
    [SerializeField] private int vidaActual;

    void Start()
    {
        vidaActual = vidaMaxima;
    }

    // Método que invoca tanto DetectorDano como AtaqueMelee
    public void RecibirDano(int dano)
    {
        vidaActual -= dano;
        Debug.Log($"Recibió {dano} de daño. Vida restante: {vidaActual}/{vidaMaxima}");

        if (vidaActual <= 0)
        {
            Muerte();
        }
    }

    public void Muerte()
    {
        gameObject.SetActive(false);
    }
}