using System;
using Fusion;
using UnityEngine;

public class NetworkCharacterControllerCustom : NetworkCharacterController
{
    public event Action<float> OnMoving = delegate {  };
    
    public override void Move(Vector3 direction)
    {
        var deltaTime = Runner.DeltaTime;
        var previousPos = transform.position;
        var moveVelocity = Velocity;

        direction = direction.normalized;

        if (Grounded && moveVelocity.y < 0)
        {
            moveVelocity.y = 0f;
        }

        moveVelocity.y += gravity * Runner.DeltaTime;

        var horizontalVel = new Vector3(moveVelocity.x, 0, moveVelocity.z);

        if (direction == default)
        {
            horizontalVel = Vector3.Lerp(horizontalVel, default, braking * deltaTime);
        }
        else
        {
            horizontalVel = direction * maxSpeed; // Velocidad máxima directamente
            if (direction.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }

        moveVelocity.x = horizontalVel.x;
        moveVelocity.z = horizontalVel.z;

        Controller.Move(moveVelocity * deltaTime);

        Velocity = (transform.position - previousPos) * Runner.TickRate;
        Grounded = Controller.isGrounded;

        OnMoving(Velocity.magnitude);
    }
}
