using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ColumnIndex
{
    Left,
    MiddleLeft,
    MiddleRight,
    Right
}

public class Column : MonoBehaviour
{
    [SerializeField] ColumnIndex columnIndex;
    public List<NoteHandler> notes;
}
