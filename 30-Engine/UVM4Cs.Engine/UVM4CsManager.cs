using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UVM.Engine;
using UVM.Interface.Interfaces;
using UVM.Logging;
using UVM.Logging.Enums;
using UVM4Cs.Bll;

namespace UVM4Cs.Engine
{
    /// <summary>
    /// Library of function for <see cref="UVM4CsCsproj"/> dependency computation.
    /// </summary>
    public class UVM4CsManager
    {

        #region Singleton
        // TBD
        #endregion Singleton

        #region Public

        /// <summary>
        /// Compute the <see cref="List{T}"/> of all <see cref="UVM4CsCsproj"/> to update. They are ordered like so : first to update first in the list. (Chronological)
        /// </summary>
        /// <param name="vfPool"><see cref="List{T}"/> of all <see cref="UVM4CsCsproj"/> that may need to be managed.</param>
        /// <param name="modifiedFiles"><see cref="List{T}"/> of all <see cref="UVM4CsCsproj"/> that have been modified.</param>
        /// <returns>The <see cref="List{T}"/> of <see cref="List{T}"/> of <see cref="UVM4CsCsproj"/> needing to be updated. They are ordered like so : first to update first in the list. (Chronological)</returns>
        public List<List<UVM4CsCsproj>> ComputeFilesToUpdateOrdered(
            List<UVM4CsCsproj> vfPool,
            List<UVM4CsCsproj> modifiedFiles)
        {

            List<I_VersionableFile> vfPoolCasted = vfPool.Cast<I_VersionableFile>().ToList();
            List<I_VersionableFile> modifiedFilesCasted = modifiedFiles.Cast<I_VersionableFile>().ToList();

            List<I_VersionableFile> filesToUpdate = UVMManager.ComputeChildrenTree(vfPoolCasted, modifiedFilesCasted);
            List<UVM4CsCsproj> filesToUpdateCasted = filesToUpdate.Cast<UVM4CsCsproj>().ToList();

            List<List<UVM4CsCsproj>> filesToUpdateOrdered = _ComputeFilesToUpdateOrderedRec([], filesToUpdateCasted, 0, _maxIter);


            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(ComputeFilesToUpdateOrdered)}");
            String preface = $"Ordered sublist of files to update";
            foreach (List<UVM4CsCsproj> filesToUpdateOrderedSub in filesToUpdateOrdered)
            {
                List<I_VersionableFile> filesToUpdateOrderedSubCasted = filesToUpdateOrderedSub.Cast<I_VersionableFile>().ToList();
                UVMLogger.AddLogListVF(E_LogLevel.TRACE, title, preface, filesToUpdateOrderedSubCasted);
            }

            return filesToUpdateOrdered;
        }

        #endregion Public

        #region Protected
        // TBD
        #endregion Protected

        #region Private

        /// <summary>
        /// Number of iteration max for recursive function. 
        /// </summary>
        private const Int32 _maxIter = 10_000;

        /// <summary>
        /// Compute the <see cref="List{T}"/> of all <see cref="I_VersionUVM4CsCsprojableFile"/> to update but ordered like a tree. Each Sub List can be updated in //
        /// </summary>
        /// <param name="vfToUpdateOrdered"><see cref="List{T}"/> of <see cref="List{T}"/> of the upper layer of the tree already computed.</param>
        /// <param name="vfToUpdatePool"><see cref="List{T}"/> of all <see cref="UVM4CsCsproj"/> to update that have not been yet put in the tree.</param>
        /// <param name="nbIter">Current iteration count./param>
        /// <param name="maxIter">Maximum number of recursive call,</param>
        /// <returns>The <see cref="List{T}"/> of <see cref="List{T}"/> of reference to <see cref="UVM4CsCsproj"/> needing to be updated. They are ordered like so : first to update first in the list. (Chronological)</returns>
        private List<List<UVM4CsCsproj>> _ComputeFilesToUpdateOrderedRec(
            List<List<UVM4CsCsproj>> vfToUpdateOrdered,
            List<UVM4CsCsproj> vfToUpdatePool,
            Int32 nbIter,
            Int32 maxIter)
        {
            // Base case. (No more file to update)
            if (vfToUpdatePool is null || vfToUpdatePool.Count() == 0)
            {
                return vfToUpdateOrdered;
            }

            // May have reached a circular dependency ...
            if (nbIter > maxIter)
            {
                String titleMaxIter = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(_ComputeFilesToUpdateOrderedRec)}");
                String prefaceMaxIter = $"The max number of iteration has been reached, either the target is too big or it have a circular dependency. ({nameof(maxIter)}={maxIter})";
                UVMLogger.AddLog(E_LogLevel.ERROR, titleMaxIter, prefaceMaxIter);

                return [];
            }

            // Copy of the vfToUpdatePool to use internally.
            List<UVM4CsCsproj> vfToUpdatePoolCopy = new(vfToUpdatePool);

            // ### Computation of the csproj being part of the curent layer ###
            // List of all csproj in the vfToUpdatePool that have no dependencie in the vfToUpdatePool
            List<UVM4CsCsproj> vfToUpdateCurrentLayer = vfToUpdatePool.Where(
                vf => vf.VFDependencies?.Any(dep => vfToUpdatePool.Contains(dep)) is false).ToList();
            List<I_VersionableFile> vfToUpdateCurrentLayerCasted = vfToUpdateCurrentLayer.Cast<I_VersionableFile>().ToList();

            vfToUpdatePoolCopy = vfToUpdatePoolCopy.Where(vf => !vfToUpdateCurrentLayer.Contains(vf)).ToList();
            // ###

            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(_ComputeFilesToUpdateOrderedRec)}");
            String preface = $"List of all csproj in the vfToUpdatePool that have no dependencie in the vfToUpdatePool";
            UVMLogger.AddLogListVF(E_LogLevel.TRACE, title, preface, vfToUpdateCurrentLayerCasted);

            vfToUpdateOrdered.Add(vfToUpdateCurrentLayer);
            return _ComputeFilesToUpdateOrderedRec(vfToUpdateOrdered, vfToUpdatePoolCopy, nbIter + 1, maxIter);
        }

        #endregion Private

        #region DEBUG

        /// <summary>
        /// <see cref="String"/> representation of the assembly name.
        /// </summary>
        private static String _asmName = Assembly.GetExecutingAssembly().Location ?? String.Empty;

        /// <summary>
        /// <see cref="String"/> representation of the class name.
        /// </summary>
        private static String _className = nameof(UVM4CsManager);

        #endregion DEBUG
    }
}
