using UnityEngine;
using System.Collections.Generic;

public class ProjectilePool : MonoBehaviour
{
    private readonly Queue<Projectile> _pool = new();
    private Projectile _projectile;

    private void Awake()
    {
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager && gameManager.prefabsConfig && gameManager.prefabsConfig.ProjectilePrefab)
            _projectile = gameManager.prefabsConfig.ProjectilePrefab.GetComponent<Projectile>();

        if (!_projectile)
            Debug.LogError($"{nameof(ProjectilePool)}: projectile prefab is not set (PrefabsConfig.ProjectilePrefab).", this);
    }

    public Projectile Get()
    {
        if (!_projectile)
            return null;

        if (_pool.Count > 0)
            return _pool.Dequeue();

        return Instantiate(_projectile);
    }

    public void Return(Projectile proj)
    {
        proj.gameObject.SetActive(false);
        _pool.Enqueue(proj);
    }
}