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
  
    ////// Events ////////////////////////////////////////////////
   
    public event Action NewTask;
    public void Trigger_NewTask() { if (NewTask != null) {NewTask();}}

    public event Action<int,bool> OnScrollPopUp;
    public void Trigger_ScrollPopUp(int id,bool onoff) { if (OnScrollPopUp != null) {OnScrollPopUp(id,onoff);}}

    

}
