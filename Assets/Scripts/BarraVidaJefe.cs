using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarraVidaJefe : MonoBehaviour
{
    [Header("Referencias del Jefe")]
    [Tooltip("Arrastra aquí el GameObject de tu Jefe Cuervo")]
    // CAMBIO AQUÍ: Ahora busca tu script SistemaVida en lugar de VidaEnemigo
    [SerializeField] private SistemaVida scriptVidaJefe;

    [Header("Elementos de UI")]
    [SerializeField] private GameObject panelBarraJefe;
    [SerializeField] private Image imagenRelleno;
    [SerializeField] private TextMeshProUGUI textoNombre;

    [Header("Configuración")]
    [SerializeField] private string nombreJefe = "El Cuervo de Éter";

    private void Start()
    {
        if (textoNombre != null) textoNombre.text = nombreJefe;
        
        if (panelBarraJefe != null) panelBarraJefe.SetActive(false);
    }

    private void Update()
    {
        if (scriptVidaJefe == null || !scriptVidaJefe.gameObject.activeInHierarchy)
        {
            if (panelBarraJefe != null && panelBarraJefe.activeSelf)
            {
                panelBarraJefe.SetActive(false);
            }
            return;
        }

        if (panelBarraJefe != null && !panelBarraJefe.activeSelf)
        {
            panelBarraJefe.SetActive(true);
        }

        if (imagenRelleno != null && scriptVidaJefe.VidaMaxima > 0)
        {
            imagenRelleno.fillAmount = (float)scriptVidaJefe.VidaActual / scriptVidaJefe.VidaMaxima;
        }
    }
}