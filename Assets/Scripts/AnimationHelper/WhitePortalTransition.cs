using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WhitePortalTransition : AnimationSequencePlayer
{
    [Header("UI Elements")]
    public RectTransform topBar;
    public RectTransform bottomBar;
    public Image whiteFade;

    [Header("Scene Elements")]

    public Transform player;

    [Header("Transition Settings")]
    public Vector3 newPlayerPosition;
    public float topTargetY = 100f;     // 从 200 移到 100，往中间靠
    public float bottomTargetY = -100f; // 从 -200 移到 -100，往中间靠
    public float barCloseDuration = 1f;
    public float doorAppearDuration = 1f;
    public float playerWalkDuration = 2f;
    public float fadeDuration = 1f;

    public List<GameObject> blockers;
    public List<GameObject> volumetricLight;
    public Light spotLight;
    public Light derectionalLight;
    public Material skybox;
    public GameObject endNote;

    protected override void StartSequence()
    {
        Debug.Log("StartSequence 执行了");

        // Null 检查（可选但推荐）
        if (whiteFade == null) {
            whiteFade = GameObject.Find("WhiteFade").GetComponent<Image>();
        }
        if (topBar == null || bottomBar == null || whiteFade == null)
        {
            Debug.LogError("UI 元素未正确绑定！");
            return;
        }

        Sequence seq = DOTween.Sequence();

        seq.OnComplete(() =>
        {
            Debug.Log("Sequence 完成 ?");
            LevelFlowExecutor.Instance.OnAnimationComplete();
        });

        // 0. 冻结玩家控制
        seq.AppendCallback(() =>
        {
            Debug.Log("Step 0: 冻结玩家控制");
            PlayerMovementSwitcher.Instance.FreezeMove();
            UIManager.Instance.HidePressFReminder();
        });

        // 1. 闭合上下 Bar
        seq.AppendCallback(() => Debug.Log("Step 1: 黑色 Bar 收起"));
        seq.Append(topBar.DOAnchorPosY(topTargetY, barCloseDuration));
        seq.Join(bottomBar.DOAnchorPosY(bottomTargetY, barCloseDuration));

        // 2. 白幕渐显
        seq.AppendCallback(() => Debug.Log("Step 2: 白幕渐显"));
        seq.Append(whiteFade.DOFade(1f, fadeDuration));

        // 3. 稍作停顿再销毁门 & 传送
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() =>
        {
            Debug.Log("Step 3.1: 销毁门对象");
            DialogueManager.Instance.DestroyDoorsObjects();
        });
        seq.AppendCallback(() =>
        {
            Debug.Log("Step 3.2: 切换 Player 到目标位置");
            PlayerMovementSwitcher.Instance.SwitchToFPS(LevelFlowExecutor.Instance.playerTransformBlack);
        });

        // 4. 打开上下 Bar + 白幕消退
        seq.AppendCallback(() => Debug.Log("Step 4: 打开上下 Bar + 白幕渐隐"));
        seq.Append(topBar.DOAnchorPosY(288f, barCloseDuration));
        seq.Join(bottomBar.DOAnchorPosY(-288f, barCloseDuration));
        seq.Join(whiteFade.DOFade(0f, fadeDuration));

        // 5. 恢复控制
        seq.AppendCallback(() =>
        {
            Debug.Log("Step 5: 恢复玩家控制");
            PlayerMovementSwitcher.Instance.ReleaseMove();
            UpdateLevel();
        });
    }

    public void UpdateLevel() {

        int currentLevel = LevelFlowExecutor.Instance.currentLevel;

        switch (currentLevel)
        {
            case 1:
                volumetricLight[currentLevel - 1].SetActive(true);
                blockers[currentLevel - 1].SetActive(false);
                Debug.Log("Directional Light: " + derectionalLight);
                Debug.Log("Spot Light: " + spotLight);
                derectionalLight.intensity = 0.2f;
                spotLight.range = 10f;
                break;

            case 2:
                blockers[currentLevel - 1].SetActive(false);
                volumetricLight[currentLevel - 1].SetActive(true);
                derectionalLight.intensity = 0.6f;
                spotLight.range = 105f;
                break;

            case 3:
                volumetricLight[currentLevel - 1].SetActive(true);
                derectionalLight.intensity = 0.8f;
                Camera playerCamera = PlayerMovementSwitcher.Instance.GetCurrentPlayerTransform().GetComponentInChildren<Camera>();

                if (playerCamera != null)
                {
                    playerCamera.clearFlags = CameraClearFlags.Skybox;
                    RenderSettings.skybox = skybox;
                    DynamicGI.UpdateEnvironment();
                }

                endNote.SetActive(true);
                break;

            default:
                Debug.LogWarning("未知关卡：" + currentLevel);
                break;
        }
    }
    [ContextMenu("Test Portal Sequence")]
    public void TestPortalSequence()
    {
        Debug.Log("点击 TestPortalSequence");
        PlaySequence();
    }
}