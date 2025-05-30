using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public abstract class BaseNode : Node
{
    public string GUID { get; private set; }

    protected Port inputPort;
    protected Port outputPort;

    public BaseNode()
    {
        GUID = System.Guid.NewGuid().ToString();
    }

    public virtual void Draw()
    {
        // 创建输入端口（除了StartNode）
        if (!(this is StartNode))
        {
            inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = "Input";
            inputContainer.Add(inputPort);
        }

        // 创建输出端口（除了EndNode）
        if (!(this is EndNode))
        {
            outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(float));
            outputPort.portName = "Output";
            outputContainer.Add(outputPort);
        }

        RefreshExpandedState();
        RefreshPorts();
    }
}

public class StartNode : BaseNode
{
    public override void Draw()
    {
        base.Draw();

        var label = new Label("游戏开始");
        mainContainer.Add(label);
    }
}

public class EndNode : BaseNode
{
    public override void Draw()
    {
        base.Draw();

        var label = new Label("游戏结束");
        mainContainer.Add(label);
    }
}

public class NarrativeNode : BaseNode
{
    public string DialogueTag { get; private set; } = "";

    public override void Draw()
    {
        base.Draw();

        var textField = new TextField("对话标签:");
        textField.RegisterValueChangedCallback(evt =>
        {
            DialogueTag = evt.newValue;
        });
        textField.SetValueWithoutNotify(DialogueTag);

        mainContainer.Add(textField);
    }

    public void SetDialogueTag(string tag)
    {
        DialogueTag = tag;
        // 更新UI显示
        var textField = mainContainer.Q<TextField>();
        if (textField != null)
        {
            textField.SetValueWithoutNotify(tag);
        }
    }
}

public class AnimationNode : BaseNode
{
    public string TriggerName { get; private set; } = "";

    public override void Draw()
    {
        base.Draw();

        var textField = new TextField("动画触发器:");
        textField.RegisterValueChangedCallback(evt =>
        {
            TriggerName = evt.newValue;
        });
        textField.SetValueWithoutNotify(TriggerName);

        mainContainer.Add(textField);
    }

    public void SetTriggerName(string trigger)
    {
        TriggerName = trigger;
        var textField = mainContainer.Q<TextField>();
        if (textField != null)
        {
            textField.SetValueWithoutNotify(trigger);
        }
    }
}

public class SceneTransitionNode : BaseNode
{
    private PopupField<string> sceneDropdown;
    private List<string> sceneNames;
    private string selectedScene;

    public string SceneName => selectedScene;


    public override void Draw()
    {
        base.Draw();
        title = "Scene Transition Node";

        // 获取 Build Settings 中的场景
        sceneNames = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => System.IO.Path.GetFileNameWithoutExtension(s.path))
            .ToList();

        if (sceneNames.Count == 0)
            sceneNames.Add("No Scenes In Build Settings");

        // 默认选择第一个
        if (string.IsNullOrEmpty(selectedScene) || !sceneNames.Contains(selectedScene))
        {
            selectedScene = sceneNames[0];
        }

        // 创建下拉框
        sceneDropdown = new PopupField<string>("目标场景", sceneNames, selectedScene);
        sceneDropdown.RegisterValueChangedCallback(evt =>
        {
            selectedScene = evt.newValue;
        });

        mainContainer.Add(sceneDropdown);

    }

    public void SetSceneName(string sceneName)
    {
        selectedScene = sceneName;
        if (sceneDropdown != null)
        {
            sceneDropdown.SetValueWithoutNotify(sceneName);
        }
    }
}
/*
public class SceneTransitionNode : BaseNode
{
    private PopupField<string> sceneDropdown;
    private List<string> sceneNames;
    private string selectedScene;

    public string SceneName => selectedScene;

    public override void Draw()
    {
        base.Draw();

        title = "Scene Transition Node";

        // 获取 Build Settings 中的场景
        sceneNames = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => System.IO.Path.GetFileNameWithoutExtension(s.path))
            .ToList();

        if (sceneNames.Count == 0)
            sceneNames.Add("No Scenes In Build Settings");

        // 默认选择第一个
        if (string.IsNullOrEmpty(selectedScene) || !sceneNames.Contains(selectedScene))
        {
            selectedScene = sceneNames[0];
        }

        // 创建下拉框
        sceneDropdown = new PopupField<string>("目标场景", sceneNames, selectedScene);
        sceneDropdown.RegisterValueChangedCallback(evt =>
        {
            selectedScene = evt.newValue;
        });

        extensionContainer.Add(sceneDropdown);

        RefreshExpandedState();
        RefreshPorts();
    }



    // 保存数据时使用
    public override NodeData SaveData()
    {
        var data = base.SaveData();
        data.extraData = selectedScene;
        return data;
    }

    // 加载数据时使用
    public override void LoadData(NodeData data)
    {
        base.LoadData(data);
        SetScene(data.extraData);
    }
}
*/