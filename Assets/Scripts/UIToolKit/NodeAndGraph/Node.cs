using UnityEngine;

public abstract class Node : ScriptableObject
{
        [HideInInspector]public string guid;
    [HideInInspector]public Vector2 position;

    // 对话节点状态枚举值为运行和等待两种状态
    public enum State{ Running , Waiting }
    // 对话节点当前状态
    public State state = State.Waiting;
    // 是否已经开始当前对话节点判断指标
    public bool started = false;
    // 每个对话节点的描述
    [TextArea] public string description;
    public Node OnUpdate(){
        // 判断该节点首次调用OnUpdate时调用一次OnStart方法
        if(!started){
            OnStart();
            started =true;
        }
        Node currentNode = LogicUpdate();
        // 判断该节点结束时调用一次OnStop方法
        if(state != State.Running){
            OnStop();
            started =false;
        }
        return currentNode;
    } 
    public abstract Node LogicUpdate();
    protected abstract void OnStart();
    protected abstract void OnStop();
}
