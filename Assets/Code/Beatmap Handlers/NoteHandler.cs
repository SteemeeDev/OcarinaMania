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

    public bool tappable = false;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }


    public IEnumerator moveNote(float delayMS)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (noteInfo.type == 128)
        {
            spriteRenderer.color = Color.yellow;
            name = "Hold Note";
        }
        else if (noteInfo.type <= 64) // We treat any other note type as just a normal note 
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

            if (endHoldPoint != null) updateLine();

            if (timeAlive >= _beatmapPlayer.noteOffset * 1.1f)
            {
                reachedEnd = true;
                _beatmapPlayer.columns[noteInfo.columnIndex].notes.Remove(this);
                if (endHoldPoint == null)
                {
                    _beatmapPlayer.points--;
                    _beatmapPlayer.pointCounter.text = _beatmapPlayer.points.ToString();
                    Destroy(gameObject);
                    break;
                }
            }
            else
            {
                if (_beatmapPlayer == null) Debug.LogWarning("GAMIGN!");
                if (reachedEnd == false)
                {
                    transform.position = new Vector3(transform.position.x, Mathf.Lerp(startY, _beatmapPlayer.tapTarget.position.y, timeAlive / _beatmapPlayer.noteOffset), transform.position.z);
                }
                else
                {
                    transform.position -= new Vector3(0, 1, 0) * Time.deltaTime;
                }
            }
            if (Vector2.Distance(new Vector2(0, transform.position.y), new Vector2(0, _beatmapPlayer.tapTarget.position.y)) <= _beatmapPlayer.noteTapDistance)
            {
               // spriteRenderer.color = Color.white;
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

