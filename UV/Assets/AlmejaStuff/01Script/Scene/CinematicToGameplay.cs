using UnityEngine;
using UnityEngine.Video;
using Systems.SceneManagement;

public class CinematicToGameplay : MonoBehaviour
{
    [Header("Scene Group Name")]
    [SerializeField] private string sceneGroupName;

    [SerializeField] private VideoPlayer videoPlayer;

    private bool _videoFinished = false;

    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void Update()
    {
        // Si el video no ha terminado, y se presiona Enter (Return), salta al siguiente nivel
        if (!_videoFinished && Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Video interrumpido con Enter");
            SkipVideo();
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (_videoFinished) return;

        Debug.Log("Video finalizado normalmente");
        _videoFinished = true;
        NextLevel();
    }

    private void SkipVideo()
    {
        videoPlayer.Stop();
        _videoFinished = true;
        NextLevel();
    }
    
    private void NextLevel()
    {
        SceneLoaderV2.Instance.LoadSceneGroupByName(sceneGroupName);
    }

}
