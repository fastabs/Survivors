using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    public PrefabsConfig prefabsConfig;

    [Header("Scene references")]
    [SerializeField] private ProjectilePool projectilePool;
    [SerializeField] private LevelUpScreenUI levelUpUI;
    [SerializeField] private EnemyRegistry enemyRegistry;

    public GameConfig Config { get; private set; }
    public PlayerController Player { get; private set; }
    public int CurrentLevel { get; private set; } = 1;
    public int CurrentXp { get; private set; }

    public EnemyRegistry Enemies => enemyRegistry;
    public ProjectilePool Projectiles => projectilePool;
    public LevelUpScreenUI LevelUpUI => levelUpUI;

    public event Action OnLevelUp;
    public event Action<int> OnLevelChanged;
    public event Action<int, int> OnXpChanged;

    public void RegisterPlayer(PlayerController player)
    {
        Player = player;
    }

    private void Awake()
    {
        Config = ConfigLoader.Load();
    }

    public void AddXp(int amount)
    {
        CurrentXp += amount;
        CurrentXp = Mathf.Max(0, CurrentXp);

        while (true)
        {
            var threshold = GetXpThresholdForLevel(CurrentLevel);
            if (threshold <= 0)
                break;

            if (CurrentXp < threshold)
            {
                OnXpChanged?.Invoke(CurrentXp, threshold);
                break;
            }

            CurrentXp -= threshold;
            CurrentLevel++;
            OnLevelChanged?.Invoke(CurrentLevel);
            OnLevelUp?.Invoke();
        }
    }

    public int GetXpThresholdForLevel(int level)
    {
        level = Mathf.Max(1, level);

        var baseThreshold = Mathf.Max(1, Config.levelUp.baseXpThreshold);
        var mult = Mathf.Max(1f, Config.levelUp.thresholdMultiplier);

        var threshold = baseThreshold * Mathf.Pow(mult, level - 1);
        return Mathf.Max(1, Mathf.RoundToInt(threshold));
    }
}