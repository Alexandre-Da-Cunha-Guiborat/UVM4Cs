using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using UVM.Logging;

namespace UVM4Cs.Bll
{
    /// <summary>
    /// Class representing the project reference in a csproj file.
    /// </summary>
    internal class UVM4CsProjectRef
    {
        #region Singleton
        // TBD
        #endregion Singleton

        #region Public

        /// <summary>
        /// <see cref="String"/> representing the "Include" attribute.
        /// </summary>
        public String Include { get; private set; }

        /// <summary>
        /// UVM4CsProjectRef constructor.
        /// </summary>
        /// <param name="projectReferenceElement"><see cref="XElement"/> of the Project Reference we want to create.</param>
        public UVM4CsProjectRef(XElement projectReferenceElement)
        {
            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(UVM4CsProjectRef)}");

            if (projectReferenceElement is not null)
            {
                XAttribute? includeAttribute = projectReferenceElement.Attribute("Include");
                if (includeAttribute is not null)
                {
                    String include = includeAttribute.Value;
                    Include = Path.GetFileNameWithoutExtension(include);
                }
                else
                {
                    Include = String.Empty;

                    Exception exception = new Exception($"{title} :\nA ProjectReference do not contain a value for attribute <Include>.");
                    throw exception;
                }
            }
            else
            {
                Include = String.Empty;

                Exception exception = new Exception($"{title} :\nprojectReferenceElement given is null.");
                throw exception;
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
