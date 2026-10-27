using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

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

    [Header("Game Feel")]
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [Tooltip("Frena la caída momentáneamente al atacar en el aire")]
    [SerializeField] private bool suspenderEnElAire = true;

    [Header("Animación")]
    [SerializeField] private Animator anim;

    private InputAction meleeAccion;
    private float tiempoSiguienteAtaque = 0f;
    private readonly int ataqueHash = Animator.StringToHash("AtaqueMelee");
    
    private Rigidbody2D rb;

    private void Awake()
    {
        if (playerInput == null) playerInput = GetComponentInParent<PlayerInput>();
        if (anim == null) anim = GetComponentInParent<Animator>();
        if (puntoAtaque == null) puntoAtaque = transform;
        if (impulseSource == null) impulseSource = GetComponent<CinemachineImpulseSource>();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    private void Start()
    {
        if (playerInput != null && playerInput.actions != null)
        {
            meleeAccion = playerInput.actions.FindAction(nombreAccionMelee, false);
            if (meleeAccion != null)
            {
                meleeAccion.Enable();
                meleeAccion.performed += OnMeleePerformed;
            }
            else
            {
                Debug.LogWarning($"[AtaqueMelee] No se encontró la acción '{nombreAccionMelee}' en el PlayerInput.");
            }
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
        if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame && Time.time >= tiempoSiguienteAtaque)
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

        // Game Feel: Detiene la caída temporalmente si el personaje ataca en el aire
        if (suspenderEnElAire && rb != null && Mathf.Abs(rb.linearVelocity.y) > 0.1f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }

        Collider2D[] enemigosGolpeados = Physics2D.OverlapCircleAll(puntoAtaque.position, rangoAtaque, capaEnemigos);
        bool golpeoEnemigo = false;

        foreach (Collider2D col in enemigosGolpeados)
        {
            SistemaVida enemigo = col.GetComponent<SistemaVida>() ?? col.GetComponentInParent<SistemaVida>();

            if (enemigo != null)
            {
                enemigo.RecibirDano(danoAtaque);
                golpeoEnemigo = true;
            }
        }

        if (golpeoEnemigo)
        {
            if (impulseSource != null) impulseSource.GenerateImpulse();
            StartCoroutine(EfectoHitStop());
        }
    }

    private IEnumerator EfectoHitStop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.05f);
        Time.timeScale = 1f;
    }

    private void OnDrawGizmosSelected()
    {
        Transform origen = puntoAtaque != null ? puntoAtaque : transform;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origen.position, rangoAtaque);
    }
}