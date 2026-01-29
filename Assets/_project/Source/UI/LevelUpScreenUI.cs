using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LevelUpScreenUI : MonoBehaviour
{
    [SerializeField] private Button[] buttons;

    private PlayerController _player;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show(PlayerController player)
    {
        _player = player;

        CursorManager.SetUI();
        _player.SetInput(false);
        Time.timeScale = 0f;
        gameObject.SetActive(true);

        var options = GenerateOptions();
        var optionsCount = Mathf.Min(options.Count, buttons.Length);

        for (var i = 0; i < optionsCount; i++)
        {
            var option = options[i];
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().text =
                $"{option.SkillType}\n{option.UpgradeType}";

            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => Select(option));
            buttons[i].gameObject.SetActive(true);
        }

        for (var i = optionsCount; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
    }

    private void Select(UpgradeOption option)
    {
        var skills = _player.Skills;

        if (!skills.HasSkill(option.SkillType))
            skills.AddSkill(option.SkillType);
        else
            skills.ApplyUpgrade(option.SkillType, option.UpgradeType);

        Close();
    }

    private List<UpgradeOption> GenerateOptions()
    {
        var list = new List<UpgradeOption>();
        var skills = _player.Skills;

        foreach (SkillType skill in System.Enum.GetValues(typeof(SkillType)))
        {
            if (!skills.HasSkill(skill))
            {
                list.Add(new UpgradeOption
                {
                    SkillType = skill,
                    UpgradeType = SkillUpgradeType.Damage
                });
            }
            else
            {
                var allowedUpgrades = GetAllowedUpgradesForSkill(skill);
                var upgrade = allowedUpgrades[Random.Range(0, allowedUpgrades.Length)];

                list.Add(new UpgradeOption
                {
                    SkillType = skill,
                    UpgradeType = upgrade
                });
            }
        }

        for (var i = 0; i < list.Count; i++)
        {
            var rnd = Random.Range(0, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }

        var countToTake = Mathf.Min(buttons.Length, list.Count);
        return list.GetRange(0, countToTake);
    }

    private static SkillUpgradeType[] GetAllowedUpgradesForSkill(SkillType skill)
    {
        switch (skill)
        {
            case SkillType.Projectile:
                return new[] { SkillUpgradeType.Damage, SkillUpgradeType.FireRate };
            case SkillType.Aura:
                return new[] { SkillUpgradeType.Damage, SkillUpgradeType.Radius };
            case SkillType.Orbit:
                return new[] { SkillUpgradeType.Damage, SkillUpgradeType.Radius, SkillUpgradeType.Count };
            default:
                return new[] { SkillUpgradeType.Damage };
        }
    }

    private void Close()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        _player.SetInput(true);
        CursorManager.SetGameplay();
    }
}