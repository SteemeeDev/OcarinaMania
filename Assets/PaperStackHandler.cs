using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperStackHandler : MonoBehaviour
{
    [SerializeField] GameObject paperPrefab;
    [SerializeField] BeatmapManager beatmapManager;
    [SerializeField] Paper[] papers;
    [SerializeField] int maxPaperStack;

    [SerializeField] int selectedIndex = 0;
    [SerializeField] int amountOfPaper = 0;

    private void Start()
    {
        for (int i = 0; i < beatmapManager.beatMapNames.Length; i++)
        {
            beatmapManager.ParseBeatmap(Application.dataPath + @"/Beatmaps/" + beatmapManager.beatMapNames[i]);
        }

        papers = new Paper[beatmapManager.beatMaps.Count];

        for (int i = 0; i < papers.Length; i++)
        {
            GameObject paper = Instantiate(paperPrefab);
            Paper paperScript = paper.GetComponent<Paper>();
            paper.transform.position = new Vector3(0,-20,0);
            paper.transform.parent = transform;
            papers[i] = paperScript;
            paperScript.UpdateSpriteOrder(amountOfPaper);
            amountOfPaper++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(papers[selectedIndex].RotatePaper(0.5f));

            selectedIndex++;
            if (amountOfPaper > 0) selectedIndex %= amountOfPaper;
        }
    }
}
