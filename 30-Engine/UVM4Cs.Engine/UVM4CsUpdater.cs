using System;
using System.Collections.Generic;
using System.Reflection;
using UVM.Engine;
using UVM.Interface.Enums;
using UVM4Cs.Bll;

namespace UVM4Cs.Engine
{

    /// <summary>
    ///  Library of function for <see cref="UVM4CsCsproj"/> updating.
    /// </summary>
    public class UVM4CsUpdater
    {

        #region Singleton
        // TBD
        #endregion Singleton

        #region Public

        /// <summary>
        /// Update the given file's version.
        /// </summary>
        /// <param name="vfToUpdate"><see cref="UVM4CsCsproj"/> to update</param>
        /// <param name="vIdx">Index of the version to upgrade.</param>
        /// <param name="buildT"><see cref="BuildType"/> to apply for the update.</param>
        /// <param name="digitToUpgrade">Version's digit to upgrade</param>
        /// <param name="devId">DevId to use for the version's update</param>
        /// <returns><see langword="true"/> => update succeed, <see langword="false"/> => otherwise.</returns>
        public Boolean UpdateFile(UVM4CsCsproj vfToUpdate, UInt16 vIdx, BuildType buildT, DigitType digitToUpgrade, UInt16 devId)
        {
            return UVMUpdater.UpdateFile(vfToUpdate, [vIdx], [buildT], [digitToUpgrade], [devId]);
        }

        /// <summary>
        /// Update the all file's version.
        /// </summary>
        /// <param name="vfsToUpdateOrdered"><see cref="List{T}"/> of <see cref="UVM4CsCsproj"/> to update. (Chronological order.)</param>
        /// <param name="vIdx">Index of the version to upgrade.</param>
        /// <param name="buildT"><see cref="BuildType"/> to apply for the update.</param>
        /// <param name="digitToUpgrade">Version's digit to upgrade</param>
        /// <param name="devId">DevId to use for the version's update</param>
        /// <returns><see langword="true"/> => update succeed, <see langword="false"/> => otherwise.</returns>
        public Boolean UpdateFiles(List<UVM4CsCsproj> vfsToUpdateOrdered, UInt16 vIdx, BuildType buildT, DigitType digitToUpgrade, UInt16 devId)
        {
            Boolean success = true;
            foreach (UVM4CsCsproj fToUpdate in vfsToUpdateOrdered)
            {
                if (!UpdateFile(fToUpdate, vIdx, buildT, digitToUpgrade, devId))
                {
                    success = false;
                }
            }

            return success;
        }

        /// <summary>
        /// Update the all file's version.
        /// </summary>
        /// <param name="vfsToUpdateOrdered"><see cref="List{T}"/> of <see cref="List{T}"/> of <see cref="UVM4CsCsproj"/> to update. (Chronological order.)</param>
        /// <param name="vIdx">Index of the version to upgrade.</param>
        /// <param name="buildT"><see cref="BuildType"/> to apply for the update.</param>
        /// <param n1ame="digitToUpgrade">Version's digit to upgrade</param>
        /// <param name="devId">DevId to use for the version's update</param>
        /// <returns><see langword="true"/> => update succeed, <see langword="false"/> => otherwise.</returns>
        public Boolean UpdateFiles(List<List<UVM4CsCsproj>> vfsToUpdateOrdered, UInt16 vIdx, BuildType buildT, DigitType digitToUpgrade, UInt16 devId)
        {
            Boolean success = true;
            foreach (List<UVM4CsCsproj> fToUpdates in vfsToUpdateOrdered)
            {
                if (!UpdateFiles(fToUpdates, vIdx, buildT, digitToUpgrade, devId))
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
        // private static String _className = nameof(UVM4CsUpdater);

        #endregion DEBUG
    }
}
