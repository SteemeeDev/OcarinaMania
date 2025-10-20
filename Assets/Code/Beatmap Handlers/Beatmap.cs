using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Beatmap
{
    public string name;
    public string author;
    public string creator;
    public float length;
    public float audioLeadIn;
    public string difficulty;

    public List<NoteInfo> notes;
}