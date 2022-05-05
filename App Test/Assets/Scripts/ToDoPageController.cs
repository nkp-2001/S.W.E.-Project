using UnityEngine;


public class ToDoPageController : MonoBehaviour,IObserver
{
    private Taskmaster taskmaster;

    [SerializeField] private Transform taskContainer;
    [SerializeField] private GameObject taskPrototype;

    public void AddTask(Taskmaster.Task t)
    {
        GameObject tp = Instantiate(taskPrototype);
        tp.GetComponent<TaskPrototype>().Setup(t, taskContainer); 
    }

    public void FetchTasks()
    {
        //clear and fill UI with existing tasks   
        for(int i = 0; i < taskContainer.childCount; i++)
        {
            Destroy(taskContainer.transform.GetChild(i).gameObject);
        }

        if (taskmaster.GetTasks() is not null)
        {
            foreach (Taskmaster.Task task in taskmaster.GetSortedTasks())
            {
                AddTask(task);
            }
        }  
    }

    void Start()
    {
        FetchTasks();
        SubscribeToEvents_Start();
    }

    private void OnEnable()
    {
        taskmaster = FindObjectOfType<Taskmaster>();
        FetchTasks(); //n�tig??
    }

    public void SubscribeToEvents_Start()
    {
        Subject.current.OnExpiredDealine += FetchTasks;
    }

    public void UnsubscribeToAllEvents()
    {
        Subject.current.OnExpiredDealine -= FetchTasks;
    }
    private void OnDisable()
    {
        UnsubscribeToAllEvents();
    }
}
