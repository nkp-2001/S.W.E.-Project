using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayPlannerRenderer : Graphic
{
    [SerializeField] private float thickness;
    [SerializeField] private int numHoursOnScreen;

    [SerializeField] private GameObject entryVisualPrototype;
    [SerializeField] private List<DayPlannerEntryPlaceholder> entries;

    private DateTime selectedDay;
    private Dictionary<DayPlannerEntryPlaceholder, bool> isToBeDisplayed = new();

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        float w = rectTransform.rect.width;
        float h = rectTransform.rect.height;

        vh.Clear();

        DrawRect(vh, -w / 2, -h / 2, w, h, Color.white);

        for (int i = 0; i < numHoursOnScreen; ++i)
        {
            float y = i * h / numHoursOnScreen - h / 2;
            DrawLine(vh, -w / 2, y, w / 2, y, thickness, Color.gray);
        }
    }

    protected override void Awake()
    {
        UpdateEntries();
    }

    void OnGUI()
    {
        DisplayTimeLabels();
        DisplayDayPlannerEntries();
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

                    if(!DateTime.TryParse(entry.StartTime, out startTime))
                    {
                        throw new Exception("Invalid StartTime in entry");
                    }

                    if (!DateTime.TryParse(entry.EndTime, out endTime))
                    {
                        throw new Exception("Invalid EndTime in entry");
                    }

                    float normalizedYstart = ConvertTimeToNormalizedCoordinates(startTime);
                    float normalizedYend = ConvertTimeToNormalizedCoordinates(endTime);

                    GameObject visualEntry = Instantiate(entryVisualPrototype, transform);
                    visualEntry.GetComponent<DayPlannerEntry>().SetNormalizedYcoordinates(normalizedYstart, normalizedYend);
                    visualEntry.SetActive(true);

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

    private void DisplayTimeLabels()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 50; //TODO: make this adjust to screen size
        GUI.color = Color.black;

        for (int i = 0; i < numHoursOnScreen; ++i)
        {
            float w = rectTransform.rect.width;
            float h = rectTransform.rect.height;

            float y = i * h / numHoursOnScreen;

            GUI.Label(new Rect(0, y, w, h / numHoursOnScreen), i.ToString("D2") + ":00", style);
        }
    }

    private void DisplayCurrentTimeLine(VertexHelper vh)
    {
        float y = ConvertTimeToNormalizedCoordinates(DateTime.Now);
        DrawLine(vh, 0, y, 1, y, thickness, Color.red);
    }


    private void DrawRect(VertexHelper vh, float x, float y, float w, float h, Color c)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = c;

        vertex.position = new Vector3(x, y);
        vh.AddVert(vertex);

        vertex.position = new Vector3(x, y + h);
        vh.AddVert(vertex);

        vertex.position = new Vector3(x + w, y + h);
        vh.AddVert(vertex);

        vertex.position = new Vector3(x + w, y);
        vh.AddVert(vertex);

        vh.AddTriangle(vh.currentVertCount - 4, vh.currentVertCount - 3, vh.currentVertCount - 2);
        vh.AddTriangle(vh.currentVertCount - 2, vh.currentVertCount - 1, vh.currentVertCount - 4);
    }

    private void DrawLine(VertexHelper vh, float x1, float y1, float x2, float y2, float thickness, Color c)
    {
        Vector3 fromPoint1ToPoint2 = new Vector3(x2 - x1, y2 - y1, 0);
        Vector3 perpendicularScaledToThickness = Vector3.Cross(fromPoint1ToPoint2, Vector3.forward) * thickness / fromPoint1ToPoint2.magnitude;

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = c;

        vertex.position = new Vector3(x1, y1) + perpendicularScaledToThickness;
        vh.AddVert(vertex);

        vertex.position = new Vector3(x2, y2) + perpendicularScaledToThickness;
        vh.AddVert(vertex);

        vertex.position = new Vector3(x2, y2) - perpendicularScaledToThickness;
        vh.AddVert(vertex);

        vertex.position = new Vector3(x1, y1) - perpendicularScaledToThickness;
        vh.AddVert(vertex);

        vh.AddTriangle(vh.currentVertCount - 4, vh.currentVertCount - 3, vh.currentVertCount - 2);
        vh.AddTriangle(vh.currentVertCount - 2, vh.currentVertCount - 1, vh.currentVertCount - 4);
    }

}
