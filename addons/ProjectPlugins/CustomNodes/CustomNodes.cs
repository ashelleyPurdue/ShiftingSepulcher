#if TOOLS
using System;
using System.Collections.Generic;
using Godot;

namespace RandomDungeons.ProjectPlugins.CustomNodes
{
    [Tool]
    public class CustomNodes : EditorPlugin
    {
        private List<string> _addedTypes = new List<string>();

        public override void _EnterTree()
        {
            Register(
                "Node",
                "DungeonTreeTemplateRoom",
                "res://Scripts/Nodes/DungeonTreeTemplates/DungeonTreeTemplateRoom.cs"
            );
        }

        public override void _ExitTree()
        {
            foreach (var type in _addedTypes)
                RemoveCustomType(type);

            _addedTypes.Clear();
        }

        private void Register(
            string parent,
            string name,
            string path,
            Texture icon = null
        )
        {
            if (icon == null)
            {
                icon = GetBuiltInIcon("Script");
            }

            var script = GD.Load<Script>(path);
            AddCustomType(name, parent, script, icon);

            _addedTypes.Add(name);
        }

        private Texture GetBuiltInIcon(string name)
        {
            var gui = GetEditorInterface().GetBaseControl();
            return gui.GetIcon(name, "EditorIcons");
        }
    }
}

#endif
