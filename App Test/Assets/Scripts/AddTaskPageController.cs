using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AddTaskPageController : MonoBehaviour
{
    [SerializeField] private TaskPlaceholder tasks;
    [SerializeField] private TMP_InputField input;
    public void AddTask()
    {
        Debug.Log(input.text);
        tasks.tasks.Add(input.text);
    }
}
