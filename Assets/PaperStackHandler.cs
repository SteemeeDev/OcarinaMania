using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PaperStackHandler : MonoBehaviour
{
    public GameObject paperPrefab;

    public int BeatmapCount = 0;
    [SerializeField] BeatmapManager beatmapManager;
    List<Paper> papers  = new List<Paper>();
    [SerializeField] int maxPaperStack;

    AudioSource musicPreview;

    [SerializeField] int selectedIndex = 0;
    [SerializeField] int amountOfPaper = 0;

    private void Start()
    {
        musicPreview = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject paper = Instantiate(paperPrefab);
            Paper paperScript = paper.GetComponent<Paper>();
            paperScript.beatmap = beatmapManager.beatMaps[selectedIndex];
            paperScript.UpdateMapData();
            StartCoroutine(paperScript.RotatePaper(0.5f));
            paper.transform.position = new Vector3(0, -20, 0);
            paper.transform.parent = transform;
            paperScript.UpdateSpriteOrder(amountOfPaper);
            amountOfPaper++;
            papers.Add(paperScript);

            if (amountOfPaper > maxPaperStack)
            {
                Paper temp = papers.ElementAt(0);
                papers.RemoveAt(0);
                StartCoroutine(temp.FadeOutPaper(0.5f));
            }

            AudioClip newClip = Resources.Load<AudioClip>(
                "Audios/" + Path.GetFileNameWithoutExtension(Application.dataPath + "/Beatmaps/Resources/Audios/" + beatmapManager.beatMaps[selectedIndex].musicFile)
            );

            if (newClip != musicPreview.clip) { 
                musicPreview.clip = newClip;
                musicPreview.time = musicPreview.clip.length * 0.25f;
                musicPreview.Play();
            }

            selectedIndex++;
            if (selectedIndex > 0) selectedIndex %= beatmapManager.beatMaps.Count;

        }

        if (papers.Count > 0 && Input.GetKeyDown(KeyCode.E))
        {
            beatmapManager.mapIndex = selectedIndex - 1;
            SceneManager.LoadScene(2);
        }
    }
}
