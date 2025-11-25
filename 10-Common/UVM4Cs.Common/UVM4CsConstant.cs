using System;
using UVM.Interface;

namespace UVM4Cs.Common
{
    /// <summary>
    /// Class containing constant used within UVM4Cs.
    /// </summary>
    public class UVM4CsConstant
    {
        #region Singleton
        // TBD
        #endregion Singleton

        #region Public

        /// <summary>
        /// <see cref="String"/>git c representation of the absolute path to the directory where to put the newly generated packages.
        /// </summary>
        public const String DEFAULT_PACKAGE_OUTPUT_DIR_PATH = $@"{UVMConstant.UVM_PACKAGE_FOLDER_PATH_DEFAULT}";

        /// <summary>
        /// <see cref="String"/> representation of the separator between the flag and the argument.
        /// </summary>
        public const String FlagArgSeparator = "=";

        /// <summary>
        /// <see cref="String"/> representation of the command flag for the function to call.
        /// </summary>
        public const String FunctionModeFlag = "--FunctionMode";

        /// <summary>
        /// <see cref="String"/> representation of the command value for upgrading the whole repo.
        /// </summary>
        public const String FunctionModeFlagWholeRepo = "WholeProject";

        /// <summary>
        /// <see cref="String"/> representation of the command value for upgrading only the necessary files to the point where the target get upgraded..
        /// </summary>
        public const String FunctionModeFlagTarget = "Target";

        /// <summary>
        /// <see cref="String"/> representation of the command flag for GitDirectory.
        /// </summary>
        public const String GitDirFlag = "--GitDirPath";

        /// <summary>
        /// <see cref="String"/> representation of the command flag for CommitId.
        /// </summary>
        public const String CommitIdFlag = "--CommitId";

        /// <summary>
        /// <see cref="String"/> representation of the command flag for the git branch to manage.
        /// </summary>
        public const String GitBranchFlag = "--GitBranchName";

        /// <summary>
        /// <see cref="String"/> representation of the command flag for Head mode, either branch/head or branch/prev_commit comparaison.
        /// </summary>
        public const String HeadModeFlag = "--HeadMode";

        /// <summary>
        /// <see cref="String"/> representation of the command value for comparaison between Gb/head and Gbr/head.
        /// </summary>
        public const String HeadModeFlagHead = "Head";

        /// <summary>
        /// <see cref="String"/> representation of the command value for comparaison between Gb/prev_commit and Gbr/head.
        /// </summary>
        public const String HeadModeFlagPrevCommit = "PrevCommit";

        /// <summary>
        /// <see cref="String"/> representation of the command flag for CommitRefId.
        /// </summary>
        public const String CommitRefIdFlag = "--CommitRefId";

        /// <summary>
        /// <see cref="String"/> representation of the command flag for the git branch uses to compare to.
        /// </summary>
        public const String GitBranchRefFlag = "--GitBranchRefName";

        /// <summary>
        /// <see cref="String"/> representation of the command flag for Release Mode.
        /// </summary>
        public const String ReleaseModeFlag = "--ReleaseMode";

        /// <summary>
        /// <see cref="String"/> representation of the command value for Release Mode Release.
        /// </summary>
        public const String ReleaseModeFlagRelease = "Release";

        /// <summary>
        /// <see cref="String"/> representation of the command value for Release Mode Alpha.
        /// </summary>
        public const String ReleaseModeFlagAlpha = "Alpha";

        /// <summary>
        /// <see cref="String"/> representation of the command value for Release Mode Beta.
        /// </summary>
        public const String ReleaseModeFlagBeta = "Beta";

        /// <summary>
        /// <see cref="String"/> representation of the command flag for DigitMode.
        /// </summary>
        public const String DigitModeFlag = "--DigitMode";

        /// <summary>
        /// <see cref="String"/> representation of the command value for DigitMode Major.
        /// </summary>
        public const String DigitModeFlagMajor = "Major";

        /// <summary>
        /// <see cref="String"/> representation of the command value for DigitMode Minor.
        /// </summary>
        public const String DigitModeFlagMinor = "Minor";

        /// <summary>
        /// <see cref="String"/> representation of the command value for DigitMode Patch.
        /// </summary>
        public const String DigitModeFlagPatch = "Patch";

        /// <summary>
        /// <see cref="String"/> representation of the command value for DigitMode SemiVersion.
        /// </summary>
        public const String DigitModeFlagSemVer = "SemiVer";

        /// <summary>
        /// <see cref="String"/> representation of the command flag for SemiVer.
        /// </summary>
        public const String SemiVerFlag = "--SemiVer";

        /// <summary>
        /// <see cref="String"/> representation of the command flag for configuration build.
        /// </summary>
        public const String ConfigBuildFlag = "--Configuration";

        /// <summary>
        /// <see cref="String"/> representation of the command flag for target files.
        /// </summary>
        public const String TargetsFlag = "--Target";

        /// <summary>
        /// <see cref="String"/> representation of the command flag for output path.
        /// </summary>
        public const String OutputPathPkgDirFlag = "--Output";

        #endregion Public

        #region Protected
        // TBD
        #endregion Protected

        #region Private
        // TBD
        #endregion Private

        #region DEBUG
        // TBD
        #endregion DEBUG
    }
}
