using UnityEngine;
using UnityEngine.Events;
using Systems.SceneManagement;
public class CinematicEndMark : MonoBehaviour
{
    [Header("Opción: reproducir segunda animación")]
    [SerializeField] private Animator animator;
    [SerializeField] private string nextAnimationTrigger;

    [Header("Opción: cambiar de escena")]
    [SerializeField] private bool triggerNextLevel;
    [SerializeField] private string sceneGroupName;

    [Header("Evento personalizado opcional")]
    [SerializeField] private UnityEvent onCinematicEnd;

    public void EndCinematic()
    {
        Debug.Log("[CinematicEndController] Cinemática finalizada");

        onCinematicEnd?.Invoke();

        if (animator != null && !string.IsNullOrEmpty(nextAnimationTrigger))
        {
            animator.SetTrigger(nextAnimationTrigger);
            Debug.Log("[CinematicEndController] Segunda animación activada");
        }

        if (triggerNextLevel && !string.IsNullOrEmpty(sceneGroupName))
        {
            SceneLoaderV2.Instance.LoadSceneGroupByName(sceneGroupName);
            Debug.Log("[CinematicEndController] Cambio de escena ejecutado");
        }
    }


}
