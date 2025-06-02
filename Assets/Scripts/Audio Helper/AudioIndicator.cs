using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioIndicator : MonoBehaviour
{
    public RawAudioData rawAudioData;

    public RawAudioData GetRawAudioData() {
        return rawAudioData;
    }

    public void OnAbsorbed() {
        Debug.Log("Absorbed");
        LevelFlowExecutor.Instance.AddAudioClip(rawAudioData);
    
    }
}
