using UnityEngine;

public class ExperiencePickup : MonoBehaviour
{
    private int _amount;
    private GameManager _gameManager;
    private Transform _player;

    private float _lifeTimer;

    public void Init(int amount, GameManager gameManager)
    {
        _amount = Mathf.Max(0, amount);
        _gameManager = gameManager;
        _player = gameManager ? gameManager.Player.transform : null;
        _lifeTimer = 0;
    }

    private void Awake()
    {
        if (!_gameManager)
            _gameManager = FindFirstObjectByType<GameManager>();
        if (!_player && _gameManager && _gameManager.Player)
            _player = _gameManager.Player.transform;
    }

    private void Update()
    {
        if (!_gameManager || !_player)
        {
            Destroy(gameObject);
            return;
        }

        var cfg = _gameManager.Config.pickups;
        var toPlayer = _player.position - transform.position;
        var dist = toPlayer.magnitude;

        if (dist <= cfg.xpCollectDistance)
        {
            _gameManager.AddXp(_amount);
            Destroy(gameObject);
            return;
        }

        if (dist <= cfg.xpAttractRadius && dist > 0.001f)
        {
            var speed = Mathf.Max(0.01f, cfg.xpAttractSpeed);
            transform.position += (toPlayer / dist) * (speed * Time.deltaTime);
        }

        _lifeTimer += Time.deltaTime;
        if (cfg.xpLifetime > 0 && _lifeTimer >= cfg.xpLifetime)
        {
            Destroy(gameObject);
        }
    }
}

