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
        [MenuItem("Window/SugarNodeEditor")]
        public static void OpenWindow() => Instance.Show();
        public void CreateGUI()
        {
            var nodeTree = Resources.Load<VisualTreeAsset>("NodeEditorWindow_TreeAsset");
            nodeTree.CloneTree(rootVisualElement);

            var styleSheet = Resources.Load<StyleSheet>("NodeEditorWindow_StyleSheet");
            rootVisualElement.styleSheets.Add(styleSheet);

            nodeGridElement = rootVisualElement.Q<NodeGridElement>();//有点像GetComponentInChild()
            nodeGridElement.graphViewChanged += NodeGridElement.OnGraphViewChanged;
        }
        private void OnFocus()
        {
            if (!activeGraph)
            {
                titleContent.text = activeGraph ? activeGraph.name : nameof(SugarNode);
                //Unity执行重编译后，会清除所有缓存字段。
                //OnFocus()此时会比CreateGUI()更先调用
                //nodeGridElement是在CreateGUI()里初始化的，会导致其null一次。因此必须判空
                nodeGridElement?.RePaint();
            }

        }
        private void OnDestroy()
        {
            nodeGridElement.graphViewChanged -= NodeGridElement.OnGraphViewChanged;

            m_instance = null;
            if (activeGraph)
            {
                activeGraph.TryClearAllRuntimeCache();
                AssetDatabase.SaveAssetIfDirty(activeGraph);
                activeGraph = null;
            }
        }
        /* void OnSelectionChange()
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
        } */
        /// <summary> 切换激活的节点图，并进行重绘制 </summary>
        internal void TrySwitchGraphAndRepaint(Graph graph)
        {
            if (activeGraph == graph) return;
            if (activeGraph) activeGraph.TryClearAllRuntimeCache();
            if (graph)
            {
                graph.TryBuildAllRuntimeCache();
                titleContent.text = graph.name;
            }
            else titleContent.text = nameof(SugarNode);
            activeGraph = graph;
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