using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using UVM.Engine;
using UVM.Interface;
using UVM.Logging;
using UVM4Cs.Bll;

namespace UVM4Cs.Engine
{
    /// <summary>
    ///  Library of function for Cs package versionne upgrading.
    /// </summary>
    public static class UVM4CsPackager
    {
        #region DEBUG

        /// <summary>
        /// String representation of the assembly name.
        /// </summary>
        // private static readonly string _asmName = "UVM4Cs.Engine";

        /// <summary>
        /// String representation of the class name.
        /// </summary>
        // private static readonly string _className = "UVM4CsPackager";

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
        /// Generate the given file.
        /// </summary>
        /// <returns>true => generation succeed. false => otherwise.</returns>
        public static bool GenerateFile(I_GenerableFile vfToGenerate)
        {
            return vfToGenerate.Generate();
        }

        /// <summary>
        /// Generate the given file. (pass it the arguments.)
        /// </summary>
        /// <param name="vfToGenerate">File to generate.</param>
        /// <param name="outputPath">String representation of the absolute path to the location to generate the package.</param>
        /// <param name="args">List of string used to specify arguments for generation.</param>
        /// <returns>true => generation successed, false => otherwise</returns>
        public static bool GenerateFile(I_GenerableFile vfToGenerate, string outputPath, List<string> args)
        {
            return UVMPackager.GenerateFile(vfToGenerate, outputPath, args);
        }

        /// <summary>
        /// Generate all files.
        /// </summary>
        /// <returns>true => all generation succeed. false => otherwise.</returns>
        public static bool GenerateFiles(List<I_GenerableFile> vfsToGenerateOrdered)
        {
            bool result = true;
            foreach (List<I_GenerableFile> fToGenerate in vfsToGenerateOrdered)
            {
                if (GenerateFiles(fToGenerate) is false)
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// Generate all files. (pass to each file the arguments (use positioning. file[i], path[i], args[i])).
        /// </summary>
        /// <param name="vfsToGenerateOrdered">List of all files to generate. They must be ordered in the chronological way.</param>
        /// <param name="outputPaths">List of string representation of the absolute path to the location to generate the package.</param>
        /// <param name="args">List of List of string used to specifie arguments for each files.</param>
        /// <returns>true => all generation succeed. false => otherwise.</returns>
        public static bool GenerateFiles(List<I_GenerableFile> vfsToGenerateOrdered, List<string> outputPaths, List<List<string>> args)
        {
            return UVMPackager.GenerateFiles(vfsToGenerateOrdered, outputPaths, args);
        }

        /// <summary>
        /// Generate all files.
        /// </summary>
        /// <returns>true => all generation succeed. false => otherwise.</returns>
        public static bool GenerateFiles(List<List<I_GenerableFile>> filesToGenerateOrdered)
        {
            bool result = true;
            foreach (List<I_GenerableFile> subFilesToGenerateOrderedPar in filesToGenerateOrdered)
            {
                if (GenerateFiles(subFilesToGenerateOrderedPar) is false)
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Generate all files. (pass to each file the arguments (use positioning. file[i], path[i], args[i])).
        /// </summary>
        /// <param name="vfsToGenerateOrdered">List of List of all files to generate. They must be ordered in the chronological way.</param>
        /// <param name="outputPaths">List of List of string representation of the absolute path to the location to generate the package.</param>
        /// <param name="args">List of List of List of string used to specifie arguments for each files.</param>
        /// <returns>true => all generation succeed. false => otherwise.</returns>
        public static bool GenerateFiles(List<List<I_GenerableFile>> vfsToGenerateOrdered, List<List<string>> outputPaths, List<List<List<string>>> args)
        {
            return UVMPackager.GenerateFiles(vfsToGenerateOrdered, outputPaths, args);
        }

        // <summary>
        /// Compute the list of all generable files in the given list.
        /// </summary>
        /// <param name="filesMayBeGenerate">List of List of all files that must be updated. (<see cref="I_VersionnableFile">)</param>
        /// <returns>List of all generable files in the given list.</returns>
        public static List<List<I_GenerableFile>> GetGenerableFiles(List<List<I_VersionnableFile>> vfPool)
        {

            List<List<I_GenerableFile>> filesToGenerateOrdered = [];
            foreach (List<I_VersionnableFile> filesMaybeGenerateSub in vfPool)
            {
                List<I_GenerableFile> filesToGenerateSub = [];
                foreach (I_VersionnableFile fMaybeGenerate in filesMaybeGenerateSub)
                {
                    if (fMaybeGenerate.VFExtension.Equals(".csproj"))
                    {
                        filesToGenerateSub.Add((UVM4CsCsproj)fMaybeGenerate);
                    }
                }

                filesToGenerateOrdered.Add(filesToGenerateSub);
            }
            return filesToGenerateOrdered;
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
