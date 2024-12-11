using System;
using UnityEngine;
namespace SugarNode
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class NodeDefaultNameAttribute : Attribute
    {
        public string name;
        public NodeDefaultNameAttribute(string name = "")
        {
            this.name = name;
        }
    }
}