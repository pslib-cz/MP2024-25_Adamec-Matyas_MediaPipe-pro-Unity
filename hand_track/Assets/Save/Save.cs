using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public sealed class Save<T> where T : SaveData
{
    public string name;
    public T saveData;

    private Save() { }

    public Save(string name, T saveData)
    {
        this.name = name;
        this.saveData = saveData;
    }

}

[System.Serializable]
public abstract class SaveData { } // Change from record to class

[System.Serializable]
public class LevelSaveData : SaveData
{
    public int levelId;
    public int maxScore;
}

[System.Serializable]
public class LevelSaveDataContainer : SaveData
{
    public List<LevelSaveData> levels = new List<LevelSaveData>();
}