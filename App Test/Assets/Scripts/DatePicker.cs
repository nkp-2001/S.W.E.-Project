using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DatePicker : MonoBehaviour
{
    [SerializeField] private Toggle isInteractibleToggle;
    [SerializeField] private TimeDropdownField hourDropdown, minuteDropdown, dayDropdown, monthDropdown, yearDropdown;

    void Start()
    {
        InitializeDropdowns();
    }

    void OnEnable()
    {
        InitializeDropdowns();
    }

    public void InitializeDropdowns()
    {
        List<string> hourOptions = new List<string>();
        for (int h = 0; h <= 23; ++h)
        {
            hourOptions.Add(h.ToString());
        }
        List<string> minuteOptions = new List<string>();
        for (int m = 0; m <= 59; ++m)
        {
            minuteOptions.Add(m.ToString());
        }
        List<string> dayOptions = new List<string>();
        for (int d = 1; d <= 31; ++d)
        {
            dayOptions.Add(d.ToString());
        }
        List<string> monthOptions = new List<string>();
        for (int month = 1; month <= 12; ++month)
        {
            monthOptions.Add(month.ToString());
        }
        List<string> yearOptions = new List<string>();
        for (int y = DateTime.Now.Year; y <= DateTime.Now.Year + 100; ++y)
        {
            yearOptions.Add(y.ToString());
        }

        hourDropdown.SetOptions(hourOptions);
        minuteDropdown.SetOptions(minuteOptions);
        dayDropdown.SetOptions(dayOptions);
        monthDropdown.SetOptions(monthOptions);
        yearDropdown.SetOptions(yearOptions);


        DateTime currentDateTime = DateTime.Now;

        hourDropdown.SetCurrentOption(currentDateTime.Hour.ToString());
        minuteDropdown.SetCurrentOption(currentDateTime.Minute.ToString());
        dayDropdown.SetCurrentOption(currentDateTime.Day.ToString());
        monthDropdown.SetCurrentOption(currentDateTime.Month.ToString());
        yearDropdown.SetCurrentOption(currentDateTime.Year.ToString());

        SetInteractability();
    }

    public void SetInteractability()
    {
        TimeDropdownField[] dropdowns = { hourDropdown, minuteDropdown, dayDropdown, monthDropdown, yearDropdown };
        foreach (TimeDropdownField dropdown in dropdowns)
        {
            dropdown.SetInteractible(isInteractibleToggle.isOn);
        }
    }

    public void OnInteractibleChanged()
    {
        SetInteractability();
    }

    public DateTime GetPickedDate()
    {
        string dateTimeString = yearDropdown.GetCurrentOption() + "-" + monthDropdown.GetCurrentOption() + "-" + dayDropdown.GetCurrentOption() 
                                    + " " + hourDropdown.GetCurrentOption() + ":" + minuteDropdown.GetCurrentOption() + ":00.0";

        DateTime dateTime;
        bool success = DateTime.TryParse(dateTimeString, out dateTime);

        return dateTime;
    }
}
