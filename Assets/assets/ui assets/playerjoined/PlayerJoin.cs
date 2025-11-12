using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerJoin : MonoBehaviour
{
    [SerializeField] private KeyCode[] Player1Keys;
    [SerializeField] private KeyCode[] Player2Keys;

    [SerializeField] GameObject Player1Ocarina;
    [SerializeField] GameObject Player2Ocarina;

    private void Update()
    {
        foreach (KeyCode key in Player1Keys)
        {
            if (Input.GetKeyDown(key))
            {
                Player1Ocarina.SetActive(true);
            }
        }
        foreach (KeyCode key in Player2Keys)
        {
            if (Input.GetKeyDown(key))
            {
                Player2Ocarina.SetActive(true);
            }
        }
    }
}
