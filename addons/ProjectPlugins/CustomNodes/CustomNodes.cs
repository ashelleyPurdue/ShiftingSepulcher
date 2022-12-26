#if TOOLS
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    [Tool]
    public class CustomNodes : EditorPlugin
    {
        private List<string> _addedTypes = new List<string>();

        public override void _EnterTree()
        {
            var customNodeClasses = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.GetCustomAttributes<CustomNodeAttribute>().Any());

            foreach (var nodeClass in customNodeClasses)
                Register(nodeClass);
        }

        public override void _ExitTree()
        {
            foreach (var type in _addedTypes)
                RemoveCustomType(type);

            _addedTypes.Clear();
        }

        private void Register(Type type)
        {
            var attribute = type.GetCustomAttribute<CustomNodeAttribute>();

            string name = type.Name;
            string path = FindScriptFilePath(type);
            string parent = attribute.Parent;
            var icon = GetBuiltInIcon(attribute.Icon);

            var script = GD.Load<Script>(path);
            AddCustomType(name, parent, script, icon);

            _addedTypes.Add(name);
        }

        private Texture GetBuiltInIcon(string name)
        {
            var gui = GetEditorInterface().GetBaseControl();
            return gui.GetIcon(name, "EditorIcons");
        }

        private static string FindScriptFilePath(Type type)
        {
            string classSignature = $"public class {type.Name}";

            // Search for the first ".cs" file that contains this node's
            // declaration
            string projectRootPath = ProjectSettings.GlobalizePath("res://");
            var projectRootDir = new DirectoryInfo(projectRootPath);
            string nodeFilePath = TraverseFolder(projectRootDir)
                .Where(f => f.Extension == ".cs")
                .Select(f => f.FullName)
                .First(f => System.IO.File.ReadAllText(f).Contains(classSignature));

            // Convert the full file path to a "res://" path.
            // For whatever reason, ProjectSettings.LocalizePath() isn't actually
            // producing a path beginning with "res://", so we need to do this
            // manually.
            // Thanks, Godot!
            string relativePath = MakeRelativePath(projectRootPath, nodeFilePath);
            return $"res://{relativePath.Replace('\\', '/')}";

            IEnumerable<FileInfo> TraverseFolder(DirectoryInfo dir)
            {
                foreach (var file in dir.EnumerateFiles())
                    yield return file;

                foreach (var childDir in dir.EnumerateDirectories())
                {
                    foreach (var file in TraverseFolder(childDir))
                        yield return file;
                }
            }
        }

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// Stolen from
        /// https://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path
        /// </summary>
        /// <param name="fromPath">
        /// Contains the directory that defines the start of the relative path.
        /// </param>
        /// <param name="toPath">
        /// Contains the path that defines the endpoint of the relative path.
        /// </param>
        /// <returns>
        /// The relative path from the start directory to the end path
        /// or <c>toPath</c> if the paths are not related.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private static String MakeRelativePath(String fromPath, String toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath))   throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(
                    System.IO.Path.AltDirectorySeparatorChar,
                    System.IO.Path.DirectorySeparatorChar
                );
            }

            return relativePath;
        }
    }
}

#endif
