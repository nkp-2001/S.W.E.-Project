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
    Toggle dltoggle;
    TMP_Dropdown day;
    TMP_Dropdown month;
    TMP_Dropdown year;
    TMP_Dropdown hour;
    TMP_Dropdown min;
    Slider prio;
    Taskmaster tm;
    Sceneloader scenlaod;

    // Start is called before the first frame update
    void Start()
    {
        titel = transform.GetChild(0).GetComponent<TMP_InputField>();
        discrip = transform.GetChild(1).GetComponent<TMP_InputField>();
        dltoggle = transform.GetChild(2).GetComponent<Toggle>();
        day = transform.GetChild(3).GetComponent<TMP_Dropdown>();
        month = transform.GetChild(4).GetComponent<TMP_Dropdown>();
        year = transform.GetChild(5).GetComponent<TMP_Dropdown>();

        hour = transform.GetChild(6).GetComponent<TMP_Dropdown>();
        min = transform.GetChild(7).GetComponent<TMP_Dropdown>();

        prio = transform.GetChild(8).GetComponent<Slider>();
        tm = FindObjectOfType<Taskmaster>();
        scenlaod = FindObjectOfType<Sceneloader>();  



        
    }

   public void test_create_Task()
    {
        // Vald-check
       if ((titel.text == "")) //| (discrip.text == ""))
       {
            MessageBox.ShowMessage("Please enter a Title");
            return;
       }
       if (!dltoggle.isOn) 
       {

            // DateTime deadline = new DateTime(2021 + year.value, month.value +1, day.value+1, hour.value, min.value,0);
            // print(deadline.ToString());
            int[] deadline = { min.value*5, hour.value, day.value + 1,  month.value + 1, 2022 + year.value, };
            int yearV = deadline[4],
                monthV = deadline[3],
                dayV = deadline[2],
                hourV = deadline[1],
                minuteV = deadline[0];

            string deadlineString = "" + yearV + "-" + monthV.ToString("D2") + "-" + dayV.ToString("D2") + " " + hourV.ToString("D2") + ":" + minuteV.ToString("D2") + ":00.0";

            if (DateTime.TryParse(deadlineString, out _))
            {
                tm.create_newTask(titel.text, discrip.text, deadline, prio.value);
            }
            else
            {
                MessageBox.ShowMessage("Please enter a valid Date");
                return;
            }
           
       }
       else
       {
            tm.create_newTask(titel.text, discrip.text, null, prio.value); // null -> Datetime.minvalue wenn zur�ckwechsel auf Datetime
       }

        scenlaod.loadScene(1);
   }
    public void grey()
    {
       
    }

    
}
