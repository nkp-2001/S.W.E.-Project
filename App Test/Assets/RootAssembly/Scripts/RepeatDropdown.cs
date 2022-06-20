using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RepeatDropdown : MonoBehaviour
{
    private List<string> optionLabels = new() {"never", "daily", "weekly", "bi-weekly", "monthly", "yearly" };
    private int[] repeatIndexToDays = { 0, 1, 7, 7 * 2, 7 * 4, 365 };
    Dictionary<int, int> daysToRepeatIndex = new();

    TMP_Dropdown dropdown;

    private void Start()
    {
        for(int i = 0; i < repeatIndexToDays.Length; ++i)
        {
            daysToRepeatIndex.Add(repeatIndexToDays[i], i);
        }

        dropdown = GetComponent<TMP_Dropdown>();

        dropdown.ClearOptions();
        dropdown.AddOptions(optionLabels);

        dropdown.value = 0;
    }

    public void SetDays(int days)
    {
        dropdown.value = daysToRepeatIndex[days];
    }

    public int GetDays()
    {
        return repeatIndexToDays[dropdown.value];
    }

    public int GetIndex()
    {
        return dropdown.value;
    }
}
