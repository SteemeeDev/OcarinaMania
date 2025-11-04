public enum noteType
{
    tapNote,
    holdNote,
    endHoldNote
}

public struct NoteInfo
{
    public int columnIndex;
    public float time;
    public noteType type;
    public int endTime;
}