using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static DragIcon Instance;
    public Canvas canvas;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image image;

    private Sprite currentSprite;
    private object payload; // 可以存放附带的数据（比如答案编号）
    private AnswerSlot sourceSlot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        image = GetComponent<Image>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        Hide();
    }

    public void Show(Sprite sprite)
    {
        image.sprite = sprite;
        image.enabled = true;
        canvasGroup.alpha = 1f;
        rectTransform.sizeDelta = new Vector2(50, 50);
    }

    public void BeginDrag(AnswerSlot slot, Sprite sprite, object data = null)
    {
        sourceSlot = slot;
        currentSprite = sprite;
        payload = data;

        image.sprite = sprite;
        image.enabled = true;
        canvasGroup.blocksRaycasts = false;

        rectTransform.SetParent(canvas.transform, true);
        rectTransform.SetAsLastSibling();
        UpdatePosition(Input.mousePosition);
    }

    public void UpdatePosition(Vector2 screenPosition)
    {
        if (canvas == null) return;

        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPosition,
            canvas.worldCamera,
            out localPosition);

        rectTransform.localPosition = localPosition;
    }

    public void EndDrag(PointerEventData eventData = null)
    {
        canvasGroup.blocksRaycasts = true;

        if (eventData != null && eventData.pointerEnter != null)
        {
            // ? 触发目标的 OnDrop
            ExecuteEvents.ExecuteHierarchy(eventData.pointerEnter, eventData, ExecuteEvents.dropHandler);
        }

        Hide();
    }

    public void Hide()
    {
        image.enabled = false;
        image.sprite = null;
        currentSprite = null;
        payload = null;
        sourceSlot = null;
        canvasGroup.alpha = 0f;
    }

    // 为了支持直接挂 DragIcon 上的拖拽事件
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdatePosition(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDrag(eventData);
    }
}
