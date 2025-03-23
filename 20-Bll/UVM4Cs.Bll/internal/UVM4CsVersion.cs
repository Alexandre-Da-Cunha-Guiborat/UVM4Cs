using System;
using UVM.Interface;
using UVM.Logging;

namespace UVM4Cs.Bll
{
    /// <summary>
    /// Class reprensenting the Package Version.
    /// </summary>
    internal class UVM4CsVersion : I_Version
    {

        #region DEBUG

        /// <summary>
        /// String representation of the assembly name.
        /// </summary>
        private const string _asmName = "UVM4Cs.Bll";

        /// <summary>
        /// String representation of the class name.
        /// </summary>
        private const string _className = "UVM4CsVersion";

        #endregion DEBUG

        #region Public

        #region Constructor

        /// <summary>
        /// UVM4CsVersion constructor.
        /// </summary>
        /// <param name="version">String representation of the version.</param>
        public UVM4CsVersion(string version)
        {
            string title = UVMLogger.CreateTitle(_asmName, _className, $"UVM4CsVersion");
            Major = 0;
            Minor = 0;
            Patch = 0;
            SemVer = 0;
            BuildT = BuildType.NONE;

            // Test for the version format.
            string[] digits = version.Split('.');
            if (digits.Length < 3)
            {
                Exception exception = new Exception($"{title} :\nPackageVersion must contains at least 3 digit.");
                throw exception;
            }

            UInt16 major = 0;
            UInt16 minor = 0;
            UInt16 patch = 0;
            UInt16 semver = 0;

            // Try to parse the version digits.
            if (!UInt16.TryParse(digits[0], out major))
            {
                Exception exception = new Exception($"{title} :\nMajor is not a conform digit.");
                throw exception;
            }

            if (!UInt16.TryParse(digits[1], out minor))
            {
                Exception exception = new Exception($"{title} :\nMinor is not a conform digit.");
                throw exception;
            }

            // Check for alpha/beta in order to handle special parsing.
            BuildT = BuildType.RELEASE;
            if (digits[2].Contains("-alpha"))
            {
                if (!UInt16.TryParse(digits[2].Replace("-alpha", ""), out patch))
                {
                    Exception exception = new Exception($"{title} :\nPatch is not a conform digit.");
                    throw exception;
                }

                BuildT = BuildType.ALPHA;
            }
            else if (digits[2].Contains("-beta"))
            {
                if (!UInt16.TryParse(digits[2].Replace("-beta", ""), out patch))
                {
                    Exception exception = new Exception($"{title} :\nPatch is not a conform digit.");
                    throw exception;
                }

                BuildT = BuildType.BETA;
            }
            else if (!UInt16.TryParse(digits[2], out patch))
            {
                Exception exception = new Exception($"{title} :\nPatch is not a conform digit.");
                throw exception;
            }

            if (digits.Length == 4)
            {
                if (!UInt16.TryParse(digits[3], out semver))
                {
                    Exception exception = new Exception($"{title} :\nSemiVer is not a conform digit.");
                    throw exception;
                }
            }

            Major = (UInt16)major;
            Minor = (UInt16)minor;
            Patch = (UInt16)patch;
            SemVer = (UInt16)semver;

            Version = ((UInt64)Major << 48) | ((UInt64)Minor << 32) | ((UInt64)Patch << 16) | SemVer;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// <see cref="UInt16"> representing the value of the first digit of a version. (Y.X.X(-alpha/beta).X)
        /// </summary>
        public UInt16 Major { get; set; }

        /// <summary>
        /// <see cref="UInt16"> representing the value of the second digit of a version. (X.Y.X(-alpha/beta).X)
        /// </summary>
        public UInt16 Minor { get; set; }

        /// <summary>
        /// <see cref="UInt16"> representing the value of the third digit of a version. (X.X.Y(-alpha/beta).X)
        /// </summary>
        public UInt16 Patch { get; set; }

        /// <summary>
        /// <see cref="UInt16"> representing the value of the fourth digit of a version. (X.X.X(-alpha/beta).Y)
        /// </summary>
        public UInt16 SemVer { get; set; }

        /// <summary>
        /// <see cref="UInt64"> representing the value of the version. ([Major, Minor, Path, SemVer])
        /// </summary>
        public UInt64 Version { get; set; }

        /// <summary>
        /// <see cref="BuildType"/> representing the actual build type of the version.
        /// </summary>
        public BuildType BuildT { get; set; }

        #endregion Properties

        #region Method

        /// <summary>
        /// Upgrade the version.
        /// </summary>
        /// <param name="buildT"><see cref="BuildType"/> representing build type to use for upgrading.</param>
        /// <param name="digitT"><see cref="DigitType"/> representing the digit to modify.</param>
        /// <param name="devId">DevId to used while constructing the semi version.</param>
        public void Upgrade(BuildType buildT, DigitType digitT, UInt16 devId)
        {
            string title = UVMLogger.CreateTitle(_asmName, _className, $"Upgrade");
            switch (buildT)
            {
                case BuildType.NONE:
                    Exception exceptionNone = new Exception($"{title} :\nCan not Upgrade using a VersionBuildType of NONE.");
                    throw exceptionNone;

                case BuildType.RELEASE:
                    _UpgradeRelease(digitT);
                    break;

                case BuildType.ALPHA:
                    _UpgradeAlpha(digitT, devId);
                    break;

                case BuildType.BETA:
                    _UpgradeBeta(digitT, devId);
                    break;

                default:
                    Exception exceptionDefault = new Exception($"{title} :\nCan not Upgrade using this VersionBuildType. (Not Supported)");
                    throw exceptionDefault;
            }

            Version = ((UInt64)Major << 48) | ((UInt64)Minor << 32) | ((UInt64)Patch << 16) | SemVer;
        }

        /// <summary>
        /// Compute the formated string corresponding to the version.
        /// </summary>
        /// <returns>The string form of the version.</returns>
        public override string ToString()
        {
            string suffix = string.Empty;

            switch (BuildT)
            {
                case BuildType.NONE:
                    suffix = string.Empty;
                    break;

                case BuildType.RELEASE:
                    suffix = string.Empty;
                    break;

                case BuildType.ALPHA:
                    suffix = $"-alpha.{SemVer}";
                    break;

                case BuildType.BETA:
                    suffix = $"-beta.{SemVer}";
                    break;

                default:
                    suffix = string.Empty;
                    break;
            }

            return $"{Major}.{Minor}.{Patch}{suffix}";
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
        /// Upgrade the version as a release.
        /// </summary>
        /// <param name="digitT"><see cref="DigitType"/> representing the digit to upgrade.</param>
        /// <exception cref="Exception"></exception>
        private void _UpgradeRelease(DigitType digitT)
        {
            string title = UVMLogger.CreateTitle(_asmName, _className, $"_UpgradeRelease");

            switch (digitT)
            {
                case DigitType.NONE:
                    Exception exceptionNone = new Exception($"{title} :\nCan not Upgrade using a DigitType of NONE.");
                    throw exceptionNone;

                case DigitType.MAJOR:
                    Major += 1;
                    Minor = 0;
                    Patch = 0;
                    SemVer = 0;
                    break;

                case DigitType.MINOR:
                    Minor += 1;
                    Patch = 0;
                    SemVer = 0;
                    break;

                case DigitType.PATCH:
                    Patch += 1;
                    SemVer = 0;
                    break;

                case DigitType.SEMVER:
                    Exception exceptionSemVer = new Exception($"{title} :\nCan not Upgrade the SemiVer when Upgrading for Release.");
                    throw exceptionSemVer;

                default:
                    Exception exceptionDefault = new Exception($"{title} :\nCan not Upgrade using this DigitType ({digitT}). (Not Supported)");
                    throw exceptionDefault;
            }
            BuildT = BuildType.RELEASE;
        }

        /// <summary>
        /// Upgrade the version as an alpha.
        /// </summary>
        /// <param name="digitT"><see cref="DigitType"/> representing the digit to upgrade.</param>
        /// <param name="devId">DevId to used while constructing the semi version.</param>
        /// <exception cref="Exception"></exception>
        private void _UpgradeAlpha(DigitType digitT, UInt16 devId)
        {
            string title = UVMLogger.CreateTitle(_asmName, _className, $"_UpgradeAlpha");

            switch (digitT)
            {
                case DigitType.NONE:
                    Exception exceptionNone = new Exception($"{title} :\nCan not Upgrade using a DigitType of NONE.");
                    throw exceptionNone;

                case DigitType.MAJOR:
                    Exception exceptionMajor = new Exception($"{title} :\nCan not Upgrade the Major when Upgrading for Alpha.");
                    throw exceptionMajor;

                case DigitType.MINOR:
                    Exception exceptionMinor = new Exception($"{title} :\nCan not Upgrade the Minor when Upgrading for Alpha.");
                    throw exceptionMinor;

                case DigitType.PATCH:
                    Exception exceptionPatch = new Exception($"{title} :\nCan not Upgrade the Patch when Upgrading for Alpha.");
                    throw exceptionPatch;

                case DigitType.SEMVER:
                    SemVer = _UpgradeSemiVer(SemVer, devId, BuildT, BuildType.ALPHA);
                    break;

                default:
                    Exception exceptionDefault = new Exception($"{title} :\nCan not Upgrade using this DigitType ({digitT}) for Alpha. (Not Supported)");
                    throw exceptionDefault;
            }
            BuildT = BuildType.ALPHA;
        }

        /// <summary>
        /// Upgrade the version as an beta.
        /// </summary>
        /// <param name="digitT"><see cref="DigitType"/> representing the digit to upgrade.</param>
        /// <param name="devId">DevId to used while constructing the semi version.</param>
        /// <exception cref="Exception"></exception>
        private void _UpgradeBeta(DigitType digitT, UInt16 devId)
        {
            string title = UVMLogger.CreateTitle(_asmName, _className, $"_UpgradeBeta");

            switch (digitT)
            {
                case DigitType.NONE:
                    Exception exceptionNone = new Exception($"{title} :\nCan not Upgrade using a DigitType of NONE.");
                    throw exceptionNone;

                case DigitType.MAJOR:
                    Exception exceptionMajor = new Exception($"{title} :\nCan not Upgrade the Major when Upgrading for Beta.");
                    throw exceptionMajor;

                case DigitType.MINOR:
                    Exception exceptionMinor = new Exception($"{title} :\nCan not Upgrade the Minor when Upgrading for Beta.");
                    throw exceptionMinor;

                case DigitType.PATCH:
                    Exception exceptionPatch = new Exception($"{title} :\nCan not Upgrade the Patch when Upgrading for Beta.");
                    throw exceptionPatch;

                case DigitType.SEMVER:
                    SemVer = _UpgradeSemiVer(SemVer, devId, BuildT, BuildType.BETA);
                    break;

                default:
                    Exception exceptionDefault = new Exception($"{title} :\nCan not Upgrade using this DigitType ({digitT}) for Beta. (Not Supported)");
                    throw exceptionDefault;
            }
            BuildT = BuildType.BETA;
        }

        /// <summary>
        /// Upgrade the SemVer using the XXYYZ.
        /// </summary>
        /// <param name="currentSemiVer">Current SemiVer.</param>
        /// <param name="devId">DevId to use for upgrading the SemiVer.</param>
        /// <param name="currentBuiltT">Current BuildType</param>
        /// <param name="nextBuildType">BuildType to use for upgrading the SemiVer.</param>
        /// <returns>The new SemiVer as a UInt16.</returns>
        private UInt16 _UpgradeSemiVer(UInt16 currentSemiVer, UInt16 devId, BuildType currentBuiltT, BuildType nextBuildType)
        {
            int Z = currentSemiVer % 10;
            int YY = (currentSemiVer % 1000 - Z) / 10;
            int XX = (currentSemiVer % 100000 - YY - Z) / 1000;

            if (currentBuiltT is BuildType.RELEASE && (nextBuildType is BuildType.ALPHA || nextBuildType is BuildType.BETA))
            {
                XX = 1;
                YY = devId;
                Z = 0;
            }
            else if (nextBuildType is BuildType.RELEASE)
            {
                // This path should not happen.
                XX = 0;
                YY = 0;
                Z = 0;
            }
            else
            {
                if (currentBuiltT == nextBuildType)
                {
                    if (YY == devId)
                    {
                        Z += 1;
                    }
                    else
                    {
                        YY = devId;
                        Z += 0;
                    }
                }
                else
                {
                    XX += 1;
                    YY = devId;
                    Z = 0;
                }
            }

            XX = XX * 1000;
            YY = YY * 10;

            return (UInt16)(XX + YY + Z);
        }

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
