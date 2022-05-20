using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppointmentMaster : DataMaster
{
    private void Awake()
    {
        LoadList();
    }
    public void CreateNewAppointment(string titel, string desp, int[] startTime, int[] endTime, int repeat)
    {
        titel = AvoidDoubleName(titel);
        int notficID = Subject.current.Triggeer_Reques_NotiIDAppointment(ConvertIntArray_toDatetime(startTime), ConvertIntArray_toDatetime(endTime), repeat, titel);
        if (notficID == 0)
        {
            print("[ManuelWarning] The NotficationSystem is not plugged here. It is either not in the Scene anymore, wasen`t in it in the first place or onRequest_NotiIDAppointent hasen`t been assigend by it");
        }
        dataSave.AddNewAppointment(new Appointment(titel, desp, startTime, endTime, repeat, notficID));
        SaveList();
    }
   
    public void DeleteAppoitment(Appointment appo)
    {
        dataSave.RemoveAppointment(appo);
        SaveList();
    }

    public void ChangeAppointment(Appointment oldAppointment,string titel, string desp, int[] startTime, int[] endTime, int repeat, int notficID)
    {
        dataSave.ChangeAppointment(oldAppointment, titel, desp, startTime, endTime, repeat, notficID);
        SaveList();
    }

    public string AvoidDoubleName(string titel) // ! bzgl Vererbung bedenken 
    {
        string checkedtitel = titel;
        bool doublefound = true;
        int repeating = 0;
        while (doublefound)
        {

            doublefound = false;
            print("Round" + repeating);
            foreach (Appointment appo in dataSave.GetAppoitmentList())
            {
                if (checkedtitel == appo.Titel)
                {
                    print("double");

                    repeating++;
                    checkedtitel = titel + "(" + repeating + ")";
                    doublefound = true;
                    break;
                }
            }

        }
        return checkedtitel;
    }

    public List<Appointment> GiveAppoints_ofThisDay()
    {
        List<Appointment> DayList = new List<Appointment>();
        foreach (Appointment appo in dataSave.GetAppoitmentList())
        {
            if (appo.AppointmentonThisDay(DateTime.Now.Date))
            {
                DayList.Add(appo);
            }
        }
        return DayList;
    }
   
}
