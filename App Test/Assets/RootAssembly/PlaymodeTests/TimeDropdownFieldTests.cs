using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class TimeDropdownFieldTests
{
    GameObject gameObject;
    TimeDropdownField timeDropdownField;
    TMP_Dropdown tmp_dropdown;

    private class TimeDropdownFieldBuilder
    {
        public static GameObject GetInstance()
        {
            GameObject timeDropdownInstance = new GameObject();

            timeDropdownInstance.AddComponent<TMP_Dropdown>();
            timeDropdownInstance.AddComponent<TimeDropdownField>();

            return timeDropdownInstance;
        }
    }

    private List<string> FillList(params string[] entries)
    {
        List<string> list = new();

        foreach(string entry in entries)
        {
            list.Add(entry);
        }

        return list;
    }

    [SetUp]
    public void BeforeEveryTest()
    {
        gameObject = TimeDropdownFieldBuilder.GetInstance();
        tmp_dropdown = gameObject.GetComponent<TMP_Dropdown>();
        timeDropdownField = gameObject.GetComponent<TimeDropdownField>();
        timeDropdownField.Initialize();
    }

    [UnityTest]
    public IEnumerator TestSetOptions_Nonempty()
    {
        yield return new WaitForEndOfFrame();

        List<string> options = FillList("option1", "option2", "option3");

        timeDropdownField.SetOptions(options);

        foreach(string option in options)
        {
            int index = options.IndexOf(option);
            string actualOption = gameObject.GetComponent<TMP_Dropdown>().options[index].text;

            Assert.AreEqual(option, actualOption);
        }
    }

    [UnityTest]
    public IEnumerator TestSetOptions_Empty()
    {
        yield return new WaitForEndOfFrame();

        List<string> options = new();
        timeDropdownField.SetOptions(options);

        Assert.IsEmpty(tmp_dropdown.options);
    }

    [UnityTest]
    public IEnumerator TestSetCurrentOption()
    {
        yield return new WaitForEndOfFrame();

        List<string> options = FillList("option1", "option2", "option3");
        timeDropdownField.SetOptions(options);

        string selection = options[1];
        timeDropdownField.SetCurrentOption(selection);

        Assert.AreEqual(selection, tmp_dropdown.options[tmp_dropdown.value].text);
    }

    [UnityTest]
    public IEnumerator TestSetInteractible_On()
    {
        yield return new WaitForEndOfFrame();

        timeDropdownField.SetInteractible(true);

        Assert.AreEqual(true, tmp_dropdown.IsInteractable());
    }

    [UnityTest]
    public IEnumerator TestSetInteractible_Off()
    {
        yield return new WaitForEndOfFrame();

        timeDropdownField.SetInteractible(false);

        Assert.AreEqual(false, tmp_dropdown.IsInteractable());
    }

    [UnityTest]
    public IEnumerator TestGetCurrentOption()
    {
        yield return new WaitForEndOfFrame();

        List<string> options = FillList("option1", "option2", "option3");
        timeDropdownField.SetOptions(options);

        string selection = options[1];
        timeDropdownField.SetCurrentOption(selection);

        Assert.AreEqual(selection, timeDropdownField.GetCurrentOption());
    }
}
