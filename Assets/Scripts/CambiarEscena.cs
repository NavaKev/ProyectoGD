using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    [SerializeField] private string nombreMenuPrincipal = "MenuPrincipal";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica que el jugador tenga el Tag "Player"
        if (collision.CompareTag("Player"))
        {
            // Imprime un mensaje en la Consola para confirmar que tocó el Trigger
            Debug.Log("Jugador tocó el objeto. Guardando PlayerPrefs...");

            PlayerPrefs.SetInt("AbrirCreditos", 1);
            PlayerPrefs.Save();

            SceneManager.LoadScene(nombreMenuPrincipal);
        }
    }
}