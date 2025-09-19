using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinConditionListener : MonoBehaviour
{
    [SerializeField] private WinConditionalSet conditionSet;
    [SerializeField] private string conditionID;

    public void TriggerCondition()
    {
        conditionSet.MarkCompleted(conditionID);
    }

}
