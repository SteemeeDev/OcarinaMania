using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    [SerializeField] float m_hoverSizeIncreasePercentage = 20;
    [SerializeField] float m_easingTime = 0.15f;


    RectTransform m_rectTransform;
    Vector2 startSize;
    public void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
        startSize = m_rectTransform.sizeDelta;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        StartCoroutine(ieSmoothHover(m_easingTime));
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        StartCoroutine(ieSmoothExit(m_easingTime));
    }


    IEnumerator ieSmoothHover(float smoothTime)
    {
        float elapsed = 0;
        float t;
        while (elapsed < smoothTime)
        {
            elapsed += Time.deltaTime;
            t = elapsed / smoothTime;
            m_rectTransform.sizeDelta = Vector2.Lerp(
                startSize, startSize * (1 + m_hoverSizeIncreasePercentage * 0.01f), 1 - Mathf.Pow(1 - t, 4)
            );
            yield return null;
        }
    }
    IEnumerator ieSmoothExit(float smoothTime)
    {
        float elapsed = smoothTime;
        float t;
        while (elapsed > 0)
        {
            elapsed -= Time.deltaTime;
            t = elapsed / smoothTime;
            m_rectTransform.sizeDelta = Vector2.Lerp(
                startSize, startSize * (1 + m_hoverSizeIncreasePercentage * 0.01f), 1 - Mathf.Pow(1 - t, 4)
            );
            yield return null;
        }
    }
}
