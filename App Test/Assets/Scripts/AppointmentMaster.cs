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
    public void CreateNewTask(string titel, string desp, int[] startTime, int[] endTime, int repeat)
    {
        titel = AvoidDoubleName(titel);
        dataSave.AddNewAppointment(new Appointment(titel, desp, startTime, endTime, repeat));
        SaveList();
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
    public void DeleteAppitment(Appointment appo)
    {
        dataSave.RemoveAppointment(appo);
        SaveList();
    }

    public void ChangeAppointment(Appointment oldAppointment,string titel, string desp, int[] startTime, int[] endTime, int repeat)
    {
        dataSave.ChangeAppointment(oldAppointment, titel, desp, startTime, endTime, repeat);
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
}
