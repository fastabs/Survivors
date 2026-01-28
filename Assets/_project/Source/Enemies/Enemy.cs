using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyType type;

    private int _health;
    private float _speed;
    private int _xpDrop;
    private float _attackTimer;
    private Transform _player;
    private GameManager _gameManager;
    private EnemyRegistry _registry;

    private float _attackDistance;
    private float _attackInterval;
    private int _damage;

    private float _separationRadius;
    private float _separationForce;
    private float _separationTickInterval;
    private float _separationTimer;
    private Vector3 _cachedSeparation;

    private void Awake()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
        _registry = _gameManager ? _gameManager.Enemies : FindFirstObjectByType<EnemyRegistry>();
    }

    private void OnEnable()
    {
        if (_registry)
            _registry.Register(this);
    }

    private void Start()
    {
        if (!_gameManager)
        {
            Debug.LogError($"{nameof(Enemy)}: {nameof(GameManager)} not found.", this);
            enabled = false;
            return;
        }

        var cfg = _gameManager.Config.enemies;
        var common = cfg.common;
        var stats = type == EnemyType.Fast ? cfg.fast : cfg.normal;

        _health = stats.health;
        _speed = stats.speed;
        _xpDrop = stats.xpDrop;

        _attackDistance = stats.attackDistance > 0 ? stats.attackDistance : common.attackDistance;
        _attackInterval = stats.attackInterval > 0 ? stats.attackInterval : common.attackInterval;
        _damage = stats.damage > 0 ? stats.damage : common.damage;

        _separationRadius = common.separationRadius;
        _separationForce = common.separationForce;
        _separationTickInterval = Mathf.Max(0.01f, common.separationTickInterval);

        _player = _gameManager.Player.transform;
    }

    private void OnDisable()
    {
        if (_registry)
            _registry.Unregister(this);
    }

    private void Update()
    {
        var toPlayer = _player.position - transform.position;
        var distance = toPlayer.magnitude;
        var moveDir = Vector3.zero;

        if (distance > _attackDistance)
        {
            moveDir += toPlayer.normalized;
        }
        else
        {
            AttackPlayer();
        }

        moveDir += CalculateSeparation();

        if (moveDir.sqrMagnitude > 0)
        {
            moveDir.Normalize();

            var move = _speed * Time.deltaTime;
            var distToStop = distance - _attackDistance;

            if (move > distToStop)
                move = distToStop;

            transform.position += moveDir * move;
        }
    }

    public void TakeDamage(int dmg)
    {
        _health -= dmg;
        if (_health <= 0)
            Die();
    }

    private void AttackPlayer()
    {
        _attackTimer += Time.deltaTime;

        if (_attackTimer >= _attackInterval)
        {
            _attackTimer = 0;
            _gameManager.Player.GetComponent<PlayerHealth>().TakeDamage(_damage);
        }
    }

    private Vector3 CalculateSeparation()
    {
        _separationTimer += Time.deltaTime;
        if (_separationTimer < _separationTickInterval)
            return _cachedSeparation;

        _separationTimer = 0;

        if (_registry == null || _separationRadius <= 0 || _separationForce <= 0)
        {
            _cachedSeparation = Vector3.zero;
            return _cachedSeparation;
        }

        var force = Vector3.zero;
        var r2 = _separationRadius * _separationRadius;
        var selfPos = transform.position;

        var enemies = _registry.Enemies;

        foreach (var enemy in enemies)
        {
            if (!enemy || enemy == this)
                continue;

            var diff = selfPos - enemy.transform.position;
            var d2 = diff.sqrMagnitude;
            if (d2 <= 0 || d2 > r2)
                continue;

            force += diff / (d2 + 0.001f);
        }

        _cachedSeparation = force.normalized * _separationForce;
        return _cachedSeparation;
    }

    private void Die()
    {
        var pickupPrefab = _gameManager.prefabsConfig ? _gameManager.prefabsConfig.XpPickupPrefab : null;
        GameObject go;
        var spawnPos = transform.position;
        if (_player)
            spawnPos.y = _player.position.y + 0.2f;

        if (pickupPrefab)
        {
            go = Instantiate(pickupPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "XP";
            go.transform.position = spawnPos;
            go.transform.localScale = Vector3.one * 0.35f;

            var col = go.GetComponent<Collider>();
            if (col) col.isTrigger = true;
        }

        if (go.TryGetComponent(out ExperiencePickup xp))
        {
            xp.Init(_xpDrop, _gameManager);
        }
        else
        {
            xp = go.AddComponent<ExperiencePickup>();
            xp.Init(_xpDrop, _gameManager);
        }

        Destroy(gameObject);
    }
}