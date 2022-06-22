using System;
using System.Collections.Generic;
using UnityEngine;

public class DatePicker : MonoBehaviour
{
    [SerializeField] private TimeDropdownField hourDropdown, minuteDropdown, dayDropdown, monthDropdown, yearDropdown;

    void OnEnable()
    {
        InitializeDropdowns();
    }

    private void InitializeDropdowns()
    {
        hourDropdown.Initialize();
        minuteDropdown.Initialize();
        dayDropdown.Initialize();
        monthDropdown.Initialize();
        yearDropdown.Initialize();

        DateTime currentDateTime = DateTime.Now;

        List<string> hourOptions = new();
        for (int h = 0; h <= 23; ++h)
        {
            hourOptions.Add(h.ToString());
        }

        List<string> minuteOptions = new();
        for (int m = 0; m <= 59; ++m)
        {
            minuteOptions.Add(m.ToString());
        }

        List<string> dayOptions = GetStringListOfDaysInMonth(currentDateTime.Year, currentDateTime.Month);

        List<string> monthOptions = new();
        for (int month = 1; month <= 12; ++month)
        {
            monthOptions.Add(month.ToString());
        }

        List<string> yearOptions = new();
        for (int y = currentDateTime.Year; y <= currentDateTime.Year + 100; ++y)
        {
            yearOptions.Add(y.ToString());
        }

        hourDropdown.SetOptions(hourOptions);
        minuteDropdown.SetOptions(minuteOptions);
        dayDropdown.SetOptions(dayOptions);
        monthDropdown.SetOptions(monthOptions);
        yearDropdown.SetOptions(yearOptions);

        hourDropdown.SetCurrentOption(currentDateTime.Hour.ToString());
        minuteDropdown.SetCurrentOption(currentDateTime.Minute.ToString());
        dayDropdown.SetCurrentOption(currentDateTime.Day.ToString());
        monthDropdown.SetCurrentOption(currentDateTime.Month.ToString());
        yearDropdown.SetCurrentOption(currentDateTime.Year.ToString());
    }

    private List<string> GetStringListOfDaysInMonth(int year, int month)
    {
        List<string> daysInSelectedMonth = new();

        int numDaysInSelectedMonth = DateTime.DaysInMonth(year, month);
        for (int day = 1; day <= numDaysInSelectedMonth; ++day)
        {
            daysInSelectedMonth.Add(day.ToString());
        }

        return daysInSelectedMonth;
    }

    public void OnInteractibleChanged(bool on)
    {
        TimeDropdownField[] dropdowns = { hourDropdown, minuteDropdown, dayDropdown, monthDropdown, yearDropdown };
        foreach (TimeDropdownField dropdown in dropdowns)
        {
            dropdown.SetInteractible(on);
        }
    }

    public void UpdateDaysList()
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
    
    public void SetSelectedDate(int[] dt)
    {
        DateTime datetime = new(dt[4], dt[3], dt[2], dt[1], dt[0], 0);

        hourDropdown.SetCurrentOption(datetime.Hour.ToString());
        minuteDropdown.SetCurrentOption(datetime.Minute.ToString());
        dayDropdown.SetCurrentOption(datetime.Day.ToString());
        monthDropdown.SetCurrentOption(datetime.Month.ToString());
        yearDropdown.SetCurrentOption(datetime.Year.ToString());
    }
}
