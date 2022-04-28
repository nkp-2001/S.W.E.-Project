using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subject : MonoBehaviour
{
    public static Subject current;
   
    public void Awake()
    {
        current = this;
    }
    public event Action NewTask;
    public void Trigger_NewTask()
    {
        if (NewTask != null)
        {
            NewTask();
        }
    }


}
