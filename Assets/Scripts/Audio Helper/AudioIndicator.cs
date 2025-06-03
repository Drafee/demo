using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioIndicator : MonoBehaviour
{
    public RawAudioData rawAudioData;

    public bool hasBeenCollected;

    private void Start()
    {
        hasBeenCollected = false;
    }
    public RawAudioData GetRawAudioData() {
        return rawAudioData;
    }

    public void OnAbsorbed() {
        if (hasBeenCollected) {
            return;
        }
        if (!hasBeenCollected) { 
            hasBeenCollected=true;
        }
        Debug.Log("Absorbed");
        LevelFlowExecutor.Instance.AddAudioClip(rawAudioData);
        GetComponent<AudioSource>().Stop();
    
    }
}
