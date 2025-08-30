using UnityEngine;

public class TagSelector : MonoBehaviour
{
    [Header("Tag Selector")] [SerializeField] public PlayerType PlayerType;
}

public enum PlayerType
{
    Sacerdote,
    Raton,
    Ixquic,
    Hunampu,
    Ixbalanque
}
