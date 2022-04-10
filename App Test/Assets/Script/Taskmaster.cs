using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System;
using System.IO;


public class Taskmaster : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] SaveObject dataSave = new SaveObject();
    string directory = "/SavedData/";
    string filename = "SavedList.txt";

    private void Awake()
    {
        Taskmaster[] objs = FindObjectsOfType<Taskmaster>(); //Sigenton , Scenenwechesel l�scht es nicht 

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {

        loadlist();
        readtest();


    }
    public void create_newTask(string t, string d, int[] dt, float p)
    {
        Task new_task = new Task(t, d, dt, p);
        dataSave.addnewtoList(new_task);
        savelist();

        

    }
    private void savelist()
    {
        string dir = Application.persistentDataPath + directory;
        
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
  
        }
      

        string saveJason = JsonUtility.ToJson(dataSave);
        
        File.WriteAllText(dir + filename, saveJason);


    }
    private void loadlist()
    {
        string loadpath = Application.persistentDataPath + directory + filename;
        print(loadpath);

        if (File.Exists(loadpath))
        {
            print("data found");
            string readstring = File.ReadAllText(loadpath);
            if(readstring != "")
            {
                dataSave = JsonUtility.FromJson<SaveObject>(readstring);
            }
            
        }
        else
        {
            Debug.Log("Keine Datei vorhanden");
        }
        // tasklist = JsonUtility.FromJson<List<Task>>(json);
    }
    public void readtest()
    {
        foreach(Task i in dataSave.returnList())
        {
           print( i.getDeadline());
        }
        
    }
    
    //////////////////// Task
    
    [Serializable]
    public class Task
    {
        [SerializeField] string titel;
        [SerializeField] string description;
        [SerializeField] int[] deadline;  /// DateTime nicht so leicht serizaible | dewegen int[]
        
        [SerializeField] float prio;
        

        bool redo = false;
        bool done = false;
        bool sucess = false;

        public Task(string t, string d, int[] dt, float p)
        {
            titel = t;
            description = d;
            deadline = dt;
            prio = p;

          

            
        }

        public string Titel { get => titel; set => titel = value; }
        public string Description { get => description; set => description = value; }
        public string getDeadline()
        {
           
            return deadline.ToString();
        
        }
        public float Prio { get => prio; set => prio = value; }

        public void change(string t, string d, int[] dt, int p)
        {
            titel = t;
            description = d;
            deadline = dt;
            prio = p;
        }

    }

    /////// Save Object
    
   [Serializable]
   public class SaveObject
   {
        [SerializeField] List<Task> tasklist = new List<Task>();
        public void addnewtoList(Task addT)
        {
            tasklist.Add(addT);
        }
        public void removefromList()
        {
           
        }
        public List<Task> returnList()
        {
            return tasklist;
        }
    }



}
