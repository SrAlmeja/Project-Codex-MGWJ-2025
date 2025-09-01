using UnityEngine;
using ScriptableObjectArchitecture;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Lista de conversaciones que se emitirán en orden y se ciclarán.")]
    public List<ConversationSO> conversations = new List<ConversationSO>();

    [Header("State (runtime)")]
    [SerializeField, Tooltip("Índice de la siguiente conversación a emitir.")]
    private int nextIndex = 0;

    [Header("Broadcasting events")]
    public ConversationSOGameEvent conversationRequestEvent;

    /// <summary>
    /// Lanza un único ConversationSO (el actual) y avanza el índice con wrap-around.
    /// </summary>
    public void TriggerConversation()
    {
        if (conversations == null || conversations.Count == 0)
        {
            Debug.LogWarning($"{name}: No hay conversaciones configuradas en 'conversations'.");
            return;
        }

        // Toma la conversación actual
        var convo = conversations[nextIndex];
        if (convo == null)
        {
            Debug.LogWarning($"{name}: conversations[{nextIndex}] es null.");
        }
        else
        {
            conversationRequestEvent?.Raise(convo);
            Debug.Log($"Ya se lanzó: {convo.name} (índice {nextIndex})");
        }

        // Avanza y cicla
        nextIndex = (nextIndex + 1) % conversations.Count;
    }

    /// <summary>
    /// Reinicia el ciclo al inicio (índice 0).
    /// </summary>
    [ContextMenu("Reset Cycle")]
    public void ResetCycle() => nextIndex = 0;

    /// <summary>
    /// Permite fijar el índice manualmente desde código.
    /// </summary>
    public void SetNextIndex(int index)
    {
        if (conversations == null || conversations.Count == 0) { nextIndex = 0; return; }
        nextIndex = Mathf.Clamp(index, 0, conversations.Count - 1);
    }

    /// <summary>
    /// Consulta cuál sería la próxima conversación sin emitirla.
    /// </summary>
    public ConversationSO PeekNext()
    {
        if (conversations == null || conversations.Count == 0) return null;
        return conversations[Mathf.Clamp(nextIndex, 0, conversations.Count - 1)];
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (conversations == null) return;
        if (conversations.Count == 0) { nextIndex = 0; return; }
        nextIndex = Mathf.Clamp(nextIndex, 0, conversations.Count - 1);
    }
#endif
}
