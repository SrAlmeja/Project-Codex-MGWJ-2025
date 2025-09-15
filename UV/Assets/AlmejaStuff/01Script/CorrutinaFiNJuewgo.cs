using System.Collections;
using System.Collections.Generic;
using Systems.SceneManagement;
using UnityEngine;

public class CorrutinaFiNJuewgo : MonoBehaviour
{
    public string sceneGroupName;

    private void Start()
    {
        StartCoroutine(WaitTenSeconds());
    }

    private IEnumerator WaitTenSeconds()
    {
        Debug.Log("Esperando 10 segundos...");
        yield return new WaitForSeconds(10f);
        SceneLoaderV2.Instance.LoadSceneGroupByName(sceneGroupName);
    }
}

