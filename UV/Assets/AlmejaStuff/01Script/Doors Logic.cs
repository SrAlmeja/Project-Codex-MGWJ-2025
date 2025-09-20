using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Eflatun.SceneReference;
using Systems.SceneManagement;
using UnityEngine.InputSystem;

public class DoorsLogic : MonoBehaviour
{
    #region Variables

    [Header("Scene Transition")]
    [SerializeField] private InputActionReference interact;
    [SerializeField] private SceneReference sceneToEnable;
    [SerializeField] private SceneReference sceneToDisable;

    [SerializeField] private bool noDoor;

    [Header("NextLevel")]
    [SerializeField] private string sceneGroupName;

    [Header("Flag a setear cuando el objeto quede activo")]
    public GlobalBoolFlag completionFlag; // ⟵ Reemplazo de SOBoolean nextLevelReady

    private bool _isOnArea;
    private GameObject _player;

    #endregion

    #region Unity Functions

    private void OnEnable()
    {
        interact.action.started += Interact;
    }

    private void OnDisable()
    {
        interact.action.started -= Interact;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        _player = other.gameObject;

        // 1. Si hay un nombre de escena válido Y el flag global está en TRUE -> ir al siguiente nivel
        if (!string.IsNullOrEmpty(sceneGroupName) && completionFlag != null && completionFlag.Value)
        {
            Debug.Log("Se llamó NextLevel");
            NextLevel(sceneGroupName);
            return;
        }

        // 2. Si NO hay puerta -> ejecutar acción automáticamente
        if (noDoor)
        {
            Debug.Log("No hay puerta");
            TurnOnScene();
            TurnOffScene();
            return;
        }

        // 3. Si HAY puerta -> esperar interacción
        Debug.Log("Hay puerta");
        _isOnArea = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _isOnArea = false;
    }

    #endregion

    private void Interact(InputAction.CallbackContext context)
    {
        if (!_isOnArea) return;
        
        if (_player != null) PlayerTransitionData.SavePosition(_player.transform.position);

        TurnOnScene();
        TurnOffScene();
    }

    #region Scene Logic

    private void TurnOnScene()
    {
        foreach (var obj in GetSceneElements(sceneToEnable))
            obj.SetActive(true);

        ViCamFollow camFollow = FindObjectOfType<ViCamFollow>();
        if (camFollow != null)
            camFollow.FindPlayer();
        else
            Debug.LogWarning("[DoorsLogic] No se encontró ViCamFollow en escena.");
    }

    private void TurnOffScene()
    {
        foreach (var obj in GetSceneElements(sceneToDisable))
        {
            if (obj != null && obj.activeSelf)
                obj.SetActive(false);
        }
    }

    public void NextLevel(string sceneGroupName)
    {
        SceneLoaderV2.Instance.LoadSceneGroupByName(sceneGroupName);
    }

    private List<GameObject> GetSceneElements(SceneReference sceneRef)
    {
        List<GameObject> result = new List<GameObject>();
        string targetName = sceneRef.Name;

        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
            if (!scene.isLoaded || scene.name != targetName) continue;

            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var child in root.GetComponentsInChildren<Transform>(true))
                {
                    if (child.CompareTag("SceneElements"))
                        result.Add(child.gameObject);
                }
            }
        }

        return result;
    }

    #endregion
}

