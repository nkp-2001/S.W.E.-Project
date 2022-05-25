using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayPlannerRenderer : MonoBehaviour
{
    [SerializeField] private int numHoursOnScreen;

    [SerializeField] private GameObject	hourSegmentPrefab;
    [SerializeField] private GameObject currentTimeIndicator;

    [SerializeField] private DayPlannerEntry entryVisualPrototype;
    [SerializeField] private List<DayPlannerEntryPlaceholder> entries;

    private DateTime selectedDay;
    private Dictionary<DayPlannerEntryPlaceholder, bool> isToBeDisplayed = new();

    private RectTransform rectTransform;

    void Awake()
    {
        UpdateEntries();
    }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Display();
    }

    private void Display()
    {
        DisplayHours();
        DisplayDayPlannerEntries();
        UpdateCurrentTimeIndicator();
    }

    private bool IsToBeDisplayed(DayPlannerEntryPlaceholder entry)
    {
        return isToBeDisplayed[entry];
    }

    private void UpdateEntries()
    {
        isToBeDisplayed.Clear();

        foreach (DayPlannerEntryPlaceholder entry in entries)
        {
            isToBeDisplayed.Add(entry, true);
        }
    }

    private void DisplayHours()
    {
        for(int i = 0; i < numHoursOnScreen; ++i)
        {
            GameObject hourSegment = Instantiate(hourSegmentPrefab, transform);
            RectTransform rectTransform = hourSegment.GetComponent<RectTransform>();

            DateTime hourDateTime = new DateTime(2000, 1, 1, i, 0, 0);
            float y = ConvertTimeToNormalizedCoordinates(hourDateTime);

            rectTransform.anchorMin = new Vector2(0, y);
            rectTransform.anchorMax = new Vector2(1, y);

            hourSegment.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString("D2") + ":00";
        }
    }

    private void DisplayDayPlannerEntries()
    {
        if (entries.Count > 0)
        {
            //for (int i = 0; i < transform.childCount; ++i)
            //{
            //    GameObject child = transform.GetChild(i).gameObject;
            //    DayPlannerEntryPlaceholder dayPlannerEntry = child.GetComponent<DayPlannerEntryPlaceholder>();

            //    if (!IsToBeDisplayed(dayPlannerEntry))
            //    {
            //        Destroy(child);
            //    }
            //}

            foreach (DayPlannerEntryPlaceholder entry in entries)
            {
                bool x = IsToBeDisplayed(entry);
                bool contains = isToBeDisplayed.ContainsKey(entry);
                int c = transform.childCount;

                if (IsToBeDisplayed(entry))
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

                    float normalizedYstart = ConvertTimeToNormalizedCoordinates(startTime);
                    float normalizedYend = ConvertTimeToNormalizedCoordinates(endTime);

                    entryVisualPrototype.Instantiate(entry.title, normalizedYstart, normalizedYend, transform);
                    isToBeDisplayed[entry] = false;
                }
            }
        }
    }

    private float ConvertTimeToNormalizedCoordinates(DateTime dateTime)
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
        float y = ConvertTimeToNormalizedCoordinates(DateTime.Now) * rectTransform.rect.height;
        currentTimeIndicator.GetComponent<RectTransform>().position = new Vector3(0, y);

        int indexOfLastSibling = transform.childCount - 1;
        currentTimeIndicator.transform.SetSiblingIndex(indexOfLastSibling);
    }

}
