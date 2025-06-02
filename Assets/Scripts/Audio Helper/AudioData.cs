using UnityEngine;

[CreateAssetMenu(fileName = "NewAudioData", menuName = "Custom/Audio Data", order = 1)]
public class RawAudioData : ScriptableObject
{
    public string soundName;
    public string id;
    public AudioClip audioClip;
}

[System.Serializable]
public class CollectAudioClip
{
    public RawAudioData audioData;
    public string notes;

    public CollectAudioClip(RawAudioData r) {
        audioData = r;
        notes = "";
    }
}