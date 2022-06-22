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
    [SerializeField] private float timeIndicatorThickness = 0.001f;

    [SerializeField] private RectTransform dayPlannerEntriesContainer;
    [SerializeField] private RectTransform hourSegmentsContainer;

    [SerializeField] private DayPlannerEntry entryVisualPrototype;
    [SerializeField] private List<Appointment> entries;

    [SerializeField] private DateTime selectedDay;

    private RectTransform rectTransform;

    [SerializeField] TextMeshProUGUI selectedDayTextUI;

    DataMaster datamaster;

    bool showingToday = true;

    void Start()
    {
        datamaster = FindObjectOfType<DataMaster>();

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

        entries = datamaster.GiveAppointsOfThisDay(selectedDay.Date);
        foreach (Appointment entry in entries)
        {
            float normalizedYstart = ConvertTimeToNormalizedY(entry.StartTimeDT());
            float normalizedYend = ConvertTimeToNormalizedY(entry.EndTimeDT());
            entryVisualPrototype.Instantiate(entry, normalizedYstart, normalizedYend, dayPlannerEntriesContainer);
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
        RectTransform rect = currentTimeIndicator.GetComponent<RectTransform>();

        if (showingToday)
        {
            currentTimeIndicator.SetActive(true);

            float y = ConvertTimeToNormalizedY(DateTime.Now);

            rect.anchorMin = new Vector2(0, y - timeIndicatorThickness);
            rect.anchorMax = new Vector2(1, y + timeIndicatorThickness);

            int indexOfLastSibling = transform.childCount - 1;
            currentTimeIndicator.transform.SetSiblingIndex(indexOfLastSibling);
        }
        else
        {
            currentTimeIndicator.SetActive(false);
        }
    }

    private void UpdateHighlight()
    {
        if (showingToday)
        {
            for (int i = 0; i < dayPlannerEntriesContainer.childCount; ++i)
            {
                DayPlannerEntry entry = dayPlannerEntriesContainer.GetChild(i).GetComponent<DayPlannerEntry>();
                entry.SetHighlight(entry.StartTime < DateTime.Now && entry.EndTime > DateTime.Now);
            }
        }      
    }

    public void SelectNewDay(bool nextDay)
    {
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

        UpdateCurrentTimeIndicator();
        UpdateHighlight();
    }
}