using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private PlayerHealth health;

    private void Start()
    {
        if (!health)
        {
            var gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager && gameManager.Player)
                health = gameManager.Player.GetComponent<PlayerHealth>();
        }

        if (!health)
        {
            Debug.LogError($"{nameof(PlayerHealthUI)}: {nameof(PlayerHealth)} not found.", this);
            enabled = false;
            return;
        }

        slider.maxValue = health.MaxHp;
        slider.value = health.CurrentHp;

        health.OnHealthChanged += UpdateBar;
    }

    private void UpdateBar(int current, int max)
    {
        slider.maxValue = max;
        slider.value = current;
    }
}