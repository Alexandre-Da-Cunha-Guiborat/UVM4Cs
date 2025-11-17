using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UVM.Engine;
using UVM.Interface.Interfaces;
using UVM4Cs.Bll;

namespace UVM4Cs.Engine
{

    /// <summary>
    ///  Library of function for <see cref="UVM4CsCsproj"/> dumping.
    /// </summary>
    public class UVM4CsDumper
    {

        #region Singleton
        // TBD
        #endregion Singleton

        #region Public

        /// <summary>
        /// Dump the <see cref="UVM4CsCsproj"/> to the filesystem.
        /// </summary>
        /// <param name="vfToDump"><see cref="UVM4CsCsproj"/> to dump to the filesystem.</param>
        /// <returns><see langword="true"/> => dump succeed, <see langword="false"/> => otherwise.</returns>
        public Boolean DumpFile(UVM4CsCsproj vfToDump)
        {
            return UVMWriter.DumpFile(vfToDump, vfToDump.VFPath);
        }

        /// <summary>
        /// Dump all <see cref="UVM4CsCsproj"/> to the filesystem.
        /// </summary>
        /// <param name="vfsToDump"><see cref="List{T}"/> of <see cref="UVM4CsCsproj"/> to dump to the filesystem.</param>
        /// <returns><see langword="true"/> => dump succeed, <see langword="false"/> => otherwise.</returns>
        public Boolean DumpFiles(List<UVM4CsCsproj> vfsToDump)
        {
            List<I_VersionableFile> vfsToDumpCasted = vfsToDump.Cast<I_VersionableFile>().ToList();

            List<String> pathsToDump = vfsToDump.Select(f => f.VFPath).ToList();

            Boolean success = true;
            foreach (UVM4CsCsproj fsToDump in vfsToDump)
            {
                if (DumpFile(fsToDump) is false)
                {
                    success = false;
                }
            }

            return success;
        }

        /// <summary>
        /// Dump all <see cref="UVM4CsCsproj"/> to the filesystem.
        /// </summary>
        /// <param name="vfsToDump"><see cref="List{T}"/> of <see cref="List{T}"/> of <see cref="UVM4CsCsproj"/> to dump to the filesystem.</param>
        /// <returns><see langword="true"/> => dump succeed, <see langword="false"/> => otherwise.</returns>
        public Boolean DumpFiles(List<List<UVM4CsCsproj>> vfsToDump)
        {
            Boolean success = true;
            foreach (List<UVM4CsCsproj> fsToDump in vfsToDump)
            {
                if (DumpFiles(fsToDump) is false)
                {
                    success = false;
                }
            }

            return success;
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
        // private static String _className = nameof(UVM4CsWriter);

        #endregion DEBUG
    }
}
