using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class DialogueTriggerChurch : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Lista de conversaciones que se emitirán en orden y se ciclarán.")]
    public List<ConversationSO> conversations = new List<ConversationSO>();

    [Header("Global override")]
    [Tooltip("Si este flag global está en TRUE, se ignorará la lista.")]
    public GlobalBoolFlag overrideFlag;

    [Tooltip("Conversación que se enviará siempre cuando overrideFlag sea TRUE.")]
    public ConversationSO forcedConversation;

    [Header("State (runtime)")]
    [SerializeField, Tooltip("Índice de la siguiente conversación a emitir.")]
    private int nextIndex = 0;

    [Header("Broadcasting events")]
    public ConversationSOGameEvent conversationRequestEvent;

    /// <summary>
    /// Lanza un único ConversationSO:
    /// - Si overrideFlag == true y forcedConversation asignada -> envía forcedConversation.
    /// - En caso contrario -> envía conversations[nextIndex] y avanza con wrap-around.
    /// </summary>
    public void TriggerConversation()
    {
        // 1) ¿Override activo?
        if (overrideFlag != null && overrideFlag.Value)
        {
            if (forcedConversation != null)
            {
                conversationRequestEvent?.Raise(forcedConversation);
                Debug.Log($"[Override] Enviado: {forcedConversation.name}");
                return; // No avanzamos índice cuando hay override
            }
            else
            {
                Debug.LogWarning($"{name}: overrideFlag está TRUE pero 'forcedConversation' no está asignada. Usaré la lista.");
            }
        }

        // 2) Flujo normal por lista
        if (conversations == null || conversations.Count == 0)
        {
            Debug.LogWarning($"{name}: No hay conversaciones configuradas en 'conversations'.");
            return;
        }

        var convo = conversations[nextIndex];
        if (convo == null)
        {
            Debug.LogWarning($"{name}: conversations[{nextIndex}] es null.");
        }
        else
        {
            conversationRequestEvent?.Raise(convo);
            Debug.Log($"Enviado: {convo.name} (índice {nextIndex})");
        }

        // Avanza y cicla
        nextIndex = (nextIndex + 1) % conversations.Count;
    }

    /// <summary>Reinicia el ciclo al inicio (índice 0).</summary>
    [ContextMenu("Reset Cycle")]
    public void ResetCycle() => nextIndex = 0;

    /// <summary>Permite fijar el índice manualmente desde código.</summary>
    public void SetNextIndex(int index)
    {
        if (conversations == null || conversations.Count == 0) { nextIndex = 0; return; }
        nextIndex = Mathf.Clamp(index, 0, conversations.Count - 1);
    }

    /// <summary>Consulta cuál sería la próxima conversación sin emitirla.</summary>
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
