using UnityEngine;
using System.IO;

// Cetakan data yang akan di-convert ke JSON
[System.Serializable]
public class SaveData
{
    public float bestTime;
}

// Class utama pengelola Save & Load
public static class SaveSystem
{
    private static string path = Application.persistentDataPath + "/savegame.json";

    public static void SaveBestTime(float time)
    {
        SaveData data = new SaveData();
        data.bestTime = time;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("Data JSON disimpan di: " + path);
    }

    public static float LoadBestTime()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data.bestTime;
        }
        return 0f;
    }

    public static void ResetData()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("File Save JSON dihapus!");
        }
    }
}