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



    public event Action<int, bool> OnScrollPopUp;
    public void Trigger_ScrollPopUp(int id, bool onoff) { if (OnScrollPopUp != null) { OnScrollPopUp(id, onoff); } }

    public event Action OnExpiredDealine;
    public void Trigger_ExpiredDeadline() { if (OnExpiredDealine != null) { OnExpiredDealine(); } }

    public event Action<Task> OnTaskSetDone;
    public void Trigger_TaskSetDone(Task doneTask) { print("Task Set "); if (OnTaskSetDone != null) { print("Task Set Done"); OnTaskSetDone(doneTask); } }

    ////////////////////////////////////////////////////////////////////////////////

    public event Action<string, string, int[], float, int> OnNewTask;
    public void Trigger_OnNewTask(string t, string d, int[] dt, float prio, int repeatindex) { if (OnNewTask != null) { OnNewTask(t, d, dt, prio, repeatindex); } }

    public event Action<Task, string, string, int[], float, int> OnTaskChange;
    public void TriggerOnTaskChange(Task oldtask, string t, string d, int[] dt, float p, int repeatIndex) { if (OnTaskChange != null) { OnTaskChange(oldtask, t, d, dt, p, repeatIndex); } }

    public event Action<Task, string, string, int[], float, int> OnTaskReturning; // 
    public void Trigger_OnTaskReturning(Task oldtask, string potNewname, string potNewDiscp, int[] potNewDt, float potNewPrioint, int repeatIndex)
    { if (OnTaskReturning != null) { OnTaskReturning(oldtask, potNewname, potNewDiscp, potNewDt, potNewPrioint, repeatIndex); } }


    public event Action<string, string, int[], int[], int, int, int[]> OnNewAppointment;
    public void TriggerOnNewAppointment(string title, string description, int[] startTime, int[] endTime, int repeatindex, int repeatTimes, int[] preW) { OnNewAppointment?.Invoke(title, description, startTime, endTime, repeatindex, repeatTimes, preW); }

    public event Action<Appointment, string, string, int[], int[], int, int, int[]> OnAppointmentChange;
    public void TriggerOnAppointmentChange(Appointment oldAppointment, string title, string description, int[] startTime, int[] endTime, int repeatindex, int repeatTimes, int[] preW) { OnAppointmentChange?.Invoke(oldAppointment, title, description, startTime, endTime, repeatindex, repeatTimes, preW); }


    ///funcs
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private Func<string, DateTime, int> onRequest_NotiIDDeadline;

    public void SetonRequest_NotiIDDeadline(Func<string, DateTime, int> delegt) { onRequest_NotiIDDeadline = delegt; }

    private Func<DateTime, DateTime, int, string,int> onRequest_NotiIAppointment;
    public void SetonRequest_NotiIDAppointment(Func<DateTime, DateTime, int, string,int> delegt)  { onRequest_NotiIAppointment = delegt; }
   



    public int Trigger_Request_NotiID(string titel, DateTime expireTime)
    {
        if (onRequest_NotiIDDeadline != null)
        {
            return onRequest_NotiIDDeadline(titel, expireTime);
        }
        return 0; //= not plugged
    }
    public int Triggeer_Reques_NotiIDAppointment(DateTime stD, DateTime endD, int rep, string Titel)
    {
        if (onRequest_NotiIAppointment != null)
        {
            return onRequest_NotiIAppointment(stD, endD, rep, Titel);
        }
        return 0; // not plugged
    }
    




}
