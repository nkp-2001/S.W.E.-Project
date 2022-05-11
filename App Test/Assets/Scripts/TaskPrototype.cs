using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TaskPrototype : MonoBehaviour, IObserver
{   //!nicht die Child Strukur anfassen
    private Taskmaster taskMaster;
    private Taskmaster.Task task;
    RectTransform rect;
    bool showingOldTask = false;
    GameObject dircTobj;
    private void Start()
    {
        taskMaster = FindObjectOfType<Taskmaster>();
        rect = GetComponent<RectTransform>();
        
        SubscribeToEvents_Start();
    }
    public void Setup(Taskmaster.Task t, Transform taskContainer)
    {
        task = t;

        transform.GetChild(0).GetComponentsInChildren<TextMeshProUGUI>()[0].text = t.Titel;

        if (t.Deadline != null && t.Deadline.Length != 0)
        {
            transform.GetChild(0).GetComponentsInChildren<TextMeshProUGUI>()[1].text =  //Format("{0:00} damit bei beispiel weise 6.2.2022 -> 06.02.2022 wird und so gleich lang ist wie z.b 12.11.2022
                 "DT:" + string.Format("{0:00}", t.Deadline[2]) + "." + string.Format("{0:00}", t.Deadline[3]) + "."
                 + t.Deadline[4] + "|" + string.Format("{0:00}", t.Deadline[1]) + ":" + string.Format("{0:00}", t.Deadline[0]);
        }
        dircTobj = transform.GetChild(0).GetChild(2).gameObject;

        dircTobj.GetComponentInChildren<TextMeshProUGUI>().text = "Priot�t: " + t.Prio + "\n Discirption: " + t.Description;
        dircTobj.SetActive(false);

        transform.SetParent(taskContainer);
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);
    }
    public void Setup_OldTask(Taskmaster.Task t, Transform taskContainer)
    {      
        Setup( t,  taskContainer);        
        Button button = GetComponentInChildren<Button>();
        if (t.Sucess)
        {
            GetComponentInChildren<Image>().color = new Color32(0, 180, 0, 255);
            ColorBlock cb = button.colors;
            cb.normalColor = new Color32(0, 180, 0, 255);
            cb.highlightedColor = new Color32(0, 100, 0, 255);
            cb.pressedColor = new Color32(0, 100, 0, 255);
            button.colors = cb;

        }
        else
        {
            GetComponentInChildren<Image>().color = new Color32(180, 0, 0, 255);
            ColorBlock cb = button.colors;
            cb.normalColor = new Color32(180, 0, 0, 255);
            cb.highlightedColor = new Color32(180, 0, 0, 255);
            cb.pressedColor = new Color32(100, 0, 0, 255);
            button.colors = cb;
        }
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Do it \n again"; //vllt doch per Feld machen , aber da nur einmal benutzt eigentlich �berfl�ssig
        
        showingOldTask = true;

    }


    public void SetTaskToDone()
    {
        task.Sucess = true;
        task.Done = true;
        for (int index = transform.GetSiblingIndex(); index >= 0; index--) //Bug Verhinderer , Obersever Anpassung ?
        {
            transform.parent.GetChild(index).GetComponent<TaskPrototype>().HideDescription();
        }
        Subject.current.Trigger_TaskSetDone(task);

       // taskMaster.RemoveTask(task);
        Destroy(gameObject);
    }
    public void SelfDestroyTest()
    {
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
        Subject.current.Trigger_ScrollPopUp(transform.GetSiblingIndex(), true);  
    }

    public void HideDescription()
    {
        if (dircTobj.activeSelf == true)
        {
            dircTobj.SetActive(false);        
            Subject.current.Trigger_ScrollPopUp(transform.GetSiblingIndex(), false);
        }
    }
    public void MoveRect(int id,bool plusUp)
    {
        print("Event output: " +id + "" + plusUp);
        if (id > transform.GetSiblingIndex())
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

            int[] dt = task.Deadline;
           
            if (dt.Length == 0)
            {
                Subject.current.Trigger_OnTaskReturning(task, task.Titel, task.Description, task.Deadline, task.Prio);
            }
            else if ((new System.DateTime(dt[4], dt[3], dt[2], dt[1], dt[0], 0) > System.DateTime.Now))
            {
                Subject.current.Trigger_OnTaskReturning(task, task.Titel, task.Description, task.Deadline, task.Prio);
            }
            else
            {
                GoIntoTaskReturningEdit();
            }
           
            Destroy(gameObject);
        }

      
    }

    /// ///Event
    public void SubscribeToEvents_Start()
    {
        Subject.current.OnScrollPopUp += MoveRect;
        
        
    }
    public void UnsubscribeToAllEvents()
    {
        Subject.current.OnScrollPopUp -= MoveRect;
    }
    private void OnDisable()
    {
        UnsubscribeToAllEvents();

       
    }
}