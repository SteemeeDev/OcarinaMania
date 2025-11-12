using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Paper : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] sprites;
    [SerializeField] Canvas[] canvases;
    public Beatmap beatmap;

    [SerializeField] float minRotation;
    [SerializeField] float maxRotation;

    public TMP_Text title;
    public TMP_Text difficulty;
    public TMP_Text length;
    public TMP_Text artist;
    public TMP_Text charter;
    public RawImage albumCover;

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        Debug.Log(beatmap.name);
    }

    public void UpdateMapData()
    {
        title.text = beatmap.name;
        difficulty.text = "Difficulty: " + beatmap.difficulty;
        length.text = "Length: " + FormatLength(beatmap.length);
        artist.text = "Song Artist: " + beatmap.author;
        charter.text = "Charter: " + beatmap.creator;
        albumCover.texture = beatmap.albumCover.texture;
    }

    string FormatLength(float length)
    {
        int minuteCount = (int)(beatmap.length / 1000f / 60f);
        int secondsCount = (int)((beatmap.length / 1000f) % 60f);

        string formattedMinutes = minuteCount.ToString();
        string formattedSeconds = secondsCount.ToString();

        if (minuteCount < 10)
        {
            formattedMinutes = "0" + minuteCount.ToString();
        }
        if (secondsCount < 10)
        {
            formattedSeconds = "0" + secondsCount.ToString();
        }

        return formattedMinutes + ":" + formattedSeconds;
    }

    public void UpdateSpriteOrder(int index)
    {
        foreach (var sprite in sprites)
        {
            sprite.sortingOrder += index * 3;
        }
        foreach (var canv in canvases)
        {
            canv.sortingOrder += index * 3;
        }
    }

    public IEnumerator RotatePaper(float animationTime)
    {
        float elapsed = 0;
        foreach (var sprite in sprites)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
        }

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, Random.Range(minRotation, maxRotation));

        while (elapsed < animationTime)
        {
            elapsed += Time.deltaTime;

            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsed / animationTime);

            yield return null;
        }
    }

    public IEnumerator FadeOutPaper(float animationTime)
    {
        float elapsed = 0;

        while (elapsed < animationTime)
        {
            elapsed += Time.deltaTime;

            foreach (var sprite in sprites)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f - elapsed / animationTime);
            }

            yield return null;
        }

        transform.position = new Vector3(0, -20, 0);
        animator.enabled = false;

        Debug.Log("Paper fade finished");

        Destroy(gameObject);
    }

}
