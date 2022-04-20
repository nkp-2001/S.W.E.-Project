using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaskPrototype : MonoBehaviour
{
    private Taskmaster taskMaster;
    private Taskmaster.Task task;

    public void Setup(Taskmaster.Task t, Transform taskContainer)
    {
        task = t;

        transform.Find("TaskPreview").GetComponentInChildren<TextMeshProUGUI>().text = t.Titel;
        transform.SetParent(taskContainer);
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);
    }

    public void SetTaskToDone()
    {
        taskMaster.removeTask(task);
        Destroy(gameObject);
    }
    public void SelfDestroyTest()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        taskMaster = FindObjectOfType<Taskmaster>();
    }
}
