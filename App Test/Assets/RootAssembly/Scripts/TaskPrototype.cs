using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TaskPrototype : MonoBehaviour 
{  
    private Datamaster taskMaster;
    [SerializeField] private Task task;
    [SerializeField] AudioClip pingSound;
    RectTransform rect;
    bool showingOldTask = false;
    [SerializeField] GameObject dircTobj;
    private void Start()
    {
        taskMaster = FindObjectOfType<Datamaster>();
        rect = GetComponent<RectTransform>();
    

    }
    public void Setup(Task t, Transform taskContainer)
    {
        task = t;
        transform.GetChild(0).GetComponentsInChildren<TextMeshProUGUI>()[0].text = t.Titel;
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
        if (t.NextDeadlineIndex != 0 & ( t.Failedtimes + t.Sucessedtimes) > 0)
        {
            dircTobj.GetComponentInChildren<TextMeshProUGUI>().text = "Priot�t: " + t.Prio + "      ||Sucess:" +t.Sucessedtimes +"|Failed:"+ t.Failedtimes + "|| \n Discirption: " + t.Description;
            if (!t.Failedprevios)
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
            dircTobj.GetComponentInChildren<TextMeshProUGUI>().text = "Priot�t: " + t.Prio + "\n Discirption: " + t.Description;
        }
    }
    public void Setup_OldTask(Task t, Transform taskContainer)
    {      
        Setup( t,  taskContainer);        
       
        if (t.Sucess)
        {
            ChangeColorBanner(new Color32(0, 180, 0, 255),new Color32(0, 100, 0, 255));

        }
        else
        {
            ChangeColorBanner(new Color32(180, 0, 0, 255), new Color32(100, 0, 0, 255));
           
        }
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Do it \n again"; //vllt doch per Feld machen , aber da nur einmal benutzt eigentlich �berfl�ssig
        
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
        task.Sucess = true; 
        task.Done = true;
        task.Sucessedtimes++;
        task.Failedprevios = false; 
        for (int index = transform.GetSiblingIndex(); index >= 0; index--) //Bug Fixer
        {
            transform.parent.GetChild(index).GetComponent<TaskPrototype>().HideDescription();
        }
        Subject.current.Trigger_TaskSetDone(task);

      
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
        print("Event output: " +id + "" + plusUp);
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
        ValueManager.tastReturninEdit = true;
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
            print("showingOldTask");
            task.Sucess = false;
            task.Done = false;

  
           
            if (task.Deadline != null)
            {
                if (task.Deadline.Length != 0)
                {
                    if ((Datamaster.ConvertIntArrayToDatetime(task.Deadline) > System.DateTime.Now))
                    {
                        Subject.current.Trigger_OnTaskReturning(task, task.Titel, task.Description, task.Deadline, task.Prio, task.NextDeadlineIndex);
                    }
                    else
                    {
                        GoIntoTaskReturningEdit();
                    }
                    Destroy(gameObject);
                    return;
                }      
               
            }
            Subject.current.Trigger_OnTaskReturning(task, task.Titel, task.Description, task.Deadline, task.Prio, task.NextDeadlineIndex);
            Destroy(gameObject);


        }

      
    }
}