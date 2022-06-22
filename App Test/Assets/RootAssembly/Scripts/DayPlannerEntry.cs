using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayPlannerEntry : MonoBehaviour
{
    [SerializeField] private string title;

    [SerializeField] private TextMeshProUGUI titleTMP;

    private Appointment underlyingAppointment;

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

    public void Instantiate(Appointment appointment, float normalizedYstart, float normalizedYend, Transform parent)
    {
        GameObject instance = Instantiate(gameObject);
        instance.transform.SetParent(parent, false);

        instance.GetComponent<DayPlannerEntry>().underlyingAppointment = appointment;

        instance.GetComponent<DayPlannerEntry>().SetTitle(appointment.Title);
        instance.GetComponent<DayPlannerEntry>().StartTime = appointment.StartTimeDT();
        instance.GetComponent<DayPlannerEntry>().EndTime = appointment.EndTimeDT();
        instance.GetComponent<DayPlannerEntry>().SetNormalizedYcoordinates(normalizedYstart, normalizedYend);
        instance.SetActive(true);
    }

    public void SetHighlight(bool highlighted)
    {
        if (highlighted)
        {
            GetComponent<Image>().color = new Color32(255, 117, 108, 255);
        }
        else
        {
            GetComponent<Image>().color = Color.white;
        }
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
        AppointmentValueManager.underlyingAppointment = underlyingAppointment;
        SceneLoader.Load(SceneLoader.Scene.CreateAppointmentPage);
    }
}
