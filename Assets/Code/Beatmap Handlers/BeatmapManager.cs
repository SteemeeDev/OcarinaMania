using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class BeatmapManager : MonoBehaviour
{
    [SerializeField] List<Beatmap> beatMaps = new List<Beatmap>();
    [SerializeField] Beatmap currentBeatmap;
    [SerializeField] GameObject notePrefab;

    [SerializeField] Transform[] columns;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ParseBeatmap(Application.dataPath + @"/Beatmaps/xi - Happy End of the World (Blocko) [4K Insane].osu");
            currentBeatmap = beatMaps[0];
        }

        if (Input.GetKeyDown(KeyCode.Space)){
            StartCoroutine(PlayBeatmap(0));
        }
    }

    public IEnumerator PlayBeatmap(int beatmapIndex)
    {
        currentBeatmap = beatMaps[beatmapIndex];
        float beatmapLength = currentBeatmap.notes[currentBeatmap.notes.Count - 1].time;
        float timeElapsed = 0f;

        int noteIndex = 0;


        while (timeElapsed < beatmapLength)
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed * 1000 > currentBeatmap.audioLeadIn && !audioSource.isPlaying)
                audioSource.Play();

            if (timeElapsed * 1000 > currentBeatmap.notes[noteIndex].time)
            {
                GameObject noteObj = Instantiate(notePrefab);
                noteObj.transform.position = columns[currentBeatmap.notes[noteIndex].columnIndex].position;
                noteIndex++;
            }

            yield return null;
        }
    }

    public void ParseBeatmap(string filePath)
    {
        Beatmap beatmapObj = new Beatmap();
        beatmapObj.notes = new List<NoteInfo>();
        beatMaps.Add(beatmapObj);

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
                case "AudioLeadIn":
                    beatmapObj.audioLeadIn = float.Parse(value);
                    break;
            }
        }


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

        int valueIndex = 0; // The values are seperated by commas, in the order of posX, posY, time, type, hitSound, objectParams, hitSample
        string num = ""; // The number at the valueIndex

        loopIndex = 0;

        Debug.Log("FOUND NOTES SECTION, READING NOTES...");

        // Read the note values
        while (!reader.EndOfStream)
        {
            currentChar = (char)reader.Read();
            if (currentChar != ',')
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
                }
                num = "";
                valueIndex++;
                if (valueIndex >= 3)
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
                        time = time
                    });

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
    }
}
