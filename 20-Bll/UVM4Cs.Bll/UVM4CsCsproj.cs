using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using UVM.Interface;
using UVM.Logging;
using UVM4Cs.Common;

namespace UVM4Cs.Bll
{
    /// <summary>
    /// Class representing a Csproj for UVM interaction.
    /// </summary>
    public class UVM4CsCsproj : I_GenerableFile
    {
        #region DEBUG

        /// <summary>
        /// String representation of the assembly name.
        /// </summary>
        private const string _asmName = "UVM4Cs.Bll";

        /// <summary>
        /// String representation of the class name.
        /// </summary>
        private const string _className = "UVM4CsCsproj";

        #endregion DEBUG

        #region Public

        #region Constructor

        /// <summary>
        /// Csproj constructor.
        /// </summary>
        /// <param name="csprojFilePath">String representation of the absolute path to the file.</param>
        /// <param name="slnPath">String representation of the absolute path to the sln file, that csproj is linked to.</param>
        public UVM4CsCsproj(string csprojFilePath, string slnPath)
        {
            FileInfo fInfo = new(csprojFilePath);
            if (fInfo.Exists && fInfo.Extension.Equals(".csproj"))
            {
                _xmlFile = XDocument.Load(csprojFilePath);
                if (_xmlFile is null)
                {
                    string title = UVMLogger.CreateTitle(_asmName, _className, "_ReadVersion");
                    throw new ArgumentNullException($"{title} :\n .csproj ({csprojFilePath}) is not a valid XML file.");
                }

                if (fInfo.Directory is not null)
                {
                    VFDirPath = fInfo.Directory.FullName.Replace("\\", "/");
                }
                else
                {
                    VFDirPath = "/";
                }
                VFPath = fInfo.FullName.Replace("\\", "/");
                VFName = Path.GetFileNameWithoutExtension(fInfo.Name);
                VFExtension = fInfo.Extension;

                VFId = _ReadId(csprojFilePath);
                VFVersions = _ReadVersion(csprojFilePath);

                IsNuGet = _ReadIsNuGet(csprojFilePath);
                SlnPath = slnPath;
                VFDependencies = [];
            }
            else
            {
                string title = UVMLogger.CreateTitle(_asmName, _className, $"UVM4CsCsproj");
                string message = $"The given path : {csprojFilePath}, is not leading to a .csproj file.";
                UVMLogger.AddLog(LogLevel.Error, title, message);

                Exception exception = new Exception($"{title} :\n{message}");
                throw exception;
            }
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// String reprensentation of an unique Id used to identify the file.
        /// </summary>
        public string VFId { get; private set; }

        /// <summary>
        /// String representation of the absolute path to the file directory.
        /// </summary>
        public string VFDirPath { get; }

        /// <summary>
        /// String representation of the absolute path to the file.
        /// </summary>
        public string VFPath { get; private set; }

        /// <summary>
        /// String representation of file's name.
        /// </summary>
        public string VFName { get; private set; }

        /// <summary>
        /// String representation of file's extension.
        /// </summary>
        public string VFExtension { get; private set; }

        /// <summary>
        /// List of <see cref="I_Version"/> reprensenting all versions described in the file.
        /// </summary>
        public List<I_Version> VFVersions { get; private set; }

        /// <summary>
        /// List of <see cref="I_VersionnableFile"/> reprensenting all file's dependencies to other <see cref="I_VersionnableFile"/>.
        /// </summary>
        public List<I_VersionnableFile> VFDependencies { get; private set; }

        /// <summary>
        /// Boolean indicating if the given .csproj can be used to generate a NuGet.
        /// </summary>
        public bool IsNuGet { get; private set; }

        /// <summary>
        /// String representation of the absolute path to the solution file linked to this csproj.
        /// </summary>
        public string SlnPath { get; private set; }

        #endregion Properties

        #region Method

        /// <summary>
        /// Compute the dependencies of the file.
        /// </summary>
        /// <param name="vfPool">List of all VF that could be a dependence of this VF.</param>
        /// <returns>The list of all VF in the versionnableFilePool that this VF depend on.</returns>
        public List<I_VersionnableFile> ComputeDependencies(List<I_VersionnableFile> vfPool)
        {
            VFDependencies.Clear();
            VFDependencies = _ComputeDep(vfPool);
            return VFDependencies;
        }

        /// <summary>
        /// Dump the file to the file system.
        /// </summary>
        /// <param name="outputPath">String representation of the absolute path to use for dumping the file.</param>
        public bool DumpFile(string outputPath)
        {
            FileStream Fs = File.Create(outputPath);
            string xmlString = _xmlFile.ToString();
            byte[] info = new UTF8Encoding(true).GetBytes(xmlString);
            Fs.Write(info, 0, info.Length);
            Fs.Close();

            return true;
        }

        /// <summary>
        /// Upgrade all version using the specified build, digit, semver.
        /// </summary>
        /// <param name="vIdxs">List of all versions that should be upgrade.</param>
        /// <param name="buildTs">List of <see cref="BuildType"/> to use when upgrading the version at the same index in vIdxs.</param>
        /// <param name="digitTs">List of <see cref="DigitType"/> to upgrade the version at the same index in vIdxs.</param>
        /// <param name="semvers">List of semver to use for the version at the same index in vIdxs.</param>
        public bool Update(List<UInt16> vIdxs, List<BuildType> buildTs, List<DigitType> digitTs, List<UInt16> semvers)
        {
            if (vIdxs.Count != buildTs.Count || vIdxs.Count != digitTs.Count || vIdxs.Count != semvers.Count)
            {
                return false;
            }

            for (int i = 0; i < vIdxs.Count; i++)
            {
                _UpdateVersion(vIdxs[i], buildTs[i], digitTs[i], semvers[i]);
            }
            _UpdateDependencies();
            return true;
        }

        /// <summary>
        /// Generate the nuget linked to this file if any exists.
        /// </summary>
        public bool Generate()
        {
            if (!IsNuGet)
            {
                return true;
            }

            FileInfo slnFInfo = new FileInfo(SlnPath);
            string nugetConfigPath = $"{slnFInfo.DirectoryName}/nuget.config".Replace("\\", "/");

            string outputPath = $"{UVM4CsConstante.DEFAULT_PACKAGE_OUTPUT_DIR_PATH}/{VFId}".Replace("\\", "/");

            _runRestoreCmd(nugetConfigPath);
            _runCleanCmds();
            _runBuildCmds();
            _runPackCmd(outputPath);

            // Should add some control over commands output.
            return true;
        }

        /// <summary>
        /// Generate the nuget linked to this file if any exists.
        /// </summary>
        /// <param name="outputDirPath">String representation of the absolute path to the output directory.</param>
        /// <param name="args">List of string for specific arguments for generation. (Must be handled internaly.)</param>
        public bool Generate(string outputDirPath, List<string> args)
        {
            string title = UVMLogger.CreateTitle(_asmName, _className, $"Generate");
            string message = $"NOT IMPLEMENTED YET, use Generate() instead.";
            UVMLogger.AddLog(LogLevel.Error, title, message);

            throw new NotImplementedException(title);
        }

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

        /// <summary>
        /// Read the id from the file.
        /// </summary>
        /// <param name="filepath">String representation of the absolute path to the file.</param>
        /// <returns>A string representation of the PackageId.</returns>
        private string _ReadId(string filepath)
        {
            string title = UVMLogger.CreateTitle(_asmName, _className, "_ReadVersion");

            XDocument? csprojXML = XDocument.Load(filepath);
            if (csprojXML is null)
            {
                throw new ArgumentNullException($"{title} :\n .csproj ({filepath}) is not a valid XML file.");
            }

            XElement? csprojXMLRoot = csprojXML.Root;
            if (csprojXMLRoot is null)
            {
                throw new NullReferenceException($"{title} :\n csproj ({filepath}) has no valid root.");
            }

            XElement? propertyGroupElement = csprojXMLRoot.Element("PropertyGroup");
            if (propertyGroupElement is null)
            {
                throw new NullReferenceException($"{title} :\n .csproj ({filepath}) file has no element <PropertyGroup>.");
            }

            XElement? packageId = propertyGroupElement.Element("PackageId");
            if (packageId is not null)
            {
                return packageId.Value;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Read the versions from the file.
        /// </summary>
        /// <param name="filepath">String representation of the absolute path to the file.</param>
        /// <returns>Returns [VersionPrefix]</returns>
        private List<I_Version> _ReadVersion(string filepath)
        {
            string title = UVMLogger.CreateTitle(_asmName, _className, "_ReadVersion");

            XDocument? csprojXML = XDocument.Load(filepath);
            if (csprojXML is null)
            {
                throw new ArgumentNullException($"{title} :\n .csproj ({filepath}) is not a valid XML file.");
            }

            XElement? csprojXMLRoot = csprojXML.Root;
            if (csprojXMLRoot is null)
            {
                throw new NullReferenceException($"{title} :\n csproj ({filepath}) has no valid root.");
            }

            XElement? propertyGroupElement = csprojXMLRoot.Element("PropertyGroup");
            if (propertyGroupElement is null)
            {
                throw new NullReferenceException($"{title} :\n .csproj ({filepath}) file has no element <PropertyGroup>.");
            }

            XElement? versionPrefix = propertyGroupElement.Element("VersionPrefix");
            if (versionPrefix is not null)
            {
                return [new UVM4CsVersion(versionPrefix.Value)];
            }
            else
            {
                return [new UVM4CsVersion(UVMConstante.BAD_VERSION_STR)];
            }
        }

        /// <summary>
        /// Read the IsNuGet from the file.
        /// </summary>
        /// <param name="filepath">String representation of the absolute path to the file.</param>
        /// <returns>Returns [VersionPrefix]</returns>
        private bool _ReadIsNuGet(string filepath)
        {
            string title = UVMLogger.CreateTitle(_asmName, _className, "_ReadIsNuGet");

            XDocument? csprojXML = XDocument.Load(filepath);
            if (csprojXML is null)
            {
                throw new ArgumentNullException($"{title} :\n .csproj ({filepath}) is not a valid XML file.");
            }

            XElement? csprojXMLRoot = csprojXML.Root;
            if (csprojXMLRoot is null)
            {
                throw new NullReferenceException($"{title} :\n csproj ({filepath}) has no valid root.");
            }

            XElement? propertyGroupElement = csprojXMLRoot.Element("PropertyGroup");
            if (propertyGroupElement is null)
            {
                throw new NullReferenceException($"{title} :\n .csproj ({filepath}) file has no element <PropertyGroup>.");
            }

            XElement? isNuGet = propertyGroupElement.Element("IsNuGet");
            if (isNuGet is not null)
            {
                if (isNuGet.Value.Equals("true"))
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Compute the dependencies of the file.
        /// </summary>
        /// <param name="versionnableFilePool">List of all file that our file might depend on.</param>
        /// <returns>A list of all files, the current fle depend on and that are in the filepool.</returns>
        private List<I_VersionnableFile> _ComputeDep(List<I_VersionnableFile> versionnableFilePool)
        {
            // Compute dependencies of type project reference.
            List<UVM4CsProjectRef> projectRefs = _ReadProjectRef(VFPath);
            List<string> projectRefsIds = projectRefs.Select(p => p.Include).ToList();
            List<I_VersionnableFile> projectDep = versionnableFilePool.Where(project => projectRefsIds.Contains(project.VFId)).ToList();

            // Compute dependencies of type package reference.
            List<UVM4CsPackageRef> pkgRefs = _ReadPackageRef(VFPath);
            List<string> pkgRefsIds = pkgRefs.Select(p => p.Include).ToList();
            List<I_VersionnableFile> pkgDep = versionnableFilePool.Where(project => pkgRefsIds.Contains(project.VFId)).ToList();

            // Return it as ProjectReference first PackageReference second.
            return projectDep.Concat(pkgDep).ToList();
        }

        /// <summary>
        /// Read the Project Reference from the file.
        /// </summary>
        /// <param name="filepath">String representation of the absolute path to the file.</param>
        /// <returns>A list of all project reference in the file as <see cref="UVM4CsProjectRef"/></returns>
        private List<UVM4CsProjectRef> _ReadProjectRef(string filepath)
        {
            string title = UVMLogger.CreateTitle(_asmName, _className, "_ReadProjectRef");

            XDocument csprojXML = XDocument.Load(filepath);
            if (csprojXML is null)
            {
                throw new ArgumentNullException($"{title} :\n .csproj ({filepath}) is not a valid XML file.");
            }

            XElement? csprojXMLRoot = csprojXML.Root;
            if (csprojXMLRoot is null)
            {
                throw new NullReferenceException($"{title} :\n csproj ({filepath}) has no valid root.");
            }

            IEnumerable<XElement> itemGroupElements = csprojXMLRoot.Elements("ItemGroup");
            if (itemGroupElements is null)
            {
                throw new NullReferenceException($"{title} :\n .csproj ({filepath}) file has no element <ItemGroup>.");
            }
            List<UVM4CsProjectRef> ProjectRefs = new List<UVM4CsProjectRef>(itemGroupElements.SelectMany(itemgroup => itemgroup.Elements("ProjectReference"))
                                                                   .Select(ProjectRef => new UVM4CsProjectRef(ProjectRef)));
            return ProjectRefs;
        }

        /// <summary>
        /// Read the Package Reference from the file.
        /// </summary>
        /// <param name="filepath">String representation of the absolute path to the file.</param>
        /// <returns>A list of all package reference in the file as <see cref="UVM4CsPackageRef"/></returns>
        private List<UVM4CsPackageRef> _ReadPackageRef(string filepath)
        {
            string title = UVMLogger.CreateTitle(_asmName, _className, "_ReadProjectRef");

            XDocument csprojXML = XDocument.Load(filepath);
            if (csprojXML is null)
            {
                throw new ArgumentNullException($"{title} :\n .csproj ({filepath}) is not a valid XML file.");
            }

            XElement? csprojXMLRoot = csprojXML.Root;
            if (csprojXMLRoot is null)
            {
                throw new NullReferenceException($"{title} :\n csproj ({filepath}) has no valid root.");
            }

            IEnumerable<XElement> itemGroupElements = csprojXMLRoot.Elements("ItemGroup");
            if (itemGroupElements is null)
            {
                throw new NullReferenceException($"{title} :\n .csproj ({filepath}) file has no element <ItemGroup>.");
            }

            List<UVM4CsPackageRef> PackageRefs = new(itemGroupElements.SelectMany(itemgroup => itemgroup.Elements("PackageReference"))
                                                                   .Select(PkgRef => new UVM4CsPackageRef(PkgRef)));

            return PackageRefs;
        }

        /// <summary>
        /// Upgrade the version at index idx according to the digit, build and semi version given.
        /// </summary>
        /// <param name="versionIdx">Index of the version to update in the Versions field.</param>
        /// <param name="buildT">Build type to use for upgrade.</param>
        /// <param name="digitToUpgrade">Digit of the versions we want to upgrade.</param>
        /// <param name="semiver">Semi Version to apply if releasing an alpha or a beta.</param>
        /// <exception cref="Exception"></exception>
        private void _UpdateVersion(int versionIdx, BuildType buildT, DigitType digitToUpgrade, UInt16 semiver)
        {
            string title = UVMLogger.CreateTitle(_asmName, _className, "_UpdateVersion");

            if (VFVersions[versionIdx] is not null)
            {
                // Upgrade the Version at versionIdx.
                VFVersions[versionIdx].Upgrade(buildT, digitToUpgrade, semiver);

                if (_xmlFile is null)
                {
                    throw new NullReferenceException($"{title} :\n .csproj ({VFPath}) is not a valid XML file.");
                }

                XElement? csprojXMLRoot = _xmlFile.Root;
                if (csprojXMLRoot is null)
                {
                    throw new NullReferenceException($"{title} :\n .csproj ({VFPath}) has no valid root.");
                }

                XElement? propertyGroupElement = csprojXMLRoot.Element("PropertyGroup");
                if (propertyGroupElement is null)
                {
                    throw new NullReferenceException($"{title} :\n .csproj ({VFPath}) has no <PropertyGroup> tag.");
                }

                // Update the VersionPrefix in the XML.
                if (versionIdx == 0)
                {
                    XElement? versionPrefix = propertyGroupElement.Element("VersionPrefix");
                    if (versionPrefix is not null && VFVersions[versionIdx].ToString() != UVMConstante.BAD_VERSION_STR)
                    {
                        versionPrefix.Value = VFVersions[versionIdx].ToString();
                    }
                    else
                    {
                        Exception exception = new Exception($"{title} :\n Can not upgrade assembly version of {VFPath}.");
                        throw exception;
                    }
                }
                else
                {
                    Exception exception = new Exception($"{title} :\n Can not upgrade the version at idx {versionIdx}. (Csproj has only [VersionPrefix].) (path = ({VFPath}))");
                    throw exception;
                }
            }
        }

        /// <summary>
        /// Update the dependencies in the XML representation of the file in ram.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        private void _UpdateDependencies()
        {
            string title = UVMLogger.CreateTitle(_asmName, _className, "_UpdateDependencies");
            if (_xmlFile is null)
            {
                throw new NullReferenceException($"{title} :\n .csproj ({VFPath}) is not a valid XML file.");
            }

            XElement? csprojXMLRoot = _xmlFile.Root;
            if (csprojXMLRoot is null)
            {
                throw new NullReferenceException($"{title} :\n .csproj ({VFPath}) has no valid root.");
            }

            XElement? propertyGroupElement = csprojXMLRoot.Element("PropertyGroup");
            if (propertyGroupElement is null)
            {
                throw new NullReferenceException($"{title} :\n .csproj ({VFPath}) has no <PropertyGroup> tag.");
            }

            // Update XElement versions based on matching Include attributes
            List<XElement> itemGroupElements = csprojXMLRoot.Elements("ItemGroup").ToList();
            if (itemGroupElements is null || itemGroupElements.Count == 0)
            {
                throw new NullReferenceException($"{title} :\n .csproj ({VFPath}) has no <ItemGroup> tag.");
            }

            List<XElement> PackageReferences = itemGroupElements.Elements("PackageReference").ToList();
            PackageReferences.ForEach(dependency =>
            {
                XAttribute? depAttributeInclude = dependency.Attribute("Include");
                if (depAttributeInclude is null)
                {
                    throw new ArgumentNullException($"{title} :\n .csproj ({VFPath}) has a <PackageReference> tag with no Include attribute.");
                }

                string include = depAttributeInclude.Value;
                I_VersionnableFile? pkgRef = VFDependencies.FirstOrDefault(p => p.VFId.Equals(include));

                // the 0 indice is hardcoded there. It correspond to the VersionPrefix of the csproj this file depends on.
                if (pkgRef is not null && pkgRef.VFVersions[0].ToString() != UVMConstante.BAD_VERSION_STR)
                {
                    dependency.SetAttributeValue("Version", pkgRef.VFVersions[0].ToString());
                }
            });
        }


        /// <summary>
        /// Launch adotnet restore for this specific .csproj.
        /// </summary>
        /// <param name="nugetConfigPath">String representation of the absolute path to the nuget.config to use for the restore.</param>
        private void _runRestoreCmd(string nugetConfigPath)
        {
            // Restore using NuGets in the outputDir.
            string cmd = $"dotnet restore \"{VFPath}\" --configfile \"{nugetConfigPath}\"";
            Process procWithSourcing = new Process();
            ProcessStartInfo pStartInfo = new ProcessStartInfo
            {
                FileName = $"cmd.exe",
                Arguments = $"/c {cmd}",
                CreateNoWindow = false,
            };
            procWithSourcing.StartInfo = pStartInfo;
            procWithSourcing.Start();
            procWithSourcing.WaitForExit();
        }

        /// <summary>
        /// Launch adotnet clean for this specific .csproj. (Release and Debug)
        /// </summary>
        private void _runCleanCmds()
        {
            // Restore using NuGets in the outputDir.
            string cmdDebug = $"dotnet clean \"{VFPath}\" --configuration Debug";
            Process pDebug = new Process();
            ProcessStartInfo pStartInfoDebug = new ProcessStartInfo
            {
                FileName = $"cmd.exe",
                Arguments = $"/c {cmdDebug}",
                CreateNoWindow = true,
            };
            pDebug.StartInfo = pStartInfoDebug;
            pDebug.Start();
            pDebug.WaitForExit();

            // Restore using NuGets in the outputDir.
            string cmdRelease = $"dotnet clean \"{VFPath}\" --configuration Release";
            Process pRelease = new Process();
            ProcessStartInfo pStartInfoRelease = new ProcessStartInfo
            {
                FileName = $"cmd.exe",
                Arguments = $"/c {cmdRelease}",
                CreateNoWindow = true,
            };
            pRelease.StartInfo = pStartInfoRelease;
            pRelease.Start();
            pRelease.WaitForExit();
        }

        /// <summary>
        /// Launchdotnet build for this specific .csproj. (Release and Debug)
        /// </summary>
        private void _runBuildCmds()
        {
            string buildCmdDebug = $"dotnet build \"{VFPath}\" --configuration Debug";
            Process procCmdDebug = new Process();
            ProcessStartInfo pStartInfoDebug = new ProcessStartInfo
            {
                FileName = $"cmd.exe",
                Arguments = $"/c {buildCmdDebug}",
                CreateNoWindow = true,
            };
            procCmdDebug.StartInfo = pStartInfoDebug;
            procCmdDebug.Start();
            procCmdDebug.WaitForExit();

            string buildCmdRelease = $"dotnet build \"{VFPath}\" --configuration Release";
            Process procCmdRelease = new Process();
            ProcessStartInfo pStartInfoRelease = new ProcessStartInfo
            {
                FileName = $"cmd.exe",
                Arguments = $"/c {buildCmdRelease}",
                CreateNoWindow = true,
            };
            procCmdRelease.StartInfo = pStartInfoRelease;
            procCmdRelease.Start();
            procCmdRelease.WaitForExit();
        }

        /// <summary>
        /// Launch adotnet restore for this specific .csproj.
        /// </summary>
        /// <param name="outputDir">String representation of the absolute path to the directory to write the generated file.</param>
        private void _runPackCmd(string outputDir)
        {
            string packCmd = $"dotnet pack \"{VFPath}\" --configuration Release -o \"{outputDir}\"";
            Process proc = new Process();
            ProcessStartInfo pStartInfo = new ProcessStartInfo
            {
                FileName = $"cmd.exe",
                Arguments = $"/c {packCmd}",
                Verb = "runas",
                CreateNoWindow = true,
            };
            proc.StartInfo = pStartInfo;
            proc.Start();
            proc.WaitForExit();
        }

        #endregion Method

        #region Function
        // TBD
        #endregion Function

        #region Field

        /// <summary>
        /// XML representation of the file.
        /// </summary>
        private XDocument _xmlFile;

        #endregion Field

        #endregion Private
    }
}

