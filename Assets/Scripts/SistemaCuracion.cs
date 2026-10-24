using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(VidaJugador))]
public class SistemaCuracion : MonoBehaviour
{
    [Header("Configuración de Inyecciones")]
    [SerializeField] private int vialesMaximos = 3;
    [SerializeField] private int curacionPorVial = 15;
    private int vialesActuales;

    [Header("Animación UI")]
    [Tooltip("Arrastra aquí Jeringa1, Jeringa2 y Jeringa3 desde el Canvas")]
    [SerializeField] private Animator[] animInyecciones;

    private VidaJugador vidaJugador;

    private void Awake()
    {
        vidaJugador = GetComponent<VidaJugador>();
        vialesActuales = vialesMaximos;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            UsarVial();
        }
    }

    private void UsarVial()
    {
        if (vialesActuales > 0 && vidaJugador != null && !vidaJugador.TieneVidaMaxima)
        {
            // Ubica el índice de la jeringa actual (si tienes 3, el índice es 2)
            int indiceVial = vialesActuales - 1;

            vialesActuales--;
            vidaJugador.Curar(curacionPorVial);

            // Activa la animación de vaciado solo en esa jeringa
            if (indiceVial < animInyecciones.Length && animInyecciones[indiceVial] != null)
            {
                animInyecciones[indiceVial].SetTrigger("Usar");
            }
        }
    }

    public void RecargarViales()
    {
        // Reproduce la animación de llenado solo en las jeringas que estén vacías
        for (int i = vialesActuales; i < vialesMaximos; i++)
        {
            if (i < animInyecciones.Length && animInyecciones[i] != null)
            {
                animInyecciones[i].SetTrigger("Recargar");
            }
        }

        vialesActuales = vialesMaximos;
    }
}