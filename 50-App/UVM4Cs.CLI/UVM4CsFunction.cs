using ADCG.DevTools.Logging.Enum;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UVM.Engine;
using UVM.Interface;
using UVM.Interface.Interfaces;
using UVM.Logging;
using UVM4Cs.Bll;
using UVM4Cs.Common;
using UVM4Cs.Engine;
using UVM4Cs.Service;



namespace UVM4Cs.CLI
{
    /// <summary>
    /// Function implementation of UVM4Cs.
    /// </summary>
    public class UVM4CsFunction
    {
        #region Singleton
        // TBD
        #endregion Singleton

        #region Public

        public void UpdateWholeRepoEx(UVM4CsConfiguration configuration)
        {
            foreach (String gitDirectory in configuration.UVMConfig.GitDirectories)
            {
                if (!_gitUtils.IsGitDirectory(gitDirectory))
                {
                    Console.WriteLine($"The given path do not lead to a git directory : {gitDirectory}");
                    return;
                }
            }

            if (!_helper.IsUVM4CsRunningConditionMet(configuration))
            {
                return;
            }

            List<UVM4CsCsproj> vfPool = _reader.ReadCsharpFiles(configuration.UVMConfig.GitDirectories);
            List<UVM4CsCsproj> modifiedVF = _gitUtils.ComputeModifiedCsprojAndCsprojWithModifiedFiles(vfPool, configuration.UVMConfig.GitDirectories, configuration.UVMConfig.CommitIdsRef, configuration.UVMConfig.CommitIds, UVM4CsHelper.FExtensions);

            if (modifiedVF.Count() == 0)
            {
                UVMLogger.DumpLogs(UVMConstant.UVM_LOG_FOLDER_PATH, ADCGLogLevelType.TRACE);
                return;
            }

            List<List<UVM4CsCsproj>> filesToUpdateOrdered = _manager.ComputeFilesToUpdateOrdered(vfPool, modifiedVF);


            Boolean u = _updater.UpdateFiles(filesToUpdateOrdered, 0, configuration.UVMConfig.BuildModes[0], configuration.UVMConfig.DigitModes[0], configuration.DevId);
            Boolean d = _dumper.DumpFiles(filesToUpdateOrdered);

            List<List<String>> outputPaths = [];
            List<List<List<String>>> args = [];
            foreach (List<UVM4CsCsproj> x in filesToUpdateOrdered)
            {
                List<String> outputPathsInner = [];
                List<List<String>> argsInner = [];
                foreach (UVM4CsCsproj y in x)
                {
                    outputPathsInner.Add(UVM4CsConstant.DEFAULT_PACKAGE_OUTPUT_DIR_PATH);
                    argsInner.Add([$"{UVM4CsConstant.ConfigBuildFlag}={configuration.Configuration}", $"{UVM4CsConstant.OutputPathPkgDirFlag}={configuration.OutputPathPkgDir}"]);
                }

                outputPaths.Add(outputPathsInner);
                args.Add(argsInner);
            }
            Boolean p = _packager.GenerateFiles(filesToUpdateOrdered, outputPaths, args);

            UVMLogger.DumpLogs(UVMConstant.UVM_LOG_FOLDER_PATH, ADCGLogLevelType.TRACE);
            // ###
        }

        /// <summary>
        /// Upgrade the Targeted
        /// </summary>
        /// <param name="configuration"></param>
        public void UpdateTargetEx(UVM4CsConfiguration configuration)
        {
            if (configuration.TargetFiles.Count() == 0)
            {
                Console.WriteLine($"The given arguments do no specify any target files");
                return;
            }


            foreach (string gitDirectory in configuration.UVMConfig.GitDirectories)
            {
                if (!_gitUtils.IsGitDirectory(gitDirectory))
                {
                    Console.WriteLine($"The given path do not lead to a git directory : {gitDirectory}");
                    return;
                }
            }

            if (!_helper.IsUVM4CsRunningConditionMet(configuration))
            {
                return;
            }

            List<I_VersionableFile> vfPool = _reader.ReadCsharpFiles(configuration.UVMConfig.GitDirectories).Cast<I_VersionableFile>().ToList();
            List<I_VersionableFile> vfLeafs = vfPool.Where(vf => configuration.TargetFiles.Contains(vf.VFPath.Replace("\\", "/")) && vf.VFExtension.Equals(".csproj")).Cast<I_VersionableFile>().ToList();

            List<I_VersionableFile> csprojToConsider = UVMManager.ComputeParentTree(vfPool.Cast<I_VersionableFile>().ToList(), vfLeafs.Cast<I_VersionableFile>().ToList());
            List<UVM4CsCsproj> csprojToConsiderCasted = csprojToConsider.Cast<UVM4CsCsproj>().ToList();


            List<UVM4CsCsproj> modifiedVF = _gitUtils.ComputeModifiedCsprojAndCsprojWithModifiedFiles(csprojToConsiderCasted, configuration.UVMConfig.GitDirectories, configuration.UVMConfig.CommitIdsRef, configuration.UVMConfig.CommitIds, UVM4CsHelper.FExtensions);
            if (modifiedVF.Count() == 0)
            {
                return;
            }

            List<List<UVM4CsCsproj>> filesToUpdateOrdered = _manager.ComputeFilesToUpdateOrdered(csprojToConsiderCasted, modifiedVF);

            Boolean u = _updater.UpdateFiles(filesToUpdateOrdered, 0, configuration.UVMConfig.BuildModes[0], configuration.UVMConfig.DigitModes[0], configuration.DevId);
            Boolean d = _dumper.DumpFiles(filesToUpdateOrdered);


            List<List<String>> outputPaths = [];
            List<List<List<String>>> args = [];
            foreach (List<UVM4CsCsproj> x in filesToUpdateOrdered)
            {
                List<String> outputPathsInner = [];
                List<List<String>> argsInner = [];
                foreach (UVM4CsCsproj y in x)
                {
                    outputPathsInner.Add(UVM4CsConstant.DEFAULT_PACKAGE_OUTPUT_DIR_PATH);
                    argsInner.Add([$"{UVM4CsConstant.ConfigBuildFlag}={configuration.Configuration}", $"{UVM4CsConstant.OutputPathPkgDirFlag}={configuration.OutputPathPkgDir}"]);
                }

                outputPaths.Add(outputPathsInner);
                args.Add(argsInner);
            }
            Boolean p = _packager.GenerateFiles(filesToUpdateOrdered, outputPaths, args);

            UVMLogger.DumpLogs(UVMConstant.UVM_LOG_FOLDER_PATH, ADCGLogLevelType.TRACE);
            // ###
        }

        #endregion Public

        #region Protected
        // TBD
        #endregion Protected

        #region Private

        /// <summary>
        /// Reader for <see cref="UVM4CsCsproj"/>.
        /// </summary>
        private UVM4CsReader _reader { get; set; } = new UVM4CsReader();

        /// <summary>
        /// Manager for <see cref="UVM4CsCsproj"/>.
        /// </summary>
        private UVM4CsManager _manager { get; set; } = new UVM4CsManager();

        /// <summary>
        /// Updater for <see cref="UVM4CsCsproj"/>.
        /// </summary>
        private UVM4CsUpdater _updater { get; set; } = new UVM4CsUpdater();

        /// <summary>
        /// Dumper for <see cref="UVM4CsCsproj"/>.
        /// </summary>
        private UVM4CsDumper _dumper { get; set; } = new UVM4CsDumper();

        /// <summary>
        /// Packager for <see cref="UVM4CsCsproj"/>.
        /// </summary>
        private UVM4CsPackager _packager { get; set; } = new UVM4CsPackager();

        /// <summary>
        /// Utilities for git manipulation.
        /// </summary>
        private UVM4CsGitutils _gitUtils { get; set; } = new UVM4CsGitutils();

        /// <summary>
        /// Helper containing bits and bobs for computation.
        /// </summary>
        private UVM4CsHelper _helper { get; set; } = new UVM4CsHelper();

        #endregion Private

        #region DEBUG

        /// <summary>
        /// <see cref="String"/> representation of the assembly name.
        /// </summary>
        // private static String _asmName = Assembly.GetExecutingAssembly().Location ?? String.Empty;

        /// <summary>
        /// <see cref="String"/> representation of the class name.
        /// </summary>
        // private static String _className = nameof(UVM4CsFunction);

        #endregion DEBUG
    }
}
