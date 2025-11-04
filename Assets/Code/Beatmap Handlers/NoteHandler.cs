using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteHandler : MonoBehaviour
{
    public BeatmapPlayer _beatmapPlayer;

    public NoteInfo noteInfo;
    float timeAlive = 0f;

    SpriteRenderer spriteRenderer;

    public Transform endHoldPoint;
    LineRenderer lineRenderer;
    bool reachedEnd = false;
    bool holdEndNote = false;
    public bool tappable = false;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }


    public IEnumerator moveNote(float delayMS)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (noteInfo.type == noteType.holdNote)
        {
            spriteRenderer.color = Color.yellow;
            name = "Hold Note";
            Debug.Log("AHHHH");
        }
        else if (noteInfo.type == noteType.tapNote)
        {
            spriteRenderer.color = Color.blue;
            name = "Normal Note";
        }
        if (noteInfo.type == noteType.endHoldNote)
        {
            spriteRenderer.color = Color.yellow * 0.5f;
            name = "Hold End Note";
            holdEndNote = true;
        }
           
        yield return new WaitForSeconds(delayMS / 1000);

        float startY = transform.position.y;
        float distToTapTarget;

        while (true)
        {
            if (this == null) break;

            timeAlive += Time.deltaTime;

            distToTapTarget = Vector2.Distance(new Vector2(0, transform.position.y), new Vector2(0, _beatmapPlayer.tapTarget.position.y));

            if (endHoldPoint != null) updateLine();

            if (timeAlive >= _beatmapPlayer.noteOffset)
            {
                if (noteInfo.type == noteType.tapNote || noteInfo.type == noteType.endHoldNote)
                {
                    if (noteInfo.type == noteType.endHoldNote) reachedEnd = true;

                    transform.position = new Vector3(
                        transform.position.x, 
                        Mathf.Lerp(startY, _beatmapPlayer.secondTapTarget.position.y, timeAlive / (_beatmapPlayer.noteOffset * 2f)), 
                        transform.position.z
                    );

                    if (timeAlive > _beatmapPlayer.noteOffset + _beatmapPlayer.noteOffset * 0.2f)
                    {
                        _beatmapPlayer.points--;
                        _beatmapPlayer.pointCounter.text = _beatmapPlayer.points.ToString();
                        _beatmapPlayer.columns[noteInfo.columnIndex].notes.Remove(this);
                        Destroy(gameObject);
                        break;
                    }
                }
            }
            else
            {
                if (_beatmapPlayer == null) Debug.LogWarning("GAMIGN!");
                    transform.position = new Vector3(transform.position.x, Mathf.Lerp(startY, _beatmapPlayer.tapTarget.position.y, timeAlive / _beatmapPlayer.noteOffset), transform.position.z);
            }
            if (distToTapTarget <= _beatmapPlayer.noteTapDistance)
            {
                spriteRenderer.color = Color.white;

                tappable = true;
            }
            
            yield return null;
        }
    }

    void updateLine()
    {
        if (endHoldPoint != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1,endHoldPoint.position);
        }
    }
}

