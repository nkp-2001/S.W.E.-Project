using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    public GameObject todo;

    public void OnButtonClick()
    {
        todo.transform.position = new Vector2(-1320, 125);
    }
}
