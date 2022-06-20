using UnityEngine;
using System;
using System.Linq;

[Serializable]
 public class Task
 {
        [SerializeField] string title;
        [SerializeField] string description;
        [SerializeField] int[] deadline = null;

        [SerializeField] int deadlineChannelId = 0;
        [SerializeField] float priority;

        [SerializeField] bool redo = false;
        [SerializeField] bool failedPrevious = false;
        [SerializeField] int failedTimes = 0;
        [SerializeField] int successfulTimes = 0;

        [SerializeField] int nextDeadlineIndex = 0;    

        [SerializeField] bool success = false;
        [SerializeField] bool done = false;

        public Task(string t, string d, int[] dt, float p)
        {
            title = t;
            description = d;
            deadline = dt;
            priority = p;       
        }

        public Task(string t, string d, int[] dt, float p,int dlID)
        {
            title = t;
            description = d;
            deadline = dt;
            priority = p;
            deadlineChannelId = dlID;
        }

        public Task(string t, string d, int[] dt, float p, int dlID,int retDtDayes)
        {
            title = t;
            description = d;
            deadline = dt;
            priority = p;
            deadlineChannelId = dlID;
            nextDeadlineIndex = retDtDayes;
            redo = retDtDayes != 0;
        }
        
        public string Title { get => title; set => title = value; }
        public string Description { get => description; set => description = value; }
        public float Priority { get => priority; set => priority = value; }
        public int[] Deadline { get => deadline; set => deadline = value; }
        public int DeadlineChannelId { get => deadlineChannelId; set => deadlineChannelId = value; }
        public bool Done { get => done; set => done = value; }
        public bool Redo { get => redo; set => redo = value; }
        public bool Success { get => success; set => success = value; }
        public int NextDeadlineIndex { get => nextDeadlineIndex; set => nextDeadlineIndex = value; }
        public bool FailedPrevious { get => failedPrevious; set => failedPrevious = value; }
        public int FailedTimes { get => failedTimes; set => failedTimes = value; }
        public int SuccessfulTimes { get => successfulTimes; set => successfulTimes = value; }

    public override bool Equals(object obj)
    {
        return this == obj;
    }

    public bool Equals(Task obj)
    {
        Debug.Log("ChannelID:" + obj.DeadlineChannelId + " ," + deadlineChannelId);

        return obj.Title == Title && obj.Description == description 
            && obj.Priority == priority & Enumerable.SequenceEqual(obj.deadline, deadline) 
            && obj.DeadlineChannelId == deadlineChannelId;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
