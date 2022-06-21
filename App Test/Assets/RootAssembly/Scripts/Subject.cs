using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subject : MonoBehaviour
{
    public static Subject current;

    public void Awake()
    {
        Subject[] objs = FindObjectsOfType<Subject>(); //Scenenwechesel loescht es nicht 

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

    public event Action<int, bool> OnScrollPopUp;
    public void Trigger_ScrollPopUp(int id, bool onoff) { if (OnScrollPopUp != null) { OnScrollPopUp(id, onoff); } }

    public event Action OnExpiredDealine;
    public void Trigger_ExpiredDeadline() { if (OnExpiredDealine != null) { OnExpiredDealine(); } }

    public event Action<Task> OnTaskSetDone;
    public void Trigger_TaskSetDone(Task doneTask) { if (OnTaskSetDone != null) { OnTaskSetDone(doneTask); } }

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

    public event Action<Appointment> OnDeleteAppointment;
    public void TriggerOnDeleteAppointment(Appointment oldAppointment) { OnDeleteAppointment?.Invoke(oldAppointment); }

    public event Action OnDateInPast;
    public void TriggerOnDateInPast() { OnDateInPast?.Invoke(); }



}
