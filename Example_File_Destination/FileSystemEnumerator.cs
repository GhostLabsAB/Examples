using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GhostNodes.DestinationAssembly
{
    public enum FileSortOptions
    {
        None,        // Default
        FileNameAscending,
        FileNameDescending,
        CreationTimeUtcDescending,
        CreationTimeUtcAscending
    }

    /// <summary>
    /// File system enumerator.  This class provides an easy to use, efficient mechanism for searching a list of
    /// directories for files matching a list of file specifications.  The search is done incrementally as matches
    /// are consumed, so the overhead before processing the first match is always kept to a minimum.
    /// </summary>
    public sealed class FileSystemEnumerator : IDisposable
    {
        private string m_path;

        /// <summary>
        /// Array of regular expressions that will detect matching files.
        /// </summary>
        private List<Wildcard> m_fileSpecs;

        private List<string> m_filterList;

        /// <summary>
        /// If true, sub-directories are searched.
        /// </summary>
        public bool IncludeSubDirs { get; set; }
        public bool IncludeEmptyFiles { get; set; }
        public bool IncludeHiddenFiles { get; set; }
        public bool IncludeReadOnlyFiles { get; set; }
        public bool IncludeSystemFiles { get; set; }
        public bool IncludeTemporatyFiles { get; set; }



        public FileSystemEnumerator(string pathsToSearch)
            : this(pathsToSearch, "*", false)
        {
        }

        public FileSystemEnumerator(string pathsToSearch, string fileTypesToMatch)
            : this(pathsToSearch, fileTypesToMatch, false)
        {
        }

        #region public FileSystemEnumerator(string path, string fileTypesToMatch, bool includeSubDirs)
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">Path to search.</param>
        /// <param name="fileTypesToMatch">Semicolon- or comma-delimited list of wildcard file specs to match.</param>
        /// <param name="includeSubDirs">If true, subdirectories are searched.</param>
        public FileSystemEnumerator(string path, string fileTypesToMatch, bool includeSubDirs)
        {
            // check for nulls
            if (null == path)
                throw new ArgumentNullException("path");
            if (null == fileTypesToMatch)
                throw new ArgumentNullException("fileTypesToMatch");

            // make sure spec doesn't contain invalid characters
            if (fileTypesToMatch.IndexOfAny(new char[] { ':', '<', '>', '/', '\\' }) >= 0)
                throw new ArgumentException("invalid characters in wildcard pattern", "fileTypesToMatch");

            // check security - ensure that caller has rights to read this directory
            //new FileIOPermission(FileIOPermissionAccess.PathDiscovery, Path.Combine(path, ".")).Demand();

            m_path = path;
            IncludeSubDirs = includeSubDirs;
            IncludeEmptyFiles = false;
            IncludeHiddenFiles = false;
            IncludeReadOnlyFiles = false;
            IncludeSystemFiles = false;
            IncludeTemporatyFiles = false;


            m_filterList = new List<string>();
            m_filterList.AddRange(fileTypesToMatch.Split(new char[] { '|', ';', ',' }));
            m_fileSpecs = new List<Wildcard>(m_filterList.Count);

            foreach (string spec in m_filterList)
                m_fileSpecs.Add(new Wildcard(spec, RegexOptions.IgnoreCase));
        }
        #endregion

        #region public void Dispose()
        /// <summary>
        /// Releases the resources used by the <b>FileSystemEnumerator</b>.
        /// </summary>
        public void Dispose()
        {
        }
        #endregion


        public IEnumerable<FileInfo> Matches()
        {
            return Matches(FileSortOptions.None, TimeSpan.Zero);
        }
        public IEnumerable<FileInfo> Matches(FileSortOptions sortOrder)
        {
            return Matches(sortOrder, TimeSpan.Zero);
        }

        #region public IEnumerable<FileInfo> Matches()
        /// <summary>
        /// Get an enumerator that returns all of the files that match the wildcards that
        /// are in any of the directories to be searched.
        /// </summary>
        /// <returns>An IEnumerable that returns all matching files one by one.</returns>
        /// <remarks>The enumerator that is returned finds files using a lazy algorithm that
        /// searches directories incrementally as matches are consumed.</remarks>

        //public IEnumerable<FileInfo> Matches(FileSortOptions sortOrder = FileSortOptions.FileNameAscending, bool hourlyFiles = false)
        public IEnumerable<FileInfo> Matches(FileSortOptions sortOrder, TimeSpan minFileAge)
        {
            // Set the search options depending on directory matches
            SearchOption searchOpt = SearchOption.TopDirectoryOnly;
            if (IncludeSubDirs)
                searchOpt = SearchOption.AllDirectories;

            DirectoryInfo diTop = new DirectoryInfo(m_path);
            IEnumerable<FileInfo> files = null;
            foreach (string filter in m_filterList)
            {
                IEnumerable<FileInfo> tmpFiles = null;

                DateTime timeStamp = DateTime.UtcNow;

                // Only send files which are at least minFileAge old
                if (minFileAge != TimeSpan.Zero)
                    timeStamp = DateTime.UtcNow.Subtract(minFileAge);

                if (sortOrder == FileSortOptions.None)
                {
                    tmpFiles = from file in diTop.EnumerateFiles(filter, searchOpt)
                               where file.CreationTimeUtc < timeStamp
                               select file;
                }
                else if (sortOrder == FileSortOptions.CreationTimeUtcAscending)
                {
                    // LINQ query to sort by CreationTime
                    tmpFiles = from file in diTop.EnumerateFiles(filter, searchOpt)
                               where file.CreationTimeUtc < timeStamp
                               orderby file.CreationTimeUtc ascending
                               select file;
                }
                else if (sortOrder == FileSortOptions.CreationTimeUtcDescending)
                {
                    tmpFiles = from file in diTop.EnumerateFiles(filter, searchOpt)
                               where file.CreationTimeUtc < timeStamp
                               orderby file.CreationTimeUtc descending
                               select file;
                }
                else if (sortOrder == FileSortOptions.FileNameAscending)
                {
                    tmpFiles = from file in diTop.EnumerateFiles(filter, searchOpt)
                               where file.CreationTimeUtc < timeStamp
                               orderby file.Name ascending
                               select file;
                }
                else if (sortOrder == FileSortOptions.FileNameDescending)
                {
                    tmpFiles = from file in diTop.EnumerateFiles(filter, searchOpt)
                               where file.CreationTimeUtc < timeStamp
                               orderby file.Name descending
                               select file;
                }

                // Remove unwanted files
                if (!IncludeEmptyFiles)
                    tmpFiles = from file in tmpFiles where file.Length > 0 select file;

                if (!IncludeHiddenFiles)
                    tmpFiles = from file in tmpFiles where !file.Attributes.HasFlag(FileAttributes.Hidden) select file;

                if (!IncludeReadOnlyFiles)
                    tmpFiles = from file in tmpFiles where !file.Attributes.HasFlag(FileAttributes.ReadOnly) select file;

                if (!IncludeSystemFiles)
                    tmpFiles = from file in tmpFiles where !file.Attributes.HasFlag(FileAttributes.System) select file;

                if (!IncludeTemporatyFiles)
                    tmpFiles = from file in tmpFiles where !file.Attributes.HasFlag(FileAttributes.Temporary) select file;


                // Concatenate the files
                if (files != null)
                    files = files.Concat(tmpFiles);
                else
                    files = tmpFiles;
            }
            return files;
        }
        #endregion




        public FileInfo FindFirstFile()
        {
            return FindFirstFile(FileSortOptions.None, TimeSpan.Zero);
        }
        public FileInfo FindFirstFile(FileSortOptions sortOrder)
        {
            return FindFirstFile(sortOrder, TimeSpan.Zero);
        }


        #region public FileInfo FindFirstFile()
        /// <summary>
        /// Finds first file that matches pattern
        /// </summary>
        /// <returns>FileInfo for first file or null</returns>
        public FileInfo FindFirstFile(FileSortOptions sortOrder, TimeSpan minFileAge)
        {
            // Set the search options depending on directory matches
            SearchOption searchOpt = SearchOption.TopDirectoryOnly;
            if (IncludeSubDirs)
                searchOpt = SearchOption.AllDirectories;

            IEnumerable<FileInfo> files = null;
            DirectoryInfo diTop = new DirectoryInfo(m_path);
            foreach (string filter in m_filterList)
            {
                DateTime timeStamp = DateTime.UtcNow;

                // Only send files which are at least minFileAge old
                if (minFileAge != TimeSpan.Zero)
                    timeStamp = DateTime.UtcNow.Subtract(minFileAge);

                if (sortOrder == FileSortOptions.None)
                {
                    files = from file in diTop.EnumerateFiles(filter, searchOpt)
                            where file.CreationTimeUtc < timeStamp
                            select file;
                }
                else if (sortOrder == FileSortOptions.CreationTimeUtcAscending)
                {
                    // LINQ query to sort by CreationTime
                    files = from file in diTop.EnumerateFiles(filter, searchOpt)
                            where file.CreationTimeUtc < timeStamp
                            orderby file.CreationTimeUtc ascending
                            select file;
                }
                else if (sortOrder == FileSortOptions.CreationTimeUtcDescending)
                {
                    files = from file in diTop.EnumerateFiles(filter, searchOpt)
                            where file.CreationTimeUtc < timeStamp
                            orderby file.CreationTimeUtc descending
                            select file;
                }
                else if (sortOrder == FileSortOptions.FileNameAscending)
                {
                    files = from file in diTop.EnumerateFiles(filter, searchOpt)
                            where file.CreationTimeUtc < timeStamp
                            orderby file.Name ascending
                            select file;
                }
                else if (sortOrder == FileSortOptions.FileNameDescending)
                {
                    files = from file in diTop.EnumerateFiles(filter, searchOpt)
                            where file.CreationTimeUtc < timeStamp
                            orderby file.Name descending
                            select file;
                }

                // Remove unwanted files
                if (!IncludeEmptyFiles)
                    files = from file in files where file.Length > 0 select file;

                if (!IncludeHiddenFiles)
                    files = from file in files where !file.Attributes.HasFlag(FileAttributes.Hidden) select file;

                if (!IncludeReadOnlyFiles)
                    files = from file in files where !file.Attributes.HasFlag(FileAttributes.ReadOnly) select file;

                if (!IncludeSystemFiles)
                    files = from file in files where !file.Attributes.HasFlag(FileAttributes.System) select file;

                if (!IncludeTemporatyFiles)
                    files = from file in files where !file.Attributes.HasFlag(FileAttributes.Temporary) select file;

                // We have atleast one file matched, so break loop
                if (files != null && files.Count() > 0)
                    break;

            }

            if (files != null)
                return files.FirstOrDefault();
            else
                return null;
        }
        #endregion
    }
}
