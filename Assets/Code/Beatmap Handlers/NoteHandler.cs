using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteHandler : MonoBehaviour
{
    public BeatmapManager beatmapManager;
    float timeAlive = 0f;

    private void Start()
    {
        StartCoroutine(moveNote());
    }

    IEnumerator moveNote()
    {
        float startY = transform.position.y;

        while (true)
        {
            timeAlive += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(startY, -4f, timeAlive / beatmapManager.noteOffset), transform.position.z);
            if (timeAlive >= beatmapManager.noteOffset)
            {
                Destroy(gameObject);
            }
            yield return null;
        }
    }
}

