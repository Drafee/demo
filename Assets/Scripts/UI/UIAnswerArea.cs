using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnswerArea : MonoBehaviour
{
    [Header("Bottom Bag")]
    public Transform bagContainer;               // Bag区域父物体
    public GameObject collectedAudioIcon;        // 预制的音频图标

    private List<CollectAudioClip> collectedAudioClipList = new List<CollectAudioClip>();
    private List<AudioAnswerPanelBag> bagIcons = new List<AudioAnswerPanelBag>();
    [Header("Answer Area")]
    [SerializeField] private AnswerSlot[] allSlots1;
    [SerializeField] private AnswerSlot[] allSlots2;
    [SerializeField] private AnswerSlot[] allSlots3;
    private AnswerSlot[] currentAllSlots;
    public List<GameObject> slotContainer;
    bool result;

    private AudioSource audioSource;
    private Coroutine playRoutine;
    private void Start()
    {
        // gameObject.SetActive(false);
        result = false;
        audioSource = gameObject.AddComponent<AudioSource>();
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
            Debug.LogError("LevelFlowExecutor.Instance是null！");
            yield break;
        }
        slotContainer[LevelFlowExecutor.Instance.currentLevel - 1].SetActive(true);
        foreach (var icon in bagIcons)
        {
            Destroy(icon.gameObject);
        }
        bagIcons.Clear();

        collectedAudioClipList = LevelFlowExecutor.Instance.collectedAudioClips;

        // 恢复 Slot 内容
        // RestoreSlots();

        // 获取 Slot 中已使用的 clip 名字
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

        // 创建未使用的物品 UI
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

    public void PlayOrStopSlotClips()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
            audioSource.Stop();
            return;
        }

        playRoutine = StartCoroutine(PlayAllSlotAudioSequentially());
    }
    private IEnumerator PlayAllSlotAudioSequentially()
    {
        foreach (var slot in currentAllSlots)
        {
            if (slot.currentAudioClipItem == null ||
                slot.currentAudioClipItem.collectAudioClip == null ||
                slot.currentAudioClipItem.collectAudioClip.audioData == null ||
                slot.currentAudioClipItem.collectAudioClip.audioData.audioClip == null)
                continue;

            AudioClip clip = slot.currentAudioClipItem.collectAudioClip.audioData.audioClip;

            audioSource.clip = clip;
            audioSource.Play();
            yield return new WaitWhile(() => audioSource.isPlaying);
        }

        playRoutine = null;
    }
}
