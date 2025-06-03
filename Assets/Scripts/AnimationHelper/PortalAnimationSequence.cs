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
    public float barCloseDuration = 1f;
    public float doorAppearDuration = 1f;
    public float playerWalkDuration = 2f;
    public float fadeDuration = 1f;

    protected override void StartSequence()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(topBar.DOAnchorPosY(0, barCloseDuration));
        seq.Join(bottomBar.DOAnchorPosY(0, barCloseDuration));

        seq.AppendCallback(() => {
            door.SetActive(true);
            door.transform.localScale = Vector3.zero;
        });
        seq.Append(door.transform.DOScale(Vector3.one, doorAppearDuration));

        seq.AppendCallback(() => playerControlScript.enabled = false);
        seq.Append(player.DOMove(doorTargetPoint.position, playerWalkDuration));

        seq.Append(whiteFade.DOFade(1f, fadeDuration));

        seq.AppendCallback(() => player.position = newPlayerPosition);

        seq.Append(topBar.DOAnchorPosY(200f, barCloseDuration));
        seq.Join(bottomBar.DOAnchorPosY(-200f, barCloseDuration));
        seq.Join(whiteFade.DOFade(0f, fadeDuration));

        seq.AppendCallback(() => playerControlScript.enabled = true);
    }
}