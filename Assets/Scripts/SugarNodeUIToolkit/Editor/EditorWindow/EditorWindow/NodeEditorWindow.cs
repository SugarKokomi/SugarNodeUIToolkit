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
        internal NodeInspectorElement inspector;
        [MenuItem("Window/SugarNode节点编辑器(UIToolkit)")]
        public static void OpenWindow() => Window.Show();
        public void CreateGUI()
        {
            var nodeTree = Resources.Load<VisualTreeAsset>("NodeEditorWindow_TreeAsset");
            nodeTree.CloneTree(rootVisualElement);

            var styleSheet = Resources.Load<StyleSheet>("NodeEditorWindow_StyleSheet");
            rootVisualElement.styleSheets.Add(styleSheet);

            nodeGridElement = rootVisualElement.Q<NodeGridElement>();//有点像GetComponentInChild()
            inspector = rootVisualElement.Q<NodeInspectorElement>();
            NodeElement.OnNodeSelected += inspector.UpdateSelection;
            nodeGridElement.graphViewChanged += NodeGridElement.OnGraphViewChanged;
        }
        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChange;

        }
        private void OnDisable()
        {
            NodeElement.OnNodeSelected -= inspector.UpdateSelection;
            Selection.selectionChanged -= OnSelectionChange;
            nodeGridElement.graphViewChanged -= NodeGridElement.OnGraphViewChanged;

        }
        void OnSelectionChange()
        {
            if (Selection.activeObject is Graph graph && activeGraph != graph)
            {
                activeGraph = graph;
                nodeGridElement.RePaint();
            }
            else if (Selection.activeObject is Node node && activeGraph != node.graph)
            {
                activeGraph = node.graph;
                nodeGridElement.RePaint();
            }
        }
    }
}