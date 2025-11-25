using System;
using System.Collections.Generic;
using System.Reflection;

namespace UVM4Cs.Service
{
    /// <summary>
    /// Class containing utils around UVM4Cs specific needs.
    /// </summary>
    public class UVM4CsHelper
    {
        #region Singleton
        // TBD
        #endregion Singleton

        #region Public

        /// <summary>
        /// <see cref="List{T}" of extensions to consider.
        /// </summary>
        public static readonly List<String> FExtensions = [".csproj", ".cs"];

        /// <summary>
        /// Check if UVM running condition are met.
        /// </summary>
        /// <param name="configuration">UVM running configuration.</param>
        /// <returns><see langword="true"/> => all condition are met, <see langword="false"/> => otherwise.</returns>
        public Boolean IsUVM4CsRunningConditionMet(UVM4CsConfiguration configuration)
        {
            Boolean isRebasedNeeded = false;
            for (Int32 i = 0; i < configuration.UVMConfig.GitDirectories.Count; i++)
            {
                if (_gitUtils.IsRebaseNeeded(configuration.UVMConfig.GitDirectories[i], configuration.UVMConfig.CommitIdsRef[i]))
                {
                    isRebasedNeeded = true;
                }
            }

            return !isRebasedNeeded;
        }

        #endregion Public

        #region Protected
        // TBD
        #endregion Protected

        #region Private

        /// <summary>
        /// Utilities for git manipulation.
        /// </summary>
        private UVM4CsGitutils _gitUtils { get; set; } = new UVM4CsGitutils();

        #endregion Private

        #region DEBUG

        /// <summary>
        /// <see cref="String"/> representation of the assembly name.
        /// </summary>
        // private static String _asmName = Assembly.GetExecutingAssembly().Location ?? String.Empty;

        /// <summary>
        /// <see cref="String"/> representation of the class name.
        /// </summary>
        // private static String _className = nameof(UVM4CsHelper);

        #endregion DEBUG
    }
}
