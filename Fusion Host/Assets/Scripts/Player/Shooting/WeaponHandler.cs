using System;
using System.Collections;
using Fusion;
using UnityEngine;

public class WeaponHandler : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef _primaryBulletPrefab;
    [SerializeField] private NetworkPrefabRef _secondaryBulletPrefab;

    [SerializeField] private Transform _shotSpawnTransform;

    public event Action OnShot = delegate { };

    public void FirePrimary()
    {
        if (!HasStateAuthority) return;

        SpawnBullet(_primaryBulletPrefab);
        RayBullet(5); // daño primario
        OnShot();
    }

    public void FireSecondary()
    {
        if (!HasStateAuthority) return;

        SpawnBullet(_secondaryBulletPrefab);
        RayBullet(20); // daño menor u otro efecto
        OnShot();
    }

    void SpawnBullet(NetworkPrefabRef prefab)
    {
        Runner.Spawn(prefab, _shotSpawnTransform.position, _shotSpawnTransform.rotation);
    }

    void RayBullet(byte damage)
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 5, Color.magenta, 2);

        Runner.LagCompensation.Raycast(origin: transform.position,
                                        direction: transform.forward,
                                        length: 5,
                                        player: Object.InputAuthority,
                                        hit: out var hitInfo);

        if (hitInfo.Hitbox == null) return;

        if (!hitInfo.Hitbox.transform.root.TryGetComponent(out LifeHandler player)) return;

        player.TakeDamage(damage);
    }
}
