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
    Taskmaster dataMaster;

    [SetUp]
    public void BeforeEveryTest()
    {
        gameObject = Resources.Load("Prefabs/Datamaster") as GameObject;
        dataMaster = gameObject.GetComponent<Taskmaster>();
        dataMaster.Directoryname = "/testdir/";
        dataMaster.Filename = "testsave";

        gameObj2 = Resources.Load("Prefabs/NotificationSystem") as GameObject;
        dataMaster.SetNotificatioSystem(gameObj2.GetComponent<NotificationSystem>());
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
      
        dataMaster.CreateNewTask("name", "beschreibung", new int[] { 54, 14, 1, 7, 2022 }, 2, 1); // string t, string d, int[] dt, float p,int repeatindex
        dataMaster.CreateNewTask("name", "beschreibung", new int[] { 54, 14, 1, 7, 2022 }, 2, 1); 
        dataMaster.Reload(); // um sicherzugehen dass was abgespeichert ist , das ist was dann geladen wird 
        List<Task> tl = dataMaster.GetTasks();
        List<Task> ExpectList = new List<Task>();
        ExpectList.Add(new Task("name", "beschreibung", new int[] { 54, 14, 1, 7, 2022 }, 2, 1, 1));
        ExpectList.Add(new Task("name(1)", "beschreibung", new int[] { 54, 14, 1, 7, 2022 }, 2, 1, 1));
        
       foreach (Task t in tl)
       {
            Debug.Log(t.Titel);
       }
        Assert.True(Enumerable.SequenceEqual(tl,ExpectList)); 
    }
    [UnityTest]
    public IEnumerator TestEdit() 
    {
        yield return new WaitForEndOfFrame();
        dataMaster.CreateNewTask("name", "beschreibung", new int[] { 54, 14, 1, 7, 2022 }, 2, 1);
        dataMaster.ChangeTask(dataMaster.GetTasks()[0], "nameE", "beschreibungE", new int[] { 54, 14, 1, 7, 2023 }, 3, 7);
        Assert.True(dataMaster.GetTasks()[0].Equals(new Task("nameE", "beschreibungE", new int[] { 54, 14, 1, 7, 2023 }, 3, 7, 1)));
    }
}
