using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SugarNode.Editor
{
    public class NodeEditorWindow : EditorWindow
    {
        [MenuItem("Window/SugarNode节点编辑器(UIToolkit)")]
        public static void ShowExample()
        {
            NodeEditorWindow wnd = GetWindow<NodeEditorWindow>();
            wnd.titleContent = new GUIContent("SugarNodeEditorWindow");
        }
        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var nodeTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(NodeEditorSetting.NodeEditorWindowFolderPath + "NodeEditorWindow.uxml");
            nodeTree.CloneTree(root);

            var styleSheet =  AssetDatabase.LoadAssetAtPath<StyleSheet>(NodeEditorSetting.NodeEditorWindowFolderPath + "NodeEditorWindow.uss");
            root.styleSheets.Add(styleSheet);
        }
    }
}