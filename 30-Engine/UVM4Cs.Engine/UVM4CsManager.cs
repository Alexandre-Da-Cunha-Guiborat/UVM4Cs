using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using UVM.Engine;
using UVM.Interface;
using UVM.Logging;

namespace UVM4Cs.Engine
{
    /// <summary>
    /// Library of function for Cs package versionning.
    /// </summary>
    public static class UVM4CsManager
    {
        #region DEBUG

        /// <summary>
        /// String representation of the assembly name.
        /// </summary>
        private static readonly string _asmName = "UVM4Cs.Engine";

        /// <summary>
        /// String representation of the class name.
        /// </summary>
        private static readonly string _className = "UVM4CsManager";

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
        /// Compute the list of all files to update. They are ordered like so : first to update first in the list. (Chronological)
        /// </summary>
        /// <param name="vfPool">List of all <see cref="I_VersionnableFile"/> that may need to be managed.</param>
        /// <param name="modifiedFiles">List of all <see cref="I_VersionnableFile"/> that have been modified.</param>
        /// <returns>A list of List of reference to <see cref="I_VersionnableFile"/> needing to be updated. They are ordered like so : first to update first in the list. (Chronological)</returns>
        public static List<List<I_VersionnableFile>> ComputeFilesToUpdateOrdered(
            List<I_VersionnableFile> vfPool,
            List<I_VersionnableFile> modifiedFiles)
        {

            List<I_VersionnableFile> filesToUpdate = UVMManager.ComputeChildrenTree(vfPool, modifiedFiles);
            List<List<I_VersionnableFile>> filesToUpdateOrdered = _ComputeFilesToUpdateOrderedRec([], filesToUpdate, 0, _maxIter);


            string title = UVMLogger.CreateTitle(_asmName, _className, $"ComputeFilesToUpdateOrdered");
            string preface = $"Ordered sublist of files to update";
            foreach (List<I_VersionnableFile> filesToUpdateOrderedSub in filesToUpdateOrdered)
            {
                UVMLogger.AddLogListVF(LogLevel.Trace, title, preface, filesToUpdateOrderedSub);
            }

            return filesToUpdateOrdered;
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

        /// <summary>
        /// Compute the list of all files to update but ordered like a tree. Each Sub List can be updated in //
        /// </summary>
        /// <param name="vfToUpdateOrdered">List of List of the upper layer of the tree already computed.</param>
        /// <param name="vfToUpdatePool">List of all files to update that have not been yet put in the tree.</param>
        /// <param name="nbIter">Current iteration count./param>
        /// <param name="maxIter">Maximum number of recursive call,</param>
        /// <returns>A list of List of reference to <see cref="I_VersionnableFile"/> needing to be updated. They are ordered like so : first to update first in the list. (Chronological)</returns>
        static private List<List<I_VersionnableFile>> _ComputeFilesToUpdateOrderedRec(
            List<List<I_VersionnableFile>> vfToUpdateOrdered,
            List<I_VersionnableFile> vfToUpdatePool,
            int nbIter,
            int maxIter)
        {
            // Base case. (No more file to update)
            if (vfToUpdatePool is null || vfToUpdatePool.Count() == 0)
            {
                return vfToUpdateOrdered;
            }

            // May have reached a circular dependencie ...
            if (nbIter > maxIter)
            {
                //? TODO : Maybe better to throw an exception there ?
                return [];
            }

            // Copy of the vfToUpdatePool to use internaly.
            List<I_VersionnableFile> vfToUpdatePoolCopy = new(vfToUpdatePool);

            // ### Computation of the csproj being part of the curent layer ###
            // List of all csproj in the vfToUpdatePool that have no dependencie in the vfToUpdatePool
            List<I_VersionnableFile> vfToUpdateCurrentLayer = vfToUpdatePool.Where(
                vf => vf.VFDependencies?.Any(dep => vfToUpdatePool.Contains(dep)) is false).ToList();

            vfToUpdatePoolCopy = vfToUpdatePoolCopy.Where(vf => !vfToUpdateCurrentLayer.Contains(vf)).ToList();
            // ###

            string title = UVMLogger.CreateTitle(_asmName, _className, $"_ComputeFilesToUpdateOrderedRec");
            string preface = $"List of all csproj in the vfToUpdatePool that have no dependencie in the vfToUpdatePool";
            UVMLogger.AddLogListVF(LogLevel.Trace, title, preface, vfToUpdateCurrentLayer);

            vfToUpdateOrdered.Add(vfToUpdateCurrentLayer);
            return _ComputeFilesToUpdateOrderedRec(vfToUpdateOrdered, vfToUpdatePoolCopy, nbIter + 1, maxIter);
        }

        #endregion Function

        #region Field

        /// <summary>
        /// Number of iteration max for recursive function. 
        /// </summary>
        private const int _maxIter = 10_000;

        #endregion Field

        #endregion Private
    }
}
