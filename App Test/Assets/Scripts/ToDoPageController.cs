using TMPro;
using UnityEngine;


public class ToDoPageController : MonoBehaviour
{

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

    void Start()
    {
        //fill tasks with existing tasks
    }
}
