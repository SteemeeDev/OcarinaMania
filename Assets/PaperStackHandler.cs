using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperStackHandler : MonoBehaviour
{
    [SerializeField] GameObject paperPrefab;
    List<GameObject> papers = new List<GameObject>();
    [SerializeField] int maxPaperStack;
    [SerializeField] float minRotation;
    [SerializeField] float maxRotation;

    int amountOfPaper = 0;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject paper = Instantiate(paperPrefab);
            papers.Add(paper);
            Paper paperScript = paper.GetComponent<Paper>();
            paperScript.UpdateSpriteOrder(amountOfPaper);
            amountOfPaper++;
            StartCoroutine(RotatePaper(0.5f, paper));

            if (papers.Count > maxPaperStack)
            {

                GameObject temp = papers[0];
                Paper tempPaperScript = temp.GetComponent<Paper>();
                papers.RemoveAt(0);
                StartCoroutine(tempPaperScript.FadeOutPaper(1f));
            }
        }
    }

    IEnumerator RotatePaper(float animationTime, GameObject paperObj)
    {
        float elapsed = 0;

        Quaternion startRotation = paperObj.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(paperObj.transform.rotation.x, paperObj.transform.rotation.y, Random.Range(minRotation, maxRotation));


        while (elapsed < animationTime)
        {
            elapsed += Time.deltaTime;

            paperObj.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsed / animationTime);

            yield return null;
        }
    }
}
