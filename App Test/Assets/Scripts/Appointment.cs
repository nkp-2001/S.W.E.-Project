using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Appointment // : Taskmaster.Task
{
    string titel;
    string desp;
    int[] startTime;
    int[] endTime;
    int repeat;

    int repeattimes; // gucken ob es verwendet wird, wenn ja  AppointmentonThisDay(DateTime selectDay) anpassen

    public Appointment(string t,string d,int[] stT,int[] enT,int reIn)
    {
        titel = t;
        desp = d;
        startTime = stT;
        endTime = enT;
        repeat = reIn;
    }

    public string Titel { get => titel; set => titel = value; }
    public int[] StartTime { get => startTime; set => startTime = value; }
    public int[] EndTime { get => endTime; set => endTime = value; }
    public int Repeat { get => repeat; set => repeat = value; }
    public string Desp { get => desp; set => desp = value; }

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
