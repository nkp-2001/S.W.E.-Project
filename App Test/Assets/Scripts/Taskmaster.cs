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
    [SerializeField] NotificationSystem NotiSy;


    private void Awake() 
    {
        Taskmaster[] objs = FindObjectsOfType<Taskmaster>(); //Sigenton , Scenenwechesel löscht es nicht 

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        loadlist();
    }
    private void Start()
    {
        NotiSy = FindObjectOfType<NotificationSystem>();

        CheckDeadlinesTask();


    }
    private void OnApplicationFocus(bool focus) // vllt noch stattdessen anderes Call Event dafür benutzten
    {
        CheckDeadlinesTask();
    }
    public void create_newTask(string t, string d, int[] dt, float p)
    {
        if (dt != null)
        {
            print("year" + dt[4] + ",month" + dt[3] + ",day" + dt[2] + ",hour" + dt[1] + ",min" + dt[0]);

            int id = NotiSy.SendNewDeadlineNotificationsX(t, new DateTime(dt[4], dt[3], dt[2],dt[1],dt[0],0));
            Task new_task = new Task(t, d, dt, p,id);
            dataSave.addnewtoList(new_task);
        }
        else
        {
            Task new_task = new Task(t, d, dt, p);
            dataSave.addnewtoList(new_task);
        }  
        savelist();
        NotiSy.NotficationStatusReaction(false);
    }
    public void removeTask(int index)
    {
        /*
         foreach (int i in dataSave.returnList()[index].DeadlineIDs)
         {
             NotiSy.CanelDeadlineNotifctions(i);
         }
        */ ///////////////////////////

        NotiSy.CanelDeadlineNotifctionsX(dataSave.returnList()[index].DeadlineChannel_ID);

        ////////////////////////////

        if (dataSave.removefromList(index) == 0)
        {
            NotiSy.NotficationStatusReaction(true);
        }
        savelist();
    }
    public void removeTask(Task tk)
    {
        /* foreach (int i in tk.)
          {
             NotiSy.CanelDeadlineNotifctions(i);
          }
        */ //////////////////////////

        NotiSy.CanelDeadlineNotifctionsX(tk.DeadlineChannel_ID);

        ////////////////////////////

        if (dataSave.removefromList(tk) == 0)
        {
            NotiSy.NotficationStatusReaction(true);
        }
        savelist();
    }
    public void removeall() // nur zum Testen sollte später entfernt werden
    {
        dataSave.removeall();
        NotiSy.NotficationStatusReaction(true);
        savelist();
    }

    public List<Task> GetTasks()
    {
        return dataSave.returnList();
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
    public void CheckDeadlinesTask()
    {
            foreach (Task t in (dataSave.returnList()).ToArray())
            {
                if (t.Deadline != null && t.Deadline.Length != 0)
            {
                    
                    //
                    if (System.DateTime.Now >= new DateTime(t.Deadline[4], t.Deadline[3], t.Deadline[2], t.Deadline[1], t.Deadline[0], 0))
                    {
                        print("Checkout");
                        removeTask(t);
                        FindObjectOfType<ToDoPageController>().FetchTasks(); // Refernez Sache anpssen Scenenwechesl etc beachten 

                    }
                }
            }
        
    }
  

    ///////////// Task ////////////
    [Serializable]
    public class Task
    {
        [SerializeField] string titel;
        [SerializeField] string description;
        [SerializeField] int[] deadline;  /// DateTime nicht so leicht serizaible | dewegen int[]

        [SerializeField] int deadlineChannel_ID = 0;
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
        public Task(string t, string d, int[] dt, float p,int dlID)
        {
            titel = t;
            description = d;
            deadline = dt;
            prio = p;
            deadlineChannel_ID = dlID;
        }

        public string Titel { get => titel; set => titel = value; }
        public string Description { get => description; set => description = value; }
        public float Prio { get => prio; set => prio = value; }
        public int[] Deadline { get => deadline; set => deadline = value; }
        public int DeadlineChannel_ID { get => deadlineChannel_ID; set => deadlineChannel_ID = value; }
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
        public int removefromList(int i)
        {
            tasklist.RemoveAt(i);
            return tasklist.Count;
        }
        public int removefromList(Task tk)
        {
            tasklist.Remove(tk);
            return tasklist.Count;
        }
        public List<Task> returnList()
        {
            return tasklist;
        }

        // Test funktion
        public void removeall()
        {
            tasklist.Clear();
        }
        
   }



}
