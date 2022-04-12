using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;

public class NotificationSystem : MonoBehaviour
{
    // Start is called before the first frame update //WIP
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
        // AndroidNotificationCenter.CheckScheduledNotificationStatus()
    }
    public void SendNewGeneralNotifcation()
    {
        var notification = new AndroidNotification(
            "To-Do-List Alert", 
            "There are Task on your to-Do List",
            System.DateTime.Now.AddDays(1),
            new System.TimeSpan(1,0,0,0)); // 1 Tag 

        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "Channel-To-Do-List");
       
    }

  
}
