using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using System;
using UnityEngine.SceneManagement;

public class NotificationSystem : MonoBehaviour , IObserver, /* Dependecy Inversion: */ IDataMasterNOSClient 
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

    public void NotficationStatusReaction(string t, string d, int[] dt, float prio,int rindex) //!! vllt anders als mit diesen "Toten" Parameter 
    {
        NotficationStatusReaction(false);
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

    public List<int> SendNewDeadlineNotifications_Other(string titel, DateTime expireTime) //Anders Notfication ID Speicher , List<int> Ansatz
    {
        int dayleft = (expireTime - System.DateTime.Now).Days;
        List<int> Notifi_ID = new List<int>();
        List<AndroidNotification> allNotifi = new List<AndroidNotification>();
        print(dayleft);

        if(dayleft >= 7)
        {
            int weeks = dayleft / 7;
            if (weeks > 4)
            {
                weeks = 4;
            }
            for (int i = weeks; i >= 1; i--)
            {
               
                var notificationDeadlines = new AndroidNotification(
                "Deadline Alert:" + titel,
                "You have " + i +" Week(s)  left to finish the task:" + titel,
                System.DateTime.Now.AddDays((dayleft/7 - i)*7)  );
                /////////////////////////////////////////
                
                notificationDeadlines.ShowTimestamp = true;
                allNotifi.Add(notificationDeadlines);
            }
        }

        int singledays = dayleft % 7;
        if (singledays == 0)
        {
            singledays = 6;
        }
        for (int i = singledays; i >= 1; i--) 
        {
            var notificationDeadlines = new AndroidNotification(
              "Deadline Alert:" + titel,
              "You have " + i + " day(s)  left to finish the task:" + titel,
              System.DateTime.Now.AddDays((dayleft - i)));
            /////////////////////////////////////////
            notificationDeadlines.ShowTimestamp = true;
            allNotifi.Add(notificationDeadlines);
        }

        var notificationDeadline = new AndroidNotification(
             "Deadline Alert:" + titel,
             "The Deadline for "+titel+ "has expired. You didn`t complete it in time",
             expireTime);
        /////////////////////////////////////////
        notificationDeadline.ShowTimestamp = true;
        
        allNotifi.Add(notificationDeadline);

        print(allNotifi.Count);
        foreach (var noti in allNotifi)
        {
           
            print("FireDays" + noti.FireTime.Day);
            Notifi_ID.Add(AndroidNotificationCenter.SendNotification(noti, "Channel-To-Do-List"));
        }
        return Notifi_ID;
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
            print(i);

            DateTime DT = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, expireTime.Hour, expireTime.Minute, 0);

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

    public void CancelDeadlineNotifications(int id)
    {
        if ( (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Delivered) | (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Scheduled))
        {
            
            AndroidNotificationCenter.CancelNotification(id);
        }
    }
    public void CancelDNotificationsByID(int id)
    {
        AndroidNotificationCenter.DeleteNotificationChannel("" + id);
    }

    public void CancelDeadlineNotificationsX(Task oldtask, string t, string d, int[] dt, float p,int rindex) //!! vllt anders als mit diesen "Toten" Parameter 
    {
        
        if (oldtask.Deadline != dt && oldtask.Deadline != null)
        {
            CancelDNotificationsByID(oldtask.DeadlineChannel_ID);
            print("Reaction on Deadline remove");
        }


    }
    public void CancelNotificationsX(Task task)
    {
        int id = task.DeadlineChannel_ID;
        if (id != 0)
        {
            AndroidNotificationCenter.DeleteNotificationChannel("" + id);
        }
        print("Event noticed" + taskmaster.GetTaskListLenght());
        if (taskmaster.GetTaskListLenght() == 0) //vllt Listelänge anderes Vermittlen , ohne aufruf aus Speicher? 
        {
            print("Zero bei Liste Länge");
            NotficationStatusReaction(true);
        }
       
       
    }
   

    public int SendAppointmentNotifcations(DateTime StartTime,DateTime EndTime,int repeat,string titel,int repeattimes, int[] preWarn)
    {
        List<AndroidNotification> allNotication = new List<AndroidNotification>();


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
        ///

        foreach(int i in preWarn)
        {
            AndroidNotification preNoti = new AndroidNotification();
            switch (i)
            {
                case 1:
                    preNoti = new AndroidNotification("Appointment Notice:" + titel, " appointment:" + titel + " will Start in 1 Hour ", StartTime.AddHours(-1));
                    break;
                case 2:
                    preNoti = new AndroidNotification("Appointment Notice:" + titel, " appointment:" + titel + " will Start in 1 Day ", StartTime.AddDays(-1));
                    break;
                case 3:
                    preNoti = new AndroidNotification("Appointment Notice:" + titel, " appointment:" + titel + " will Start in 1 Mounth ", StartTime.AddMonths(-1));
                    break;
                case 4:
                    preNoti = new AndroidNotification("Appointment Notice:" + titel, " appointment:" + titel + " will Start in 1 Years ", StartTime.AddYears(-1));
                    break;
                default:
                    break;
            }
            preNoti.ShowTimestamp = true;
            if (repeat != 0 & repeattimes == 0)
            {
                preNoti.RepeatInterval = new TimeSpan(repeat, 0, 0, 0);
                allNotication.Add(preNoti);
            }
            else if (repeat != 0 & repeattimes > 0)
            {
                allNotication.Add(preNoti);
                for (int it = repeattimes; repeattimes > 0; it--) // in AndroidNotification ist keinen Parameter den man setzten kann das eine Meldung begrenzte mal Widerholt werden  kann deswegenn das:
                {
                    Debug.Log(preNoti.FireTime);
                    preNoti.FireTime = preNoti.FireTime.AddDays(repeat);  // + new TimeSpan(repeat, 0, 0, 0);
                    allNotication.Add(preNoti);

                }
            }
            else
            {
                allNotication.Add(preNoti);
            }
          
            
        }

        AndroidNotification atStart = new AndroidNotification("Appointment Notice:" + titel, " appointment:" + titel + " has started", StartTime);
        AndroidNotification atEnd = new AndroidNotification("Appointment Notice:" + titel, " appointment:" + titel + " is over now", EndTime);
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
        print("________________Channle createtd with : " +appo_id);
        return appo_id;
    }


    public void SubscribeToEvents_Start()
    {
        print("I have assigend my Stuff");
        Subject.current.OnTaskSetDone += CancelNotificationsX;
        Subject.current.OnNewTask += NotficationStatusReaction;
        Subject.current.OnTaskReturning += NotficationStatusReaction;
        Subject.current.OnTaskChange += CancelDeadlineNotificationsX;
      //  Subject.current.SetonRequest_NotiIDDeadline(SendNewDeadlineNotifications);
       // Subject.current.SetonRequest_NotiIDAppointment(SendAppointmentNotifcations);

        
    }

    public void UnsubscribeToAllEvents()
    {
        Subject.current.OnTaskSetDone -= CancelNotificationsX;
        Subject.current.OnNewTask -= NotficationStatusReaction;
        Subject.current.OnTaskChange -= CancelDeadlineNotificationsX;
        Subject.current.OnTaskReturning -= NotficationStatusReaction;
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
