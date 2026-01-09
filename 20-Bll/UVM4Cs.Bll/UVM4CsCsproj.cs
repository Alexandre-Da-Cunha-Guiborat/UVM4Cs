using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using UVM.Interface;
using UVM.Interface.Enums;
using UVM.Interface.Interfaces;
using UVM.Logging;
using UVM.Logging.Enums;
using UVM4Cs.Common;

namespace UVM4Cs.Bll
{
    /// <summary>
    /// Class representing a Csproj for UVM interaction.
    /// </summary>
    public class UVM4CsCsproj : I_VersionableFile, I_GenerableFile
    {

        #region Singleton
        // TBD
        #endregion Singleton

        #region Public

        /// <summary>
        /// <see cref="String"/> representation of an unique Id used to identify the file.
        /// </summary>
        public String VFId { get; private set; }

        /// <summary>
        /// <see cref="String"/> representation of the absolute path to the file directory.
        /// </summary>
        public String VFDirPath { get; private set; }

        /// <summary>
        /// <see cref="String"/> representation of the absolute path to the file.
        /// </summary>
        public String VFPath { get; private set; }

        /// <summary>
        /// <see cref="String"/> representation of file's name.
        /// </summary>
        public String VFName { get; private set; }

        /// <summary>
        /// <see cref="String"/> representation of file's extension.
        /// </summary>
        public String VFExtension { get; private set; }

        /// <summary>
        /// <see cref="List{T}"/> of <see cref="I_Version"/> representing all versions described in the file.
        /// </summary>
        public List<I_Version> VFVersions { get; private set; }

        /// <summary>
        /// List of <see cref="I_VersionableFile"/> representing all file's dependencies to other <see cref="I_VersionableFile"/>.
        /// </summary>
        public List<I_VersionableFile> VFDependencies { get; private set; }

        /// <summary>
        /// Csproj constructor.
        /// </summary>
        /// <param name="csprojFilePath"><see cref="String"/> representation of the absolute path to the file.</param>
        /// <param name="slnPath"><see cref="String"/> representation of the absolute path to the sln file, that csproj is linked to.</param>
        public UVM4CsCsproj(String csprojFilePath, String slnPath)
        {
            FileInfo fInfo = new(csprojFilePath);
            if (fInfo.Exists && fInfo.Extension.Equals(".csproj"))
            {
                _xmlFile = XDocument.Load(csprojFilePath);
                if (_xmlFile is null)
                {
                    String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(UVM4CsCsproj)}");
                    throw new ArgumentNullException($"{title} :\n .csproj ({csprojFilePath}) is not a valid XML file.");
                }

                VFDirPath = fInfo.Directory?.FullName.Replace("\\", "/") ?? "/";
                VFPath = fInfo.FullName.Replace("\\", "/");
                VFName = Path.GetFileNameWithoutExtension(fInfo.Name);
                VFExtension = fInfo.Extension;

                VFId = _ReadId(csprojFilePath);
                VFVersions = _ReadVersion(csprojFilePath);

                _isPackable = _ReadIsPackable(csprojFilePath);
                _slnPath = slnPath;
                VFDependencies = [];
            }
            else
            {
                String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(UVM4CsCsproj)}");
                String message = $"The given path : {csprojFilePath}, is not leading to a .csproj file.";
                UVMLogger.AddLog(E_LogLevel.ERROR, title, message);

                Exception exception = new Exception($"{title} :\n{message}");
                throw exception;
            }
        }

        /// <summary>
        /// Compute the dependencies of the file.
        /// </summary>
        /// <param name="vfPool"><see cref="List{T}"/> of all <see cref="I_VersionableFile"/> that could be a dependence of this <see cref="UVM4CsCsproj"/>.</param>
        /// <returns>The <see cref="List{T}"/> of all <see cref="I_VersionableFile"/> in the vfPool that this <see cref="UVM4CsCsproj"/> depend on.</returns>
        public List<I_VersionableFile> ComputeDependencies(List<I_VersionableFile> vfPool)
        {
            VFDependencies = _ComputeDep(vfPool);
            return VFDependencies;
        }

        /// <summary>
        /// Dump the file to the file system.
        /// </summary>
        /// <param name="outputPath"><see cref="String"/> representation of the absolute path to use for dumping the file.</param>
        /// <returns><see langword="true"/> => dump succeed, <see langword="false"/> => otherwise.</returns>
        public Boolean DumpFile(String outputPath)
        {
            FileStream Fs = File.Create(outputPath);
            String xmlString = _xmlFile.ToString();
            Byte[] info = new UTF8Encoding(true).GetBytes(xmlString);
            Fs.Write(info, 0, info.Length);
            Fs.Close();

            return true;
        }

        /// <summary>
        /// Upgrade all version using the specified build, digit, semver.
        /// </summary>
        /// <param name="versionIndexes"><see cref="List{T}"/> of indexes of all versions that should be upgrade. (Positionally.)</param>
        /// <param name="buildTs"><see cref="List{T}"/> of <see cref="BuildType"/> to use when upgrading the version at the same index in versionIndexes. (Positionally.)</param>
        /// <param name="digitTs"><see cref="List{T}"/> of <see cref="DigitType"/> to upgrade the version at the same index in versionIndexes. (Positionally.)</param>
        /// <param name="semiVersions"><see cref="List{T}"/> of semi version to use for the version at the same index in versionIndexes. (Positionally.)</param>
        /// <returns><see langword="true"/> => update succeed, <see langword="false"/> => otherwise.</returns>
        public Boolean Update(List<UInt16> versionIndexes, List<BuildType> buildTs, List<DigitType> digitTs, List<UInt16> semiVersions)
        {
            if (versionIndexes.Count != buildTs.Count || versionIndexes.Count != digitTs.Count || versionIndexes.Count != semiVersions.Count)
            {
                return false;
            }

            for (Int32 i = 0; i < versionIndexes.Count; i++)
            {
                _UpdateVersion(versionIndexes[i], buildTs[i], digitTs[i], semiVersions[i]);
            }
            _UpdateDependencies();
            return true;
        }

        /// <summary>
        /// Generate the nuget linked to this file if any exists.
        /// </summary>
        /// <returns><see langword="true"/> => generation succeed, <see langword="false"/> => otherwise.</returns>
        public Boolean Generate()
        {
            if (!_isPackable)
            {
                return true;
            }

            Boolean success = true;

            FileInfo slnFInfo = new FileInfo(_slnPath);
            String configFilePath = $"{slnFInfo.DirectoryName}/nuget.config".Replace("\\", "/");

            Dictionary<String, String> restoreArgs = [];
            restoreArgs.Add("--configFile", configFilePath);
            if (_runRestoreCmd(restoreArgs) is false)
            {
                success = false;
            }

            Dictionary<String, String> cleanArgs = [];
            cleanArgs.Add("--configuration", "Release");
            if (_runCleanCmds(cleanArgs))
            {
                success = false;
            }

            Dictionary<String, String> buildArgs = [];
            buildArgs.Add("--configuration", "Release");
            if (_runBuildCmds(buildArgs))
            {
                success = false;
            }

            String outputPath = $"{UVM4CsConstant.DEFAULT_PACKAGE_OUTPUT_DIR_PATH}/{VFId}".Replace("\\", "/");
            Dictionary<String, String> packArgs = [];
            packArgs.Add("--configuration", "Release");
            packArgs.Add("--output", outputPath);
            if (_runPackCmd(packArgs))
            {
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Generate the nuget linked to this file if any exists.
        /// </summary>
        /// <param name="outputDirPath"><see cref="String"/> representation of the absolute path to the output directory.</param>
        /// <param name="args"><see cref="List{T}"/> of <see cref="String"/> for specific arguments for generation. (Must be handled internally.)</param>
        /// <returns><see langword="true"/> => generation succeed, <see langword="false"/> => otherwise.</returns>
        public Boolean Generate(String outputDirPath, List<String> args)
        {
            if (!_isPackable)
            {
                return true;
            }

            Boolean success = true;

            String configurationArg = args.Where(arg => arg.Contains(UVM4CsConstant.ConfigBuildFlag)).LastOrDefault() ?? String.Empty;
            String configuration = configurationArg.Split("=").LastOrDefault() ?? String.Empty;

            String ouputArg = args.Where(arg => arg.Contains(UVM4CsConstant.OutputPathPkgDirFlag)).LastOrDefault() ?? String.Empty;
            String outputPath = ouputArg.Split("=").LastOrDefault() ?? String.Empty;

            FileInfo slnFInfo = new FileInfo(_slnPath);
            String configFilePath = $"{slnFInfo.DirectoryName}/nuget.config".Replace("\\", "/");

            Dictionary<String, String> restoreArgs = [];
            restoreArgs.Add("--configFile", configFilePath);
            if (_runRestoreCmd(restoreArgs) is false)
            {
                success = false;
            }

            Dictionary<String, String> cleanArgs = [];
            cleanArgs.Add("--configuration", configuration);
            if (_runCleanCmds(cleanArgs))
            {
                success = false;
            }

            Dictionary<String, String> buildArgs = [];
            buildArgs.Add("--configuration", configuration);
            if (_runBuildCmds(buildArgs))
            {
                success = false;
            }

            Dictionary<String, String> packArgs = [];
            packArgs.Add("--configuration", configuration);
            packArgs.Add("--output", outputPath);
            if (_runPackCmd(packArgs))
            {
                success = false;
            }

            return success;
        }

        #endregion Public

        #region Protected
        // TBD
        #endregion Protected

        #region Private

        /// <summary>
        /// Index of the VersionPrefix in the <see cref="VFVersions"/>.
        /// </summary>
        private const Int32 _versionPrefixIndex = 0;

        /// <summary>
        /// Index of the VersionSuffix in the <see cref="VFVersions"/>.
        /// </summary>
        private const Int32 _versionSuffixIndex = 1;

        /// <summary>
        /// <see cref="Boolean"/> indicating if the given .csproj can be used to generate a NuGet.
        /// </summary>
        private Boolean _isPackable { get; set; }

        /// <summary>
        /// <see cref="String"/> representation of the absolute path to the solution file linked to this csproj.
        /// </summary>
        private String _slnPath { get; set; }

        /// <summary>
        /// XML representation of the file.
        /// </summary>
        private XDocument _xmlFile { get; set; }

        /// <summary>
        /// Read the id from the file.
        /// </summary>
        /// <param name="filepath"><see cref="String"/> representation of the absolute path to the file.</param>
        /// <returns>A <see cref="String"/> representation of the PackageId.</returns>
        private String _ReadId(String filepath)
        {
            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(_ReadId)}");

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

            IEnumerable<XElement>? propertyGroupElement = csprojXMLRoot.Elements("PropertyGroup");
            if (propertyGroupElement is null)
            {
                throw new NullReferenceException($"{title} :\n .csproj ({filepath}) file has no element <PropertyGroup>.");
            }

            XElement? packageId = propertyGroupElement.Elements("PackageId").LastOrDefault();
            if (packageId is not null)
            {
                return packageId.Value;
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Read the versions from the file.
        /// </summary>
        /// <param name="filepath"><see cref="String"/> representation of the absolute path to the file.</param>
        /// <returns>Returns [VersionPrefix]</returns>
        private List<I_Version> _ReadVersion(String filepath)
        {
            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(_ReadVersion)}");

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

            IEnumerable<XElement> propertyGroupElement = csprojXMLRoot.Elements("PropertyGroup");
            if (propertyGroupElement is null)
            {
                throw new NullReferenceException($"{title} :\n .csproj ({filepath}) file has no element <PropertyGroup>.");
            }

            XElement? versionPrefix = propertyGroupElement.Elements("VersionPrefix").LastOrDefault();
            XElement? versionSuffix = propertyGroupElement.Elements("VersionSuffix").LastOrDefault();
            if (versionPrefix is not null && versionSuffix is not null)
            {
                return [new UVM4CsVersionPrefix(versionPrefix.Value), new UVM4CsVersionSuffix(versionSuffix.Value)];
            }
            else
            {
                return [new UVM4CsVersionPrefix(UVMConstant.BAD_VERSION_STR), new UVM4CsVersionSuffix(UVMConstant.BAD_VERSION_STR)];
            }
        }

        /// <summary>
        /// Read the IsPackable from the file.
        /// </summary>
        /// <param name="filepath"><see cref="String"/> representation of the absolute path to the file.</param>
        /// <returns><see langword="true"/> => if the IsPackable tag is set to <see langword="true"/>, <see langword="false"/> => otherwise.</returns>
        private Boolean _ReadIsPackable(String filepath)
        {
            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(_ReadIsPackable)}");

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

            IEnumerable<XElement>? propertyGroupElement = csprojXMLRoot.Elements("PropertyGroup");
            if (propertyGroupElement is null)
            {
                throw new NullReferenceException($"{title} :\n .csproj ({filepath}) file has no element <PropertyGroup>.");
            }

            XElement? isPackable = propertyGroupElement.Elements("IsPackable").LastOrDefault();
            if (isPackable is not null)
            {
                return isPackable.Value.Equals("true");
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Compute the dependencies of the file.
        /// </summary>
        /// <param name="vfPool"><see cref="List{T}"/> of all <see cref="I_VersionableFile"/> that our file might depend on.</param>
        /// <returns>The <see cref="List{T}"/> of all <see cref="I_VersionableFile"/>, the current fle depend on and that are in the file pool.</returns>
        private List<I_VersionableFile> _ComputeDep(List<I_VersionableFile> vfPool)
        {
            // Compute dependencies of type project reference.
            List<UVM4CsProjectRef> projectRefs = _ReadProjectRef(VFPath);
            List<String> projectRefsIds = projectRefs.Select(p => p.Include).ToList();
            List<I_VersionableFile> projectDep = vfPool.Where(project => projectRefsIds.Contains(project.VFId)).ToList();

            // Compute dependencies of type package reference.
            List<UVM4CsPackageRef> pkgRefs = _ReadPackageRef(VFPath);
            List<String> pkgRefsIds = pkgRefs.Select(p => p.Include).ToList();
            List<I_VersionableFile> pkgDep = vfPool.Where(project => pkgRefsIds.Contains(project.VFId)).ToList();

            // Return it as ProjectReference first PackageReference second.
            return projectDep.Concat(pkgDep).ToList();
        }

        /// <summary>
        /// Read the Project Reference from the file.
        /// </summary>
        /// <param name="filepath"><see cref="String"/> representation of the absolute path to the file.</param>
        /// <returns>The <see cref="List{T}"/> of all project reference in the file as <see cref="UVM4CsProjectRef"/></returns>
        private List<UVM4CsProjectRef> _ReadProjectRef(String filepath)
        {
            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(_ReadProjectRef)}");

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

            List<UVM4CsProjectRef> ProjectRefs = itemGroupElements.SelectMany(itemgroup => itemgroup.Elements("ProjectReference"))
                                                                    .Select(ProjectRef => new UVM4CsProjectRef(ProjectRef))
                                                                    .ToList();
            return ProjectRefs;
        }

        /// <summary>
        /// Read the Package Reference from the file.
        /// </summary>
        /// <param name="filepath"><see cref="String"/> representation of the absolute path to the file.</param>
        /// <returns>The <see cref="List{T}"/> of all package reference in the file as <see cref="UVM4CsPackageRef"/></returns>
        private List<UVM4CsPackageRef> _ReadPackageRef(String filepath)
        {
            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(_ReadPackageRef)}");

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

            List<UVM4CsPackageRef> PackageRefs = itemGroupElements.SelectMany(itemgroup => itemgroup.Elements("PackageReference"))
                                                                   .Select(PkgRef => new UVM4CsPackageRef(PkgRef)).ToList();

            return PackageRefs;
        }

        /// <summary>
        /// Upgrade the version at index idx according to the digit, build and semi version given.
        /// </summary>
        /// <param name="versionIdx">Index of the version to update in the Versions field.</param>
        /// <param name="buildT">Build type to use for upgrade.</param>
        /// <param name="digitToUpgrade">Digit of the versions we want to upgrade.</param>
        /// <param name="semiVersion">Semi Version to apply if releasing an alpha or a beta.</param>
        private void _UpdateVersion(UInt16 versionIdx, BuildType buildT, DigitType digitToUpgrade, UInt16 semiVersion)
        {
            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(_UpdateVersion)}");

            if (VFVersions[versionIdx] is not null)
            {
                // Upgrade the Version at versionIdx.
                VFVersions[versionIdx].Upgrade(buildT, digitToUpgrade, semiVersion);

                if (_xmlFile is null)
                {
                    throw new NullReferenceException($"{title} :\n .csproj ({VFPath}) is not a valid XML file.");
                }

                XElement? csprojXMLRoot = _xmlFile.Root;
                if (csprojXMLRoot is null)
                {
                    throw new NullReferenceException($"{title} :\n .csproj ({VFPath}) has no valid root.");
                }

                IEnumerable<XElement>? propertyGroupElement = csprojXMLRoot.Elements("PropertyGroup");
                if (propertyGroupElement is null)
                {
                    throw new NullReferenceException($"{title} :\n .csproj ({VFPath}) has no <PropertyGroup> tag.");
                }

                if (versionIdx == _versionPrefixIndex)
                {
                    XElement? versionPrefix = propertyGroupElement.Elements("VersionPrefix").LastOrDefault();
                    if (versionPrefix is not null && VFVersions[versionIdx].ToString() != UVMConstant.BAD_VERSION_STR)
                    {
                        versionPrefix.Value = VFVersions[versionIdx].ToString();
                    }
                    else
                    {
                        Exception exception = new Exception($"{title} :\n Can not upgrade VersionPrefix of {VFPath}.");
                        throw exception;
                    }
                }
                else if (versionIdx == _versionSuffixIndex)
                {
                    XElement? versionSuffix = propertyGroupElement.Elements("VersionSuffix").LastOrDefault();
                    if (versionSuffix is not null && VFVersions[versionIdx].ToString() != UVMConstant.BAD_VERSION_STR)
                    {
                        versionSuffix.Value = VFVersions[versionIdx].ToString();
                    }
                    else
                    {
                        Exception exception = new Exception($"{title} :\n Can not upgrade VersionSuffix of {VFPath}.");
                        throw exception;
                    }
                }
                else
                {
                    Exception exception = new Exception($"{title} :\n Can not upgrade the version at index {versionIdx}. (Csproj has only [VersionPrefix, VersionSuffix].) (path = ({VFPath}))");
                    throw exception;
                }
            }
        }

        /// <summary>
        /// Update the dependencies in the XML representation of the file in ram.
        /// </summary>
        private void _UpdateDependencies()
        {
            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(_UpdateDependencies)}");

            if (_xmlFile is null)
            {
                throw new NullReferenceException($"{title} :\n .csproj ({VFPath}) is not a valid XML file.");
            }

            XElement? csprojXMLRoot = _xmlFile.Root;
            if (csprojXMLRoot is null)
            {
                throw new NullReferenceException($"{title} :\n .csproj ({VFPath}) has no valid root.");
            }

            IEnumerable<XElement>? itemGroupElements = csprojXMLRoot.Elements("ItemGroup");
            if (itemGroupElements is null || itemGroupElements.Count() == 0)
            {
                throw new NullReferenceException($"{title} :\n .csproj ({VFPath}) has no <ItemGroup> tag.");
            }

            // Handling PackageReference under the Release Configuration.
            IEnumerable<XElement>? itemGroupElementsRelease =
                itemGroupElements.Where(item => item.Attribute("Label")?.Value.Equals("Release_Dependencies") ?? false);

            IEnumerable<XElement>? packageReferencesRelease = itemGroupElementsRelease.Elements("PackageReference");
            foreach (XElement pkgRefRelease in packageReferencesRelease)
            {
                XAttribute? pkgRefInclude = pkgRefRelease.Attribute("Include");
                if (pkgRefInclude is null)
                {
                    throw new ArgumentNullException($"{title} :\n .csproj ({VFPath}) has a <PackageReference> tag with no Include attribute.");
                }

                I_VersionableFile? vfPkgRef = VFDependencies.FirstOrDefault(p => p.VFId.Equals(pkgRefInclude.Value));

                if (vfPkgRef is not null && vfPkgRef.VFVersions[_versionPrefixIndex].ToString() != UVMConstant.BAD_VERSION_STR)
                {
                    String pkgRefVersion = $"{vfPkgRef.VFVersions[_versionPrefixIndex].ToString()}";

                    if (vfPkgRef.VFVersions[_versionSuffixIndex].ToString() != UVMConstant.BAD_VERSION_STR)
                    {
                        pkgRefVersion += $"-{vfPkgRef.VFVersions[_versionSuffixIndex].ToString()}";
                    }

                    pkgRefRelease.SetAttributeValue("Version", pkgRefVersion);
                }
            }

            // Handling PackageReference under the Debug Configuration.
            IEnumerable<XElement>? itemGroupElementsDebug =
                itemGroupElements.Where(item => item.Attribute("Label")?.Value.Equals("Debug_Dependencies") ?? false);

            IEnumerable<XElement>? packageReferencesDebug = itemGroupElementsDebug.Elements("PackageReference");
            foreach (XElement pkgRefDebug in packageReferencesDebug)
            {
                XAttribute? pkgRefInclude = pkgRefDebug.Attribute("Include");
                if (pkgRefInclude is null)
                {
                    throw new ArgumentNullException($"{title} :\n .csproj ({VFPath}) has a <PackageReference> tag with no Include attribute.");
                }

                I_VersionableFile? vfPkgRef = VFDependencies.FirstOrDefault(p => p.VFId.Equals(pkgRefInclude.Value));

                if (vfPkgRef is not null && vfPkgRef.VFVersions[_versionPrefixIndex].ToString() != UVMConstant.BAD_VERSION_STR)
                {
                    String pkgRefVersion = $"{vfPkgRef.VFVersions[_versionPrefixIndex].ToString()}";

                    if (vfPkgRef.VFVersions[_versionSuffixIndex].ToString() != UVMConstant.BAD_VERSION_STR)
                    {
                        pkgRefVersion += $"-{vfPkgRef.VFVersions[_versionSuffixIndex].ToString()}";
                    }

                    pkgRefVersion += "-DEBUG";
                    pkgRefDebug.SetAttributeValue("Version", pkgRefVersion);
                }
            }
        }

        /// <summary>
        /// Launch a dotnet restore for this specific .csproj.
        /// </summary>
        /// <param name="nugetConfigPath"><see cref="String"/> representation of the command to execute.</param>
        private void _runCmd(String cmd)
        {
            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(UVM4CsCsproj)}");
            String message = $"running : {cmd}";
            UVMLogger.AddLog(E_LogLevel.INFO, title, message);

            String shellPath = String.Empty;
            String shellArgs = String.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                shellPath = "cmd.exe";
                shellArgs = $"/c {cmd}";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                shellPath = "/bin/bash";
                shellArgs = $"-c \"{cmd}\"";
            }

            Process procWithSourcing = new Process();
            ProcessStartInfo pStartInfo = new ProcessStartInfo
            {
                FileName = shellPath,
                Arguments = shellArgs,
                CreateNoWindow = false,
            };
            procWithSourcing.StartInfo = pStartInfo;
            procWithSourcing.Start();
            procWithSourcing.WaitForExit();
        }

        /// <summary>
        /// Launch a dotnet restore for this specific .csproj.
        /// </summary>
        /// <param name="args"><see cref="Dictionary{T,T}"/> containing arguments to give to the dotnet restore command.</param>
        /// <returns><see langword="true"/> => restore successful, <see langword="false"/> => otherwise.</returns>
        private Boolean _runRestoreCmd(Dictionary<String, String> args)
        {
            if (args.ContainsKey("--configfile"))
            {
                String configFilePath = args["--configfile"];
                _runCmd($"dotnet restore \"{VFPath}\" --configfile \"{configFilePath}\"");
            }
            else
            {
                _runCmd($"dotnet restore \"{VFPath}\"");
            }

            return true;
        }

        /// <summary>
        /// Launch a dotnet clean for this specific .csproj.
        /// </summary>
        /// <param name="args"><see cref="Dictionary{T,T}"/> containing arguments to give to the dotnet clean command.</param>
        /// <returns><see langword="true"/> => clean successful, <see langword="false"/> => otherwise.</returns>
        private Boolean _runCleanCmds(Dictionary<String, String> args)
        {
            if (args.ContainsKey("--configuration"))
            {
                String configuration = args["--configuration"];
                _runCmd($"dotnet clean \"{VFPath}\" --configuration {configuration}");
            }
            else
            {
                _runCmd($"dotnet clean \"{VFPath}\" --configuration Debug");
                _runCmd($"dotnet clean \"{VFPath}\" --configuration Release");
            }

            return true;
        }

        /// <summary>
        /// Launch a dotnet build for this specific .csproj. (Release and Debug)
        /// </summary>
        /// <param name="args"><see cref="Dictionary{T,T}"/> containing arguments to give to the dotnet clean command.</param>
        /// <returns><see langword="true"/> => build successful, <see langword="false"/> => otherwise.</returns>
        private Boolean _runBuildCmds(Dictionary<String, String> args)
        {
            if (args.ContainsKey("--configuration"))
            {
                String configuration = args["--configuration"];
                _runCmd($"dotnet build \"{VFPath}\" --configuration {configuration}");
            }
            else
            {
                _runCmd($"dotnet build \"{VFPath}\" --configuration Debug");
                _runCmd($"dotnet build \"{VFPath}\" --configuration Release");
            }

            return true;
        }

        /// <summary>
        /// Launch a dotnet restore for this specific .csproj.
        /// </summary>
        /// <param name="args"><see cref="Dictionary{T,T}"/> containing arguments to give to the dotnet clean command.</param>
        /// <returns><see langword="true"/> => pack successful, <see langword="false"/> => otherwise.</returns>
        private Boolean _runPackCmd(Dictionary<String, String> args)
        {
            if (args.ContainsKey("--configuration") && args.ContainsKey("--output"))
            {
                String configuration = args["--configuration"];
                String ouputPath = args["--output"];
                _runCmd($"dotnet pack \"{VFPath}\" --configuration {configuration} --output \"{ouputPath}\"");

                return true;
            }

            return false;
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
        private static String _className = nameof(UVM4CsCsproj);

        #endregion DEBUG
    }
}

