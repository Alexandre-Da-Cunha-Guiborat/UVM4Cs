using LibGit2Sharp;
using UVM.Interface;
using UVM.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using UVM.Logging;

namespace UVM4Cs.Service
{
    /// <summary>
    /// Small git utils for UVM4Cs project.
    /// </summary>
    public static class UVM4CsGitutils
    {

        #region DEBUG

        /// <summary>
        /// String representation of the assembly name.
        /// </summary>
        // private const string _asmName = "UVM4Cs.Service";

        /// <summary>
        /// String representation of the class name.
        /// </summary>
        // private const string _className = "UVM4CsGitUtil";

        #endregion DEBUG

        #region Public

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

        /// <summary>
        /// Check if the given folder is a git directory.
        /// </summary>
        /// <param name="gitDir">String representation of the absolute path to the git directory.</param>
        /// <returns>true => the given path leads to a git directory, false => otherwise.</returns>
        public static bool IsGitDirectory(string gitDir)
        {
            return UVMGitUtils.IsGitDirectory(gitDir);
        }

        /// <summary>
        /// Check if a rebase is needed.
        /// </summary>
        /// <param name="gitDir">String representation of the absolute path to the git directory.</param>
        /// <param name="commitIdRef">String representation of the commit Id we want to compare to..</param>
        /// <returns>true => a rebase need to be done, false => otherwise.</returns>
        public static bool IsRebaseNeeded(string gitDir, string commitIdRef)
        {
            if (!UVMGitUtils.IsGitDirectory(gitDir))
            {
                throw new Exception($"{gitDir}, is not leading to a git directory.");
            }

            Repository repo = new Repository(gitDir);
            Branch branch = repo.Head;
            string branchHeadId = GetBranchHeadCommitId(gitDir, branch.FriendlyName);

            return UVMGitUtils.IsRebaseNeeded(gitDir, branch.FriendlyName, commitIdRef, branchHeadId);
        }

        /// <summary>
        /// Compute the CommitId of the last commit of the given branch if it exists.
        /// </summary>
        /// <param name="gitDir">String representation of the absolute path to the git directory.</param>
        /// <param name="branchName">String representation of the branch name.</param>
        /// <returns>The ID of the last commit of the given branch if it exists.</returns>
        public static string GetBranchHeadCommitId(string gitDir, string branchName)
        {

            if (!UVMGitUtils.IsGitDirectory(gitDir))
            {
                throw new Exception($"{gitDir}, is not leading to a git directory.");
            }

            Repository repo = new(gitDir);
            Branch compareBranch = repo.Branches[branchName];

            if (compareBranch == null)
            {
                throw new Exception($"{branchName}, is not leading to a local branch.");
            }
            else
            {
                return compareBranch.Commits.First().Sha;
            }
        }

        /// <summary>
        /// Get the CommitId of the previous commit in the current branch.
        /// </summary>
        /// <param name="gitDir">String representation of the git repository.</param>
        /// <param name="branchName">String representation of the branch name.</param>
        /// <returns>A string representation of the commit Id.</returns>
        public static string GetPrevCommitId(string gitDir, string branchName)
        {
            if (!UVMGitUtils.IsGitDirectory(gitDir))
            {
                throw new Exception($"{gitDir}, is not leading to a git directory.");
            }

            Repository repo = new(gitDir);
            Branch branch = repo.Branches[branchName];

            if (branch == null)
            {
                throw new Exception($"{branchName}, is not leading to a local branch.");
            }
            else
            {
                int lastCommidIdx = 1; // This is the way the commits are organized, first in the list is the newest commit.
                return branch.Commits.ElementAt(lastCommidIdx).Sha;
            }
        }

        /// <summary>
        /// Compute the list of all <see cref="I_VersionnableFile"> that has been modified.
        /// </summary>
        /// <param name="vfPool">List of all <see cref="I_VersionnableFile"> that may have been modified.</param>
        /// <param name="gitDirPaths">List of string representating the absolute path to all git directories to consider.</param>
        /// <param name="commitIdsRef">List of string representating the commit Id we want to compare to.</param>
        /// <param name="commitIds">List of string representating the commitId we want to compare.</param>
        /// <param name="fExtensions">List of string representing the extensions to look for in modified files.</param>
        /// <returns>A list of reference to <see cref="I_VersionnableFile"> that has been modified.</returns>
        public static List<I_VersionnableFile> ComputeModifiedVF(
            List<I_VersionnableFile> vfPool,
            List<string> gitDirPaths,
            List<string> commitIdsRef,
            List<string> commitIds)
        {
            foreach(string gitDirPath in gitDirPaths)
            {
                if (!UVMGitUtils.IsGitDirectory(gitDirPath))
                {
                    throw new Exception($"{gitDirPath}, is not leading to a git directory.");
                }
            }
            
            return UVMGitUtils.ComputeModifiedVF(vfPool, gitDirPaths, commitIdsRef, commitIds);
        }

        /// <summary>
        /// Compute the list of all <see cref="I_VersionnableFile"> that have a modified file (with matching extensions) in it.
        /// </summary>
        /// <param name="vfPool">List of all <see cref="I_VersionnableFile"> that may have been modified.</param>
        /// <param name="gitDirPaths">List of string representating the absolute path to all git directories to consider.</param>
        /// <param name="commitIdsRef">List of string representating the commit Id we want to compare to.</param>
        /// <param name="commitIds">List of string representating the commitId we want to compare.</param>
        /// <param name="fExtensions">List of string representing the extensions to look for modified files.</param>
        /// <returns>A list of reference to <see cref="I_VersionnableFile"> needing to be updated.</returns>
        public static List<I_VersionnableFile> ComputeVFWithModifiedFiles(
            List<I_VersionnableFile> vfPool,
            List<string> gitDirPaths,
            List<string> commitIdsRef,
            List<string> commitIds,
            List<string> fExtensions)
        {
            foreach (string gitDirPath in gitDirPaths)
            {
                if (!UVMGitUtils.IsGitDirectory(gitDirPath))
                {
                    throw new Exception($"{gitDirPath}, is not leading to a git directory.");
                }
            }

            return UVMGitUtils.ComputeVFWithModifiedFiles(vfPool, gitDirPaths, commitIdsRef, commitIds, fExtensions);
        }

        /// <summary>
        /// Compute the list of all <see cref="I_VersionnableFile"> that has been modified or that have a file in its children forlder that have been modified and that its extensions match one of the givens extensions.
        /// </summary>
        /// <param name="vfPool">List of all <see cref="I_VersionnableFile"> that may need to be updated.</param>
        /// <param name="gitDirPaths">List of string representing the absolute path to a git directory.</param>
        /// <param name="commitIdRefs">List of string representing the commit Id we want to compare to.</param>
        /// <param name="commitIds">List of string representing the commitId we want to compare.</param>
        /// <param name="fExtensions">List of string representing the extensions to look for modified files.</param>
        /// <returns>A list of reference to <see cref="I_VersionnableFile"> needing to be updated.</returns>
        public static List<I_VersionnableFile> ComputeModifiedVFAndVFWithModifiedFiles(List<I_VersionnableFile> vfPool, List<string> gitDirPaths, List<string> commitIdRefs, List<string> commitIds, List<string> fExtensions)
        {
            foreach (string gitDirPath in gitDirPaths)
            {
                if (!UVMGitUtils.IsGitDirectory(gitDirPath))
                {
                    throw new Exception($"{gitDirPath}, is not leading to a git directory.");
                }
            }

            return UVMGitUtils.ComputeModifiedVFAndVFWithModifiedFiles(vfPool, gitDirPaths, commitIdRefs, commitIds, fExtensions);
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
