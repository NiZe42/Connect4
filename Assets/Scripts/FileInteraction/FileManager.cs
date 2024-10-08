using System;
using System.IO;
using UnityEngine;

public static class FileManager {
    public static bool WriteToFile(string fileName, string fileContent) {
        var fullPath = Path.Combine(Application.persistentDataPath, fileName);

        try {
            File.WriteAllText(fullPath, fileContent);
            return true;
        }
        catch (Exception e) {
            Debug.LogError($"Failed to write to {fullPath} with exception {e}");
            return false;
        }
    }

    public static bool LoadFromFile(string fileName, out string result) {
        var fullPath = Path.Combine(Application.persistentDataPath, fileName);

        try {
            result = File.ReadAllText(fullPath);
            return true;
        }
        catch (Exception e) {
            Debug.LogError($"Failed to read from {fullPath} with exception {e}");
            result = "";
            return false;
        }
    }
}
