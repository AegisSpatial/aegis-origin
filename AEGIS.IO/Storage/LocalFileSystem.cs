﻿/// <copyright file="LocalFileSystem.cs" company="Eötvös Loránd University (ELTE)">
///     Copyright (c) 2011-2014 Roberto Giachetta. Licensed under the
///     Educational Community License, Version 2.0 (the "License"); you may
///     not use this file except in compliance with the License. You may
///     obtain a copy of the License at
///     http://opensource.org/licenses/ECL-2.0
///
///     Unless required by applicable law or agreed to in writing,
///     software distributed under the License is distributed on an "AS IS"
///     BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
///     or implied. See the License for the specific language governing
///     permissions and limitations under the License.
/// </copyright>
/// <author>Roberto Giachetta</author>

using ELTE.AEGIS.IO.Storage.Authentication;
using System;
using System.IO;
using System.Linq;

namespace ELTE.AEGIS.IO.Storage
{
    /// <summary>
    /// Represents the local file system.
    /// </summary>
    public class LocalFileSystem : FileSystem
    {
        #region Private static fields

        /// <summary>
        /// The URI of the localhost. This field is read-only.
        /// </summary>
        private static readonly Uri LocalhostUri = new Uri("file://localhost/");

        /// <summary>
        /// The anyonymous authentication. This field is read-only.
        /// </summary>
        private static readonly IFileSystemAuthentication AnyonymousAuthentication = new AnonymousFileSystemAuthentication();

        #endregion

        #region FileSystem public properties

        /// <summary>
        /// Gets the scheme name for this file system.
        /// </summary>
        /// <value>The scheme name for this file system.</value>
        public override String UriScheme { get { return Uri.UriSchemeFile; } }

        /// <summary>
        /// Gets the directory separator for this file system.
        /// </summary>
        /// <value>The directory separator for this file system.</value>
        public override Char DirectorySeparator { get { return Path.DirectorySeparatorChar; } }

        /// <summary>
        /// Gets the path separator for this file system.
        /// </summary>
        /// <value>The path separator for this file system.</value>
        public override Char PathSeparator { get { return Path.PathSeparator; } }

        /// <summary>
        /// Gets the volume separator for this file system.
        /// </summary>
        /// <value>The volume separator for this file system.</value>
        public override Char VolumeSeparator { get { return Path.VolumeSeparatorChar; } }

        /// <summary>
        /// Gets a value indicating whether the file system is connected.
        /// </summary>
        /// <value><c>true</c> if operations can be executed on the file system; otherwise, <c>false</c>.</value>
        public override Boolean IsConnected 
        { 
            get 
            {
                try
                {
                    return Directory.GetLogicalDrives().Length > 0;
                }
                catch 
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether file streaming is supported by the file system.
        /// </summary>
        /// <value><c>true</c> if file streaming commands can be executed on the file system; otherwise, <c>false</c>.</value>
        public override Boolean IsStreamingSupported { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether content browsing is supported by the file system.
        /// </summary>
        /// <value><c>true</c> if the content of the file system can be listed; otherwise, <c>false</c>.</value>
        public override Boolean IsContentBrowsingSupported { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether content writing is supported by the file system.
        /// </summary>
        /// <value><c>true</c> if file creation, modification and removal operations are supported by the file system; otherwise, <c>false</c>.</value>
        public override Boolean IsContentWritingSupported { get { return true; } }

        /// <summary>
        /// Gets the authentication used by the file system.
        /// </summary>
        /// <value>The authentication used by the file system.</value>
        public override IFileSystemAuthentication Authentication { get { return AnyonymousAuthentication; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileSystem"/> class.
        /// </summary>
        public LocalFileSystem() : base(LocalhostUri) { }

        #endregion

        #region FileSystem public methods

        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="path">The path of the directory to create.</param>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
        /// or
        /// The path exceeds the maximum length supported by the file system.
        /// or
        /// The specified path is a file.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for the path.</exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public override void CreateDirectory(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            if (File.Exists(path))
                throw new ArgumentException(MessagePathIsFile, "path");

            try
            {
                Directory.CreateDirectory(path);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
            }
            catch (NotSupportedException ex)
            {
                throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
            }
            catch (PathTooLongException ex)
            {
                throw new ArgumentException(MessagePathTooLong, "path", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new ConnectionException(MessageNoConnectionToPath, path, ex);
            }
            catch (IOException ex)
            {
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex);
            }
        }

        /// <summary>
        /// Creates or overwrites a file on the specified path.
        /// </summary>
        /// <param name="path">The path of a file to create.</param>
        /// <param name="overwrite">A value that specifies whether the file should be overwritten in case it exists.</param>
        /// <returns>A stream that provides read/write access to the file specified in path.</returns>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
        /// or
        /// The path exceeds the maximum length supported by the file system.
        /// or
        /// The path already exists.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for the path.</exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public override Stream CreateFile(String path, Boolean overwrite)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            if (!overwrite && Exists(path))
                throw new ArgumentException(MessagePathExists, "path");

            try
            {
                return File.Create(path);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
            }
            catch (NotSupportedException ex)
            {
                throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
            }
            catch (PathTooLongException ex)
            {
                throw new ArgumentException(MessagePathTooLong, "path", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new ConnectionException(MessageNoConnectionToPath, path, ex);
            }
            catch (IOException ex)
            {
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex);
            }
        }

        /// <summary>
        /// Opens a stream on the specified path.
        /// </summary>
        /// <param name="path">The path of a file to open.</param>
        /// <param name="mode">A value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten.</param>
        /// <param name="access">A value that specifies the operations that can be performed on the file.</param>
        /// <returns>A stream in the specified mode and path, with the specified access.</returns>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path does not exist.
        /// or
        /// The path is a directory.
        /// or
        /// The path is in an invalid format.
        /// or
        /// The path exceeds the maximum length supported by the file system.
        /// or
        /// The file mode is invalid.
        /// or
        /// The file access is invalid.
        /// or
        /// The specified file mode and file access combination is invalid.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The file on path is read-only.
        /// or
        /// The caller does not have the required permission for the path.
        /// </exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public override Stream OpenFile(String path, FileMode mode, FileAccess access)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                return File.Open(path, mode, access);
            }
            catch (FileNotFoundException ex)
            {
                throw new ArgumentException(MessagePathNotExists, "path", ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                if (ex.ParamName == "mode")
                    throw new ArgumentException(MessageInvalidFileMode, "path", ex);
                else
                    throw new ArgumentException(MessageInvalidFileAccess, "mode", ex);
            }
            catch (ArgumentException ex)
            {
                if (ex.ParamName == "path")
                    throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
                else
                    throw new ArgumentException(MessageInvalidFileModeOrAccess, "access", ex);
            }
            catch (NotSupportedException ex)
            {
                throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
            }
            catch (PathTooLongException ex)
            {
                throw new ArgumentException(MessagePathTooLong, "path", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                if (Directory.Exists(path))
                    throw new ArgumentException(MessagePathIsDirectory, "path", ex);

                if (mode == FileMode.Create || mode == FileMode.CreateNew)
                    throw new ArgumentException(MessagePathNotExists, "path", ex);
                if (access == FileAccess.ReadWrite || access == FileAccess.Write)
                    throw new UnauthorizedAccessException(MessagePathReadOnly, ex);

                throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new ConnectionException(MessageNoConnectionToPath, path, ex);
            }
            catch (IOException ex)
            {
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex);
            }
        }

        /// <summary>
        /// Deletes the filesystem entry located at the specified path.
        /// </summary>
        /// <param name="path">The path of the entry to delete.</param>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
        /// or
        /// The path exceeds the maximum length supported by the file system.
        /// or
        /// The path does not exist.
        /// or
        /// The file system entry on the specified path is currently in use.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for the path.</exception>
        public override void Delete(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            // determine whether the specified path is a directory or file
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    return;
                }
                catch (PathTooLongException ex)
                {
                    throw new ArgumentException(MessagePathTooLong, "path", ex);
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
                }
                catch (IOException ex)
                {
                    throw new ArgumentException(MessagePathInUse, "path", ex);
                }
            }
            else
            {
                try
                {
                    Directory.Delete(path, true);
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
                }
                catch (NotSupportedException ex)
                {
                    throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
                }
                catch (PathTooLongException ex)
                {
                    throw new ArgumentException(MessagePathTooLong, "path", ex);
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
                }
                catch (DirectoryNotFoundException ex)
                {
                    throw new ArgumentException(MessagePathNotExists, "path", ex);
                }
                catch (IOException ex)
                {
                    throw new ArgumentException(MessagePathInUse, "path", ex);
                }
            }
        }

        /// <summary>
        /// Moves a filesystem entry and its contents to a new location.
        /// </summary>
        /// <param name="sourcePath">The path of the file or directory to move.</param>
        /// <param name="destinationPath">The path to the new location for the entry.</param>
        /// <exception cref="System.ArgumentNullException">
        /// The source path is null.
        /// or
        /// The destination path is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The source path is empty, or consists only of whitespace characters.
        /// or
        /// The destination path is empty, or consists only of whitespace characters.
        /// or
        /// The source and destination paths are equal.
        /// or
        /// The source path does not exist.
        /// or
        /// The destination path already exists.
        /// or
        /// The source path is in an invalid format.
        /// or
        /// The destination path is in an invalid format.
        /// or
        /// The path exceeds the maximum length supported by the file system.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for either the source or the destination path.</exception>
        /// <exception cref="ConnectionException">No connection is available to the specified destination path.</exception>
        public override void Move(String sourcePath, String destinationPath)
        {
            if (sourcePath == null)
                throw new ArgumentNullException("sourcePath", MessageSourcePathIsNull);
            if (destinationPath == null)
                throw new ArgumentNullException("destinationPath", MessageDestinationPathIsNull);
            if (String.IsNullOrWhiteSpace(sourcePath))
                throw new ArgumentException(MessageSourcePathIsEmpty, "sourcePath");
            if (String.IsNullOrWhiteSpace(destinationPath))
                throw new ArgumentException(MessageDestinationPathIsEmpty, "destinationPath");

            if (sourcePath.Equals(destinationPath))
                throw new ArgumentException(MessageSourceDestinationPathEqual, "destinationPath");

            if (Exists(destinationPath))
                throw new ArgumentException(MessageDestinationPathExists, "destinationPath");

            // move to entry to the new destination
            try
            {
                Directory.Move(sourcePath, destinationPath);
            }
            catch (ArgumentException ex)
            {
                if (ex.ParamName == "sourceDirName")
                    throw new ArgumentException(MessageSourcePathInvalidFormat, "sourcePath", ex);
                else
                    throw new ArgumentException(MessageDestinationPathInvalidFormat, "destinationPath", ex);
            }
            catch (PathTooLongException ex)
            {
                throw new ArgumentException(MessageSourceDestinationPathTooLong, "sourcePath", ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new ArgumentException(MessageSourcePathNotExists, "sourcePath", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(MessageSourceDestinationPathUnauthorized, ex);
            }
            catch (IOException ex)
            {
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex);
            }  
        }

        /// <summary>
        /// Copies an existing filesystem entry to a new location.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="destinationPath">The destination path.</param>
        /// <exception cref="System.ArgumentNullException">
        /// The source path is null.
        /// or
        /// The destination path is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The source path is empty, or consists only of whitespace characters.
        /// or
        /// The destination path is empty, or consists only of whitespace characters.
        /// or
        /// The source and destination paths are equal.
        /// or
        /// The source path does not exist.
        /// or
        /// The destination path already exists.
        /// or
        /// The source path is in an invalid format.
        /// or
        /// The destination path is in an invalid format.
        /// or
        /// The path exceeds the maximum length supported by the file system.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for either the source or the destination path.</exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public override void Copy(String sourcePath, String destinationPath)
        {
            if (sourcePath == null)
                throw new ArgumentNullException("sourcePath", MessageSourcePathIsNull);
            if (destinationPath == null)
                throw new ArgumentNullException("destinationPath", MessageDestinationPathIsNull);
            if (String.IsNullOrWhiteSpace(sourcePath))
                throw new ArgumentException(MessageSourcePathIsEmpty, "sourcePath");
            if (String.IsNullOrWhiteSpace(destinationPath))
                throw new ArgumentException(MessageDestinationPathIsEmpty, "destinationPath");

            if (sourcePath.Equals(destinationPath))
                throw new ArgumentException(MessageSourceDestinationPathEqual, "destinationPath");

            if (Exists(destinationPath))
                throw new ArgumentException(MessageDestinationPathExists, "destinationPath");

            // move to entry to the new destination
            try
            {
                if (Directory.Exists(sourcePath))
                {
                    CopyDirectory(sourcePath, destinationPath);
                }
                else
                {
                    File.Copy(sourcePath, destinationPath);
                }
            }
            catch (FileNotFoundException ex)
            {
                throw new ArgumentException(MessageSourcePathNotExists, "sourcePath", ex);
            }
            catch (ArgumentException ex)
            {
                if (ex.ParamName == "sourceFileName")
                    throw new ArgumentException(MessageSourcePathInvalidFormat, "sourcePath", ex);
                else
                    throw new ArgumentException(MessageDestinationPathInvalidFormat, "destinationPath", ex);
            }
            catch (NotSupportedException ex)
            {
                throw new ArgumentException(MessageSourcePathInvalidFormat, "sourcePath", ex);
            }
            catch (PathTooLongException ex)
            {
                throw new ArgumentException(MessageSourceDestinationPathTooLong, "sourcePath", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(MessageSourceDestinationPathUnauthorized, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new ConnectionException(MessageNoConnectionToPath, destinationPath, ex);
            }
            catch (IOException ex)
            {
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex);
            } 
        }

        /// <summary>
        /// Determines whether the specified path exists.
        /// </summary>
        /// <param name="path">The path of a file or directory to check.</param>
        /// <returns><c>true</c> if the path exists, otherwise, <c>false</c>.</returns>
        public override Boolean Exists(String path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }

        /// <summary>
        /// Determines whether the specified path is an existing directory.
        /// </summary>
        /// <param name="path">The path of a directory to check.</param>
        /// <returns><c>true</c> if the path exists, and is a directory, otherwise, <c>false</c>.</returns>
        public override Boolean IsDirectory(String path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// Determines whether the specified path is an existing file.
        /// </summary>
        /// <param name="path">The path of a file to check.</param>
        /// <returns><c>true</c> if the path exists, and is a file, otherwise, <c>false</c>.</returns>
        public override Boolean IsFile(String path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Returns the root information for the specified path.
        /// </summary>
        /// <param name="path">The path of a file or directory.</param>
        /// <returns>A string containing the root information.</returns>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
        /// or
        /// The path exceeds the maximum length supported by the file system.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for the path.</exception>
        public override String GetDirectoryRoot(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                return Directory.GetDirectoryRoot(path);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
            }
            catch (PathTooLongException ex)
            {
                throw new ArgumentException(MessagePathTooLong, "path", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
            }
        }

        /// <summary>
        /// Returns the path of the parent directory for the specified path.
        /// </summary>
        /// <param name="path">The path of a file or directory.</param>
        /// <returns>The string containing the full path to the parent directory.</returns>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
        /// or
        /// The path exceeds the maximum length supported by the file system.
        /// or
        /// The path does not exist.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for the path.</exception>
        public override String GetParent(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                DirectoryInfo info = Directory.GetParent(path);

                // in case the root directory is queried, the return value is null
                if (info == null)
                    return null;

                return info.FullName;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
            }
            catch (PathTooLongException ex)
            {
                throw new ArgumentException(MessagePathTooLong, "path", ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new ArgumentException(MessagePathNotExists, "path", ex);
            }
            catch (IOException ex)
            {
                throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
            }
        }

        /// <summary>
        /// Returns the full directory name for a specified path.
        /// </summary>
        /// <param name="path">The path of a file or directory.</param>
        /// <returns>The full directory name for <paramref name="path" />.</returns>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
        /// or
        /// The path exceeds the maximum length supported by the file system.
        /// </exception>
        public override String GetDirectory(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                return Path.GetDirectoryName(path);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
            }
            catch (PathTooLongException ex)
            {
                throw new ArgumentException(MessagePathTooLong, "path", ex);
            }
        }

        /// <summary>
        /// Returns the file name and file extension for a specified path.
        /// </summary>
        /// <param name="path">The path of a file.</param>
        /// <returns>The file name and file extension for <paramref name="path" />.</returns>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
        /// </exception>
        public override String GetFileName(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                return Path.GetFileName(path);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
            }
        }

        /// <summary>
        /// Returns the file name without file extension for a specified path.
        /// </summary>
        /// <param name="path">The path of a file.</param>
        /// <returns>The file name without file extension for <paramref name="path" />.</returns>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
        /// </exception>
        public override String GetFileNameWithoutExtension(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");
            
            try
            {
                return Path.GetFileNameWithoutExtension(path);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
            }
        }

        /// <summary>
        /// Returns the names of the root directories of the file system.
        /// </summary>
        /// <returns>The array containing the root directories in the file system.</returns>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="ConnectionException">No connection is available to the file system.</exception>
        public override String[] GetRootDirectories()
        {
            try
            {
                return Directory.GetLogicalDrives();
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(MessageUnauthorized, ex);
            }
            catch (IOException ex)
            {
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex);
            }
        }

        /// <summary>
        /// Returns the directories located on the specified path.
        /// </summary>
        /// <param name="path">The path of the directory to search.</param>
        /// <param name="searchPattern">The search string to match against the names of files in path.</param>
        /// <param name="recursive">A value that specifies whether subdirectories are included in the search.</param>
        /// <returns>An array containing the full paths to all directories.</returns>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
        /// or
        /// The path exceeds the maximum length supported by the file system.
        /// or
        /// The path does not exist.
        /// or
        /// The specified path is a file.
        /// or
        /// The search pattern is an invalid format.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for the path.</exception>
        /// <exception cref="ConnectionException">No connection is available to the file system.</exception>
        public override String[] GetDirectories(String path, String searchPattern, Boolean recursive)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                return Directory.GetDirectories(path, searchPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            }
            catch (ArgumentException ex)
            {
                if (ex.ParamName == "searchPattern")
                    throw new ArgumentException(MessageInvalidSearchPattern, "searchPattern", ex);
                else
                    throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
            }
            catch (PathTooLongException ex)
            {
                throw new ArgumentException(MessagePathTooLong, "path", ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new ArgumentException(MessagePathNotExists, "path", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
            }
            catch (IOException ex)
            {
                if (File.Exists(path))
                    throw new ArgumentException(MessagePathIsFile, "path", ex);

                throw new ConnectionException(MessageNoConnectionToFileSystem, ex);
            }
        }

        /// <summary>
        /// Returns the files located on the specified path.
        /// </summary>
        /// <param name="path">The path of the directory to search.</param>
        /// <param name="searchPattern">The search string to match against the names of files in path.</param>
        /// <param name="recursive">A value that specifies whether subdirectories are included in the search.</param>
        /// <returns>An array containing the full paths to all files.</returns>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
        /// or
        /// The path exceeds the maximum length supported by the file system.
        /// or
        /// The path does not exist.
        /// or
        /// The specified path is a file.
        /// or
        /// The search pattern is an invalid format.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for the path.</exception>
        /// <exception cref="ConnectionException">No connection is available to the file system.</exception>
        public override String[] GetFiles(String path, String searchPattern, Boolean recursive)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                return Directory.GetFiles(path, searchPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            }
            catch (ArgumentException ex)
            {
                if (ex.ParamName == "searchPattern")
                    throw new ArgumentException(MessageInvalidSearchPattern, "searchPattern", ex);
                else
                    throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
            }
            catch (PathTooLongException ex)
            {
                throw new ArgumentException(MessagePathTooLong, "path", ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new ArgumentException(MessagePathNotExists, "path", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
            }
            catch (IOException ex)
            {
                if (File.Exists(path))
                    throw new ArgumentException(MessagePathIsFile, "path", ex);

                throw new ConnectionException(MessageNoConnectionToFileSystem, ex);
            }
        }

        /// <summary>
        /// Returns the file system entries located on the specified path.
        /// </summary>
        /// <param name="path">The path of the directory to search.</param>
        /// <param name="searchPattern">The search string to match against the names of files in path.</param>
        /// <param name="recursive">A value that specifies whether subdirectories are included in the search.</param>
        /// <returns>An array containing the full paths to all file system entries.</returns>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
        /// or
        /// The path exceeds the maximum length supported by the file system.
        /// or
        /// The path does not exist.
        /// or
        /// The path is a file.
        /// or
        /// The search pattern is an invalid format.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for the path.</exception>
        /// <exception cref="ConnectionException">No connection is available to the file system.</exception>
        public override String[] GetFileSystemEntries(String path, String searchPattern, Boolean recursive)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                return Directory.GetFileSystemEntries(path, searchPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            }
            catch (ArgumentException ex)
            {
                if (ex.ParamName == "searchPattern")
                    throw new ArgumentException(MessageInvalidSearchPattern, "searchPattern", ex);
                else
                    throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
            }
            catch (PathTooLongException ex)
            {
                throw new ArgumentException(MessagePathTooLong, "path", ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new ArgumentException(MessagePathNotExists, "path", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
            }
            catch (IOException ex)
            {
                if (File.Exists(path))
                    throw new ArgumentException(MessagePathIsFile, "path", ex);

                throw new ConnectionException(MessageNoConnectionToFileSystem, ex);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Copies the specified directory.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="destinationPath">The destination path.</param>
        private void CopyDirectory(String sourcePath, String destinationPath)
        {
            // create the destination directory
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            // copy the files in the directory
            foreach (String sourceFilePath in Directory.GetFiles(sourcePath))
            {
                String destinationFilePath = Path.Combine(destinationPath, Path.GetFileName(sourceFilePath));
                File.Copy(sourceFilePath, destinationFilePath);
            }

            // copy all subdirectories
            foreach (String sourceDirectoryPath in Directory.GetDirectories(sourcePath))
            {
                String destinationDirectoryPath = Path.Combine(destinationPath, Path.GetFileName(sourceDirectoryPath));
                CopyDirectory(sourceDirectoryPath, destinationDirectoryPath);
            }
        }

        #endregion
    }
}
