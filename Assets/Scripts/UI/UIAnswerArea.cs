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
    [SerializeField] private AnswerSlot[] allSlots1;
    [SerializeField] private AnswerSlot[] allSlots2;
    [SerializeField] private AnswerSlot[] allSlots3;
    private AnswerSlot[] currentAllSlots;
    public List<GameObject> slotContainer;
    bool result;

    /*
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

        }
    */
    private void Start()
    {
        // gameObject.SetActive(false);
        result = false;
    }

    private void OnEnable()
    {

        StartCoroutine(InitBagUI());
    }

    private void OnDisable()
    {
        slotContainer[LevelFlowExecutor.Instance.currentLevel - 1].SetActive(false);
    }
    private IEnumerator InitBagUI()
    {
        yield return null;

        if (LevelFlowExecutor.Instance == null)
        {
            Debug.LogError("LevelFlowExecutor.Instance��null��");
            yield break;
        }
        slotContainer[LevelFlowExecutor.Instance.currentLevel - 1].SetActive(true);
        foreach (var icon in bagIcons)
        {
            Destroy(icon.gameObject);
        }
        bagIcons.Clear();

        collectedAudioClipList = LevelFlowExecutor.Instance.collectedAudioClips;

        // �ָ� Slot ����
        // RestoreSlots();

        // ��ȡ Slot ����ʹ�õ� clip ����
        HashSet<string> usedClipNames = new HashSet<string>();
        switch (LevelFlowExecutor.Instance.currentLevel) {
            case 1:
                currentAllSlots = allSlots1;
                break;
            case 2:
                currentAllSlots = allSlots2;
                break;
            case 3:
                currentAllSlots = allSlots3;
                break;
        }
        foreach (var slot in currentAllSlots)
        {
            if (slot.currentAudioClipItem!= null && slot.currentAudioClipItem.collectAudioClip != null && slot.currentAudioClipItem.collectAudioClip.audioData != null && slot.currentAudioClipItem.collectAudioClip.audioData.id != null)
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
        UIManager.Instance.HideAnswerAreaPanel();
    }

    public void CheckAnswer() {
        if (result)
            return;
        string[] clipIds = new string[currentAllSlots.Length];
        for (int i = 0; i < currentAllSlots.Length; i++)
        {
            var slot = currentAllSlots[i];
            string clipId = slot.currentAudioClipItem != null ? slot.currentAudioClipItem.collectAudioClip.audioData.id : "";
            clipIds[i] = clipId;
        }

        string joined = string.Join("", clipIds);
        Debug.Log("Current Answer" + joined);

        result = LevelFlowExecutor.Instance.CheckAnswer(joined);
        if (result) {
            UIManager.Instance.HideAnswerAreaPanel();
            result = false;
        }
    }
}
