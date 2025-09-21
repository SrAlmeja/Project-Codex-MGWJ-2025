using System.Collections.Generic;
using UnityEngine;

public class SkinChanger : MonoBehaviour
{
    #region Variables

    [Header("Personajes en escena")]
    [SerializeField] private List<GameObject> characterReferences;

    private GameObject _selectedCharacter;
    
    #endregion

    #region Suscriptions

    private void OnEnable()
    {
        TagSelector.OnTagSelectorReady += HandleTagSelector;
    }

    private void OnDisable()
    {
        TagSelector.OnTagSelectorReady -= HandleTagSelector;
    }
    
    #endregion
    
    #region SkinChanger Functions
    
    
    private void HandleTagSelector(TagSelector selector)
    {
        Debug.Log($"[SkinChanger] Recibido tipo de jugador: {selector.PlayerType}");

        // Guardar posición del personaje actual antes de cambiar
        if (_selectedCharacter != null)
        {
            foreach (var child in _selectedCharacter.GetComponentsInChildren<Transform>(true))
            {
                if (child.CompareTag("Player"))
                {
                    PlayerTransitionData.SavePosition(child.position);
                    Debug.Log($"[SkinChanger] Posición guardada desde personaje anterior: {child.position}");
                    break;
                }
            }
        }

        // Desactivar todos los personajes
        foreach (var personaje in characterReferences)
        {
            if (personaje != null)
                personaje.SetActive(false);
        }

        // Si el tipo no es None, activar el nuevo personaje
        if (selector.PlayerType != PlayerType.None)
        {
            ChangeSkin(selector.PlayerType);

            if (_selectedCharacter != null)
            {
                Vector3 spawnPosition = PlayerTransitionData.GetPosition();

                _selectedCharacter.SetActive(true);

                foreach (var child in _selectedCharacter.GetComponentsInChildren<Transform>(true))
                {
                    if (child.CompareTag("Player"))
                    {
                        child.position = spawnPosition;
                        Debug.Log($"[SkinChanger] Posicionado nuevo personaje en: {spawnPosition}");
                        break;
                    }
                }
            }
        }
    }

    private void GetPosition()
    {
        
        
        Vector3 spawnPosition = PlayerTransitionData.GetPosition();

        if (_selectedCharacter.CompareTag("Player"))
        {
            _selectedCharacter.transform.position = spawnPosition;
            Debug.Log($"[SkinChanger] Posicionado personaje principal en: {spawnPosition}");
            return;
        }

        foreach (var child in _selectedCharacter.GetComponentsInChildren<Transform>(true))
        {
            if (child.CompareTag("Player"))
            {
                child.position = spawnPosition;
                Debug.Log($"[SkinChanger] Posicionado hijo con tag Player en: {spawnPosition}");
                return;
            }
        }

        Debug.LogWarning("[SkinChanger] No se encontró objeto con tag Player");
    }

    private void ChangeSkin(PlayerType type)
    {
        int index = type switch
        {
            PlayerType.Sacerdote   => 0,
            PlayerType.Raton       => 1,
            PlayerType.Ixquic      => 2,
            PlayerType.Hunampu     => 3,
            PlayerType.None        => -1,
            _ => -1

        };

        if (index < 0 || index >= characterReferences.Count)
        {
            Debug.LogError($"No hay personaje asignado para {type}");
            return;
        }

        _selectedCharacter = characterReferences[index];
        Debug.Log($"[SkinChanger] Activado personaje: {_selectedCharacter.name}");
    }
    #endregion
}
