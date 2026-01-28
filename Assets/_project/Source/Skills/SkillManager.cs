using UnityEngine;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    private readonly Dictionary<SkillType, ISkill> _skills = new();

    public bool HasSkill(SkillType type)
    {
        return _skills.ContainsKey(type);
    }

    public void AddSkill(SkillType type)
    {
        if (_skills.ContainsKey(type))
            return;

        ISkill skill = type switch
        {
            SkillType.Projectile => gameObject.AddComponent<ProjectileSkill>(),
            SkillType.Aura => gameObject.AddComponent<AuraSkill>(),
            SkillType.Orbit => gameObject.AddComponent<OrbitSkill>(),
            _ => null
        };

        if (skill != null)
            _skills.Add(type, skill);
    }

    public void ApplyUpgrade(SkillType skill, SkillUpgradeType upgrade)
    {
        if (_skills.TryGetValue(skill, out var s))
            s.ApplyUpgrade(upgrade);
    }

    public void Tick()
    {
        foreach (var skill in _skills.Values)
            skill.Tick();
    }
}