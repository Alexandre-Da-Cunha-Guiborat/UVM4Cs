using ADCG.DevTools.Logging.Enum;
using System;
using UVM.Logging;
using UVM4Cs.CLI;
using UVM4Cs.Common;
using UVM4Cs.Service;

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
#if DEBUGPROJECT
            String functionMode_1 = $"{UVM4CsConstant.FunctionModeFlag}{UVM4CsConstant.FlagArgSeparator}{UVM4CsConstant.FunctionModeFlagWholeRepo}";
            String gitDir_1 = $"{UVM4CsConstant.GitDirFlag}{UVM4CsConstant.FlagArgSeparator}\"C:/Users/dacun/Desktop/20-Workspaces/10-Git/UVM\"";
            String gitBranch_1 = $"{UVM4CsConstant.GitBranchFlag}{UVM4CsConstant.FlagArgSeparator}\"Test_UVM4Cs\"";
            String headMode_1 = $"{UVM4CsConstant.HeadModeFlag}{UVM4CsConstant.FlagArgSeparator}{UVM4CsConstant.HeadModeFlagHead}";
            String gitBranchRef_1 = $"{UVM4CsConstant.GitBranchRefFlag}{UVM4CsConstant.FlagArgSeparator}\"main\"";
            String releaseMode_1 = $"{UVM4CsConstant.ReleaseModeFlag}{UVM4CsConstant.FlagArgSeparator}{UVM4CsConstant.ReleaseModeFlagAlpha}";
            String digitMode_1 = $"{UVM4CsConstant.DigitModeFlag}{UVM4CsConstant.FlagArgSeparator}{UVM4CsConstant.DigitModeFlagSemVer}";
            String semiVer_1 = $"{UVM4CsConstant.SemiVerFlag}{UVM4CsConstant.FlagArgSeparator}7";
            String configuration_1 = $"{UVM4CsConstant.ConfigBuildFlag}{UVM4CsConstant.FlagArgSeparator}Release";
            String target_1 = $"{UVM4CsConstant.TargetsFlag}{UVM4CsConstant.FlagArgSeparator}\"UVM.Service\"";
            String output_1 = $"{UVM4CsConstant.OutputPathPkgDirFlag}{UVM4CsConstant.FlagArgSeparator}{UVM4CsConstant.DEFAULT_PACKAGE_OUTPUT_DIR_PATH}";
            String Debug_CMD_1 = $"./UVM4Cs  {functionMode_1} {gitDir_1} {gitBranch_1} {headMode_1} {gitBranchRef_1} {releaseMode_1} {digitMode_1} {semiVer_1} {configuration_1} {output_1}";

            String functionMode_2 = $"{UVM4CsConstant.FunctionModeFlag}{UVM4CsConstant.FlagArgSeparator}{UVM4CsConstant.FunctionModeFlagTarget}";
            String gitDir_2 = $"{UVM4CsConstant.GitDirFlag}{UVM4CsConstant.FlagArgSeparator}\"C:/Users/dacun/Desktop/20-Workspaces/10-Git/UVM\"";
            String gitBranch_2 = $"{UVM4CsConstant.GitBranchFlag}{UVM4CsConstant.FlagArgSeparator}\"Test_UVM4Cs\"";
            String headMode_2 = $"{UVM4CsConstant.HeadModeFlag}{UVM4CsConstant.FlagArgSeparator}{UVM4CsConstant.HeadModeFlagHead}";
            String gitBranchRef_2 = $"{UVM4CsConstant.GitBranchRefFlag}{UVM4CsConstant.FlagArgSeparator}\"main\"";
            String releaseMode_2 = $"{UVM4CsConstant.ReleaseModeFlag}{UVM4CsConstant.FlagArgSeparator}{UVM4CsConstant.ReleaseModeFlagAlpha}";
            String digitMode_2 = $"{UVM4CsConstant.DigitModeFlag}{UVM4CsConstant.FlagArgSeparator}{UVM4CsConstant.DigitModeFlagSemVer}";
            String semiVer_2 = $"{UVM4CsConstant.SemiVerFlag}{UVM4CsConstant.FlagArgSeparator}7";
            String configuration_2 = $"{UVM4CsConstant.ConfigBuildFlag}{UVM4CsConstant.FlagArgSeparator}Release";
            String target_2 = $"{UVM4CsConstant.TargetsFlag}{UVM4CsConstant.FlagArgSeparator}\"C:/Users/dacun/Desktop/20-Workspaces/10-Git/UVM/40-Service/UVM.Service/UVM.Service.csproj\"";
            String output_2 = $"{UVM4CsConstant.OutputPathPkgDirFlag}{UVM4CsConstant.FlagArgSeparator}{UVM4CsConstant.DEFAULT_PACKAGE_OUTPUT_DIR_PATH}";
            String Debug_CMD_2 = $"./UVM4Cs  {functionMode_2} {gitDir_2} {gitBranch_2} {headMode_2} {gitBranchRef_2} {releaseMode_2} {digitMode_2} {semiVer_2} {configuration_2} {output_2} {target_2}";

            //String cmd = Debug_CMD_1;
            String cmd = Debug_CMD_2;
#else
            String cmd = String.Join(" ", args);
#endif
            CLIParser parser = new CLIParser();
            UVM4CsConfiguration uvmConfiguration = parser.ParseCommand(cmd);
            
            UVM4CsFunction uvmFunction = new UVM4CsFunction();
            if (uvmConfiguration.FunctionMode is UVM4CsFunctionMode.TARGET)
            {
                uvmFunction.UpdateTargetEx(uvmConfiguration);
            }
            else if (uvmConfiguration.FunctionMode is UVM4CsFunctionMode.WHOLE_PROJECT)
            {
                uvmFunction.UpdateWholeRepoEx(uvmConfiguration);
            }
            return;

        }
        catch (Exception e)
        {
            String title = UVMLogger.CreateTitle($"_", $"{nameof(Program)}", $"{nameof(Main)}");
            String message = $"An exception occurred. ({e.GetType()}, {e.Message})";
            UVMLogger.AddLog(ADCGLogLevelType.ERROR, title, message);

            Console.WriteLine(message);
            return;
        }
    }
}
