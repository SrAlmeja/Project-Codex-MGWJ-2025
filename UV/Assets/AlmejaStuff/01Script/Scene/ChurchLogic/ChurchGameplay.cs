using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChurchGameplay : MonoBehaviour
{
    public SOBoolean IsMissionComplete;
    public SOBoolean ReadyToNextLevel;
    [SerializeField] private GameObject specialObject;

    void Start()
    {
        IsMissionComplete.Value = false;
    }
    
    private void NextLevelCondition()
    {
    }
}
