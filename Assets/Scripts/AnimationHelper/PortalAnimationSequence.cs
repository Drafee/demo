using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PortalTransition : AnimationSequencePlayer
{
    [Header("UI Elements")]
    public RectTransform topBar;
    public RectTransform bottomBar;
    public Image whiteFade;

    [Header("Scene Elements")]
    public GameObject door;
    public Transform doorTargetPoint;
    public Transform player;
    public MonoBehaviour playerControlScript;

    [Header("Transition Settings")]
    public Vector3 newPlayerPosition;
    public float topTargetY = 100f;     // 从 200 移到 100，往中间靠
    public float bottomTargetY = -100f; // 从 -200 移到 -100，往中间靠
    public float barCloseDuration = 1f;
    public float doorAppearDuration = 1f;
    public float playerWalkDuration = 2f;
    public float fadeDuration = 1f;

    protected override void StartSequence()
    {
        Sequence seq = DOTween.Sequence();
        seq.OnComplete(() =>
        {
            LevelFlowExecutor.Instance.OnAnimationComplete();
        });

        // 1. 闭合上下 Bar
        seq.Append(topBar.DOAnchorPosY(topTargetY, barCloseDuration));
        seq.Join(bottomBar.DOAnchorPosY(bottomTargetY, barCloseDuration));

        // 2. 找玩家周围 2 米空位生成门
        player = PlayerMovementSwitcher.Instance.GetCurrentPlayerTransform();

        Vector3 doorPos = door.transform.position;

        door.SetActive(true);

        // 3. 门出现动画

        // 4. 玩家转向门
        Quaternion lookRot = Quaternion.LookRotation(door.transform.position - player.position);
        seq.Append(player.DORotateQuaternion(lookRot, 1f));

        // 5. 玩家行走
        seq.AppendCallback(() => PlayerMovementSwitcher.Instance.FreezeMove());
        seq.Append(player.DOMove(doorPos, playerWalkDuration));

        // 6. 白屏
        seq.Join(whiteFade.DOFade(1f, fadeDuration));

        // 7. 传送到新位置
        // 切换 Player 的controller
        seq.AppendCallback(() => PlayerMovementSwitcher.Instance.SwitchToSimple(LevelFlowExecutor.Instance.playerTransformWhite));

        // 8. 打开上下 Bar + 白屏消退
        seq.Append(topBar.DOAnchorPosY(288f, barCloseDuration));
        seq.Join(bottomBar.DOAnchorPosY(-288f, barCloseDuration));
        seq.Join(whiteFade.DOFade(0f, fadeDuration));

        // 9. 恢复控制
        seq.AppendCallback(() => PlayerMovementSwitcher.Instance.ReleaseMove());
    }

    [ContextMenu("Test Portal Sequence")]
    public void TestPortalSequence()
    {
        PlaySequence();
    }
}