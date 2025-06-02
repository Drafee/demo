using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnswerSlot : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public AudioClipItem currentAudioClipItem;
    public Sprite currentSprite;
    public AudioAnswerPanelBag linkedBag; // �����λ��Ӧ��Bag���壨����Bag�ϻأ�

    private Image bgImage;
    private Color normalColor = Color.white;
    private Color occupiedColor = new Color(0.7f, 0.9f, 1f);

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Awake()
    {
        bgImage = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        ClearSlot();
    }
    /*
    public void SetClip(CollectAudioClip clip)
    {
        if (clip != null)
        {
            icon.sprite = clip.iconSprite; // �������и� sprite
            icon.enabled = true;

            label.text = clip.audioName;
        }
    }
    */
    public void ClearSlot()
    {
        currentAudioClipItem = null;
        linkedBag = null;
        bgImage.color = normalColor;
        currentSprite = null;
    }

    // �������Ϸŵ��ò�λ
    public void OnDrop(PointerEventData eventData)
    {

        // Debug.Log("Here I Drop");
        var draggedBag = eventData.pointerDrag?.GetComponent<AudioAnswerPanelBag>();
        var draggedSlot = eventData.pointerDrag?.GetComponent<AnswerSlot>();

        if (draggedBag != null)
        {
            // �ӱ�����
            if (currentAudioClipItem != null)
                linkedBag?.Show();

            currentAudioClipItem = draggedBag.audioClipItem;
            currentSprite = draggedBag.GetComponent<Image>().sprite;
            linkedBag = draggedBag;

            draggedBag.Hide();
            bgImage.color = occupiedColor;
        }
        else if (draggedSlot != null && draggedSlot != this)
        {
            // Slot �� Slot ����
            // Debug.Log("Change Slot");
            var tempClip = currentAudioClipItem;
            var tempImage = currentSprite;
            var tempBag = linkedBag;

            currentAudioClipItem = draggedSlot.currentAudioClipItem;
            currentSprite = draggedSlot.currentSprite;
            linkedBag = draggedSlot.linkedBag;

            draggedSlot.currentAudioClipItem = tempClip;
            draggedSlot.currentSprite = tempImage;
            draggedSlot.linkedBag = tempBag;

            bgImage.color = currentAudioClipItem != null ? occupiedColor : normalColor;
            draggedSlot.bgImage.color = draggedSlot.currentAudioClipItem != null ? occupiedColor : normalColor;

            if (linkedBag != null) linkedBag.Hide();
            // if (tempBag != null) tempBag.Show();
        }
    }


    // ֧���ϻ�Bag��������קSlotʱ��������קͼ��
    public void OnBeginDrag(PointerEventData eventData)
    {
        DragIcon.Instance.Show(currentSprite);
        eventData.pointerDrag = gameObject; // ����ק����ָ���Լ����߼���
        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {
        if (currentAudioClipItem == null) return;

        // ������קͼ��λ��
        DragIcon.Instance.UpdatePosition(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragIcon.Instance.Hide();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentAudioClipItem != null && !string.IsNullOrEmpty(currentAudioClipItem.collectAudioClip.notes))
        {
            TooltipUI.Instance.ShowTooltip(GetComponent<RectTransform>(), currentAudioClipItem.collectAudioClip.notes);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.HideTooltip();
    }

}