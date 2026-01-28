using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    private float _timer;
    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Update()
    {
        if (!_gameManager || !_gameManager.Player || _gameManager.Config == null)
            return;

        var cfg = _gameManager.Config.spawner;
        if (cfg == null)
            return;

        if (cfg.maxAlive > 0 && _gameManager.Enemies != null && _gameManager.Enemies.Enemies.Count >= cfg.maxAlive)
            return;

        _timer += Time.deltaTime;

        if (_timer >= Mathf.Max(0.01f, cfg.interval))
        {
            _timer = 0;
            Spawn();
        }
    }

    private void Spawn()
    {
        var cfg = _gameManager.Config.spawner;

        var playerPos = _gameManager.Player.transform.position;
        var dir2 = Random.insideUnitCircle.normalized;
        if (dir2.sqrMagnitude < 0.001f)
            dir2 = Vector2.right;

        var dist = Random.Range(cfg.minDistance, cfg.maxDistance);
        dist = Mathf.Max(0.1f, dist);

        var pos = playerPos + new Vector3(dir2.x, 0, dir2.y) * dist;
        pos.y = cfg.spawnHeight;

        var enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
        enemy.GetComponent<Enemy>().type = Random.value < cfg.fastChance ? EnemyType.Fast : EnemyType.Normal;
    }
}