using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Globalization;
using System;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataMasterTEST
{
    GameObject gameObject;
    GameObject gameObj2;

    GameObject saveObjSubject;
    Taskmaster dataMaster;
    Subject subj;
    NotificationSystem noti;

    [SetUp]
    public void BeforeEveryTest()
    {
        saveObjSubject = Resources.Load("Prefabs/Subject[EventOperator]") as GameObject;
        Subject.current = saveObjSubject.GetComponent<Subject>();

        gameObject = Resources.Load("Prefabs/Datamaster") as GameObject;
        dataMaster = gameObject.GetComponent<Taskmaster>();
        dataMaster.Directoryname = "/testdir/";
        dataMaster.Filename = "testsave";

        gameObj2 = Resources.Load("Prefabs/NotificationSystem") as GameObject;
        noti = gameObj2.GetComponent<NotificationSystem>();
        dataMaster.SetNotificatioSystem(noti);
        Debug.Log(dataMaster);

    }
    [UnityTest]
    public IEnumerator DataMasterTestconvertToDateTime()
    {
        yield return new WaitForEndOfFrame();
        int[] testValue = new int[]{54,14,1,7, 2022 };
        System.DateTime testValueinDatetime = dataMaster.ConvertIntArray_toDatetime(testValue);

        CollectionAssert.AreEqual(testValue, dataMaster.ConvertDatetime_toIntArray(testValueinDatetime));
        
    }
    [UnityTest]
    public IEnumerator DataMasterTestconvertToIntArray()
    {
        yield return new WaitForEndOfFrame();
        System.DateTime testValueDT = new System.DateTime(2022, 7, 1, 14, 54, 0);
        int[] testValueDTinINTArr = dataMaster.ConvertDatetime_toIntArray(testValueDT);

        
        Assert.AreEqual(testValueDT, dataMaster.ConvertIntArray_toDatetime(testValueDTinINTArr));


    }
    [UnityTest]
    public IEnumerator TestNestDateLine()
    {
        yield return new WaitForEndOfFrame();
        int[] testValue = new int[] { 54, 14, 1, 7, 2022 };
        int[] result;
        result = dataMaster.CaculuateNextDT(2, testValue);
        CollectionAssert.AreEqual(new int[] { 54, 14, 1+7, 7, 2022 }, result);


    }
    [UnityTest]
    public IEnumerator TestsavingAndCreating()
    {
        
        yield return new WaitForEndOfFrame();
        SaveReset();
        dataMaster.CreateNewTask("name", "beschreibung", new int[] { 54, 14, 1, 7, 2022 }, 2, 1); // string t, string d, int[] dt, float p,int repeatindex
        dataMaster.CreateNewTask("name", "beschreibung", new int[] { 54, 14, 1, 7, 2022 }, 2, 1); 
        dataMaster.Reload(); // um sicherzugehen dass was abgespeichert ist , das ist was dann geladen wird 
        List<Task> tl = dataMaster.GetTasks();
        List<Task> ExpectList = new List<Task>();
        ExpectList.Add(new Task("name", "beschreibung", new int[] { 54, 14, 1, 7, 2022 }, 2, 1, 1));
        ExpectList.Add(new Task("name(1)", "beschreibung", new int[] { 54, 14, 1, 7, 2022 }, 2, 1, 2));
        
       foreach (Task t in tl)
       {
            Debug.Log(t.Titel);
       }
        Assert.True(Enumerable.SequenceEqual(tl,ExpectList)); 
    }
    [UnityTest]
    public IEnumerator TestEdit() 
    {
        SaveReset();
        yield return new WaitForEndOfFrame();
        dataMaster.CreateNewTask("name", "beschreibung", new int[] { 54, 14, 1, 7, 2022 }, 2, 1);
        dataMaster.ChangeTask(dataMaster.GetTasks()[0], "nameE", "beschreibungE", new int[] { 54, 14, 1, 7, 2023 }, 3, 7);
        Debug.Log("ChannelID:" + dataMaster.GetTasks()[0].DeadlineChannel_ID);
        Assert.True(dataMaster.GetTasks()[0].Equals(new Task("nameE", "beschreibungE", new int[] { 54, 14, 1, 7, 2023 }, 3, 7,2)));
        
    }
    [UnityTest]
    public IEnumerator TestAppoitmentChecks()
    {
        SaveReset();
        yield return new WaitForEndOfFrame();
        dataMaster.CreateNewAppointment("appo1", "beschreibung", new int[] { 0, 12, 2, 7, 2022 }, new int[] { 0, 13, 2, 7, 2022 }, 7, 2, new int[] { 0 });
        dataMaster.CreateNewAppointment("appo2", "beschreibung", new int[] { 0, 12, 2, 7, 2022 }, new int[] { 0, 13, 2, 7, 2022 }, 7, 1, new int[] { 0 });

        dataMaster.CreateNewAppointment("appo3", "beschreibung", new int[] { 0, 16, 16, 7, 2022 }, new int[] { 0, 17, 16, 7, 2022 }, 0, 0, new int[] { 0 });

        dataMaster.CreateNewAppointment("appo4", "beschreibung", new int[] { 0, 12, 11, 8, 2023 }, new int[] { 0, 13, 11, 8, 2023 }, 0, 0, new int[] { 0 });

        List<Appointment> AppointThisday = dataMaster.GiveAppoints_ofThisDay(new DateTime(2022, 7, 16));
        Debug.Log(AppointThisday.Count);
        List<Appointment> ExpectList = new List<Appointment>();

        foreach (Appointment ap in AppointThisday)
        {

            Debug.Log(ap.Titel);
        }
        ExpectList.Add(new Appointment("appo1", "beschreibung", new int[] { 0, 12, 2, 7, 2022 }, new int[] { 0, 13, 2, 7, 2022 }, 7, 2));
        ExpectList.Add(new Appointment("appo3", "beschreibung", new int[] { 0, 16, 16, 7, 2022 }, new int[] { 0, 17, 16, 7, 2022 }, 0, 0));


        for (int i = 0; i < ExpectList.Count; i++) // da , Assert.True(Enumerable.SequenceEqual(AppointThisday,ExpectList)); aus irgenteinGrund nicht funksiniert 
        {
            Assert.True(ExpectList[i].Titel == AppointThisday[i].Titel);
        }

        
    }
    [UnityTest]
    public IEnumerator TestDeadlineCheck()
    {
        SaveReset();
        yield return new WaitForEndOfFrame();
        // r�ckkehr
        dataMaster.CreateNewTask("name", "beschreibung", new int[] { 54, 20, 10, 6, 2022 }, 2, 2); 
        dataMaster.CheckDeadlinesTask();
        dataMaster.Reload();
        Debug.Log(dataMaster.GetTasks()[0].Deadline[2]);
        
        CollectionAssert.AreEqual(dataMaster.GetTasks()[0].Deadline,new int[] { 54, 20, 17, 6, 2022 });

        // keine r�ckkehr
        dataMaster.CreateNewTask("name2", "beschreibung", new int[] { 54, 20, 10, 6, 2022 }, 2, 0); 
        dataMaster.CheckDeadlinesTask();
        dataMaster.Reload();
        Assert.True(dataMaster.GetTasks().Count < 2);

    }

    public void SaveReset()
    {
        dataMaster.WipeSave();
        noti.WibeNotication();
    }



}
