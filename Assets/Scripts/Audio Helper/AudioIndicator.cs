using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioIndicator : MonoBehaviour
{
    public RawAudioData rawAudioData;
    public bool hasBeenCollected;

    private AudioSource audioSource;
    private Coroutine playbackRoutine;

    private void Start()
    {
        hasBeenCollected = false;
        audioSource = GetComponent<AudioSource>();

        if (rawAudioData != null && rawAudioData.audioClip != null)
        {
            audioSource.clip = rawAudioData.audioClip;
            audioSource.loop = false;
            playbackRoutine = StartCoroutine(PlayRandomly());
        }
    }

    public RawAudioData GetRawAudioData()
    {
        return rawAudioData;
    }

    public void OnAbsorbed()
    {
        if (hasBeenCollected)
            return;

        hasBeenCollected = true;
        Debug.Log("Absorbed");

        LevelFlowExecutor.Instance.AddAudioClip(rawAudioData);
        if (playbackRoutine != null)
            StopCoroutine(playbackRoutine);

        audioSource.Stop();
    }

    private IEnumerator PlayRandomly()
    {
        while (!hasBeenCollected)
        {
            audioSource.Play();
            // �ȴ���ǰ��Ƶ�������
            yield return new WaitForSeconds(audioSource.clip.length);

            // Ȼ������ȴ� 3��5 ��
            yield return new WaitForSeconds(Random.Range(1f, 1.5f));
        }
    }
}
