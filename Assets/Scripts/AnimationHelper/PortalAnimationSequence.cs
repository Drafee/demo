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
    public List<GameObject> doors;
    public int currentIndex = 0;
    public Transform doorTargetPoint;
    public Transform player;
    public MonoBehaviour playerControlScript;

    [Header("Transition Settings")]
    public Vector3 newPlayerPosition;
    public float topTargetY = 100f;     // �� 200 �Ƶ� 100�����м俿
    public float bottomTargetY = -100f; // �� -200 �Ƶ� -100�����м俿
    public float barCloseDuration = 1f;
    public float doorAppearDuration = 1f;
    public float playerWalkDuration = 2f;
    public float fadeDuration = 1f;

    protected override void StartSequence()
    {
        Sequence seq = DOTween.Sequence();
        seq.OnComplete(() =>
        {
            Debug.Log("������");
            LevelFlowExecutor.Instance.OnAnimationComplete();
        });

        // 1. �պ����� Bar
        seq.Append(topBar.DOAnchorPosY(topTargetY, barCloseDuration));
        seq.Join(bottomBar.DOAnchorPosY(bottomTargetY, barCloseDuration));

        // 2. �������Χ 2 �׿�λ������
        player = PlayerMovementSwitcher.Instance.GetCurrentPlayerTransform();

        Vector3 doorPos = doors[currentIndex].transform.position;

        doors[currentIndex].SetActive(true);

        // 3. �ų��ֶ���

        // 4. ���ת����
        Quaternion lookRot = Quaternion.LookRotation(doors[currentIndex].transform.position - player.position);
        seq.Append(player.DORotateQuaternion(lookRot, 1f));

        // 5. �������
        seq.AppendCallback(() => PlayerMovementSwitcher.Instance.FreezeMove());
        seq.Append(player.DOMove(doorPos, playerWalkDuration));

        // 6. ����
        seq.Join(whiteFade.DOFade(1f, fadeDuration));

        // 7. ���͵���λ��
        // �л� Player ��controller
        seq.AppendCallback(() => PlayerMovementSwitcher.Instance.SwitchToSimple(LevelFlowExecutor.Instance.playerTransformWhite));

        // 8. ������ Bar + ��������
        seq.Append(topBar.DOAnchorPosY(288f, barCloseDuration));
        seq.Join(bottomBar.DOAnchorPosY(-288f, barCloseDuration));
        seq.Join(whiteFade.DOFade(0f, fadeDuration));

        // 9. �ָ�����
        seq.AppendCallback(() => PlayerMovementSwitcher.Instance.ReleaseMove());
        currentIndex++;
    }

    [ContextMenu("Test Portal Sequence")]
    public void TestPortalSequence()
    {
        PlaySequence();
    }
}