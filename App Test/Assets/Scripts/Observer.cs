using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver 
{
    public void SubscribeToEvents_Start();
    public void UnsubscribeToAllEvents(); //On Display/On Destroy Benutzten ansonsten k�nnte es zu Exception kommen
}
