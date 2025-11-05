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

    LineRenderer lineRenderer;
    bool reachedEnd = false;
    public bool tappable = false;
    [Header("Hold note specific")]
    public NoteHandler holdEndNote;
    public bool holdingNote;

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
        }
           
        yield return new WaitForSeconds(delayMS / 1000);

        float startY = transform.position.y;
        float distToTapTarget;

        while (true)
        {
            if (this == null) break;

            if (!holdingNote) timeAlive += Time.deltaTime;
            else if (holdingNote)
            {
                transform.position = new Vector3(
                    transform.position.x,
                    _beatmapPlayer.tapTarget.position.y,
                    transform.position.z
                );
            }


            distToTapTarget = Vector2.Distance(new Vector2(0, transform.position.y), new Vector2(0, _beatmapPlayer.tapTarget.position.y));

            if (holdEndNote != null) updateLine();
            
            if (noteInfo.type == noteType.holdNote && holdEndNote == null)
            {
                DeleteNote();
                break;
            }


            if (timeAlive >= _beatmapPlayer.noteOffset)
            {
                if (noteInfo.type == noteType.endHoldNote) reachedEnd = true;

                if (!holdingNote)
                {
                    transform.position = new Vector3(
                        transform.position.x,
                        Mathf.Lerp(startY, _beatmapPlayer.secondTapTarget.position.y, timeAlive / (_beatmapPlayer.noteOffset * 2f)),
                        transform.position.z
                    );
                }


                if (timeAlive > _beatmapPlayer.noteOffset + _beatmapPlayer.noteOffset * 0.2f && !holdingNote)
                {
                    if (noteInfo.type == noteType.holdNote && holdEndNote != null) 
                        holdEndNote.DeleteNote();
                    DeleteNote();
                    break;
                }
            }
            else
            {
                if (_beatmapPlayer == null) Debug.LogWarning("GAMIGN!");

                transform.position = new Vector3(
                    transform.position.x,
                    Mathf.Lerp(startY, _beatmapPlayer.tapTarget.position.y, timeAlive / _beatmapPlayer.noteOffset), 
                    transform.position.z
                );
            }
            if (distToTapTarget <= _beatmapPlayer.noteTapDistance)
            {
               // spriteRenderer.color = Color.white;

                tappable = true;
            }
            
            yield return null;
        }
    }

    void DeleteNote()
    {
        _beatmapPlayer.missedNotes++;
        _beatmapPlayer.missedNoteCounter.text = _beatmapPlayer.missedNotes.ToString();
        _beatmapPlayer.columns[noteInfo.columnIndex].notes.Remove(this);
        Destroy(gameObject);
    }

    void updateLine()
    {
        if (holdEndNote != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, holdEndNote.transform.position);
        }
    }
}

