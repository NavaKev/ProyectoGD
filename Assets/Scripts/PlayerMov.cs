using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMov : MonoBehaviour
{
    [Header("Movimiento Horizontal")]
    [SerializeField] private float velocidadMaxima = 8f;
    [SerializeField] private float aceleracion = 50f;
    [SerializeField] private float desaceleracion = 40f;

    [Header("Salto y Doble Salto")]
    [SerializeField] private float fuerzaSalto = 12f;
    [SerializeField] private int saltosMaximos = 2;
    [SerializeField] private int saltosRestantes;
    [SerializeField] private float cooldownRecargaSuelo = 0.15f;
    private float tiempoUltimoSalto;

    [Header("Detección de Suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isGrounded;

    [Header("Estabilidad en Plataformas")]
    [SerializeField] private float coyoteTime = 0.1f;
    private float contadorCoyote = 0f;

    [Header("Referencias")]
    public Rigidbody2D rb;
    public PlayerInput pi;
    public Animator anim;
    public SpriteRenderer spriteRenderer;

    private InputAction moverAccion;
    private InputAction saltarAccion;
    private Vector2 inputMovimiento;
    private bool mirandoDerecha = true;

    // Hashes para optimizar el rendimiento del Animator
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int isGroundedHash = Animator.StringToHash("isGrounded");
    private readonly int verticalVelocityHash = Animator.StringToHash("verticalVelocity");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pi = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        moverAccion = pi.actions.FindAction("Mover");
        saltarAccion = pi.actions.FindAction("Saltar");
    }

    void OnEnable()
    {
        moverAccion?.Enable();

        if (saltarAccion != null)
        {
            saltarAccion.Enable();
            saltarAccion.performed += OnJumpPerformed;
        }
    }

    void OnDisable()
    {
        moverAccion?.Disable();

        if (saltarAccion != null)
        {
            saltarAccion.performed -= OnJumpPerformed;
            saltarAccion.Disable();
        }
    }

    void Update()
    {
        if (moverAccion != null)
        {
            inputMovimiento = moverAccion.ReadValue<Vector2>();
        }

        if (contadorCoyote > 0f)
        {
            contadorCoyote -= Time.deltaTime;
        }

        GestionarGiro();
        ActualizarAnimaciones();
    }

    private void FixedUpdate()
    {
        CheckGroundStatus();
        AplicarMovimientoHorizontal();
    }

    private void ActualizarAnimaciones()
    {
        if (anim == null) return;

        bool estadoSueloEstable = isGrounded || contadorCoyote > 0f;

        anim.SetFloat(speedHash, Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool(isGroundedHash, estadoSueloEstable);

        float velYAnimacion = estadoSueloEstable ? 0f : rb.linearVelocity.y;
        anim.SetFloat(verticalVelocityHash, velYAnimacion);
    }

    private void AplicarMovimientoHorizontal()
    {
        float targetVelocityX = inputMovimiento.x * velocidadMaxima;
        float velocidadActualX = rb.linearVelocity.x;
        float diferenciaVelocidad = targetVelocityX - velocidadActualX;

        float tasaAceleracion = (Mathf.Abs(targetVelocityX) > 0.01f) ? aceleracion : desaceleracion;
        float fuerzaHorizontal = diferenciaVelocidad * tasaAceleracion;

        rb.AddForce(new Vector2(fuerzaHorizontal, 0f), ForceMode2D.Force);
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        Saltar();
    }

    private void Saltar()
    {
        if (saltosRestantes > 0)
        {
            // 1. Desvincular de la plataforma inmediatamente
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }

            // 2. Forzar despegue vertical neto
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);

            saltosRestantes--;
            tiempoUltimoSalto = Time.time;
            isGrounded = false;
            contadorCoyote = 0f;

            if (anim != null)
            {
                anim.SetBool(isGroundedHash, false);
                anim.SetFloat(verticalVelocityHash, fuerzaSalto);
            }
        }
    }

    private void CheckGroundStatus()
    {
        // Durante los primeros 0.15s del salto, ignorar detección para permitir despegar de la plataforma ascendente
        if (Time.time < (tiempoUltimoSalto + cooldownRecargaSuelo))
        {
            isGrounded = false;
            return;
        }

        if (groundCheck != null)
        {
            bool tocandoFisico = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            if (tocandoFisico)
            {
                isGrounded = true;
                contadorCoyote = coyoteTime;
            }
            else
            {
                isGrounded = false;
            }
        }

        if (isGrounded || contadorCoyote > 0f)
        {
            saltosRestantes = saltosMaximos;
        }
    }

    private void GestionarGiro()
    {
        if (inputMovimiento.x > 0.1f && !mirandoDerecha)
        {
            Voltear();
        }
        else if (inputMovimiento.x < -0.1f && mirandoDerecha)
        {
            Voltear();
        }
    }

    private void Voltear()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = (isGrounded || contadorCoyote > 0f) ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}