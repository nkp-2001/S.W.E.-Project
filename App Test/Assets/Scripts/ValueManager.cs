using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ValueManager : MonoBehaviour
{
    TMP_InputField titel;
    TMP_InputField discrip;
    [SerializeField] Toggle dltoggle;

    DatePicker datePicker;

    [SerializeField] Slider prio;
    Taskmaster tm;
    SceneLoader sceneLoader;

    public static Task taskOnEdit = null; //muss noch mit potinziallen wegfallen von Szenenwechsel überdenkt werden / Andere Lösung allg. vllt 
    public static bool tastReturninEdit = false;

    [SerializeField] TextMeshProUGUI HeadTitle;
    [SerializeField] TextMeshProUGUI ButtonText;
    [SerializeField] TMP_Dropdown repeatDropDown;

    void Start()
    {
       
        titel = transform.GetChild(0).GetComponent<TMP_InputField>();
        discrip = transform.GetChild(1).GetComponent<TMP_InputField>();
       
        datePicker = FindObjectOfType<DatePicker>();
        
        tm = FindObjectOfType<Taskmaster>();
        sceneLoader = FindObjectOfType<SceneLoader>();

        if (taskOnEdit != null) // !! noch überdenken , bei Wegfallen von Szenewecx
        {
            StartEditMode();
        }
    }

   public void CreateTaskAndValidate()
   {
        // Valid-check
       if ((titel.text == ""))
       {
            MessageBox.ShowMessage("Please enter a Title");
            return;
       }
      
       DateTime dtraw = datePicker.GetSelectedDate();
       int[] dt;
       int repeatIndex = 0;
       if (dltoggle.isOn)
       {
            dt = new int[]{dtraw.Minute, dtraw.Hour, dtraw.Day, dtraw.Month, dtraw.Year};
            repeatIndex = repeatDropDown.value;
        }
       else
       {
            dt = null;
       }
        ///////////////////////////////
        // int[] dt = { dtraw.Minute, dtraw.Hour, dtraw.Day, dtraw.Month, dtraw.Year };
        // tm.CreateNewTask(titel.text, discrip.text,dt, prio.value);
        if (taskOnEdit == null)
        {
            Subject.current.Trigger_OnNewTask(titel.text, discrip.text, dt, prio.value, repeatIndex);
        }
        else
        {
            if (!tastReturninEdit)
            {
                Subject.current.TriggerOnTaskChange(taskOnEdit, titel.text, discrip.text, dt, prio.value,repeatIndex);
            }
            else
            {
                Subject.current.Trigger_OnTaskReturning(taskOnEdit, titel.text, discrip.text, dt, prio.value,repeatIndex);
                tastReturninEdit = false;
            }

            StopFromEditMode(); //
        }
     sceneLoader.LoadScene(0);
   }    
    /// <EditMode> ///
   public void StartEditMode(Task oldtask) //vllt zum Event unmwandeln ? Konflikt mit datePicker.GetSelectedDate(); | nicht immer (/lieber nicht) es mit funcs machen
   {
      

        taskOnEdit = oldtask;
        titel.text = oldtask.Titel;
        discrip.text = oldtask.Description;
        prio.value = oldtask.Prio;
        if (oldtask.Deadline != null)
        {
            datePicker.SetInteractability();
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
    public void StartEditMode()  //vllt zum Event unmwandeln ? Konflikt mit datePicker.GetSelectedDate(); | nicht immer (/lieber nicht) es mit funcs machen
    {
     
        titel.text = taskOnEdit.Titel;
        discrip.text = taskOnEdit.Description;
        prio.value = taskOnEdit.Prio;
      
        if (taskOnEdit.Deadline.Length ==0)
        {
            datePicker.SetInteractability();
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
