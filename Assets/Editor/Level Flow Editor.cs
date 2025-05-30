
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

        CreateStartEndNodes();
    }

    private void CreateStartEndNodes()
    {
        // Create startNode and end Node in the begining
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

        AddToolbarButton(toolbar, "Add Dialogue Node", NodeType.Dialogue);
        AddToolbarButton(toolbar, "Add Animation Node", NodeType.Animation);
        AddToolbarButton(toolbar, "Add Scene Transition Node", NodeType.SceneTransition);
        AddToolbarButton(toolbar, "Add Narrative Node", NodeType.Narrative);

        rootVisualElement.Add(toolbar);
    }

    private void AddToolbarButton(VisualElement toolbar, string label, NodeType type)
    {
        var button = new Button(() => graphView.CreateNode(type));
        button.text = label;
        toolbar.Add(button);
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
}

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