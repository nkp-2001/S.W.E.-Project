using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataMaster : MonoBehaviour, IObserver
{
    private IDataMasterNOSClient clientNotificationSystem;
    [SerializeField] private SaveObject dataSave = new();
    private string directoryName = "/SavedData/";
    private string fileName = "SavedList.txt";

    public string DirectoryName { get => directoryName; set => directoryName = value; }
    public string FileName { get => fileName; set => fileName = value; }

    private void Awake()
    {
        DataMaster[] objs = FindObjectsOfType<DataMaster>(); //Scenenwechesel loescht es nicht 

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
        SubscribeToEvents();
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
                    int id = clientNotificationSystem.SendNewDeadlineNotifications(t, ConvertIntArrayToDateTime(dt));
                    Task new_task = new(t, d, dt, p, id, repeatindex);
                    dataSave.AddNewToList(new_task);
                }
                else
                {
                    print("[Warning] The NotficationSystem is not plugged");
                    Task new_task = new(t, d, dt, p);
                    dataSave.AddNewToList(new_task);
                }
                SaveList();
                return;
           }
        }      
        Task new_t = new(t, d, dt, p);
        dataSave.AddNewToList(new_t);     
        SaveList();
    }

    public void RemoveTask(int index) 
    {       
        dataSave.RemoveFromList(dataSave.GetList()[index].DeadlineChannelId);
        SaveList();
    }

    public void RemoveTask(Task tk)
    {
        dataSave.RemoveFromList(tk);
        SaveList();
    }

    public void ChangeTask(Task oldtask, string t, string d, int[] dt, float p,int rindex)
    {
        if (t != oldtask.Title)
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
                int new_id = clientNotificationSystem.SendNewDeadlineNotifications(t, ConvertIntArrayToDateTime(dt));
                dataSave.ChangeTask(oldtask, t, d, dt, p, new_id,rindex);
            }
            else
            {
                print("[Warning] The NotficationSystem is not plugged here.");
                dataSave.ChangeTask(oldtask, t, d, dt, p, oldtask.DeadlineChannelId, rindex);
            }         
        }
        else
        {
            dataSave.ChangeTask(oldtask, t, d, dt, p, oldtask.DeadlineChannelId,rindex);
        }
        SaveList();
    }

    public List<Task> GetTasks()
    {
        return dataSave.GetList();
    }

    public int GetTaskListLength()
    {
        return GetTasks().Count;
    }

    public List<Task> GetSortedTasks(int sortBy)
    {
        List<Task> unsort = dataSave.GetList();

        if (sortBy == 0)
        {
            return unsort.OrderBy(t => t.DeadlineChannelId)
                .ThenBy(t => t.Priority)
                .ThenByDescending(t => {
                    if (t.DeadlineChannelId == 0) return 0;
                    return new DateTimeOffset(ConvertIntArrayToDateTime(t.Deadline)).ToUnixTimeSeconds();
                })
                .ToList();
        }
        else if (sortBy == 1)
        {
            return unsort.OrderBy(t => t.DeadlineChannelId)
                .ThenByDescending(t => {
                    if (t.DeadlineChannelId == 0) return 0;
                    return new DateTimeOffset(ConvertIntArrayToDateTime(t.Deadline)).ToUnixTimeSeconds();
                })
                .ThenBy(t => t.Priority)
                .ToList();
        }
        else
        {
            return unsort.OrderByDescending(t => t.Title)
                .ToList();
        }
    }

    public List<Task> GetArchivedTasks()
    {
        return dataSave.GetArchivedList();
    }

    private void SaveList()
    {
        string dir = Application.persistentDataPath + directoryName;

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        string saveJason = JsonUtility.ToJson(dataSave);

        File.WriteAllText(dir + fileName, saveJason);
    }

    private void LoadList()
    {
        string loadpath = Application.persistentDataPath + directoryName + fileName;
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
            print("No file found");
        }
    }
    public void CheckDeadlinesTask()
    {
        foreach (Task t in (dataSave.GetList()).ToArray())
        {
            if (t.Deadline != null && t.Deadline.Length != 0)
            {
                if (DateTime.Now >= ConvertIntArrayToDateTime(t.Deadline))
                {
                    if (t.NextDeadlineIndex != 0)
                    {
                         int[] NextDeadline = CalculateNextDT(t.NextDeadlineIndex, t.Deadline);
                         int NextDeadlineNotifId = clientNotificationSystem.SendNewDeadlineNotifications(t.Title, ConvertIntArrayToDateTime(NextDeadline)); // ex: Subject.Trigger_Request_NotiID
                         dataSave.ChangeTaskCauseRepeat(t, NextDeadline,NextDeadlineNotifId) ;
                         SaveList();
                    }
                    else
                    {
                        RemoveTask(t);
                    }
                    Subject.Trigger_ExpiredDeadline();
                }
            }
        }

        foreach (Task t in (dataSave.GetWaitingList()).ToArray())
        {
            if (DateTime.Now >= ConvertIntArrayToDateTime(t.Deadline))
            {
                dataSave.RemoveFromWaitList(t); 
                t.Deadline = CalculateNextDT(t.NextDeadlineIndex, t.Deadline);               
                t.DeadlineChannelId = clientNotificationSystem.SendNewDeadlineNotifications(t.Title, ConvertIntArrayToDateTime(t.Deadline));
                dataSave.AddNewToList(t);
                Subject.Trigger_ExpiredDeadline();
            }
        }
    }

    public static DateTime ConvertIntArrayToDateTime(int[] toconvert)
    {
        return new DateTime(toconvert[4], toconvert[3], toconvert[2], toconvert[1], toconvert[0], 0);
    }

    public static int[] ConvertDateTimeToIntArray(DateTime toconvert)
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
                if (checkedtitel == task.Title)
                {
                    repeating++;
                    checkedtitel = titel + "(" + repeating + ")";
                    doublefound = true;
                    break;
                }
            }

        }
        return checkedtitel;
    }

    public int[] CalculateNextDT(int nextDeadlineIndex, int[] currentDeadline) // nur public damit sie testbar ist
    {
        DateTime dateTime = ConvertIntArrayToDateTime(currentDeadline);
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
            notficID = clientNotificationSystem.SendAppointmentNotifcations(ConvertIntArrayToDateTime(startTime), ConvertIntArrayToDateTime(endTime), repeat, titel,repeatTimes,preW);
        }
        else
        {
            print("[Warning] The NotficationSystem is not plugged");
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
                int newnotficID = clientNotificationSystem.SendAppointmentNotifcations(ConvertIntArrayToDateTime(startTime), ConvertIntArrayToDateTime(endTime), repeat, titel, repeatTimes, preW);
                dataSave.ChangeAppointment(oldAppointment, titel, desp, startTime, endTime, repeat, newnotficID, repeatTimes);
            }
            else
            {
                print("[Warning] The NotficationSystem is not plugged");
                dataSave.ChangeAppointment(oldAppointment, titel, desp, startTime, endTime, repeat, 0, repeatTimes);
            }
            
        }
        else
        {
            dataSave.ChangeAppointment(oldAppointment, titel, desp, startTime, endTime, repeat, oldAppointment.NotificationId, repeatTimes);
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

            foreach (Appointment appo in dataSave.GetAppointmentList())
            {
                if (checkedtitel == appo.Title)
                {
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
        List<Appointment> DayList = new();

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

    public void SubscribeToEvents()
    {
        Subject.OnTaskSetDone += RemoveTask;
        Subject.OnNewTask += CreateNewTask;
        Subject.OnTaskChange += ChangeTask;
        Subject.OnTaskReturning += ManageTaskReturn;

        Subject.OnNewAppointment += CreateNewAppointment;
        Subject.OnAppointmentChange += ChangeAppointment;
        Subject.OnDeleteAppointment += DeleteAppoitment;
    }

    public void UnsubscribeToAllEvents()
    {
        Subject.OnTaskSetDone -= RemoveTask;
        Subject.OnNewTask -= CreateNewTask;
        Subject.OnTaskChange -= ChangeTask;
        Subject.OnTaskReturning -= ManageTaskReturn;

        Subject.OnNewAppointment -= CreateNewAppointment;
        Subject.OnAppointmentChange -= ChangeAppointment;
        Subject.OnDeleteAppointment -= DeleteAppoitment;
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
