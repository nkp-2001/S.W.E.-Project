using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

public class DayPlanerEntryTest
{
    [Test]
    public void StartTime_Test()
    {
        var dayPlannerEntry = new DayPlannerEntry();
        dayPlannerEntry.StartTime = new DateTime(2022, 06, 11, 07, 00, 00);

        Assert.That(dayPlannerEntry.StartTime, Is.EqualTo(new DateTime(2022, 06, 11, 07, 00, 00)));
    }

    [Test]
    public void EndTime_Test()
    {
        var dayPlannerEntry = new DayPlannerEntry();
        dayPlannerEntry.EndTime = new DateTime(2022, 06, 11, 08, 00, 00);

        Assert.That(dayPlannerEntry.EndTime, Is.EqualTo(new DateTime(2022, 06, 11, 08, 00, 00)));
    }

    [Test]
    public void StartTime_NormalizedYstart_Checked()
    {
        var numHoursOnScreen = 24;
        var startTime = new DateTime(2022, 06, 11, 07, 00, 00);
        var instantiateChecker = new InstantiateChecker();
        var Ystart = instantiateChecker.CordinateY(numHoursOnScreen, startTime, 5f);

        Assert.That(Ystart, Is.EqualTo(0.708333373f));
    }

    [Test]
    public void EndTime_NormalizedYend_Checked()
    {
        var numHoursOnScreen = 24;
        var endTime = new DateTime(2022, 06, 11, 08, 00, 00);
        var instantiateChecker = new InstantiateChecker();
        var Yend = instantiateChecker.CordinateY(numHoursOnScreen, endTime, 5f);

        Assert.That(Yend, Is.EqualTo(0.666666666f));
    }
}
