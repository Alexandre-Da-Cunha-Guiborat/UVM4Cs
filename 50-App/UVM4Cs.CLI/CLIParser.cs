using System;
using System.Collections.Generic;
using System.Linq;
using UVM.Interface;
using UVM4Cs.Service;

namespace UVM4Cs.CLI
{
    /// <summary>
    /// Command line parser for UVM4Cs.
    /// </summary>
    public static class CLIParser
    {
        #region DEBUG

        /// <summary>
        /// String representation of the assembly name.
        /// </summary>
        private const string _asmName = "UVM4Cs.CLI";

        /// <summary>
        /// String representation of the class name.
        /// </summary>
        private const string _className = "CLIParser";

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

        public static UVM4CsConfiguration ParseCommand(string cmd)
        {
            string[] args = cmd.Split(" ");

            if (args.Count() == 1)
            {
                Console.WriteLine(cmd);
                Console.WriteLine(_usage);
                throw new Exception("The command passed is not a valid command.");
            }

            UVM4CsFunctionMode functionMode = UVM4CsFunctionMode.NONE;
            List<string> gitDirPaths = [];
            List<string> branchNames = [];
            UVM4CsHeadMode headMode = UVM4CsHeadMode.NONE;
            List<string> branchRefNames = [];
            BuildType release = BuildType.NONE;
            DigitType digit = DigitType.NONE;
            UInt16 devId = UInt16.MaxValue;
            List<string> targetFiles = [];

            // Parse the whole command and fill in the configuration.
            for (int idx = 0; idx < args.Count(); idx++)
            {
                string arg = args[idx];
                switch (arg)
                {
                    case _functionModeFlag:
                        if (idx + 1 < args.Count())
                        {
                            if (args[idx + 1].Equals(_functionModeFlagTarget))
                            {
                                functionMode = UVM4CsFunctionMode.TARGET;
                            }
                            else if (args[idx + 1].Equals(_functionModeFlagWholeRepo))
                            {
                                functionMode = UVM4CsFunctionMode.WHOLEPROJECT;
                            }
                            else
                            {
                                Exception exception = new Exception($"The given FunctionMode value is not valid. Please use '{_functionModeFlag} {_functionModeFlagWholeRepo}' or '{_functionModeFlag} {_functionModeFlagTarget}'");
                                throw exception;
                            }

                            idx += 1;
                        }
                        break;

                    case _gitDirFlag:
                        if (idx + 1 < args.Count())
                        {
                            gitDirPaths.Add(args[idx + 1]);
                            idx += 1;
                        }
                        break;

                    case _gitBranchFlag:
                        if (idx + 1 < args.Count())
                        {
                            branchNames.Add(args[idx + 1]);
                            idx += 1;
                        }
                        break;

                    case _gitBranchRefFlag:
                        if (idx + 1 < args.Count())
                        {
                            branchRefNames.Add(args[idx + 1]);
                            idx += 1;
                        }
                        break;

                    case _headModeFlag:
                        if (idx + 1 < args.Count())
                        {
                            if (args[idx + 1].Equals(_headModeFlagPrevCommit))
                            {
                                headMode = UVM4CsHeadMode.PREVCOMMIT;
                            }
                            else if (args[idx + 1].Equals(_headModeFlagHead))
                            {
                                headMode = UVM4CsHeadMode.HEAD;
                            }
                            else
                            {
                                Exception exception = new Exception($"The given HeadMode value is not valid. Please use '{_headModeFlag} {_headModeFlagHead}' or '{_headModeFlag} {_headModeFlagPrevCommit}'");
                                throw exception;
                            }

                            idx += 1;
                        }
                        break;

                    case _releaseModeFlag:
                        if (idx + 1 < args.Count())
                        {
                            if (args[idx + 1] == _releaseModeFlagRelease)
                            {
                                release = BuildType.RELEASE;
                            }
                            else if (args[idx + 1] == _releaseModeFlagAlpha)
                            {
                                release = BuildType.ALPHA;
                            }
                            else if (args[idx + 1] == _releaseModeFlagBeta)
                            {
                                release = BuildType.BETA;
                            }
                            else
                            {
                                Exception exception = new Exception($"The given ReleaseMode value is not valid. Please use '{_releaseModeFlag} {_releaseModeFlagRelease}' or '{_releaseModeFlag} {_releaseModeFlagAlpha}' or '{_releaseModeFlag} {_releaseModeFlagBeta}'");
                                throw exception;
                            }
                            idx += 1;
                        }
                        break;

                    case _digitModeFlag:
                        if (idx + 1 < args.Count())
                        {
                            if (args[idx + 1] == _digitModeFlagMajor)
                            {
                                digit = DigitType.MAJOR;
                            }
                            else if (args[idx + 1] == _digitModeFlagMinor)
                            {
                                digit = DigitType.MINOR;
                            }
                            else if (args[idx + 1] == _digitModeFlagPatch)
                            {
                                digit = DigitType.PATCH;
                            }
                            else if (args[idx + 1] == _digitModeFlagSemVer)
                            {
                                digit = DigitType.SEMVER;
                            }
                            else
                            {
                                Exception exception = new Exception($"The given ReleaseMode value is not valid. Please use '{_digitModeFlag} {_digitModeFlagMajor}' or '{_digitModeFlag} {_digitModeFlagMinor}' or '{_digitModeFlag} {_digitModeFlagPatch}' or '{_digitModeFlag} {_digitModeFlagSemVer}'");
                                throw exception;
                            }
                            idx += 1;
                        }
                        break;

                    case _semverFlag:
                        if (idx + 1 < args.Count())
                        {
                            int semver;
                            if (int.TryParse(args[idx + 1], out semver))
                            {
                                devId = (UInt16)semver;
                            }
                            else
                            {
                                Exception exception = new Exception($"The given SemVer is not a valid value. Please use an integer value. Ex : {_semverFlag} 10");
                                throw exception;
                            }
                            idx += 1;
                        }
                        break;

                    case _targetsFlag:
                        if (idx + 1 < args.Count())
                        {
                            targetFiles.Add(args[idx + 1]);
                            idx += 1;
                        }
                        break;
                }
            }

            UVM4CsConfiguration configuration = new(functionMode, gitDirPaths, branchNames, headMode, branchRefNames, release, digit, devId, targetFiles);
            if (_TestConfigurationValidity(configuration))
            {
                return configuration;
            }
            else
            {
                Console.WriteLine(cmd);
                Console.WriteLine(_usage);
                throw new Exception("Configuration is not valid.");
                // return configuration;
            }

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

        /// <summary>
        /// Test if the configuration given in parameter is fully filled.
        /// </summary>
        /// <param name="configuration">UVMConfiguration to test.</param>
        /// <returns>True if the configuration is valid, False otherwise.</returns>
        private static bool _TestConfigurationValidity(UVM4CsConfiguration configuration)
        {
            bool functionModeValidity = configuration.FunctionMode is not UVM4CsFunctionMode.NONE;
            bool gitDirValidity = !(configuration.UVMConfig.GitDirectories.Count == 0);
            bool branchNameValidity = !(configuration.BranchNames.Count == 0);
            bool headModeValidity = configuration.HeadMode is not UVM4CsHeadMode.NONE;
            bool branchNameRefValidity = !(configuration.BranchRefNames.Count == 0);
            bool buildValidity = !(configuration.UVMConfig.BuildModes.Count == 0);
            bool digitValidity = !(configuration.UVMConfig.DigitModes.Count == 0);
            bool semVerValidity = configuration.DevId is not UInt16.MaxValue;
            bool commitIdValidity = !(configuration.UVMConfig.CommitIds.Count == 0);
            bool commitRefIdValidity = !(configuration.UVMConfig.CommitIdsRef.Count == 0);

            bool validity_part1 = functionModeValidity && gitDirValidity && branchNameValidity && headModeValidity && branchNameRefValidity;
            bool validity_part2 = buildValidity && digitValidity && semVerValidity && commitIdValidity && commitRefIdValidity;
            return validity_part1 && validity_part2;
        }

        #endregion Function

        #region Field

        /// <summary>
        /// String representation of an exemple command line to run the program.
        /// </summary>
        private const string _usage = $"UVM4Cs.exe -F <FunctionMode> -G <PathGitDir> -Gb <BranchName> -Hm <HeadMode> -Gbr <BranchNameRef>  -B <BuildMode> -D <DigitMode> -S <DevId> -P <PathPackageDir> -T <PathToTarget1> -T <PathToTarget2>";

        /// <summary>
        /// String representation of the command flag for the function to call.
        /// </summary>
        private const string _functionModeFlag = "-F";

        /// <summary>
        /// String representation of the command value for upgrading the whole repo.
        /// </summary>
        private const string _functionModeFlagWholeRepo = "WholeProject";

        /// <summary>
        /// String representation of the command value for upgrading only the necessary files to the point where the target get upgraded..
        /// </summary>
        private const string _functionModeFlagTarget = "Target";

        /// <summary>
        /// String representation of the command flag for GitDirectory.
        /// </summary>
        private const string _gitDirFlag = "-G";

        /// <summary>
        /// String representation of the command flag for CommitId.
        /// </summary>
        private const string _commitIdFlag = "-C";

        /// <summary>
        /// String representation of the command flag for the git branch to manage.
        /// </summary>
        private const string _gitBranchFlag = "-Gb";

        /// <summary>
        /// String representation of the command flag for Head mode, either branch/head or branch/prev_commit comparaison.
        /// </summary>
        private const string _headModeFlag = "-Hm";

        /// <summary>
        /// String representation of the command value for comparaison between Gb/head and Gbr/head.
        /// </summary>
        private const string _headModeFlagHead = "Head";

        /// <summary>
        /// String representation of the command value for comparaison between Gb/prev_commit and Gbr/head.
        /// </summary>
        private const string _headModeFlagPrevCommit = "PrevCommit";

        /// <summary>
        /// String representation of the command flag for CommitRefId.
        /// </summary>
        private const string _CommitRefIdFlag = "-Cr";

        /// <summary>
        /// String representation of the command flag for the git branch uses to compare to.
        /// </summary>
        private const string _gitBranchRefFlag = "-Gbr";

        /// <summary>
        /// String representation of the command flag for Release Mode.
        /// </summary>
        private const string _releaseModeFlag = "-B";

        /// <summary>
        /// String representation of the command value for Release Mode Release.
        /// </summary>
        private const string _releaseModeFlagRelease = "Release";

        /// <summary>
        /// String representation of the command value for Release Mode Alpha.
        /// </summary>
        private const string _releaseModeFlagAlpha = "Alpha";

        /// <summary>
        /// String representation of the command value for Release Mode Beta.
        /// </summary>
        private const string _releaseModeFlagBeta = "Beta";

        /// <summary>
        /// String representation of the command flag for DigitMode.
        /// </summary>
        private const string _digitModeFlag = "-D";

        /// <summary>
        /// String representation of the command value for DigitMode Major.
        /// </summary>
        private const string _digitModeFlagMajor = "Major";

        /// <summary>
        /// String representation of the command value for DigitMode Minor.
        /// </summary>
        private const string _digitModeFlagMinor = "Minor";

        /// <summary>
        /// String representation of the command value for DigitMode Patch.
        /// </summary>
        private const string _digitModeFlagPatch = "Patch";

        /// <summary>
        /// String representation of the command value for DigitMode SemiVersion.
        /// </summary>
        private const string _digitModeFlagSemVer = "SemiVer";

        /// <summary>
        /// String representation of the command flag for SemVer.
        /// </summary>
        private const string _semverFlag = "-S";

        /// <summary>
        /// String representation of the command flag for target files.
        /// </summary>
        private const string _targetsFlag = "-T";

        #endregion Field

        #endregion Private

    }
}
