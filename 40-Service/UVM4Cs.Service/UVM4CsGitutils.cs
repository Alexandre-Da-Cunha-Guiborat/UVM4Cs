using LibGit2Sharp;
using UVM.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using UVM.Interface.Interfaces;
using UVM4Cs.Bll;

namespace UVM4Cs.Service
{
    /// <summary>
    /// Small git utils for UVM4Cs project.
    /// </summary>
    public class UVM4CsGitutils
    {

        #region Singleton
        // TBD
        #endregion Singleton

        #region Public

        /// <summary>
        /// Check if the given folder is a git directory.
        /// </summary>
        /// <param name="gitDir"><see cref="String"/> representation of the absolute path to the git directory.</param>
        /// <returns><see langword="true"/> => the given path leads to a git directory, <see langword="false"/> => otherwise.</returns>
        public Boolean IsGitDirectory(String gitDir)
        {
            return UVMGitUtils.IsGitDirectory(gitDir);
        }

        /// <summary>
        /// Check if a rebase is needed.
        /// </summary>
        /// <param name="gitDir"><see cref="String"/> representation of the absolute path to the git directory.</param>
        /// <param name="commitIdRef"><see cref="String"/> representation of the commit Id we want to compare to..</param>
        /// <returns><see langword="true"/> => a rebase need to be done, <see langword="false"/> => otherwise.</returns>
        public Boolean IsRebaseNeeded(String gitDir, String commitIdRef)
        {
            if (!UVMGitUtils.IsGitDirectory(gitDir))
            {
                throw new Exception($"{gitDir}, is not leading to a git directory.");
            }

            Repository repo = new Repository(gitDir);
            Branch branch = repo.Head;
            String branchHeadId = GetBranchHeadCommitId(gitDir, branch.FriendlyName);

            return UVMGitUtils.IsRebaseNeeded(gitDir, branch.FriendlyName, commitIdRef, branchHeadId);
        }

        /// <summary>
        /// Compute the CommitId of the last commit of the given branch if it exists.
        /// </summary>
        /// <param name="gitDir"><see cref="String"/> representation of the absolute path to the git directory.</param>
        /// <param name="branchName"><see cref="String"/> representation of the branch name.</param>
        /// <returns>The ID of the last commit of the given branch if it exists.</returns>
        public String GetBranchHeadCommitId(String gitDir, String branchName)
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
        /// <param name="gitDir"><see cref="String"/> representation of the git repository.</param>
        /// <param name="branchName"><see cref="String"/> representation of the branch name.</param>
        /// <returns>The <see cref="String"/> representation of the commit Id.</returns>
        public String GetPrevCommitId(String gitDir, String branchName)
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
                Int32 lastCommidIdx = 1; // This is the way the commits are organized, first in the list is the newest commit.
                return branch.Commits.ElementAt(lastCommidIdx).Sha;
            }
        }

        /// <summary>
        /// Compute the <see cref="List{T}"/> of all <see cref="I_VersionableFile"> that has been modified.
        /// </summary>
        /// <param name="vfPool"><see cref="List{T}"/> of all <see cref="UVM4CsCsproj"> that may have been modified.</param>
        /// <param name="gitDirPaths"><see cref="List{T}"/> of <see cref="String"/> representing the absolute path to all git directories to consider.</param>
        /// <param name="commitIdsRef"><see cref="List{T}"/> of <see cref="String"/> representing the commit Id we want to compare to.</param>
        /// <param name="commitIds"><see cref="List{T}"/> of <see cref="String"/> representing the commitId we want to compare.</param>
        /// <param name="fExtensions"><see cref="List{T}"/> of <see cref="String"/> representing the extensions to look for in modified files.</param>
        /// <returns>The <see cref="List{T}"/> of reference to <see cref="UVM4CsCsproj"> that has been modified.</returns>
        public List<UVM4CsCsproj> ComputeModifiedCsproj(
            List<UVM4CsCsproj> vfPool,
            List<String> gitDirPaths,
            List<String> commitIdsRef,
            List<String> commitIds)
        {
            foreach (String gitDirPath in gitDirPaths)
            {
                if (!UVMGitUtils.IsGitDirectory(gitDirPath))
                {
                    throw new Exception($"{gitDirPath}, is not leading to a git directory.");
                }
            }

            List<I_VersionableFile> modifiedVf = UVMGitUtils.ComputeModifiedVF(vfPool.Cast<I_VersionableFile>().ToList(), gitDirPaths, commitIdsRef, commitIds);
            return modifiedVf.Cast<UVM4CsCsproj>().ToList();
        }

        /// <summary>
        /// Compute the <see cref="List{T}"/> of all <see cref="I_VersionableFile"> that have a modified file (with matching extensions) in it.
        /// </summary>
        /// <param name="vfPool"><see cref="List{T}"/> of all <see cref="I_VersionableFile"> that may have been modified.</param>
        /// <param name="gitDirPaths"><see cref="List{T}"/> of <see cref="String"/> representing the absolute path to all git directories to consider.</param>
        /// <param name="commitIdsRef"><see cref="List{T}"/> of <see cref="String"/> representing the commit Id we want to compare to.</param>
        /// <param name="commitIds"><see cref="List{T}"/> of <see cref="String"/> representing the commitId we want to compare.</param>
        /// <param name="fExtensions"><see cref="List{T}"/> of <see cref="String"/> representing the extensions to look for modified files.</param>
        /// <returnst>The <see cref="List{T}"/> of reference to <see cref="I_VersionableFile"> needing to be updated.</returns>
        public List<UVM4CsCsproj> ComputeCsprojWithModifiedFiles(
            List<UVM4CsCsproj> vfPool,
            List<String> gitDirPaths,
            List<String> commitIdsRef,
            List<String> commitIds,
            List<String> fExtensions)
        {
            foreach (String gitDirPath in gitDirPaths)
            {
                if (!UVMGitUtils.IsGitDirectory(gitDirPath))
                {
                    throw new Exception($"{gitDirPath}, is not leading to a git directory.");
                }
            }
            List<I_VersionableFile> vfWithModifiedFiles = UVMGitUtils.ComputeVFWithModifiedFiles(vfPool.Cast<I_VersionableFile>().ToList(), gitDirPaths, commitIdsRef, commitIds, fExtensions);
            return vfWithModifiedFiles.Cast<UVM4CsCsproj>().ToList();
        }

        /// <summary>
        /// Compute the <see cref="List{T}"/> of all <see cref="I_VersionableFile"> that has been modified or that have a file in its children folder that have been modified and that its extensions match one of the givens extensions.
        /// </summary>
        /// <param name="vfPool"><see cref="List{T}"/> of all <see cref="I_VersionableFile"> that may need to be updated.</param>
        /// <param name="gitDirPaths"><see cref="List{T}"/> of <see cref="String"/> representing the absolute path to a git directory.</param>
        /// <param name="commitIdRefs"><see cref="List{T}"/> of <see cref="String"/> representing the commit Id we want to compare to.</param>
        /// <param name="commitIds"><see cref="List{T}"/> of <see cref="String"/> representing the commitId we want to compare.</param>
        /// <param name="fExtensions"><see cref="List{T}"/> of <see cref="String"/> representing the extensions to look for modified files.</param>
        /// <returns>The <see cref="List{T}"/> of reference to <see cref="I_VersionableFile"> needing to be updated.</returns>
        public List<UVM4CsCsproj> ComputeModifiedCsprojAndCsprojWithModifiedFiles(
            List<UVM4CsCsproj> vfPool,
            List<String> gitDirPaths,
            List<String> commitIdRefs,
            List<String> commitIds,
            List<String> fExtensions)
        {
            foreach (String gitDirPath in gitDirPaths)
            {
                if (!UVMGitUtils.IsGitDirectory(gitDirPath))
                {
                    throw new Exception($"{gitDirPath}, is not leading to a git directory.");
                }
            }

            List<I_VersionableFile> modifiedVFAndVFWithModifiedFiles = UVMGitUtils.ComputeModifiedVFAndVFWithModifiedFiles(vfPool.Cast<I_VersionableFile>().ToList(), gitDirPaths, commitIdRefs, commitIds, fExtensions);
            return modifiedVFAndVFWithModifiedFiles.Cast<UVM4CsCsproj>().ToList();
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
        // private static String _asmName = Assembly.GetExecutingAssembly().Location ?? String.Empty;

        /// <summary>
        /// <see cref="String"/> representation of the class name.
        /// </summary>
        // private static String _className = nameof(UVM4CsConfiguration);

        #endregion DEBUG
    }
}
