using System;
using System.Reflection;
using System.Xml.Linq;
using UVM.Interface;
using UVM.Logging;

namespace UVM4Cs.Bll
{
    /// <summary>
    /// Class representing the package reference in a csproj file.
    /// </summary>
    internal class UVM4CsPackageRef
    {
        #region Singleton
        // TBD
        #endregion Singleton

        #region Public

        /// <summary>
        /// <see cref="String"/> representation of the "Include" attribute.
        /// </summary>
        public String Include { get; private set; }

        /// <summary>
        /// <see cref="String"/> representation of the "Version" attribute.
        /// </summary
        public String Version { get; private set; }

        /// <summary>
        /// UVM4CsPackageRef constructor.
        /// </summary>
        /// <param name="packageReferenceElement"><see cref="XElement"/> of the Package Reference.</param>
        public UVM4CsPackageRef(XElement packageReferenceElement)
        {
            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(UVM4CsPackageRef)}");

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
                    Version = UVMConstant.BAD_VERSION_STR;
                }
            }

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
        private static String _asmName = Assembly.GetExecutingAssembly().Location ?? String.Empty;

        /// <summary>
        /// <see cref="String"/> representation of the class name.
        /// </summary>
        private static String _className = nameof(UVM4CsPackageRef);

        #endregion DEBUG
    }
}
