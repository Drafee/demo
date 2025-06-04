using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class AnimationSequencePlayer : MonoBehaviour
{
    // Start is called before the first frame update

    protected virtual void StartSequence()
    {

    }

    public void PlaySequence() {
        StartSequence();
    }


}
