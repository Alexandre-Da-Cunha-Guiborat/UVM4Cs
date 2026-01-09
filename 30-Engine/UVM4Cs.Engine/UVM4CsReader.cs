using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UVM4Cs.Bll;

namespace UVM4Cs.Engine
{
    /// <summary>
    ///  Library of function for <see cref="UVM4CsCsproj"/> reading.
    /// </summary>
    public class UVM4CsReader
    {
        #region Singleton
        // TBD
        #endregion Singleton

        #region Public

        /// <summary>
        /// Read from the filesystem, all .csproj in the top directory and its children.
        /// </summary>
        /// <param name="topDirPaths"><see cref="List{T}"/> of <see cref="String"/> representing the absolute path to all top directories to consider.</param>
        /// <returns>The <see cref="List{T}"/> of <see cref="UVM4CsCsproj"/> with all csproj under all top directory.</returns>
        public List<UVM4CsCsproj> ReadCsharpFiles(List<String> topDirPaths)
        {
            List<String> slnsPath = FindSlnsPathInChildren(topDirPaths);
            List<String> csprojsPath = FindCsprojsPathInChildren(topDirPaths);

            List<UVM4CsCsproj> csprojs = [];
            foreach (String slnP in slnsPath)
            {
                FileInfo slnFInfo = new FileInfo(slnP);

                foreach (String csprojP in csprojsPath)
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
        /// Read from the filesystem, all .sln in the top directory and its children.
        /// </summary>
        /// <param name="topDirPaths"><see cref="List{T}"/> of <see cref="String"/> representing the absolute path to all top directories to consider.</param>
        /// <returns>The <see cref="List{T}"/> of absolute path to all .sln in the top directory and its children.</returns>
        public List<String> FindSlnsPathInChildren(List<String> topDirPaths)
        {
            List<String> slnPaths = [];
            foreach (String topDirPath in topDirPaths)
            {
                slnPaths = slnPaths.Concat(Directory.GetFiles(topDirPath, "*.sln", SearchOption.AllDirectories).Select(slnPath => slnPath.Replace("\\", "/")).ToList()).ToList();
                slnPaths = slnPaths.Concat(Directory.GetFiles(topDirPath, "*.slnx", SearchOption.AllDirectories).Select(slnPath => slnPath.Replace("\\", "/")).ToList()).ToList();
            }

            return slnPaths;
        }

        /// <summary>
        /// Read from the filesystem, all .csproj in the top directory and its children.
        /// </summary>
        /// <param name="topDirPaths"><see cref="List{T}"/> of <see cref="String"/> representing the absolute path to all top directories to consider.</param>
        /// <returns>The <see cref="List{T}"/> of absolute path to all .csproj in the top directory and its children.</returns>
        public List<String> FindCsprojsPathInChildren(List<String> topDirPaths)
        {

            List<String> csporjPaths = [];

            foreach (String topDirPath in topDirPaths)
            {
                csporjPaths = csporjPaths.Concat(Directory.GetFiles(topDirPath, "*.csproj", SearchOption.AllDirectories).Select(slnPath => slnPath.Replace("\\", "/")).ToList()).ToList();
            }

            return csporjPaths;
        }

        #endregion Public

        #region Protected
        // TBD
        #endregion Protected

        #region Private
        // TBD
        #endregion Private

        #region DEBUG

        /// <summary>
        /// <see cref="String"/> representation of the assembly name.
        /// </summary>
        // private static String _asmName = Assembly.GetExecutingAssembly().Location ?? String.Empty;

        /// <summary>
        /// <see cref="String"/> representation of the class name.
        /// </summary>
        // private static String _className = nameof(UVM4CsReader);

        #endregion DEBUG
    }
}
