using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaskPrototype : MonoBehaviour //!nicht die Child Strukur anfassen
{
    private Taskmaster taskMaster;
    private Taskmaster.Task task;

    TextMeshProUGUI discrTM;
    GameObject dircTobj;
    RectTransform rt;

    public void Setup(Taskmaster.Task t, Transform taskContainer)
    {
        task = t;

       // transform.Find("TaskPreview").GetComponentInChildren<TextMeshProUGUI>().text = t.Titel;
        transform.GetChild(0).GetComponentsInChildren<TextMeshProUGUI>()[0].text = t.Titel;
       
        if (t.Deadline != null && t.Deadline.Length !=0)
        {
           transform.GetChild(0).GetComponentsInChildren<TextMeshProUGUI>()[1].text =  //Format("{0:00} damit bei beispiel weise 6.2.2022 -> 06.02.2022 wird und so gleich lang ist wie z.b 12.11.2022
                "DT:" + string.Format("{0:00}", t.Deadline[2]) + "." + string.Format("{0:00}", t.Deadline[3]) + "." 
                + t.Deadline[4] + "|" + string.Format("{0:00}", t.Deadline[1]) + ":" + string.Format("{0:00}", t.Deadline[0]);
        }
        dircTobj = transform.GetChild(0).GetChild(2).gameObject;

       // discrTM = transform.GetChild(0).GetComponentsInChildren<TextMeshProUGUI>()[2];
        dircTobj.GetComponentInChildren<TextMeshProUGUI>().text = "Priotät: " + t.Prio + "\n Discirption: " + t.Description;
        //  discrTM.enabled = false;
        dircTobj.SetActive(false);





        transform.SetParent(taskContainer);
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);
    }

    public void SetTaskToDone()
    {

        for (int index = transform.GetSiblingIndex(); index >= 0; index--) //Bug Verhinderer 
        {
            transform.parent.GetChild(index).GetComponent<TaskPrototype>().Hidediscrip();
        }

        taskMaster.removeTask(task);
        Destroy(gameObject);
    }
    public void SelfDestroyTest()
    {
        Destroy(gameObject);
    }
    public void ToogleDiscrip()
    {
        if (dircTobj.activeSelf == true)
        {
            Hidediscrip();
        }
        else
        {
            Showdisrip();
        }

    }

    private void Showdisrip()
    {
        dircTobj.SetActive(true);

        // rt.SetPositionAndRotation(new Vector3(rt.position.x, rt.position.y + 2, rt.position.z),Quaternion.identity);

        for (int index = transform.GetSiblingIndex() - 1; index >= 0; index--)
        {

            RectTransform rect = transform.parent.GetChild(index).GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(2500, rect.sizeDelta.y +900); 

        }
    }

    public void Hidediscrip()
    {
        if (dircTobj.activeSelf == true)
        {
            dircTobj.SetActive(false);
            for (int index = transform.GetSiblingIndex()-1; index >= 0; index--)
            {
                RectTransform rect = transform.parent.GetChild(index).GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(2500, rect.sizeDelta.y - 900);
            }
            //rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rt.rect.height * 1/6);
        }
    }

    private void Start()
    {
        taskMaster = FindObjectOfType<Taskmaster>();
        rt = GetComponent<RectTransform>();
    }
}
