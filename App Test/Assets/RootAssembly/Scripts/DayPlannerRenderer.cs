using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayPlannerRenderer : MonoBehaviour
{
    [SerializeField] private int numHoursOnScreen;

    [SerializeField] private GameObject hourSegmentPrefab;
    [SerializeField] private GameObject currentTimeIndicator;

    [SerializeField] private RectTransform dayPlannerEntriesContainer;
    [SerializeField] private RectTransform hourSegmentsContainer;

    [SerializeField] private DayPlannerEntry entryVisualPrototype;
    [SerializeField] private List<Appointment> entries;

    [SerializeField] private DateTime selectedDay;

    private RectTransform rectTransform;

    [SerializeField] TextMeshProUGUI selectedDayTextUI;

    Taskmaster taskmaster;

    bool showingToday = true;


    void Start()
    {
        taskmaster = FindObjectOfType<Taskmaster>();

        rectTransform = GetComponent<RectTransform>();

        InvokeRepeating("UpdateCurrentTimeIndicator", 0, 1);
        InvokeRepeating("UpdateHighlight", 0, 1);

        selectedDay = DateTime.Now;
        
        selectedDayTextUI.text =  selectedDay.ToString("d");

        DisplayHours();
        UpdateDisplay();
    }

    private void DisplayHours()
    {
        for (int i = 0; i < numHoursOnScreen; ++i)
        {
            GameObject hourSegment = Instantiate(hourSegmentPrefab, hourSegmentsContainer);

            RectTransform rectTransform = hourSegment.GetComponent<RectTransform>();

            DateTime hourDateTime = new DateTime(1, 1, 1, i, 0, 0);
            float y = ConvertTimeToNormalizedY(hourDateTime);

            rectTransform.anchorMin = new Vector2(0, y);
            rectTransform.anchorMax = new Vector2(1, y);

            hourSegment.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString("D2") + ":00";
        }

        currentTimeIndicator.transform.SetSiblingIndex(0);
    }

    private void UpdateDisplay()
    {
        for (int i = 0; i < dayPlannerEntriesContainer.childCount; ++i)
        {
            GameObject child = dayPlannerEntriesContainer.GetChild(i).gameObject;
            Destroy(child);
        }

        entries = taskmaster.GiveAppoints_ofThisDay(selectedDay.Date);
        foreach (Appointment entry in entries)
        {
            // DateTime startTime, endTime;

            /*
            if (!DateTime.TryParse(entry.StartTime.ToString(), out startTime))
            {
                throw new Exception("Invalid StartTime in entry");
            }

            if (!DateTime.TryParse(entry.EndTime.ToString(), out endTime))
            {
                throw new Exception("Invalid EndTime in entry");
            }
            */



            float normalizedYstart = ConvertTimeToNormalizedY(entry.StartTimeDT());
            float normalizedYend = ConvertTimeToNormalizedY(entry.EndTimeDT());

            entryVisualPrototype.Instantiate(entry.Titel, entry.StartTimeDT(), entry.EndTimeDT(), normalizedYstart, normalizedYend, dayPlannerEntriesContainer);

        }
    }

    private float ConvertTimeToNormalizedY(DateTime dateTime)
    {
        float h = rectTransform.rect.height;
        float pixelsPerHour = h / numHoursOnScreen;
        float pixelsPerMinute = pixelsPerHour / 60;

        float y = dateTime.Hour * pixelsPerHour + dateTime.Minute * pixelsPerMinute;

        float normalizedY = 1 - y / h;

        return normalizedY;
    }

    private void UpdateCurrentTimeIndicator()
    {
        if (showingToday)
        {
            float y = ConvertTimeToNormalizedY(DateTime.Now) * rectTransform.rect.height;
            currentTimeIndicator.GetComponent<RectTransform>().position = new Vector3(0, y); // Für GetComponent<RectTransform>() sollte ein Feld gemacht werden 

            int indexOfLastSibling = transform.childCount - 1;
            currentTimeIndicator.transform.SetSiblingIndex(indexOfLastSibling);
        }
        else
        {
            currentTimeIndicator.GetComponent<RectTransform>().position = new Vector3(0, 0); // Hier sollte ein Feld gemacht werden 
        }
    }

    private void UpdateHighlight()
    {
        if (showingToday)
        {
            for (int i = 0; i < dayPlannerEntriesContainer.childCount; ++i)
            {
                DayPlannerEntry entry = dayPlannerEntriesContainer.GetChild(i).GetComponent<DayPlannerEntry>();

                if (entry.StartTime < DateTime.Now && entry.EndTime > DateTime.Now)
                {
                    entry.SetHighlight(true);
                }
                else
                {
                    entry.SetHighlight(false);
                }
            }
        }
        
    }
    public void SelectNewDay(bool nextDay)
    {
        print("Pressed");
        if (nextDay)
        {
            selectedDay =  selectedDay.AddDays(1);
          
        }
        else
        {
            selectedDay = selectedDay.AddDays(-1);
        }
        UpdateDisplay();
        selectedDayTextUI.text = selectedDay.ToString("d");
        
        showingToday = selectedDay.Date == (DateTime.Now.Date);
        print(showingToday);
        UpdateCurrentTimeIndicator();
        UpdateHighlight();

    }
}