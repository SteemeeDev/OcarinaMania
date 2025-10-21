using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteHandler : MonoBehaviour
{
    public BeatmapManager beatmapManager;
    public NoteInfo noteInfo;
    float timeAlive = 0f;

    SpriteRenderer spriteRenderer;

    public IEnumerator moveNote(float delayMS)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (noteInfo.type == 128)
        {
            spriteRenderer.color = Color.yellow;
            name = "Hold Note";
        }
        else if (noteInfo.type == 1)
        {
            spriteRenderer.color = Color.blue;
            name = "Normal Note";
        }

        if (delayMS > 0)
        {
            spriteRenderer.color = Color.yellow * 0.5f;
            name = "Hold End Note";
            Debug.Log("HOLD END NOTE SPAWNED");
        }

        yield return new WaitForSeconds(delayMS / 1000);

        float startY = transform.position.y;

        while (true)
        {
            timeAlive += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(startY, -4f, timeAlive / beatmapManager.noteOffset), transform.position.z);
            if (timeAlive >= beatmapManager.noteOffset)
            {
                Destroy(gameObject);
                break;
            }
            yield return null;
        }
    }
}

