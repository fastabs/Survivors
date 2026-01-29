using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private GameManager _gameManager;
    private float _timer;

    private void Awake()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Update()
    {
        if (!_gameManager || !_gameManager.Player)
            return;

        var cfg = _gameManager.Config.spawner;
        var targetAlive = cfg.baseCount + _gameManager.CurrentLevel * cfg.perLevel;

        if (_gameManager.Enemies.Enemies.Count >= targetAlive)
            return;

        _timer += Time.deltaTime;
        if (_timer >= cfg.interval)
        {
            _timer = 0;
            Spawn();
        }
    }

    private void Spawn()
    {
        var playerPos = _gameManager.Player.transform.position;
        var dir = Random.insideUnitCircle.normalized;
        var dist = Random.Range(6f, 10f);
        var pos = playerPos + new Vector3(dir.x, 0, dir.y) * dist;

        _gameManager.EnemyPool.Get(pos, Random.value < 0.25f ? EnemyType.Fast : EnemyType.Normal);
    }
}