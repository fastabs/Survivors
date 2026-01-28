using UnityEngine;

public static class ConfigLoader
{
    public static GameConfig Load()
    {
        var json = Resources.Load<TextAsset>("game_config");
        return JsonUtility.FromJson<GameConfig>(json.text);
    }
}