using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioBag : MonoBehaviour
{
    public Transform collectedAudioContainer;

    public GameObject collectedAudioIcon;

    List<CollectAudioClip> collectedAudioClipList = new List<CollectAudioClip>();
    List<GameObject> audioClipUI = new List<GameObject>();
    private CollectAudioClip currentShowingAudioClip;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(InitUIAfterFrame());
    }

    private IEnumerator InitUIAfterFrame()
    {
        yield return null;

        if (LevelFlowExecutor.Instance == null)
        {
            Debug.LogError("LevelFlowExecutor.Instance是null！");
            yield break;
        }

        // 获取音频列表
        collectedAudioClipList = LevelFlowExecutor.Instance.collectedAudioClips;

        // 清理旧的 UI
        foreach (GameObject go in audioClipUI)
        {
            Destroy(go);
        }
        audioClipUI.Clear();

        // 创建新的 UI 元素
        foreach (CollectAudioClip a in collectedAudioClipList)
        {
            GameObject t = Instantiate(collectedAudioIcon, collectedAudioContainer);
            t.GetComponent<CollectedAudioIndicator>().collectedAudioClip = a;
            audioClipUI.Add(t);
        }
    }

    private void ShowCurrentAudioClip(){
        if (audioClipUI.Count > 0) { 
            audioClipUI[0].gameObject.SetActive(true);
        }
    }
}
