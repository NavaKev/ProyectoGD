using UnityEngine;

public class DetectarDanoEnemy : MonoBehaviour
{
    private SistemaVida vidaEnemigo;
    private float siguienteDanoVeneno = 0f;
    private float siguienteDanoElectrico = 0f;
    private bool estabaElectricoActivo = false;

    void Awake()
    {
        
        vidaEnemigo = GetComponentInParent<SistemaVida>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (vidaEnemigo == null) return;

        // 1. Proyectil (Bala)
        if (collision.CompareTag("Bala"))
        {
            Bala bala = collision.GetComponent<Bala>() ?? collision.GetComponentInParent<Bala>();
            if (bala != null)
            {
                vidaEnemigo.RecibirDano(bala.Dano);
            }
            Destroy(collision.gameObject);
            return;
        }

        // 2. Trampas u Objetos Dañinos simples (Pinchos, piedras)
        ObjetoDanino objDanino = collision.GetComponent<ObjetoDanino>() ?? collision.GetComponentInParent<ObjetoDanino>();
        if (objDanino != null)
        {
            vidaEnemigo.RecibirDano(objDanino.Dano);
        }

        // 3. Charco de veneno
        ZonaDanina veneno = collision.GetComponent<ZonaDanina>() ?? collision.GetComponentInParent<ZonaDanina>();
        if (veneno != null)
        {
            vidaEnemigo.RecibirDano(veneno.Dano);
            siguienteDanoVeneno = Time.time + veneno.IntervaloDano;
        }

        // 4. Charco eléctrico
        CharcoElectrico charco = collision.GetComponent<CharcoElectrico>() ?? collision.GetComponentInParent<CharcoElectrico>();
        if (charco != null && charco.EstaActivo)
        {
            vidaEnemigo.RecibirDano(charco.DanoEntrada);
            siguienteDanoElectrico = Time.time + charco.IntervaloDano;
            estabaElectricoActivo = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (vidaEnemigo == null) return;

        // Veneno continuo
        ZonaDanina veneno = collision.GetComponent<ZonaDanina>() ?? collision.GetComponentInParent<ZonaDanina>();
        if (veneno != null && Time.time >= siguienteDanoVeneno)
        {
            vidaEnemigo.RecibirDano(veneno.Dano);
            siguienteDanoVeneno = Time.time + veneno.IntervaloDano;
        }

        // Electricidad continua
        CharcoElectrico charco = collision.GetComponent<CharcoElectrico>() ?? collision.GetComponentInParent<CharcoElectrico>();
        if (charco != null)
        {
            if (charco.EstaActivo)
            {
                if (!estabaElectricoActivo)
                {
                    vidaEnemigo.RecibirDano(charco.DanoEntrada);
                    siguienteDanoElectrico = Time.time + charco.IntervaloDano;
                    estabaElectricoActivo = true;
                }
                else if (Time.time >= siguienteDanoElectrico)
                {
                    vidaEnemigo.RecibirDano(charco.DanoZona);
                    siguienteDanoElectrico = Time.time + charco.IntervaloDano;
                }
            }
            else
            {
                estabaElectricoActivo = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CharcoElectrico charco = collision.GetComponent<CharcoElectrico>() ?? collision.GetComponentInParent<CharcoElectrico>();
        if (charco != null)
        {
            estabaElectricoActivo = false;
        }
    }
}