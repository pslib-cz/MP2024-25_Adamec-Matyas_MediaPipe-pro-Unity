using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

public static class SaveManager
{
    private static readonly string saveFolder = Application.persistentDataPath + "/GameData";
    private static readonly string profileName = "levelData";

    public static void Delete()
    {
        if (File.Exists($"{saveFolder}/{profileName}"))
            File.Delete($"{saveFolder}/{profileName}");
    }
    public static Save<T> Load<T>() where T : SaveData, new()
    {
        Debug.Log($"Loading save file: {saveFolder}/{profileName}");
        if (!File.Exists($"{saveFolder}/{profileName}"))
        {
            Debug.Log($"Save file not found: {saveFolder}/{profileName}");
            return new Save<T>(profileName, saveData: new T());
        }

        var fileContents = File.ReadAllText($"{saveFolder}/{profileName}");

        Debug.Log($"Loaded save file: {profileName}");

        return JsonUtility.FromJson<Save<T>>(fileContents);
    }
    public static void Save<T>(Save<T> save) where T : SaveData
    {
        if (File.Exists($"{saveFolder}/{save.name}"))
            Debug.Log($"Overwriting save file: {saveFolder}/{save.name}");

        var jsonString = JsonUtility.ToJson(save);

        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        File.WriteAllText($"{saveFolder}/{save.name}", jsonString);
        Debug.Log($"Saved file to {saveFolder}/{save.name}");
    }

    public static void SaveLevel(int id, int maxScore)
    {
        if (!File.Exists($"{saveFolder}/{profileName}"))
        {
            LevelSaveDataContainer save = new LevelSaveDataContainer();
            LevelSaveData saveData = new LevelSaveData
            {
                levelId = id,
                maxScore = maxScore
            };
            save.levels.Add(saveData);
            Save(new Save<LevelSaveDataContainer>("levelData", save));
        }
        else
        {
            // only update the level data
            var save = Load<LevelSaveDataContainer>();
            var levelData = save.saveData.levels.Find(x => x.levelId == id);
            if (levelData == null)
            {
                levelData = new LevelSaveData
                {
                    levelId = id,
                    maxScore = maxScore
                };
                save.saveData.levels.Add(levelData);
            }
            else
            {
                levelData.maxScore = maxScore;
            }
            Save(new Save<LevelSaveDataContainer>("levelData", save.saveData));
        }


    }


}
