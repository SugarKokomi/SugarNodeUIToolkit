using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SugarNode.Editor
{
    [EditorWindowTitle(title = "SugarNode")]
    public class NodeEditorWindow : EditorWindow
    {
        static NodeEditorWindow m_window;
        internal static NodeEditorWindow Window { get { if (!m_window) m_window = GetWindow<NodeEditorWindow>(); return m_window; } }
        internal static Graph activeGraph;
        internal NodeGridElement nodeGridElement;
        [MenuItem("Window/SugarNode节点编辑器(UIToolkit)")]
        public static void OpenWindow() => Window.Show();
        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var nodeTree = Resources.Load<VisualTreeAsset>("NodeEditorWindow_TreeAsset");
            nodeTree.CloneTree(root);

            var styleSheet = Resources.Load<StyleSheet>("NodeEditorWindow_StyleSheet");
            root.styleSheets.Add(styleSheet);
        }
        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChange;
        }
        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChange;
        }
        void OnSelectionChange()
        {
            if (Selection.activeObject is Graph graph)
            {
                activeGraph = graph;
                nodeGridElement.RePaint();
            }
        }
    }
}