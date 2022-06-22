using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Appointment
{
    [SerializeField] private string title;
    [SerializeField] private string description;
    [SerializeField] private int[] startTime;
    [SerializeField] private int[] endTime;
    [SerializeField] private int repeat;
    [SerializeField] private int repeatTimes = 0;

    [SerializeField] private int notifcationId;

    public Appointment(string t, string d, int[] stT, int[] enT, int reIn, int notficId)
    {
        title = t;
        description = d;
        startTime = stT;
        endTime = enT;
        repeat = reIn;
        notifcationId = notficId;
    }

    public Appointment(string t, string d, int[] stT, int[] enT, int reIn, int notficId, int repTimes)
    {
        title = t;
        description = d;
        startTime = stT;
        endTime = enT;
        repeat = reIn;
        notifcationId = notficId;
        repeatTimes = repTimes;
    }

    public string Title { get => title; set => title = value; }
    public string Description { get => description; set => description = value; }
    public int[] StartTime { get => startTime; set => startTime = value; }
    public int[] EndTime { get => endTime; set => endTime = value; }
    public int Repeat { get => repeat; set => repeat = value; }
    public int RepeatTimes { get => repeatTimes; set => repeatTimes = value; }
    public int NotificationId { get => notifcationId; set => notifcationId = value; }

    public DateTime StartTimeDT()
    {
        return DataMaster.ConvertIntArrayToDateTime(startTime);
    }

    public DateTime EndTimeDT()
    {
        return DataMaster.ConvertIntArrayToDateTime(endTime);
    }

    public bool AppointmentOnThisDay(DateTime selectDay)
    {
        DateTime CurrentStart = new(startTime[4], startTime[3], startTime[2]);

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
            if (daydiff / repeat > repeatTimes & repeatTimes != 0)
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

    public static DateTime ConvertIntArrayToDateTime(int[] toconvert)
    {
        return new DateTime(toconvert[4], toconvert[3], toconvert[2], toconvert[1], toconvert[0], 0);
    }


    public override bool Equals(object obj)
    {
        return this == obj;
    }

    public bool Equals(Appointment appos)
    {
        return appos.Title == title && appos.Description == description
            && appos.StartTime.Equals(startTime) && appos.EndTime.Equals(EndTime)
            && appos.repeat == repeat & appos.repeatTimes == repeatTimes;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
