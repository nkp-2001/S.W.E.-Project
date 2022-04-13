using TMPro;
using UnityEngine;


public class ToDoPageController : MonoBehaviour
{
    private Taskmaster taskmaster;

    [SerializeField] private Transform taskContainer;
    [SerializeField] private GameObject taskPrototype;

    public void AddTask(Taskmaster.Task t) ///TODO: string should be an actual task object
    {
        ///TODO: make prototype contruct instance
        GameObject tp = Instantiate(taskPrototype);
        tp.GetComponent<TaskPrototype>().Setup(t, taskContainer);
    }

    private void FetchTasks()
    {
        //clear and fill UI with existing tasks
        for(int i = 0; i < taskContainer.childCount; ++i)
        {
            Destroy(taskContainer.GetChild(i).gameObject);
        }

        if (taskmaster.GetTasks() is not null)
        {
            foreach (Taskmaster.Task task in taskmaster.GetTasks())
            {
                AddTask(task);
            }
        }
    }

    void Start()
    {
        FetchTasks();
    }

    private void OnEnable()
    {
        taskmaster = FindObjectOfType<Taskmaster>();
        FetchTasks();
    }
}
