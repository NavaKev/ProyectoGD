using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorEscena : MonoBehaviour
{
    [Tooltip("Nombre de la escena principal a cargar")]
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    [Header("Referencias UI")]
    [SerializeField] private Creditos scriptCreditos; // Asignamos la referencia al script Creditos

    void Start()
    {
        // Leemos el valor guardado
        int abrirCreditos = PlayerPrefs.GetInt("AbrirCreditos", 0);
        Debug.Log("Valor detectado en MenuPrincipal: " + abrirCreditos);

        if (abrirCreditos == 1)
        {
            // Limpiamos la clave para que no se vuelva a abrir al entrar normal
            PlayerPrefs.SetInt("AbrirCreditos", 0);
            PlayerPrefs.Save();

            // Llamamos a la función de Créditos
            MostrarCreditos();
        }
    }

    public void MostrarCreditos()
    {
        // Si no asignamos el script individualmente, lo busca en el mismo GameObject
        if (scriptCreditos == null)
        {
            scriptCreditos = GetComponent<Creditos>();
        }

        if (scriptCreditos != null)
        {
            // Ejecutamos la misma función que usa tu botón Créditos en el menú
            scriptCreditos.MostrarCreditos(); // Ajusta el nombre si tu método se llama distinto (ej. AbrirCreditos o Mostrar)
        }
        else
        {
            Debug.LogWarning("No se encontró el script Creditos adjunto.");
        }
    }

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

        if (Application.CanStreamedLevelBeLoaded(nombreEscena))
        {
            Debug.Log($"Cargando escena: {nombreEscena}");
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogError($"No se puede cargar '{nombreEscena}'. Verifica que esté agregada en File > Build Settings.");
        }
    }
}