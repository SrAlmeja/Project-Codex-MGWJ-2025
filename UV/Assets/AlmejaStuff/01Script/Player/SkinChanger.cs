using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinChanger : MonoBehaviour
{
    [SerializeField] private List<GameObject> characters;
    [SerializeField] private GameObject SelectedCharacter;
    private TagSelector _tagSelector;
    
    private void Start()
    {
        SelectedCharacter = characters[0];
        TagSelector selector = FindObjectOfType<TagSelector>();
        
        if (selector == null)
        {
            Debug.LogError("No se ha encontrado el TagSelector");
            return;
        }
        
        ChangeSkin(selector.PlayerType);
        Instantiate(SelectedCharacter);
    }

    private void ChangeSkin(PlayerType type)
    {
        switch (type)
        {
            case PlayerType.Sacerdote:
                SelectedCharacter = characters[0];
                break;
            case PlayerType.Raton:
                SelectedCharacter = characters[1];
                break;
            case PlayerType.Ixquic:
                SelectedCharacter = characters[2];
                break;
            case PlayerType.Hunampu:
                SelectedCharacter = characters[3];
                break;
            case PlayerType.Ixbalanque:
                SelectedCharacter = characters[4];
                break;
        }
    }
}
