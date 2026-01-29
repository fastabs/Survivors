using System;
using UnityEngine;
using System.Collections.Generic;

public class OrbitSkill : MonoBehaviour, ISkill
{
    private int _count;
    private float _radius;
    private float _rotationSpeed;
    private int _damage;
    private GameManager _gameManager;
    private Transform _orbitRoot;
    private readonly List<Transform> _orbs = new();

    private void Awake()
    {
        _orbitRoot = new GameObject("OrbitRoot").transform;
        _orbitRoot.SetParent(transform);
        _orbitRoot.localPosition = Vector3.zero;
        _orbitRoot.localRotation = Quaternion.identity;
    }

    private void Start()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
        if (!_gameManager)
        {
            Debug.LogError($"{nameof(OrbitSkill)}:{nameof(GameManager)} not found.", this);
            enabled = false;
            return;
        }

        var cfg = _gameManager.Config.skills.orbit;
        _damage = cfg.damage;
        _count = Mathf.Max(1, cfg.count);
        _radius = Mathf.Max(0.1f, cfg.radius);
        _rotationSpeed = cfg.rotationSpeed;

        CreateOrbs();
    }

    public void Tick()
    {
        _orbitRoot.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime, Space.Self);
        UpdateOrbPositions();
    }

    private void CreateOrbs()
    {
        ClearOrbs();

        var orbPrefab = _gameManager.prefabsConfig.OrbitOrbPrefab;
        for (var i = 0; i < _count; i++)
        {
            var orb = Instantiate(orbPrefab, _orbitRoot);
            orb.GetComponent<OrbitOrb>().Init(_damage);
            _orbs.Add(orb.transform);
        }

        UpdateOrbPositions();
    }

    private void UpdateOrbPositions()
    {
        for (var i = 0; i < _orbs.Count; i++)
        {
            var angle = i * (360f / _orbs.Count);
            var pos = Quaternion.Euler(0, angle, 0) * Vector3.forward * _radius;
            _orbs[i].localPosition = pos;
        }
    }

    private void ClearOrbs()
    {
        foreach (var orb in _orbs)
            Destroy(orb.gameObject);

        _orbs.Clear();
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
            case SkillUpgradeType.Count:
                _count++;
                CreateOrbs();
                break;
            case SkillUpgradeType.FireRate:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}