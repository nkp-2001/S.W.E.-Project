using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;


public class TimeDropdownField : MonoBehaviour
{
    private TMP_Dropdown dropdown;
    private Dictionary<string, int> indexOfOption = new();

    public void Initialize()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }

    public void SetOptions(List<string> options)
    {
        indexOfOption.Clear();

        int index = 0;
        foreach(string option in options)
        {
            indexOfOption.Add(option, index);
            ++index;
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }

    public void SetCurrentOption(string option)
    {
        dropdown.SetValueWithoutNotify(indexOfOption[option]);
    }

    public void SetInteractible(bool interactible)
    {
        dropdown.interactable = interactible;
    }
    
    public string GetCurrentOption()
    {
        return dropdown.options[dropdown.value].text;
    }
}
