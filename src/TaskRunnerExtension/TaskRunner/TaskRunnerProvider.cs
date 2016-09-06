using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.TaskRunnerExplorer;

namespace TaskRunnerExtension
{
    /// <summary>
    /// This class will be called by Visual Studio automatically when a project
    /// is being opened that contains myconfig.json in the project- or solution root folder.
    /// </summary>
    [TaskRunnerExport("myconfig.json")]
    public class TaskRunnerProvider : ITaskRunner
    {
        private List<ITaskRunnerOption> _options = null;

        /// <summary>
        ///  This is where buttons from the VSCommandTable.vsct file are registered.
        ///  The string parameter is any argument that must be passed on to the task when it's enabled.
        /// </summary>
        public List<ITaskRunnerOption> Options
        {
            get
            {
                if (_options == null)
                {
                    _options = new List<ITaskRunnerOption>();
                    _options.Add(new TaskRunnerOption("Verbose", PackageIds.cmdVerbose, PackageGuids.guidVSPackageCmdSet, false, " (verbose)"));
                }

                return _options;
            }
        }

        public async Task<ITaskRunnerConfig> ParseConfig(ITaskRunnerCommandContext context, string configPath)
        {
            ITaskRunnerNode hierarchy = LoadHierarchy(configPath);

            return await Task.Run(() =>
            {
                return new TaskRunnerConfig(hierarchy);
            });
        }

        /// <summary>
        /// Construct any task hierarchy that you need.
        /// Task Runner Explorer will automatically have node.exe and npm.cmd on the PATH
        /// and you can control that in Tools -> Options -> Projects & Solutions -> External Web Tools
        /// </summary>
        private ITaskRunnerNode LoadHierarchy(string configPath)
        {
            string cwd = Path.GetDirectoryName(configPath);

            var root = new TaskRunnerNode("My Config");

            root.Children.Add(new TaskRunnerNode("Task 1", true)
            {
                Description = "Executes Task 1.",
                Command = new TaskRunnerCommand(cwd, "cmd.exe", "/c echo Task 1")
            });

            root.Children.Add(new TaskRunnerNode("Task 2", true)
            {
                Description = "Executes Task 2.",
                Command = new TaskRunnerCommand(cwd, "cmd.exe", "/c echo Task 2")
            });

            return root;
        }

    }
}
