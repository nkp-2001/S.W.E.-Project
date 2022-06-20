using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ValueManager : MonoBehaviour
{
    protected TMP_InputField titel;
    protected TMP_InputField discrip;
    [SerializeField] Toggle dltoggle;

    [SerializeField] protected DatePicker datePicker;

    [SerializeField] Slider prio;
    protected SceneLoader sceneLoader;

    public static Task taskOnEdit = null; 

    public static bool tastReturninEdit = false;

    [SerializeField] protected TextMeshProUGUI HeadTitle;
    [SerializeField] protected TextMeshProUGUI ButtonText;
    [SerializeField] protected TMP_Dropdown repeatDropDown;

    [SerializeField] TextMeshProUGUI PrioText;
    [SerializeField] GameObject PrioFill;

    void Start()
    {
        titel = transform.GetChild(0).GetComponent<TMP_InputField>();
        discrip = transform.GetChild(1).GetComponent<TMP_InputField>();

        sceneLoader = FindObjectOfType<SceneLoader>();

        if (taskOnEdit != null) 
        {
            StartEditMode();
        }

        prio.onValueChanged.AddListener((value) =>
        {
            PrioText.text = value.ToString("0");

            if (value == 1)
            {
                PrioFill.GetComponent<Image>().color = new Color32(94, 8, 0, 102);
            }
            else if (value == 2)
            {
                PrioFill.GetComponent<Image>().color = new Color32(94, 8, 0, 154);
            }
            else if (value == 3)
            {
                PrioFill.GetComponent<Image>().color = new Color32(94, 8, 0, 192);
            }
            else if (value == 4)
            {
                PrioFill.GetComponent<Image>().color = new Color32(94, 8, 0, 221);
            }
            else if (value == 5)
            {
                PrioFill.GetComponent<Image>().color = new Color32(94, 8, 0, 255);
            }
        });
    }

    public void CreateTaskAndValidate()
    {
        if ((titel.text == ""))
        {
            return;
        }

        DateTime dtraw = datePicker.GetSelectedDate();

        if (dtraw < DateTime.Now && dltoggle.isOn)
        {
            Subject.current.TriggerOnDateInPast();
            return;
        }

        int[] dt;
        int repeatIndex = 0;
        if (dltoggle.isOn)
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
            Subject.current.Trigger_OnNewTask(titel.text, discrip.text, dt, prio.value, repeatIndex);
        }
        else
        {
            if (!tastReturninEdit)
            {
                Subject.current.TriggerOnTaskChange(taskOnEdit, titel.text, discrip.text, dt, prio.value, repeatIndex);
            }
            else
            {
                Subject.current.Trigger_OnTaskReturning(taskOnEdit, titel.text, discrip.text, dt, prio.value, repeatIndex);
                tastReturninEdit = false;
            }

            StopFromEditMode(); 
        }
        sceneLoader.LoadScene(0);
    }

    public void StartEditMode(Task oldtask) 
    {
        taskOnEdit = oldtask;
        titel.text = oldtask.Titel;
        discrip.text = oldtask.Description;
        prio.value = oldtask.Prio;
        if (oldtask.Deadline != null)
        {
            datePicker.OnInteractibleChanged(true);
        }
        else
        {
            datePicker.SetSelectedDate(oldtask.Deadline);
        }

        if (!tastReturninEdit)
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
        titel.text = taskOnEdit.Titel;
        discrip.text = taskOnEdit.Description;
        prio.value = taskOnEdit.Prio;
        repeatDropDown.value = taskOnEdit.NextDeadlineIndex;

        if (taskOnEdit.Deadline.Length == 0)
        {
            datePicker.OnInteractibleChanged(false);
        }
        else
        {
            print("Put in deadline");
            datePicker.SetSelectedDate(taskOnEdit.Deadline);
        }

        if (!tastReturninEdit)
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
        tastReturninEdit = false;
        HeadTitle.text = "Create new Task";
        ButtonText.text = "Add Task";
    }
}





