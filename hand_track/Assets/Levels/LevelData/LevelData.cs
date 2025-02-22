using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelId;
    public string levelName;
    public AudioClip levelMusic;
    public Sprite levelIcon;
    public TextAsset jsonConfig; 
}


[System.Serializable]
public class LevelJson
{
    public int id;
    public string name;
    public string description;
    public float Time;
    public Block[] blocks;
}

[System.Serializable]
public class Block
{
    public int id;
    public float time;
    public Position position; 
    public string type;
    public int points;
    public bool isRight;
    public Long longData;
}

[System.Serializable]
public class Position
{
    public float x;
    public float y;
}

[System.Serializable]
public class Long
{
    public float duration;
    public Stop[] stops;
}

[System.Serializable]
public class Stop
{
    public float x;
    public float y;
    public float time;
}
