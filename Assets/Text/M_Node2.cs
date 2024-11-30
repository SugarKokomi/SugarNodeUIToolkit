using SugarNode;
using UnityEngine;

public class M_Node2 : SugarNode.Node
{
    public int num = 123;
    public InputPort<int> input = new InputPort<int>();
    public OutputPort<string> output1 = new OutputPort<string>();
    public OutputPort<string> output2 = new OutputPort<string>();
    protected override void Init()
    {
        input.GetValue = () => num;
        output1.GetValue = () => num + 666;
        output2.GetValue = () => num + 10086;

        InitPort(input);
        InitPort(output1);
        InitPort(output2);
    }
}
