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


    [SerializeField] int repeattimes = 0; 
   
    public Appointment(string t,string d,int[] stT,int[] enT,int reIn, int notficID)
    {
        titel = t;
        desp = d;
        startTime = stT;
        endTime = enT;
        repeat = reIn;
        notifcation_id = notficID;
    }
    public Appointment(string t, string d, int[] stT, int[] enT, int reIn, int notficID,int repTimes)
    {
        titel = t;
        desp = d;
        startTime = stT;
        endTime = enT;
        repeat = reIn;
        notifcation_id = notficID;
        repeattimes = repTimes;
    }



    public string Titel { get => titel; set => titel = value; }
    public int[] StartTime { get => startTime; set => startTime = value; }
    public int[] EndTime { get => endTime; set => endTime = value; }

    public System.DateTime StartTimeDT()
    {
        return ConvertIntArray_toDatetime(StartTime);
    }
    public System.DateTime EndTimeDT()
    {
        return ConvertIntArray_toDatetime(EndTime);
    }

    public int Repeat { get => repeat; set => repeat = value; }
    public string Desp { get => desp; set => desp = value; }
    public int Notifcation_id { get => notifcation_id; set => notifcation_id = value; }
    public int Repeattimes { get => repeattimes; set => repeattimes = value; }

    public bool AppointmentonThisDay(DateTime selectDay)
    {
        DateTime CurrenStart = new DateTime(startTime[4], startTime[3], startTime[2]);

        if (selectDay == CurrenStart)
        {
            return true;
            
        }
        if (repeat == 0)
        {
            return false;
        }
        else
        {
            int daydiff = (selectDay - CurrenStart).Days;
            if (daydiff / repeat > repeattimes & repeattimes != 0)
            {
                return false;
            }
            else if (daydiff % repeat == 0 & (selectDay >= CurrenStart))
            {
                return true;
            }
            return false;

        }    
    }
    public System.DateTime ConvertIntArray_toDatetime(int[] toconvert)
    {
        return new DateTime(toconvert[4], toconvert[3], toconvert[2], toconvert[1], toconvert[0], 0);
    }

}
