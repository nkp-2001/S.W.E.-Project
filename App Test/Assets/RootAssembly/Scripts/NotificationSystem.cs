using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using System;
using UnityEngine.SceneManagement;

public class NotificationSystem : MonoBehaviour , IObserver, /* Dependecy Inversion : */ IDataMasterNOSClient 
{
    Taskmaster taskmaster;
    private void Awake() 
    {
        NotificationSystem[] objs = FindObjectsOfType<NotificationSystem>(); // Scenenwechesel löscht es nicht 

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        
    }
       
    void Start() //Register 
    {
        SubscribeToEvents_Start();

        taskmaster = FindObjectOfType<Taskmaster>();
        taskmaster.SetNotificatioSystem(this);
      

        var channel = new AndroidNotificationChannel()
        {
            Id = "Channel-To-Do-List",
            Name = "To-Do-List Alert",
            Importance = Importance.Default ,
            Description = "Channel for the App",

        };
        channel.EnableVibration = true;
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

       
    }
    public void NotficationStatusReaction(bool ListEmpty)
    {
        var status = AndroidNotificationCenter.CheckScheduledNotificationStatus(1000);
        if (status == NotificationStatus.Scheduled) //Meldung besteht bereits
        {
            if (ListEmpty)
            {
                AndroidNotificationCenter.CancelNotification(1000);
                
            }
        }
        else if (status == NotificationStatus.Unknown) //Meldung besteht nicht
        {
            if (!ListEmpty)
            {
                SendNewGeneralNotifcation();
            }
        }
    }

    public void NotficationDeadline(string t, string d, int[] dt, float prio,int rindex) //!! vllt anders als mit diesen "Toten" Parameter 
    {
        NotficationStatusReaction(false);
        if (dt != null)
        {
            if (dt.Length != 0) 
            {
                SendNewDeadlineNotifications(t, Taskmaster.ConvertIntArray_toDatetime(dt));
            }
        }
        print("Reaction on new Task");
    }
    public void NotficationDeadline(Task task ,string t, string d, int[] dt, float prio, int rindex) //!! vllt anders als mit diesen "Toten" Parameter 
    {
        NotficationStatusReaction(false);
        if (dt != null)
        {
            if (dt.Length != 0)
            {
                SendNewDeadlineNotifications(t, Taskmaster.ConvertIntArray_toDatetime(dt));
            }
        }
        print("Reaction on new Task");
    }
    public void NotficationStatusReaction(Task task,string t, string d, int[] dt, float prio,int rindex) //!! vllt anders als mit diesen "Toten" Parameter 
    {
        NotficationStatusReaction(false);
        print("Reaction on new Task");
    }

    public void SendNewGeneralNotifcation()
    {
        var notification = new AndroidNotification(
            "To-Do-List Alert", 
            "There are Task on your to-Do List",  
            System.DateTime.Now.AddDays(1),
            new System.TimeSpan(1, 0, 0, 0)); // 1 Tag Repeat

        notification.ShowTimestamp = true;
        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "Channel-To-Do-List", 1000);

    }


    public int SendNewDeadlineNotifications(string titel, DateTime expireTime)
    {
       
        int id = GetFreeNotiChannelId();
        var channelDealineNew = new AndroidNotificationChannel()
        {
            Id = "ID" + titel,
            Name = "TaskDeadline" + titel,
            Importance = Importance.Default,
            Description = "Channel for the App",

        };
        channelDealineNew.EnableVibration = true;
        AndroidNotificationCenter.RegisterNotificationChannel(channelDealineNew);
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        int dayleft = (expireTime - System.DateTime.Now).Days;
        List<int> Notifi_ID = new List<int>();
        List<AndroidNotification> allNotifi = new List<AndroidNotification>();
        print(dayleft);

        if (dayleft >= 7)
        {
            int weeks = dayleft / 7;
            if (weeks > 4)
            {
                weeks = 4;
            }
            for (int i = weeks; i >= 1; i--)
            {
                DateTime DT = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, expireTime.Hour, expireTime.Minute, 0);



                var notificationDeadlines = new AndroidNotification(
                "Deadline Alert:" + titel,
                "You have " + i + " Week(s)  left to finish the task:" + titel,
                DT.AddDays((dayleft / 7 - i) * 7));
                /////////////////////////////////////////

                notificationDeadlines.ShowTimestamp = true;

                AndroidNotificationCenter.SendNotification(notificationDeadlines, "ID" + titel);
            }
        }

        int singleDays = dayleft % 7;
        if (singleDays == 0 && dayleft != 0)
        {
            singleDays = 6;

        }

        for (int i = singleDays; i >= 1; i--)
        {
            print(i);

            DateTime DT = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, expireTime.Hour, expireTime.Minute, 0);

            var notificationDeadlines = new AndroidNotification(
              "Deadline Alert:" + titel,
              "You have " + i + " day(s)  left to finish the task:" + titel,
              DT.AddDays((dayleft - i)));
            /////////////////////////////////////////
            notificationDeadlines.ShowTimestamp = true;
            AndroidNotificationCenter.SendNotification(notificationDeadlines, "ID" + titel);
        }

        var notificationDeadline = new AndroidNotification(
             "Deadline Alert:" + titel,
             "The Deadline for " + titel + "has expired. You didn`t complete it in time",
             expireTime);
        ////////////////////////////////////////////////////
        notificationDeadline.ShowTimestamp = true;
        AndroidNotificationCenter.SendNotification(notificationDeadline, "ID" + titel);
        ///////////////
        print("Notication ertsellt" + id + "||||"+ expireTime.ToString());
        return id;
    }

    private int GetFreeNotiChannelId()
    {
        int id = 1;
        List<int> t = new List<int>();
        foreach (AndroidNotificationChannel x in AndroidNotificationCenter.GetNotificationChannels())
        {
            print(x.Id);
            try
            {

                t.Add(Int32.Parse(x.Id));
            }
            catch (FormatException) { }

        }
        t.Sort();
        foreach (int i in t)
        {
            if (id == i)
            {
                id++;
            }
        }
        return id;
    }


    public void ChangeDeadlineNotificationsX(Task oldtask, string t, string d, int[] dt, float p, int rindex) //!! vllt anders als mit diesen "Toten" Parameter 
    {

        CancelNotificationsX(oldtask);
        if (oldtask.Deadline != null)
        {
            SendNewDeadlineNotifications(t, Taskmaster.ConvertIntArray_toDatetime(dt));
        }


    }

    public void CancelNotificationsX(Task task)
    {
        
        
        // AndroidNotificationCenter.DeleteNotificationChannel("TaskDeadline" + id);
        AndroidNotificationCenter.DeleteNotificationChannel("ID" + task.Titel); 
        print("Event noticed" + taskmaster.GetTaskListLenght());
        
        if (taskmaster.GetTaskListLenght() == 0) //
        {
            print("Zero bei Liste Länge");
            NotficationStatusReaction(true);
        }
    
    }
    public void CancelNotificationsX(Appointment appo)
    {
        AndroidNotificationCenter.DeleteNotificationChannel("ID" + appo.Title);
        print("Event noticed" + taskmaster.GetTaskListLenght());
    }


    public int SendAppointmentNotifcations(DateTime StartTime,DateTime EndTime,int repeat,string titel,int repeattimes, int[] preWarn)
    {
        List<AndroidNotification> allNotication = new List<AndroidNotification>();
        DateTime cacheDT = StartTime; // Die FireTime in den NoticationObject zu editen löst einen Fehler aus Out of Memory oder AgrumentExpection die sich wiederspricht
        string cacheText = "";


        int appo_id = GetFreeNotiChannelId();
        var channelAppointmentNew = new AndroidNotificationChannel()
        {
            Id = "ID" + titel,
            Name = "Appointment" + titel,
            Importance = Importance.Default,
            Description = "Channel for the Appointment",

        };
        channelAppointmentNew.EnableVibration = true;
        AndroidNotificationCenter.RegisterNotificationChannel(channelAppointmentNew);
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        bool breakUp = false;
        foreach (int i in preWarn)
        {
           
            AndroidNotification preNoti = new AndroidNotification();
            switch (i)
            {
                case 1:
                    cacheText = " will Start in 1 Hour ";
                    preNoti = new AndroidNotification("Appointment Notice:" + titel, " appointment:" + titel + cacheText, cacheDT.AddHours(-1));
                    break;
                case 2:
                    cacheText = " will Start in 1 Day ";
                    preNoti = new AndroidNotification("Appointment Notice:" + titel, " appointment:" + titel + cacheText, cacheDT.AddDays(-1));
                    break;
                case 3:
                    cacheText = " will Start in 1 Mounth ";
                    preNoti = new AndroidNotification("Appointment Notice:" + titel, " appointment:" + titel + cacheText, cacheDT.AddMonths(-1));
                    break;
                case 4:
                    cacheText = " will Start in 1 Years ";
                    preNoti = new AndroidNotification("Appointment Notice:" + titel, " appointment:" + titel + cacheText, cacheDT.AddYears(-1));
                    break;
                default:
                    breakUp = true;
                    break;
            }
            if (!breakUp)
            {
                preNoti.ShowTimestamp = true;
                if (repeat != 0 & repeattimes == 0)
                {
                    preNoti.RepeatInterval = new TimeSpan(repeat, 0, 0, 0);
                    allNotication.Add(preNoti);
                }
                else if (repeat != 0 & repeattimes > 0)
                {
                    allNotication.Add(preNoti);
                    for (int a = repeattimes; a > 0; a--) // das es keine begrente Wiederholung in AndroidNotifartion Api gibt
                    {
                        allNotication.Add(new AndroidNotification("Appointment Notice:" + titel, " appointment:" + titel + cacheText, cacheDT.AddDays(repeat)));
                    }
                }
                else
                {
                    allNotication.Add(preNoti);
                }
            }
        }
        /////////////////Start and End /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        AndroidNotification atStart = new AndroidNotification("Appointment Notice:" + titel, " appointment:" + titel + " has started", StartTime);
        AndroidNotification atEnd = new AndroidNotification("Appointment Notice:" + titel, " appointment:" + titel + " is over now", EndTime);
        atEnd.ShowTimestamp = true;
        atStart.ShowTimestamp = true;

        if (repeat != 0 & repeattimes > 0)
        {
            for (int a = repeattimes; a > 0; a--) // das es keine begrente Wiederholung in AndroidNotifartion Api gibt
            {   
                
                allNotication.Add(new AndroidNotification("Appointment Notice:" + titel, " appointment:" + titel + " has started", StartTime.AddDays(repeat)));
                allNotication.Add(new AndroidNotification("Appointment Notice:" + titel, " appointment:" + titel + " is over now", EndTime.AddDays(repeat)));
                
            }
        }
        else if(repeat != 0 & repeattimes == 0)
        {
            atStart.RepeatInterval = new TimeSpan(repeat, 0, 0, 0);
            atEnd.RepeatInterval = new TimeSpan(repeat, 0, 0, 0);
            allNotication.Add(atStart);
            allNotication.Add(atEnd);
        }
       


        foreach (AndroidNotification Noti in allNotication)
        {
            if (DateTime.Now < Noti.FireTime)
            {
                AndroidNotificationCenter.SendNotification(Noti, channelAppointmentNew.Id);
            }      
        }   
        print("________________Channle createtd with : " +appo_id);
        return appo_id;
    }

    public void SendAppointmentNotifcations(string title, string description, int[] startTime, int[] endTime, int repeatindex, int repeatTimes, int[] preW)
    {
        SendAppointmentNotifcations(Taskmaster.ConvertIntArray_toDatetime(startTime), Taskmaster.ConvertIntArray_toDatetime(endTime), repeatindex, title, repeatTimes, preW);
    }
    public void ChangeAppointmentNotifcations(Appointment oldAppointment, string title, string description, int[] startTime, int[] endTime, int repeatindex, int repeatTimes, int[] preW)
    {
        CancelNotificationsX(oldAppointment);
        SendAppointmentNotifcations(Taskmaster.ConvertIntArray_toDatetime(startTime), Taskmaster.ConvertIntArray_toDatetime(endTime), repeatindex, title, repeatTimes, preW);
    }


    /// /////////////////
    public void WibeNotication()
    {
        foreach (AndroidNotificationChannel x in AndroidNotificationCenter.GetNotificationChannels())
        {
            Debug.Log("Wibed:" + x.Id);
            AndroidNotificationCenter.DeleteNotificationChannel(x.Id);
        }


    }
    public void SubscribeToEvents_Start()
    {
        print("I have assigend my Stuff");
        Subject.current.OnTaskSetDone += CancelNotificationsX;
        Subject.current.OnNewTask += NotficationDeadline;
        Subject.current.OnTaskReturning += NotficationDeadline;
        Subject.current.OnTaskChange += ChangeDeadlineNotificationsX;
        Subject.current.OnExpiredDealine += NotficationDeadlineReNew;

        Subject.current.OnNewAppointment += SendAppointmentNotifcations;
        Subject.current.OnAppointmentChange += ChangeAppointmentNotifcations;
      //  Subject.current.SetonRequest_NotiIDDeadline(SendNewDeadlineNotifications);
      // Subject.current.SetonRequest_NotiIDAppointment(SendAppointmentNotifcations);


    }

    private void NotficationDeadlineReNew(Task t, bool check)
    {
        if (check)
        {
            SendNewDeadlineNotifications(t.Titel, Taskmaster.ConvertIntArray_toDatetime(t.Deadline));
        }
    }

    public void UnsubscribeToAllEvents()
    {
        Subject.current.OnTaskSetDone -= CancelNotificationsX;
        Subject.current.OnNewTask -= NotficationDeadline;
        Subject.current.OnTaskReturning -= NotficationDeadline;
        Subject.current.OnTaskChange -= ChangeDeadlineNotificationsX;
        Subject.current.OnExpiredDealine -= NotficationDeadlineReNew;

        Subject.current.OnNewAppointment -= SendAppointmentNotifcations;
        Subject.current.OnAppointmentChange -= ChangeAppointmentNotifcations;
    }
    private void OnDisable()
    {
        UnsubscribeToAllEvents();
        print("2xxxx");

    }
    private void OnEnable()
    {
       
        Debug.Log("OnEnable");
    }
   
}
