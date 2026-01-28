using System;
using UnityEngine;
using System.Collections.Generic;

public class OrbitSkill : MonoBehaviour, ISkill
{
    private readonly List<Transform> _orbs = new();
    private int _count;
    private float _radius;
    private float _speed;
    private int _damage;
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
        if (!_gameManager)
        {
            Debug.LogError($"{nameof(OrbitSkill)}: {nameof(GameManager)} not found.", this);
            enabled = false;
            return;
        }

        var cfg = _gameManager.Config.skills.orbit;
        _damage = cfg.damage;
        _count = Mathf.Max(1, cfg.count);
        _radius = Mathf.Max(0.1f, cfg.radius);
        _speed = cfg.rotationSpeed;

        CreateOrbs();
    }

    public void Tick()
    {
        transform.Rotate(Vector3.up, _speed * Time.deltaTime);

        for (int i = 0; i < _orbs.Count; i++)
        {
            var angle = (360f / _orbs.Count) * i;
            var pos = Quaternion.Euler(0, angle, 0) * Vector3.forward * _radius;
            _orbs[i].localPosition = pos;
        }
    }

    private void CreateOrbs()
    {
        var orbPrefab = _gameManager.prefabsConfig.OrbitOrbPrefab;
        for (var i = 0; i < _count; i++)
        {
            GameObject orb;
            if (orbPrefab)
            {
                orb = Instantiate(orbPrefab, transform);
            }
            else
            {
                orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                orb.transform.SetParent(transform);
                orb.transform.localScale = Vector3.one * 0.4f;
                orb.name = "Orb";
                var col = orb.GetComponent<Collider>();
                if (col) col.isTrigger = true;
            }

            if (!orb.TryGetComponent(out OrbitOrb orbDamage))
                orbDamage = orb.AddComponent<OrbitOrb>();

            orbDamage.Init(_damage);
            _orbs.Add(orb.transform);
        }
    }

    public void ApplyUpgrade(SkillUpgradeType type)
    {
        switch (type)
        {
            case SkillUpgradeType.Damage:
                _damage++;
                break;
            case SkillUpgradeType.Radius:
                _radius += 0.5f;
                break;
            case SkillUpgradeType.FireRate:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}