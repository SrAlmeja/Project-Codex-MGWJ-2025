using UnityEngine;

public static class PlayerTransitionData
{
    /// <summary>
    /// Última posición registrada del jugador antes de cambiar de escena.
    /// </summary>
    public static Vector3 LastPlayerPosition = Vector3.zero;

    /// <summary>
    /// Guarda la posición actual del jugador.
    /// </summary>
    public static void SavePosition(Vector3 position)
    {
        LastPlayerPosition = position;
        Debug.Log($"[PlayerTransitionData] Posición guardada: {position}");
    }

    /// <summary>
    /// Devuelve la posición guardada.
    /// </summary>
    public static Vector3 GetPosition()
    {
        return LastPlayerPosition;
    }

    public static Vector3 ResetPosition()
    {
        return LastPlayerPosition = Vector3.zero;
    }
}
