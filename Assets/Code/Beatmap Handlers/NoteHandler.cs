using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteHandler : MonoBehaviour
{
    public BeatmapPlayer _beatmapPlayer;
    public NoteInfo noteInfo;
    
    public float distToTapTarget;
    float timeAlive = 0f;

    SpriteRenderer spriteRenderer;
    LineRenderer lineRenderer;

    [SerializeField] Sprite[] sprites;
    [SerializeField] GameObject leafParticle;

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
            spriteRenderer.sprite = sprites[0];
            name = "Hold Note";
        }
        else if (noteInfo.type == noteType.tapNote)
        {
            spriteRenderer.sprite = sprites[1];
            name = "Normal Note";
        }
        if (noteInfo.type == noteType.endHoldNote)
        {
            spriteRenderer.sprite = sprites[2];
            name = "Hold End Note";
        }
           
        yield return new WaitForSeconds(delayMS / 1000);

        float startY = transform.position.y;

        while (true)
        {
            if (this == null) break;

            if (!holdingNote) timeAlive += Time.deltaTime;


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


            if (distToTapTarget <= _beatmapPlayer.noteTapDistance) tappable = true;

            if (holdingNote)
            {
                transform.position = new Vector3(
                    transform.position.x,
                    _beatmapPlayer.tapTarget.position.y,
                    transform.position.z
                );
            }

            yield return null;
        }
    }

    public void SpawnParticle()
    {
        GameObject particle = Instantiate(leafParticle);
        particle.transform.position = transform.position;
    }
    void DeleteNote()
    {
        _beatmapPlayer.missedNotes++;
        _beatmapPlayer.missedNoteCounter.text = _beatmapPlayer.missedNotes.ToString();
        _beatmapPlayer.columns[noteInfo.columnIndex].notes.Remove(this);
        _beatmapPlayer.errorSound.PlayOneShot(_beatmapPlayer.errorSound.clip); 
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

