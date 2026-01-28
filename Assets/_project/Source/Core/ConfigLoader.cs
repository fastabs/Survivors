using UnityEngine;

public static class ConfigLoader
{
    public static GameConfig Load()
    {
        var json = Resources.Load<TextAsset>("game_config");
        if (!json)
        {
            Debug.LogError("ConfigLoader: Resources/game_config.json not found.");
            return new GameConfig();
        }

        return JsonUtility.FromJson<GameConfig>(json.text);
    }
}