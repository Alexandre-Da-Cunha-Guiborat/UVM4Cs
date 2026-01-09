using System;
using System.Collections.Generic;
using System.Reflection;
using UVM.Interface.Enums;
using UVM.Logging;
using UVM.Service;

namespace UVM4Cs.Service
{
    /// <summary>
    /// Configuration class for UVM4Cs project.
    /// </summary>
    public class UVM4CsConfiguration
    {

        #region Singleton
        // TBD
        #endregion Singleton

        #region Public

        /// <summary>
        /// Full configuration needed for UVM.
        /// </summary>
        public UVMConfiguration UVMConfig { get; private set; }

        /// <summary>
        /// <see cref="String"/> representation Function mode to use when running UVM4Cs. 
        /// </summary>
        public UVM4CsFunctionMode FunctionMode { get; private set; } = UVM4CsFunctionMode.UVM4CsFunctionMode_NONE;

        /// <summary>
        /// <see cref="String"/> representation branch name. 
        /// </summary>
        public List<String> BranchNames { get; private set; } = [];

        /// <summary>
        /// <see cref="String"/> representation Head mode to use when running UVMUVM4Cs. 
        /// </summary>
        public UVM4CsHeadMode HeadMode { get; private set; } = UVM4CsHeadMode.UVM4CsHeadMode_NONE;

        /// <summary>
        /// <see cref="List{T}"/> of <see cref="String"/> representating the reference branch name. 
        /// </summary>
        public List<String> BranchRefNames { get; private set; } = [];

        /// <summary>
        /// Developer Id to be used for version upgrading.
        /// </summary>
        public UInt16 DevId { get; private set; } = UInt16.MaxValue;

        /// <summary>
        /// <see cref="List{T}"/> of <see cref="String"/> representing the absolute file path to the targets files.
        /// </summary>
        public List<String> TargetFiles { get; private set; } = [];

        /// <summary>
        /// <see cref="String"/> representation of the configuration to use for dotnet CLI.
        /// </summary>
        public String Configuration { get; private set; } = String.Empty;

        /// <summary>
        /// <see cref="String"/> representation of the absolute path to the output directory for package generation.
        /// </summary>
        public String OutputPathPkgDir { get; private set; } = String.Empty;

        /// <summary>
        /// UVM4CsConfiguration constructor.
        /// </summary>
        /// <param name="functionMode">FunctionMode to use when running UVM4Cs.</param>
        /// <param name="gitDirPaths"><see cref="List{T}"/> of <see cref="String"/> representing the absolute path to all git directories to manipulate.</param>
        /// <param name="branchNames"><see cref="List{T}"/> of <see cref="String"/> representing the branch name.</param>
        /// <param name="headMode">HeadMode to use when computing the CommitRefId.</param>
        /// <param name="branchRefNames"><see cref="List{T}"/> of <see cref="String"/> representing the reference branch name.</param>
        /// <param name="buildMode">BuildMode to use when upgrading the targeted project.</param>
        /// <param name="digitMode">DigitMode to use when upgrading the targeted project.</param>
        /// <param name="devId">DevId to use when upgrading the targeted project.</param>
        /// <param name="targetFilesPath"><see cref="List{T}"/> of <see cref="String"/> representing the absolute file path to the targets files.</param>
        /// <param name="configuration"><see cref="String"/> representation of the configuration to use for dotnet CLI.</param>
        /// <param name="outputPathPkgDir"><see cref="String"/> representation of the absolute path to the output directory for package generation.</param>
        public UVM4CsConfiguration(UVM4CsFunctionMode functionMode,
            List<String> gitDirPaths,
            List<String> branchNames,
            UVM4CsHeadMode headMode,
            List<String> branchRefNames,
            BuildType buildMode,
            DigitType digitMode,
            UInt16 devId,
            List<String> targetFilesPath,
            String configuration,
            String outputPathPkgDir)
        {
            FunctionMode = functionMode;
            BranchNames = branchNames;
            HeadMode = headMode;
            BranchRefNames = branchRefNames;

            if (gitDirPaths.Count != branchNames.Count || gitDirPaths.Count != branchRefNames.Count)
            {
                String title = UVMLogger.CreateTitle(_asmName, _className, nameof(UVM4CsConfiguration));
                throw new Exception($"{title} : gitDirPaths, branchNames and branchRefNames must be the same size");
            }

            List<String> uvmconfCommitIds = [];
            for (Int32 i = 0; i < gitDirPaths.Count; i++)
            {
                uvmconfCommitIds.Add(_gitUtils.GetBranchHeadCommitId(gitDirPaths[i], BranchNames[i]));
            }

            List<String> uvmconfCommitIdsRef = [];
            if (HeadMode is UVM4CsHeadMode.HEAD)
            {
                for (Int32 i = 0; i < gitDirPaths.Count; i++)
                {
                    uvmconfCommitIdsRef.Add(_gitUtils.GetBranchHeadCommitId(gitDirPaths[i], BranchRefNames[i]));
                }
            }
            else if (HeadMode is UVM4CsHeadMode.PREVIOUS_COMMIT)
            {
                for (Int32 i = 0; i < gitDirPaths.Count; i++)
                {
                    uvmconfCommitIdsRef.Add(_gitUtils.GetPrevCommitId(gitDirPaths[i], BranchRefNames[i]));
                }
            }

            UVMConfig = new UVMConfiguration(gitDirPaths, uvmconfCommitIdsRef, uvmconfCommitIds, [buildMode], [digitMode]);

            DevId = devId;
            TargetFiles = targetFilesPath;
            Configuration = configuration;
            OutputPathPkgDir = outputPathPkgDir;
        }

        #endregion Public

        #region Protected
        // TBD
        #endregion Protected

        #region Private

        /// <summary>
        /// Utilities for git manipulation.
        /// </summary>
        private UVM4CsGitutils _gitUtils { get; set; } = new UVM4CsGitutils();

        #endregion Private

        #region DEBUG

        /// <summary>
        /// <see cref="String"/> representation of the assembly name.
        /// </summary>
        private static String _asmName = Assembly.GetExecutingAssembly().Location ?? String.Empty;

        /// <summary>
        /// <see cref="String"/> representation of the class name.
        /// </summary>
        private static String _className = nameof(UVM4CsConfiguration);

        #endregion DEBUG
    }
}
