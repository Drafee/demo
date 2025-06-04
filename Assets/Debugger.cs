using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Debugger : MonoBehaviour
{
    private List<GraphicRaycaster> raycasters;
    private EventSystem eventSystem;

    void Start()
    {
        // �ҵ����������е� GraphicRaycaster��ÿ�� Canvas һ����
        raycasters = new List<GraphicRaycaster>(FindObjectsOfType<GraphicRaycaster>());

        eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            Debug.LogError("No EventSystem in scene!");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pointerPos = Input.mousePosition;
            PointerEventData pointerData = new PointerEventData(eventSystem)
            {
                position = pointerPos
            };

            Debug.Log("?? ���λ��: " + pointerPos);

            bool foundUI = false;

            foreach (var raycaster in raycasters)
            {
                List<RaycastResult> results = new List<RaycastResult>();
                raycaster.Raycast(pointerData, results);

                foreach (var result in results)
                {
                    Debug.Log($"?? ������� UI: {result.gameObject.name}������ Canvas: {raycaster.gameObject.name}��");
                    foundUI = true;
                    break; // ֻ��ӡ��һ�����е� UI Ԫ�أ��ɰ����Ƴ�
                }

                if (foundUI) break; // �����ĳ�� Canvas �ҵ��ˣ��Ͳ��ټ���������� Canvas
            }

            if (!foundUI)
            {
                Debug.Log("?? û�е�����κ� UI Ԫ��");
            }
        }
    }
}
