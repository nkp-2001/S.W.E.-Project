using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewTaskBox : MonoBehaviour, IObserver
{

    [SerializeField] Image MessageBox;
    [SerializeField] TextMeshProUGUI messagerText;

    // Start is called before the first frame update
    void Start()
    {
        NewTaskBox[] objs = FindObjectsOfType<NewTaskBox>();
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        //MessageBox = GetComponentInChildren<Image>();
        //messagerText = MessageBox.GetComponentInChildren<TextMeshProUGUI>();
        SubscribeToEvents_Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowBoxNewTask(string t, string d, int[] dt, float prio, int repeatindex)
    {
        if (MessageBox != null)
        {
            Debug.Log("11111");
            StartCoroutine(ShowText("Task was created"));
        }
    }

    public void ShowBoxTaskChange(Taskmaster.Task oldtask, string t, string d, int[] dt, float p, int repeatIndex)
    {
        Debug.Log("22222");
        StartCoroutine(ShowText("Task was changed"));
    }

    IEnumerator ShowText(string text)
    {
        MessageBox.color = new Color32(255, 255, 255, 100);
        Debug.Log(33333);
        //MessageBox.enabled = true;
        messagerText.text = text;
        yield return new WaitForSeconds(10);
        messagerText.text = "";
        MessageBox.color = new Color32(0, 0, 0, 0);   
    }

    public void SubscribeToEvents_Start()
    {
        Subject.current.OnNewTask += ShowBoxNewTask;
        Subject.current.OnTaskChange += ShowBoxTaskChange;
    }

    public void UnsubscribeToAllEvent()
    {
        Subject.current.OnNewTask -= ShowBoxNewTask;
        Subject.current.OnTaskChange -= ShowBoxTaskChange;
    }

    private void OnEnable()
    {
        //SubscribeToEvents_Start();
    }

    private void OnDestroy()
    {
        UnsubscribeToAllEvent();
    }

    public void UnsubscribeToAllEvents()
    {
        throw new System.NotImplementedException();
    }
}
