using UnityEngine;
using UnityEditor;

namespace SugarNode.Editor
{
    // [CustomEditor(typeof(Node), true)]
    public class NodeInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("在节点编辑器中打开", GUILayout.Height(50)))
            {
                NodeEditorWindow.OpenWindow();
                NodeEditorWindow.Instance.TrySwitchGraphAndRepaint((target as Node).Graph);
            }
            DrawDefaultInspector();
            /* if (GUILayout.Button("前往GitHub"))
            {
                Application.OpenURL("https://github.com/SugarKokomi/SugarNode/");
            } */
        }
    }
}
