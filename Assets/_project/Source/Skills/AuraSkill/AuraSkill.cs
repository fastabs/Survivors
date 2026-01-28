using System;
using UnityEngine;

public class AuraSkill : MonoBehaviour, ISkill
{
    private float _radius;
    private int _damage;
    private float _interval;
    private float _timer;

    private GameObject _visual;
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
        if (!_gameManager)
        {
            Debug.LogError($"{nameof(AuraSkill)}: {nameof(GameManager)} not found.", this);
            enabled = false;
            return;
        }

        var cfg = _gameManager.Config.skills.aura;
        _radius = cfg.radius;
        _damage = cfg.damage;
        _interval = cfg.interval;

        CreateVisual();
    }

    public void Tick()
    {
        _timer += Time.deltaTime;

        if (_timer >= _interval)
        {
            _timer = 0;
            DamageEnemies();
        }
    }

    private void DamageEnemies()
    {
        var hits = Physics.OverlapSphere(transform.position, _radius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Enemy enemy))
                enemy.TakeDamage(_damage);
        }
    }

    private void CreateVisual()
    {
        var prefab = _gameManager.prefabsConfig.AuraPrefab;
        if (!prefab)
        {
            _visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _visual.transform.SetParent(transform);
            _visual.transform.localPosition = Vector3.zero;
            _visual.name = "Aura";
            var col = _visual.GetComponent<Collider>();

            if (col)
                Destroy(col);

            UpdateVisualScale();
            return;
        }

        _visual = Instantiate(prefab, transform);
        _visual.name = "Aura";
        _visual.transform.localPosition = Vector3.zero;

        UpdateVisualScale();
    }

    private void UpdateVisualScale()
    {
        _visual.transform.localScale = new Vector3(_radius * 2f, 0.001f, _radius * 2f);
    }

    public void ApplyUpgrade(SkillUpgradeType type)
    {
        switch (type)
        {
            case SkillUpgradeType.Damage:
                _damage += 1;
                break;
            case SkillUpgradeType.Radius:
                _radius += 0.5f;
                UpdateVisualScale();
                break;
            case SkillUpgradeType.FireRate:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}