using System.Collections;
using Cinemachine;
using UnityEngine;

public class ViCamFollow : MonoBehaviour
{
    [SerializeField] private GameObject viCam;
    private GameObject player;
    private Transform _playerTransform;
    private CinemachineVirtualCamera _virtualCam;


    private void Start()
    {
        _virtualCam = viCam.GetComponent<CinemachineVirtualCamera>();
        StartCoroutine(WaitForPlayerAndAssingCam());
    }
    
    private IEnumerator WaitForPlayerAndAssingCam()
    {
        player = null;

        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }
        _playerTransform = player.transform;
        _virtualCam.Follow = _playerTransform;
    }

}
