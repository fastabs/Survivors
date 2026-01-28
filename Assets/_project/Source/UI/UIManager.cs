using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;
    [SerializeField] private GameManager gameManager;

    private void Awake()
    {
        if (!gameManager)
            gameManager = FindFirstObjectByType<GameManager>();
    }

    private void OnEnable()
    {
        if (!gameManager)
            return;

        gameManager.OnLevelChanged += OnLevelChanged;
        gameManager.OnXpChanged += OnXpChanged;

        OnLevelChanged(gameManager.CurrentLevel);
        OnXpChanged(gameManager.CurrentXp, gameManager.GetXpThresholdForLevel(gameManager.CurrentLevel));
    }

    private void OnDisable()
    {
        if (!gameManager)
            return;

        gameManager.OnLevelChanged -= OnLevelChanged;
        gameManager.OnXpChanged -= OnXpChanged;
    }

    private void OnLevelChanged(int level)
    {
        levelText.text = $"Level: {level}";
    }

    private void OnXpChanged(int xp, int threshold)
    {
        xpText.text = $"XP: {xp}/{threshold}";
    }
}