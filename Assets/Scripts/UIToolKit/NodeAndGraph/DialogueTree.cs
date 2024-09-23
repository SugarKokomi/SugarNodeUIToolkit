using UnityEngine;

[CreateAssetMenu()]
public class DialogueTree : NodeTree{
    public override void OnTreeStart(){
        base.OnTreeStart();
        runningNode.state = Node.State.Running;
    }
}
