using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraZ : MonoBehaviour
{
    void Update()
    {
        if (transform.position.z < -10 || transform.position.z > -10)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        }
    }
}
