using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public class Paper : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] sprites;
    [SerializeField] Canvas[] canvases;
    [SerializeField] Beatmap beatmap;

    [SerializeField] float minRotation;
    [SerializeField] float maxRotation;

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdateSpriteOrder(int index)
    {
        foreach (var sprite in sprites)
        {
            sprite.sortingOrder = index*3;
        }
        foreach (var canv in canvases)
        {
            canv.sortingOrder = index*3;
        }
    }

    public IEnumerator RotatePaper(float animationTime)
    {

        Debug.Log("Starting paper rotate");
        yield return StartCoroutine(FadeOutPaper(1f));
        Debug.Log("End of paper rotate");
        float elapsed = 0;
        foreach (var sprite in sprites)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
        }

        animator.enabled = true;
        animator.SetTrigger("PlacePaper");

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, Random.Range(minRotation, maxRotation));

        while (elapsed < animationTime)
        {
            elapsed += Time.deltaTime;

            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsed / animationTime);

            yield return null;
        }

        Debug.Log("End of rotatepaer");
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
    }

}
