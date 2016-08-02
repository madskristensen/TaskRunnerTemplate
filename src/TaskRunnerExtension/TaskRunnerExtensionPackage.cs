using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace TaskRunnerExtension
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [Guid(PackageGuids.guidVSPackageString)]
    public sealed class TaskRunnerExtensionPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();
        }
    }
}
