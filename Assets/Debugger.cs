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
        // 找到场景中所有的 GraphicRaycaster（每个 Canvas 一个）
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

            Debug.Log("?? 点击位置: " + pointerPos);

            bool foundUI = false;

            foreach (var raycaster in raycasters)
            {
                List<RaycastResult> results = new List<RaycastResult>();
                raycaster.Raycast(pointerData, results);

                foreach (var result in results)
                {
                    Debug.Log($"?? 点击到了 UI: {result.gameObject.name}（来自 Canvas: {raycaster.gameObject.name}）");
                    foundUI = true;
                    break; // 只打印第一个命中的 UI 元素，可按需移除
                }

                if (foundUI) break; // 如果在某个 Canvas 找到了，就不再继续检查其他 Canvas
            }

            if (!foundUI)
            {
                Debug.Log("?? 没有点击到任何 UI 元素");
            }
        }
    }
}
