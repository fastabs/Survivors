using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private DeathScreenUI deathUI;

    public int MaxHp { get; private set; }
    public int CurrentHp { get; private set; }

    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;

    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
        MaxHp = _gameManager ? _gameManager.Config.player.maxHealth : 100;
        CurrentHp = MaxHp;
    }

    public void TakeDamage(int damage)
    {
        if (CurrentHp <= 0)
            return;

        CurrentHp -= damage;
        CurrentHp = Mathf.Max(CurrentHp, 0);

        OnHealthChanged?.Invoke(CurrentHp, MaxHp);

        if (CurrentHp == 0)
            Die();
    }

    private void Die()
    {
        OnDeath?.Invoke();
        deathUI.Show();
    }
}