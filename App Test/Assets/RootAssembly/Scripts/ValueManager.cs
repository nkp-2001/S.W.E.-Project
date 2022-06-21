using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ValueManager : MonoBehaviour
{
    public static Task taskOnEdit = null;
    public static bool taskReturnInEdit = false;

    [SerializeField] private Toggle deadlineToggle;
    [SerializeField] private DatePicker datePicker;
    [SerializeField] private Slider priority;
    [SerializeField] private TMP_Dropdown repeatDropDown;

    [SerializeField] private TextMeshProUGUI HeadTitle;
    [SerializeField] private TextMeshProUGUI ButtonText;

    [SerializeField] private TextMeshProUGUI PrioText;
    [SerializeField] private Image PrioFill;

    private TMP_InputField title;
    private TMP_InputField description;

    void Start()
    {
        title = transform.GetChild(0).GetComponent<TMP_InputField>();
        description = transform.GetChild(1).GetComponent<TMP_InputField>();

        if (taskOnEdit != null) 
        {
            StartEditMode();
        }

        priority.onValueChanged.AddListener((value) =>
        {
            PrioText.text = value.ToString("0");

            if (value == 1)
            {
                PrioFill.color = new Color32(94, 8, 0, 102);
            }
            else if (value == 2)
            {
                PrioFill.color = new Color32(94, 8, 0, 154);
            }
            else if (value == 3)
            {
                PrioFill.color = new Color32(94, 8, 0, 192);
            }
            else if (value == 4)
            {
                PrioFill.color = new Color32(94, 8, 0, 221);
            }
            else if (value == 5)
            {
                PrioFill.color = new Color32(94, 8, 0, 255);
            }
        });
    }

    public void CreateTaskAndValidate()
    {
        if ((title.text == ""))
        {
            return;
        }

        DateTime dtraw = datePicker.GetSelectedDate();

        if (dtraw < DateTime.Now && deadlineToggle.isOn)
        {
            Subject.current.TriggerOnDateInPast();
            return;
        }

        int[] dt;
        int repeatIndex = 0;
        if (deadlineToggle.isOn)
        {
            dt = new int[] { dtraw.Minute, dtraw.Hour, dtraw.Day, dtraw.Month, dtraw.Year };
            repeatIndex = repeatDropDown.value;
        }
        else
        {
            dt = null;
        }

        if (taskOnEdit == null)
        {
            Subject.current.Trigger_OnNewTask(title.text, description.text, dt, priority.value, repeatIndex);
        }
        else
        {
            if (!taskReturnInEdit)
            {
                Subject.current.TriggerOnTaskChange(taskOnEdit, title.text, description.text, dt, priority.value, repeatIndex);
            }
            else
            {
                Subject.current.Trigger_OnTaskReturning(taskOnEdit, title.text, description.text, dt, priority.value, repeatIndex);
                taskReturnInEdit = false;
            }

            StopFromEditMode(); 
        }
        SceneLoader.Load(SceneLoader.Scene.MainPage);
    }

    public void StartEditMode(Task oldtask) 
    {
        taskOnEdit = oldtask;
        title.text = oldtask.Title;
        description.text = oldtask.Description;
        priority.value = oldtask.Priority;
        if (oldtask.Deadline != null)
        {
            datePicker.OnInteractibleChanged(true);
        }
        else
        {
            datePicker.SetSelectedDate(oldtask.Deadline);
        }

        if (!taskReturnInEdit)
        {
            HeadTitle.text = "Edit Task";
            ButtonText.text = "Save Changes";
        }
        else
        {
            HeadTitle.text = "Please give New Deadline";
            ButtonText.text = "Reinstiate Task";
        }
    }

    public void StartEditMode() 
    {
        title.text = taskOnEdit.Title;
        description.text = taskOnEdit.Description;
        priority.value = taskOnEdit.Priority;
        repeatDropDown.value = taskOnEdit.NextDeadlineIndex;

        if (taskOnEdit.Deadline == null || taskOnEdit.Deadline.Length == 0)
        {
            datePicker.OnInteractibleChanged(false);
        }
        else
        {
            print("Put in deadline");
            datePicker.SetSelectedDate(taskOnEdit.Deadline);
        }

        if (!taskReturnInEdit)
        {
            HeadTitle.text = "Edit Task";
            ButtonText.text = "Save Changes";
        }
        else
        {
            HeadTitle.text = "Please give New Deadline";
            ButtonText.text = "Reinstiate Task";
        }
    }
    private void StopFromEditMode()
    {
        ValueManager.taskOnEdit = null;
        taskReturnInEdit = false;
        HeadTitle.text = "Create new Task";
        ButtonText.text = "Add Task";
    }
}