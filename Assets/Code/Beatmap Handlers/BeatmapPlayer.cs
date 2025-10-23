using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatmapPlayer : MonoBehaviour
{
    [SerializeField, Range(0,3)] SpriteRenderer[] keys;
    [SerializeField] KeyCode[] keyBinds;

    private void Update()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keyBinds[i]))
            {
                keys[i].transform.localScale = Vector2.one * 1.5f;
            }
            else if (Input.GetKeyUp(keyBinds[i]))
            {
                keys[i].transform.localScale = Vector2.one;
            }
        }
    }
}
