using UnityEngine;

public class ExperiencePickup : MonoBehaviour
{
    private int _amount;
    private GameManager _gameManager;
    private Transform _player;

    private float _lifeTimer;

    private float _attractRadius;
    private float _attractSpeed;
    private float _collectDistance;
    private float _lifetime;

    public void Init(int amount, GameManager gameManager)
    {
        _amount = Mathf.Max(0, amount);
        _gameManager = gameManager;
        _player = gameManager ? gameManager.Player.transform : null;
        _lifeTimer = 0;

        CacheConfig();
        SnapToPlayerHeight();
    }

    private void Awake()
    {
        if (!_gameManager)
            _gameManager = FindFirstObjectByType<GameManager>();
        if (!_player && _gameManager && _gameManager.Player)
            _player = _gameManager.Player.transform;

        CacheConfig();
        SnapToPlayerHeight();
    }

    private void Update()
    {
        if (!_gameManager || !_player)
        {
            Destroy(gameObject);
            return;
        }

        var toPlayer = _player.position - transform.position;
        toPlayer.y = 0;
        var dist = toPlayer.magnitude;

        if (dist <= _collectDistance)
        {
            _gameManager.AddXp(_amount);
            Destroy(gameObject);
            return;
        }

        if (dist <= _attractRadius && dist > 0.001f)
        {
            transform.position += (toPlayer / dist) * (_attractSpeed * Time.deltaTime);
            SnapToPlayerHeight();
        }

        _lifeTimer += Time.deltaTime;
        if (_lifetime > 0 && _lifeTimer >= _lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void CacheConfig()
    {
        if (!_gameManager || _gameManager.Config?.pickups == null)
        {
            _attractRadius = 4f;
            _attractSpeed = 12f;
            _collectDistance = 1f;
            _lifetime = 30f;
            return;
        }

        var cfg = _gameManager.Config.pickups;
        _attractRadius = Mathf.Max(0.01f, cfg.xpAttractRadius);
        _attractSpeed = Mathf.Max(0.01f, cfg.xpAttractSpeed);
        _collectDistance = Mathf.Max(0.01f, cfg.xpCollectDistance);
        _lifetime = cfg.xpLifetime;
    }

    private void SnapToPlayerHeight()
    {
        if (!_player)
            return;

        var p = transform.position;
        p.y = _player.position.y + 0.2f;
        transform.position = p;
    }
}

