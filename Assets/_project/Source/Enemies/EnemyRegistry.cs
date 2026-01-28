using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyRegistry : MonoBehaviour
{
    private readonly List<Enemy> _enemies = new(256);
    private readonly HashSet<Enemy> _set = new();

    public IReadOnlyList<Enemy> Enemies => _enemies;

    public void Register(Enemy enemy)
    {
        if (!enemy)
            return;

        if (_set.Add(enemy))
            _enemies.Add(enemy);
    }

    public void Unregister(Enemy enemy)
    {
        if (!enemy)
            return;

        if (_set.Remove(enemy))
            _enemies.Remove(enemy);
    }

    public void TryGetClosest(Vector3 from, float maxDistance, out Enemy closest)
    {
        closest = null;
        var maxDistSqr = maxDistance <= 0 ? float.PositiveInfinity : maxDistance * maxDistance;
        var bestSqr = maxDistSqr;

        foreach (var enemy in _enemies)
        {
            if (!enemy)
                continue;

            var delta = enemy.transform.position - from;
            var d2 = delta.sqrMagnitude;

            if (d2 < bestSqr)
            {
                bestSqr = d2;
                closest = enemy;
            }
        }
    }
}

