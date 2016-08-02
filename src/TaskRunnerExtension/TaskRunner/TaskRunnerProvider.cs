﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.TaskRunnerExplorer;

namespace TaskRunnerExtension
{
    [TaskRunnerExport("myconfig.json")]
    public class TaskRunnerProvider : ITaskRunner
    {
        private ImageSource _icon;
        private List<ITaskRunnerOption> _options = null;

        public TaskRunnerProvider()
        {
            _icon = new BitmapImage(new Uri(@"pack://application:,,,/TaskRunnerExtension;component/Resources/logo.png"));
        }

        private void InitializeNpmTaskRunnerOptions()
        {
            _options = new List<ITaskRunnerOption>();
            _options.Add(new TaskRunnerOption("Verbose", PackageIds.cmdVerbose, PackageGuids.guidVSPackageCmdSet, false, " (verbose)"));
        }

        public List<ITaskRunnerOption> Options
        {
            get
            {
                if (_options == null)
                {
                    InitializeNpmTaskRunnerOptions();
                }

                return _options;
            }
        }

        public async Task<ITaskRunnerConfig> ParseConfig(ITaskRunnerCommandContext context, string configPath)
        {
            ITaskRunnerNode hierarchy = LoadHierarchy(configPath);

            return await Task.Run(() =>
            {
                return new TaskRunnerConfig(context, hierarchy, _icon);
            });
        }
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
