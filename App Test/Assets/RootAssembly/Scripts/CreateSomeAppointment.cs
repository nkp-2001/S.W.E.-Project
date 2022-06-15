using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSomeAppointment : MonoBehaviour
{
    [SerializeField] List<Appointment> AppointmentList;
    // Start is called before the first frame update
    void Start()
    {
        Taskmaster tm = FindObjectOfType<Taskmaster>();
        foreach (Appointment appo in AppointmentList)
        {
            tm.CreateNewAppointment(appo.Titel, appo.Desp, appo.StartTime, appo.EndTime, appo.Repeat, appo.Repeattimes,new int[1]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
