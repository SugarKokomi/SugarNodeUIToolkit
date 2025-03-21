using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using PortElement = UnityEditor.Experimental.GraphView.Port;
using System;
using static UnityEditor.Experimental.GraphView.Port;

namespace SugarNode.Editor
{
    public class NodePortElement : PortElement
    {
        protected static Type GetPortType(Type objectType)
        {
            if (objectType.IsGenericType)
            {
                Type tType = objectType.GetGenericTypeDefinition();
                if (tType == typeof(InputPort<>) || tType == typeof(OutputPort<>))
                    return objectType.GetGenericArguments()[0];
                else return null;
            }
            else return null;
        }
        public static NodePortElement Create(Direction portDirection, int maxConnectionCount, string attachedGUID, Type type)
        {

            var ret = new NodePortElement(portDirection, type)
            {
                m_EdgeConnector = new EdgeConnector<Edge>(new DefaultEdgeConnectorListener()),
                guid = attachedGUID,
                maxConnectionCount = maxConnectionCount
            };
            ret.AddManipulator(ret.m_EdgeConnector);
            NodeElement.uiPairsPortReverse.TryAdd(attachedGUID, ret);
            return ret;
        }
        public string guid;
        int maxConnectionCount;
        protected int MaxConnectionCount => maxConnectionCount;
        // private NodeElement nodeElement;
        // protected NodeElement NodeElement => nodeElement;
        public NodePortElement(Direction portDirection, Type type)
            : base(Orientation.Horizontal, portDirection, Capacity.Multi, type)
        {
        }
        //=================================================
        public virtual void OnDropOutsidePort(Edge edge, Vector2 position)
        {
        }
        /// <summary> 当成功创建了一条连接线时 </summary>
        /* public virtual void OnDrop(GraphView graphView, Edge edge)
        {
            GraphViewChange m_GraphViewChange = new GraphViewChange();
            List<Edge> m_EdgesToCreate = new List<Edge>();
            List<Edge> m_EdgesToDelete = new List<Edge>();
            m_EdgesToCreate.Add(edge);

            if (edge.input.connections.Count() >= MaxConnectionCount)
            {
                m_EdgesToDelete.Add(edge.input.connections.FirstOrDefault());
            }
            if (edge.output.connections.Count() >= MaxConnectionCount)
            {
                m_EdgesToDelete.Add(edge.output.connections.FirstOrDefault());
            }
            if (m_EdgesToDelete.Count > 0)
            {
                graphView.DeleteElements(m_EdgesToDelete);
            }

            if (graphView.graphViewChanged != null)
            {
                m_EdgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
            }

            foreach (Edge item in m_EdgesToCreate)
            {
                graphView.AddElement(item);
                edge.input.Connect(item);
                edge.output.Connect(item);
            }

        } */
    }
    internal class DefaultEdgeConnectorListener : IEdgeConnectorListener
    {
        private GraphViewChange m_GraphViewChange;

        private List<Edge> m_EdgesToCreate;

        private List<GraphElement> m_EdgesToDelete;

        public DefaultEdgeConnectorListener()
        {
            m_EdgesToCreate = new List<Edge>();
            m_EdgesToDelete = new List<GraphElement>();
            m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            m_EdgesToCreate.Clear();
            m_EdgesToCreate.Add(edge);
            m_EdgesToDelete.Clear();
            if (edge.input.capacity == Capacity.Single)
            {
                foreach (Edge connection in edge.input.connections)
                {
                    if (connection != edge)
                    {
                        m_EdgesToDelete.Add(connection);
                    }
                }
            }

            if (edge.output.capacity == Capacity.Single)
            {
                foreach (Edge connection2 in edge.output.connections)
                {
                    if (connection2 != edge)
                    {
                        m_EdgesToDelete.Add(connection2);
                    }
                }
            }

            if (m_EdgesToDelete.Count > 0)
            {
                graphView.DeleteElements(m_EdgesToDelete);
            }

            List<Edge> edgesToCreate = m_EdgesToCreate;
            if (graphView.graphViewChanged != null)
            {
                edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
            }

            foreach (Edge item in edgesToCreate)
            {
                graphView.AddElement(item);
                edge.input.Connect(item);
                edge.output.Connect(item);
            }
        }
    }
}