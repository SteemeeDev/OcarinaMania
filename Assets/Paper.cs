using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Paper : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] sprites;
    [SerializeField] Canvas[] canvases;

    public void UpdateSpriteOrder(int index)
    {
        foreach (var sprite in sprites)
        {
            sprite.sortingOrder += index;
        }
        foreach (var canv in canvases)
        {
            canv.sortingOrder += index;
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

        Destroy(gameObject);
    }

}
