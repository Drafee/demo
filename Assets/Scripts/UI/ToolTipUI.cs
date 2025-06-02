using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    public TextMeshProUGUI tooltipText;
    public RectTransform backgroundRectTransform;

    private RectTransform canvasRectTransform;

    [Header("Settings")]
    public Vector2 offset = new Vector2(10f, 10f); // 鼠标偏移量

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        HideTooltip();
    }

    private void Update()
    {
        /*
        if (gameObject.activeSelf)
        {
            FollowMouse();
        }
        */
    }

    public void ShowTooltip(RectTransform target, string text)
    {
        gameObject.SetActive(true);
        tooltipText.text = text;
        tooltipText.ForceMeshUpdate();

        Vector2 textSize = tooltipText.GetRenderedValues(false);
        backgroundRectTransform.sizeDelta = textSize + new Vector2(8f, 8f); // 内边距

        FollowMouse( target);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    private void FollowMouse(RectTransform target)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, target.position);
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            backgroundRectTransform.parent as RectTransform,
            screenPos + offset,
            null,
            out localPos
        );
        backgroundRectTransform.pivot = new Vector2(0f, 0f);
        backgroundRectTransform.localPosition = localPos;
        /*
        Vector2 localPoint;
        Vector2 mousePos = Input.mousePosition;

        // 将屏幕坐标转换为Canvas本地坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            mousePos,
            null,
            out localPoint
        );

        // 应用偏移量
        localPoint += offset;

        // 设定 Tooltip 的 pivot 为左下角
        tooltipRectTransform.pivot = new Vector2(0f, 0f);

        // 设置位置
        tooltipRectTransform.localPosition = localPoint;
        */
    }
}