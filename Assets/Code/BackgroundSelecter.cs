using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSelecter : MonoBehaviour
{
    [SerializeField] Sprite[] backgrounds;

    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ChooseBackground();
        StartCoroutine(FadeIn(5f));
    }

    void ChooseBackground()
    {
        spriteRenderer.sprite = backgrounds[Random.Range(0, backgrounds.Length)];
    }

    IEnumerator FadeIn(float fadeTime)
    {
        float elapsed = 0;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;

            spriteRenderer.color = new Color(1, 1, 1, elapsed / fadeTime);

            yield return null;
        }

        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
}
