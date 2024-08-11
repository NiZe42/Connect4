using UnityEngine;

public static class SaveDataManager
{
    public static void SaveDataObject(SaveData saveData) {
        if (FileManager.WriteToFile("SaveData.json", saveData.ToJson())) {
            Debug.Log("Save successful");
        }
    }

    public static SaveData LoadSaveDataObject() {
        if (FileManager.LoadFromFile("SaveData.json", out string json)) {
            SaveData saveData = new SaveData();
            saveData.LoadFromJson(json);
            Debug.Log("Load complete");
            return saveData;
        }

        Debug.Log("Load failed");
        return null;
    }
}
