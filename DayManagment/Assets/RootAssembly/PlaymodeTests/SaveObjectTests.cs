using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SaveObjectTests
{
    private SaveObject saveObject;

    private Task task1_no_redo, task2_no_redo, task3_no_redo, task4_no_redo;
    private Task task1_redo, task2_redo, task3_redo, task4_redo;

    private Appointment appointment1, appointment2, appointment3, appointment4;

    [SetUp]
    public void BeforeEveryTest()
    {
        saveObject = new SaveObject();

        task1_no_redo = new Task("task1", "task1 description", new int[] { 1, 1, 1, 1, 1, 1 }, 0);
        task2_no_redo = new Task("task2", "task2 description", new int[] { 1, 1, 1, 1, 1, 1 }, 0);
        task3_no_redo = new Task("task3", "task3 description", new int[] { 1, 1, 1, 1, 1, 1 }, 0);
        task4_no_redo = new Task("task4", "task4 description", new int[] { 1, 1, 1, 1, 1, 1 }, 0);

        task1_redo = new Task("task1", "task1 description", new int[] { 1, 1, 1, 1, 1, 1 }, 0, 0, 1);
        task2_redo = new Task("task2", "task2 description", new int[] { 1, 1, 1, 1, 1, 1 }, 0, 0, 1);
        task3_redo = new Task("task3", "task3 description", new int[] { 1, 1, 1, 1, 1, 1 }, 0, 0, 1);
        task4_redo = new Task("task4", "task4 description", new int[] { 1, 1, 1, 1, 1, 1 }, 0, 0, 1);

        appointment1 = new Appointment("appointment1", "appointment1 description", new int[] { 1, 1, 1, 1, 1, 1 }, new int[] { 1, 1, 1, 1, 1, 1 }, 0, 0);
        appointment2 = new Appointment("appointment2", "appointment2 description", new int[] { 1, 1, 1, 1, 1, 1 }, new int[] { 1, 1, 1, 1, 1, 1 }, 0, 0);
        appointment3 = new Appointment("appointment3", "appointment3 description", new int[] { 1, 1, 1, 1, 1, 1 }, new int[] { 1, 1, 1, 1, 1, 1 }, 0, 0);
        appointment4 = new Appointment("appointment4", "appointment4 description", new int[] { 1, 1, 1, 1, 1, 1 }, new int[] { 1, 1, 1, 1, 1, 1 }, 0, 0);
    }

    private bool AreEqual(SaveObject saveObject1, SaveObject saveObject2)
    {
        for(int i = 0; i < saveObject1.GetList().Count; ++i)
        {
            if (!saveObject1.GetList()[i].Equals(saveObject2.GetList()[i]))
            {
                return false;
            }
        }

        for (int i = 0; i < saveObject1.GetArchivedList().Count; ++i)
        {
            if (!saveObject1.GetArchivedList()[i].Equals(saveObject2.GetArchivedList()[i]))
            {
                return false;
            }
        }

        for (int i = 0; i < saveObject1.GetWaitingList().Count; ++i)
        {
            if (!saveObject1.GetWaitingList()[i].Equals(saveObject2.GetWaitingList()[i]))
            {
                return false;
            }
        }

        for (int i = 0; i < saveObject1.GetAppointmentList().Count; ++i)
        {
            if (!saveObject1.GetAppointmentList()[i].Equals(saveObject2.GetAppointmentList()[i]))
            {
                return false;
            }
        }

        return true;
    }

    [Test]
    public void Test_Serializability()
    {
        string saveObjectJSON = JsonUtility.ToJson(saveObject);
        SaveObject saveObjectDeserialized = JsonUtility.FromJson<SaveObject>(saveObjectJSON);

        Assert.IsTrue(AreEqual(saveObject, saveObjectDeserialized));
    }

    [Test]
    public void TestAddNewToList()
    {
        saveObject.AddNewToList(task1_no_redo);

        Assert.AreEqual(task1_no_redo, saveObject.GetList()[0]);
    }

    [Test]
    public void TestRemoveFromList_By_Index()
    {
        saveObject.AddNewToList(task1_no_redo);
        saveObject.AddNewToList(task2_no_redo);
        saveObject.AddNewToList(task3_no_redo);
        saveObject.AddNewToList(task4_no_redo);

        saveObject.RemoveFromList(2);

        Assert.AreEqual(3, saveObject.GetList().Count);
        Assert.AreEqual(task4_no_redo, saveObject.GetList()[2]);
        Assert.IsFalse(saveObject.GetList().Contains(task3_no_redo));
    }

    [Test]
    public void TestRemoveFromList_By_Task()
    {
        saveObject.AddNewToList(task1_no_redo);
        saveObject.AddNewToList(task2_no_redo);
        saveObject.AddNewToList(task3_no_redo);
        saveObject.AddNewToList(task4_no_redo);

        saveObject.RemoveFromList(task3_no_redo);

        Assert.AreEqual(3, saveObject.GetList().Count);
        Assert.AreEqual(task4_no_redo, saveObject.GetList()[2]);
        Assert.IsFalse(saveObject.GetList().Contains(task3_no_redo));
    }

    [Test]
    public void TestRemoveFromArchiveList()
    {
        saveObject.AddNewToList(task1_no_redo);
        saveObject.AddNewToList(task2_no_redo);
        saveObject.AddNewToList(task3_no_redo);
        saveObject.AddNewToList(task4_no_redo);

        saveObject.RemoveFromList(task1_no_redo);
        saveObject.RemoveFromList(task2_no_redo);
        saveObject.RemoveFromList(task3_no_redo);
        saveObject.RemoveFromList(task4_no_redo);

        saveObject.RemoveFromArchiveList(task3_no_redo);

        Assert.AreEqual(3, saveObject.GetArchivedList().Count);
        Assert.AreEqual(task4_no_redo, saveObject.GetArchivedList()[2]);
        Assert.IsFalse(saveObject.GetArchivedList().Contains(task3_no_redo));
    }

    [Test]
    public void TestRemoveFromWaitList()
    {
        saveObject.AddNewToList(task1_redo);
        saveObject.AddNewToList(task2_redo);
        saveObject.AddNewToList(task3_redo);
        saveObject.AddNewToList(task4_redo);

        saveObject.RemoveFromList(task1_redo);
        saveObject.RemoveFromList(task2_redo);
        saveObject.RemoveFromList(task3_redo);
        saveObject.RemoveFromList(task4_redo);

        saveObject.RemoveFromWaitList(task3_redo);

        Assert.AreEqual(3, saveObject.GetWaitingList().Count);
        Assert.AreEqual(task4_redo, saveObject.GetWaitingList()[2]);
        Assert.IsFalse(saveObject.GetWaitingList().Contains(task3_redo));
    }

    [Test]
    public void TestAddNewAppointment()
    {
        saveObject.AddNewAppointment(appointment1);

        Assert.AreEqual(appointment1, saveObject.GetAppointmentList()[0]);
    }

    [Test]
    public void TestRemoveAppointment()
    {
        saveObject.AddNewAppointment(appointment1);
        saveObject.AddNewAppointment(appointment2);
        saveObject.AddNewAppointment(appointment3);
        saveObject.AddNewAppointment(appointment4);

        saveObject.RemoveAppointment(appointment3);

        Assert.AreEqual(3, saveObject.GetAppointmentList().Count);
        Assert.AreEqual(appointment4, saveObject.GetAppointmentList()[2]);
        Assert.IsFalse(saveObject.GetAppointmentList().Contains(appointment3));
    }

    [Test]
    public void TestGetList()
    {
        saveObject.AddNewToList(task1_no_redo);
        saveObject.AddNewToList(task2_no_redo);
        saveObject.AddNewToList(task3_no_redo);
        saveObject.AddNewToList(task4_no_redo);

        Assert.AreEqual(task1_no_redo, saveObject.GetList()[0]);
        Assert.AreEqual(task2_no_redo, saveObject.GetList()[1]);
        Assert.AreEqual(task3_no_redo, saveObject.GetList()[2]);
        Assert.AreEqual(task4_no_redo, saveObject.GetList()[3]);
    }

    [Test]
    public void TestGetArchivedList()
    {
        saveObject.AddNewToList(task1_no_redo);
        saveObject.AddNewToList(task2_no_redo);
        saveObject.AddNewToList(task3_no_redo);
        saveObject.AddNewToList(task4_no_redo);

        saveObject.RemoveFromList(task1_no_redo);
        saveObject.RemoveFromList(task2_no_redo);
        saveObject.RemoveFromList(task3_no_redo);
        saveObject.RemoveFromList(task4_no_redo);

        Assert.AreEqual(task1_no_redo, saveObject.GetArchivedList()[0]);
        Assert.AreEqual(task2_no_redo, saveObject.GetArchivedList()[1]);
        Assert.AreEqual(task3_no_redo, saveObject.GetArchivedList()[2]);
        Assert.AreEqual(task4_no_redo, saveObject.GetArchivedList()[3]);
    }

    [Test]
    public void TestGetWaitingList()
    {
        saveObject.AddNewToList(task1_redo);
        saveObject.AddNewToList(task2_redo);
        saveObject.AddNewToList(task3_redo);
        saveObject.AddNewToList(task4_redo);

        saveObject.RemoveFromList(task1_redo);
        saveObject.RemoveFromList(task2_redo);
        saveObject.RemoveFromList(task3_redo);
        saveObject.RemoveFromList(task4_redo);

        Assert.AreEqual(task1_redo, saveObject.GetWaitingList()[0]);
        Assert.AreEqual(task2_redo, saveObject.GetWaitingList()[1]);
        Assert.AreEqual(task3_redo, saveObject.GetWaitingList()[2]);
        Assert.AreEqual(task4_redo, saveObject.GetWaitingList()[3]);
    }

    [Test]
    public void TestClearArchiveList()
    {
        saveObject.AddNewToList(task1_no_redo);
        saveObject.AddNewToList(task2_no_redo);
        saveObject.AddNewToList(task3_no_redo);
        saveObject.AddNewToList(task4_no_redo);

        saveObject.RemoveFromList(task1_no_redo);
        saveObject.RemoveFromList(task2_no_redo);
        saveObject.RemoveFromList(task3_no_redo);
        saveObject.RemoveFromList(task4_no_redo);

        Assert.AreEqual(4, saveObject.GetArchivedList().Count);

        saveObject.ClearArchiveList();

        Assert.AreEqual(0, saveObject.GetArchivedList().Count);
    }

    [Test]
    public void TestGetAppointmentList()
    {
        saveObject.AddNewAppointment(appointment1);
        saveObject.AddNewAppointment(appointment2);
        saveObject.AddNewAppointment(appointment3);
        saveObject.AddNewAppointment(appointment4);

        Assert.AreEqual(appointment1, saveObject.GetAppointmentList()[0]);
        Assert.AreEqual(appointment2, saveObject.GetAppointmentList()[1]);
        Assert.AreEqual(appointment3, saveObject.GetAppointmentList()[2]);
        Assert.AreEqual(appointment4, saveObject.GetAppointmentList()[3]);
    }

    [Test]
    public void ClearCurrentList()
    {
        saveObject.AddNewToList(task1_no_redo);
        saveObject.AddNewToList(task2_no_redo);
        saveObject.AddNewToList(task3_no_redo);
        saveObject.AddNewToList(task4_no_redo);

        Assert.AreEqual(4, saveObject.GetList().Count);

        saveObject.ClearCurrentList();

        Assert.AreEqual(0, saveObject.GetList().Count);
    }

    [Test]
    public void TestChangeTask()
    {
        saveObject.AddNewToList(task1_no_redo);
        saveObject.AddNewToList(task2_no_redo);
        saveObject.AddNewToList(task3_no_redo);
        saveObject.AddNewToList(task4_no_redo);

        saveObject.ChangeTask(task2_no_redo, "changed title", "changed description", new int[] { 2, 2, 2, 2, 2, 2, 2 }, 3, 4, 5);

        Assert.AreEqual("changed title", saveObject.GetList()[1].Title);
        Assert.AreEqual("changed description", saveObject.GetList()[1].Description);
    }

    [Test]
    public void TestChangeTaskCauseRepeat()
    {
        saveObject.AddNewToList(task1_no_redo);
        saveObject.AddNewToList(task2_no_redo);
        saveObject.AddNewToList(task3_no_redo);
        saveObject.AddNewToList(task4_no_redo);

        Assert.AreEqual(false, saveObject.GetList()[1].FailedPrevious);
        Assert.AreEqual(0, saveObject.GetList()[1].FailedTimes);

        int[] deadline = new int[] { 2, 2, 2, 2, 2, 2 };
        saveObject.ChangeTaskCauseRepeat(task2_no_redo, deadline, 123);

        Assert.AreEqual(123, saveObject.GetList()[1].DeadlineChannelId);
        Assert.AreEqual(deadline, saveObject.GetList()[1].Deadline);
        Assert.AreEqual(true, saveObject.GetList()[1].FailedPrevious);
        Assert.AreEqual(1, saveObject.GetList()[1].FailedTimes);
    }

    [Test]
    public void TestChangeAppointment()
    {
        saveObject.AddNewAppointment(appointment1);
        saveObject.AddNewAppointment(appointment2);
        saveObject.AddNewAppointment(appointment3);
        saveObject.AddNewAppointment(appointment4);

        saveObject.ChangeAppointment(appointment2, "changed title", "changed description", new int[] { 2, 2, 2, 2, 2, 2}, new int[] { 2, 2, 2, 2, 2, 2 }, 3, 4, 5);

        Assert.AreEqual("changed title", saveObject.GetAppointmentList()[1].Title);
        Assert.AreEqual("changed description", saveObject.GetAppointmentList()[1].Description);
    }
}
