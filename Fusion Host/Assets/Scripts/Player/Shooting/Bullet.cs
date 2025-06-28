using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

[RequireComponent(typeof(NetworkRigidbody3D))]
public class Bullet : NetworkBehaviour
{
    TickTimer _lifeTimer = TickTimer.None;

    [SerializeField] private byte _damage = 25;

    public override void Spawned()
    {
        GetComponent<NetworkRigidbody3D>().Rigidbody.AddForce(transform.forward * 10, ForceMode.VelocityChange);

        if (HasStateAuthority)
        {
            _lifeTimer = TickTimer.CreateFromSeconds(Runner, 2);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (_lifeTimer.Expired(Runner))
        {
            DespawnObject();
        }
    }

    void DespawnObject()
    {
        _lifeTimer = TickTimer.None;
        
        Runner.Despawn(Object);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object || !Object.HasStateAuthority) return;

        if (other.TryGetComponent(out LifeHandler lifeHandler))
        {
            lifeHandler.TakeDamage(_damage);
        }
        
        DespawnObject();
    }
}
