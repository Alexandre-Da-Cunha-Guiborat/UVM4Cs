using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace UVM4Cs.Service
{
    /// <summary>
    /// Dotnet command wrapper for UVM4Cs project usage.
    /// </summary>
    public static class UVM4CsDotNetWrapper
    {
        #region DEBUG

        /// <summary>
        /// String representation of the assembly name.
        /// </summary>
        // private const string _asmName = "UVM4Cs.Service";

        /// <summary>
        /// String representation of the class name.
        /// </summary>
        // private const string _className = "UVM4CsDotNetWrapper";

        #endregion DEBUG

        #region Public

        #region Constructor
        // TBD
        #endregion Constructor

        #region Properties
        // TBD
        #endregion Properties

        #region Method

        /// <summary>
        /// Restore, Rebuild (Clean + Build) all solutions given as parameters.
        /// </summary>
        /// <param name="slnFilePaths">List of all solutions to rebuild.</param>
        public static void CleanBuildSolutions(List<string> slnFilePaths)
        {
            foreach (string slnFilePath in slnFilePaths)
            {
                RunRestoreCmd(slnFilePath);
                RunCleanCmd(slnFilePath);
                RunBuildCmd(slnFilePath);
            }
        }

        /// <summary>
        /// Run the donet restore command with and without the --source.
        /// </summary>
        /// <param name="slnFilePath">String representation of the absolute path to the .sln file to restore.</param>
        public static void RunRestoreCmd(string slnFilePath)
        {
            FileInfo slnFInfo = new FileInfo(slnFilePath);
            string nugetConfigFilePath = $"{slnFInfo.DirectoryName}/nuget.config";
            string cmd = $"dotnet restore \"{slnFilePath}\" --configfile \" {nugetConfigFilePath} \"";

            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = $" /c {cmd}";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();
        }

        /// <summary>
        /// Run the donet clean command in Debug and Release.
        /// </summary>
        /// <param name="slnFilePath">String representation of the absolute path to the .sln file to restore.</param>
        public static void RunCleanCmd(string slnFilePath)
        {
            string cmdCleanRelease = $"dotnet clean \"{slnFilePath}\" --configuration Release";
            string cmdCleanDebug = $"dotnet clean \"{slnFilePath}\" --configuration Debug";

            Process pRelease = new Process();
            pRelease.StartInfo.FileName = "cmd.exe";
            pRelease.StartInfo.Arguments = $" /c {cmdCleanRelease}";
            pRelease.StartInfo.UseShellExecute = false;
            pRelease.StartInfo.RedirectStandardOutput = true;
            pRelease.StartInfo.CreateNoWindow = true;
            pRelease.Start();
            pRelease.WaitForExit();

            Process pDebug = new Process();
            pDebug.StartInfo.FileName = "cmd.exe";
            pDebug.StartInfo.Arguments = $" /c {cmdCleanDebug}";
            pDebug.StartInfo.UseShellExecute = false;
            pDebug.StartInfo.RedirectStandardOutput = true;
            pDebug.StartInfo.CreateNoWindow = true;
            pDebug.Start();
            pDebug.WaitForExit();
        }

        /// <summary>
        /// Run the donet build command in Debug and Release.
        /// </summary>
        /// <param name="slnFilePath">String representation of the absolute path to the .sln file to restore.</param>
        public static void RunBuildCmd(string slnFilePath)
        {
            string cmdBuildRelease = $"dotnet build \"{slnFilePath}\" --configuration Release";
            string cmdBuildDebug = $"dotnet build \"{slnFilePath}\" --configuration Debug";

            Process pRelease = new Process();
            pRelease.StartInfo.FileName = "cmd.exe";
            pRelease.StartInfo.Arguments = $" /c {cmdBuildRelease}";
            pRelease.StartInfo.UseShellExecute = false;
            pRelease.StartInfo.RedirectStandardOutput = true;
            pRelease.StartInfo.CreateNoWindow = true;
            pRelease.Start();
            pRelease.WaitForExit();

            Process pDebug = new Process();
            pDebug.StartInfo.FileName = "cmd.exe";
            pDebug.StartInfo.Arguments = $" /c {cmdBuildDebug}";
            pDebug.StartInfo.UseShellExecute = false;
            pDebug.StartInfo.RedirectStandardOutput = true;
            pDebug.StartInfo.CreateNoWindow = true;
            pDebug.Start();
            pDebug.WaitForExit();
        }


        #endregion Method

        #region Function
        // TBD
        #endregion Function

        #region Field
        // TBD
        #endregion Field

        #endregion Public

        #region Protected

        #region Constructor
        // TBD
        #endregion Constructor

        #region Properties
        // TBD
        #endregion Properties

        #region Method
        // TBD
        #endregion Method

        #region Function
        // TBD
        #endregion Function

        #region Field
        // TBD
        #endregion Field

        #endregion Protected

        #region Private

        #region Constructor
        // TBD
        #endregion Constructor

        #region Properties
        // TBD
        #endregion Properties

        #region Method
        // TBD
        #endregion Method

        #region Function
        // TBD
        #endregion Function

        #region Field
        // TBD
        #endregion Field

        #endregion Private
    }
}
