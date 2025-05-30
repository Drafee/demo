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
        // ��������˿ڣ�����StartNode��
        if (!(this is StartNode))
        {
            inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = "Input";
            inputContainer.Add(inputPort);
        }

        // ��������˿ڣ�����EndNode��
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

        var label = new Label("��Ϸ��ʼ");
        mainContainer.Add(label);
    }
}

public class EndNode : BaseNode
{
    public override void Draw()
    {
        base.Draw();

        var label = new Label("��Ϸ����");
        mainContainer.Add(label);
    }
}

public class NarrativeNode : BaseNode
{
    public string DialogueTag { get; private set; } = "";

    public override void Draw()
    {
        base.Draw();

        var textField = new TextField("�Ի���ǩ:");
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
        // ����UI��ʾ
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

        var textField = new TextField("����������:");
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

        // ��ȡ Build Settings �еĳ���
        sceneNames = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => System.IO.Path.GetFileNameWithoutExtension(s.path))
            .ToList();

        if (sceneNames.Count == 0)
            sceneNames.Add("No Scenes In Build Settings");

        // Ĭ��ѡ���һ��
        if (string.IsNullOrEmpty(selectedScene) || !sceneNames.Contains(selectedScene))
        {
            selectedScene = sceneNames[0];
        }

        // ����������
        sceneDropdown = new PopupField<string>("Ŀ�곡��", sceneNames, selectedScene);
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

        // ��ȡ Build Settings �еĳ���
        sceneNames = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => System.IO.Path.GetFileNameWithoutExtension(s.path))
            .ToList();

        if (sceneNames.Count == 0)
            sceneNames.Add("No Scenes In Build Settings");

        // Ĭ��ѡ���һ��
        if (string.IsNullOrEmpty(selectedScene) || !sceneNames.Contains(selectedScene))
        {
            selectedScene = sceneNames[0];
        }

        // ����������
        sceneDropdown = new PopupField<string>("Ŀ�곡��", sceneNames, selectedScene);
        sceneDropdown.RegisterValueChangedCallback(evt =>
        {
            selectedScene = evt.newValue;
        });

        extensionContainer.Add(sceneDropdown);

        RefreshExpandedState();
        RefreshPorts();
    }



    // ��������ʱʹ��
    public override NodeData SaveData()
    {
        var data = base.SaveData();
        data.extraData = selectedScene;
        return data;
    }

    // ��������ʱʹ��
    public override void LoadData(NodeData data)
    {
        base.LoadData(data);
        SetScene(data.extraData);
    }
}
*/