using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Subject
{
    public static event Action<int, bool> OnScrollPopUp;
    public static void Trigger_ScrollPopUp(int id, bool onoff) { if (OnScrollPopUp != null) { OnScrollPopUp(id, onoff); } }

    public static event Action OnExpiredDealine;
    public static void Trigger_ExpiredDeadline() { if (OnExpiredDealine != null) { OnExpiredDealine(); } }

    public static event Action<Task> OnTaskSetDone;
    public static void Trigger_TaskSetDone(Task doneTask) { if (OnTaskSetDone != null) { OnTaskSetDone(doneTask); } }

    public static event Action<string, string, int[], float, int> OnNewTask;
    public static void Trigger_OnNewTask(string t, string d, int[] dt, float prio, int repeatindex) { if (OnNewTask != null) { OnNewTask(t, d, dt, prio, repeatindex); } }

    public static event Action<Task, string, string, int[], float, int> OnTaskChange;
    public static void TriggerOnTaskChange(Task oldtask, string t, string d, int[] dt, float p, int repeatIndex) { if (OnTaskChange != null) { OnTaskChange(oldtask, t, d, dt, p, repeatIndex); } }

    public static event Action<Task, string, string, int[], float, int> OnTaskReturning; // 
    public static void Trigger_OnTaskReturning(Task oldtask, string potNewname, string potNewDiscp, int[] potNewDt, float potNewPrioint, int repeatIndex)
    { if (OnTaskReturning != null) { OnTaskReturning(oldtask, potNewname, potNewDiscp, potNewDt, potNewPrioint, repeatIndex); } }

    public static event Action<string, string, int[], int[], int, int, int[]> OnNewAppointment;
    public static void TriggerOnNewAppointment(string title, string description, int[] startTime, int[] endTime, int repeatindex, int repeatTimes, int[] preW) { OnNewAppointment?.Invoke(title, description, startTime, endTime, repeatindex, repeatTimes, preW); }

    public static event Action<Appointment, string, string, int[], int[], int, int, int[]> OnAppointmentChange;
    public static void TriggerOnAppointmentChange(Appointment oldAppointment, string title, string description, int[] startTime, int[] endTime, int repeatindex, int repeatTimes, int[] preW) { OnAppointmentChange?.Invoke(oldAppointment, title, description, startTime, endTime, repeatindex, repeatTimes, preW); }

    public static event Action<Appointment> OnDeleteAppointment;
    public static void TriggerOnDeleteAppointment(Appointment oldAppointment) { OnDeleteAppointment?.Invoke(oldAppointment); }

    public static event Action OnDateInPast;
    public static void TriggerOnDateInPast() { OnDateInPast?.Invoke(); }
}
