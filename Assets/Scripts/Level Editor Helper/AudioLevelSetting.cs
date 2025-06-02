using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewAudioLevel", menuName = "Custom/Audio Level Data", order = 2)]

public class AudioLevelSetting : ScriptableObject
{
    public int id;
    public List<RawAudioData> audioAnswer;
}
