using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPoolManager : MonoBehaviour
{
    public static AudioPoolManager Instance { get; private set; }
    [Header("Pool Settings")]
    public AudioSource audioPrefab;
    public int initialSize = 10;
    public bool autoExpand = true;

    private List<AudioSource> pool = new List<AudioSource>();

    void Awake()
    {
        // Singleton 初始化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 初始化池
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewAudioSource();
        }
    }

    private AudioSource CreateNewAudioSource()
    {
        AudioSource audioSource = Instantiate(audioPrefab, transform);
        audioSource.gameObject.SetActive(false);
        pool.Add(audioSource);
        return audioSource;
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (var audio in pool)
        {
            if (!audio.isPlaying)
            {
                return audio;
            }
        }

        return autoExpand ? CreateNewAudioSource() : null;
    }

    public AudioSource PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        AudioSource audio = GetAvailableAudioSource();
        if (audio == null)
        {
            Debug.LogWarning("No available AudioSource in pool.");
            return null;
        }

        audio.transform.position = position;
        audio.clip = clip;
        audio.volume = volume;
        audio.gameObject.SetActive(true);
        audio.Play();

        StartCoroutine(DisableAfterPlaying(audio));
        return audio;
    }

    private System.Collections.IEnumerator DisableAfterPlaying(AudioSource audio)
    {
        yield return new WaitForSeconds(audio.clip.length);
        audio.Stop();
        audio.gameObject.SetActive(false);
    }
}
