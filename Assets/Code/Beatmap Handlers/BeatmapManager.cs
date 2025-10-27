using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BeatmapManager : MonoBehaviour
{
    [SerializeField] string[] beatMapNames;
    [SerializeField] int mapIndex = 0;

    [SerializeField] List<Beatmap> beatMaps = new List<Beatmap>();
    [SerializeField] Beatmap currentBeatmap;
    [SerializeField] GameObject notePrefab;

    public Column[] columns;

    AudioSource audioSource;

    public float noteOffset = 0.5f; // Time in seconds for a note to travel from spawn to hit position
    public float noteTapDistance = 0.2f; // Distance to the tap target (both ways), where tapping the note is valid

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        for (int i = 0; i < beatMapNames.Length; i++)
        {
            ParseBeatmap(Application.dataPath + @"/Beatmaps/" + beatMapNames[i]);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            StopAllCoroutines();
            currentBeatmap = beatMaps[mapIndex];
            StartCoroutine(PlayBeatmap(mapIndex));
        }
    }

    public IEnumerator PlayBeatmap(int beatmapIndex)
    {
        for (int i = 0; i < columns.Length; i++)
        {
            for(int j = 0; j < columns[i].notes.Count - 1; i++)
            {
                Destroy(columns[i].notes[j].gameObject);
                columns[i].notes.RemoveAt(j);
            }
        }

        currentBeatmap = beatMaps[beatmapIndex];

        audioSource.clip = Resources.Load<AudioClip>("Audios/" + Path.GetFileNameWithoutExtension(Application.dataPath + "/Beatmaps/Resources/Audios/" + currentBeatmap.musicFile));

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

        currentNoteHandler.beatmapManager = this;
        currentNoteHandler.noteInfo = currentNoteInfo;
        noteObj.transform.position = columns[currentNoteInfo.columnIndex].transform.position;
        noteObj.transform.parent = columns[currentNoteInfo.columnIndex].transform;
        columns[currentNoteInfo.columnIndex].notes.Add(currentNoteHandler);

        if (currentBeatmap.notes[noteIndex].type == 128)
        {    
            GameObject holdEndObj = Instantiate(notePrefab);
            NoteHandler holdEndNoteHandler = holdEndObj.GetComponent<NoteHandler>();

            currentNoteHandler.endHoldPoint = holdEndObj.transform;
            holdEndNoteHandler.beatmapManager = this;
            holdEndNoteHandler.noteInfo = currentNoteInfo;
            holdEndObj.transform.position = columns[currentNoteInfo.columnIndex].transform.position;
            columns[currentNoteInfo.columnIndex].notes.Add(holdEndNoteHandler);
            holdEndObj.transform.parent = columns[currentNoteInfo.columnIndex].transform;
            StartCoroutine(holdEndNoteHandler.moveNote((float)currentNoteInfo.endTime - elapsed * 1000f));
        }
        
        StartCoroutine(currentNoteHandler.moveNote(0f));
    }

    public void ParseBeatmap(string filePath)
    {
        Beatmap beatmapObj = new Beatmap();
        beatmapObj.notes = new List<NoteInfo>();

        Debug.Log("PARSING BEATMAP...");

        StreamReader reader;

        try
        {
            reader = new StreamReader(filePath);
        }
        catch (Exception e)
        {
            Debug.LogError("FAILED TO OPEN BEATMAP FILE! " + e.Message);
            return;
        }

        string currentLine = "";
        char currentChar = ' ';

        int loopIndex = 0; // Safety loop index

        // ------------------------------------------------------------ //
        // Reach the general section
        while (!reader.EndOfStream)
        {
            currentLine = reader.ReadLine();
            if (currentLine == "[General]") break;

            loopIndex++;

            if (loopIndex >= 100000)
            {
                Debug.LogError("FAILED TO FIND GENERAL SECTION IN BEATMAP!");
                break;
            }
        }

        // Read the general values (only used for audioleadin as of now)
        while (!reader.EndOfStream)
        {
            currentLine = reader.ReadLine();
            if (currentLine == "") break; // End of general section

            string[] splitLine = currentLine.Split(':');
            if (splitLine.Length < 2) continue;

            string key = splitLine[0].Trim();
            string value = splitLine[1].Trim();

            Debug.Log($"{key} : {value}");

            switch (key)
            {
                case "AudioFilename":
                    beatmapObj.musicFile = value;
                    Debug.Log("Found music file: " + value);
                    break;
                case "AudioLeadIn":
                    beatmapObj.audioLeadIn = float.Parse(value);
                    break;
            }
        }

        // ------------------------------------------------------------ //
        // Reach the metadata section
        while (!reader.EndOfStream)
        {
            currentLine = reader.ReadLine();
            if (currentLine == "[Metadata]") break;

            loopIndex++;

            if (loopIndex >= 100000)
            {
                Debug.LogError("FAILED TO FIND METADATA SECTION IN BEATMAP!");
                break;
            }
        }

        // Read the metadata values
        while (!reader.EndOfStream)
        {
            currentLine = reader.ReadLine();
            if (currentLine == "") break; // End of metadata section

            string[] splitLine = currentLine.Split(':');
            if (splitLine.Length < 2) continue;

            string key = splitLine[0].Trim();
            string value = splitLine[1].Trim();

            Debug.Log($"{key} : {value}");

            switch (key)
            {
                case "Title":
                    beatmapObj.name = value;
                    break;
                case "Artist":
                    beatmapObj.author = value;
                    break;
                case "Creator":
                    beatmapObj.creator = value;
                    break;
                case "Version":
                    beatmapObj.difficulty = value;
                    break;
            }
        }

        loopIndex = 0;

        // ------------------------------------------------------------ 
        // Reach the notes section
        while (!reader.EndOfStream)
        {
            currentLine = reader.ReadLine();
            if (currentLine == "[HitObjects]") break;

            loopIndex++;

            if (loopIndex >= 100000)
            {
                Debug.LogError("FAILED TO FIND NOTES SECTION IN BEATMAP!");
                break;
            }
        }

        int posX = -999;
        int posY = -999;
        float time = -999f;
        int type = -999;
        int endTime = -999;

        int valueIndex = 0; // The values are seperated by commas, in the order of posX, posY, time, type, hitSound, objectParams, hitSample
        string num = ""; // The number at the valueIndex

        loopIndex = 0;

        Debug.Log("FOUND NOTES SECTION, READING NOTES...");

        // Read the note values
        while (!reader.EndOfStream)
        {
            currentChar = (char)reader.Read();
            if (currentChar != ',' && currentChar != ':')
            {
                num += currentChar;
            }
            else
            {
                switch (valueIndex)
                {
                    case 0:
                        posX = Int32.Parse(num);
                        break;
                    case 1:
                        posY = Int32.Parse(num);
                        break;
                    case 2:
                        time = float.Parse(num);
                        break;
                    case 3:
                        type = Int32.Parse(num);
                        break;
                    case 4:
                        // We don't care about hitsound for now
                        break;
                    case 5:
                        if (type == 128) // Hold note
                        {
                            endTime = Int32.Parse(num);
                        }
                        break;
                }
                num = "";
                valueIndex++;
                if (valueIndex >= 6)
                {
                    valueIndex = 0;

                    /*Debug.Log(
                        $"Note found!" +
                        $"\nPOS X: " + posX.ToString() +
                        $"\nColumn: " + Mathf.Floor(posX * 4 / 512).ToString() +
                        $"\nPOS Y: " + posY.ToString() +
                        $"\nTIME: " + time.ToString()
                    );*/

                    beatmapObj.notes.Add(new NoteInfo
                    {
                        columnIndex = (int)Mathf.Floor(posX * 4 / 512),
                        time = time,
                        type = type,
                        endTime = endTime
                    });

                    posX = -999;
                    posY = -999;
                    time = -999f;
                    type = -999;
                    endTime = -999;

                    reader.ReadLine(); // Move to next line
                }
            }

            loopIndex++;

            if (loopIndex >= 100000)
            {
                Debug.LogError("FAILED TO FIND NOTES!");
                break;
            }

        }

        reader.Close();

        beatMaps.Add(beatmapObj);
    }
}
