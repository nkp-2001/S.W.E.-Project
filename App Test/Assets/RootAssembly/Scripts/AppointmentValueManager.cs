using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class AppointmentValueManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField title;
    [SerializeField] private TMP_InputField description;

    [SerializeField] private DatePicker startTimePicker;
    [SerializeField] private DatePicker endTimePicker;

    public static Appointment underlyingAppointment = null;

    [SerializeField] private TextMeshProUGUI HeaderText;
    [SerializeField] private TextMeshProUGUI ButtonText;
    [SerializeField] private RepeatDropdown repeatDropDown;
    [SerializeField] private Button delete;

    void Start()
    {
        delete.gameObject.SetActive(false);

        if (underlyingAppointment != null)
        {
            StartEditMode();
        }
    }

    private void OnDestroy()
    {
        StopFromEditMode();
    }

    public void CreateAppointmentAndValidate()
    {
        if ((title.text == ""))
        {
            return;
        }

        DateTime startTime = startTimePicker.GetSelectedDate();
        DateTime endTime = endTimePicker.GetSelectedDate();

        if (endTime < DateTime.Now)
        {
            Subject.current.TriggerOnDateInPast();
            return;
        }

        int[] start = DataMaster.ConvertDatetimeToIntArray(startTime);
        int[] end = DataMaster.ConvertDatetimeToIntArray(endTime); 

        if (underlyingAppointment == null)
        {
            Subject.current.TriggerOnNewAppointment(title.text, description.text, start, end, repeatDropDown.GetDays(), 0, new int[] { 0 });
        }
        else
        {
            Subject.current.TriggerOnAppointmentChange(underlyingAppointment, title.text, description.text, start, end, repeatDropDown.GetDays(), 0, new int[] { 0 });
            StopFromEditMode();
        }

        SceneLoader.Load(SceneLoader.Scene.MainPage);
    }

    public void DeleteAppointment()
    {
        Subject.current.TriggerOnDeleteAppointment(underlyingAppointment);
    }

    public void StartEditMode()
    {
        title.text = underlyingAppointment.Title;
        description.text = underlyingAppointment.Description;
        
        repeatDropDown.SetDays(underlyingAppointment.Repeat);

        startTimePicker.SetSelectedDate(underlyingAppointment.StartTime);
        endTimePicker.SetSelectedDate(underlyingAppointment.EndTime);

        HeaderText.text = "Edit Appointment";
        ButtonText.text = "Save Changes";

        delete.gameObject.SetActive(true);
    }

    private void StopFromEditMode()
    {
        underlyingAppointment = null;
    }
}
