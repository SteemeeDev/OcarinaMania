using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BeatmapManager : MonoBehaviour
{
    [SerializeField] string[] beatMapNames;
    public List<Beatmap> beatMaps = new List<Beatmap>();

    private void Start()
    {
        for (int i = 0; i < beatMapNames.Length; i++)
        {
            ParseBeatmap(Application.dataPath + @"/Beatmaps/" + beatMapNames[i]);
        }
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
        int numType = -999;
        noteType _noteType = noteType.tapNote;
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
                        numType = Int32.Parse(num);
                        if (numType <= 64) _noteType = noteType.tapNote;
                        if (numType == 128) _noteType = noteType.holdNote;
                        break;
                    case 4:
                        // We don't care about hitsound for now
                        break;
                    case 5:
                        if (numType == 128)
                        {
                            _noteType = noteType.endHoldNote;
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
                        type = _noteType,
                        endTime = endTime
                    });

                    posX = -999;
                    posY = -999;
                    time = -999f;
                    numType = -999;
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
