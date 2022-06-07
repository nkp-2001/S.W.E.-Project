using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayPlannerEntry : MonoBehaviour
{
    [SerializeField] private string title;
    [SerializeField] private string Description;

    [SerializeField] private TextMeshProUGUI titleTMP;

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
    }

    public void Instantiate(string title, float normalizedYstart, float normalizedYend, Transform parent)
    {
        GameObject instance = Instantiate(gameObject);
        instance.transform.SetParent(parent, false);

        instance.GetComponent<DayPlannerEntry>().SetTitle(title);
        instance.GetComponent<DayPlannerEntry>().SetNormalizedYcoordinates(normalizedYstart, normalizedYend);
        instance.SetActive(true);

    }
    public void SetHighlight(bool highlighted)
    {
        if (highlighted)
        {
            GetComponent<Image>().color = Color.red;
        }
        else
        {
            GetComponent<Image>().color = Color.white;
        }
        Debug.Log("sethighlight" + highlighted);
    }

    public void SetTitle(string t)
    {
        title = t;
        titleTMP.text = title;
    }

    public void SetNormalizedYcoordinates(float y1, float y2)
    {
        float yMin = Math.Min(y1, y2);
        float yMax = Math.Max(y1, y2);

        rect.anchorMin = new Vector2(0, yMin);
        rect.anchorMax = new Vector2(1, yMax);
    }

    public void OnClick()
    {

    }
}
