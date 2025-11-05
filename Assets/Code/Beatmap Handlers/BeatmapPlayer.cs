using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

public class BeatmapPlayer : MonoBehaviour
{
    [SerializeField] BeatmapManager manager; // The beatmap parser
    public float points; // Placeholder for player scores
    public TMP_Text pointCounter; // Text object to show scores
    public int missedNotes;
    public TMP_Text missedNoteCounter;
    [SerializeField] SpriteRenderer[] keys; // The 4 pressable key objects
    [SerializeField] KeyCode[] keyBinds;
    [SerializeField] GameObject notePrefab;
    public Column[] columns;
    
    AudioSource audioSource;

    [SerializeField] int mapIndex; // Index of the map we want to play
    [SerializeField] Beatmap currentBeatmap;

    [Header("Gameplay settings")]
    /// <summary>
    /// Time in seconds for a note to travel from spawn to hit position
    /// </summary>
    public float noteOffset = 0.5f;

    /// <summary>
    ///  Distance to the tap target (both ways), where tapping the note is valid
    /// </summary>
    public float noteTapDistance = 0.2f; 

    public Transform tapTarget;
    public Transform secondTapTarget;

    Coroutine playerRoutine;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (playerRoutine != null) StopCoroutine(playerRoutine);
            currentBeatmap = manager.beatMaps[mapIndex];
            playerRoutine = StartCoroutine(PlayBeatmap(mapIndex));
        }

        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keyBinds[i]))
            {
                if (columns[i].notes.Count > 0 && columns[i].notes[0].tappable)
                {
                    NoteHandler currentNote = columns[i].notes[0];
                    if (currentNote.noteInfo.type == noteType.tapNote)
                    {
                        DeleteNote(i, 0);
                    }

                    else if (currentNote.noteInfo.type == noteType.holdNote)
                    {
                        if (currentNote.tappable)
                        {
                            currentNote.holdingNote = true;
                        }
                    }
                }
                keys[i].transform.localScale = Vector2.one * 1.1f;
            }
            else if (Input.GetKeyUp(keyBinds[i]))
            {
                keys[i].transform.localScale = Vector2.one;

                if (columns[i].notes.Count == 0) continue;

                NoteHandler currentNote = columns[i].notes[0];
                
                if (columns[i].notes.Count > 1 && columns[i].notes[1].tappable)
                {
                    if (columns[i].notes[1].noteInfo.type == noteType.endHoldNote && currentNote.holdingNote)
                    {
                        DeleteNote(i, 1);
                        DeleteNote(i, 0);
                    }
                }

                if (currentNote.noteInfo.type == noteType.holdNote)
                {
                    currentNote.holdingNote = false;
                }
            }
        }
    }

    void DeleteNote(int columnIndex, int noteIndex)
    {
        GameObject temp = columns[columnIndex].notes[noteIndex].gameObject;
        temp.GetComponent<NoteHandler>().StopAllCoroutines();
        columns[columnIndex].notes.RemoveAt(noteIndex);
        Destroy(temp);
        points++;
        pointCounter.text = points.ToString();
    }

    public IEnumerator PlayBeatmap(int beatmapIndex)
    {
        for (int i = 0; i < columns.Length; i++)
        {
            for (int j = 0; j < columns[i].notes.Count - 1; i++)
            {
                Destroy(columns[i].notes[j].gameObject);
                columns[i].notes.RemoveAt(j);
            }
        }

        currentBeatmap = manager.beatMaps[beatmapIndex];

        audioSource.clip = Resources.Load<AudioClip>(
            "Audios/" + Path.GetFileNameWithoutExtension(Application.dataPath + "/Beatmaps/Resources/Audios/" + currentBeatmap.musicFile)
        );

        float beatmapLength = currentBeatmap.notes[currentBeatmap.notes.Count - 1].time;
        float timeElapsed = 0f;

        int noteIndex = 0;

        double audioPlayTime = AudioSettings.dspTime + currentBeatmap.audioLeadIn / 1000 + noteOffset;

        audioSource.PlayScheduled(audioPlayTime);

        while (timeElapsed < beatmapLength)
        {
            if (AudioSettings.dspTime >= audioPlayTime - noteOffset)
            {
                timeElapsed += Time.deltaTime;

                while (timeElapsed * 1000 > currentBeatmap.notes[noteIndex].time)
                {
                    SpawnNote(noteIndex, timeElapsed);
                    noteIndex++;
                }
            }
            yield return null;
        }
    }

    void SpawnNote(int noteIndex, float elapsed)
    {
        GameObject noteObj = Instantiate(notePrefab);
        NoteInfo currentNoteInfo = currentBeatmap.notes[noteIndex];
        NoteHandler currentNoteHandler = noteObj.GetComponent<NoteHandler>();

        currentNoteHandler._beatmapPlayer = this;
        currentNoteHandler.noteInfo = currentNoteInfo;
        noteObj.transform.position = columns[currentNoteInfo.columnIndex].transform.position;
        noteObj.transform.parent = columns[currentNoteInfo.columnIndex].transform;
        columns[currentNoteInfo.columnIndex].notes.Add(currentNoteHandler);

        if (currentNoteInfo.type == noteType.holdNote)
        {
            GameObject holdEndObj = Instantiate(notePrefab);
            NoteHandler holdEndNoteHandler = holdEndObj.GetComponent<NoteHandler>();

            currentNoteHandler.holdEndNote = holdEndNoteHandler;
            holdEndNoteHandler._beatmapPlayer = this;
            holdEndNoteHandler.noteInfo = currentNoteInfo;
            holdEndNoteHandler.noteInfo.type = noteType.endHoldNote;
            holdEndObj.transform.position = columns[currentNoteInfo.columnIndex].transform.position;
            columns[currentNoteInfo.columnIndex].notes.Add(holdEndNoteHandler);
            holdEndObj.transform.parent = columns[currentNoteInfo.columnIndex].transform;
            StartCoroutine(holdEndNoteHandler.moveNote((float)currentNoteInfo.endTime - elapsed * 1000f));
        }

        StartCoroutine(currentNoteHandler.moveNote(0f));
    }

}
