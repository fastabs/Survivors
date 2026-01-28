using System;
using UnityEngine;

public class ProjectileSkill : MonoBehaviour, ISkill
{
    private float _fireRate;
    private float _timer;
    private int _damage;
    private float _projectileSpeed;
    private float _hitDistance;

    private GameManager _gameManager;
    private EnemyRegistry _enemies;
    private ProjectilePool _pool;

    private void Start()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
        _enemies = _gameManager ? _gameManager.Enemies : FindFirstObjectByType<EnemyRegistry>();
        _pool = _gameManager ? _gameManager.Projectiles : FindFirstObjectByType<ProjectilePool>();

        if (!_gameManager)
        {
            Debug.LogError($"{nameof(ProjectileSkill)}: {nameof(GameManager)} not found.", this);
            enabled = false;
            return;
        }

        var cfg = _gameManager.Config.skills.projectile;
        _fireRate = cfg.fireRate;
        _damage = cfg.damage;
        _projectileSpeed = cfg.projectileSpeed;
        _hitDistance = cfg.hitDistance;
    }

    public void Tick()
    {
        _timer += Time.deltaTime;

        if (_timer >= _fireRate)
        {
            _timer = 0;
            Fire();
        }
    }

    private void Fire()
    {
        var target = FindClosestEnemy();
        if (target == null)
            return;

        if (!_pool)
            return;

        var proj = _pool.Get();
        if (!proj)
            return;

        proj.transform.position = transform.position + Vector3.up;
        proj.gameObject.SetActive(true);
        proj.Init(target, _damage, _projectileSpeed, _hitDistance, _pool);
    }

    private Enemy FindClosestEnemy()
    {
        if (_enemies == null)
            return null;

        _enemies.TryGetClosest(transform.position, 0, out var closest);
        return closest;
    }

    public void ApplyUpgrade(SkillUpgradeType type)
    {
        switch (type)
        {
            case SkillUpgradeType.FireRate:
                _fireRate *= 0.85f;
                break;
            case SkillUpgradeType.Damage:
                _damage += 2;
                break;
            case SkillUpgradeType.Radius:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}