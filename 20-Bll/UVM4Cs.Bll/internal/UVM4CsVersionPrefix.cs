using System;
using System.Reflection;
using UVM.Interface.Enums;
using UVM.Interface.Interfaces;
using UVM.Logging;

namespace UVM4Cs.Bll
{
    /// <summary>
    /// Class representing the VersionPrefix tag in csprojs.
    /// </summary>
    internal class UVM4CsVersionPrefix : I_Version
    {
        #region Singleton
        // TBD
        #endregion Singleton

        #region Public

        /// <summary>
        /// <see cref="UInt16"> representing the value of the first digit of a version. (Y.X.X.X)
        /// </summary>
        public UInt16 Major { get; private set; }

        /// <summary>
        /// <see cref="UInt16"> representing the value of the second digit of a version. (X.Y.X.X)
        /// </summary>
        public UInt16 Minor { get; private set; }

        /// <summary>
        /// <see cref="UInt16"> representing the value of the third digit of a version. (X.X.Y.X)
        /// </summary>
        public UInt16 Patch { get; private set; }

        /// <summary>
        /// <see cref="UInt16"> representing the value of the fourth digit of a version. (X.X.X.Y)
        /// </summary>
        public UInt16 SemVer { get; private set; }

        /// <summary>
        /// <see cref="UInt64"> representing the value of the version. ([Major, Minor, Path, SemVer])
        /// </summary>
        public UInt64 Version { get; private set; }

        /// <summary>
        /// <see cref="BuildType"/> representing the actual build type of the version.
        /// </summary>
        public BuildType BuildT { get; private set; }

        /// <summary>
        /// <see cref="UVM4CsVersionPrefix"/> constructor.
        /// </summary>
        /// <param name="version"><see cref="String"/> representation of the version.</param>
        public UVM4CsVersionPrefix(String version)
        {
            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(UVM4CsVersionPrefix)}");

            // Test for the version format.
            String[] digits = version.Split('.');
            if (digits.Length < 3)
            {
                Exception exception = new Exception($"{title} :\n{nameof(UVM4CsVersionPrefix)} must contains at least 3 digit.");
                throw exception;
            }

            UInt16 major = 0;
            UInt16 minor = 0;
            UInt16 patch = 0;
            UInt16 semver = 0;

            // Try to parse the version digits.
            if (!UInt16.TryParse(digits[0], out major))
            {
                Exception exception = new Exception($"{title} :\n{nameof(major)} is not a conform digit. ({digits[0]})");
                throw exception;
            }

            if (!UInt16.TryParse(digits[1], out minor))
            {
                Exception exception = new Exception($"{title} :\n{nameof(minor)} is not a conform digit. ({digits[1]})");
                throw exception;
            }

            if (!UInt16.TryParse(digits[2], out patch))
            {
                Exception exception = new Exception($"{title} :\n{nameof(patch)} is not a conform digit. ({digits[2]})");
                throw exception;
            }

            if (digits.Length == 4)
            {
                if (!UInt16.TryParse(digits[3], out semver))
                {
                    Exception exception = new Exception($"{title} :\n{nameof(semver)} is not a conform digit. ({digits[3]})");
                    throw exception;
                }
            }

            Major = (UInt16)major;
            Minor = (UInt16)minor;
            Patch = (UInt16)patch;
            SemVer = (UInt16)semver;
            BuildT = BuildType.RELEASE;

            Version = ((UInt64)Major << 48) | ((UInt64)Minor << 32) | ((UInt64)Patch << 16) | SemVer;
        }

        /// <summary>
        /// Upgrade the version.
        /// </summary>
        /// <param name="buildT"><see cref="BuildType"/> representing build type to use for upgrading.</param>
        /// <param name="digitT"><see cref="DigitType"/> representing the digit to modify.</param>
        /// <param name="devId">DevId to used while constructing the semi version.</param>
        public void Upgrade(BuildType buildT, DigitType digitT, UInt16 devId)
        {
            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(Upgrade)}");
            switch (buildT)
            {
                case BuildType.BuildType_NONE:
                    Exception exceptionNone = new Exception($"{title} :\nCan not upgrade using a {nameof(BuildType)} of {BuildType.BuildType_NONE}.");
                    throw exceptionNone;

                case BuildType.RELEASE:
                    _UpgradeRelease(digitT);
                    break;

                case BuildType.ALPHA:
                    _UpgradeAlpha(digitT);
                    break;

                case BuildType.BETA:
                    _UpgradeBeta(digitT);
                    break;

                default:
                    Exception exceptionDefault = new Exception($"{title} :\nCan not upgrade using this {nameof(BuildType)}. (Not Supported)");
                    throw exceptionDefault;
            }

            Version = ((UInt64)Major << 48) | ((UInt64)Minor << 32) | ((UInt64)Patch << 16) | SemVer;
        }

        /// <summary>
        /// Compute the formatted <see cref="String"/> corresponding to the version.
        /// </summary>
        /// <returns>The <see cref="String"/> form of the version.</returns>
        public override String ToString()
        {
            return $"{Major}.{Minor}.{Patch}.{SemVer}";
        }

        #endregion Public

        #region Protected
        // TBD
        #endregion Protected

        #region Private

        /// <summary>
        /// Upgrade the version as a release.
        /// </summary>
        /// <param name="digitT"><see cref="DigitType"/> representing the digit to upgrade.</param>
        /// <exception cref="Exception"></exception>
        private void _UpgradeRelease(DigitType digitT)
        {
            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(_UpgradeRelease)}");

            switch (digitT)
            {
                case DigitType.DigitType_NONE:
                    Exception exceptionNone = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {DigitType.DigitType_NONE}.");
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

                case DigitType.SEMI_VERSION:
                    Exception exceptionSemVer = new Exception($"{title} :\nCan not upgrade the SemiVer when Upgrading for Release.");
                    throw exceptionSemVer;

                default:
                    Exception exceptionDefault = new Exception($"{title} :\nCan not upgrade using this DigitType ({digitT}). (Not Supported)");
                    throw exceptionDefault;
            }
            BuildT = BuildType.RELEASE;
        }

        /// <summary>
        /// Upgrade the version as a alpha.
        /// </summary>
        /// <param name="digitT"><see cref="DigitType"/> representing the digit to upgrade.</param>
        /// <exception cref="Exception"></exception>
        private void _UpgradeAlpha(DigitType digitT)
        {
            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(_UpgradeAlpha)}");

            switch (digitT)
            {
                case DigitType.DigitType_NONE:
                    Exception exceptionNone = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {DigitType.DigitType_NONE}.");
                    throw exceptionNone;

                case DigitType.MAJOR:
                    Exception exceptionMajor = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {DigitType.MAJOR}.");
                    throw exceptionMajor;

                case DigitType.MINOR:
                    Exception exceptionMinor = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {DigitType.MINOR}.");
                    throw exceptionMinor;

                case DigitType.PATCH:
                    Exception exceptionPatch = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {DigitType.PATCH}.");
                    throw exceptionPatch;

                case DigitType.SEMI_VERSION:
                    SemVer += 1;
                    break;

                default:
                    Exception exceptionDefault = new Exception($"{title} :\nCan not upgrade using this DigitType ({digitT}). (Not Supported)");
                    throw exceptionDefault;
            }
            BuildT = BuildType.ALPHA;
        }

        /// <summary>
        /// Upgrade the version as a beta.
        /// </summary>
        /// <param name="digitT"><see cref="DigitType"/> representing the digit to upgrade.</param>
        /// <exception cref="Exception"></exception>
        private void _UpgradeBeta(DigitType digitT)
        {
            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(_UpgradeBeta)}");

            switch (digitT)
            {
                case DigitType.DigitType_NONE:
                    Exception exceptionNone = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {DigitType.DigitType_NONE}.");
                    throw exceptionNone;

                case DigitType.MAJOR:
                    Exception exceptionMajor = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {DigitType.MAJOR}.");
                    throw exceptionMajor;

                case DigitType.MINOR:
                    Exception exceptionMinor = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {DigitType.MINOR}.");
                    throw exceptionMinor;

                case DigitType.PATCH:
                    Exception exceptionPatch = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {DigitType.PATCH}.");
                    throw exceptionPatch;

                case DigitType.SEMI_VERSION:
                    SemVer += 1;
                    break;

                default:
                    Exception exceptionDefault = new Exception($"{title} :\nCan not upgrade using this DigitType ({digitT}). (Not Supported)");
                    throw exceptionDefault;
            }
            BuildT = BuildType.BETA;
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
        private static String _className = nameof(UVM4CsVersionPrefix);

        #endregion DEBUG
    }
}
