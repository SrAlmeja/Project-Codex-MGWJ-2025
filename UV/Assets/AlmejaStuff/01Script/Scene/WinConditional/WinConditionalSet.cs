using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WinConditionalSet", menuName = "Almeja'sCSO/AWinConditionalSet", order = 1)]
public class WinConditionalSet : ScriptableObject
{
    [SerializeField] private List<string> requiredIDs = new();
    private HashSet<string> completedIDs = new();
    
    public bool IsComplete => completedIDs.Count >= requiredIDs.Count;

    public void MarkCompleted(string id)
    {
        if (requiredIDs.Contains(id) && !completedIDs.Contains(id))
        {
            completedIDs.Add(id);
            Debug.Log($"WinConditionalSet: {id} marked as completed.");
            if (IsComplete)
                OnWin?.Invoke();
        };
    }

    public void ResetConditions()
    {
        completedIDs.Clear();
    }
    
    public System.Action OnWin;
}
