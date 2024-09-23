using UnityEngine;

// 普通对话节点 后续只会返回一种情况的对话内容
public class NormalDialogue : SingleNode
{
    [TextArea] public string dialogueContent;
    public override Node LogicUpdate()
    {
        // 判断进入下一节点条件成功时 需将节点状态改为非运行中 且 返回对应子节点
        if(Input.GetKeyDown(KeyCode.Space)){
            state = State.Waiting;
            if(child != null){
                child.state = State.Running;
                return child;
            }
        }
        return this;
    }
    //首次进入该节点时打印对话内容
    protected override void OnStart()
    {
        Debug.Log(dialogueContent);
    }
	// 结束时打印OnStop
    protected override void OnStop()
    {
        Debug.Log("OnStop");
    }
}
