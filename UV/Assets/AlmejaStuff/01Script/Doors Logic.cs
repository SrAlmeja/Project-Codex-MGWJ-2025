using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Eflatun.SceneReference;
using UnityEngine.InputSystem;

public class DoorsLogic : MonoBehaviour
{
    #region Variables

    [Header("Scene Transition")]
    [SerializeField] private InputActionReference interact;
    [SerializeField] private SceneReference sceneToEnable;
    [SerializeField] private SceneReference sceneToDisable;


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
        if (other.CompareTag("Player"))
        {
            _isOnArea = true;
            _player = other.gameObject;    
        }
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
        {
            camFollow.FindPlayer();
            //Debug.Log("[DoorsLogic] Cámara encontrada y configurada.");
        }
        else
        {
            Debug.LogWarning("[DoorsLogic] No se encontró ViCamFollow en escena.");
        }
    }
    private void TurnOffScene()
    {
        foreach (var obj in GetSceneElements(sceneToDisable))
        {
            if (obj != null && obj.activeSelf)
                obj.SetActive(false);
        }

        //Debug.Log("[DoorsLogic] Objetos con tag 'SceneElements' desactivados en la escena a apagar.");
        //SkinChanger.Instance.TagReader(_player.transform);
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