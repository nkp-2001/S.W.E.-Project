using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskPrototype : MonoBehaviour
{

    public float GetHeight()
    {
        return transform.Find("TaskPreview").GetComponent<RectTransform>().rect.height;
    }

    public RectTransform GetRectTransform()
    {
        return transform.Find("TaskPreview").GetComponent<RectTransform>();
    }

    public void SetTaskToDone()
    {
        Destroy(gameObject);
        ///TODO: handle underlying task object
    }
}
