using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteHandler : MonoBehaviour
{
    float timeAlive = 0f;

    private void Update()
    {
        timeAlive += Time.deltaTime;

        if (timeAlive >= 0.15f)
        {
            Destroy(gameObject);
        }
    }
}

