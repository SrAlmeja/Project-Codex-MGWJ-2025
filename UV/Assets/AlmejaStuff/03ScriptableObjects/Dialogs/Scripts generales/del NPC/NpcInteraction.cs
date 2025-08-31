using UnityEngine;
using ScriptableObjectArchitecture;

public class NpcInteraction : MonoBehaviour
{
    [Header("Configuration")]
    public string interactableTag;

    [Header("Broadcasting events")]
    public BoolGameEvent interactionRequestEvent;

    private Interactable _interactable;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(interactableTag))
        {
            var interactable = this.GetComponent<Interactable>();
            this._interactable = interactable;
        }

        this.interactionRequestEvent.Raise((_interactable != null));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(interactableTag))
        {
            this._interactable = null;
        }

        this.interactionRequestEvent.Raise((_interactable != null));
    }

    public void EnableInteractable()
    {
        Debug.Log("Se lanzó el evento global");
        if (this._interactable != null)
        {
            Debug.Log("lanzamos evento en el script interact");
            this._interactable.Interact();
        }
    }
}
