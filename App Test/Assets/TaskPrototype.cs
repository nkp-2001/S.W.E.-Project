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

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
