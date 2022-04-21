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

       // transform.Find("TaskPreview").GetComponentInChildren<TextMeshProUGUI>().text = t.Titel;
        transform.Find("TaskPreview").GetComponentsInChildren<TextMeshProUGUI>()[0].text = t.Titel;
       
        if (t.Deadline != null && t.Deadline.Length !=0)
        {
           transform.Find("TaskPreview").GetComponentsInChildren<TextMeshProUGUI>()[1].text = 
                "DT:" + string.Format("{0:00}", t.Deadline[2]) + "." + string.Format("{0:00}", t.Deadline[3]) + "." 
                + t.Deadline[4] + "|" + string.Format("{0:00}", t.Deadline[1]) + ":" + string.Format("{0:00}", t.Deadline[0]);
        }
        
       


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
