using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using System;
using UnityEngine.SceneManagement;

public class NotificationSystem : MonoBehaviour , IObserver,  IDataMasterNOSClient 
{
    DataMaster taskmaster;
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
       
    void Start() 
    {
        SubscribeToEvents();

        taskmaster = FindObjectOfType<DataMaster>();
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

    public void NotficationStatusReaction(string t, string d, int[] dt, float prio,int rindex) 
    {
        NotficationStatusReaction(false);     
    }
    public void NotficationStatusReaction(Task task,string t, string d, int[] dt, float prio,int rindex) 
    {
        NotficationStatusReaction(false);
    }

    public void SendNewGeneralNotifcation()
    {
        var notification = new AndroidNotification(
            "To-Do-List Alert", 
            "There are Task on your to-Do List",  
            DateTime.Now.AddDays(1),
            new System.TimeSpan(1, 0, 0, 0)); // 1 Tag Repeat

        notification.ShowTimestamp = true;
        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "Channel-To-Do-List", 1000);
    }


    public int SendNewDeadlineNotifications(string titel, DateTime expireTime)
    {
       
        int id = GetFreeNotiChannelId();
        var channelDealineNew = new AndroidNotificationChannel()
        {
            Id = "" + id,
            Name = "TaskDeadline" + id,
            Importance = Importance.Default,
            Description = "Channel for the App",

        };
        channelDealineNew.EnableVibration = true;
        AndroidNotificationCenter.RegisterNotificationChannel(channelDealineNew);
        /////////////////////////////////////////////////////////////////////////
        int dayleft = (expireTime - DateTime.Now).Days;

        if (dayleft >= 7)
        {
            int weeks = dayleft / 7;
            if (weeks > 4)
            {
                weeks = 4;
            }
            for (int i = weeks; i >= 1; i--)
            {
                DateTime DT = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, expireTime.Hour, expireTime.Minute, 0);

                var notificationDeadlines = new AndroidNotification(
                "Deadline Alert:" + titel,
                "You have " + i + " Week(s)  left to finish the task:" + titel,
                DT.AddDays((dayleft / 7 - i) * 7));
                /////////////////////////////////////////

                notificationDeadlines.ShowTimestamp = true;

                AndroidNotificationCenter.SendNotification(notificationDeadlines, "" + id);
            }
        }

        int singleDays = dayleft % 7;
        if (singleDays == 0 && dayleft != 0)
        {
            singleDays = 6;

        }

        for (int i = singleDays; i >= 1; i--)
        {
            DateTime DT = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, expireTime.Hour, expireTime.Minute, 0);

            var notificationDeadlines = new AndroidNotification(
              "Deadline Alert:" + titel,
              "You have " + i + " day(s)  left to finish the task:" + titel,
              DT.AddDays((dayleft - i)));
            /////////////////////////////////////////
            notificationDeadlines.ShowTimestamp = true;
            AndroidNotificationCenter.SendNotification(notificationDeadlines, "" + id);
        }

        var notificationDeadline = new AndroidNotification(
             "Deadline Alert:" + titel,
             "The Deadline for " + titel + "has expired. You didn`t complete it in time",
             expireTime);
        ////////////////////////////////////////////////////
        notificationDeadline.ShowTimestamp = true;
        AndroidNotificationCenter.SendNotification(notificationDeadline, "" + id);
        ///////////////
        print("Notication ertsellt" + id + "||||"+ expireTime.ToString());
        return id;
    }

    private int GetFreeNotiChannelId()
    {
        int id = 1;
        List<int> t = new();
        foreach (AndroidNotificationChannel x in AndroidNotificationCenter.GetNotificationChannels())
        {
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

    public void CancelNotificationsByID(int id)
    {
        AndroidNotificationCenter.DeleteNotificationChannel("" + id);
    }

    public void CancelDeadlineNotifications(Task oldtask, string t, string d, int[] dt, float p,int rindex) 
    {
        
        if (oldtask.Deadline != dt && oldtask.Deadline != null)
        {
            CancelNotificationsByID(oldtask.DeadlineChannelId);          
        }


    }
    public void CancelNotifications(Task task)
    {
        int id = task.DeadlineChannelId;
        if (id != 0)
        {
            AndroidNotificationCenter.DeleteNotificationChannel("TaskDeadline" + id);
        }
        if (taskmaster.GetTaskListLength() == 0) 
        {
         
            NotficationStatusReaction(true);
        }
    
    }


    public int SendAppointmentNotifcations(DateTime StartTime,DateTime EndTime,int repeat,string titel,int repeattimes, int[] preWarn)
    {
        List<AndroidNotification> allNotication = new();
        DateTime cacheDT = StartTime; // Die FireTime in den NoticationObject zu editen löst einen Fehler aus:  Out of Memory oder AgrumentExpection
        string cacheText = "";


        int appo_id = GetFreeNotiChannelId();
        var channelAppointmentNew = new AndroidNotificationChannel()
        {
            Id = "" + appo_id,
            Name = "Appointment" + appo_id,
            Importance = Importance.Default,
            Description = "Channel for the Appointment",

        };
        channelAppointmentNew.EnableVibration = true;
        AndroidNotificationCenter.RegisterNotificationChannel(channelAppointmentNew);
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        bool breakUp = false;
        foreach (int i in preWarn)
        {
           
            AndroidNotification preNoti = new();
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
                    for (int a = repeattimes; a > 0; a--) // das es keine begrente Wiederholung in der AndroidNotifartion Api gibt
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
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        AndroidNotification atStart = new("Appointment Notice:" + titel, " appointment:" + titel + " has started", StartTime);
        AndroidNotification atEnd = new("Appointment Notice:" + titel, " appointment:" + titel + " is over now", EndTime);

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
        }
        atEnd.ShowTimestamp = true;
        atStart.ShowTimestamp = true;
        allNotication.Add(atStart);
        allNotication.Add(atEnd);


        foreach (AndroidNotification Noti in allNotication)
        {
            if (DateTime.Now < Noti.FireTime)
            {
                AndroidNotificationCenter.SendNotification(Noti, channelAppointmentNew.Id);
            }      
        }   
        return appo_id;
    }

    public void WipeNotification()
    {
        foreach (AndroidNotificationChannel x in AndroidNotificationCenter.GetNotificationChannels())
        {
            AndroidNotificationCenter.DeleteNotificationChannel(x.Id);
        }

    }
    public void SubscribeToEvents()
    {
        Subject.OnTaskSetDone += CancelNotifications;
        Subject.OnNewTask += NotficationStatusReaction;
        Subject.OnTaskReturning += NotficationStatusReaction;
        Subject.OnTaskChange += CancelDeadlineNotifications;     
    }

    public void UnsubscribeToAllEvents()
    {
        Subject.OnTaskSetDone -= CancelNotifications;
        Subject.OnNewTask -= NotficationStatusReaction;
        Subject.OnTaskChange -= CancelDeadlineNotifications;
        Subject.OnTaskReturning -= NotficationStatusReaction;
    }
    private void OnDisable()
    {
        UnsubscribeToAllEvents();
    }
}
