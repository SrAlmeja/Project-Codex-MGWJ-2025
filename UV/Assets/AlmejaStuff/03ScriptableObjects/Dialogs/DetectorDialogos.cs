using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectorDialogos : MonoBehaviour
{
    [Header("Flag global a modificar")]
    public GlobalBoolFlag globalFlag;

    [Header("Estado (runtime)")]
    [SerializeField] private bool hasEverBeenActivated = false;

    private void OnEnable()
    {
        if (hasEverBeenActivated) return; // Solo la primera vez
        hasEverBeenActivated = true;

        if (globalFlag != null)
        {
            globalFlag.SetTrue(); // Deja el flag global en TRUE para toda la app
            // Debug.Log($"[{name}] Primera activación. Global '{globalFlag.name}' = {globalFlag.Value}");
        }
        else
        {
            Debug.LogWarning($"{name}: No se asignó 'globalFlag'.");
        }
    }

    [ContextMenu("Reset Local Flag (solo este objeto)")]
    private void ResetLocalFlag() => hasEverBeenActivated = false;
}
