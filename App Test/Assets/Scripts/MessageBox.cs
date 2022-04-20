using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageBox : MonoBehaviour
{
    private static GameObject instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = gameObject;
        }
        
        gameObject.SetActive(false);
    }

    public static void ShowMessage(string message)
    {
        if(instance != null)
        {
            instance.GetComponentInChildren<TextMeshProUGUI>().text = message;
            instance.SetActive(true);
        }
        else Debug.LogError("instance is null");
    }

    public static void HideMessage()
    {
        instance.SetActive(false);
    }
}
