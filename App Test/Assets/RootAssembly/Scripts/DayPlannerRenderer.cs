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
    [SerializeField] private List<DayPlannerEntryPlaceholder> entries;

    [SerializeField] GameObject RedLine;

    private DateTime selectedDay;

    //public static float start;
    //public static float end;

    private RectTransform rectTransform;


    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        InvokeRepeating("UpdateCurrentTimeIndicator", 0, 1);
        InvokeRepeating("UpdateHighlight", 0, 1);

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


        foreach (DayPlannerEntryPlaceholder entry in entries)
        {
            DateTime startTime, endTime;

            if (!DateTime.TryParse(entry.StartTime, out startTime))
            {
                throw new Exception("Invalid StartTime in entry");
            }

            if (!DateTime.TryParse(entry.EndTime, out endTime))
            {
                throw new Exception("Invalid EndTime in entry");
            }

            float normalizedYstart = ConvertTimeToNormalizedY(startTime);
            float normalizedYend = ConvertTimeToNormalizedY(endTime);

            //start = normalizedYstart;
            //end = normalizedYend;

            entryVisualPrototype.Instantiate(entry.title, normalizedYstart, normalizedYend, dayPlannerEntriesContainer);

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
        float y = ConvertTimeToNormalizedY(DateTime.Now) * rectTransform.rect.height;
        currentTimeIndicator.GetComponent<RectTransform>().position = new Vector3(0, y);

        int indexOfLastSibling = transform.childCount - 1;
        currentTimeIndicator.transform.SetSiblingIndex(indexOfLastSibling);
    }

    private void UpdateHighlight()
    {
        foreach (DayPlannerEntryPlaceholder entry in entries)
        {
            DateTime startTime;
            DateTime.TryParse(entry.StartTime, out startTime);
            DateTime endTime;
            DateTime.TryParse(entry.EndTime, out endTime);
            Debug.Log(startTime < DateTime.Now);
            if (startTime < DateTime.Now && endTime > DateTime.Now)
            {
                entryVisualPrototype.SetHighlight(true);
            }
            else
            {
                entryVisualPrototype.SetHighlight(false);
            }
        }
        
    }
}