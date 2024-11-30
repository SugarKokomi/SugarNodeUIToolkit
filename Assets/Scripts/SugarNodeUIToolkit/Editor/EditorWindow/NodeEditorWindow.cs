using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace SugarNode.Editor
{
    [EditorWindowTitle(title = "SugarNode")]
    public class NodeEditorWindow : EditorWindow
    {
        static NodeEditorWindow m_instance;
        internal static NodeEditorWindow Instance { get { if (!m_instance) { m_instance = GetWindow<NodeEditorWindow>(); m_instance.titleContent.text = nameof(SugarNode); } return m_instance; } }
        internal static Graph activeGraph;
        internal NodeGridElement nodeGridElement;
        internal NodeInspectorElement inspector;
        [MenuItem("Window/SugarNodeEditor")]
        public static void OpenWindow() => Instance.Show();
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

            m_instance = null;
            if (activeGraph)
            {
                activeGraph.TryClearAllRuntimeCache();
                activeGraph = null;
            }
        }
        void OnSelectionChange()
        {
            if (Selection.activeObject is Graph graph)
                TrySwitchGraphAndRepaint(graph);
            else if (Selection.activeObject is Node node)
                TrySwitchGraphAndRepaint(node.graph);
        }
        internal void SetSelectionNoEvent(Node node)
        {
            Selection.selectionChanged -= OnSelectionChange;
            Selection.activeObject = node;
            Selection.selectionChanged += OnSelectionChange;
        }
        /// <summary> 切换激活的节点图，并进行重绘制 </summary>
        void TrySwitchGraphAndRepaint(Graph graph)
        {
            if (activeGraph == graph) return;
            else if (activeGraph) activeGraph.TryClearAllRuntimeCache();
            activeGraph = graph;
            titleContent.text = graph.name;
            nodeGridElement.RePaint();
        }
        [OnOpenAsset]
        public static bool OpenAsset(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is Graph graph)
            {
                OpenWindow();
                Instance.TrySwitchGraphAndRepaint(graph);
                return true;
            }
            else return false;
        }
    }
}