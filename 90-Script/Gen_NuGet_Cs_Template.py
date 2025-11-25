import os
import pathlib
import sys

from utils.Gen_NuGet_Utils import *

PROJECT_NAME = "Cs_Template"
CSPROJ_FOLDER_RPATH = f"../10-Common/{PROJECT_NAME}"
CSPROJ_NAME = f"{PROJECT_NAME}.csproj"
CSPROJ_RPATH = f"{CSPROJ_FOLDER_RPATH}/{CSPROJ_NAME}"

NUGET_CONFIG_FILE_RPATH = "../nuget.config"
NUGET_OUTPUT_FOLDER_DEBUG = f"/UVM/Packages"

if __name__ == "__main__" :

    # Script
    str__script_apath = os.path.abspath(__file__)
    path__script_parent_apath = pathlib.Path(str__script_apath)
    str__script_parent_apath = str(path__script_parent_apath.parent.absolute()).replace("\\", "/")

    # Csproj
    str__csproj_apath = f"{str__script_parent_apath}/{CSPROJ_RPATH}"
    # print(str__csproj_apath)

    # Configuration
    args = sys.argv
    if(len(args) != 2) :
        exit()

    str__configuration = args[1]

    # NuGet config
    str__nuget_config_file_apath = f"{str__script_parent_apath}/{NUGET_CONFIG_FILE_RPATH}"
    # print(str__nuget_config_file_apath)

    # Pack
    pack_project(str__csproj_apath, str__configuration, str__nuget_config_file_apath, NUGET_OUTPUT_FOLDER_DEBUG)
