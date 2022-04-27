using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DatePicker : MonoBehaviour, IObserver<int>
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

    private void InitializeDropdowns()
    {
        DateTime currentDateTime = DateTime.Now;

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

        List<string> dayOptions = GetStringListOfDaysInMonth(currentDateTime.Year, currentDateTime.Month);

        List<string> monthOptions = new List<string>();
        for (int month = 1; month <= 12; ++month)
        {
            monthOptions.Add(month.ToString());
        }

        List<string> yearOptions = new List<string>();
        for (int y = currentDateTime.Year; y <= currentDateTime.Year + 100; ++y)
        {
            yearOptions.Add(y.ToString());
        }

        hourDropdown.SetOptions(hourOptions);
        minuteDropdown.SetOptions(minuteOptions);
        dayDropdown.SetOptions(dayOptions);
        monthDropdown.SetOptions(monthOptions);
        yearDropdown.SetOptions(yearOptions);

        monthDropdown.AddObserver(this);
        yearDropdown.AddObserver(this);

        hourDropdown.SetCurrentOption(currentDateTime.Hour.ToString());
        minuteDropdown.SetCurrentOption(currentDateTime.Minute.ToString());
        dayDropdown.SetCurrentOption(currentDateTime.Day.ToString());
        monthDropdown.SetCurrentOption(currentDateTime.Month.ToString());
        yearDropdown.SetCurrentOption(currentDateTime.Year.ToString());

        SetInteractability();
    }

    private List<string> GetStringListOfDaysInMonth(int year, int month)
    {
        List<string> daysInSelectedMonth = new List<string>();

        int numDaysInSelectedMonth = DateTime.DaysInMonth(year, month);
        for (int day = 1; day <= numDaysInSelectedMonth; ++day)
        {
            daysInSelectedMonth.Add(day.ToString());
        }

        return daysInSelectedMonth;
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

    public void Notify<T>(T _)
    {
        DateTime selectedDate = GetSelectedDate();
        List<string> daysInSelectedMonth = GetStringListOfDaysInMonth(selectedDate.Year, selectedDate.Month);
        dayDropdown.SetOptions(daysInSelectedMonth);
    }

    public DateTime GetSelectedDate()
    {
        string dateTimeString = yearDropdown.GetCurrentOption() + "-" + monthDropdown.GetCurrentOption() + "-" + dayDropdown.GetCurrentOption() 
                                    + " " + hourDropdown.GetCurrentOption() + ":" + minuteDropdown.GetCurrentOption() + ":00.0";

        DateTime dateTime;
        bool success = DateTime.TryParse(dateTimeString, out dateTime);

        if (!success)
        {
            return DateTime.Now;
        }

        return dateTime;
    }
}
