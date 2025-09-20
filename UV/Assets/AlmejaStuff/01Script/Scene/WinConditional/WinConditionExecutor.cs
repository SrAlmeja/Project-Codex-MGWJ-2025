using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WinConditionExecutor : MonoBehaviour
{
    [Header("Condiciones de victoria")]
    [SerializeField] private WinConditionalSet conditionSet;

    [Header("Acciones al ganar")]
    [SerializeField] private UnityEvent onWin;

    [Header("Objetos a activar")]
    [SerializeField] private List<GameObject> objectsToActivate = new();

    [Header("Objetos a desactivar")]
    [SerializeField] private List<GameObject> objectsToDeactivate = new();

    private void Awake()
    {
        if (conditionSet != null)
            conditionSet.OnWin += HandleWin;
    }

    private void OnDestroy()
    {
        if (conditionSet != null)
            conditionSet.OnWin -= HandleWin;
    }

    private void HandleWin()
    {
        Debug.Log("[WinConditionExecutor] ¡Nivel completado!");

        foreach (var obj in objectsToActivate)
            if (obj != null) obj.SetActive(true);

        foreach (var obj in objectsToDeactivate)
            if (obj != null) obj.SetActive(false);

        onWin?.Invoke();
    }

}
