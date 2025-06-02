using UnityEngine;

[CreateAssetMenu(fileName = "NewAudioData", menuName = "Custom/Audio Data", order = 1)]
public class RawAudioData : ScriptableObject
{
    public string soundName;
    public string id;
    public AudioClip audioClip;
}

[System.Serializable]
public class CollectAudioClip // Used in audio Bag
{
    public RawAudioData audioData;
    public string notes;

    public CollectAudioClip(RawAudioData r) {
        audioData = r;
        notes = "";
    }
}

[System.Serializable]
public class AudioClipItem
{
    public CollectAudioClip collectAudioClip;
    public GameObject iconGO; // 当前Icon物体引用（方便操作）

    public AudioClipItem(CollectAudioClip r)
    {
        collectAudioClip = r;
        iconGO = null ;
    }
}