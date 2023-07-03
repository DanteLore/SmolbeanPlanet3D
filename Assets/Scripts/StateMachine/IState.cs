using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState 
{
    public void Tick();

    public void OnEnter();

    public void OnExit();
}
