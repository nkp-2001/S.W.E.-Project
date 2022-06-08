using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppointmentValueManager : ValueManager
{

    [SerializeField] private DatePicker endTimePicker;

    public void CreateAppointmentAndValidate()
    {
        if ((titel.text == ""))
        {
            return;
        }

        DateTime dtraw = datePicker.GetSelectedDate();
        int[] startTime = new int[] { dtraw.Minute, dtraw.Hour, dtraw.Day, dtraw.Month, dtraw.Year };

        dtraw = endTimePicker.GetSelectedDate();
        int[] endTime = new int[] { dtraw.Minute, dtraw.Hour, dtraw.Day, dtraw.Month, dtraw.Year };


        int repeatIndex = repeatDropDown.value;

        Subject.current.TriggerOnNewAppointment(titel.text, discrip.text, startTime, endTime, repeatIndex, 0, new int[] { 0 });
        /*else
        {
            if (!tastReturninEdit)
            {
                Subject.current.TriggerOnAppointmentChange(taskOnEdit, titel.text, discrip.text, dt, prio.value, repeatIndex);
            }
            else
            {
                Subject.current.TriggerOnAppointmentReturning(taskOnEdit, titel.text, discrip.text, dt, prio.value, repeatIndex);
                tastReturninEdit = false;
            }

            StopFromEditMode();
        }*/
        sceneLoader.LoadScene(0);
    }
}
