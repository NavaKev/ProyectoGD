using UnityEngine;

public class DetectorDano : MonoBehaviour
{
    private VidaJugador vdJugador;

    private float siguienteDanoVeneno = 0f;
    private float siguienteDanoElectrico = 0f;
    private bool estabaElectricoActivo = false;

    void Awake()
    {
        vdJugador = GetComponentInParent<VidaJugador>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (vdJugador == null) return;

        // 1. Objeto Dañino simple
        ObjetoDanino objDanino = collision.GetComponent<ObjetoDanino>() ?? collision.GetComponentInParent<ObjetoDanino>();
        if (objDanino != null)
        {
            vdJugador.recibirDano(objDanino.Dano);
        }

        // 2. Zona Dañina continua (Veneno)
        ZonaDanina znDanina = collision.GetComponent<ZonaDanina>() ?? collision.GetComponentInParent<ZonaDanina>();
        if (znDanina != null)
        {
            vdJugador.recibirDano(znDanina.Dano);
            siguienteDanoVeneno = Time.time + znDanina.IntervaloDano;
        }

        // 3. Charco Eléctrico
        CharcoElectrico charco = collision.GetComponent<CharcoElectrico>() ?? collision.GetComponentInParent<CharcoElectrico>();
        if (charco != null && charco.EstaActivo)
        {
            vdJugador.recibirDano(charco.DanoEntrada); // 2 de daño de entrada
            siguienteDanoElectrico = Time.time + charco.IntervaloDano; // Espera 0.5s para empezar el daño continuo
            estabaElectricoActivo = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (vdJugador == null) return;

        // Zona Dañina continua (Veneno)
        ZonaDanina znDanina = collision.GetComponent<ZonaDanina>() ?? collision.GetComponentInParent<ZonaDanina>();
        if (znDanina != null && Time.time >= siguienteDanoVeneno)
        {
            vdJugador.recibirDano(znDanina.Dano);
            siguienteDanoVeneno = Time.time + znDanina.IntervaloDano;
        }

        // Charco Eléctrico continuo
        CharcoElectrico charco = collision.GetComponent<CharcoElectrico>() ?? collision.GetComponentInParent<CharcoElectrico>();
        if (charco != null)
        {
            if (charco.EstaActivo)
            {
                // Si el charco se acaba de activar mientras el jugador ya estaba adentro:
                if (!estabaElectricoActivo)
                {
                    vdJugador.recibirDano(charco.DanoEntrada); // Aplica el daño de entrada inicial (2)
                    siguienteDanoElectrico = Time.time + charco.IntervaloDano;
                    estabaElectricoActivo = true;
                }
                // Daño continuo cada 0.5s (5 de daño de zona)
                else if (Time.time >= siguienteDanoElectrico)
                {
                    vdJugador.recibirDano(charco.DanoZona);
                    siguienteDanoElectrico = Time.time + charco.IntervaloDano;
                }
            }
            else
            {
                // Si el charco se apaga mientras el jugador sigue adentro, reseteamos la bandera
                estabaElectricoActivo = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Al salir del charco, reseteamos el estado
        CharcoElectrico charco = collision.GetComponent<CharcoElectrico>() ?? collision.GetComponentInParent<CharcoElectrico>();
        if (charco != null)
        {
            estabaElectricoActivo = false;
        }
    }
}