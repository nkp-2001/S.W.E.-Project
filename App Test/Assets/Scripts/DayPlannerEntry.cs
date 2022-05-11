using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayPlannerEntry : MonoBehaviour
{
    [SerializeField] private float margin;
    public string Title
    {
        get;
        set;
    }

    public string Description
    {
        get;
        set;
    }

    public DateTime StartTime{
        get;
        set;
    }

    public DateTime EndTime
    {
        get;
        set;
    }

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        Debug.Log(rect);
    }

    public void SetNormalizedYcoordinates(float y1, float y2)
    {
        float yMin = Math.Min(y1, y2);
        float yMax = Math.Max(y1, y2);

        rect.anchorMin = new Vector2(0, yMin);
        rect.anchorMax = new Vector2(1, yMax);

        Vector2 marginVector = new Vector2(margin, margin);
        rect.offsetMin = marginVector;
        rect.offsetMax = marginVector;
    }

    public void OnClick()
    {

    }
}
