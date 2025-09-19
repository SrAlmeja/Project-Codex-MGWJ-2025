using UnityEngine;
using System;

public class TagSelector : MonoBehaviour
{
    [Header("Tag Selector")]
    [SerializeField] private PlayerType playerType;
    [HideInInspector] public PlayerType PlayerType => playerType;
    public static event Action<TagSelector> OnTagSelectorReady;

    private void Start()
    {
        Debug.Log($"[TagSelector] Evento OnTagSelectorReady disparado con tipo: {playerType}");
        OnTagSelectorReady?.Invoke(this);
    }
}

public enum PlayerType
{
    Sacerdote,
    Raton,
    Ixquic,
    Hunampu,
    Ixbalanque
}
