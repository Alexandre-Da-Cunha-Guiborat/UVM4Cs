using System;
using System.Reflection;
using UVM.Interface.Enums;
using UVM.Interface.Interfaces;
using UVM.Logging;

namespace UVM4Cs.Bll
{
    /// <summary>
    /// Class representing the VersionSuffix tag in csprojs.
    /// </summary>
    internal class UVM4CsVersionSuffix : I_Version
    {
        #region Singleton
        // TBD
        #endregion Singleton

        #region Public

        /// <summary>
        /// <see cref="UInt16"> representing the value of the <see cref="BuildType"/>  of a version. (alpha.X.X.X)
        /// </summary>
        public UInt16 Major { get; private set; }

        /// <summary>
        /// <see cref="UInt16"> representing the value of the first digit of a version. (alpha.Y.X.X)
        /// </summary>
        public UInt16 Minor { get; private set; }

        /// <summary>
        /// <see cref="UInt16"> representing the value of the second digit of a version. (alpha.X.Y.X)
        /// </summary>
        public UInt16 Patch { get; private set; }

        /// <summary>
        /// <see cref="UInt16"> representing the value of the third digit of a version. (alpha.X.X.Y)
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
        /// <see cref="UVM4CsVersionSuffix"/> constructor.
        /// </summary>
        /// <param name="version"><see cref="String"/> representation of the version.</param>
        public UVM4CsVersionSuffix(String version)
        {
            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(UVM4CsVersionSuffix)}");
            Major = 0;
            Minor = 0;
            Patch = 0;
            SemVer = 0;
            BuildT = BuildType.BuildType_NONE;

            // Test for the version format.
            String[] digits = version.Split('.');
            if (digits.Length < 4)
            {
                Exception exception = new Exception($"{title} :\n{nameof(UVM4CsVersionSuffix)} must contains exactly 4 digit.");
                throw exception;
            }

            BuildType buildType = BuildType.BuildType_NONE;
            UInt16 build = 0;
            UInt16 iteration = 0;
            UInt16 devId = 0;

            if (digits[0].Equals($"alpha"))
            {
                buildType = BuildType.ALPHA;
            }
            else if (digits[0].Equals($"beta"))
            {
                buildType = BuildType.BETA;
            }
            else
            {
                buildType = BuildType.BuildType_NONE;
            }

            if (!UInt16.TryParse(digits[1], out build))
            {
                Exception exception = new Exception($"{title} :\n{nameof(build)} is not a conform digit. ({digits[1]})");
                throw exception;
            }

            if (!UInt16.TryParse(digits[2], out iteration))
            {
                Exception exception = new Exception($"{title} :\n{nameof(iteration)} is not a conform digit. ({digits[2]})");
                throw exception;
            }

            if (!UInt16.TryParse(digits[3].Replace($"-DEBUG", String.Empty), out devId))
            {
                Exception exception = new Exception($"{title} :\n{nameof(devId)} is not a conform digit. ({digits[3]})");
                throw exception;
            }

            Major = (UInt16)buildType;
            Minor = (UInt16)build;
            Patch = (UInt16)iteration;
            SemVer = (UInt16)devId;
            BuildT = buildType;

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
                    _UpgradeAlpha(digitT, devId);
                    break;

                case BuildType.BETA:
                    _UpgradeBeta(digitT, devId);
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
            String buildT = String.Empty;

            switch (BuildT)
            {
                case BuildType.BuildType_NONE:
                    buildT = String.Empty;
                    break;

                case BuildType.RELEASE:
                    buildT = String.Empty;
                    break;

                case BuildType.ALPHA:
                    buildT = $"alpha";
                    break;

                case BuildType.BETA:
                    buildT = $"beta";
                    break;

                default:
                    buildT = String.Empty;
                    break;
            }

            return $"{buildT}.{Minor}.{Patch}.{SemVer}";
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
                    Exception exceptionNone = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {nameof(DigitType.DigitType_NONE)}.");
                    throw exceptionNone;

                case DigitType.MAJOR:
                    Exception exceptionMajor = new Exception($"{title} :\nCan not Upgrade using a {nameof(DigitType)} of {nameof(DigitType.MAJOR)}.");
                    throw exceptionMajor;

                case DigitType.MINOR:
                    Exception exceptionMinor = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {nameof(DigitType.MINOR)}.");
                    throw exceptionMinor;

                case DigitType.PATCH:
                    Exception exceptionPatch = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {nameof(DigitType.PATCH)}.");
                    throw exceptionPatch;

                case DigitType.SEMI_VERSION:
                    Exception exceptionSemiVersion = new Exception($"{title} :\nCan not Upgrade using this {nameof(DigitType)} ({digitT}). (Not Supported)");
                    throw exceptionSemiVersion;

                default:
                    Exception exceptionDefault = new Exception($"{title} :\nCan not upgrade using this DigitType ({digitT}). (Not Supported)");
                    throw exceptionDefault;
            }
        }

        /// <summary>
        /// Upgrade the version as an alpha.
        /// </summary>
        /// <param name="digitT"><see cref="DigitType"/> representing the digit to upgrade.</param>
        /// <param name="devId">DevId to used while constructing the semi version.</param>
        /// <exception cref="Exception"></exception>
        private void _UpgradeAlpha(DigitType digitT, UInt16 devId)
        {
            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(_UpgradeAlpha)}");

            switch (digitT)
            {
                case DigitType.DigitType_NONE:
                    Exception exceptionNone = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {nameof(DigitType.DigitType_NONE)}.");
                    throw exceptionNone;

                case DigitType.MAJOR:
                    Exception exceptionMajor = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {nameof(DigitType.MAJOR)}.");
                    throw exceptionMajor;

                case DigitType.MINOR:
                    Exception exceptionMinor = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {nameof(DigitType.MINOR)}.");
                    throw exceptionMinor;

                case DigitType.PATCH:
                    Exception exceptionPatch = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {nameof(DigitType.PATCH)}.");
                    throw exceptionPatch;

                case DigitType.SEMI_VERSION:
                    _UpgradeSemiVer(devId, BuildT, BuildType.ALPHA);
                    break;

                default:
                    Exception exceptionDefault = new Exception($"{title} :\nCan not upgrade using this {nameof(DigitType)} ({digitT}) for Alpha. (Not Supported)");
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
            String title = UVMLogger.CreateTitle(_asmName, _className, $"{nameof(_UpgradeBeta)}");

            switch (digitT)
            {
                case DigitType.DigitType_NONE:
                    Exception exceptionNone = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {nameof(DigitType.DigitType_NONE)}.");
                    throw exceptionNone;

                case DigitType.MAJOR:
                    Exception exceptionMajor = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {nameof(DigitType.MAJOR)}.");
                    throw exceptionMajor;

                case DigitType.MINOR:
                    Exception exceptionMinor = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {nameof(DigitType.MINOR)}.");
                    throw exceptionMinor;

                case DigitType.PATCH:
                    Exception exceptionPatch = new Exception($"{title} :\nCan not upgrade using a {nameof(DigitType)} of {nameof(DigitType.PATCH)}.");
                    throw exceptionPatch;

                case DigitType.SEMI_VERSION:
                    _UpgradeSemiVer(devId, BuildT, BuildType.BETA);
                    break;

                default:
                    Exception exceptionDefault = new Exception($"{title} :\nCan not upgrade using this {nameof(DigitType)} ({digitT}) for Beta. (Not Supported)");
                    throw exceptionDefault;
            }

            BuildT = BuildType.BETA;
        }

        /// <summary>
        /// Upgrade the SemVer using the XXYYZ.
        /// </summary>
        /// <param name="devId">DevId to use for upgrading the semi version.</param>
        /// <param name="currentBuiltT">Current <see cref="BuildType"/>.</param>
        /// <param name="nextBuildType"><see cref="BuildType"/> to use for upgrading the semi version.</param>
        /// <returns>The new semi version as a <see cref="UInt16"/>.</returns>
        private void _UpgradeSemiVer(UInt16 devId, BuildType currentBuiltT, BuildType nextBuildType)
        {
            if (currentBuiltT == nextBuildType)
            {
                Patch += 1;
                SemVer = devId;
            }
            else
            {
                Minor += 1;
                Patch = 0;
                SemVer = devId;
            }

            Major = (UInt16)nextBuildType;
            BuildT = nextBuildType;
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
        private static String _className = nameof(UVM4CsVersionSuffix);

        #endregion DEBUG
    }
}
