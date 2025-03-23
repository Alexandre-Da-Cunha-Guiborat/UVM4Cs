using System.Collections.Generic;

namespace UVM4Cs.Service
{
    /// <summary>
    /// Class containing utils around UVM4Cs specific needs.
    /// </summary>
    public class UVM4CsHelper
    {
        #region DEBUG

        /// <summary>
        /// String representation of the assembly name.
        /// </summary>
        // private const string _asmName = "UVM4Cs.Service";

        /// <summary>
        /// String representation of the class name.
        /// </summary>
        // private const string _className = "UVM4CsHelper";

        #endregion DEBUG

        #region Public

        #region Constructor
        // TBD
        #endregion Constructor

        #region Properties

        /// <summary>
        /// List of extensions to consider.
        /// </summary>
        public static readonly List<string> FExtensions = [".csproj", ".cs"];

        #endregion Properties

        #region Method
        // TBD
        #endregion Method

        #region Function

        /// <summary>
        /// Check if UVM running condition are met.
        /// </summary>
        /// <param name="configuration">UVM running configuration.</param>
        /// <returns>True if all condition are met, False otherwise.</returns>
        public static bool IsUVM4CsRunningConditionMet(UVM4CsConfiguration configuration)
        {
            bool isRebasedNeeded = false;
            for (int i = 0; i < configuration.UVMConfig.GitDirectories.Count; i++)
            {
                if (UVM4CsGitutils.IsRebaseNeeded(configuration.UVMConfig.GitDirectories[i], configuration.UVMConfig.CommitIdsRef[i]))
                {
                    isRebasedNeeded = true;
                }
            }

            return !isRebasedNeeded;
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
