using System;
using Fusion;
using UnityEngine;

public class NetworkCharacterControllerCustom : NetworkCharacterController
{
    public event Action<float> OnMoving = delegate { };
    [SerializeField] private float rotationSpeed = 720f; // Grados por segundo
    [SerializeField] private float dashSpeed = 20f; // Velocidad del dash
    [SerializeField] private float dashDuration = 0.2f; // Duración del dash en segundos
    [SerializeField] private float dashCooldown = 1f; // Cooldown entre dashes

    [Networked] private float dashTimer { get; set; } // Temporizador para duración del dash
    [Networked] private float dashCooldownTimer { get; set; } // Temporizador para cooldown
    [Networked] private bool isDashing { get; set; } // Estado del dash

    public override void Move(Vector3 direction)
    {
        if (!HasStateAuthority) return; // Solo el cliente con autoridad ejecuta el movimiento

        var deltaTime = Runner.DeltaTime;
        var previousPos = transform.position;
        var moveVelocity = Velocity;

        direction = direction.normalized;

        // Manejo del temporizador de dash y cooldown
        if (isDashing)
        {
            dashTimer -= deltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false; // Termina el dash
            }
        }
        else
        {
            dashCooldownTimer -= deltaTime; // Reducir cooldown cuando no está en dash
        }

        // Aplicar gravedad
        if (Grounded && moveVelocity.y < 0)
        {
            moveVelocity.y = 0f;
        }
        moveVelocity.y += gravity * deltaTime;

        var horizontalVel = new Vector3(moveVelocity.x, 0, moveVelocity.z);

        if (direction == Vector3.zero && !isDashing)
        {
            horizontalVel = Vector3.Lerp(horizontalVel, Vector3.zero, braking * deltaTime);
        }
        else
        {
            // Usar velocidad de dash si está activo, o velocidad normal
            float currentSpeed = isDashing ? dashSpeed : maxSpeed;
            horizontalVel = direction * currentSpeed;

            // Rotar hacia la dirección de movimiento
            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1 - Mathf.Exp(-rotationSpeed * deltaTime));
            }
        }

        moveVelocity.x = horizontalVel.x;
        moveVelocity.z = horizontalVel.z;

        Controller.Move(moveVelocity * deltaTime);

        Velocity = (transform.position - previousPos) * Runner.TickRate;
        Grounded = Controller.isGrounded;

        OnMoving(Velocity.magnitude);
    }

    // Método para iniciar el dash
    public void StartDash(Vector3 direction)
    {
        if (!HasStateAuthority) return; // Solo el cliente con autoridad puede iniciar el dash
        if (dashCooldownTimer > 0 || isDashing) return; // No iniciar si está en cooldown o ya está en dash

        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown + dashDuration; // Cooldown comienza después del dash
    }
}
