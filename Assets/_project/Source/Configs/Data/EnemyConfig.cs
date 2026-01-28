[System.Serializable]
public class EnemyConfig
{
    public EnemyCombatConfig common;
    public EnemyStats normal;
    public EnemyStats fast;
}

[System.Serializable]
public class EnemyCombatConfig
{
    public float attackDistance;
    public float attackInterval;
    public int damage;
    public float separationRadius;
    public float separationForce;
    public float separationTickInterval;
}

[System.Serializable]
public class EnemyStats
{
    public float speed;
    public int health;
    public int xpDrop;

    // Optional
    public float attackDistance;
    public float attackInterval;
    public int damage;
}