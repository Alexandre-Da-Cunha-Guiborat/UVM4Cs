using System.Xml.Linq;
using System;
using UVM.Interface;
using UVM.Logging;

namespace UVM4Cs.Bll
{
    /// <summary>
    /// Class representing the package reference in a csproj file.
    /// </summary>
    internal class UVM4CsPackageRef
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
        /// UVM4CsPackageRef constructor.
        /// </summary>
        /// <param name="packageReferenceElement"><see cref="XElement"/> of the Package Reference.</param>
        public UVM4CsPackageRef(XElement packageReferenceElement)
        {
            string title = UVMLogger.CreateTitle(_asmName, _className, $"UVM4CsPackageRef");

            if (packageReferenceElement is null)
            {
                Exception exception = new Exception($"{title} :\nThe packageReferenceElement is null.");
                throw exception;
            }
            else
            {
                XAttribute? packageReferenceElementAttributeInclude = packageReferenceElement.Attribute("Include");

                if (packageReferenceElementAttributeInclude is not null)
                {
                    Include = packageReferenceElementAttributeInclude.Value;
                }
                else
                {
                    Exception exception = new Exception($"{title} :\nA PackageReference do not contain a value for attribute <Include>");
                    throw exception;
                }

                XAttribute? attributeVersion = packageReferenceElement.Attribute("Version");
                XElement? elementVersion = packageReferenceElement.Element("Version");
                if (attributeVersion is null && elementVersion is null)
                {
                    Exception exception = new Exception($"{title} :\nA PackageReference do not contain a value for element/attribute <Version>");
                    throw exception;
                }
                else if (attributeVersion is not null)
                {
                    Version = attributeVersion.Value;
                }
                else if (elementVersion is not null)
                {
                    Version = elementVersion.Value;
                }
                else
                {
                    Version = UVMConstante.BAD_VERSION_STR;
                }
            }

        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// String representation of the "Include" attribute.
        /// </summary>
        public string Include { get; }

        /// <summary>
        /// String representation of the "Version" attribute.
        /// </summary
        public string Version { get; }

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
