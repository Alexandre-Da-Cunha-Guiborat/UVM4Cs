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
    ///  Library of function for <see cref="UVM4CsCsproj"/> packaging.
    /// </summary>
    public class UVM4CsPackager
    {
        #region Singleton
        // TBD
        #endregion Singleton

        #region Public

        /// <summary>
        /// Generate the given file.
        /// </summary>
        /// <returns><see langword="true"/> => generation succeed, <see langword="false"/> => otherwise.</returns>
        public Boolean GenerateFile(UVM4CsCsproj fileToGenerate)
        {
            return fileToGenerate.Generate();
        }

        /// <summary>
        /// Generate the given file. (pass it the arguments.)
        /// </summary>
        /// <param name="fileToGenerate">File to generate.</param>
        /// <param name="outputPath"><see cref="String"/> representation of the absolute path to the location to generate the package.</param>
        /// <param name="args"><see cref="List{T}"/> of <see cref="String"/> used to specify arguments for generation.</param>
        /// <returns><see langword="true"/> => generation succeed, <see langword="false"/> => otherwise.</returns>
        public Boolean GenerateFile(UVM4CsCsproj fileToGenerate, String outputPath, List<String> args)
        {
            return UVMPackager.GenerateFile(fileToGenerate, outputPath, args);
        }

        /// <summary>
        /// Generate all files.
        /// </summary>
        /// <returns><see langword="true"/> => generation succeed, <see langword="false"/> => otherwise.</returns>
        public Boolean GenerateFiles(List<UVM4CsCsproj> filesToGenerateOrdered)
        {
            return UVMPackager.GenerateFiles(filesToGenerateOrdered.Cast<I_GenerableFile>().ToList());
        }

        /// <summary>
        /// Generate all files. (pass to each file the arguments (use positioning. file[i], path[i], args[i])).
        /// </summary>
        /// <param name="filesToGenerateOrdered"><see cref="List{T}"/> of all <see cref="UVM4CsCsproj"/> to generate. They must be ordered in the chronological way.</param>
        /// <param name="outputPath"><see cref="List{T}"/> of <see cref="String"/> representation of the absolute path to the location to generate the package.</param>
        /// <param name="args"><see cref="List{T}"/> of <see cref="List{T}"/> of <see cref="String"/> used to specify arguments for generation.</param>
        /// <returns><see langword="true"/> => generation succeed, <see langword="false"/> => otherwise.</returns>
        public Boolean GenerateFiles(List<UVM4CsCsproj> filesToGenerateOrdered, List<String> outputPaths, List<List<String>> args)
        {
            return UVMPackager.GenerateFiles(filesToGenerateOrdered.Cast<I_GenerableFile>().ToList(), outputPaths, args);
        }

        /// <summary>
        /// Generate all files.
        /// </summary>
        /// <returns><see langword="true"/> => generation succeed, <see langword="false"/> => otherwise.</returns>
        public Boolean GenerateFiles(List<List<UVM4CsCsproj>> filesToGenerateOrdered)
        {
            Boolean success = true;
            foreach (List<UVM4CsCsproj> subFilesToGenerateOrderedPar in filesToGenerateOrdered)
            {
                if (GenerateFiles(subFilesToGenerateOrderedPar) is false)
                {
                    success = false;
                }
            }

            return success;
        }

        /// <summary>
        /// Generate all files. (pass to each file the arguments (use positioning. file[i], path[i], args[i])).
        /// </summary>
        /// <param name="filesToGenerateOrdered"><see cref="List{T}"/> of <see cref="List{T}"/> of all <see cref="UVM4CsCsproj"/> to generate. They must be ordered in the chronological way.</param>
        /// <param name="outputPath"><see cref="List{T}"/> of <see cref="List{T}"/> of <see cref="String"/> representation of the absolute path to the location to generate the package.</param>
        /// <param name="args"><see cref="List{T}"/> of <see cref="List{T}"/> of <see cref="List{T}"/> of <see cref="String"/> used to specify arguments for generation.</param>
        /// <returns><see langword="true"/> => generation succeed, <see langword="false"/> => otherwise.</returns>
        public Boolean GenerateFiles(List<List<UVM4CsCsproj>> filesToGenerateOrdered, List<List<String>> outputPaths, List<List<List<String>>> args)
        {
            List<List<I_GenerableFile>> filesToGenerateOrderedCasted = [];
            foreach (List<UVM4CsCsproj> filesToGenerateOrderedSub in filesToGenerateOrdered)
            {
                filesToGenerateOrderedCasted.Add(filesToGenerateOrderedSub.Cast<I_GenerableFile>().ToList());
            }

            return UVMPackager.GenerateFiles(filesToGenerateOrderedCasted, outputPaths, args);
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
        // private static String _className = nameof(UVM4CsPackager);

        #endregion DEBUG
    }
}
