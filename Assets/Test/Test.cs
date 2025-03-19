using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using SugarNode;
using System;

public class Test : MonoBehaviour
{
    public M_DialogueGraph m_DialogueGraph;
    // public ListPort<OutputPort<int>> outputPorts;
    public TestList<string> strings;
    void Start()
    {
        m_DialogueGraph.TryBuildAllRuntimeCache();
        foreach (var _node in m_DialogueGraph.Nodes)
        {
            Debug.Log(_node.name);
        }
        var node0 = (m_DialogueGraph.Nodes as List<Node>)[0];
        var node1 = (m_DialogueGraph.Nodes as List<Node>)[1];
        var node2 = (m_DialogueGraph.Nodes as List<Node>)[2];
        Debug.Log(node1.GetValue(node2.DefaultInput.Connection));
    }
}
[Serializable]//
public class TestList<T>:List<T>{}