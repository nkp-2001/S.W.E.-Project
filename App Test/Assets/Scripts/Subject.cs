using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subject : MonoBehaviour
{
    public static Subject current;
   
    public void Awake() 
    {
        Subject[] objs = FindObjectsOfType<Subject>(); //Singleton , Scenenwechesel loescht es nicht 

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
        if (current == null)
        {
            current = this;
        }
        
    }
  
    ////// Events ////////////////////////////////////////////////
   
   

    public event Action<int,bool> OnScrollPopUp;
    public void Trigger_ScrollPopUp(int id,bool onoff) { if (OnScrollPopUp != null) {OnScrollPopUp(id,onoff);}}

    public event Action OnExpiredDealine;
    public void Trigger_ExpiredDeadline() { if (OnExpiredDealine != null) { OnExpiredDealine(); } }

    public event Action<Taskmaster.Task> OnTaskSetDone; 
    public void Trigger_TaskSetDone(Taskmaster.Task doneTask) { print("Task Set "); if (OnTaskSetDone != null) { print("Task Set Done" ); OnTaskSetDone(doneTask); } }

    ////////////////////////////////////////////////////////////////////////////////

    public event Action<string, string, int[], float> OnNewTask;
    public void Trigger_OnNewTask(string t, string d, int[] dt, float prio ) { if (OnNewTask != null) { OnNewTask(t, d,  dt, prio); } }

    public event Action<Taskmaster.Task,string,string,int[], float > OnTaskChange;
    public void TriggerOnTaskChange(Taskmaster.Task oldtask, string t, string d, int[] dt, float p) { if (OnTaskChange != null) { OnTaskChange(oldtask, t, d, dt, p); } }


    ///funcs
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private Func<string , DateTime ,int> onRequest_NotiID;
    public void SetonRequest_NotiID(Func<string ,DateTime, int> delegt)
    {
        onRequest_NotiID = delegt;
    
    }
    public int Trigger_Request_NotiID(string titel, DateTime expireTime)
    {
        if (onRequest_NotiID != null)
        {
            return onRequest_NotiID(titel, expireTime);
        }
        return 0; //= not plugged
    }
    




}
