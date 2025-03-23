using System;
using UVM4Cs.CLI;
using UVM4Cs.Service;

// UVM4Cs.exe -F <FunctionMode> -G <PathGitDir> -Gb <BranchName> -Hm <HeadMode> -Gbr <BranchNameRef>  -B <BuildMode> -D <DigitMode> -S <DevId> -T <PathToTarget1> -T <PathToTarget2>
int main()
{
    try
    {
#if DEBUG
        string Debug_CMD_1 = $"./UVM4Cs -F WholeProject -G C:/Users/dacun/Desktop/Workspace/UniversalVersionManager/UVM -Gb Test_UVM4Cs -Hm Head -Gbr main -B Alpha -D SemiVer -S 7 ";
        string Debug_CMD_2 = $"./UVM4Cs -F Target -G C:/Users/dacun/Desktop/Workspace/UniversalVersionManager/UVM -Gb Test_UVM4Cs -Hm Head -Gbr main -B Alpha -D SemiVer -S 7 -T UVM.Service";

        string cmd = Debug_CMD_1;
#else
        string[] args = Environment.GetCommandLineArgs();
        string cmd = string.Join(" ", args);
#endif

        UVM4CsConfiguration configuration = CLIParser.ParseCommand(cmd);

        if (configuration.FunctionMode is UVM4CsFunctionMode.TARGET)
        {
            UVM4CsFunction.UpdateTargetEx(configuration);
        }
        else if (configuration.FunctionMode is UVM4CsFunctionMode.WHOLEPROJECT)
        {
            UVM4CsFunction.UpdateWholeRepoEx(configuration);
        }
        return 0;

    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return 1;
    }

}

main();