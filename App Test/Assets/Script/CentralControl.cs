using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Notifications.Android;

public class CentralControl : MonoBehaviour
{
    [SerializeField] RectTransform ProTest;
    [SerializeField] GameObject toplace;

    // Start is called before the first frame update
    void Start()
    {
        // Benarichtigungen - Setup --aus offizialen Unity Guide
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Notifications Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        //---------------------------------------------------------------
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadSceneMode(int i)
    {
        SceneManager.LoadScene(i);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void PlaceTest()
    {
        GameObject placed = Instantiate(toplace, ProTest.anchoredPosition , Quaternion.identity);
        placed.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false); //Damit als UI-Elemt / Child von Canvas
    }
    public void Messagtest()
    {
        var notification = new AndroidNotification();
        notification.Title = "_Test_";
        notification.Text = "Das ist ein Text";
        notification.FireTime = System.DateTime.Now;

        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }
    public void Messagtest(int i)
    {
        var notification = new AndroidNotification();
        notification.Title = "_Test_";
        notification.Text = "Das ist ein Text";
        notification.FireTime = System.DateTime.Now.AddMinutes(i);

        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }

}
