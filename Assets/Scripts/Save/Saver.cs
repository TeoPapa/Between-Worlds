using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Saver
{
    static string FileName = "/save.game";

    public static void SaveGame(SaveData sav) {
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath + FileName;
        FileStream stream = new FileStream(path, FileMode.Create);

        bf.Serialize(stream, sav);
        stream.Close();
    }

    public static SaveData LoadGame() {
        string path = Application.persistentDataPath + FileName;

        try {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = bf.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        } catch (FileNotFoundException e) {
            Debug.LogException(e);
            return null;
        }
    }

    public static bool SaveExists() {
        string path = Application.persistentDataPath + FileName;
        return File.Exists(path);
    }

    public static void DeleteSave() {
            string path = Application.persistentDataPath + FileName;
    
            if (File.Exists(path)) {
                File.Delete(path);
            }
    }
}
