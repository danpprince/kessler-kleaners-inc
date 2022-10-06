using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";
    public static void Init()
    {
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }
    }

    public static void Save(string saveString, string saveName)
    {
        File.WriteAllText(SAVE_FOLDER + saveName + ".txt", saveString);
    }

    public static string Load(string saveName)
    {
        if (File.Exists(SAVE_FOLDER + saveName + ".txt"))
        {
            string saveString = File.ReadAllText(SAVE_FOLDER + saveName + ".txt");
            return saveString;
        }
        else
        {
            Debug.Log("No File: " + saveName + ".txt at " + SAVE_FOLDER);
            return null;
        }
    }
}
