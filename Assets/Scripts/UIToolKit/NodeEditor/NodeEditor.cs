using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class NodeEditor : EditorWindow
{
    NodeTreeViewer nodeTreeViewer;
    InspectorViewer inspectorViewer;
    [MenuItem("Window/UI Toolkit/NodeEditor")]
    public static void ShowExample()
    {
        NodeEditor wnd = GetWindow<NodeEditor>();
        wnd.titleContent = new GUIContent("NodeEditor");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var nodeTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/UIToolKit/NodeEditor/NodeEditor.uxml");
        // 此处不使用visualTree.Instantiate() 为了保证行为树的单例防止重复实例化，以及需要将此root作为传参实时更新编辑器状态
        nodeTree.CloneTree(root);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/UIToolKit/NodeEditor/NodeEditor.uss");
        root.styleSheets.Add(styleSheet);

        // 将节点树视图添加到节点编辑器中
        nodeTreeViewer = root.Q<NodeTreeViewer>();
        // 将节属性面板视图添加到节点编辑器中
        inspectorViewer = root.Q<InspectorViewer>();
    }
    void OnEnable()
    {
        Selection.selectionChanged += OnSelectionChange;
    }
    private void OnDisable() {
        Selection.selectionChanged -= OnSelectionChange;
    }
    private void OnSelectionChange()
    {
        // 检测该选中对象中是否存在节点树
        NodeTree tree = Selection.activeObject as NodeTree;
        // 判断如果选中对象不为节点树，则获取该对象下的节点树运行器中的节点树
        if (!tree)
        {
            if (Selection.activeGameObject)
            {
                /* NodeTreeRunner runner = Selection.activeGameObject.GetComponent<NodeTreeRunner>();
                if (runner)
                {
                    tree = runner.tree;
                } */
            }
        }
        if (Application.isPlaying)
        {
            if (tree)
            {
                if (nodeTreeViewer != null)
                {
                    nodeTreeViewer.PopulateView(tree);
                }
            }
        }
        else
        {
            if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
            {
                if (nodeTreeViewer != null)
                {
                    nodeTreeViewer.PopulateView(tree);
                }
            }
        }
    }
}
