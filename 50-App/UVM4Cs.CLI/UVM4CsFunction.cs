using Microsoft.Extensions.Logging;
using UVM.Interface;
using UVM.Engine;
using UVM.Logging;
using UVM4Cs.Service;
using System.Collections.Generic;
using UVM4Cs.Engine;
using System;
using System.Linq;



namespace UVM4Cs.CLI
{
    /// <summary>
    /// Function implementation of UVM4Cs.
    /// </summary>
    public static class UVM4CsFunction
    {
        #region DEBUG

        /// <summary>
        /// String representation of the assembly name.
        /// </summary>
        private const string _asmName = "UVM4Cs.CLI";

        /// <summary>
        /// String representation of the class name.
        /// </summary>
        private const string _className = "UVM4CsFunction";

        #endregion DEBUG

        #region Public

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

        public static void UpdateWholeRepoEx(UVM4CsConfiguration configuration)
        {
            foreach (string gitDirectory in configuration.UVMConfig.GitDirectories)
            {
                if (!UVM4CsGitutils.IsGitDirectory(gitDirectory))
                {
                    Console.WriteLine($"The given path do not lead to a git directory : {gitDirectory}");
                    return;
                }
            }

            if (!UVM4CsHelper.IsUVM4CsRunningConditionMet(configuration))
            {
                return;
            }

            List<I_VersionnableFile> vfPool = UVM4CsReader.ReadCsharpFiles(configuration.UVMConfig.GitDirectories);
            List<I_VersionnableFile> modifiedVF = UVM4CsGitutils.ComputeModifiedVFAndVFWithModifiedFiles(vfPool, configuration.UVMConfig.GitDirectories, configuration.UVMConfig.CommitIdsRef, configuration.UVMConfig.CommitIds, UVM4CsHelper.FExtensions);

            if (modifiedVF.Count() == 0)
            {
                UVMLogger.DumpLogs(UVMConstante.UVM_LOG_FOLDER_PATH, LogLevel.Trace);
                return;
            }

            List<List<I_VersionnableFile>> filesToUpdateOrdered = UVM4CsManager.ComputeFilesToUpdateOrdered(vfPool, modifiedVF);


            bool u = UVM4CsUpdater.UpdateFiles(filesToUpdateOrdered, 0, configuration.UVMConfig.BuildModes[0], configuration.UVMConfig.DigitModes[0], configuration.DevId);
            bool w = UVM4CsWriter.DumpFiles(filesToUpdateOrdered);

            List<List<I_GenerableFile>> filesToGenerate = UVM4CsPackager.GetGenerableFiles(filesToUpdateOrdered);
            bool p = UVM4CsPackager.GenerateFiles(filesToGenerate);

            UVMLogger.DumpLogs(UVMConstante.UVM_LOG_FOLDER_PATH, LogLevel.Trace);
            // ###
        }

        /// <summary>
        /// Upgrade the Targeted
        /// </summary>
        /// <param name="configuration"></param>
        public static void UpdateTargetEx(UVM4CsConfiguration configuration)
        {
            if (configuration.TargetFiles.Count() == 0)
            {
                Console.WriteLine($"The given arguments do no specify any target files");
                return;
            }


            foreach (string gitDirectory in configuration.UVMConfig.GitDirectories)
            {
                if (!UVM4CsGitutils.IsGitDirectory(gitDirectory))
                {
                    Console.WriteLine($"The given path do not lead to a git directory : {gitDirectory}");
                    return;
                }
            }

            if (!UVM4CsHelper.IsUVM4CsRunningConditionMet(configuration))
            {
                return;
            }

            List<I_VersionnableFile> vfPool = UVM4CsReader.ReadCsharpFiles(configuration.UVMConfig.GitDirectories);
            List<I_VersionnableFile> vfLeafs = vfPool.Where(vf => configuration.TargetFiles.Contains(vf.VFPath.Replace("\\", "/")) && vf.VFExtension.Equals(".csproj")).ToList();
            List<I_VersionnableFile> csprojToConsider = UVMManager.ComputeParentTree(vfPool, vfLeafs);


            List<I_VersionnableFile> modifiedVF = UVM4CsGitutils.ComputeModifiedVFAndVFWithModifiedFiles(csprojToConsider, configuration.UVMConfig.GitDirectories, configuration.UVMConfig.CommitIdsRef, configuration.UVMConfig.CommitIds, UVM4CsHelper.FExtensions);
            if (modifiedVF.Count() == 0)
            {
                return;
            }

            List<List<I_VersionnableFile>> filesToUpdateOrdered = UVM4CsManager.ComputeFilesToUpdateOrdered(csprojToConsider, modifiedVF);


            bool u = UVM4CsUpdater.UpdateFiles(filesToUpdateOrdered, 0, configuration.UVMConfig.BuildModes[0], configuration.UVMConfig.DigitModes[0], configuration.DevId);
            bool w = UVM4CsWriter.DumpFiles(filesToUpdateOrdered);

            List<List<I_GenerableFile>> filesToGenerate = UVM4CsPackager.GetGenerableFiles(filesToUpdateOrdered);
            bool p = UVM4CsPackager.GenerateFiles(filesToGenerate);

            UVMLogger.DumpLogs(UVMConstante.UVM_LOG_FOLDER_PATH, LogLevel.Trace);
            // ###
        }

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
