using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAudioBag : MonoBehaviour
{
    [Header("Left Side Node Collection")]
    public Transform collectedAudioContainer;

    public GameObject collectedAudioIcon;

    List<CollectAudioClip> collectedAudioClipList = new List<CollectAudioClip>();
    List<CollectedAudioIndicator> audioClipUI = new List<CollectedAudioIndicator>();
    private CollectedAudioIndicator currentSelected;

    [Header("Right Side Play Panel")]
    public GameObject rightPanel;
    public Button playButton;
    public TMP_InputField descriptionInput;

    public AudioSource audioSource;
    public AudioClip audioClip;


    public Image progressCircle;
    private Coroutine progressRoutine;

    private void Start()
    {
        // gameObject.SetActive(false);
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
            Debug.LogError("LevelFlowExecutor.Instance��null��");
            yield break;
        }

        // ��ȡ��Ƶ�б�
        collectedAudioClipList = LevelFlowExecutor.Instance.collectedAudioClips;

        // ����ɵ� UI
        foreach (var ui in audioClipUI)
        {
            Destroy(ui.gameObject);
        }
        audioClipUI.Clear();

        // �����µ� UI Ԫ��
        foreach (CollectAudioClip a in collectedAudioClipList)
        {
            GameObject go = Instantiate(collectedAudioIcon, collectedAudioContainer);
            var indicator = go.GetComponent<CollectedAudioIndicator>();
            indicator.Init(this, a);
            audioClipUI.Add(indicator);
        }
        rightPanel.SetActive(false);
        if (audioClipUI.Count > 0)
        {
            SelectAudioClip(audioClipUI[0]);
        }
    }

    public void SelectAudioClip(CollectedAudioIndicator indicator)
    {
        if (currentSelected != null)
        {
            currentSelected.Deselect();
        }

        currentSelected = indicator;
        currentSelected.Select();

        rightPanel.SetActive(true);
        descriptionInput.text = indicator.collectedAudioClip.notes;
        audioClip = indicator.collectedAudioClip.audioData.audioClip;

        // ���ò��Ű�ť
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(PlayCurrentAudio);

    }

    private void PlayCurrentAudio()
    {
        audioSource = AudioPoolManager.Instance.PlaySound(audioClip, transform.position);

        if (progressRoutine != null)
            StopCoroutine(progressRoutine);
        progressRoutine = StartCoroutine(UpdateProgress());
    }

    private IEnumerator UpdateProgress()
    {
        progressCircle.fillAmount = 0f;
        float duration = audioClip.length;

        while (audioSource.isPlaying)
        {
            progressCircle.fillAmount = audioSource.time / duration;
            yield return null;
        }

        progressCircle.fillAmount = 1f;
    }

    public void OnDescriptionChanged(string newText)
    {
        if (currentSelected != null)
        {
            currentSelected.collectedAudioClip.notes = newText;
        }
    }
}
