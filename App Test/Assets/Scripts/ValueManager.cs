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

    public static Taskmaster.Task taskOnEdit = null; //muss noch mit potinziallen wegfallen von Szenenwechsel überdenkt werden / Andere Lösung allg. vllt 

    [SerializeField] TextMeshProUGUI HeadTitle;
    [SerializeField] TextMeshProUGUI ButtonText;

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
       if (dltoggle.isOn)
       {
            DateTime dtraw = datePicker.GetSelectedDate();
            int[] dt = { dtraw.Minute, dtraw.Hour, dtraw.Day, dtraw.Month, dtraw.Year };
            // tm.CreateNewTask(titel.text, discrip.text,dt, prio.value);
            if (taskOnEdit == null)
            {
                Subject.current.Trigger_OnNewTask(titel.text, discrip.text, dt, prio.value);
            }
            else
            {
                Subject.current.TriggerOnTaskChange(taskOnEdit, titel.text, discrip.text, dt, prio.value);
                StopFromEditMode(); //
            }
       }
       else
       {
            // tm.CreateNewTask(titel.text, discrip.text, null, prio.value); 
            if (taskOnEdit == null)
            {
                Subject.current.Trigger_OnNewTask(titel.text, discrip.text, null, prio.value); // null -> Datetime.minvalue wenn zurückwechsel auf Datetime
            }
            else
            {
                Subject.current.TriggerOnTaskChange(taskOnEdit, titel.text, discrip.text, null, prio.value);
                StopFromEditMode();
            }
       }

        sceneLoader.LoadScene(0);
   }    
    /// <EditMode> ///
   public void StartEditMode(Taskmaster.Task oldtask) //vllt zum Event unmwandeln ? Konflikt mit datePicker.GetSelectedDate(); | nicht immer (/lieber nicht) es mit funcs machen
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

        HeadTitle.text = "Edit Task";
        ButtonText.text = "Save Changes";
   }
    public void StartEditMode() 
    {
     
        titel.text = taskOnEdit.Titel;
        discrip.text = taskOnEdit.Description;
        prio.value = taskOnEdit.Prio;
        if (taskOnEdit.Deadline != null)
        {
            datePicker.SetInteractability();
        }
        else
        {
            datePicker.SetSelectedDate(taskOnEdit.Deadline);
        }

        HeadTitle.text = "Edit Task";
        ButtonText.text = "Save Changes";
    }
    private void StopFromEditMode()
    {
        ValueManager.taskOnEdit = null;
        HeadTitle.text = "Create new Task";
        ButtonText.text = "Add Task";
    }
}
