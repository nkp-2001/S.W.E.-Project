using UnityEngine;
using System;


 ///////////// Task ////////////
[Serializable]
 public class Task
 {
        [SerializeField] string titel;
        [SerializeField] string description;
        [SerializeField] int[] deadline = null;  /// DateTime nicht (so leicht) serizaible | dewegen muss auf int[] ausgeweischt werden

        [SerializeField] int deadlineChannel_ID = 0;
        [SerializeField] float prio;

        [SerializeField] bool redo = false;
        [SerializeField] bool failedprevios = false;
        [SerializeField] int failedtimes = 0;
        [SerializeField] int sucessedtimes = 0;

        [SerializeField] int nextDeadlineIndex = 0;
       

        [SerializeField] bool sucess = false;
        [SerializeField] bool done = false;
       

        public Task(string t, string d, int[] dt, float p)
        {
            titel = t;
            description = d;
            deadline = dt;
            prio = p;       
        }
        public Task(string t, string d, int[] dt, float p,int dlID)
        {
            titel = t;
            description = d;
            deadline = dt;
            prio = p;
            deadlineChannel_ID = dlID;
        }
        public Task(string t, string d, int[] dt, float p, int dlID,int retDtDayes)
        {
            titel = t;
            description = d;
            deadline = dt;
            prio = p;
            deadlineChannel_ID = dlID;
            nextDeadlineIndex = retDtDayes;
            redo = retDtDayes != 0;
        }
        

        


        public string Titel { get => titel; set => titel = value; }
        public string Description { get => description; set => description = value; }
        public float Prio { get => prio; set => prio = value; }
        public int[] Deadline { get => deadline; set => deadline = value; }
        public int DeadlineChannel_ID { get => deadlineChannel_ID; set => deadlineChannel_ID = value; }
        public bool Done { get => done; set => done = value; }
        public bool Redo { get => redo; set => redo = value; }
        public bool Sucess { get => sucess; set => sucess = value; }
        public int NextDeadlineIndex { get => nextDeadlineIndex; set => nextDeadlineIndex = value; }
        public bool Failedprevios { get => failedprevios; set => failedprevios = value; }
        public int Failedtimes { get => failedtimes; set => failedtimes = value; }
        public int Sucessedtimes { get => sucessedtimes; set => sucessedtimes = value; }
        public int NextDeadlineIndex1 { get => nextDeadlineIndex; set => nextDeadlineIndex = value; }

    public override bool Equals(object obj)
    {
        // eigentlich überflüssig da es wegen der überladenung nicht eintretten kann 
        if (obj.GetType() != typeof(Task))
        {
            return false;
        }
        else 
        {
            return true;
        }
        
    }
    public bool Equals(Task obj)
    {
        if (obj.Titel == Titel | obj.Description == description | obj.deadline == deadline | obj.DeadlineChannel_ID == DeadlineChannel_ID | obj.Prio == prio)
        {
            return true;
        }
        return false;

    }
}

