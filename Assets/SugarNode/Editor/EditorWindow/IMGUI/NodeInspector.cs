using UnityEngine;
using UnityEditor;

namespace SugarNode.Editor
{
    [CustomEditor(typeof(Node), true)]
    public class NodeInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("在 节 点 编 辑 器 中 打 开", GUILayout.Height(50)))
            {
                NodeEditorWindow.OpenWindow();
                NodeEditorWindow.Instance.TrySwitchGraphAndRepaint((target as Node).graph);
            }
            /* if (GUILayout.Button("前往GitHub"))
            {
                Application.OpenURL("https://github.com/SugarKokomi/SugarNode/");
            } */
        }
    }
}
