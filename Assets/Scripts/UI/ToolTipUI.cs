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
    public Vector2 offset = new Vector2(10f, 10f); // ���ƫ����

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
        backgroundRectTransform.sizeDelta = textSize + new Vector2(8f, 8f); // �ڱ߾�

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

        // ����Ļ����ת��ΪCanvas��������
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            mousePos,
            null,
            out localPoint
        );

        // Ӧ��ƫ����
        localPoint += offset;

        // �趨 Tooltip �� pivot Ϊ���½�
        tooltipRectTransform.pivot = new Vector2(0f, 0f);

        // ����λ��
        tooltipRectTransform.localPosition = localPoint;
        */
    }
}