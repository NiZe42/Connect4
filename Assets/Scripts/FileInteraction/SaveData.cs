using System;
using UnityEngine;

[Serializable]
public class SaveData {
    
    public GameMode gameMode;
    public int winningLineLength;
    public int horizontalTileCount;
    public int verticalTileCount;
    

    public string ToJson() {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string json) {
        JsonUtility.FromJsonOverwrite(json, this);
    }
}
