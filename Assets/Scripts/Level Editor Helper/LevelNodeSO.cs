using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelFlow", menuName = "LevelFlow/Graph")]
public class LevelFlowGraphData : ScriptableObject
{
    public List<NodeData> nodes = new List<NodeData>();
    public List<ConnectionData> connections = new List<ConnectionData>();
}

[System.Serializable]
public class NodeData
{
    public string guid;
    public string type; // 节点类型名称
    public Vector2 position;

    // 不同节点类型的特定数据
    public string tagValue;      // For NarrativeNode
    public string triggerValue;  // For AnimationNode  
    public string sceneName;     // For SceneTransitionNode
    public string title;         // 节点标题
}

[System.Serializable]
public class ConnectionData
{
    public string fromNodeGuid;
    public string fromPortName;
    public string toNodeGuid;
    public string toPortName;
}
