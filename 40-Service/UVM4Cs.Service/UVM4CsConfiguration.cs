using UVM.Interface;
using UVM.Service;
using System;
using System.Collections.Generic;

namespace UVM4Cs.Service
{
    /// <summary>
    /// Configuration class for UVM4Cs project.
    /// </summary>
    public class UVM4CsConfiguration
    {
        #region DEBUG

        /// <summary>
        /// String representation of the assembly name.
        /// </summary>
        private const string _asmName = "UVM4Cs.Service";

        /// <summary>
        /// String representation of the class name.
        /// </summary>
        private const string _className = "UVM4CsConfiguration";

        #endregion DEBUG

        #region Public

        #region Constructor

        /// <summary>
        /// UVM4CsConfiguration constructor.
        /// </summary>
        /// <param name="functionMode">FunctionMode to use when running UVM4Cs.</param>
        /// <param name="gitDirPaths">List of string representating the absolute path to all git directories to manipulate.</param>
        /// <param name="branchNames">List of string representating the branch name.</param>
        /// <param name="headMode">HeadMode to use when computing the CommitRefId.</param>
        /// <param name="branchRefNames">List of string representating the reference branch name.</param>
        /// <param name="buildMode">BuildMode to use when upgrading the targeted project.</param>
        /// <param name="digitMode">DigitMode to use when upgrading the targeted project.</param>
        /// <param name="devId">DevId to use when upgrading the targeted project.</param>
        /// <param name="targetFilesPath">List of string representing the absolute file path to the targets files.</param>
        public UVM4CsConfiguration(UVM4CsFunctionMode functionMode,
            List<string> gitDirPaths,
            List<string> branchNames,
            UVM4CsHeadMode headMode,
            List<string> branchRefNames,
            BuildType buildMode,
            DigitType digitMode,
            UInt16 devId,
            List<string> targetFilesPath)
        {
            FunctionMode = functionMode;
            BranchNames = branchNames;
            HeadMode = headMode;
            BranchRefNames = branchRefNames;
            DevId = devId;

            if (gitDirPaths.Count != branchNames.Count || gitDirPaths.Count != branchRefNames.Count)
            {
                throw new Exception($"{_asmName} | {_className} | UVM4CsConfiguration : gitDirPaths, branchNames and branchRefNames must be the same size");
            }

            List<string> uvmconfCommitIds = [];
            for (int i = 0; i < gitDirPaths.Count; i++)
            {
                uvmconfCommitIds.Add(UVM4CsGitutils.GetBranchHeadCommitId(gitDirPaths[i], BranchNames[i]));
            }

            List<string> uvmconfCommitIdsRef = [];
            if (HeadMode is UVM4CsHeadMode.HEAD)
            {
                for (int i = 0; i < gitDirPaths.Count; i++)
                {
                    uvmconfCommitIdsRef.Add(UVM4CsGitutils.GetBranchHeadCommitId(gitDirPaths[i], BranchRefNames[i]));
                }
            }
            else if (HeadMode is UVM4CsHeadMode.PREVCOMMIT)
            {
                for (int i = 0; i < gitDirPaths.Count; i++)
                {
                    uvmconfCommitIdsRef.Add(UVM4CsGitutils.GetPrevCommitId(gitDirPaths[i], BranchRefNames[i]));
                }
            }

            UVMConfig = new UVMConfiguration(gitDirPaths, uvmconfCommitIdsRef, uvmconfCommitIds, [buildMode], [digitMode]);

            TargetFiles = targetFilesPath;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Full configuration needed for UVM.
        /// </summary>
        public UVMConfiguration UVMConfig { get; private set; }

        /// <summary>
        /// String representation Function mode to use when running UVMUVM4Cs. 
        /// </summary>
        public UVM4CsFunctionMode FunctionMode { get; private set; } = UVM4CsFunctionMode.NONE;

        /// <summary>
        /// String representation branch name. 
        /// </summary>
        public List<string> BranchNames { get; private set; } = [];

        /// <summary>
        /// String representation Head mode to use when running UVMUVM4Cs. 
        /// </summary>
        public UVM4CsHeadMode HeadMode { get; private set; } = UVM4CsHeadMode.NONE;

        /// <summary>
        /// List of string representating the reference branch name. 
        /// </summary>
        public List<string> BranchRefNames { get; private set; } = [];

        /// <summary>
        /// Developper Id to be used for version upgrading.
        /// </summary>
        public UInt16 DevId { get; private set; } = UInt16.MaxValue;


        /// <summary>
        /// List of string representing the absolute file path to the targets files.
        /// </summary>
        public List<string> TargetFiles { get; private set; } = [];

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
