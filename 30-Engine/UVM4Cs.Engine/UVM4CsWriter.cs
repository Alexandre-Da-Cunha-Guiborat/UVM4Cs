using System.Collections.Generic;
using System.Linq;
using UVM.Engine;
using UVM.Interface;

namespace UVM4Cs.Engine
{

    /// <summary>
    ///  Library of function for .csproj dumping.
    /// </summary>
    public static class UVM4CsWriter
    {
        #region DEBUG

        /// <summary>
        /// String representation of the assembly name.
        /// </summary>
        // private static readonly string _asmName = "UVM4Cs.Engine";

        /// <summary>
        /// String representation of the class name.
        /// </summary>
        //private static readonly string _className = "UVM4CsWriter";

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
        /// Dump the VersionnableFiles to the filesystem.
        /// </summary>
        /// <param name="vfToDump">VersionnableFiles to dump to the filesystem.</param>
        /// <returns>true => dump succeed, false => otherwise.</returns>
        public static bool DumpFile(I_VersionnableFile vfToDump)
        {
            UVMWriter.DumpFile(vfToDump, vfToDump.VFPath);
            return true;
        }

        /// <summary>
        /// Dump all VersionnableFiles to the filesystem.
        /// </summary>
        /// <param name="vfsToDump">List of VersionnableFiles to dump to the filesystem.</param>
        /// <returns>true => all dumped succeed, false => otherwise.</returns>
        public static bool DumpFiles(List<I_VersionnableFile> vfsToDump)
        {
            List<string> pathsToDump = vfsToDump.Select(f => f.VFPath).ToList();
            UVMWriter.DumpFiles(vfsToDump, pathsToDump);
            return true;
        }

        /// <summary>
        /// Dump all VersionnableFiles to the filesystem.
        /// </summary>
        /// <param name="vfsToDump">List of List of VersionnableFiles to dump to the filesystem.</param>
        /// <returns>true => all dumped succeed, false => otherwise.</returns>
        public static bool DumpFiles(List<List<I_VersionnableFile>> vfsToDump)
        {

            foreach (List<I_VersionnableFile> fsToDump in vfsToDump)
            {
                DumpFiles(fsToDump);
            }

            return true;
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
