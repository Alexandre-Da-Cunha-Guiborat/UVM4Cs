"""

created by : Alexandre DA CUNHA--GUIBORAT
created the : 12/10/2024 (dd/m/yyyy)

This script aims at removing all obj/ and bin/ in a C# project,before restoring 
the solution to help to handle some issue in a Csharp developpement.

You have to install python3 first to execute this code.
for Ubuntu you can run this in the terminal :
    sudo apt update
    sudo apt upgrade
    sudo apt install python3

"""

import os
import sys
import shutil
import pathlib

if __name__ == "__main__":

    print("Usage, specify -r to restore the solution afterward : $ python3 bin_obj_restore_script.py -r \n\n\n")

    args = sys.argv
    should_restore = False;
    if(len(args) > 1 ) :
        if(args[1] == "-r") :
            should_restore = True;

    script_path, script_name = os.path.split(os.path.abspath(__file__))
    sln_dir = (script_path + "/..")
    print(sln_dir)

    # Remove all bin/ and all obj/
    for path, dirs, files in os.walk(sln_dir):
        _, folder_name = os.path.split(path)

        if folder_name == "obj":
            shutil.rmtree(path)

        if folder_name == "bin":
            shutil.rmtree(path)

    # Restore the solution
    if(should_restore) :
        files = os.listdir(sln_dir)

        sln_file_path = ""
        for f in files:
            f_name, f_ext = os.path.splitext(f)
            if f_ext == ".sln":
                sln_file_path = sln_dir + "/" + f
                break

        restore_cmd = "dotnet restore {sln_file}".format(sln_file=sln_file_path)
        print("Running :" + restore_cmd)
        os.system(restore_cmd)
