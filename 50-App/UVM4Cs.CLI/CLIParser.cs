using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UVM.Interface.Enums;
using UVM.Logging;
using UVM.Logging.Enums;
using UVM4Cs.Common;
using UVM4Cs.Service;

namespace UVM4Cs.CLI
{
    /// <summary>
    /// Command line parser for UVM4Cs.
    /// </summary>
    public class CLIParser
    {
        #region Singleton
        // TBD
        #endregion Singleton

        #region Public

        /// <summary>
        /// Parse the given command and try to create a configuration from it.
        /// </summary>
        /// <param name="cmd"><see cref="String"/> representation of the command to parse.</param>
        /// <returns>The <see cref="UVM4CsConfiguration"/> corresponding to the command.</returns>
        /// <exception cref="Exception"></exception>
        public UVM4CsConfiguration ParseCommand(String cmd)
        {
            String[] args = cmd.Split(" ");

            if (args.Count() == 1)
            {
                Console.WriteLine(cmd);
                Console.WriteLine(_usage);
                throw new Exception("The command passed is not a valid command.");
            }

            UVM4CsFunctionMode functionMode = UVM4CsFunctionMode.UVM4CsFunctionMode_NONE;
            List<String> gitDirPaths = [];
            List<String> branchNames = [];
            UVM4CsHeadMode headMode = UVM4CsHeadMode.UVM4CsHeadMode_NONE;
            List<String> branchRefNames = [];
            BuildType release = BuildType.BuildType_NONE;
            DigitType digit = DigitType.DigitType_NONE;
            UInt16 devId = UInt16.MaxValue;
            List<String> targetFiles = [];
            String configuration = String.Empty;
            String outputPathPkgDir = UVM4CsConstant.DEFAULT_PACKAGE_OUTPUT_DIR_PATH;

            Dictionary<String, String> flagDictionary = [];
            foreach (String flagValue in args)
            {
                string[] flagValueSplitted = flagValue.Split(UVM4CsConstant.FlagArgSeparator);
                if (flagValueSplitted.Count() != 2)
                {
                    String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(ParseCommand)}");
                    String message = $"A combination Flag{UVM4CsConstant.FlagArgSeparator}Value is not correctly formatted. ({flagValue})";
                    UVMLogger.AddLog(E_LogLevel.ERROR, title, message);
                }
                else
                {
                    flagDictionary[flagValueSplitted[0]] = flagValueSplitted[1];
                }
            }

            foreach (KeyValuePair<string, string> cmdArg in flagDictionary)
            {
                String key = cmdArg.Key;
                String value = cmdArg.Value;

                switch (key)
                {

                    case UVM4CsConstant.FunctionModeFlag:
                        if (value.Equals(UVM4CsConstant.FunctionModeFlagTarget))
                        {
                            functionMode = UVM4CsFunctionMode.TARGET;
                        }
                        else if (value.Equals(UVM4CsConstant.FunctionModeFlagWholeRepo))
                        {
                            functionMode = UVM4CsFunctionMode.WHOLE_PROJECT;
                        }
                        else
                        {
                            Exception exception = new Exception($"The given {nameof(UVM4CsFunctionMode)} value is not valid.");
                            throw exception;
                        }
                        break;

                    case UVM4CsConstant.GitDirFlag:
                        gitDirPaths.Add(value.Replace("\"", ""));
                        break;

                    case UVM4CsConstant.GitBranchFlag:
                        branchNames.Add(value.Replace("\"", ""));
                        break;

                    case UVM4CsConstant.GitBranchRefFlag:
                        branchRefNames.Add(value.Replace("\"", ""));
                        break;

                    case UVM4CsConstant.HeadModeFlag:
                        if (value.Equals(UVM4CsConstant.HeadModeFlagPrevCommit))
                        {
                            headMode = UVM4CsHeadMode.PREVIOUS_COMMIT;
                        }
                        else if (value.Equals(UVM4CsConstant.HeadModeFlagHead))
                        {
                            headMode = UVM4CsHeadMode.HEAD;
                        }
                        else
                        {
                            Exception exception = new Exception($"The given {nameof(UVM4CsHeadMode)} value is not valid.");
                            throw exception;
                        }
                        break;

                    case UVM4CsConstant.ReleaseModeFlag:
                        if (value.Equals(UVM4CsConstant.ReleaseModeFlagRelease))
                        {
                            release = BuildType.RELEASE;
                        }
                        else if (value.Equals(UVM4CsConstant.ReleaseModeFlagAlpha))
                        {
                            release = BuildType.ALPHA;
                        }
                        else if (value.Equals(UVM4CsConstant.ReleaseModeFlagBeta))
                        {
                            release = BuildType.BETA;
                        }
                        else
                        {
                            Exception exception = new Exception($"The given {nameof(BuildType)} value is not valid.");
                            throw exception;
                        }
                        break;

                    case UVM4CsConstant.DigitModeFlag:
                        if (value.Equals(UVM4CsConstant.DigitModeFlagMajor))
                        {
                            digit = DigitType.MAJOR;
                        }
                        else if (value.Equals(UVM4CsConstant.DigitModeFlagMinor))
                        {
                            digit = DigitType.MINOR;
                        }
                        else if (value.Equals(UVM4CsConstant.DigitModeFlagPatch))
                        {
                            digit = DigitType.PATCH;
                        }
                        else if (value.Equals(UVM4CsConstant.DigitModeFlagSemVer))
                        {
                            digit = DigitType.SEMI_VERSION;
                        }
                        else
                        {
                            Exception exception = new Exception($"The given {nameof(DigitType)} value is not valid.");
                            throw exception;
                        }
                        break;

                    case UVM4CsConstant.SemiVerFlag:
                        UInt16 semver;
                        if (UInt16.TryParse(value, out semver))
                        {
                            devId = (UInt16)semver;
                        }
                        else
                        {
                            Exception exception = new Exception($"The given Semi Version is not a valid value.");
                            throw exception;
                        }
                        break;

                    case UVM4CsConstant.TargetsFlag:
                        targetFiles.Add(value.Replace("\"", ""));
                        break;

                    case UVM4CsConstant.ConfigBuildFlag:
                        configuration = value.Replace("\"", ""); ;
                        break;

                    case UVM4CsConstant.OutputPathPkgDirFlag:
                        outputPathPkgDir = value.Replace("\"", ""); ;
                        break;
                }
            }

            UVM4CsConfiguration uvmConfiguration = new(functionMode, gitDirPaths, branchNames, headMode, branchRefNames, release, digit, devId, targetFiles, configuration, outputPathPkgDir);
            if (_TestConfigurationValidity(uvmConfiguration))
            {
                return uvmConfiguration;
            }
            else
            {
                Console.WriteLine(cmd);
                Console.WriteLine(_usage);
                throw new Exception($"The given command is not valid. ({cmd})");
            }

        }

        #endregion Public

        #region Protected
        // TBD
        #endregion Protected

        #region Private

        /// <summary>
        /// <see cref="String"/> representation of an example command line to run the program.
        /// </summary>
        private const String _usage =
            $"usage :\n" +
            $"UVM4Cs " +
            $"{UVM4CsConstant.FunctionModeFlag}<FunctionMode> " +
            $"{UVM4CsConstant.GitDirFlag}<PathGitDir> " +
            $"{UVM4CsConstant.GitBranchFlag}<BranchName> " +
            $"{UVM4CsConstant.HeadModeFlag}<HeadMode> " +
            $"{UVM4CsConstant.GitBranchRefFlag}<BranchNameRef> " +
            $"{UVM4CsConstant.ReleaseModeFlag}<BuildMode> " +
            $"{UVM4CsConstant.DigitModeFlag}<DigitMode> " +
            $"{UVM4CsConstant.SemiVerFlag}<DevId> " +
            $"{UVM4CsConstant.ConfigBuildFlag}<Configuration> " +
            $"{UVM4CsConstant.OutputPathPkgDirFlag}<OutputPathPackageDir> (opt) (default : \"{UVM4CsConstant.DEFAULT_PACKAGE_OUTPUT_DIR_PATH}\") " +
            $"{UVM4CsConstant.TargetsFlag}<PathToTarget1> (opt) " +
            $"{UVM4CsConstant.TargetsFlag}<PathToTarget2> (opt) " +
            $"\n";

        /// <summary>
        /// Test if the configuration given in parameter is fully filled.
        /// </summary>
        /// <param name="configuration">UVMConfiguration to test.</param>
        /// <returns>True if the configuration is valid, False otherwise.</returns>
        private Boolean _TestConfigurationValidity(UVM4CsConfiguration configuration)
        {
            Boolean functionModeValidity = configuration.FunctionMode is not UVM4CsFunctionMode.UVM4CsFunctionMode_NONE;
            Boolean gitDirValidity = !(configuration.UVMConfig.GitDirectories.Count == 0);
            Boolean branchNameValidity = !(configuration.BranchNames.Count == 0);
            Boolean headModeValidity = configuration.HeadMode is not UVM4CsHeadMode.UVM4CsHeadMode_NONE;
            Boolean branchNameRefValidity = !(configuration.BranchRefNames.Count == 0);
            Boolean buildValidity = !(configuration.UVMConfig.BuildModes.Count == 0);
            Boolean digitValidity = !(configuration.UVMConfig.DigitModes.Count == 0);
            Boolean semVerValidity = configuration.DevId is not UInt16.MaxValue;
            Boolean commitIdValidity = !(configuration.UVMConfig.CommitIds.Count == 0);
            Boolean commitRefIdValidity = !(configuration.UVMConfig.CommitIdsRef.Count == 0);
            Boolean configurationValidity = !(String.IsNullOrEmpty(configuration.Configuration));

            Boolean validity_part1 = functionModeValidity && gitDirValidity && branchNameValidity && headModeValidity && branchNameRefValidity;
            Boolean validity_part2 = buildValidity && digitValidity && semVerValidity && commitIdValidity && commitRefIdValidity && configurationValidity;
            return validity_part1 && validity_part2;
        }

        #endregion Private

        #region DEBUG

        /// <summary>
        /// <see cref="String"/> representation of the assembly name.
        /// </summary>
        private static String _asmName = Assembly.GetExecutingAssembly().Location ?? String.Empty;

        /// <summary>
        /// <see cref="String"/> representation of the class name.
        /// </summary>
        private static String _className = nameof(CLIParser);

        #endregion DEBUG
    }
}
