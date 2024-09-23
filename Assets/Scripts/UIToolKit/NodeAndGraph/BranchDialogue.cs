using System.Collections.Generic;
using UnityEngine;

public class BranchDialogue : CompositeNode
{
    [TextArea] public string dialogueContent;
    public int nextDialogueIndex = 0;
    public override Node LogicUpdate()
    {
    	// 判断进入哪个对话节点
        if(Input.GetKeyDown(KeyCode.A)){
            nextDialogueIndex = 0;
        }
        if(Input.GetKeyDown(KeyCode.B)){
            nextDialogueIndex = 1;
        }
        // 判断进入下一节点条件成功时 需将节点状态改为非运行中 且 返回对应子节点
        if(Input.GetKeyDown(KeyCode.Space)){
            state = State.Waiting;
            if(children.Count > nextDialogueIndex){
                children[nextDialogueIndex].state = State.Running;
                return children[nextDialogueIndex];
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
