using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class LevelFlowExecutor : MonoBehaviour
{
    [Header("Flow Graph Settings")]
    public LevelFlowGraphData flowGraph;

    [Header("Events")]
    public UnityEngine.Events.UnityEvent OnFlowStart;
    public UnityEngine.Events.UnityEvent OnFlowComplete;
    public UnityEngine.Events.UnityEvent<string> OnSceneTransition;

    [Header("Debug")]
    public bool debugMode = true;

    private Dictionary<string, NodeData> nodeMap;
    private Dictionary<string, List<string>> connectionMap; // from node GUID -> to node GUIDs
    private NodeData currentNode;
    private bool isExecuting = false;

    public string audioAnswer;
    public Transform playerTransformWhite;
    public Transform playerTransformBlack;

    // 执行状态
    public bool IsExecuting => isExecuting;
    public NodeData CurrentNode => currentNode;
    public int currentLevel;
    public static LevelFlowExecutor Instance { get; private set; }

    public List<CollectAudioClip> collectedAudioClips = new List<CollectAudioClip>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }
    private void Start()
    {
        InitializeFlow();
        StartFlow();
    }

    public void InitializeFlow()
    {
        if (flowGraph == null)
        {
            Debug.LogError("LevelFlowExecutor: No flow graph assigned!");
            return;
        }

        BuildNodeMaps();

    }

    private void BuildNodeMaps()
    {
        nodeMap = new Dictionary<string, NodeData>();
        connectionMap = new Dictionary<string, List<string>>();

        // 构建节点映射
        foreach (var node in flowGraph.nodes)
        {
            nodeMap[node.guid] = node;
            connectionMap[node.guid] = new List<string>();
        }

        // 构建连接映射
        foreach (var connection in flowGraph.connections)
        {
            if (connectionMap.ContainsKey(connection.fromNodeGuid))
            {
                connectionMap[connection.fromNodeGuid].Add(connection.toNodeGuid);
            }
        }
    }

    public void StartFlow()
    {
        if (isExecuting)
        {
            Debug.LogWarning("Flow is already executing!");
            return;
        }

        var startNode = nodeMap.Values.FirstOrDefault(n => n.type == "StartNode");
        if (startNode == null)
        {
            Debug.LogError("No StartNode found in flow graph!");
            return;
        }

        isExecuting = true;
        OnFlowStart?.Invoke();

        ExecuteNode(startNode);
    }

    public void StopFlow()
    {
        isExecuting = false;
        currentNode = null;

        if (debugMode)
        {
            Debug.Log("Flow execution stopped.");
        }
    }

    private void ExecuteNode(NodeData node)
    {
        if (!isExecuting) return;

        currentNode = node;

        if (debugMode)
        {
            Debug.Log($"Executing node: {node.type} - {node.title}");
        }

        switch (node.type)
        {
            case "StartNode":
                ExecuteStartNode(node);
                break;
            case "EndNode":
                ExecuteEndNode(node);
                break;
            case "NarrativeNode":
                ExecuteNarrativeNode(node);
                break;
            case "AnimationNode":
                ExecuteAnimationNode(node);
                break;
            case "AnimationScriptNode":
                ExecuteAnimationScriptNode(node);
                break;
            case "SceneTransitionNode":
                ExecuteSceneTransitionNode(node);
                break;
            case "AudioLevelNode":
                ExecuteAudioLevelNode(node);
                break;
            default:
                Debug.LogWarning($"Unknown node type: {node.type}");
                MoveToNextNode(node);
                break;
        }
    }

    private void ExecuteStartNode(NodeData node)
    {
        // Start节点直接进入下一个节点
        MoveToNextNode(node);
    }

    private void ExecuteEndNode(NodeData node)
    {
        // End节点结束流程
        isExecuting = false;
        OnFlowComplete?.Invoke();
    }

    private void ExecuteNarrativeNode(NodeData node)
    {
        // 执行对话逻辑
        if (!string.IsNullOrEmpty(node.tagValue))
        {
            // 这里可以触发对话系统
            var dialogueManager = FindObjectOfType<DialogueManager>();
            if (dialogueManager != null)
            {
                if (node.isBrightArea)
                {
                    dialogueManager.StartDialogueWhiteArea(node.tagValue);
                }
                else {
                    dialogueManager.StartDialogue(node.tagValue, () => {
                        // 对话完成后继续下一个节点
                        MoveToNextNode(node);
                    });
                }

            }
            else
            {
                Debug.Log($"Narrative: {node.tagValue}");
                // 模拟对话时间后自动继续
                Invoke("MoveToNextNode", 2f);
            }
        }
        else
        {
            MoveToNextNode(node);
        }
    }

    private void ExecuteAnimationNode(NodeData node)
    {
        // 执行动画逻辑
        if (!string.IsNullOrEmpty(node.triggerValue))
        {
            var animator = node.animatorValue;
            if (animator != null)
            {
                animator.SetTrigger(node.triggerValue);
                // Has call back function at the end of the animation
            }
            else
            {
                Debug.Log($"Animation Trigger: {node.triggerValue}");
                MoveToNextNode(node);
            }
        }
        else
        {
            MoveToNextNode(node);
        }
    }

    private void ExecuteAnimationScriptNode(NodeData node) {
        Debug.Log(node);
        node.animationSequencePlayer.PlaySequence();
    }

    public void OnSequenceComplete() {
        MoveToNextNode(currentNode);
    }

    public void OnAnimationComplete() {
        Debug.Log("runComplete");
        MoveToNextNode(currentNode);
    }
    private void ExecuteSceneTransitionNode(NodeData node)
    {
        // 执行场景切换
        if (!string.IsNullOrEmpty(node.sceneName))
        {
            OnSceneTransition?.Invoke(node.sceneName);

            if (debugMode)
            {
                Debug.Log($"Transitioning to scene: {node.sceneName}");
            }

            // 场景切换会停止当前流程
            isExecuting = false;

            // 加载场景
            SceneManager.LoadScene(node.sceneName);
        }
        else
        {
            Debug.LogWarning("SceneTransitionNode has no scene name specified!");
            MoveToNextNode(node);
        }
    }

    private void ExecuteAudioLevelNode(NodeData node) {
        GameManager.Instance.SetState(GameStateType.Dark);
        currentLevel = node.levelSetting.id;
        audioAnswer = string.Join("", node.levelSetting.audioAnswer.Select(r => r.id));
        collectedAudioClips.Clear();

    }

    public bool CheckAnswer(string a) {
        if (a == audioAnswer) {
            MoveToNextNode(currentNode);
            return true;
        }
        return false;
    }

    public void AddAudioClip(RawAudioData rawAudio) {
        collectedAudioClips.Add(new CollectAudioClip(rawAudio));
    }

    public void ClearAudioClip() { 
        collectedAudioClips.Clear();
    }

    public void RemoveAudioClip(CollectAudioClip rawAudio) {
        if (collectedAudioClips.Contains(rawAudio))
        {
            collectedAudioClips.Remove(rawAudio);
            Debug.Log("Audio clip removed.");
        }
        else
        {
            Debug.LogWarning("Audio clip not found in the list.");
        }
    }

    private void MoveToNextNode(NodeData currentNode)
    {
        // if (!isExecuting) return;

        if (connectionMap.ContainsKey(currentNode.guid))
        {
            var nextNodeGuids = connectionMap[currentNode.guid];

            if (nextNodeGuids.Count > 0)
            {
                // 如果有多个连接，选择第一个（可以扩展为条件选择）
                var nextNodeGuid = nextNodeGuids[0];

                if (nodeMap.ContainsKey(nextNodeGuid))
                {

                    ExecuteNode(nodeMap[nextNodeGuid]);
                }
                else
                {
                    Debug.LogError($"Next node not found: {nextNodeGuid}");
                    StopFlow();
                }
            }
            else
            {
                // 没有下一个节点，流程结束
                Debug.Log("No next node found, ending flow.");
                StopFlow();
            }
        }
        else
        {
            Debug.LogError($"No connections found for node: {currentNode.guid}");
            StopFlow();
        }
    }

    // 公共方法：手动继续到下一个节点（用于需要外部确认的情况）
    public void ContinueToNext()
    {
        if (currentNode != null && isExecuting)
        {
            MoveToNextNode(currentNode);
        }
    }

    // 公共方法：跳转到指定节点
    public void JumpToNode(string nodeGuid)
    {
        if (nodeMap.ContainsKey(nodeGuid))
        {
            ExecuteNode(nodeMap[nodeGuid]);
        }
        else
        {
            Debug.LogError($"Node not found: {nodeGuid}");
        }
    }

    // 获取当前节点的所有可能的下一个节点
    public List<NodeData> GetNextNodes()
    {
        if (currentNode == null) return new List<NodeData>();

        var nextNodes = new List<NodeData>();
        if (connectionMap.ContainsKey(currentNode.guid))
        {
            foreach (var nextGuid in connectionMap[currentNode.guid])
            {
                if (nodeMap.ContainsKey(nextGuid))
                {
                    nextNodes.Add(nodeMap[nextGuid]);
                }
            }
        }
        return nextNodes;
    }
}