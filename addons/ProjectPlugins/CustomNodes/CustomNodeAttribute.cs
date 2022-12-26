using System;
using System.Reflection;

namespace RandomDungeons
{
    public class CustomNodeAttribute : Attribute
    {
        public readonly string Parent;

        public CustomNodeAttribute(
            string parent = "Node"
        )
        {
            Parent = parent;
        }
    }
}
