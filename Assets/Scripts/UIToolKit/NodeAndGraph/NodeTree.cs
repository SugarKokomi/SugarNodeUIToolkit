using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/* 继承脚本数据化结构对象 ScriptableObject */
public class NodeTree : ScriptableObject
{
    // 当前正在播放的对话
    public Node rootNode;
    // 当前正在播放的对话
    public Node runningNode;
    // 对话树当前状态 用于判断是否要开始这段对话
    public Node.State treeState = Node.State.Waiting;
    // 所有对话内容的存储列表
    public List<Node> nodes = new List<Node>();

    // 判断当前对话树和对话内容都是运行中状态则进行OnUpdate()方法更新
    public virtual void Update() {
        if(treeState == Node.State.Running && runningNode.state == Node.State.Running){
            runningNode = runningNode.OnUpdate();
        }
    }
    // 对话树开始的触发方法
    public virtual void OnTreeStart(){
        treeState = Node.State.Running;
    }
    // 对话树结束的触发方法
    public virtual void OnTreeEnd(){
        treeState = Node.State.Waiting;
    }
    #if UNITY_EDITOR
        public Node CreateNode(System.Type type){
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.name =type.Name;
            node.guid = GUID.Generate().ToString();
         
            nodes.Add(node);
            if(!Application.isPlaying){
                AssetDatabase.AddObjectToAsset(node,this);
            }
            AssetDatabase.SaveAssets();
            return node;
        }
        public Node DeleteeNode(Node node){
            nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            // Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
            return node;
        }
#endif
}
