using UnityEngine;

public class OrbitOrb : MonoBehaviour
{
    private int _damage;

    public void Init(int dmg)
    {
        _damage = dmg;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
            enemy.TakeDamage(_damage);
    }
}