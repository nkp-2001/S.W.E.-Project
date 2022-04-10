using TMPro;
using UnityEngine;


public class ToDoPageController : MonoBehaviour
{
    [SerializeField] private TaskPlaceholder tasks;

    [SerializeField] private Transform taskContainer;
    [SerializeField] private GameObject taskPrototype;

    public void AddTask(string t) ///TODO: string should be an actual task object
    {
        ///TODO: make prototype contruct instance
        GameObject newTask = Instantiate(taskPrototype);

        newTask.transform.Find("TaskPreview").GetComponentInChildren<TextMeshProUGUI>().text = t;
        newTask.transform.SetParent(taskContainer);
        newTask.transform.localScale = Vector2.one;
        newTask.SetActive(true);
    }

    private void FetchTasks()
    {
        //clear and fill UI with existing tasks
        for(int i = 0; i < taskContainer.childCount; ++i)
        {
            Destroy(taskContainer.GetChild(i).gameObject);
        }

        foreach (string task in tasks.tasks)
        {
            AddTask(task);
        }
    }

    void Start()
    {
        FetchTasks();
    }

    private void OnEnable()
    {
        FetchTasks();
    }
}
