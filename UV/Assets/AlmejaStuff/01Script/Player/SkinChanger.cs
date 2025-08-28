using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinChanger : PersistentSingleton<SkinChanger>
{
    #region Variables

    [Header("Characters")]
    [SerializeField] private List<GameObject> characters;
    
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
        PrefabSpawner();
        TagReader();
    }

    #endregion
    
    #region SkinChanger Functions

    private void PrefabSpawner()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i] = Instantiate(characters[i]);
            characters[i].SetActive(false);
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
    
    private void ChangeSkin(PlayerType type)
    {
        switch (type)
        {
            case PlayerType.Sacerdote:
                _selectedCharacter = characters[0];
                break;
            case PlayerType.Raton:
                _selectedCharacter = characters[1];
                break;
            case PlayerType.Ixquic:
                _selectedCharacter = characters[2];
                break;
            case PlayerType.Hunampu:
                _selectedCharacter = characters[3];
                break;
            case PlayerType.Ixbalanque:
                _selectedCharacter = characters[4];
                break;
        }

        if (_previousCharacter != null && _previousCharacter != _selectedCharacter)
            _previousCharacter.SetActive(false);

        _selectedCharacter.SetActive(true);
    }
    #endregion
}
