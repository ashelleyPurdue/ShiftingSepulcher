using System;
using System.Reflection;

namespace RandomDungeons
{
    public class CustomNodeAttribute : Attribute
    {
        public readonly string Parent;
        public readonly string Icon;

        public CustomNodeAttribute(
            string parent = "Node",
            string icon = "Script"
        )
        {
            Parent = parent;
            Icon = icon;
        }
    }
}
