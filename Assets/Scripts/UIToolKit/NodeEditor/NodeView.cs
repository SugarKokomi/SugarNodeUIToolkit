using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected;
    public Node node;
    public Port input;
    public Port output;
    public TextField textField;
    public NodeView(Node node)
    {
        this.node = node;
        this.title = node.name;
        // 将guid作为Node类中的viewDataKey关联进行后续的视图层管理
        this.viewDataKey = node.guid;
        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
        textField = new TextField();
        mainContainer.Add(textField);
    }
    private void CreateInputPorts()
    {
        /*将节点入口设置为 
            接口链接方向 横向Orientation.Vertical  竖向Orientation.Horizontal
            接口可链接数量 Port.Capacity.Single
            接口类型 typeof(bool)
        */
        // 默认所有节点为多入口类型
        input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(bool));
        
        if(input != null){
            // 将端口名设置为空
            input.portName = "";
            inputContainer.Add(input);
        }
    }

    private void CreateOutputPorts()
    {
       	output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        if(output != null){
            output.portName = "";
            outputContainer.Add(output);
        }
    }
    // 设置节点在节点树视图中的位置
    public override void SetPosition(Rect newPos)
    {
        // 将视图中节点位置设置为最新位置newPos
        base.SetPosition(newPos);
        // 将最新位置记录到运行时节点树中持久化存储
        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;
        EditorUtility.SetDirty(node);
    }

    // 复写Node类中的选中方法OnSelected
    public override void OnSelected()
    {
        base.OnSelected();
        // 如果当前OnNodeSelected选中部位空则将该节点视图传递到OnNodeSelected方法中视为选中
        if(OnNodeSelected != null){
            OnNodeSelected.Invoke(this);
        }
    }
}

