using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Appointment
{
    [SerializeField] string title;
    [SerializeField] string description;
    [SerializeField] int[] startTime;
    [SerializeField] int[] endTime;
    [SerializeField] int repeat;
    [SerializeField] int repeattimes = 0;

    [SerializeField] int notifcation_id;

    public Appointment(string t, string d, int[] stT, int[] enT, int reIn, int notficID)
    {
        title = t;
        description = d;
        startTime = stT;
        endTime = enT;
        repeat = reIn;
        notifcation_id = notficID;
    }
    public Appointment(string t, string d, int[] stT, int[] enT, int reIn, int notficID, int repTimes)
    {
        title = t;
        description = d;
        startTime = stT;
        endTime = enT;
        repeat = reIn;
        notifcation_id = notficID;
        repeattimes = repTimes;
    }

    public string Title { get => title; set => title = value; }
    public string Description { get => description; set => description = value; }
    public int[] StartTime { get => startTime; set => startTime = value; }
    public int[] EndTime { get => endTime; set => endTime = value; }
    public int Repeat { get => repeat; set => repeat = value; }
    public int Repeattimes { get => repeattimes; set => repeattimes = value; }
    public int Notifcation_id { get => notifcation_id; set => notifcation_id = value; }

    public DateTime StartTimeDT()
    {
        return ConvertIntArrayToDatetime(StartTime);
    }
    public DateTime EndTimeDT()
    {
        return ConvertIntArrayToDatetime(EndTime);
    }

    public bool AppointmentonThisDay(DateTime selectDay)
    {
        DateTime CurrentStart = new DateTime(startTime[4], startTime[3], startTime[2]);

        if (selectDay == CurrentStart)
        {
            return true;
        }

        if (repeat == 0)
        {
            return false;
        }
        else
        {
            int daydiff = (selectDay - CurrentStart).Days;
            if (daydiff / repeat > repeattimes & repeattimes != 0)
            {
                return false;
            }
            else if (daydiff % repeat == 0 & (selectDay >= CurrentStart))
            {
                return true;
            }
            return false;

        }
    }

    public static DateTime ConvertIntArrayToDatetime(int[] toconvert)
    {
        return new DateTime(toconvert[4], toconvert[3], toconvert[2], toconvert[1], toconvert[0], 0);
    }


    public override bool Equals(object obj)
    {
        if (obj.GetType() != typeof(Task))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public bool Equals(Appointment appos)
    {
        return appos.Title == title && appos.Description == description
            && appos.StartTime.Equals(startTime) && appos.EndTime.Equals(EndTime)
            && appos.repeat == repeat & appos.repeattimes == repeattimes;
    }

}
