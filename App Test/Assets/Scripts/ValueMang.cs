using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ValueMang : MonoBehaviour
{
    TMP_InputField titel;
    TMP_InputField discrip;
    [SerializeField] Toggle dltoggle;

    TMP_Dropdown day;
    TMP_Dropdown month;
    TMP_Dropdown year;
    TMP_Dropdown hour;
    TMP_Dropdown min;

    DatePicker datePicker;

    [SerializeField] Slider prio;
    Taskmaster tm;
    Sceneloader scenlaod;

    // Start is called before the first frame update
    void Start()
    {
        titel = transform.GetChild(0).GetComponent<TMP_InputField>();
        discrip = transform.GetChild(1).GetComponent<TMP_InputField>();
       

        datePicker = FindObjectOfType<DatePicker>();

        /*
        day = transform.GetChild(3).GetComponent<TMP_Dropdown>();
        month = transform.GetChild(4).GetComponent<TMP_Dropdown>();
        year = transform.GetChild(5).GetComponent<TMP_Dropdown>();

        hour = transform.GetChild(6).GetComponent<TMP_Dropdown>();
        min = transform.GetChild(7).GetComponent<TMP_Dropdown>();
        */

        
        tm = FindObjectOfType<Taskmaster>();
        scenlaod = FindObjectOfType<Sceneloader>();  



        
    }

   public void test_create_Task()  // return 
    {
        // Vald-check
       if ((titel.text == "")) //| (discrip.text == ""))
       {
            MessageBox.ShowMessage("Please enter a Title");
            return;
       }
       if (dltoggle.isOn)
       {
            DateTime dtraw = datePicker.GetSelectedDate();
            int[] dt = { dtraw.Minute, dtraw.Hour, dtraw.Day, dtraw.Month, dtraw.Year };
            tm.create_newTask(titel.text, discrip.text,dt, prio.value);
       }
       else
       {
            tm.create_newTask(titel.text, discrip.text, null, prio.value); // null -> Datetime.minvalue wenn zurückwechsel auf Datetime
       }

        scenlaod.loadScene(1);
   }
    public void grey()
    {
       
    }

    
}
