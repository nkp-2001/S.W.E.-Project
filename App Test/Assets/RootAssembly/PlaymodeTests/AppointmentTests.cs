using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

public class AppointmentTests
{
     public int[] stT;
     public int[] enT;

    [SetUp]
    public void Set()
    {
        stT = new int[5];
        stT[4] = 2022;
        stT[3] = 06;
        stT[2] = 15;
    }

    [Test]
    public void ReturnTrue_AssignmentPassed()
    {
        var selectDay = new DateTime(2022, 06, 15);
        var t = "Doing";
        var d = "Homework";
        var reIn = 1;
        var notficID = 1;

        Appointment appointment_test = new Appointment(t, d, stT, enT, reIn, notficID);

        bool OnThisDay = appointment_test.AppointmentOnThisDay(selectDay);

        Assert.IsTrue(OnThisDay);
    }

    [Test]
    public void GetTitelSuccessed()
    {
        var t = "Doing";
        var d = "Homework";
        var reIn = 0;
        var notficID = 1;

        Appointment appointment_test = new Appointment(t, d, stT, enT, reIn, notficID);

        string _titel = appointment_test.Title;
     
        Assert.That(_titel, Is.EqualTo("Doing")); 
    }

    [Test]
    public void GetDespSuccessed()
    {
        var t = "Doing";
        var d = "Homework";
        var reIn = 0;
        var notficID = 1;

        Appointment appointment_test = new Appointment(t, d, stT, enT, reIn, notficID);

        string _desp = appointment_test.Description;

        Assert.That(_desp, Is.EqualTo("Homework"));
    }

    [Test]
    public void GetReinSuccessed()
    {
        var t = "Doing";
        var d = "Homework";
        var reIn = 0;
        var notficID = 1;

        Appointment appointment_test = new Appointment(t, d, stT, enT, reIn, notficID);

        int _reIn = appointment_test.Repeat;

        Assert.That(_reIn, Is.EqualTo(0));  
    }

    [Test]
    public void GetNotiIDSuccessed()
    {
        var t = "Doing";
        var d = "Homework";
        var reIn = 0;
        var notficID = 1;

        Appointment appointment_test = new Appointment(t, d, stT, enT, reIn, notficID);

        int _notiID = appointment_test.Notifcation_id;

        Assert.That(_notiID, Is.EqualTo(1));
    }
}
