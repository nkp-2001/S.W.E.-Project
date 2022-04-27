using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver<T>
{
    public void Notify<T>(T args);
}

public interface IObservee<T>
{
    public void AddObserver(IObserver<T> observer);
}
