using SugarNode;
using UnityEngine;

public class M_Node2 : SugarNode.Node
{
    public int num = 123;
    public InputPort<int> input;
    public OutputPort<string> output1;
    public OutputPort<string> output2;
    protected override void Init()
    {
        input = new InputPort<int>();
        output1 = new OutputPort<string>();
        output2 = new OutputPort<string>();

        input.GetValue = () => num;
        output1.GetValue = () => num + 666;
        output2.GetValue = () => num + 10086;

        AddPort(input);
        AddPort(output1);
        AddPort(output2);
    }
}
