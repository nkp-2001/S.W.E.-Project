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

    public event Action OnExpiredDealine;
    public void Trigger_ExpiredDeadline() { if (OnExpiredDealine != null) { OnExpiredDealine(); } }
    

    


    ///Test funcs
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private Func<int> onRequest_NotiID;
    public void SetonRequest_NotiID(Func<int> delegt)
    {
        onRequest_NotiID = delegt;
    }
    public int Trigger_Request_NotiID()
    {
        if (onRequest_NotiID != null)
        {
            return onRequest_NotiID();
        }
        return 0; //= not plugged
    }




}
