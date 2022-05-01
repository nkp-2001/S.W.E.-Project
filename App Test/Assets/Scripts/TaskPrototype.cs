using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaskPrototype : MonoBehaviour, IObserver
{   //!nicht die Child Strukur anfassen
    private Taskmaster taskMaster;
    private Taskmaster.Task task;
    RectTransform rect;

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

        dircTobj.GetComponentInChildren<TextMeshProUGUI>().text = "Priotät: " + t.Prio + "\n Discirption: " + t.Description;
        dircTobj.SetActive(false);

        transform.SetParent(taskContainer);
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);
    }

    public void SetTaskToDone()
    {

        for (int index = transform.GetSiblingIndex(); index >= 0; index--) //Bug Verhinderer 
        {
            transform.parent.GetChild(index).GetComponent<TaskPrototype>().HideDescription();
        }

        taskMaster.RemoveTask(task);
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
        print(transform.GetSiblingIndex());
        Subject.current.Trigger_ScrollPopUp(transform.GetSiblingIndex(), true);
        //for (int index = transform.GetSiblingIndex() - 1; index >= 0; index--)
        //{

        //    RectTransform rect = transform.parent.GetChild(index).GetComponent<RectTransform>();
        //    rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y + 900);
        //}


    }

    public void HideDescription()
    {
        if (dircTobj.activeSelf == true)
        {
            dircTobj.SetActive(false);
            print(transform.GetSiblingIndex());
            Subject.current.Trigger_ScrollPopUp(transform.GetSiblingIndex(), false);
            //for (int index = transform.GetSiblingIndex() - 1; index >= 0; index--)
            //{
            //    RectTransform rect = transform.parent.GetChild(index).GetComponent<RectTransform>();
            //    rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y - 900);
            //}
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
   
   
    /// ///
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