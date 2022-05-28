using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveObject
{
    [SerializeField] List<Task> tasklist = new List<Task>();
    [SerializeField] List<Task> archivedTasks = new List<Task>();
    [SerializeField] List<Task> repeatingTaskOnWait = new List<Task>(); // 

    [SerializeField] List<Appointment> appointmentlist = new List<Appointment>(); // gucken ob auf einen sepaertes gespeicherten Objeckt zu verschieben.

    public void AddNewToList(Task addT)
    {
        tasklist.Add(addT);
    }
   
    public int RemoveFromList(int i)
    {
        tasklist.RemoveAt(i);
        return tasklist.Count;
    }
    public int RemoveFromListAndGiveCount(Task tk) // veraltet 
    {
        tasklist.Remove(tk);
        return tasklist.Count;
    }
    public void RemoveFromList(Task tk)
    {
        tasklist.Remove(tk);
        if (tk.NextDeadlineIndex == 0)
        {
            archivedTasks.Add(tk);
        }
        else
        {
            repeatingTaskOnWait.Add(tk);
        }

    }
    public void RemoveFromArchieList(Task tk)
    {
        archivedTasks.Remove(tk);
    }
    public void RemoveFromWaitList(Task tk)
    {
        repeatingTaskOnWait.Remove(tk);
    }

    public void AddNewAppointment(Appointment appo)
    {
        appointmentlist.Add(appo);
    }
    public void RemoveAppointment(Appointment appo)
    {
        appointmentlist.Remove(appo);
    }

    public List<Task> GetList()
    {
        return tasklist;
    }
    public List<Task> GetArchivedList()
    {
        return archivedTasks;
    }
    public List<Task> GetWaitingList()
    {
        return repeatingTaskOnWait;
    }
    public void ClearArchviedList()
    {
        archivedTasks.Clear();
    }
    public List<Appointment> GetAppoitmentList()
    {
        return appointmentlist;
    }
    public void ClearCurrentList()
    {
        tasklist.Clear();
    }
    public void ChangeTask(Task altertT, string t, string d, int[] dt, float p, int id, int rindex)
    {
        int index = tasklist.FindLastIndex(task => task.Titel == altertT.Titel); //Kann nur klappen wenn allles Unterschidlich , dewegen Avoiddoubblename !!
        tasklist[index] = new Task(t, d, dt, p, id, rindex);
        tasklist[index].Redo = rindex != 0;
    }
    public void ChangeTaskCauseRepeat(Task altertT, int[] newDealine,int newDeadlineID)
    {
        int index = tasklist.FindLastIndex(task => task.Titel == altertT.Titel);
        tasklist[index].DeadlineChannel_ID = newDeadlineID;
        tasklist[index].Deadline = newDealine;
        tasklist[index].Failedprevios = true;
        tasklist[index].Failedtimes++;
    }

    public void ChangeAppointment(Appointment appo, string titel, string desp, int[] startTime, int[] endTime, int repeat, int notiID)
    {
        int index = tasklist.FindLastIndex(appoT => appoT.Titel == appo.Titel);
        appointmentlist[index] = new Appointment(titel, desp, startTime, endTime,repeat,notiID);

    }
}