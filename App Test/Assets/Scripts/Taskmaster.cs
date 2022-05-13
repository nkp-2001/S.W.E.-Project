using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

public class Taskmaster : MonoBehaviour, IObserver
{
    [SerializeField] SaveObject dataSave = new SaveObject();
    string directory = "/SavedData/";
    string filename = "SavedList.txt";
    [SerializeField] NotificationSystem notificationSystem; // nicht mehr nötig


    private void Awake()
    {
        Taskmaster[] objs = FindObjectsOfType<Taskmaster>(); //Singleton , Scenenwechesel loescht es nicht 

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

        LoadList();
    }
    private void Start()
    {
       // notificationSystem = FindObjectOfType<NotificationSystem>(); // nicht mehr nötig
       
        CheckDeadlinesTask();

      
    }

    private void OnApplicationFocus(bool focus) // vllt noch stattdessen anderes Call Event dafür benutzten
    {
        CheckDeadlinesTask();
    }
    public void CreateNewTask(string t, string d, int[] dt, float p)
    {
        t = AvoidDoubleName(t); // Namecheck  , keine Doppelten

        if (dt != null & dt.Length != 0)
        {
            print("year" + dt[4] + ",month" + dt[3] + ",day" + dt[2] + ",hour" + dt[1] + ",min" + dt[0]);

           // int id = notificationSystem.SendNewDeadlineNotificationsX(t, new DateTime(dt[4], dt[3], dt[2], dt[1], dt[0], 0));
            
            int id = Subject.current.Trigger_Request_NotiID(t, new DateTime(dt[4], dt[3], dt[2], dt[1], dt[0], 0));
            if (id != 0)
            {
                print("System plugged");
                Task new_task = new Task(t, d, dt, p, id);
                dataSave.AddNewToList(new_task);
            }
            else 
            {
                print("[ManuelWarning] The NotficationSystem is not plugged here. It is either not in the Scene anymore, wasen`t in it in the first place or onRequest_NotiID hasen`t been assigend by it");
                Task new_task = new Task(t, d, dt, p);
                dataSave.AddNewToList(new_task);
            } 
        }
        else
        {
            Task new_task = new Task(t, d, dt, p);
            dataSave.AddNewToList(new_task);
        }
        SaveList();
       // notificationSystem.NotficationStatusReaction(false);
    }

    public void RemoveTask(int index) 
    {
       
        dataSave.RemoveFromList(dataSave.GetList()[index].DeadlineChannel_ID);
        SaveList();
    }
    public void RemoveTask(Task tk)
    {

        //notificationSystem.CancelDeadlineNotificationsX(tk.DeadlineChannel_ID);

        //if (dataSave.RemoveFromList(tk) == 0)
        //{
        //    notificationSystem.NotficationStatusReaction(true);
        //}

        dataSave.RemoveFromList(tk);
        SaveList();
    }
    public void ChangeTask(Task oldtask, string t, string d, int[] dt, float p)
    {
        t = AvoidDoubleName(t); // Namecheck  , keine Doppelten

        if (dt == null)
        {
            dataSave.ChangeTask(oldtask, t, d, dt, p, 0); //0 = keine Meldungen , Cancel der Alten über das Event (im Notfi)
        }
        else if (oldtask.Deadline != dt)
        {
           // notificationSystem.CancelDeadlineNotificationsX(oldtask.DeadlineChannel_ID);
            int new_id = Subject.current.Trigger_Request_NotiID(t, new DateTime(dt[4], dt[3], dt[2], dt[1], dt[0], 0));
            if (new_id != 0)
            {
                dataSave.ChangeTask(oldtask, t, d, dt, p, new_id);
            }
            else
            {
                print("[ManuelWarning] The NotficationSystem is not plugged here. It is either not in the Scene anymore, wasen`t in it in the first place or onRequest_NotiID hasen`t been assigend by it");
                dataSave.ChangeTask(oldtask, t, d, dt, p, oldtask.DeadlineChannel_ID);
            }
           
        }
        else
        {
            dataSave.ChangeTask(oldtask, t, d, dt, p, oldtask.DeadlineChannel_ID);
        }
        SaveList();
    }
    public List<Task> GetTasks()
    {
        return dataSave.GetList();
    }
    public int GetTaskListLenght()
    {
        return GetTasks().Count;
    }


    public List<Task> GetSortedTasks(int sortBy)
    {
        List<Task> unsort = dataSave.GetList();

        if (sortBy == 0)
        {
            return unsort.OrderBy(t => t.DeadlineChannel_ID)
                .ThenBy(t => t.Prio)
                .ThenByDescending(t => {
                    if (t.DeadlineChannel_ID == 0) return 0;
                    return new DateTimeOffset(new DateTime(t.Deadline[4], t.Deadline[3], t.Deadline[2], t.Deadline[1], t.Deadline[0], 0)).ToUnixTimeSeconds();
                })
                .ToList();
        }
        else if (sortBy == 1)
        {
            return unsort.OrderBy(t => t.DeadlineChannel_ID)
                .ThenByDescending(t => {
                    if (t.DeadlineChannel_ID == 0) return 0;
                    return new DateTimeOffset(new DateTime(t.Deadline[4], t.Deadline[3], t.Deadline[2], t.Deadline[1], t.Deadline[0], 0)).ToUnixTimeSeconds();
                })
                .ThenBy(t => t.Prio)
                .ToList();
        }
        else
        {
            return unsort.OrderByDescending(t => t.Titel)
                .ToList();
        }
    }
    public List<Task> GetArchivedTasks()
    {
        return dataSave.GetArchivedList();
    }

    private void SaveList()
    {
        string dir = Application.persistentDataPath + directory;

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);

        }


        string saveJason = JsonUtility.ToJson(dataSave);

        File.WriteAllText(dir + filename, saveJason);


    }
    private void LoadList()
    {
        string loadpath = Application.persistentDataPath + directory + filename;
        print(loadpath);

        if (File.Exists(loadpath))
        {
            string readstring = File.ReadAllText(loadpath);
            if (readstring != "")
            {
                dataSave = JsonUtility.FromJson<SaveObject>(readstring);
            }
        }
        else
        {
            Debug.Log("Keine Datei vorhanden");
        }
    }
    public void CheckDeadlinesTask()
    {
        foreach (Task t in (dataSave.GetList()).ToArray())
        {
            if (t.Deadline != null && t.Deadline.Length != 0)
            {
                if (System.DateTime.Now >= new DateTime(t.Deadline[4], t.Deadline[3], t.Deadline[2], t.Deadline[1], t.Deadline[0], 0))
                {
                    print("Checkout");
                    t.Done = true;
                    RemoveTask(t);
                    Subject.current.Trigger_ExpiredDeadline();



                }
            }
        }

    }

    public void ManageTaskReturn(Taskmaster.Task oldtask, string t, string d, int[] dt, float prio)
    {
        dataSave.RemoveFromArchieList(oldtask);
        CreateNewTask(t, d, dt, prio);
    }

    public string AvoidDoubleName(string titel)
    {
        string checkedtitel = titel;
        bool doublefound = true;
        int repeating = 0;
        while (doublefound)
        {
            
            doublefound = false;
            print("Round" + repeating);
            foreach (Task task in dataSave.GetList())
            {
                if (checkedtitel == task.Titel)
                {
                    print("double");

                    repeating++;
                    checkedtitel = titel + "(" + repeating + ")";
                    doublefound = true;
                    break;
                }
            }

        }
        return checkedtitel;
       
    }

    public void SubscribeToEvents_Start()
    {
        Subject.current.OnTaskSetDone += RemoveTask;
        Subject.current.OnNewTask += CreateNewTask;
        Subject.current.OnTaskChange += ChangeTask;
        Subject.current.OnTaskReturning += ManageTaskReturn;


    }

    public void UnsubscribeToAllEvents()
    {
        Subject.current.OnTaskSetDone -= RemoveTask;
        Subject.current.OnNewTask -= CreateNewTask;
        Subject.current.OnTaskChange -= ChangeTask;
        Subject.current.OnTaskReturning -= ManageTaskReturn;
    }
    
    private void OnDestroy()
    {
        UnsubscribeToAllEvents();
        print("2xxxx");

    }
    private void OnEnable()
    {
        SubscribeToEvents_Start();
        
        Debug.Log("OnEnable");
    }
  




    ///////////// Task ////////////
    [Serializable]
    public class Task
    {
        [SerializeField] string titel;
        [SerializeField] string description;
        [SerializeField] int[] deadline;  /// DateTime nicht (so leicht) serizaible | dewegen muss auf int[] ausgeweischt werden

        [SerializeField] int deadlineChannel_ID = 0;
        [SerializeField] float prio;

        [SerializeField] bool redo = false;

        [SerializeField] int nextDeadlineIndex = 0;
        [SerializeField] public int[] nextDeadline = null;

        [SerializeField] bool sucess = false;
        [SerializeField] bool done = false;
       

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
        public Task(string t, string d, int[] dt, float p, int dlID,int retDtDayes)
        {
            titel = t;
            description = d;
            deadline = dt;
            prio = p;
            deadlineChannel_ID = dlID;

            nextDeadlineIndex = retDtDayes;
        }



        public string Titel { get => titel; set => titel = value; }
        public string Description { get => description; set => description = value; }
        public float Prio { get => prio; set => prio = value; }
        public int[] Deadline { get => deadline; set => deadline = value; }
        public int DeadlineChannel_ID { get => deadlineChannel_ID; set => deadlineChannel_ID = value; }
        public bool Done { get => done; set => done = value; }
        public bool Redo { get => redo; set => redo = value; }
        public bool Sucess { get => sucess; set => sucess = value; }
        public int NextDeadlineIndex { get => nextDeadlineIndex; set => nextDeadlineIndex = value; }
    }

    /////// Save Object
    
   [Serializable]
   public class SaveObject
   {
        [SerializeField] List<Task> tasklist = new List<Task>();
        [SerializeField] List<Task> archivedTasks = new List<Task>();
        public void AddNewToList(Task addT)
        {
            tasklist.Add(addT);
        }
        public int RemoveFromList(int i)
        {
            tasklist.RemoveAt(i);
            return tasklist.Count;
        }
        public int RemoveFromListAndGiveCount(Task tk) // veraltet 
        {
            tasklist.Remove(tk);
            return tasklist.Count;
        }
        public void RemoveFromList(Task tk)
        {
            tasklist.Remove(tk);
            if(tk.Redo == true && tk.Sucess == true)
            {
                tk.nextDeadline = CaculuateNextDT(tk.NextDeadlineIndex,tk.Deadline);

            }
            archivedTasks.Add(tk);
        }

        private int[] CaculuateNextDT(int nextDeadlineIndex, int[] currentDeadline)
        {
            DateTime dateTime = new DateTime(currentDeadline[4], currentDeadline[3], currentDeadline[2], currentDeadline[1], currentDeadline[0], 0);
            switch (nextDeadlineIndex)
            {
                case 1:
                    dateTime = dateTime.AddDays(1);
                    break;
                case 2:
                    dateTime = dateTime.AddDays(7);
                    break;
                case 3:
                    dateTime = dateTime.AddMonths(1);
                    break;
                case 4:
                    dateTime = dateTime.AddYears(1);
                    break;
            }
            return new int[] { dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute };
        }

        public void RemoveFromArchieList(Task tk)
        {
            archivedTasks.Remove(tk);
        }
        public List<Task> GetList()
        {
            return tasklist;
        }
        public List<Task> GetArchivedList()
        {
            return archivedTasks;
        }
        public void ClearArchviedList()
        {
            archivedTasks.Clear();
        }
        public void ChangeTask(Task altertT, string t, string d, int[] dt, float p,int id)
        {
            int index = tasklist.FindLastIndex(task => task.Titel == altertT.Titel); //Kann nur klappen wenn allles Unterschidlich heißt anpassen!!!
            print(index);
            tasklist[index] = new Task(t, d, dt, p,id);     
        }        
   }
}
