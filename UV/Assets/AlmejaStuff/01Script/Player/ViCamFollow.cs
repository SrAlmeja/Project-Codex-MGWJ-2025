using System.Collections;
using Cinemachine;
using UnityEngine;

public class ViCamFollow : MonoBehaviour
{
    [SerializeField] private GameObject viCam;
    private GameObject player;
    private CinemachineVirtualCamera _virtualCam;

    public static ViCamFollow Instance { get; private set; }
    
    private void OnEnable()
    {
        FindPlayer();
    }
    public void FindPlayer()
    {
        if (_virtualCam == null) _virtualCam = viCam.GetComponent<CinemachineVirtualCamera>();

        StartCoroutine(WaitForPlayerAndAssingCam());
    }
    
    private IEnumerator WaitForPlayerAndAssingCam()
    {
        player = null;
        while (player == null || !player.activeInHierarchy)
        {
            //print("Waiting for player");
            player = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }
        
        _virtualCam.Follow = player.transform;
        //print("Player found" + player.name);
    }

}
