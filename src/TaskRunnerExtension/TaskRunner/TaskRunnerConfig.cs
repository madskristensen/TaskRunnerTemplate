using System;
using System.IO;
using System.Text;
using System.Windows.Media;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TaskRunnerExplorer;

namespace TaskRunnerExtension
{
    class TaskRunnerConfig : ITaskRunnerConfig
    {
        private ImageSource _icon;
        private ITaskRunnerCommandContext _context;
        ITaskRunnerNode _hierarchy;

        public TaskRunnerConfig(ITaskRunnerCommandContext context, ITaskRunnerNode hierarchy, ImageSource icon)
        {
            _context = context;
            _hierarchy = hierarchy;
            _icon = icon;
        }

        public ImageSource Icon
        {
            get { return _icon; }
        }

        public ITaskRunnerNode TaskHierarchy
        {
            get { return _hierarchy; }
        }

        public string LoadBindings(string configPath)
        {
            string bindingPath = configPath + ".bindings";


            if (File.Exists(bindingPath))
            {
                foreach (var line in File.ReadAllLines(bindingPath))
                {
                    if (line.StartsWith("///<binding"))
                        return line.TrimStart('/').Trim();
                }
            }

            return "<binding />";
        }

        public bool SaveBindings(string configPath, string bindingsXml)
        {
            string bindingPath = configPath + ".bindings";

            try
            {
                var sb = new StringBuilder();

                if (File.Exists(bindingPath))
                {
                    var lines = File.ReadAllLines(bindingPath);

                    foreach (var line in lines)
                    {
                        if (!line.TrimStart().StartsWith("///<binding", StringComparison.OrdinalIgnoreCase))
                            sb.AppendLine(line);
                    }
                }

                if (bindingsXml != "<binding />")
                    sb.Insert(0, "///" + bindingsXml);

                ProjectHelpers.CheckFileOutOfSourceControl(bindingPath);

                if (sb.Length == 0)
                {
                    ProjectHelpers.DeleteFileFromProject(bindingPath);
                }
                else
                {
                    File.WriteAllText(bindingPath, sb.ToString(), Encoding.UTF8);
                    ProjectHelpers.AddNestedFile(configPath, bindingPath);
                }

                IVsPersistDocData persistDocData;
                if (!IsDocumentDirty(configPath, out persistDocData) && persistDocData != null)
                {
                    int cancelled;
                    string newName;
                    persistDocData.SaveDocData(VSSAVEFLAGS.VSSAVE_SilentSave, out newName, out cancelled);
                }
                else if (persistDocData == null)
                {
                    new FileInfo(configPath).LastWriteTime = DateTime.Now;
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.Write(ex);
                return false;
            }
        }

        public static bool IsDocumentDirty(string documentPath, out IVsPersistDocData persistDocData)
        {
            var serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)ProjectHelpers.DTE);

            IVsHierarchy vsHierarchy;
            uint itemId, docCookie;
            VsShellUtilities.GetRDTDocumentInfo(serviceProvider, documentPath, out vsHierarchy, out itemId, out persistDocData, out docCookie);

            if (persistDocData != null)
            {
                int isDirty;
                persistDocData.IsDocDataDirty(out isDirty);
                return isDirty == 1;
            }

            return false;
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
