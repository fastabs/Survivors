using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreenUI : MonoBehaviour
{
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        gameObject.SetActive(false);
        restartButton.onClick.AddListener(Restart);
    }

    public void Show()
    {
        CursorManager.SetUI();
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    private void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        CursorManager.SetGameplay();
    }
}