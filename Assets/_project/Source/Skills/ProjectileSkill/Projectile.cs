using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Enemy _target;
    private int _damage;
    private float _speed;
    private float _hitDistance;
    private ProjectilePool _pool;

    public void Init(Enemy enemy, int dmg, float speed, float hitDistance, ProjectilePool pool)
    {
        _target = enemy;
        _damage = dmg;
        _speed = Mathf.Max(0.01f, speed);
        _hitDistance = Mathf.Max(0.01f, hitDistance);
        _pool = pool;
    }

    private void Update()
    {
        if (_target == null)
        {
            Return();
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            _target.transform.position,
            _speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, _target.transform.position) < _hitDistance)
        {
            _target.TakeDamage(_damage);
            Return();
        }
    }

    private void Return()
    {
        if (_pool)
            _pool.Return(this);
        else
            gameObject.SetActive(false);
    }
}