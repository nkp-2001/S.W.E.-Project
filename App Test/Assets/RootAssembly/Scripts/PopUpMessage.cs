using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PopUpMessage : MonoBehaviour
{

    [SerializeField] Image MessageBox;
    [SerializeField] Image HeaderBox;
    [SerializeField] TextMeshProUGUI messagerText;
    [SerializeField] TextMeshProUGUI headerText;

    // Start is called before the first frame update
    void Start()
    {
        PopUpMessage[] objs = FindObjectsOfType<PopUpMessage>();
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        SubscribeToEvents_Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowBoxNewTask(string t, string d, int[] dt, float prio, int repeatindex)
    //{
        //if (MessageBox != null)
        {
            Debug.Log("111");
            StartCoroutine(ShowText("Task was created"));
        }
    //}

    public void ShowBoxTaskChange(Task oldtask, string t, string d, int[] dt, float p, int repeatIndex)
    {
        Debug.Log("222");
        StartCoroutine(ShowText("Task was changed"));
    }

    public void ShowBoxTaskDone(Task doneTask)
    {
        if (doneTask.Deadline.Length != 0)
        {
            DateTime expDate = ConvertIntArray_toDatetime(doneTask.Deadline);
            DateTime localDate = System.DateTime.Now;
            print(localDate);
            int remain = (expDate - localDate).Days;
            print(remain);

            StartCoroutine(ShowText("You have completed the task" + " " + remain + " " + "days before deadline"));
        }
        else
        {
            StartCoroutine(ShowText("You have completed the task"));
        }
       
    }
    public System.DateTime ConvertIntArray_toDatetime(int[] toconvert)
    {
        return new DateTime(toconvert[4], toconvert[3], toconvert[2], toconvert[1], toconvert[0], 0);
    }

    public void ShowBoxTaskReturn(Task oldtask, string potNewname, string potNewDiscp, int[] potNewDt, float potNewPrioint, int repeatIndex)
    {
        Debug.Log("444");
        StartCoroutine(ShowText("You have a new task to do"));
    }

    public void ShowBoxTaskExpired()
    {
        Debug.Log("555");
        StartCoroutine(ShowText("A task has expired"));
    }

    IEnumerator ShowText(string text)
    {
        HeaderBox.color = new Color32(0, 0, 0, 190);
        MessageBox.color = new Color32(54, 99, 99, 200);
        headerText.text = "Message!";
        messagerText.text = text;
        yield return new WaitForSeconds(2);
        for (byte i = 200; i >= 1; i--)
        {
            HeaderBox.color = new Color32(0, 0, 0, i);
            MessageBox.color = new Color32(54, 99, 99, i);

            headerText.color = new Color32(255, 0, 0, i);
            messagerText.color = new Color32(255, 255, 255, i);
            yield return new WaitForSeconds(0.0075f);


        }
        messagerText.text = "";
        headerText.text = "";
        MessageBox.color = new Color32(0, 0, 0, 0);
        HeaderBox.color = new Color32(0, 0, 0, 0);
    }

    public void SubscribeToEvents_Start()
    {
        Subject.current.OnNewTask += ShowBoxNewTask;
        Subject.current.OnTaskChange += ShowBoxTaskChange;
        Subject.current.OnTaskSetDone += ShowBoxTaskDone;
        Subject.current.OnTaskReturning += ShowBoxTaskReturn;
        Subject.current.OnExpiredDealine += ShowBoxTaskExpired;
    }

    public void UnsubscribeToAllEvent()
    {
        Subject.current.OnNewTask -= ShowBoxNewTask;
        Subject.current.OnTaskChange -= ShowBoxTaskChange;
        Subject.current.OnTaskSetDone -= ShowBoxTaskDone;
        Subject.current.OnTaskReturning -= ShowBoxTaskReturn;
        Subject.current.OnExpiredDealine -= ShowBoxTaskExpired;
    }

    private void OnEnable()
    {
        //SubscribeToEvents_Start();
    }

    private void OnDestroy()
    {
        UnsubscribeToAllEvent();
    }

    public void UnsubscribeToAllEvents()
    {
        throw new System.NotImplementedException();
    }
}
