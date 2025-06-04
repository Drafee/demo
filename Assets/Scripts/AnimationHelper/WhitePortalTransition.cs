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
    public float topTargetY = 100f;     // �� 200 �Ƶ� 100�����м俿
    public float bottomTargetY = -100f; // �� -200 �Ƶ� -100�����м俿
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
        Debug.Log("StartSequence ִ����");

        // Null ��飨��ѡ���Ƽ���
        if (whiteFade == null) {
            whiteFade = GameObject.Find("WhiteFade").GetComponent<Image>();
        }
        if (topBar == null || bottomBar == null || whiteFade == null)
        {
            Debug.LogError("UI Ԫ��δ��ȷ�󶨣�");
            return;
        }

        Sequence seq = DOTween.Sequence();

        seq.OnComplete(() =>
        {
            Debug.Log("Sequence ��� ?");
            LevelFlowExecutor.Instance.OnAnimationComplete();
        });

        // 0. ������ҿ���
        seq.AppendCallback(() =>
        {
            Debug.Log("Step 0: ������ҿ���");
            PlayerMovementSwitcher.Instance.FreezeMove();
            UIManager.Instance.HidePressFReminder();
        });

        // 1. �պ����� Bar
        seq.AppendCallback(() => Debug.Log("Step 1: ��ɫ Bar ����"));
        seq.Append(topBar.DOAnchorPosY(topTargetY, barCloseDuration));
        seq.Join(bottomBar.DOAnchorPosY(bottomTargetY, barCloseDuration));

        // 2. ��Ļ����
        seq.AppendCallback(() => Debug.Log("Step 2: ��Ļ����"));
        seq.Append(whiteFade.DOFade(1f, fadeDuration));

        // 3. ����ͣ���������� & ����
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() =>
        {
            Debug.Log("Step 3.1: �����Ŷ���");
            DialogueManager.Instance.DestroyDoorsObjects();
        });
        seq.AppendCallback(() =>
        {
            Debug.Log("Step 3.2: �л� Player ��Ŀ��λ��");
            PlayerMovementSwitcher.Instance.SwitchToFPS(LevelFlowExecutor.Instance.playerTransformBlack);
        });

        // 4. ������ Bar + ��Ļ����
        seq.AppendCallback(() => Debug.Log("Step 4: ������ Bar + ��Ļ����"));
        seq.Append(topBar.DOAnchorPosY(288f, barCloseDuration));
        seq.Join(bottomBar.DOAnchorPosY(-288f, barCloseDuration));
        seq.Join(whiteFade.DOFade(0f, fadeDuration));

        // 5. �ָ�����
        seq.AppendCallback(() =>
        {
            Debug.Log("Step 5: �ָ���ҿ���");
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
                Debug.LogWarning("δ֪�ؿ���" + currentLevel);
                break;
        }
    }
    [ContextMenu("Test Portal Sequence")]
    public void TestPortalSequence()
    {
        Debug.Log("��� TestPortalSequence");
        PlaySequence();
    }
}