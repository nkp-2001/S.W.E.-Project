using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AppointmentValueManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField title;
    [SerializeField] private TMP_InputField description;

    [SerializeField] private DatePicker startTimePicker;
    [SerializeField] private DatePicker endTimePicker;

    public static Appointment underlyingAppointment = null;

    [SerializeField] private TextMeshProUGUI HeaderText;
    [SerializeField] private TextMeshProUGUI ButtonText;
    [SerializeField] private TMP_Dropdown repeatDropDown;
    [SerializeField] private Button delete;

    private SceneLoader sceneLoader;

    void Start()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();

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

        DateTime dateTime = startTimePicker.GetSelectedDate();
        int[] startTime = new int[] { dateTime.Minute, dateTime.Hour, dateTime.Day, dateTime.Month, dateTime.Year };

        dateTime = endTimePicker.GetSelectedDate();
        int[] endTime = new int[] { dateTime.Minute, dateTime.Hour, dateTime.Day, dateTime.Month, dateTime.Year };


        int repeatIndex = repeatDropDown.value;

        if (underlyingAppointment == null)
        {
            Subject.current.TriggerOnNewAppointment(title.text, description.text, startTime, endTime, repeatIndex, 0, new int[] { 0 });
        }
        else
        {
            Subject.current.TriggerOnAppointmentChange(underlyingAppointment, title.text, description.text, startTime, endTime, repeatIndex, 0, new int[] { 0 });
            StopFromEditMode();
        }
        sceneLoader.LoadScene(0);
    }

    public void DeleteAppointment()
    {
        Subject.current.TriggerOnDeleteAppointment(underlyingAppointment);
    }

    public void StartEditMode()
    {
        title.text = underlyingAppointment.Title;
        description.text = underlyingAppointment.Description;
        repeatDropDown.value = underlyingAppointment.Repeat;

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
