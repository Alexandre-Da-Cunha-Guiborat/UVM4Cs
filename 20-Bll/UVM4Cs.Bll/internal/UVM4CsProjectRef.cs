using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using System;
using System.IO;
using UVM.Logging;

namespace UVM4Cs.Bll
{
    /// <summary>
    /// Class representing the project reference in a csproj file.
    /// </summary>
    internal class UVM4CsProjectRef
    {

        #region DEBUG

        /// <summary>
        /// String representation of the assembly name.
        /// </summary>
        private const string _asmName = "UVM4Cs.Bll";

        /// <summary>
        /// String representation of the class name.
        /// </summary>
        private const string _className = "UVM4CsPackageRef";

        #endregion DEBUG

        #region Public

        #region Constructor

        /// <summary>
        /// UVM4CsProjectRef constructor.
        /// </summary>
        /// <param name="projectReferenceElement"><see cref="XElement"/> of the Project Reference we want to create.</param>
        public UVM4CsProjectRef(XElement projectReferenceElement)
        {
            string title = UVMLogger.CreateTitle(_asmName, _className, $"UVM4CsProjectRef");

            if (projectReferenceElement is not null)
            {
                XAttribute? includeAttribute = projectReferenceElement.Attribute("Include");
                if (includeAttribute is not null)
                {
                    string include = includeAttribute.Value;
                    Include = Path.GetFileNameWithoutExtension(include);
                }
                else
                {
                    Include = string.Empty;

                    Exception exception = new Exception($"{title} :\nA ProjectReference do not contain a value for attribute <Include>.");
                    throw exception;
                }
            }
            else
            {
                Include = string.Empty;

                Exception exception = new Exception($"{title} :\nprojectReferenceElement given is null.");
                throw exception;
            }

        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// String representing the "Include" attribute.
        /// </summary>
        public string Include { get; }

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
