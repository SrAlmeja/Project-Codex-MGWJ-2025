using UnityEngine;
using ScriptableObjectArchitecture;

public class NPCInteraction : MonoBehaviour
{
    [Header("Configuration")]
    public string playerTag;

    [Header("Broadcasting events")]
    public BoolGameEvent interactionRequestEvent;

    [Header("Boolean variable"), SerializeField]
    private SOBoolean canInteract;

    private Interactable _interactable;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            canInteract.Value = true;
            var interactable = this.GetComponent<Interactable>();
            this._interactable = interactable;
        }

        this.interactionRequestEvent.Raise((_interactable != null));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            canInteract.Value = false;
            this._interactable = null;
        }

        this.interactionRequestEvent.Raise((_interactable != null));
    }

    public void EnableInteractable()
    {
        if (this._interactable != null)
        {
            this._interactable.Interact();
            Debug.Log("se lanzó evento global");
        }
    }
}
