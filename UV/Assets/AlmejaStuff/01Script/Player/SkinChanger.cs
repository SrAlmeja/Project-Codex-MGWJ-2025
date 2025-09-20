using System.Collections.Generic;
using UnityEngine;

public class SkinChanger : PersistentSingleton<SkinChanger>
{
    #region Variables

    [Header("Characters Prefabs")]
    [SerializeField] private List<GameObject> characterPrefabs; // Prefabs originales
    
    private List<GameObject> _characters = new(); // Instancias activas
    private GameObject _selectedCharacter;
    
    #endregion

    #region Unity Functions

    private void OnEnable()
    {
        TagSelector.OnTagSelectorReady += HandleTagSelector;
    }

    private void OnDisable()
    {
        TagSelector.OnTagSelectorReady -= HandleTagSelector;
    }
    private void Awake()
    {
        base.Awake();
    }
    
    private void Start()
    {
        InstantiateCharacters();
    }
    #endregion
    
    #region SkinChanger Functions
    /// <summary>
    /// Instancia todos los personajes y los desactiva.
    /// </summary>
    private void InstantiateCharacters()
    {
        foreach (var c in _characters)
        {
            if (c != null) Destroy(c);
        }
        _characters.Clear();

        foreach (var prefab in characterPrefabs)
        {
            if (prefab == null) continue;

            var instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            instance.SetActive(false);
            _characters.Add(instance);
        }
    }
    private void HandleTagSelector(TagSelector selector)
    {
        Debug.Log($"[SkinChanger] Recibido tipo de jugador: {selector.PlayerType}");
        
        
        foreach (var personaje in _characters)
        {
            if (personaje != null)
                personaje.SetActive(false);
        }

        ChangeSkin(selector.PlayerType);

        if (_selectedCharacter != null)
        {
            _selectedCharacter.SetActive(true);

            Vector3 spawnPosition = PlayerTransitionData.GetPosition();

            foreach (var child in _selectedCharacter.GetComponentsInChildren<Transform>(true))
            {
                if (child.CompareTag("Player"))
                {
                    child.position = spawnPosition;
                    Debug.Log($"[SkinChanger] Posicionado MainPlayer en: {spawnPosition}");
                    break;
                }
            }
        }
    }
    private void ChangeSkin(PlayerType type)
    {
        int index = type switch
        {
            PlayerType.Sacerdote   => 0,
            PlayerType.Raton       => 1,
            PlayerType.Ixquic      => 2,
            PlayerType.Hunampu     => 3,
            PlayerType.Ixbalanque  => 4,
            _ => 0
        };

        if (index < 0 || index >= _characters.Count)
        {
            Debug.LogError($"No hay personaje asignado para {type}");
            return;
        }

        for (int i = 0; i < _characters.Count; i++)
        {
            if (_characters[i] == null) continue;

            bool esSeleccionado = i == index;
            _characters[i].SetActive(esSeleccionado);

            if (esSeleccionado)
                _selectedCharacter = _characters[i];
        }

        Debug.Log($"[SkinChanger] Activado personaje: {_selectedCharacter.name}");
    }
    #endregion
}
