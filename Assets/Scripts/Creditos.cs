using UnityEngine;

public class Creditos : MonoBehaviour
{
    [Header("Imagen de Créditos")]
    [SerializeField] private GameObject panelCreditos;

    void Start()
    {
        
    }

    public void MostrarCreditos()
    {
        if (panelCreditos != null)
        {
            panelCreditos.SetActive(true);
        }
    }

    public void OcultarCreditos()
    {
        if (panelCreditos != null)
        {
            panelCreditos.SetActive(false);
        }
    }

    public void SalirDelJuego()
    {
        Debug.Log("Cerrando el juego...");
        Application.Quit();
    }
}