using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyPool : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int preloadCount = 32;

    private readonly Queue<Enemy> _pool = new();
    private Transform _root;

    private void Awake()
    {
        _root = new GameObject("Enemies").transform;

        for (var i = 0; i < preloadCount; i++)
            Create();
    }

    private Enemy Create()
    {
        var go = Instantiate(enemyPrefab, _root);
        go.SetActive(false);
        return go.GetComponent<Enemy>();
    }

    public void Get(Vector3 position, EnemyType type)
    {
        var enemy = _pool.Count > 0 ? _pool.Dequeue() : Create();

        enemy.transform.position = position;
        enemy.type = type;
        enemy.gameObject.SetActive(true);
        enemy.ResetState();
    }

    public void Release(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        _pool.Enqueue(enemy);
    }
}