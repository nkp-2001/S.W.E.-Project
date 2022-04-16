using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using System;

public class NotificationSystem : MonoBehaviour
{
    // Start is called before the first frame update //WIP

    private void Awake() 
    {
        Taskmaster[] objs = FindObjectsOfType<Taskmaster>(); //Sigenton , Scenenwechesel löscht es nicht 

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
    void Start() //Register 
    {
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
    public void CheckSpeficNotifcation()
    {

    }
    public List<int> SendNewDeadlineNotifications(string titel, DateTime expireTime) //Anders Notfication ID Speicher , List<int> Ansatz
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

        int singeldayes = dayleft % 7;
        if (singeldayes == 0)
        {
            singeldayes = 6;
        }
        for (int i = singeldayes; i >= 1; i--) 
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

    public int SendNewDeadlineNotificationsX(string titel, DateTime expireTime)
    {
        List<int> t = new List<int>();
        int id = 0;
        foreach (AndroidNotificationChannel x in AndroidNotificationCenter.GetNotificationChannels())
        {
            t.Add(Int32.Parse(x.Id));

            print(Int32.Parse(x.Id));
        }
        t.Sort();
        foreach (int i in t)
        {
           if (id == i)
           {
                id++;
           } 
        }
        var channelDealineNew = new AndroidNotificationChannel()
        {
            Id = ""+ id,
            Name = "To-Do-List Alert",
            Importance = Importance.Default,
            Description = "Channel for the App",

        };
        channelDealineNew.EnableVibration = true;
        AndroidNotificationCenter.RegisterNotificationChannel(channelDealineNew);
        //////////
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

                var notificationDeadlines = new AndroidNotification(
                "Deadline Alert:" + titel,
                "You have " + i + " Week(s)  left to finish the task:" + titel,
                System.DateTime.Now.AddDays((dayleft / 7 - i) * 7));
                /////////////////////////////////////////

                notificationDeadlines.ShowTimestamp = true;
                
                AndroidNotificationCenter.SendNotification(notificationDeadlines, "" + id);
            }
        }

        int singeldayes = dayleft % 7;
        if (singeldayes == 0)
        {
            singeldayes = 6;
        }
        for (int i = singeldayes; i >= 1; i--)
        {
            var notificationDeadlines = new AndroidNotification(
              "Deadline Alert:" + titel,
              "You have " + i + " day(s)  left to finish the task:" + titel,
              System.DateTime.Now.AddDays((dayleft - i)));
            /////////////////////////////////////////
            notificationDeadlines.ShowTimestamp = true;
            AndroidNotificationCenter.SendNotification(notificationDeadlines, "" + id);
        }

        var notificationDeadline = new AndroidNotification(
             "Deadline Alert:" + titel,
             "The Deadline for " + titel + "has expired. You didn`t complete it in time",
             expireTime);
        /////////////////////////////////////////
        notificationDeadline.ShowTimestamp = true;
        AndroidNotificationCenter.SendNotification(notificationDeadline, "" + id);
        ///////////////
        return id;
    }

    public void CanelDeadlineNotifctions(int id)
    {
        if ( (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Delivered) | (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Scheduled))
        {
            
            AndroidNotificationCenter.CancelNotification(id);
        }
    }
    public void CanelDeadlineNotifctionsX(int id)
    {
        AndroidNotificationCenter.DeleteNotificationChannel("" + id);
    }


}
