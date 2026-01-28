[System.Serializable]
public class SkillConfig
{
    public ProjectileSkillConfig projectile;
    public AuraSkillConfig aura;
    public OrbitSkillConfig orbit;
}

[System.Serializable]
public class ProjectileSkillConfig
{
    public int damage;
    public float fireRate;
    public float projectileSpeed;
    public float hitDistance;
}

[System.Serializable]
public class AuraSkillConfig
{
    public int damage;
    public float radius;
    public float interval;
}

[System.Serializable]
public class OrbitSkillConfig
{
    public int damage;
    public int count;
    public float radius;
    public float rotationSpeed;
}