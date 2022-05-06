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

    void Start()
    {
        titel = transform.GetChild(0).GetComponent<TMP_InputField>();
        discrip = transform.GetChild(1).GetComponent<TMP_InputField>();
       
        datePicker = FindObjectOfType<DatePicker>();
        
        tm = FindObjectOfType<Taskmaster>();
        sceneLoader = FindObjectOfType<SceneLoader>(); 
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
            Subject.current.Trigger_OnNewTask(titel.text, discrip.text,dt, prio.value);

       }
       else
       {
           // tm.CreateNewTask(titel.text, discrip.text, null, prio.value); 
            Subject.current.Trigger_OnNewTask(titel.text, discrip.text, null, prio.value); // null -> Datetime.minvalue wenn zurückwechsel auf Datetime
       }

        sceneLoader.LoadScene(1);
   }    
}
