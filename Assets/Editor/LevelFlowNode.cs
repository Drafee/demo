using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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
    public bool isBrightArea { get; private set; } = false;

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

        var boolField = new Toggle("是否是光亮区域");
        boolField.value = isBrightArea;
        boolField.RegisterValueChangedCallback(evt =>
        {
            isBrightArea = evt.newValue;
        });
        mainContainer.Add(boolField);
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



    public void SetDialogueType(bool type)
    {
        isBrightArea = type;
        // 更新UI显示
        var boolField = mainContainer.Q<Toggle>();
        if (boolField != null)
        {
            boolField.SetValueWithoutNotify(type);
        }
    }
}

public class AnimationNode : BaseNode
{
    public string TriggerName { get; private set; } = "";
    public Animator TargetAnimator { get; private set; }

    public override void Draw()
    {
        base.Draw();

        // Trigger 文本输入框
        var textField = new TextField("动画触发器:");
        textField.RegisterValueChangedCallback(evt =>
        {
            TriggerName = evt.newValue;
        });
        textField.SetValueWithoutNotify(TriggerName);
        mainContainer.Add(textField);

        // Animator 拖拽字段
        var animatorField = new ObjectField("Animator")
        {
            objectType = typeof(Animator),
            allowSceneObjects = true
        };
        animatorField.RegisterValueChangedCallback(evt =>
        {
            TargetAnimator = evt.newValue as Animator;
        });
        animatorField.SetValueWithoutNotify(TargetAnimator);
        mainContainer.Add(animatorField);
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

    public void SetAnimator(Animator animator)
    {
        TargetAnimator = animator;
        var animatorField = mainContainer.Q<ObjectField>();
        if (animatorField != null)
        {
            animatorField.SetValueWithoutNotify(animator);
        }
    }
}

public class AnimationScriptNode : BaseNode
{
    public AnimationSequencePlayer animationSequencePlayer;

    public override void Draw()
    {
        base.Draw();


        // Animator 拖拽字段
        var animatorScriptField = new ObjectField("动画脚本")
        {
            objectType = typeof(AnimationSequencePlayer),
            allowSceneObjects = true
        };
        animatorScriptField.RegisterValueChangedCallback(evt =>
        {
            animationSequencePlayer = evt.newValue as AnimationSequencePlayer;
        });
        animatorScriptField.SetValueWithoutNotify(animationSequencePlayer);
        mainContainer.Add(animatorScriptField);
    }

    public void SetAnimationScript(AnimationSequencePlayer a)
    {
        animationSequencePlayer = a;
        var animatorScriptField = mainContainer.Q<ObjectField>();
        if (animatorScriptField != null)
        {
            animatorScriptField.SetValueWithoutNotify(a);
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


public class AudioLevelNode : BaseNode
{
    public AudioLevelSetting audioLevelSetting { get; private set; } = null;
    public override void Draw()
    {
        base.Draw();
        title = "音频等级节点";

        // ObjectField 选择 AudioLevelSetting
        var soField = new ObjectField("音频设置")
        {
            objectType = typeof(AudioLevelSetting),
            allowSceneObjects = false
        };

        soField.RegisterValueChangedCallback(evt =>
        {
            audioLevelSetting = evt.newValue as AudioLevelSetting;
        });

        mainContainer.Add(soField);
    }

    public void SetAudioLevelData(AudioLevelSetting a)
    {
        audioLevelSetting = a;
        var audioLevelField = mainContainer.Q<ObjectField>();
        if (audioLevelField != null)
        {
            audioLevelField.SetValueWithoutNotify(a);
        }
    }
}
