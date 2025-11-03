using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeatmapPlayer : MonoBehaviour
{
    [SerializeField] BeatmapManager manager;
    [SerializeField] float points;
    [SerializeField] TMP_Text pointCounter;
    [SerializeField, Range(0,3)] SpriteRenderer[] keys;
    [SerializeField] KeyCode[] keyBinds;

    private void Update()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keyBinds[i]))
            {
                if (manager.columns[i].notes.Count > 0 && manager.columns[i].notes[0].tappable)
                {
                    manager.columns[i].notes[0].gameObject.SetActive(false);
                    points++;
                    pointCounter.text = points.ToString();
                }
                keys[i].transform.localScale = Vector2.one * 1.5f;
            }
            else if (Input.GetKeyUp(keyBinds[i]))
            {
                keys[i].transform.localScale = Vector2.one;
            }
        }
    }
}
