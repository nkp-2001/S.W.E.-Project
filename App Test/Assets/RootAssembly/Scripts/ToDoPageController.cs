using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ToDoPageController : MonoBehaviour,IObserver
{
    private Taskmaster taskmaster;

    [SerializeField] private Transform taskContainer;
    [SerializeField] private GameObject taskPrototype;
    [SerializeField] bool ShowOldTask = false;
    [SerializeField] TextMeshProUGUI ButtonText;
    TMPro.TMP_Dropdown sortByDropdown;

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
            //clear and fill UI with existing tasks   
            ClearScrollView();
            print(taskmaster);
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
    public void FetchTasks(Task task, bool prob)
    {
        FetchTasks();
    }


    void Start()
    {     
       
        SubscribeToEvents_Start();
        FetchTasks();
        StartCoroutine(FetchUpdate());
    }

    private void OnEnable()
    {
        sortByDropdown = transform.GetComponentInChildren<TMPro.TMP_Dropdown>();
        print(taskmaster);
        FetchTasks(); 
    }
    private void Awake()
    {
        taskmaster = FindObjectOfType<Taskmaster>();
    }

    IEnumerator FetchUpdate()
    {
        while (true)
        {            
            yield return new WaitForSeconds(60);
            FetchTasks();
        }
       
    }

    public void SubscribeToEvents_Start()
    {
        Subject.current.OnExpiredDealine += FetchTasks;
        Subject.current.OnTaskReturning += FetchTasks;
    }

    public void UnsubscribeToAllEvents()
    {
        Subject.current.OnExpiredDealine -= FetchTasks;
        Subject.current.OnTaskReturning -= FetchTasks;
    }
    private void OnDisable()
    {
        UnsubscribeToAllEvents();
    }
}
