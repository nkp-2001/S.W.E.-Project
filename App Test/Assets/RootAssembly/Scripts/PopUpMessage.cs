using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PopUpMessage : MonoBehaviour,IObserver
{

    [SerializeField] Image MessageBox;
    [SerializeField] Image HeaderBox;
    [SerializeField] TextMeshProUGUI messagerText;
    [SerializeField] TextMeshProUGUI headerText;

    void Start()
    {
        PopUpMessage[] objs = FindObjectsOfType<PopUpMessage>();

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        SubscribeToEvents();
    }

    public void ShowBoxNewTask(string t, string d, int[] dt, float prio, int repeatindex)
    {     
        StopAllCoroutines();
        StartCoroutine(ShowText("Task was created"));     
    }

    public void ShowBoxTaskChange(Task oldtask, string t, string d, int[] dt, float p, int repeatIndex)
    {
        StopAllCoroutines();   
        StartCoroutine(ShowText("Task was changed"));
    }

    public void ShowBoxTaskDone(Task doneTask)
    {
        StopAllCoroutines();
        
        if (doneTask.Deadline != null)
        {
            if (doneTask.Deadline.Length != 0)
            {
                DateTime deadline = DataMaster.ConvertIntArrayToDatetime(doneTask.Deadline);
                DateTime currentDate = DateTime.Now;
                
                int remaining = (deadline - currentDate).Days;

                StartCoroutine(ShowText("You have completed the task" + " " + remaining + " " + "days before deadline"));
                return;
            }            
        }

        StartCoroutine(ShowText("You have completed the task")); 
    }

    public void ShowBoxTaskReturn(Task oldtask, string potNewname, string potNewDiscp, int[] potNewDt, float potNewPrioint, int repeatIndex)
    {
        StopAllCoroutines();
        StartCoroutine(ShowText("You have a new task to do"));
    }

    public void ShowBoxTaskExpired()
    {
        StopAllCoroutines();  
        StartCoroutine(ShowText("A task has expired"));
    }

    public void ShowDateInPastMessage()
    {
        StopAllCoroutines();
        StartCoroutine(ShowText("Please enter a future date"));
    }

    IEnumerator ShowText(string text)
    { 
        HeaderBox.color = new Color32(0, 0, 0, 190);
        MessageBox.color = new Color32(54, 99, 99, 200);
        headerText.color = new Color32(255, 0, 0, 190);
        messagerText.color = new Color32(255, 255, 255, 200);
        headerText.text = "Message!";
        messagerText.text = text;

        yield return new WaitForSeconds(2);

        StartCoroutine(FadeOut());  
    }

    IEnumerator FadeOut()
    {
        for (byte i = 200; i >= 1; i--)
        {
            HeaderBox.color = new Color32(0, 0, 0, i);
            MessageBox.color = new Color32(54, 99, 99, i);

            headerText.color = new Color32(255, 0, 0, i);
            messagerText.color = new Color32(255, 255, 255, i);
            yield return new WaitForSeconds(0.0015f);
        }
    }

    public void SubscribeToEvents()
    {
        Subject.OnNewTask += ShowBoxNewTask;
        Subject.OnTaskChange += ShowBoxTaskChange;
        Subject.OnTaskSetDone += ShowBoxTaskDone;
        Subject.OnTaskReturning += ShowBoxTaskReturn;
        Subject.OnExpiredDealine += ShowBoxTaskExpired;

        Subject.OnDateInPast += ShowDateInPastMessage;
    }

    public void UnsubscribeToAllEvents()
    {
        Subject.OnNewTask -= ShowBoxNewTask;
        Subject.OnTaskChange -= ShowBoxTaskChange;
        Subject.OnTaskSetDone -= ShowBoxTaskDone;
        Subject.OnTaskReturning -= ShowBoxTaskReturn;
        Subject.OnExpiredDealine -= ShowBoxTaskExpired;

        Subject.OnDateInPast -= ShowDateInPastMessage;
    }

    private void OnDestroy()
    {
        UnsubscribeToAllEvents();
    }
}
