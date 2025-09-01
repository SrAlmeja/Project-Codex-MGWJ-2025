using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinChanger : PersistentSingleton<SkinChanger>
{
    #region Variables

    [Header("Characters Prefabs")]
    [SerializeField] private List<GameObject> characterPrefabs; // Prefabs originales
    
    private List<GameObject> _characters = new(); // Instancias activas
    private GameObject _selectedCharacter;
    private GameObject _previousCharacter;
    private Vector3 _spawnPosition;
    private TagSelector _tagSelector;

    #endregion

    #region Unity Functions
    private void Awake()
    {
        base.Awake();
    }
    
    private void Start()
    {
        _spawnPosition = Vector3.zero;
        InstantiateCharacters();
        TagReader();
    }

    #endregion
    
    #region SkinChanger Functions

    /// <summary>
    /// Instancia todos los personajes y los desactiva.
    /// </summary>
    private void InstantiateCharacters()
    {
        // Limpiar por si ya había
        foreach (var c in _characters)
        {
            if (c != null) Destroy(c);
        }
        _characters.Clear();

        foreach (var prefab in characterPrefabs)
        {
            if (prefab == null) continue;

            var instance = Instantiate(prefab);
            instance.SetActive(false);
            _characters.Add(instance);
        }
    }
    
    public void TagReader(Transform playerPosition = null)
    {
        print("TagReader llamado");
        TagSelector selector = FindObjectOfType<TagSelector>();
        if (selector == null)
        {
            Debug.LogError("No se ha encontrado el TagSelector");
            return;
        }

        if (_selectedCharacter != null)
            _previousCharacter = _selectedCharacter;

        if (playerPosition != null)
            _spawnPosition = playerPosition.position;

        ChangeSkin(selector.PlayerType);

        // Mover el hijo con el tag "Player" dentro del nuevo personaje
        foreach (var child in _selectedCharacter.GetComponentsInChildren<Transform>(true))
        {
            if (child.CompareTag("Player"))
            {
                child.position = _spawnPosition;
                break;
            }
        }
    }

    /// <summary>
    /// Elimina y reinstancia todos los personajes.
    /// </summary>
    public void ReloadCharacters()
    {
        InstantiateCharacters();
        TagReader();
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

        _selectedCharacter = _characters[index];

        if (_previousCharacter != null && _previousCharacter != _selectedCharacter)
            _previousCharacter.SetActive(false);

        _selectedCharacter.SetActive(true);
    }
    #endregion
}
