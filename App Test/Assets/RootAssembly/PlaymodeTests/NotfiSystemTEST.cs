using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NotfiSystemTEST
{

    GameObject gameObject;
    GameObject gameObj2;

    GameObject saveObjSubject;
    Taskmaster dataMaster;
    //Subject subj;

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
        dataMaster.SetNotificatioSystem(gameObj2.GetComponent<NotificationSystem>());
        Debug.Log(dataMaster);

    }
   
   
}
