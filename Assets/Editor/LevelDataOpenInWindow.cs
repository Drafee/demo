using UnityEditor;
using UnityEditor.Callbacks;

public static class FlowGraphAssetOpener
{
    [OnOpenAsset(1)]
    public static bool OpenGraphAsset(int instanceID, int line)
    {
        var asset = EditorUtility.InstanceIDToObject(instanceID) as LevelFlowGraphData;
        if (asset != null)
        {
            // 获取或创建LevelFlowEditor窗口
            var window = EditorWindow.GetWindow<LevelFlowEditor>();
            window.titleContent = new UnityEngine.GUIContent("Level Flow Editor - " + asset.name);

            // 加载资源到编辑器
            window.LoadGraphAsset(asset);

            // 聚焦窗口
            window.Focus();

            return true; // 表示我们处理了这个资源的打开
        }
        return false; // 让Unity用默认方式处理
    }
}