using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ToDoPageController : MonoBehaviour,IObserver
{
    private DataMaster taskmaster;

    [SerializeField] private Transform taskContainer;
    [SerializeField] private GameObject taskPrototype;
    [SerializeField] private bool ShowOldTask = false;
    [SerializeField] private TextMeshProUGUI ButtonText;
    [SerializeField] private TMP_Dropdown sortByDropdown;

    public void AddTask(Task t)
    {
        GameObject tp = Instantiate(taskPrototype);
        tp.GetComponent<TaskPrototype>().Setup(t, taskContainer); 
    }

    public void AddOldTask(Task t)
    {
        GameObject tp = Instantiate(taskPrototype);
        tp.GetComponent<TaskPrototype>().Setup_OldTask(t, taskContainer);
    }

    public void FetchTasks()
    {
        if (!ShowOldTask)
        {
            ClearScrollView();

            List<Task> tasks = taskmaster.GetSortedTasks(sortByDropdown.value);

            if (tasks is not null)
            {
                foreach (Task task in tasks)
                {
                    AddTask(task);
                }
            }
        }
        else
        {
            FetchOldTask();
        } 
    }

    private void ClearScrollView()
    {
        for (int i = 0; i < taskContainer.childCount; i++)
        {
            Destroy(taskContainer.transform.GetChild(i).gameObject);
        }
    }

    public void ShowarchivedTask()
    {
        if (!ShowOldTask)
        {
            FetchOldTask();
            ShowOldTask = true;
            ButtonText.text = "Return to main Page";
        }
        else
        {
            ShowOldTask = false;
            FetchTasks();
            ButtonText.text = "Show archived Task";
        }
    }

    public void FetchOldTask()
    {
        ClearScrollView();

        if (taskmaster.GetArchivedTasks() is not null)
        {
            foreach (Task task in taskmaster.GetArchivedTasks())
            {
                AddOldTask(task);
            }
        }
    }

    public void FetchTasks(Task task, string t, string d, int[] dt, float prio,int i)
    {
        FetchTasks();
    }

    void Start()
    {     
        SubscribeToEvents();
        FetchTasks();
        StartCoroutine(FetchUpdate());
    }

    private void OnEnable()
    {
        taskmaster = FindObjectOfType<DataMaster>();
        FetchTasks(); 
    }

    IEnumerator FetchUpdate()
    {
        while (true)
        {            
            yield return new WaitForSeconds(60);
            FetchTasks();
        }
    }

    public void SubscribeToEvents()
    {
        Subject.OnExpiredDealine += FetchTasks;
        Subject.OnTaskReturning += FetchTasks;
    }
    public void UnsubscribeToAllEvents()
    {
        Subject.OnExpiredDealine -= FetchTasks;
        Subject.OnTaskReturning -= FetchTasks;
    }
    private void OnDisable()
    {
        UnsubscribeToAllEvents();
    }
}
