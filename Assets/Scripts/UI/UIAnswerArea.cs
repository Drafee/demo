using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnswerArea : MonoBehaviour
{
    [Header("Bottom Bag")]
    public Transform bagContainer;               // Bag��������
    public GameObject collectedAudioIcon;        // Ԥ�Ƶ���Ƶͼ��

    private List<CollectAudioClip> collectedAudioClipList = new List<CollectAudioClip>();
    private List<AudioAnswerPanelBag> bagIcons = new List<AudioAnswerPanelBag>();
    [Header("Answer Area")]
    [SerializeField] private AnswerSlot[] allSlots;
    public void SaveAllSlots()
    {
        for (int i = 0; i < allSlots.Length; i++)
        {
            var slot = allSlots[i];
            string clipId = slot.currentAudioClipItem != null ? slot.currentAudioClipItem.collectAudioClip.audioData.id: "";
            PlayerPrefs.SetString($"Slot_{i}", clipId);
        }
        PlayerPrefs.Save();
    }
    private void RestoreSlots()
    {
        /*
        foreach (var slot in allSlots)
        {
            slot.ClearSlot();
        }

        foreach (var clip in collectedAudioClipList)
        {
            for (int i = 0; i < allSlots.Length; i++)
            {
                string savedClipId = PlayerPrefs.GetString($"Slot_{i}", "");
                if (!string.IsNullOrEmpty(savedClipId) && savedClipId == clip.audioData.id)
                {
                    allSlots[i].SetClip(clip); // �Զ��巽�������� clip������ UI
                }
            }
        }
        */
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(InitBagUI());
    }
    private IEnumerator InitBagUI()
    {
        yield return null;

        if (LevelFlowExecutor.Instance == null)
        {
            Debug.LogError("LevelFlowExecutor.Instance��null��");
            yield break;
        }

        foreach (var icon in bagIcons)
        {
            Destroy(icon.gameObject);
        }
        bagIcons.Clear();

        collectedAudioClipList = LevelFlowExecutor.Instance.collectedAudioClips;

        // �ָ� Slot ����
        RestoreSlots();

        // ��ȡ Slot ����ʹ�õ� clip ����
        HashSet<string> usedClipNames = new HashSet<string>();
        foreach (var slot in allSlots)
        {
            if (slot.currentAudioClipItem!= null && slot.currentAudioClipItem.collectAudioClip.audioData.id != null)
            {
                usedClipNames.Add(slot.currentAudioClipItem.collectAudioClip.audioData.id);
            }
        }

        // ����δʹ�õ���Ʒ UI
        foreach (var clip in collectedAudioClipList)
        {
            if (usedClipNames.Contains(clip.audioData.id)) continue;

            GameObject go = Instantiate(collectedAudioIcon, bagContainer);
            var indicator = go.GetComponent<AudioAnswerPanelBag>();
            indicator.Init(this, clip);
            bagIcons.Add(indicator);
        }
    }



    public void ClosePanel() {
        gameObject.SetActive(false);
    }
}
