using UnityEngine;

[CreateAssetMenu(menuName = "Globals/Global Bool Flag", fileName = "GlobalBoolFlag_")]
public class GlobalBoolFlag : ScriptableObject
{
    [Tooltip("Valor actual del flag.")]
    [SerializeField] private bool value;

    [Header("Persistencia (opcional)")]
    [Tooltip("Si está activo, guarda/carga en PlayerPrefs.")]
    [SerializeField] private bool persistBetweenSessions = false;

    [Tooltip("Clave usada en PlayerPrefs (por defecto, el nombre del asset).")]
    [SerializeField] private string playerPrefsKey = "";

    public bool Value => value;

    private void OnEnable()
    {
        // Carga desde PlayerPrefs si se configuró persistencia
        if (persistBetweenSessions && !string.IsNullOrEmpty(playerPrefsKey) && PlayerPrefs.HasKey(playerPrefsKey))
        {
            value = PlayerPrefs.GetInt(playerPrefsKey, 0) != 0;
        }

#if UNITY_EDITOR
        // Si no se definió clave, usa el nombre del asset
        if (string.IsNullOrEmpty(playerPrefsKey))
            playerPrefsKey = name;
#endif
    }

    public void Set(bool v)
    {
        if (value == v) return;
        value = v;

        if (persistBetweenSessions && !string.IsNullOrEmpty(playerPrefsKey))
        {
            PlayerPrefs.SetInt(playerPrefsKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    [ContextMenu("Set TRUE")]
    public void SetTrue() => Set(true);

    [ContextMenu("Clear (FALSE)")]
    public void Clear() => Set(false);
}
