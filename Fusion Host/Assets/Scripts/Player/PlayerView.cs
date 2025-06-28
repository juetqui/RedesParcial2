using Fusion;
using UnityEngine;

public class PlayerView : NetworkBehaviour
{
    [SerializeField] private GameObject _playerVisual;

    private NetworkMecanimAnimator _mecanimAnimator;

    [Networked] private NetworkBool Firing { get; set; }

    public override void Spawned()
    {
        var weaponComponent = GetComponentInParent<WeaponHandler>();

        if (weaponComponent)
        {
            weaponComponent.OnShot += ()=> Firing = !Firing;
        }
        
        var lifeComponent = GetComponentInParent<LifeHandler>();

        if (lifeComponent)
        {
            lifeComponent.OnDeadChanged += EnableMeshRender;
        }
        
        _mecanimAnimator = GetComponent<NetworkMecanimAnimator>();

        if (_mecanimAnimator)
        {
            var movementComponent = GetComponentInParent<NetworkCharacterControllerCustom>();

            if (movementComponent)
            {
                movementComponent.OnMoving += MoveAnimation;
            }
        }
    }

    void MoveAnimation(float xValue)
    {
        _mecanimAnimator.Animator.SetFloat("xAxi", xValue);
    }

    void EnableMeshRender(bool e)
    {
        _playerVisual.SetActive(!e);
    }
}
