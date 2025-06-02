using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AudioAnswerPanelBag : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public AudioClipItem audioClipItem;
    private UIAnswerArea uiAnswerArea;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void Init(UIAnswerArea manager, CollectAudioClip clip)
    {
        uiAnswerArea = manager;
        audioClipItem = new AudioClipItem(clip);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioClipItem != null && !string.IsNullOrEmpty(audioClipItem.collectAudioClip.notes))
        {
            TooltipUI.Instance.ShowTooltip(GetComponent<RectTransform>(), audioClipItem.collectAudioClip.notes);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.HideTooltip();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root); // 临时移出 Layout Group
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;
        canvasGroup.blocksRaycasts = true;
    }

    public void ClearIcon()
    {
        gameObject.SetActive(false); // 拖拽成功后隐藏
    }

    public void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        ClearIcon();
        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;
    }
}
