using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using Unity.VisualScripting;
using UnityEngine;

public class MissionCompleteByZone : MonoBehaviour
{
    public SOBoolean IsMissionComplete;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IsMissionComplete.Value = true;            
        }
    }
}
