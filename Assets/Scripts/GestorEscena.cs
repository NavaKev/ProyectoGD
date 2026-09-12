using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorEscena : MonoBehaviour
{
    [Tooltip("Nombre de la escena principal a cargar")]
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    public void CargarNivelUno()
    {
        CargarEscena(nombreEscenaPrincipal);
    }

    public void CargarEscena(string nombreEscena)
    {
        if (string.IsNullOrEmpty(nombreEscena))
        {
            Debug.LogError("El nombre de la escena está vacío.");
            return;
        }

        // Verifica si la escena existe en la lista de Build Settings
        if (Application.CanStreamedLevelBeLoaded(nombreEscena))
        {
            Debug.Log($"Cargando escena: {nombreEscena}");
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogError($"No se puede cargar '{nombreEscena}'. Verifica que esté agregada en File > Build Settings y que el nombre coincida exactamente.");
        }
    }
}