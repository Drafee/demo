using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public abstract class AnimationSequencePlayer : MonoBehaviour
{
    // Start is called before the first frame update

    protected virtual void StartSequence()
    {
        Sequence seq = DOTween.Sequence();
        seq.OnComplete(() =>
        {
            LevelFlowExecutor.Instance.OnAnimationComplete();
        });
    }

    public void PlaySequence() {
        StartSequence();
    }


}
