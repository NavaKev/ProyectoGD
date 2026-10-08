using UnityEngine;

public class Creditos : MonoBehaviour
{
    [Header("Imagen de Créditos")]
    [SerializeField] private GameObject panelCreditos;

    void Start()
    {
        // No forzamos SetActive(false) aquí para no sobrescribir la orden de GestorEscena.
        // Asegúrate de dejar el 'PanelCreditos' desactivado desde el Editor en la Hierarchy si deseas que inicie oculto por defecto.
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