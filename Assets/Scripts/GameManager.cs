using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GameStateType
{
    Light,
    Dark,
    Other
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private IGameState currentState;

    private LightState dayState;
    private BlackState nightState;
    private OtherState otherState;

    private void Awake()
    {
        // Singleton ��ʼ��
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // ��ʼ��״̬ʵ����Ҳ�����ⲿ���룩
        dayState = new LightState();
        nightState = new BlackState();
        otherState = new OtherState();
    }

    private void Start()
    {
        SetState(GameStateType.Light); // Ĭ������Ϊ����״̬
    }

    /// <summary>
    /// �л�״̬
    /// </summary>
    /// <param name="stateType">״̬����</param>
    public void SetState(GameStateType stateType)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        switch (stateType)
        {
            case GameStateType.Light:
                currentState = dayState;
                break;
            case GameStateType.Dark:
                currentState = nightState;
                break;
            case GameStateType.Other:
                currentState = otherState;
                break;
        }

        // Debug.Log($"Enter {currentState} State");
        currentState?.Enter();
    }
}


public interface IGameState
{
    void Enter();
    void Exit();
}

public class LightState : IGameState
{
    public void Enter()
    {
        UIManager.Instance.ShowLightUI();
        // PlayerMovementSwitcher.Instance.SwitchToSimple();
        Debug.Log("Entered Day State");
    }

    public void Exit()
    {
        UIManager.Instance.HideLightUI();
        Debug.Log("Exited Day State");
    }
}
public class BlackState : IGameState
{
    public void Enter()
    {
        Debug.Log("Entered Night State");
        UIManager.Instance.EnableDarkUIMode();
        // PlayerMovementSwitcher.Instance.SwitchToFPS();
    }

    public void Exit()
    {
        Debug.Log("Exiting Night State");
        UIManager.Instance.DisableDarkUIMode();
        UIManager.Instance.HideDarkUI();
    }
}

public class OtherState : IGameState
{
    public void Enter()
    {
        Debug.Log("Entered Other State");
        UIManager.Instance.ShowOtherUI();
    }

    public void Exit()
    {
        Debug.Log("Exiting Other State");
        UIManager.Instance.HideOtherUI();
    }
}