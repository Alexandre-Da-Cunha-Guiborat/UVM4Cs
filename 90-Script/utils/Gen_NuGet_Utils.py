import os

def run_cmd(str__cmd) :
    print(f"Running > {str__cmd}")
    os.system(str__cmd)

def dotnet_clean(str__sln_or_csproj_apath, str__configuration, str__opts) :
    str__cmd_clean = f"dotnet clean {str__sln_or_csproj_apath} --configuration {str__configuration} {str__opts}"
    run_cmd(str__cmd_clean)

def dotnet_restore(str__sln_or_csproj_apath, str__nuget_config_file_apath, str__opts) :
    str__cmd_restore = f"dotnet restore {str__sln_or_csproj_apath} --configfile {str__nuget_config_file_apath} {str__opts}"
    run_cmd(str__cmd_restore)

def dotnet_build(str__sln_or_csproj_apath, str__configuration, str__opts) :
    str__cmd_build = f"dotnet build {str__sln_or_csproj_apath} --configuration {str__configuration} {str__opts}"
    run_cmd(str__cmd_build)

def dotnet_pack(str__sln_or_csproj_apath, str__configuration, str__output_folder_apath, str__opts) :
    str__cmd_pack = f"dotnet pack {str__sln_or_csproj_apath} --configuration {str__configuration} --output {str__output_folder_apath} {str__opts}"
    run_cmd(str__cmd_pack)

def pack_debug(str__csproj_apath, str__nuget_config_file_apath, str__output_folder_apath) :
    dotnet_clean(str__csproj_apath, f"Debug", f"--verbosity quiet")
    dotnet_restore(str__csproj_apath, str__nuget_config_file_apath, "--verbosity quiet")
    dotnet_build(str__csproj_apath, f"Debug", f"--verbosity quiet")
    dotnet_pack(str__csproj_apath, f"Debug", str__output_folder_apath, f"--verbosity quiet --include-symbols")

def pack_release(str__csproj_apath, str__nuget_config_file_apath, str__output_folder_apath) :
    dotnet_clean(str__csproj_apath, f"Release", f"--verbosity quiet")
    dotnet_restore(str__csproj_apath, str__nuget_config_file_apath, "--verbosity quiet")
    dotnet_build(str__csproj_apath, f"Release", f"--verbosity quiet")
    dotnet_pack(str__csproj_apath, f"Release", str__output_folder_apath, f"--verbosity quiet")

def pack_project(str__csproj_apath, str__configuration, str__nuget_config_file_apath, str__output_folder_apath) :

    if(str__configuration== "Debug") :
        pack_debug(str__csproj_apath, str__nuget_config_file_apath, str__output_folder_apath)
    else :
        pack_release(str__csproj_apath, str__nuget_config_file_apath, str__output_folder_apath)