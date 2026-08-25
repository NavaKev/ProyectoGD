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
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isGrounded;

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

        
        anim.SetFloat(speedHash, Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool(isGroundedHash, isGrounded);
        anim.SetFloat(verticalVelocityHash, rb.linearVelocity.y);
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
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);

            saltosRestantes--;
            tiempoUltimoSalto = Time.time;
            isGrounded = false;

            if (anim != null) anim.SetBool(isGroundedHash, false);
        }
    }

    private void CheckGroundStatus()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        if (isGrounded && Time.time > (tiempoUltimoSalto + cooldownRecargaSuelo) && rb.linearVelocity.y <= 0.1f)
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
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}