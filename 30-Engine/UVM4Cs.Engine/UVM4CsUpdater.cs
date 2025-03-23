using System.Collections.Generic;
using System;
using UVM.Interface;
using UVM.Logging;
using UVM.Engine;

namespace UVM4Cs.Engine
{

    /// <summary>
    ///  Library of function for .csproj reading.
    /// </summary>
    public static class UVM4CsUpdater
    {

        #region DEBUG

        /// <summary>
        /// String representation of the assembly name.
        /// </summary>
        // private static readonly string _asmName = "UVM4Cs.Engine";

        /// <summary>
        /// String representation of the class name.
        /// </summary>
        // private static readonly string _className = "UVM4CsUpdater";

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
        /// Update the given file's version.
        /// </summary>
        /// <param name="vfToUpdate">VersionnableFile to update</param>
        /// <param name="vIdx">Index of the version to upgrade.</param>
        /// <param name="buildT">BuilType to apply for the update.</param>
        /// <param name="digitToUpgrade">Version's digit to upgrade</param>
        /// <param name="devId">DevId to use for the version's update</param>
        /// <returns>true => update succeed, flase => otherwise.</returns>
        public static bool UpdateFile(
            I_VersionnableFile vfToUpdate, UInt16 vIdx, BuildType buildT, DigitType digitToUpgrade, UInt16 devId)
        {
            return UVMUpdater.UpdateFile(vfToUpdate, [vIdx], [buildT], [digitToUpgrade], [devId]);
        }

        /// <summary>
        /// Update the all file's version.
        /// </summary>
        /// <param name="vfsToUpdateOrdered">List of VersionnableFile to update. (Chronological order.)</param>
        /// <param name="vIdx">Index of the version to upgrade.</param>
        /// <param name="buildT">BuilType to apply for the update.</param>
        /// <param name="digitToUpgrade">Version's digit to upgrade</param>
        /// <param name="devId">DevId to use for the version's update</param>
        /// <returns>true => all update succeed, flase => otherwise.</returns>
        public static bool UpdateFiles(List<I_VersionnableFile> vfsToUpdateOrdered, UInt16 vIdx, BuildType buildT, DigitType digitToUpgrade, UInt16 devId)
        {
            bool result = true;
            foreach (I_VersionnableFile fToUpdate in vfsToUpdateOrdered)
            {
                if (!UpdateFile(fToUpdate, vIdx, buildT, digitToUpgrade, devId))
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Update the all file's version.
        /// </summary>
        /// <param name="vfsToUpdateOrdered">List of VersionnableFile to update. (Chronological order.)</param>
        /// <param name="vIdx">Index of the version to upgrade.</param>
        /// <param name="buildT">BuilType to apply for the update.</param>
        /// <param name="digitToUpgrade">Version's digit to upgrade</param>
        /// <param name="devId">DevId to use for the version's update</param>
        /// <returns>true => all update succeed, false => otherwise.</returns>
        public static bool UpdateFiles(List<List<I_VersionnableFile>> vfsToUpdateOrdered, UInt16 vIdx, BuildType buildT, DigitType digitToUpgrade, UInt16 devId)
        {
            bool result = true;
            foreach (List<I_VersionnableFile> fToUpdates in vfsToUpdateOrdered)
            {
                if (!UpdateFiles(fToUpdates, vIdx, buildT, digitToUpgrade, devId))
                {
                    result = false;
                }
            }

            return result;
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
