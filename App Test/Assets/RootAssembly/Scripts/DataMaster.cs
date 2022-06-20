using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

public class Datamaster : MonoBehaviour, IObserver
{
    IDataMasterNOSClient clientNotificationSystem;
    [SerializeField] SaveObject dataSave = new SaveObject();
    string directoryname = "/SavedData/";
    string filename = "SavedList.txt";

    public string Directoryname { get => directoryname; set => directoryname = value; }
    public string Filename { get => filename; set => filename = value; }

    private void Awake()
    {
        Datamaster[] objs = FindObjectsOfType<Datamaster>(); //Scenenwechesel loescht es nicht 

        if (objs.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        LoadList();

    }
    private void Start()
    {
        CheckDeadlinesTask();
        SubscribeToEvents_Start();
    }

    private void OnApplicationFocus(bool focus) 
    {
        CheckDeadlinesTask();
    }
    public void CreateNewTask(string t, string d, int[] dt, float p,int repeatindex)
    {
        t = AvoidDoubleName(t); 
        if (dt != null)
        {
           if (dt.Length != 0) 
           {
               
                if (clientNotificationSystem != null) 
                {
                    int id = clientNotificationSystem.SendNewDeadlineNotifications(t, ConvertIntArrayToDatetime(dt));
                    Task new_task = new Task(t, d, dt, p, id, repeatindex);
                    dataSave.AddNewToList(new_task);
                }
                else
                {
                    print("[ManuelWarning] The NotficationSystem is not plugged");
                    Task new_task = new Task(t, d, dt, p);
                    dataSave.AddNewToList(new_task);
                }
                SaveList();
                return;
           }
        }      
        Task new_t = new Task(t, d, dt, p);
        dataSave.AddNewToList(new_t);     
        SaveList();
    }

    public void RemoveTask(int index) 
    {       
        dataSave.RemoveFromList(dataSave.GetList()[index].DeadlineChannel_ID);
        SaveList();
    }
    public void RemoveTask(Task tk)
    {
        dataSave.RemoveFromList(tk);
        SaveList();
    }

    public void ChangeTask(Task oldtask, string t, string d, int[] dt, float p,int rindex)
    {
        if (t != oldtask.Titel)
        {
            t = AvoidDoubleName(t);

        }
        if (dt == null)
        {
            dataSave.ChangeTask(oldtask, t, d, dt, p, 0,0); 
        }
        else if (oldtask.Deadline != dt)
        {
            if (clientNotificationSystem != null) 
            {
                int new_id = clientNotificationSystem.SendNewDeadlineNotifications(t, ConvertIntArrayToDatetime(dt));
                dataSave.ChangeTask(oldtask, t, d, dt, p, new_id,rindex);
            }
            else
            {
                print("[ManuelWarning] The NotficationSystem is not plugged here.");
                dataSave.ChangeTask(oldtask, t, d, dt, p, oldtask.DeadlineChannel_ID, rindex);
            }         
        }
        else
        {
            dataSave.ChangeTask(oldtask, t, d, dt, p, oldtask.DeadlineChannel_ID,rindex);
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
                    return new DateTimeOffset(ConvertIntArrayToDatetime(t.Deadline)).ToUnixTimeSeconds();
                })
                .ToList();
        }
        else if (sortBy == 1)
        {
            return unsort.OrderBy(t => t.DeadlineChannel_ID)
                .ThenByDescending(t => {
                    if (t.DeadlineChannel_ID == 0) return 0;
                    return new DateTimeOffset(ConvertIntArrayToDatetime(t.Deadline)).ToUnixTimeSeconds();
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
        string dir = Application.persistentDataPath + directoryname;

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);

        }


        string saveJason = JsonUtility.ToJson(dataSave);

        File.WriteAllText(dir + filename, saveJason);
    }
    private void LoadList()
    {
        string loadpath = Application.persistentDataPath + directoryname + filename;
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
            print("Keine Datei vorhanden");
        }
    }
    public void CheckDeadlinesTask()
    {
        foreach (Task t in (dataSave.GetList()).ToArray())
        {
            if (t.Deadline != null && t.Deadline.Length != 0)
            {
                if (System.DateTime.Now >= ConvertIntArrayToDatetime(t.Deadline))
                {
                    if (t.NextDeadlineIndex != 0)
                    {
                         int[] NextDeadline = CaculuateNextDT(t.NextDeadlineIndex, t.Deadline);
                         int NextDeadlineNotifId = clientNotificationSystem.SendNewDeadlineNotifications(t.Titel, ConvertIntArrayToDatetime(NextDeadline)); // ex: Subject.current.Trigger_Request_NotiID
                         dataSave.ChangeTaskCauseRepeat(t, NextDeadline,NextDeadlineNotifId) ;
                         SaveList();

                    }
                    else
                    {
                        RemoveTask(t);
                    }
                    Subject.current.Trigger_ExpiredDeadline();
                }
            }
        }
        foreach (Task t in (dataSave.GetWaitingList()).ToArray())
        {
            if (System.DateTime.Now >= ConvertIntArrayToDatetime(t.Deadline))
            {
                dataSave.RemoveFromWaitList(t); 
                t.Deadline = CaculuateNextDT(t.NextDeadlineIndex, t.Deadline);               
                t.DeadlineChannel_ID = clientNotificationSystem.SendNewDeadlineNotifications(t.Titel, ConvertIntArrayToDatetime(t.Deadline));
                dataSave.AddNewToList(t);
                Subject.current.Trigger_ExpiredDeadline();
            }
        }
       



    }
    public static System.DateTime ConvertIntArrayToDatetime(int[] toconvert)
    {
        return new DateTime(toconvert[4], toconvert[3], toconvert[2], toconvert[1], toconvert[0], 0);
    }
    public static int[] ConvertDatetimeToIntArray(DateTime toconvert)
    {
        return new int[] { toconvert.Minute, toconvert.Hour, toconvert.Day, toconvert.Month, toconvert.Year };
    }

    public void ManageTaskReturn(Task oldtask, string t, string d, int[] dt, float prio,int repeatindex)
    {
        dataSave.RemoveFromArchiveList(oldtask);
        CreateNewTask(t, d, dt, prio,repeatindex);     
    }

    public string AvoidDoubleName(string titel)
    {
        string checkedtitel = titel;
        bool doublefound = true;
        int repeating = 0;
        while (doublefound)
        {
            
            doublefound = false;
            foreach (Task task in dataSave.GetList().Concat(dataSave.GetWaitingList()))
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
    public int[] CaculuateNextDT(int nextDeadlineIndex, int[] currentDeadline) // nur public damit sie testbar ist
    {
        DateTime dateTime = ConvertIntArrayToDatetime(currentDeadline);
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
        return new int[] { dateTime.Minute, dateTime.Hour, dateTime.Day, dateTime.Month, dateTime.Year };
    }
    public void CreateNewAppointment(string titel, string desp, int[] startTime, int[] endTime, int repeat, int repeatTimes,int[] preW)
    {
        titel = AvoidDoubleNameAppointment(titel);
        int notficID = 0;
        if (clientNotificationSystem != null)
        {
            notficID = clientNotificationSystem.SendAppointmentNotifcations(ConvertIntArrayToDatetime(startTime), ConvertIntArrayToDatetime(endTime), repeat, titel,repeatTimes,preW);
        }
        else
        {
            print("[ManuelWarning] The NotficationSystem is not plugged");
        }
        dataSave.AddNewAppointment(new Appointment(titel, desp, startTime, endTime, repeat, notficID, repeatTimes));
        SaveList();
    }

    public void DeleteAppoitment(Appointment appo)
    {
        dataSave.RemoveAppointment(appo);
        SaveList();
    }

    public void ChangeAppointment(Appointment oldAppointment, string titel, string desp, int[] startTime, int[] endTime, int repeat, int repeatTimes, int[] preW)
    {
        if (titel != oldAppointment.Title)
        {
            titel = AvoidDoubleNameAppointment(titel);
        }


        if (oldAppointment.StartTime != startTime | oldAppointment.EndTime != endTime)
        {
            if (clientNotificationSystem != null)
            {
                int newnotficID = clientNotificationSystem.SendAppointmentNotifcations(ConvertIntArrayToDatetime(startTime), ConvertIntArrayToDatetime(endTime), repeat, titel, repeatTimes, preW);
                dataSave.ChangeAppointment(oldAppointment, titel, desp, startTime, endTime, repeat, newnotficID, repeatTimes);
            }
            else
            {
                print("[ManuelWarning] The NotficationSystem is not plugged");
                dataSave.ChangeAppointment(oldAppointment, titel, desp, startTime, endTime, repeat, 0, repeatTimes);
            }
            
        }
        else
        {
            dataSave.ChangeAppointment(oldAppointment, titel, desp, startTime, endTime, repeat, oldAppointment.Notifcation_id, repeatTimes);
        }
        SaveList();
    }

    public string AvoidDoubleNameAppointment(string titel)  
    {
        string checkedtitel = titel;
        bool doublefound = true;
        int repeating = 0;
        while (doublefound)
        {

            doublefound = false;
            print("Round" + repeating);
            foreach (Appointment appo in dataSave.GetAppointmentList())
            {
                if (checkedtitel == appo.Title)
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

    public List<Appointment> GiveAppointsOfThisDay(DateTime askedday)
    {
        List<Appointment> DayList = new List<Appointment>();
        foreach (Appointment appo in dataSave.GetAppointmentList())
        {
            if (appo.AppointmentOnThisDay(askedday))
            {
                DayList.Add(appo);
            }
        }
        return DayList;
    }
    public void Reload() // für Uni Test 
    {
        LoadList();
    }
    public void WipeSave()
    {
        dataSave = new SaveObject();
        SaveList();
    }
 

    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void SubscribeToEvents_Start()
    {
        Subject.current.OnTaskSetDone += RemoveTask;
        Subject.current.OnNewTask += CreateNewTask;
        Subject.current.OnTaskChange += ChangeTask;
        Subject.current.OnTaskReturning += ManageTaskReturn;

        Subject.current.OnNewAppointment += CreateNewAppointment;
        Subject.current.OnAppointmentChange += ChangeAppointment;
        Subject.current.OnDeleteAppointment += DeleteAppoitment;
    }

    public void UnsubscribeToAllEvents()
    {
        Subject.current.OnTaskSetDone -= RemoveTask;
        Subject.current.OnNewTask -= CreateNewTask;
        Subject.current.OnTaskChange -= ChangeTask;
        Subject.current.OnTaskReturning -= ManageTaskReturn;

        Subject.current.OnNewAppointment -= CreateNewAppointment;
        Subject.current.OnAppointmentChange -= ChangeAppointment;
        Subject.current.OnDeleteAppointment -= DeleteAppoitment;
    }
    
    private void OnDestroy()
    {
        UnsubscribeToAllEvents();
    }

    public void SetNotificatioSystem(IDataMasterNOSClient notS) // Muss von Außen getsezt werden , SerzliedFlied ist beim Interface nicht möglich
    { 
        clientNotificationSystem = notS;
    }




}
