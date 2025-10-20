using System.Collections.Generic;

public struct Beatmap
{
    public string name;
    public string author;
    public string creator;
    public float length;
    public string musicFile;
    public float audioLeadIn;
    public string difficulty;

    public List<NoteInfo> notes;
}