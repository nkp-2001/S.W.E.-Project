using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class taskmaster : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<task> tasklist;
    private void Awake()
    {
        taskmaster[] objs = FindObjectsOfType<taskmaster>(); //Sigenton , Scenenwechesel löscht es nicht 

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
    public class task
    {
        string titel;
        string description;
        Calendar deadline;
        int prio;
        

        bool redo = false;
        bool done = false;
        bool sucess = false;

        public task(string t, string d, Calendar c, int p)
        {
            titel = t;
            description = d;
            deadline = c;
            prio = p;
        }

        public string Titel { get => titel; set => titel = value; }
        public string Description { get => description; set => description = value; }
        public Calendar Deadline { get => deadline; set => deadline = value; }

        public void change(string t, string d, Calendar c, int p)
        {
            titel = t;
            description = d;
            deadline = c;
            prio = p;
        }

    }
    public void create_newTask()
    {
        
        
    
    }

}
