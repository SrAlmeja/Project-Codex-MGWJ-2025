using UnityEngine;
using ScriptableObjectArchitecture;
using System.Collections.Generic;

public class NpcInteraction : MonoBehaviour
{
    [Header("Configuration")]
    public string interactableTag;

    [Header("Tecla E")]
    public GameObject eKey; //Tecla que indica que el jugador debe de presionar la tecla E para interactuar

    [Header("Broadcasting events")]
    public BoolGameEvent interactionRequestEvent;

    private Interactable _interactable;

    [Tooltip("Material que usará ESTE objeto mientras un collider con el tag esté dentro del trigger.")]
    public Material highlightMaterial;

    [Tooltip("Afectar también los SpriteRenderers hijos de este objeto.")]
    public bool affectChildrenSpriteRenderers = true;

    // Cache de los SpriteRenderers de ESTE objeto y sus materiales originales
    private SpriteRenderer[] _selfRenderers;
    private readonly Dictionary<SpriteRenderer, Material> _originalSelfMats = new();

    private void Awake()
    {
        eKey.SetActive(false);

        _selfRenderers = affectChildrenSpriteRenderers
            ? GetComponentsInChildren<SpriteRenderer>(true)
            : new[] { GetComponent<SpriteRenderer>() };

        foreach (var sr in _selfRenderers)
        {
            if (sr == null) continue;
            _originalSelfMats[sr] = sr.sharedMaterial;   // puede ser Sprites-Default u otro
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(interactableTag))
        {
            var interactable = this.GetComponent<Interactable>();
            this._interactable = interactable;
        }

        this.interactionRequestEvent.Raise((_interactable != null));
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag(interactableTag))
        {
            eKey.SetActive(true);

            if (highlightMaterial != null)
                ApplyHighlightToSelf();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(interactableTag))
        {
            eKey.SetActive(false);

            this._interactable = null;
            RestoreSelfMaterials();
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

    // ---------- Helpers (aplican sobre ESTE objeto) ----------

    private void ApplyHighlightToSelf()
    {
        if (highlightMaterial == null) return;

        foreach (var sr in _selfRenderers)
        {
            if (sr == null) continue;
            if (sr.sharedMaterial != highlightMaterial)
                sr.sharedMaterial = highlightMaterial;   // reemplaza Sprites-Default por tu material
        }
    }

    private void RestoreSelfMaterials()
    {
        foreach (var sr in _selfRenderers)
        {
            if (sr == null) continue;
            if (_originalSelfMats.TryGetValue(sr, out var original))
                sr.sharedMaterial = original;            // restaura (normalmente Sprites-Default)
        }
    }
}
