using UnityEngine;

public class VidaJugador : MonoBehaviour
{

    private int vidaMaxima = 50;
    private int vidaActual;
    
    
    void Start()
    {
        vidaActual = vidaMaxima;
    }

    // Curación del jugador
    public void Curar(int cantidad)
    {
    vidaActual = Mathf.Min(vidaActual + cantidad, vidaMaxima);
    Debug.Log("Vida actual: " + vidaActual);
    }

    public bool TieneVidaMaxima => vidaActual >= vidaMaxima;

    public void recibirDano(int cantidad)
    {
        vidaActual -= cantidad;
        Debug.Log("Vida actual: " + vidaActual);

        if (vidaActual <= 0)
        {
            morir();
        }
    }


    public void morir()
    {
        this.gameObject.SetActive(false);
    }
}
