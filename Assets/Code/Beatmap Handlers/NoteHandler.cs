using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteHandler : MonoBehaviour
{
    public BeatmapManager beatmapManager;
    public NoteInfo noteInfo;
    float timeAlive = 0f;

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (noteInfo.type == 128)
        {
            Debug.Log("HOLD NOTE!");
            spriteRenderer.color = Color.yellow;
        }else if (noteInfo.type == 1)
        {
            spriteRenderer.color = Color.blue;
        }

        Debug.Log(noteInfo.type);

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

