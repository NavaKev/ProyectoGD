using UnityEngine;
using UnityEngine.InputSystem;

public class AtaqueMelee : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    [SerializeField] private int danoAtaque = 2;
    [SerializeField] private float rangoAtaque = 0.8f;
    [SerializeField] private float tiempoEntreAtaques = 0.4f;
    [SerializeField] private LayerMask capaEnemigos;

    [Header("Punto de Impacto")]
    [SerializeField] private Transform puntoAtaque;

    [Header("Input System")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private string nombreAccionMelee = "AtaqueMelee";

    [Header("Animación")]
    [SerializeField] private Animator anim;

    private InputAction meleeAccion;
    private float tiempoSiguienteAtaque = 0f;
    private readonly int ataqueHash = Animator.StringToHash("AtaqueMelee");

    private void Awake()
    {
        if (playerInput == null) playerInput = GetComponentInParent<PlayerInput>();
        if (anim == null) anim = GetComponentInParent<Animator>();
        if (puntoAtaque == null) puntoAtaque = transform;

        if (playerInput != null && playerInput.actions != null)
        {
            meleeAccion = playerInput.actions.FindAction(nombreAccionMelee, false);
        }
    }

    private void OnEnable()
    {
        if (meleeAccion != null)
        {
            meleeAccion.Enable();
            meleeAccion.performed += OnMeleePerformed;
        }
    }

    private void OnDisable()
    {
        if (meleeAccion != null)
        {
            meleeAccion.performed -= OnMeleePerformed;
            meleeAccion.Disable();
        }
    }

    private void Update()
    {
        
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame && Time.time >= tiempoSiguienteAtaque)
        {
            EjecutarAtaque();
            tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques;
        }
    }

    private void OnMeleePerformed(InputAction.CallbackContext context)
    {
        if (Time.time >= tiempoSiguienteAtaque)
        {
            EjecutarAtaque();
            tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques;
        }
    }

    private void EjecutarAtaque()
    {
        if (anim != null)
        {
            anim.SetTrigger(ataqueHash);
        }

        // Detecta todos los colliders dentro del área de corte
        Collider2D[] enemigosGolpeados = Physics2D.OverlapCircleAll(puntoAtaque.position, rangoAtaque, capaEnemigos);

        foreach (Collider2D col in enemigosGolpeados)
        {
            
            SistemaVida enemigo = col.GetComponent<SistemaVida>() ?? col.GetComponentInParent<SistemaVida>();

            if (enemigo != null)
            {
                enemigo.RecibirDano(danoAtaque);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Transform origen = puntoAtaque != null ? puntoAtaque : transform;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origen.position, rangoAtaque);
    }
}