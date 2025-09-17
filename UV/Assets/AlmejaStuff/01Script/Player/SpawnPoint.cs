using UnityEngine;
using System;

public class SpawnPoint : MonoBehaviour
{
    [Header("SpawnPoint")]
    [SerializeField] private string spawnID = "default";
    public string SpawnID => spawnID;

    public static event Action<string, Vector3> OnSpawnPointReady;

    private void Start()
    {
        Debug.Log($"[SpawnPoint] Emitiendo ID: {spawnID} en posición: {transform.position}");
        OnSpawnPointReady?.Invoke(spawnID, transform.position);
    }
}

