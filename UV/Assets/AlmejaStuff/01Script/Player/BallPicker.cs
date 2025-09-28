using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BallPicker : MonoBehaviour
{
    [SerializeField] private SOBoolean unArmed;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            unArmed.Value = false;
            Destroy(gameObject);
        }
    }
}
