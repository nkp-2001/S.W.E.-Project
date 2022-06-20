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

        DateTime startTime = startTimePicker.GetSelectedDate();
        DateTime endTime = endTimePicker.GetSelectedDate();

        if (endTime < DateTime.Now)
        {
            Subject.current.TriggerOnDateInPast();
            return;
        }

        int[] start = new int[] { startTime.Minute, startTime.Hour, startTime.Day, startTime.Month, startTime.Year };
        int[] end = new int[] { endTime.Minute, endTime.Hour, endTime.Day, endTime.Month, endTime.Year };

        int repeatIndex = repeatDropDown.value;
        int repeat = 0;
        switch (repeatDropDown.value)
        {
            case 1:
                repeat = 7;
                break;
            case 2:
                repeat = 7*2;
                break;
            case 3:
                repeat = 7*4; 
                break;
            case 4:
                repeat = 365;
                break;
            default:
                break;
           
        }



        if (underlyingAppointment == null)
        {
            Subject.current.TriggerOnNewAppointment(title.text, description.text, start, end, repeatIndex, 0, new int[] { 0 });
        }
        else
        {
            Subject.current.TriggerOnAppointmentChange(underlyingAppointment, title.text, description.text, start, end, repeat, 0, new int[] { 0 });
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
        print(underlyingAppointment.Repeat);
        switch (underlyingAppointment.Repeat)
        {
            case 0:
                repeatDropDown.value = 0;
                break;
            case 1:
                repeatDropDown.value = 1;
                break;
            case 7:
                repeatDropDown.value = 2;
                break;
            case 14:
                repeatDropDown.value = 3;
                break;
            case 7*4:
                repeatDropDown.value = 4;
                break;
            case 365:
                repeatDropDown.value = 5;
                break;
            default:
                break;

        }
        


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
