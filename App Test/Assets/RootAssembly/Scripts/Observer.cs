using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver 
{
    public void SubscribeToEvents_Start();
    public void UnsubscribeToAllEvents(); // Wenn sie gel�scht werden m�ssen sie sich wieder "abmelden" 
}

