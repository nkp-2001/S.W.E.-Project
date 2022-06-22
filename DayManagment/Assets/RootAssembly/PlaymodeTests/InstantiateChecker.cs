using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InstantiateChecker
{
    public float CordinateY(int numHoursOnScreen, DateTime dateTime, float h)
    {
        float pixelsPerHour = h / numHoursOnScreen;
        float pixelsPerMinute = pixelsPerHour / 60;

        float y = dateTime.Hour * pixelsPerHour + dateTime.Minute * pixelsPerMinute;

        float normalizedY = 1 - y / h;

        return normalizedY;
    }
} 
   