using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using System;

using TMPro;

public class DatePickerBuilder
{
    private static GameObject prefab = Resources.Load("Prefabs/DatePicker") as GameObject;

    public static GameObject GetInstance()
    {
        return MonoBehaviour.Instantiate(prefab);
    }
}

public class DatePickerTests
{
    GameObject gameObject;
    DatePicker datePicker;

    [SetUp]
    public void BeforeEveryTest()
    {
        gameObject = DatePickerBuilder.GetInstance();
        datePicker = gameObject.GetComponent<DatePicker>();
    }

    private TMP_Dropdown SelectDateAndUpdateDaysList(int[] dateArray)
    {
        datePicker.SetSelectedDate(dateArray);
        datePicker.UpdateDaysList();

        return gameObject.transform.Find("Day").GetComponent<TMP_Dropdown>();
    }

    [UnityTest]
    public IEnumerator TestUpdateDaysList_February_NoLeapYear()
    {
        yield return new WaitForEndOfFrame();

        TMP_Dropdown dayDropdown = SelectDateAndUpdateDaysList(new int[] { 0, 0, 1, 2, 2022 });

        Assert.AreEqual(28, dayDropdown.options.Count);
    }

    [UnityTest]
    public IEnumerator TestUpdateDaysList_February_LeapYear()
    {
        yield return new WaitForEndOfFrame();

        TMP_Dropdown dayDropdown = SelectDateAndUpdateDaysList(new int[] { 0, 0, 1, 2, 2024 });

        Assert.AreEqual(29, dayDropdown.options.Count);
    }

    [UnityTest]
    public IEnumerator TestUpdateDaysList_March()
    {
        yield return new WaitForEndOfFrame();

        TMP_Dropdown dayDropdown = SelectDateAndUpdateDaysList(new int[] { 0, 0, 1, 3, 2024 });

        Assert.AreEqual(31, dayDropdown.options.Count);
    }

    [UnityTest]
    public IEnumerator TestUpdateDaysList_April()
    {
        yield return new WaitForEndOfFrame();

        TMP_Dropdown dayDropdown = SelectDateAndUpdateDaysList(new int[] { 0, 0, 1, 4, 2060 });

        Assert.AreEqual(30, dayDropdown.options.Count);
    }

    [UnityTest]
    public IEnumerator TestSelectingAndGettingDate()
    {
        yield return new WaitForEndOfFrame();

        int[] dateArray = { 20, 15, 3, 12, 2024 };
        datePicker.SetSelectedDate(dateArray);

        DateTime dateTime = new DateTime(dateArray[4], dateArray[3], dateArray[2], dateArray[1], dateArray[0], 0);
        Assert.AreEqual(dateTime, datePicker.GetSelectedDate());
    }
}
