using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class DataMaster : MonoBehaviour
{
    IDataMasterNOSClient clientNotificationSystem;
    [SerializeField] protected SaveObject dataSave = new SaveObject();
    protected string directory = "/SavedData/";
    protected string filename = "SavedList.txt";


    public DateTime ConvertIntArray_toDatetime(int[] toconvert)
    {
        return new DateTime(toconvert[4], toconvert[3], toconvert[2], toconvert[1], toconvert[0], 0);
    }

    protected void SaveList()
    {
        string dir = Application.persistentDataPath + directory;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);

        }
        string saveJason = JsonUtility.ToJson(dataSave);
        File.WriteAllText(dir + filename, saveJason);


    }
    protected void LoadList()
    {
        string loadpath = Application.persistentDataPath + directory + filename;
        print(loadpath);
        if (File.Exists(loadpath))
        {
            string readstring = File.ReadAllText(loadpath);
            if (readstring != "")
            {
               
                dataSave = JsonUtility.FromJson<SaveObject>(readstring);
              //  JsonUtility.FromJsonOverwrite(readstring, dataSave);

            }
        }
        else
        {
            Debug.Log("Keine Datei vorhanden");
        }
    }
}
