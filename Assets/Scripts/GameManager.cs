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
        // Singleton 初始化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 初始化状态实例（也可以外部传入）
        dayState = new LightState();
        nightState = new BlackState();
        otherState = new OtherState();
    }

    private void Start()
    {
        SetState(GameStateType.Light); // 默认启动为白天状态
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="stateType">状态类型</param>
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