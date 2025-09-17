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
    
    private Dictionary<string, Vector3> _spawnPoints = new();
    private string _selectedSpawnID = "default"; // Se puede definir desde otro sistemaa ver 

    #endregion

    #region Unity Functions

    private void OnEnable()
    {
        TagSelector.OnTagSelectorReady += HandleTagSelector;
        SpawnPoint.OnSpawnPointReady += SetSpawnPosition;

    }
    private void OnDisable()
    {
        TagSelector.OnTagSelectorReady -= HandleTagSelector;
        SpawnPoint.OnSpawnPointReady -= SetSpawnPosition;
    }
    
    private void Awake()
    {
        base.Awake();
    }
    
    private void Start()
    {
        _spawnPosition = Vector3.zero;
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
    
    public void SetSpawnPosition(Vector3 position)
    {
        _spawnPosition = position;
        Debug.Log($"[SkinChanger] Posición de spawn recibida: {_spawnPosition}");
    }


    private void HandleTagSelector(TagSelector selector)
    {
        Debug.Log($"[SkinChanger] Recibido tipo de jugador: {selector.PlayerType}");
        _tagSelector = selector;

        // Apagar todos los prefabs
        foreach (var personaje in _characters)
        {
            if (personaje != null)
                personaje.SetActive(false);
        }

        // Activar el personaje correcto
        ChangeSkin(selector.PlayerType);

        // Mover el hijo "Player" del personaje activo
        if (_selectedCharacter != null)
        {
            _selectedCharacter.SetActive(true);

            foreach (var child in _selectedCharacter.GetComponentsInChildren<Transform>(true))
            {
                if (child.CompareTag("Player"))
                {
                    child.position = _spawnPosition;
                    Debug.Log($"[SkinChanger] Posicionado MainPlayer en: {child.position}");
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
