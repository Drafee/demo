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
            // ��ȡ�򴴽�LevelFlowEditor����
            var window = EditorWindow.GetWindow<LevelFlowEditor>();
            window.titleContent = new UnityEngine.GUIContent("Level Flow Editor - " + asset.name);

            // ������Դ���༭��
            window.LoadGraphAsset(asset);

            // �۽�����
            window.Focus();

            return true; // ��ʾ���Ǵ����������Դ�Ĵ�
        }
        return false; // ��Unity��Ĭ�Ϸ�ʽ����
    }
}