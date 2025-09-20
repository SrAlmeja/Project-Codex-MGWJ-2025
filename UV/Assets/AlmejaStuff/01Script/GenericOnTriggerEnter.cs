using UnityEngine;

public class GenericOnTriggerEnter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var listener = GetComponent<WinConditionListener>();
            if (listener != null)
                listener.TriggerCondition();

            gameObject.SetActive(false);
        }
    }
}
