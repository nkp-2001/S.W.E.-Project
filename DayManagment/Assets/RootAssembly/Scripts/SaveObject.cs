using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveObject
{
    [SerializeField] private List<Task> tasklist = new();
    [SerializeField] private List<Task> archivedTasks = new();
    [SerializeField] private List<Task> repeatingTaskOnWait = new(); 

    [SerializeField] private List<Appointment> appointmentlist = new(); 

    public void AddNewToList(Task addT)
    {
        tasklist.Add(addT);
    }
   
    public int RemoveFromList(int i)
    {
        tasklist.RemoveAt(i);
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

    public void RemoveFromArchiveList(Task tk)
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

    public void RemoveAppointment(Appointment appointment)
    {
        for(int i = 0; i < appointmentlist.Count; ++i)
        {
            if (appointmentlist[i].Equals(appointment))
            {
                appointmentlist.RemoveAt(i);
            }
        }
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

    public void ClearArchiveList()
    {
        archivedTasks.Clear();
    }

    public List<Appointment> GetAppointmentList()
    {
        return appointmentlist;
    }
    public void ClearCurrentList()
    {
        tasklist.Clear();
    }

    public void ChangeTask(Task altertT, string t, string d, int[] dt, float p, int id, int rindex)
    {
        int index = tasklist.FindLastIndex(task => task.Title == altertT.Title); 
        tasklist[index] = new Task(t, d, dt, p, id, rindex);
        tasklist[index].Redo = rindex != 0;
    }

    public void ChangeTaskCauseRepeat(Task altertT, int[] newDealine,int newDeadlineID)
    {
        int index = tasklist.FindLastIndex(task => task.Title == altertT.Title);
        tasklist[index].DeadlineChannelId = newDeadlineID;
        tasklist[index].Deadline = newDealine;
        tasklist[index].FailedPrevious = true;
        tasklist[index].FailedTimes++;
    }

    public void ChangeAppointment(Appointment appo, string titel, string desp, int[] startTime, int[] endTime, int repeat, int notiID, int repeatTimes)
    {
        int index = appointmentlist.FindLastIndex(appoT => appoT.Title == appo.Title);
        appointmentlist[index] = new Appointment(titel, desp, startTime, endTime,repeat,notiID,repeatTimes);

    }
}