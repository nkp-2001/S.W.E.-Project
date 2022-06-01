using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Appointment // : Taskmaster.Task
{
   [SerializeField] string titel;
    [SerializeField] string desp;
    [SerializeField] int[] startTime;
    [SerializeField] int[] endTime;
    [SerializeField] int repeat;

    [SerializeField] int notifcation_id;


    [SerializeField] int repeattimes; // gucken ob es verwendet wird, wenn ja  AppointmentonThisDay(DateTime selectDay) anpassen
   
    public Appointment(string t,string d,int[] stT,int[] enT,int reIn, int notficID)
    {
        titel = t;
        desp = d;
        startTime = stT;
        endTime = enT;
        repeat = reIn;
        notifcation_id = notficID;
    }

    public string Titel { get => titel; set => titel = value; }
    public int[] StartTime { get => startTime; set => startTime = value; }
    public int[] EndTime { get => endTime; set => endTime = value; }
    public int Repeat { get => repeat; set => repeat = value; }
    public string Desp { get => desp; set => desp = value; }
    public int Notifcation_id { get => notifcation_id; set => notifcation_id = value; }

    public bool AppointmentonThisDay(DateTime selectDay)
    {
        DateTime CurrenStart = new DateTime(startTime[4], startTime[3], startTime[2]);
        
        if (selectDay == CurrenStart)
        {
            return true;
            
        }
        if ((repeat != 0) & (selectDay >= CurrenStart))
        {          
            int daydiff = (selectDay - CurrenStart).Days;
            if (daydiff % repeat == 0)
            {
                return true;
            } 
        }
        return false;


    }





    /*
    public Appointment(string t, string d, int[] dt, float p, int dlID, int retDtDayes)
    {
        
        this.Titel = t;
        this.Description = d;
        this.Deadline = dt;
        this.DeadlineChannel_ID = dlID;
        this.NextDeadlineIndex = retDtDayes;
    }
    */

}
