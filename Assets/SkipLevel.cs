using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipLevel : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) {
            SkipLevel1();
        }
    }
    public void SkipLevel1() {
        LevelFlowExecutor.Instance.OnAnimationComplete();
    }
}
