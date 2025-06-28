using System.Collections;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkCharacterControllerCustom))]
[RequireComponent(typeof(WeaponHandler))]
public class PlayerController : NetworkBehaviour
{
    private NetworkCharacterControllerCustom _characterMovement;
    private WeaponHandler _weaponHandler;

    private bool _canFirePrimary = true, _canFireSecondary = true;

    public override void Spawned()
    {
        _characterMovement = GetComponent<NetworkCharacterControllerCustom>();
        _weaponHandler = GetComponent<WeaponHandler>();

        if (!TryGetBehaviour(out LifeHandler lifeHandler)) return;
        
        lifeHandler.OnDeadChanged += b =>
        {
            enabled = !b;
        };

        lifeHandler.OnRespawn += () =>
        {
            _characterMovement.Teleport(transform.position + Vector3.up * 3);
        };
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData inputs)) return;
        
        //Movimiento
        Vector3 moveDirection = (Vector3.forward * inputs.verticalMovementInput) + (Vector3.right * inputs.horizontalMovementInput);
        _characterMovement.Move(moveDirection);

        //Salto
        if (inputs.networkButtons.IsSet(MyButtons.Jump))
        {
            _characterMovement.StartDash(moveDirection);
        }

        //Disparo primario
        if (inputs.isFirePressed && _canFirePrimary)
        {
            StartCoroutine(PrimaryShootCD());
            _weaponHandler.FirePrimary();
        } 
        
        //Disparo secundario
        if (inputs.isFireSecondaryPressed && _canFireSecondary)
        {
            StartCoroutine(SecondaryShootCD());
            _weaponHandler.FireSecondary();
        }
    }

    private IEnumerator PrimaryShootCD()
    {
        _canFirePrimary = false;
        yield return new WaitForSeconds(0.25f);
        _canFirePrimary = true;
    }

    private IEnumerator SecondaryShootCD()
    {
        _canFireSecondary = false;
        yield return new WaitForSeconds(1f);
        _canFireSecondary = true;
    }
}
