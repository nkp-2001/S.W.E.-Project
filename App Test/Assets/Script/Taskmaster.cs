using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System;
using System.IO;

public class Taskmaster : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<Task> tasklist;
    string directory = "/SavedData/";
    string filename = "SavedList";

    private void Awake()
    {
        Taskmaster[] objs = FindObjectsOfType<Taskmaster>(); //Sigenton , Scenenwechesel löscht es nicht 

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {

        loadlist();


    }
    public void create_newTask(string t, string d, DateTime dt, float p)
    {
        Task new_task = new Task(t, d, dt, p);
        tasklist.Add(new_task);
        savelist();


    }
    private void savelist()
    {
        string dir = Application.persistentDataPath + directory;
        
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
  
        }
        if (!File.Exists(dir + filename))
        {
            File.Create(dir + filename);
        }
        string saveJason = JsonUtility.ToJson(tasklist);
        File.WriteAllText(dir + filename, saveJason);


    }
    private void loadlist()
    {
        string loadpath = Application.persistentDataPath + directory + filename;

        if (File.Exists(loadpath))
        {
            tasklist = JsonUtility.FromJson<List<Task>>(loadpath);
        }
        else
        {
            Debug.Log("Keine Datei vorhanden");
        }
        // tasklist = JsonUtility.FromJson<List<Task>>(json);
    }
    
    /// ////////////////
    
    [Serializable]
    public class Task
    {
        [SerializeField] string titel;
        [SerializeField] string description;
        [SerializeField] DateTime deadline;
        [SerializeField] float prio;
        

        bool redo = false;
        bool done = false;
        bool sucess = false;

        public Task(string t, string d, DateTime dt, float p)
        {
            titel = t;
            description = d;
            deadline = dt;
            prio = p;
        }

        public string Titel { get => titel; set => titel = value; }
        public string Description { get => description; set => description = value; }
        

        public void change(string t, string d, DateTime dt, int p)
        {
            titel = t;
            description = d;
            deadline = dt;
            prio = p;
        }

    }
    


}
