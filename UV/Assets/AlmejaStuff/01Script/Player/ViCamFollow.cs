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
        player = GameObject.FindGameObjectWithTag("Player");
        _playerTransform = player.transform;
        _virtualCam = viCam.GetComponent<CinemachineVirtualCamera>();

        _virtualCam.Follow = _playerTransform;
    }

}
