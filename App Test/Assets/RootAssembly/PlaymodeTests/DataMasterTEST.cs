using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DataMasterTEST
{
    GameObject gameObject;
    Taskmaster dataMaster;

    [SetUp]
    public void BeforeEveryTest()
    {
        gameObject = Resources.Load("Prefabs/Datamaster") as GameObject;
        dataMaster = gameObject.GetComponent<Taskmaster>();
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
}
