import os
import sys
import shutil

def del_sub_tree(str__folder_apath) :
    for apath, _, _ in os.walk(str__folder_apath):
        _, folder_name = os.path.split(apath)

        if folder_name == f"obj":
            shutil.rmtree(apath)

        if folder_name == f"bin":
            shutil.rmtree(apath)

def restore_sln(str__sln_dir_apath) :
    list__files = os.listdir(str__sln_dir_apath)

    str__sln_file_apath = ""
    for f in list__files:
        _, f_ext = os.path.splitext(f)
        if f_ext == ".sln":
            str__sln_file_apath = f"{str__sln_dir_apath}/{f}"
            break

    str__restore_cmd = f"dotnet restore {str__sln_file_apath}"

    print(f"Running : {str__restore_cmd}")
    os.system(str__restore_cmd)

if __name__ == "__main__":

    print(f"Usage, specify -r to restore the solution afterward : $ python3 bin_obj_restore_script.py -r \n\n\n")

    list__args = sys.argv
    bool__should_restore = False;
    if(len(list__args) > 1 and list__args[1] == f"-r") :
        bool__should_restore = True;

    str__script_apath, _ = os.path.split(os.path.abspath(__file__))
    str__sln_dir_apath = f"{str__script_apath}/.."
    # print(str__sln_dir_apath)

    # Remove all bin/ and all obj/
    del_sub_tree(str__sln_dir_apath)

    # Restore the solution
    if(bool__should_restore) :
        restore_sln(str__sln_dir_apath)
