using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class TaskPrototype : MonoBehaviour 
{
    [SerializeField] private Task task;
    [SerializeField] private AudioClip pingSound;
    [SerializeField] private GameObject dircTobj;

    private DataMaster taskMaster;
    private RectTransform rect;
    private bool showingOldTask = false;

    private void Start()
    {
        taskMaster = FindObjectOfType<DataMaster>();
        rect = GetComponent<RectTransform>();
    }

    public void Setup(Task t, Transform taskContainer)
    {
        task = t;
        transform.GetChild(0).GetComponentsInChildren<TextMeshProUGUI>()[0].text = t.Title;

        if (t.Deadline != null && t.Deadline.Length != 0)
        {
            transform.GetChild(0).GetComponentsInChildren<TextMeshProUGUI>()[1].text =  //Format("{0:00} damit bei beispiel weise 6.2.2022 -> 06.02.2022 wird und so gleich lang ist wie z.b 12.11.2022
                 "DT:" + string.Format("{0:00}", t.Deadline[2]) + "." + string.Format("{0:00}", t.Deadline[3]) + "."
                 + t.Deadline[4] + "|" + string.Format("{0:00}", t.Deadline[1]) + ":" + string.Format("{0:00}", t.Deadline[0]);
        }
       
        dircTobj.SetActive(false);

        transform.SetParent(taskContainer);
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);

        // Bei Repeat 
        if (t.NextDeadlineIndex != 0 & ( t.FailedTimes + t.SuccessfulTimes) > 0)
        {
            dircTobj.GetComponentInChildren<TextMeshProUGUI>().text = "Priotät: " + t.Priority + "      ||Sucess:" +t.SuccessfulTimes +"|Failed:"+ t.FailedTimes + "|| \n Discirption: " + t.Description;
            if (!t.FailedPrevious)
            {
                ChangeColorBanner(new Color32(0, 90, 0, 255), new Color32(0, 50, 0, 255));
            }
            else
            {
                ChangeColorBanner(new Color32(90, 0, 0, 255), new Color32(50, 0, 0, 255));
            }
        }
        else
        {
            dircTobj.GetComponentInChildren<TextMeshProUGUI>().text = "Priotät: " + t.Priority + "\n Discirption: " + t.Description;
        }
    }

    public void Setup_OldTask(Task t, Transform taskContainer)
    {      
        Setup( t,  taskContainer);        
       
        if (t.Success)
        {
            ChangeColorBanner(new Color32(0, 180, 0, 255),new Color32(0, 100, 0, 255));
        }
        else
        {
            ChangeColorBanner(new Color32(180, 0, 0, 255), new Color32(100, 0, 0, 255)); 
        }

        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Do it \n again"; //vllt doch per Feld machen , aber da nur einmal benutzt eigentlich überflüssig
        
        showingOldTask = true;
    }

    private void ChangeColorBanner(Color32 newColor,Color32 highlightcolor)
    {
        Button button = GetComponentInChildren<Button>();
        GetComponentInChildren<Image>().color = newColor;
        ColorBlock cb = button.colors;
        cb.normalColor = newColor;
        cb.highlightedColor = highlightcolor;
        cb.pressedColor = highlightcolor;
        button.colors = cb;
    }

    public void SetTaskToDone()
    {
        AudioSource.PlayClipAtPoint(pingSound, Camera.main.transform.position);
        task.Success = true; 
        task.Done = true;
        task.SuccessfulTimes++;
        task.FailedPrevious = false;

        for (int index = transform.GetSiblingIndex(); index >= 0; index--)
        {
            transform.parent.GetChild(index).GetComponent<TaskPrototype>().HideDescription();
        }

        Subject.Trigger_TaskSetDone(task);
      
        Destroy(gameObject);
    }

    public void ToggleDescription()
    {
        if (dircTobj.activeSelf == true)
        {
            HideDescription();
        }
        else
        {
            ShowDescription();
        }
    }

    private void ShowDescription()
    {
        dircTobj.SetActive(true);
      
        for (int index = transform.GetSiblingIndex() - 1; index >= 0; index--)
        {
            transform.parent.GetChild(index).GetComponent<TaskPrototype>().MoveRect(true);
        }
    }

    public void HideDescription()
    {
        if (dircTobj.activeSelf == true)
        {
            dircTobj.SetActive(false);         

            for (int index = transform.GetSiblingIndex() - 1; index >= 0; index--)
            {
                transform.parent.GetChild(index).GetComponent<TaskPrototype>().MoveRect(false);
            }

        }
    }
    public void MoveRect(int id,bool plusUp)
    { 
        if (id > transform.GetSiblingIndex())
        {
            MoveRect(plusUp);
        } 
    }

    public void MoveRect(bool plusUp)
    {        
        if (plusUp)
        {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y + 900);
        }
        else
        {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y - 900);
        }
    }

    public void GoIntoTaskEdit()
    {
        ValueManager.taskOnEdit = task;
        SceneManager.LoadScene(1);
    }

    public void GoIntoTaskReturningEdit()
    {
        ValueManager.taskOnEdit = task;
        ValueManager.taskReturnInEdit = true;
        SceneManager.LoadScene(1);
    }
    public void ButtonReaction()
    {
        if (!showingOldTask)
        {
            GoIntoTaskEdit();
        }
        else
        {
            task.Success = false;
            task.Done = false;

            if (task.Deadline != null)
            {
                if (task.Deadline.Length != 0)
                {
                    if ((DataMaster.ConvertIntArrayToDatetime(task.Deadline) > DateTime.Now))
                    {
                        Subject.Trigger_OnTaskReturning(task, task.Title, task.Description, task.Deadline, task.Priority, task.NextDeadlineIndex);
                    }
                    else
                    {
                        GoIntoTaskReturningEdit();
                    }
                  
                    return;
                }        
            }

            Subject.Trigger_OnTaskReturning(task, task.Title, task.Description, task.Deadline, task.Priority, task.NextDeadlineIndex);
        }
    }
}