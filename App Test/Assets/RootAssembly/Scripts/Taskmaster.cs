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
    IDataMasterNOSClient clientNotificationSystem; // entfernen bei Vererbung
    [SerializeField] SaveObject dataSave = new SaveObject();
    string directoryname = "/SavedData/";
    string filename = "SavedList.txt";

    public string Directoryname { get => directoryname; set => directoryname = value; }
    public string Filename { get => filename; set => filename = value; }

    private void Awake()
    {
        Taskmaster[] objs = FindObjectsOfType<Taskmaster>(); //Scenenwechesel loescht es nicht 

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

    private void OnApplicationFocus(bool focus) // vllt noch stattdessen anderes Call Event dafür benutzten
    {
        CheckDeadlinesTask();
    }
    public void CreateNewTask(string t, string d, int[] dt, float p,int repeatindex)
    {
        t = AvoidDoubleName(t); // Namecheck  , keine Doppelten
        if (dt != null)
        {
           if (dt.Length != 0) // noch mal drüber gucken warum manchmal lengt 0 aber nicht null
           {
                print("year" + dt[4] + ",month" + dt[3] + ",day" + dt[2] + ",hour" + dt[1] + ",min" + dt[0]);
                // int id = notificationSystem.SendNewDeadlineNotificationsX(t, new DateTime(dt[4], dt[3], dt[2], dt[1], dt[0], 0));           
                //int id = Subject.current.Trigger_Request_NotiID(t, new DateTime(dt[4], dt[3], dt[2], dt[1], dt[0], 0));
                if (clientNotificationSystem != null) //(id != 0 )
                {
                    print("System plugged");
                    int id = clientNotificationSystem.SendNewDeadlineNotifications(t, new DateTime(dt[4], dt[3], dt[2], dt[1], dt[0], 0));
                    Task new_task = new Task(t, d, dt, p, id, repeatindex);
                    dataSave.AddNewToList(new_task);
                }
                else
                {
                    print("[ManuelWarning] The NotficationSystem is not plugged");
                    Task new_task = new Task(t, d, dt, p);
                    dataSave.AddNewToList(new_task);
                }
           }
        }
        else
        {
            Task new_task = new Task(t, d, dt, p);
            dataSave.AddNewToList(new_task);
        }
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
            t = AvoidDoubleName(t);// Namecheck  , keine Doppelten

        }
        if (dt == null)
        {
            dataSave.ChangeTask(oldtask, t, d, dt, p, 0,0); //0 = keine Meldungen , Cancel der Alten über das Event (im Notfi)
        }
        else if (oldtask.Deadline != dt)
        {
            // notificationSystem.CancelDeadlineNotificationsX(oldtask.DeadlineChannel_ID);
            // int new_id = Subject.current.Trigger_Request_NotiID(t, new DateTime(dt[4], dt[3], dt[2], dt[1], dt[0], 0));
           

            if (clientNotificationSystem != null) //(id != 0 )
            {
                int new_id = clientNotificationSystem.SendNewDeadlineNotifications(t, new DateTime(dt[4], dt[3], dt[2], dt[1], dt[0], 0));
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
                    if (t.NextDeadlineIndex != 0)
                    {
                         int[] NextDeadline = CaculuateNextDT(t.NextDeadlineIndex, t.Deadline);
                         int NextDeadlineNotifId = clientNotificationSystem.SendNewDeadlineNotifications(t.Titel, ConvertIntArray_toDatetime(NextDeadline)); // ex: Subject.current.Trigger_Request_NotiID
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
            if (System.DateTime.Now >= new DateTime(t.Deadline[4], t.Deadline[3], t.Deadline[2], t.Deadline[1], t.Deadline[0], 0))
            {
                dataSave.RemoveFromWaitList(t); // Änd.
                t.Deadline = CaculuateNextDT(t.NextDeadlineIndex, t.Deadline);
               // t.DeadlineChannel_ID = Subject.current.Trigger_Request_NotiID(t.Titel, new DateTime(t.Deadline[4], t.Deadline[3], t.Deadline[2], t.Deadline[1], t.Deadline[0], 0));
                t.DeadlineChannel_ID = clientNotificationSystem.SendNewDeadlineNotifications(t.Titel, new DateTime(t.Deadline[4], t.Deadline[3], t.Deadline[2], t.Deadline[1], t.Deadline[0], 0));
                dataSave.AddNewToList(t);
                Subject.current.Trigger_ExpiredDeadline();
            }
        }
       



    }

    public System.DateTime ConvertIntArray_toDatetime(int[] toconvert)
    {
        return new DateTime(toconvert[4], toconvert[3], toconvert[2], toconvert[1], toconvert[0], 0);
    }
    public int[] ConvertDatetime_toIntArray(DateTime toconvert)
    {
        return new int[] { toconvert.Minute, toconvert.Hour, toconvert.Day, toconvert.Month, toconvert.Year };
    }

    public void ManageTaskReturn(Task oldtask, string t, string d, int[] dt, float prio,int repeatindex)
    {
        dataSave.RemoveFromArchieList(oldtask);
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
            print("Round" + repeating);
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
        return new int[] { dateTime.Minute, dateTime.Hour, dateTime.Day, dateTime.Month, dateTime.Year };
    }

    ///!!!! ////////////////////////////////////////////////////////////////////////////////// Appointmenmts Vererbansatz sollte diskutiert werden , dobbelter SaveObject Problem dabei diskutiren (siehe DataMaster.cs)

    public void CreateNewAppointment(string titel, string desp, int[] startTime, int[] endTime, int repeat, int repeatTimes,int[] preW)
    {
        titel = AvoidDoubleNameAppo(titel);
        int notficID = 0;
        if (clientNotificationSystem != null)
        {
            notficID = clientNotificationSystem.SendAppointmentNotifcations(ConvertIntArray_toDatetime(startTime), ConvertIntArray_toDatetime(endTime), repeat, titel,repeatTimes,preW);
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

    public void ChangeAppointment(Appointment oldAppointment, string titel, string desp, int[] startTime, int[] endTime, int repeat, int notficID, int repeatTimes,int[] preW)
    {
        if (oldAppointment.StartTime != startTime | oldAppointment.EndTime != endTime)
        {
            if (clientNotificationSystem != null)
            {
                int newnotficID = clientNotificationSystem.SendAppointmentNotifcations(ConvertIntArray_toDatetime(startTime), ConvertIntArray_toDatetime(endTime), repeat, titel, repeatTimes, preW);
                dataSave.ChangeAppointment(oldAppointment, AvoidDoubleNameAppo(titel), desp, startTime, endTime, repeat, newnotficID, repeatTimes);
            }
            else
            {
                print("[ManuelWarning] The NotficationSystem is not plugged");
                dataSave.ChangeAppointment(oldAppointment, AvoidDoubleNameAppo(titel), desp, startTime, endTime, repeat, 0, repeatTimes);
            }
            
        }
        else
        {
            dataSave.ChangeAppointment(oldAppointment, AvoidDoubleNameAppo(titel), desp, startTime, endTime, repeat, oldAppointment.Notifcation_id, repeatTimes);
        }
        SaveList();
    }

    public string AvoidDoubleNameAppo(string titel)  
    {
        string checkedtitel = titel;
        bool doublefound = true;
        int repeating = 0;
        while (doublefound)
        {

            doublefound = false;
            print("Round" + repeating);
            foreach (Appointment appo in dataSave.GetAppoitmentList())
            {
                if (checkedtitel == appo.Titel)
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

    public List<Appointment> GiveAppoints_ofThisDay(DateTime askedday)
    {
        List<Appointment> DayList = new List<Appointment>();
        foreach (Appointment appo in dataSave.GetAppoitmentList())
        {
            if (appo.AppointmentonThisDay(askedday))
            {
                DayList.Add(appo);
            }
        }
        return DayList;
    }
    public void Reload() // nur für Teseten 
    {
        LoadList();
    }
 

    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void SubscribeToEvents_Start()
    {
        print(Subject.current);
        Subject.current.OnTaskSetDone += RemoveTask;
        Subject.current.OnNewTask += CreateNewTask;
        Subject.current.OnTaskChange += ChangeTask;
        Subject.current.OnTaskReturning += ManageTaskReturn;

        Subject.current.OnNewAppointment += CreateNewAppointment;
        /*Subject.current.OnAppointmentChange += ChangeAppointment;
        Subject.current.OnAppointmentReturning += ManageAppointmentReturn;*/
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
    }

    public void SetNotificatioSystem(IDataMasterNOSClient notS) // Muss von Außen getsezt werden ´, SerzliedFlied geht nicht bei Interfaces
    {
        clientNotificationSystem = notS;
    }




}
