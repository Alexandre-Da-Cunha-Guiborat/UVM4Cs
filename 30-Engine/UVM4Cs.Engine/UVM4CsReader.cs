using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using UVM.Interface;
using UVM4Cs.Bll;

namespace UVM4Cs.Engine
{
    /// <summary>
    ///  Library of function for csproj reading.
    /// </summary>
    public static class UVM4CsReader
    {
        #region DEBUG

        /// <summary>
        /// String representation of the assembly name.
        /// </summary>
        // private static readonly string _asmName = "UVM4Cs.Engine";

        /// <summary>
        /// String representation of the class name.
        /// </summary>
        // private static readonly string _className = "UVM4CsReader";

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

        /// <summary>
        /// Read from the filesystem, all .csproj in the top directory and its childrens.
        /// </summary>
        /// <param name="topDirPaths">List of string representating the absolute path to all top directories to consider.</param>
        /// <returns>A list of <see cref="I_VersionnableFile"/> with all csproj under all top directory.</returns>
        public static List<I_VersionnableFile> ReadCsharpFiles(List<string> topDirPaths)
        {
            List<string> slnsPath = FindSlnsPathInChildren(topDirPaths);
            List<string> csprojsPath = FindCsprojsPathInChildren(topDirPaths);

            List<I_VersionnableFile> csprojs = [];
            foreach (string slnP in slnsPath)
            {
                FileInfo slnFInfo = new FileInfo(slnP);

                foreach (string csprojP in csprojsPath)
                {
                    if (slnFInfo.DirectoryName is not null && csprojP.Contains(slnFInfo.DirectoryName.Replace("\\", "/")))
                    {
                        csprojs.Add(new UVM4CsCsproj(csprojP, slnP));
                    }
                }
            }

            return csprojs;
        }

        /// <summary>
        /// Read from the filesystem, all .sln in the top directory and its childrens.
        /// </summary>
        /// <param name="topDirPaths">List of string representating the absolute path to all top directories to consider.</param>
        /// <returns>A list of absolute path to all .sln in the top directory and its childrens.</returns>
        public static List<string> FindSlnsPathInChildren(List<string> topDirPaths)
        {
            List<string> slnPaths = [];

            foreach (string topDirPath in topDirPaths)
            {
                slnPaths = slnPaths.Concat(Directory.GetFiles(topDirPath, "*.sln", SearchOption.AllDirectories).Select(slnPath => slnPath.Replace("\\", "/")).ToList()).ToList();
            }

            return slnPaths;
        }

        /// <summary>
        /// Read from the filesystem, all .csproj in the top directory and its childrens.
        /// </summary>
        /// <param name="topDirPaths">List of string representating the absolute path to all top directories to consider.</param>
        /// <returns>A list of absolute path to all .csproj in the top directory and its childrens.</returns>
        public static List<string> FindCsprojsPathInChildren(List<string> topDirPaths)
        {

            List<string> csporjPaths = [];

            foreach (string topDirPath in topDirPaths)
            {
                csporjPaths = csporjPaths.Concat(Directory.GetFiles(topDirPath, "*.csproj", SearchOption.AllDirectories).Select(slnPath => slnPath.Replace("\\", "/")).ToList()).ToList();
            }

            return csporjPaths;
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
        // TBD
        #endregion Function

        #region Field
        // TBD
        #endregion Field

        #endregion Private

    }
}
