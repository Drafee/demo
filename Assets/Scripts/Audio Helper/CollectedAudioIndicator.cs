using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CollectedAudioIndicator : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public CollectAudioClip collectedAudioClip;

    private UIAudioBag audioBagManager;

    private Image backgroundImage;

    private Vector3 originalScale;

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
        originalScale = transform.localScale;
    }

    public void Init(UIAudioBag manager, CollectAudioClip clip)
    {
        audioBagManager = manager;
        collectedAudioClip = clip;
        Deselect();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        audioBagManager.SelectAudioClip(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 鼠标悬停时放大一点点
        transform.localScale = originalScale * 1.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 鼠标离开还原大小
        transform.localScale = originalScale;
    }

    public void Select()
    {
        if (backgroundImage != null)
            backgroundImage.color = Color.yellow;
    }

    public void Deselect()
    {
        if (backgroundImage != null)
            backgroundImage.color = Color.white;
    }

}
