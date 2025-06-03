
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.SceneManagement;

using System.Collections.Generic;
using System.Linq; 

public class LevelFlowEditor : EditorWindow
{
    private LevelFlowGraphView graphView;
    private StartNode startNode;
    private EndNode endNode;
    private LevelFlowGraphData currentGraphData;
    private bool shouldLoadAssetOnEnable = false;
    private LevelFlowGraphData assetToLoad;

    [MenuItem("Window/Level Flow Editor")]
    public static void OpenWindow()
    {
        LevelFlowEditor wnd = GetWindow<LevelFlowEditor>();
        wnd.titleContent = new GUIContent("Level Flow Editor");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();

        // if there are values store to open the SO, do it
        if (shouldLoadAssetOnEnable && assetToLoad != null)
        {
            LoadGraphFromAsset(assetToLoad);
            currentGraphData = assetToLoad;
            shouldLoadAssetOnEnable = false;
            assetToLoad = null;
        }
        else
        {
            CreateStartEndNodes();
        }
    }

    // Create startNode and end Node in the begining
    private void CreateStartEndNodes()
    {
        if (startNode == null)
        {
            startNode = new StartNode();
            startNode.title = "Start Node";
            startNode.SetPosition(new Rect(100, 100, 200, 150));
            graphView.AddElement(startNode);
            startNode.Draw();
        }

        if (endNode == null)
        {
            endNode = new EndNode();
            endNode.title = "End Node";
            endNode.SetPosition(new Rect(400, 100, 200, 150));
            graphView.AddElement(endNode);
            endNode.Draw();
        }
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }

    private void ConstructGraphView()
    {
        graphView = new LevelFlowGraphView
        {
            name = "Level Flow Graph"
        };
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    // Generate the tool bar 
    private void GenerateToolbar()
    {
        var toolbar = new VisualElement();

        toolbar.style.flexDirection = FlexDirection.Row;
        toolbar.style.flexWrap = Wrap.Wrap;
        toolbar.style.backgroundColor = new StyleColor(Color.gray);
        toolbar.style.paddingTop = 5;
        toolbar.style.paddingBottom = 5;

        AddToolbarButton(toolbar, "Add Dialogue Node", NodeType.Dialogue);
        AddToolbarButton(toolbar, "Add Animation Node", NodeType.Animation);
        AddToolbarButton(toolbar, "Add Animation Script Node", NodeType.AnimationScript);
        AddToolbarButton(toolbar, "Add Scene Transition Node", NodeType.SceneTransition);
        AddToolbarButton(toolbar, "Add Narrative Node", NodeType.Narrative);
        AddToolbarButton(toolbar, "Add Audio Level Node", NodeType.AudioLevel);

        // Add a sagve button
        var saveButton = new Button(() =>
        {
            if (currentGraphData != null)
            {
                // if it's saving current data, then rewrite it
                string assetPath = AssetDatabase.GetAssetPath(currentGraphData);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    graphView.UpdateExistingAsset(currentGraphData);
                    EditorUtility.SetDirty(currentGraphData);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Updated existing graph: " + assetPath);
                    return;
                }
            }
            string path = EditorUtility.SaveFilePanelInProject("Save Flow Graph", "LevelFlowGraph", "asset", "");
            if (!string.IsNullOrEmpty(path))
            {
                currentGraphData = graphView.SaveGraphToAsset(path);
                Debug.Log("Saved graph to: " + path);
            }
        })
        {
            text = "Save Flow Asset"
        };

        toolbar.Add(saveButton);
    
        var clearButton = new Button(() =>
        {
            if (EditorUtility.DisplayDialog("Clear Graph", "Are you sure you want to clear the entire graph?", "Yes", "No"))
            {
                ClearGraph();
            }
        })
        {
            text = "Clear Graph"
        };
        toolbar.Add(clearButton);

        rootVisualElement.Add(toolbar);
    }

    private void AddToolbarButton(VisualElement toolbar, string label, NodeType type)
    {
        var button = new Button(() => graphView.CreateNode(type));
        button.text = label;
        toolbar.Add(button);
    }

    private void LoadGraphFromAsset(LevelFlowGraphData graphData)
    {
        ClearGraph(false);

        // Dictionary to store created nodes by GUID for connection creation
        Dictionary<string, BaseNode> nodeMap = new Dictionary<string, BaseNode>();

        // Create nodes
        foreach (var nodeData in graphData.nodes)
        {
            BaseNode node = CreateNodeFromData(nodeData);
            if (node != null)
            {
                nodeMap[nodeData.guid] = node;
                graphView.AddElement(node);
            }
        }

        // Create connections
        foreach (var connectionData in graphData.connections)
        {
            if (nodeMap.TryGetValue(connectionData.fromNodeGuid, out BaseNode fromNode) &&
                nodeMap.TryGetValue(connectionData.toNodeGuid, out BaseNode toNode))
            {
                // Find the ports
                Port outputPort = fromNode.outputContainer.Q<Port>();
                Port inputPort = toNode.inputContainer.Q<Port>();

                if (outputPort != null && inputPort != null)
                {
                    var edge = outputPort.ConnectTo(inputPort);
                    graphView.AddElement(edge);
                }
            }
        }
    }

    private BaseNode CreateNodeFromData(NodeData nodeData)
    {
        BaseNode node = null;

        switch (nodeData.type)
        {
            case "StartNode":
                startNode = new StartNode();
                node = startNode;
                break;
            case "EndNode":
                endNode = new EndNode();
                node = endNode;
                break;
            case "NarrativeNode":
                var narrativeNode = new NarrativeNode();
                node = narrativeNode;
                break;
            case "AnimationNode":
                var animationNode = new AnimationNode();
                node = animationNode;
                break;
            case "AnimationScriptNode":
                var animationScriptNode = new AnimationScriptNode();
                node = animationScriptNode;
                break;
            case "SceneTransitionNode":
                var sceneNode = new SceneTransitionNode();
                node = sceneNode;
                break;
            case "AudioLevelNode":
                var audioNode = new AudioLevelNode();
                node = audioNode;
                break;
        }

        if (node != null)
        {
            node.title = !string.IsNullOrEmpty(nodeData.title) ? nodeData.title : nodeData.type;
            node.SetPosition(new Rect(nodeData.position.x, nodeData.position.y, 200, 150));
            node.Draw();

            // Set node-specific data after drawing
            if (node is NarrativeNode narrativeNode && !string.IsNullOrEmpty(nodeData.tagValue))
            {
                narrativeNode.SetDialogueTag(nodeData.tagValue);
            }
            else if (node is AnimationNode animationNode && !string.IsNullOrEmpty(nodeData.triggerValue))
            {
                animationNode.SetTriggerName(nodeData.triggerValue);
                animationNode.SetAnimator(nodeData.animatorValue);
            }
            else if (node is SceneTransitionNode sceneTransitionNode && !string.IsNullOrEmpty(nodeData.sceneName))
            {
                sceneTransitionNode.SetSceneName(nodeData.sceneName);
            }
            else if (node is AudioLevelNode audioLevelNode)
            {
                audioLevelNode.SetAudioLevelData(nodeData.levelSetting);
            }
            else if (node is AnimationScriptNode animationScriptNode)
            {
                animationScriptNode.SetAnimationScript(nodeData.animationSequencePlayer);
            }
        }

        return node;
    }

    private void ClearGraph(bool createStartEndNode = true)
    {
        // Remove all nodes except start and end
        var nodesToRemove = graphView.nodes.OfType<BaseNode>().ToList();
        foreach (var node in nodesToRemove)
        {
            graphView.RemoveElement(node);
        }

        // Remove all edges
        var edgesToRemove = graphView.edges.ToList();
        foreach (var edge in edgesToRemove)
        {
            graphView.RemoveElement(edge);
        }

        // Reset start and end nodes
        startNode = null;
        endNode = null;
        currentGraphData = null;

        // Recreate start and end nodes
        if (createStartEndNode) {
            CreateStartEndNodes();
        }
    }

    public void LoadGraphAsset(LevelFlowGraphData asset)
    {
        if (asset == null) return;

        if (graphView != null)
        {
            LoadGraphFromAsset(asset);
            currentGraphData = asset;
        }
        else
        {
            shouldLoadAssetOnEnable = true;
            assetToLoad = asset;
        }
    }
}

public class LevelFlowGraphView : GraphView
{
    public LevelFlowGraphView()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach(port =>
        {
            // Don't connect to self
            if (startPort.node == port.node) return;

            // Only allow input-to-output or output-to-input connections
            if (startPort.direction != port.direction &&
                startPort.portType == port.portType)
            {
                compatiblePorts.Add(port);
            }
        });

        return compatiblePorts;
    }

    public void CreateNode(NodeType nodeType)
    {
        BaseNode node = null;

        switch (nodeType)
        {
            case NodeType.Dialogue:
                node = new NarrativeNode(); // 或者 DialogueNode
                break;
            case NodeType.Animation:
                node = new AnimationNode();
                break;
            case NodeType.SceneTransition:
                node = new SceneTransitionNode();
                break;
            case NodeType.AudioLevel:
                node = new AudioLevelNode();
                break;
            case NodeType.AnimationScript:
                node = new AnimationScriptNode();
                break;
            default:
                Debug.LogWarning("Unsupported node type");
                return;
        }

        if (node != null)
        {
            node.title = nodeType.ToString() + " Node";
            var x = Random.Range(100, 400);
            var y = Random.Range(150, 250);
            node.SetPosition(new Rect(x, y, 200, 150));
            node.Draw();
            AddElement(node);
        }
    }
    public LevelFlowGraphData SaveGraphToAsset(string assetPath)
    {
        //Create Node and Edge and store it into the SO
        var asset = ScriptableObject.CreateInstance<LevelFlowGraphData>();

        foreach (var node in nodes.OfType<BaseNode>())
        {
            var nodeData = new NodeData
            {
                guid = node.GUID,
                type = node.GetType().Name,
                position = node.GetPosition().position,
                title = node.title
            };

            if (node is NarrativeNode narrativeNode)
                nodeData.tagValue = narrativeNode.DialogueTag;

            if (node is AnimationNode animationNode)
            {
                nodeData.triggerValue = animationNode.TriggerName;
                nodeData.animatorValue = animationNode.TargetAnimator;
            }

            if (node is SceneTransitionNode sceneNode)
                nodeData.sceneName = sceneNode.SceneName;

            if (node is AudioLevelNode audioLevelNode)
                nodeData.levelSetting = audioLevelNode.audioLevelSetting;

            if (node is AnimationScriptNode animationScriptNode)
                nodeData.animationSequencePlayer = animationScriptNode.animationSequencePlayer;

            asset.nodes.Add(nodeData);
        }

        foreach (var edge in edges)
        {
            if (edge.input.node is BaseNode inputNode && edge.output.node is BaseNode outputNode)
            {
                var connection = new ConnectionData
                {
                    fromNodeGuid = outputNode.GUID,
                    fromPortName = edge.output.portName,
                    toNodeGuid = inputNode.GUID,
                    toPortName = edge.input.portName
                };
                asset.connections.Add(connection);
            }
        }

        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        return asset;
    }

    public void UpdateExistingAsset(LevelFlowGraphData asset)
    {
        // Clear Currnet Data
        asset.nodes.Clear();
        asset.connections.Clear();

        // Rewrite Data
        foreach (var node in nodes.OfType<BaseNode>())
        {
            var nodeData = new NodeData
            {
                guid = node.GUID,
                type = node.GetType().Name,
                position = node.GetPosition().position,
                title = node.title
            };

            if (node is NarrativeNode narrativeNode)
                nodeData.tagValue = narrativeNode.DialogueTag;

            if (node is AnimationNode animationNode)
            {
                nodeData.triggerValue = animationNode.TriggerName;
                nodeData.animatorValue = animationNode.TargetAnimator;
            }

            if (node is SceneTransitionNode sceneNode)
                nodeData.sceneName = sceneNode.SceneName;

            if (node is AudioLevelNode audioLevelNode)
                nodeData.levelSetting = audioLevelNode.audioLevelSetting;

            if (node is AnimationScriptNode animationScriptNode)
                nodeData.animationSequencePlayer = animationScriptNode.animationSequencePlayer;

            asset.nodes.Add(nodeData);
        }

        foreach (var edge in edges)
        {
            if (edge.input.node is BaseNode inputNode && edge.output.node is BaseNode outputNode)
            {
                var connection = new ConnectionData
                {
                    fromNodeGuid = outputNode.GUID,
                    fromPortName = edge.output.portName,
                    toNodeGuid = inputNode.GUID,
                    toPortName = edge.input.portName
                };
                asset.connections.Add(connection);
            }
        }
    }

}

/*
public abstract class BaseNode : Node
{
    public string GUID;
    public virtual void Initialize(string nodeName)
    {
        title = nodeName;
        GUID = System.Guid.NewGuid().ToString();
    }

    public abstract void Draw();
}

public class NarrativeNode : BaseNode
{
    private TextField tagField;
    public string DialogueTag => tagField.value;

    public override void Draw()
    {
        title = "Narrative Node";

        var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        inputPort.portName = "In";
        inputContainer.Add(inputPort);

        var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
        outputPort.portName = "Out";
        outputContainer.Add(outputPort);

        // Add Dialogue Tag
        tagField = new TextField("Tag");
        tagField.value = "MainCharacterIntro";
        extensionContainer.Add(tagField);

        RefreshExpandedState();
        RefreshPorts();
    }
}
public class AnimationNode : BaseNode
{
    private ObjectField animatorField;
    private TextField triggerField;

    public Animator AnimatorRef => animatorField.value as Animator;
    public string TriggerName => triggerField.value;

    public override void Draw()
    {
        title = "Animation Node";

        var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        inputPort.portName = "In";
        inputContainer.Add(inputPort);

        var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
        outputPort.portName = "Out";
        outputContainer.Add(outputPort);

        // 拖入 Animator 引用
        animatorField = new ObjectField("Animator")
        {
            objectType = typeof(Animator),
            allowSceneObjects = true
        };
        extensionContainer.Add(animatorField);

        // 输入 Trigger 名称
        triggerField = new TextField("Trigger");
        triggerField.value = "PlayIntro";
        extensionContainer.Add(triggerField);

        RefreshExpandedState();
        RefreshPorts();
    }


}

public class SceneTransitionNode : BaseNode
{
    private PopupField<string> sceneDropdown;

    // 可选的场景列表
    private List<string> sceneNames;

    public string SceneName => sceneDropdown.value;

    public override void Draw()
    {
        title = "Scene Transition Node";

        var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        inputPort.portName = "In";
        inputContainer.Add(inputPort);

        var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
        outputPort.portName = "Out";
        outputContainer.Add(outputPort);

        // 获取Editor Build Settings中所有可用场景名
        sceneNames = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => System.IO.Path.GetFileNameWithoutExtension(s.path))
            .ToList();

        if (sceneNames.Count == 0)
        {
            sceneNames.Add("No Scenes In Build Settings");
        }

        sceneDropdown = new PopupField<string>("Target Scene", sceneNames, 0);
        extensionContainer.Add(sceneDropdown);

        RefreshExpandedState();
        RefreshPorts();
    }

    public void Execute()
    {
        if (!string.IsNullOrEmpty(SceneName) && SceneName != "No Scenes In Build Settings")
        {
            Debug.Log($"切换场景到 {SceneName}");
            SceneManager.LoadScene(SceneName);
        }
        else
        {
            Debug.LogWarning("未设置有效的目标场景名");
        }
    }
}

public class StartNode : BaseNode
{
    public override void Draw()
    {
        title = "Start Node";

        // 只有输出口，没有输入口
        var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
        outputPort.portName = "Out";
        outputContainer.Add(outputPort);
        this.capabilities &= ~Capabilities.Deletable;

        RefreshExpandedState();
        RefreshPorts();
    }
}

public class EndNode : BaseNode
{
    public override void Draw()
    {
        title = "End Node";

        // 只有输入口，没有输出口
        var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        inputPort.portName = "In";
        inputContainer.Add(inputPort);
        this.capabilities &= ~Capabilities.Deletable;

        RefreshExpandedState();
        RefreshPorts();
    }
}
*/