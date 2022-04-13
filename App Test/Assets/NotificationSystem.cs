using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;

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
            Importance = Importance.Default,
            Description = "",

        };
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
            new System.TimeSpan(1,0,0,0)); // 1 Tag Repeat

        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "Channel-To-Do-List", 1000);
       
    }

  
}
