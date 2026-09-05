using UnityEngine;

public class VidaEnemigo : MonoBehaviour
{
    [SerializeField] private int vidaMaxima = 5;
    [SerializeField] private int vidaActual;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vidaActual = vidaMaxima;
    }

    public void RecibirDano(int dano)
    {
        vidaActual -= dano;
        if (vidaActual <= 0)
        {
            Muerte();
        }
    }

    public void Muerte()
    {
        this.gameObject.SetActive(false);
    }
}
