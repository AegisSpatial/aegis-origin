﻿/// <copyright file="HadoopFileSystem.cs" company="Eötvös Loránd University (ELTE)">
///     Copyright (c) 2011-2022 Roberto Giachetta. Licensed under the
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
/// <author>Tamás Nagy</author>

using ELTE.AEGIS.IO.Storage.Authentication;
using ELTE.AEGIS.IO.Storage.Operation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ELTE.AEGIS.IO.Storage
{
    /// <summary>
    /// Represents a HDFS file system.
    /// </summary>
    public class HadoopFileSystem : FileSystem
    {
        #region Private fields

        /// <summary>
        /// The root path within the file system location. This field is constant.
        /// </summary>
        private const String RootPath = "webhdfs/v1";

        /// <summary>
        /// The HTTP client.
        /// </summary>
        private HttpClient _client;

        #endregion

        #region Public FileSystem properties

        /// <summary>
        /// Gets the scheme name for this file system.
        /// </summary>
        /// <value>The scheme name for this file system.</value>
        public override String UriScheme { get { return "hdfs"; } }

        /// <summary>
        /// Gets the directory separator for this file system.
        /// </summary>
        /// <value>The directory separator for this file system.</value>
        public override Char DirectorySeparator { get { return '/'; } }

        /// <summary>
        /// Gets the path separator for this file system.
        /// </summary>
        /// <value>The path separator for this file system.</value>
        public override Char PathSeparator { get { return Path.PathSeparator; } }

        /// <summary>
        /// Gets the volume separator for this file system.
        /// </summary>
        /// <value>The volume separator for this file system.</value>
        public override Char VolumeSeparator { get { return '/'; } }

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
                    return IsDirectory("/");
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
        /// Gets a value indicating whether the connection to the file system is secure.
        /// </summary>
        /// <value><c>true</c> if operations ans credentials are handled in a secure manner; otherwise, <c>false</c>.</value>
        public override Boolean IsSecureConnection { get { return false; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HadoopFileSystem" /> class.
        /// </summary>
        /// <param name="hostname">The hostname.</param>
        /// <param name="portNumber">The port number.</param>
        /// <exception cref="System.ArgumentNullException">The hostname is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The hostname is empty, or consists only of whitespace characters.
        /// or
        /// The hostname is in an invalid format.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The port number is less than 1.</exception>
        public HadoopFileSystem(String hostname, Int32 portNumber) : this(CreateLocation(hostname, portNumber)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HadoopFileSystem" /> class.
        /// </summary>
        /// <param name="hostname">The HDFS hostname.</param>
        /// <param name="portNumber">The HDFS port.</param>
        /// <param name="authentication">The authentication.</param>
        /// <exception cref="System.ArgumentNullException">
        /// The hostname is null.
        /// or
        /// The authentication is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The hostname is empty, or consists only of whitespace characters.
        /// or
        /// The hostname is in an invalid format.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The port number is less than 1.</exception>
        public HadoopFileSystem(String hostname, Int32 portNumber, IHadoopFileSystemAuthentication authentication) : this(CreateLocation(hostname, portNumber), authentication) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HadoopFileSystem" /> class.
        /// </summary>
        /// <param name="hostname">The HDFS hostname.</param>
        /// <param name="portNumber">The HDFS port.</param>
        /// <param name="authentication">The authentication.</param>
        /// <param name="client">The HTTP client.</param>
        /// <exception cref="System.ArgumentNullException">
        /// The hostname is null.
        /// or
        /// The authentication is null.
        /// or
        /// The HTTP client is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The hostname is empty, or consists only of whitespace characters.
        /// or
        /// The hostname is in an invalid format.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The port number is less than 1.</exception>
        public HadoopFileSystem(String hostname, Int32 portNumber, IHadoopFileSystemAuthentication authentication, HttpClient client) : this(CreateLocation(hostname, portNumber), authentication, client) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HadoopFileSystem" /> class.
        /// </summary>
        /// <param name="hostname">The hostname.</param>
        /// <param name="portNumber">The port number.</param>
        /// <param name="client">The HTTP client.</param>
        /// <exception cref="System.ArgumentNullException">
        /// The hostname is null.
        /// or
        /// The HTTP client is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The hostname is empty, or consists only of whitespace characters.
        /// or
        /// The hostname is in an invalid format.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The port number is less than 1.</exception>
        public HadoopFileSystem(String hostname, Int32 portNumber, HttpClient client) : this(CreateLocation(hostname, portNumber), client) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HadoopFileSystem" /> class.
        /// </summary>
        /// <param name="location">The URI of the file system location.</param>
        /// <exception cref="System.ArgumentNullException">The location is null.</exception>
        public HadoopFileSystem(Uri location)
            : base(location, new HadoopAnonymousAuthentication())
        {
            if (location == null)
                throw new ArgumentNullException("location", "The location is null.");

            _client = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HadoopFileSystem" /> class.
        /// </summary>
        /// <param name="location">The location of the file system.</param>
        /// <param name="authentication">The authentication used by file system operation.</param>
        /// <exception cref="System.ArgumentNullException">
        /// The location is null.
        /// or
        /// The authentication is null.
        /// </exception>
        public HadoopFileSystem(Uri location, IHadoopFileSystemAuthentication authentication)
            : base(location, authentication)
        {
            if (location == null)
                throw new ArgumentNullException("location", "The location is null.");
            if (authentication == null)
                throw new ArgumentNullException("authentication", "The authentication is null.");

            _client = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HadoopFileSystem" /> class.
        /// </summary>
        /// <param name="location">The location of the file system.</param>
        /// <param name="authentication">The authentication used by file system operation.</param>
        /// <param name="client">The HTTP client.</param>
        /// <exception cref="System.ArgumentNullException">
        /// The location is null.
        /// or
        /// The authentication is null.
        /// or
        /// The HTTP client is null.
        /// </exception>
        public HadoopFileSystem(Uri location, IHadoopFileSystemAuthentication authentication, HttpClient client)
            : base(location, authentication)
        {
            if (location == null)
                throw new ArgumentNullException("location", "The location is null.");
            if (authentication == null)
                throw new ArgumentNullException("authentication", "The authentication is null.");
            if (client == null)
                throw new ArgumentNullException("client", "The client is null.");

            _client = client;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HadoopFileSystem" /> class.
        /// </summary>
        /// <param name="location">The location of the file system.</param>
        /// <param name="client">The HTTP client.</param>
        /// <exception cref="System.ArgumentNullException">
        /// The location is null.
        /// or
        /// The HTTP client is null.
        /// </exception>
        public HadoopFileSystem(Uri location, HttpClient client)
            : base(location, new HadoopAnonymousAuthentication())
        {
            if (location == null)
                throw new ArgumentNullException("location", "The location is null.");
            if (client == null)
                throw new ArgumentNullException("client", "The client is null.");

            _client = client;
        }

        #endregion

        #region Public FileSystem methods

        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="path">The path of the directory to create.</param>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
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

            try
            {
                HadoopFileSystemOperation operation;

                // create operation
                if (_client != null)
                    operation = new HadoopCreateDirectoryOperation(_client, Location.ToString() + RootPath + path, Authentication as IHadoopFileSystemAuthentication);
                else
                    operation = new HadoopCreateDirectoryOperation(Location.ToString() + RootPath + path, Authentication as IHadoopFileSystemAuthentication);

                // execute operation
                HadoopFileSystemOperationResult result = operation.ExecuteAsync().Result;

                // process the result
                if (!(result as HadoopBooleanOperationResult).Success)
                    throw new ArgumentException(MessagePathNotExists, "path");
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "WebApplicationException":
                            return; // the directory already exists
                        case "IllegalArgumentException":
                            throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
                        case "FileNotFoundException":
                        case "NotFoundException":
                            throw new ArgumentException(MessagePathNotExists, "path", ex);
                        case "SecurityException":
                            throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Creates the directory asynchronously.
        /// </summary>
        /// <param name="path">The path of the directory to create.</param>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for the path.</exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public override async Task CreateDirectoryAsync(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                HadoopFileSystemOperation operation;

                // create operation
                if (_client != null)
                    operation = new HadoopCreateDirectoryOperation(_client, Location.ToString() + RootPath + path, Authentication as IHadoopFileSystemAuthentication);
                else
                    operation = new HadoopCreateDirectoryOperation(Location.ToString() + RootPath + path, Authentication as IHadoopFileSystemAuthentication);

                // execute operation
                HadoopFileSystemOperationResult result = await operation.ExecuteAsync();

                // process the result
                if (!(result as HadoopBooleanOperationResult).Success)
                    throw new ArgumentException(MessagePathNotExists, "path");
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "WebApplicationException":
                            return; // the directory already exists
                        case "IllegalArgumentException":
                            throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
                        case "FileNotFoundException":
                        case "NotFoundException":
                            throw new ArgumentException(MessagePathNotExists, "path", ex);
                        case "SecurityException":
                            throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Creates or overwrites a file on the specified path.
        /// </summary>
        /// <param name="path">The path of a file to create.</param>
        /// <param name="overwrite">A value that specifies whether the file should be overwritten in case it exists.</param>
        /// <returns>A stream that provides read/write access to the file specified in path.</returns>
        /// <exception cref="System.NotSupportedException">The operation is not supported by the file system.</exception>
        public override Stream CreateFile(String path, Boolean overwrite)
        {
            // TODO
            throw new NotSupportedException("The operation is not supported by the file system.");
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
        /// <exception cref="System.NotSupportedException">The operation is not supported by the file system.</exception>
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

            // TODO: process write operations

            try
            {
                HadoopFileSystemOperation operation;

                // create operation
                if (_client != null)
                    operation = new HadoopReadFileOperation(_client, Location.ToString() + RootPath + path, Authentication as IHadoopFileSystemAuthentication);
                else
                    operation = new HadoopReadFileOperation(Location.ToString() + RootPath + path, Authentication as IHadoopFileSystemAuthentication);

                // execute operation
                HadoopFileSystemOperationResult result = operation.ExecuteAsync().Result;

                // process the result                
                return (result as HadoopFileStreamingOperationResult).FileStream;
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                            throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
                        case "FileNotFoundException":
                        case "NotFoundException":
                            throw new ArgumentException(MessagePathNotExists, "path", ex);
                        case "SecurityException":
                            throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Opens a stream asynchronously on the specified path.
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
        /// <exception cref="System.NotSupportedException">The operation is not supported by the file system.</exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public override async Task<Stream> OpenFileAsync(String path, FileMode mode, FileAccess access)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            // TODO: process write operations

            try
            {
                HadoopFileSystemOperation operation;

                // create operation
                if (_client != null)
                    operation = new HadoopReadFileOperation(_client, Location.ToString() + RootPath + path, Authentication as IHadoopFileSystemAuthentication);
                else
                    operation = new HadoopReadFileOperation(Location.ToString() + RootPath + path, Authentication as IHadoopFileSystemAuthentication);

                // execute operation
                HadoopFileSystemOperationResult result = await operation.ExecuteAsync();

                // process the result                
                return (result as HadoopFileStreamingOperationResult).FileStream;
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                            throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
                        case "FileNotFoundException":
                        case "NotFoundException":
                            throw new ArgumentException(MessagePathNotExists, "path", ex);
                        case "SecurityException":
                            throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Deletes the file system entry located at the specified path.
        /// </summary>
        /// <param name="path">The path of the entry to delete.</param>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
        /// or
        /// The path does not exist.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for the path.</exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public override void Delete(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                HadoopFileSystemOperation operation;

                // create operation
                if (_client != null)
                    operation = new HadoopDeleteOperation(_client, Location.ToString() + RootPath + path, Authentication as IHadoopFileSystemAuthentication, true);
                else
                    operation = new HadoopDeleteOperation(Location.ToString() + RootPath + path, Authentication as IHadoopFileSystemAuthentication, true);

                // execute operation
                HadoopFileSystemOperationResult result = operation.ExecuteAsync().Result;

                // process the result                
                if (!(result as HadoopBooleanOperationResult).Success)
                    throw new ArgumentException(MessagePathNotExists, "path");
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                            throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
                        case "FileNotFoundException":
                        case "NotFoundException":
                            throw new ArgumentException(MessagePathNotExists, "path", ex);
                        case "SecurityException":
                            throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Deletes the file system entry located at the specified path asynchronously.
        /// </summary>
        /// <param name="path">The path of the entry to delete.</param>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
        /// or
        /// The path does not exist.
        /// or
        /// The file system entry on the specified path is currently in use.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for the path.</exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public async override Task DeleteAsync(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                HadoopFileSystemOperation operation;

                // create operation
                if (_client != null)
                    operation = new HadoopDeleteOperation(_client, Location.ToString() + RootPath + path, Authentication as IHadoopFileSystemAuthentication, true);
                else
                    operation = new HadoopDeleteOperation(Location.ToString() + RootPath + path, Authentication as IHadoopFileSystemAuthentication, true);

                // execute operation
                HadoopFileSystemOperationResult result = await operation.ExecuteAsync();

                // process the result                
                if (!(result as HadoopBooleanOperationResult).Success)
                    throw new ArgumentException(MessagePathNotExists, "path");
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                            throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
                        case "FileNotFoundException":
                        case "NotFoundException":
                            throw new ArgumentException(MessagePathNotExists, "path", ex);
                        case "SecurityException":
                            throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Moves a file system entry and its contents to a new location.
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
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for either the source or the destination path.</exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
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

            try
            {
                HadoopFileSystemOperation operation;

                // create operation
                if (_client != null)
                    operation = new HadoopRenameOperation(_client, Location.ToString() + RootPath + sourcePath, Authentication as IHadoopFileSystemAuthentication, destinationPath);
                else
                    operation = new HadoopRenameOperation(Location.ToString() + RootPath + sourcePath, Authentication as IHadoopFileSystemAuthentication, destinationPath);

                // execute operation
                HadoopFileSystemOperationResult result = operation.ExecuteAsync().Result;

                // process the result                
                if (!(result as HadoopBooleanOperationResult).Success)
                    throw new ArgumentException(MessagePathNotExists, "sourcePath");
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                            throw new ArgumentException(MessagePathInvalidFormat, "sourcePath", ex);
                        case "FileNotFoundException":
                        case "NotFoundException":
                            throw new ArgumentException(MessagePathNotExists, "sourcePath", ex);
                        case "SecurityException":
                            throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, sourcePath, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        // <summary>
        /// Moves a file system entry asynchronously to a new location.
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
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for either the source or the destination path.</exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public async override Task MoveAsync(String sourcePath, String destinationPath)
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

            try
            {
                HadoopFileSystemOperation operation;

                // create operation
                if (_client != null)
                    operation = new HadoopRenameOperation(_client, Location.ToString() + RootPath + sourcePath, Authentication as IHadoopFileSystemAuthentication, destinationPath);
                else
                    operation = new HadoopRenameOperation(Location.ToString() + RootPath + sourcePath, Authentication as IHadoopFileSystemAuthentication, destinationPath);

                // execute operation
                HadoopFileSystemOperationResult result = await operation.ExecuteAsync();

                // process the result                
                if (!(result as HadoopBooleanOperationResult).Success)
                    throw new ArgumentException(MessagePathNotExists, "sourcePath");
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                            throw new ArgumentException(MessagePathInvalidFormat, "sourcePath", ex);
                        case "FileNotFoundException":
                        case "NotFoundException":
                            throw new ArgumentException(MessagePathNotExists, "sourcePath", ex);
                        case "SecurityException":
                            throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, sourcePath, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Copies an existing file system entry to a new location.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="destinationPath">The destination path.</param>
        /// <exception cref="System.NotSupportedException">The operation is not supported by the file system.</exception>
        public override void Copy(String sourcePath, String destinationPath)
        {
            throw new NotSupportedException(MessageNotSupported);
        }

        /// <summary>
        /// Determines whether the specified path exists.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the path exists, otherwise, <c>false</c>.</returns>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public override Boolean Exists(String path)
        {
            if (path == null)
                return false;
            if (String.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                HadoopFileSystemOperationResult result = GetFileEntryStatusAsync(path).Result;

                return true;
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                        case "FileNotFoundException":
                        case "NotFoundException":
                        case "SecurityException":
                            return false;
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Asynchronously determines whether the specified path exists.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the path exists, otherwise, <c>false</c>.</returns>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public async override Task<Boolean> ExistsAsync(String path)
        {
            if (path == null)
                return false;
            if (String.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                HadoopFileSystemOperationResult result = await GetFileEntryStatusAsync(path);

                return true;
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                        case "FileNotFoundException":
                        case "NotFoundException":
                        case "SecurityException":
                            return false;
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Determines whether the specified path is an existing directory.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the path exists, and is a directory, otherwise, <c>false</c>.</returns>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public override Boolean IsDirectory(String path)
        {
            if (path == null)
                return false;
            if (String.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                HadoopFileSystemOperationResult result = GetFileEntryStatusAsync(path).Result;

                // process the result
                return (result as HadoopFileStatusOperationResult).EntryType == FileSystemEntryType.Directory;
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                        case "FileNotFoundException":
                        case "NotFoundException":
                        case "SecurityException":
                            return false;
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Asynchronously determines whether the specified path is an existing directory.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the path exists, and is a directory, otherwise, <c>false</c>.</returns>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public async override Task<Boolean> IsDirectoryAsync(String path)
        {
            if (path == null)
                return false;
            if (String.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                HadoopFileSystemOperationResult result = await GetFileEntryStatusAsync(path);

                // process the result
                return (result as HadoopFileStatusOperationResult).EntryType == FileSystemEntryType.Directory;
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                        case "FileNotFoundException":
                        case "NotFoundException":
                        case "SecurityException":
                            return false;
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Determines whether the specified path is an existing file.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the path exists, and is a file, otherwise, <c>false</c>.</returns>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public override Boolean IsFile(String path)
        {
            if (path == null)
                return false;
            if (String.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                HadoopFileSystemOperationResult result = GetFileEntryStatusAsync(path).Result;

                // process the result
                return (result as HadoopFileStatusOperationResult).EntryType == FileSystemEntryType.File;
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                        case "FileNotFoundException":
                        case "NotFoundException":
                        case "SecurityException":
                            return false;
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Asynchronously determines whether the specified path is an existing file.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the path exists, and is a file, otherwise, <c>false</c>.</returns>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public async override Task<Boolean> IsFileAsync(String path)
        {
            if (path == null)
                return false;
            if (String.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                HadoopFileSystemOperationResult result = await GetFileEntryStatusAsync(path);

                // process the result
                return (result as HadoopFileStatusOperationResult).EntryType == FileSystemEntryType.File;
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                        case "FileNotFoundException":
                        case "NotFoundException":
                        case "SecurityException":
                            return false;
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Returns the root information for the specified path.
        /// </summary>
        /// <param name="path">The path of a file or directory.</param>
        /// <returns>A string containing the root information.</returns>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">The path is empty, or consists only of whitespace characters.</exception>
        public override String GetDirectoryRoot(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            return DirectorySeparator.ToString();
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
        /// The path does not exist.
        /// </exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public override String GetParent(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            // in case the root directory is queried, the return value is null
            if (path.Length == 1 && path[0] == DirectorySeparator)
                return null;

            path = path.TrimEnd(DirectorySeparator);

            try
            {
                HadoopFileSystemOperationResult result = GetFileEntryStatusAsync(path).Result;

                // the path exists, take the directory part
                if (path.LastIndexOf(DirectorySeparator) == 0)
                    return DirectorySeparator.ToString();
                else
                    return path.Substring(0, path.LastIndexOf(DirectorySeparator));
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                        case "FileNotFoundException":
                        case "NotFoundException":
                        case "SecurityException":
                            throw new ArgumentException(MessagePathNotExists, "path");
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Returns the path of the parent directory for the specified path asynchronously.
        /// </summary>
        /// <param name="path">The path of a file or directory.</param>
        /// <returns>The string containing the full path to the parent directory.</returns>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path exceeds the maximum length supported by the file system.
        /// or
        /// The path does not exist.
        /// </exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public async override Task<String> GetParentAsync(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            // in case the root directory is queried, the return value is null
            if (path.Length == 1 && path[0] == DirectorySeparator)
                return null;

            path = path.TrimEnd(DirectorySeparator);

            try
            {
                HadoopFileSystemOperationResult result = await GetFileEntryStatusAsync(path);

                // the path exists, take the directory part
                if (path.LastIndexOf(DirectorySeparator) == 0)
                    return DirectorySeparator.ToString();
                else
                    return path.Substring(0, path.LastIndexOf(DirectorySeparator));
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                        case "FileNotFoundException":
                        case "NotFoundException":
                        case "SecurityException":
                            throw new ArgumentException(MessagePathNotExists, "path");
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
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
        /// The path does not exist.
        /// </exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public override String GetDirectory(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            if (path.Length > 1)
                path = path.TrimEnd(DirectorySeparator);

            try
            {
                HadoopFileSystemOperationResult result = GetFileEntryStatusAsync(path).Result;

                // the path exists, take the directory part
                switch ((result as HadoopFileStatusOperationResult).EntryType)
                { 
                    case FileSystemEntryType.Directory:
                        return path;
                    default:
                        if (path.LastIndexOf(DirectorySeparator) == 0)
                            return DirectorySeparator.ToString();
                        else
                            return path.Substring(0, path.LastIndexOf(DirectorySeparator));
                }
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                        case "FileNotFoundException":
                        case "NotFoundException":
                        case "SecurityException":
                            throw new ArgumentException(MessagePathNotExists, "path");
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Returns the full directory name for a specified path asynchronously.
        /// </summary>
        /// <param name="path">The path of a file or directory.</param>
        /// <returns>The full directory name for <paramref name="path" />.</returns>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// or
        /// The path exceeds the maximum length supported by the file system.
        /// or
        /// The path does not exist.
        /// </exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public async override Task<String> GetDirectoryAsync(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            if (path.Length > 1)
                path = path.TrimEnd(DirectorySeparator);

            try
            {
                HadoopFileSystemOperationResult result = await GetFileEntryStatusAsync(path);

                // the path exists, take the directory part
                switch ((result as HadoopFileStatusOperationResult).EntryType)
                {
                    case FileSystemEntryType.Directory:
                        return path;
                    default:
                        if (path.LastIndexOf(DirectorySeparator) == 0)
                            return DirectorySeparator.ToString();
                        else
                            return path.Substring(0, path.LastIndexOf(DirectorySeparator));
                }
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                        case "FileNotFoundException":
                        case "NotFoundException":
                        case "SecurityException":
                            throw new ArgumentException(MessagePathNotExists, "path");
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
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
        /// The path does not exist.
        /// </exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public override String GetFileName(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                HadoopFileSystemOperationResult result = GetFileEntryStatusAsync(path).Result;

                // the path exists, take the file part
                switch ((result as HadoopFileStatusOperationResult).EntryType)
                {
                    case FileSystemEntryType.Directory:
                        return String.Empty;
                    default:
                        return path.Substring(path.LastIndexOf(DirectorySeparator));
                }
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                        case "FileNotFoundException":
                        case "NotFoundException":
                        case "SecurityException":
                            throw new ArgumentException(MessagePathNotExists, "path");
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Returns the file name and file extension for a specified path asynchronously.
        /// </summary>
        /// <param name="path">The path of a file.</param>
        /// <returns>The file name and file extension for <paramref name="path" />.</returns>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path does not exist.
        /// </exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public async override Task<String> GetFileNameAsync(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                HadoopFileSystemOperationResult result = await GetFileEntryStatusAsync(path);

                // the path exists, take the file part
                switch ((result as HadoopFileStatusOperationResult).EntryType)
                {
                    case FileSystemEntryType.Directory:
                        return String.Empty;
                    default:
                        return path.Substring(path.LastIndexOf(DirectorySeparator));
                }
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                        case "FileNotFoundException":
                        case "NotFoundException":
                        case "SecurityException":
                            throw new ArgumentException(MessagePathNotExists, "path");
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
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
                HadoopFileSystemOperationResult result = GetFileEntryStatusAsync(path).Result;

                // the path exists, take the file part
                switch ((result as HadoopFileStatusOperationResult).EntryType)
                {
                    case FileSystemEntryType.Directory:
                        return String.Empty;
                    default:
                        // take the file name part
                        String fileName = path.Substring(path.LastIndexOf(DirectorySeparator));
                        return fileName.Substring(0, fileName.LastIndexOf('.'));
                }
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                        case "FileNotFoundException":
                        case "NotFoundException":
                        case "SecurityException":
                            throw new ArgumentException(MessagePathNotExists, "path");
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Returns the file name without file extension for a specified path asynchronously.
        /// </summary>
        /// <param name="path">The path of a file.</param>
        /// <returns>The file name without file extension for <paramref name="path" />.</returns>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
        /// or
        /// The path does not exist.
        /// </exception>
        /// <exception cref="System.NotSupportedException">The operation is not supported by the file system.</exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public async override Task<String> GetFileNameWithoutExtensionAsync(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                HadoopFileSystemOperationResult result = await GetFileEntryStatusAsync(path);

                // the path exists, take the file part
                switch ((result as HadoopFileStatusOperationResult).EntryType)
                {
                    case FileSystemEntryType.Directory:
                        return String.Empty;
                    default:
                        // take the file name part
                        String fileName = path.Substring(path.LastIndexOf(DirectorySeparator));
                        return fileName.Substring(0, fileName.LastIndexOf('.'));
                }
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                        case "FileNotFoundException":
                        case "NotFoundException":
                        case "SecurityException":
                            throw new ArgumentException(MessagePathNotExists, "path");
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Returns the extension for a specified path.
        /// </summary>
        /// <param name="path">The path of a file.</param>
        /// <returns>The extension for <paramref name="path" />.</returns>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
        /// or
        /// The path does not exist.
        /// </exception>
        /// <exception cref="System.NotSupportedException">The operation is not supported by the file system.</exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public override String GetExtension(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                HadoopFileSystemOperationResult result = GetFileEntryStatusAsync(path).Result;

                // the path exists, take the file part
                switch ((result as HadoopFileStatusOperationResult).EntryType)
                {
                    case FileSystemEntryType.Directory:
                        return String.Empty;
                    default:
                        // take the file name part
                        String fileName = path.Substring(path.LastIndexOf(DirectorySeparator));
                        return fileName.Substring(fileName.LastIndexOf('.') + 1);
                }
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                        case "FileNotFoundException":
                        case "NotFoundException":
                        case "SecurityException":
                            throw new ArgumentException(MessagePathNotExists, "path");
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Returns the extension for a specified path.
        /// </summary>
        /// <param name="path">The path of a file.</param>
        /// <returns>The extension for <paramref name="path" />.</returns>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
        /// or
        /// The path does not exist.
        /// </exception>
        /// <exception cref="System.NotSupportedException">The operation is not supported by the file system.</exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public async override Task<String> GetExtensionAsync(String path)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                HadoopFileSystemOperationResult result = await GetFileEntryStatusAsync(path);

                // the path exists, take the file part
                switch ((result as HadoopFileStatusOperationResult).EntryType)
                {
                    case FileSystemEntryType.Directory:
                        return String.Empty;
                    default:
                        // take the file name part
                        String fileName = path.Substring(path.LastIndexOf(DirectorySeparator));
                        return fileName.Substring(fileName.LastIndexOf('.') + 1);
                }
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                        case "FileNotFoundException":
                        case "NotFoundException":
                        case "SecurityException":
                            throw new ArgumentException(MessagePathNotExists, "path");
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Returns the names of the root directories of the file system.
        /// </summary>
        /// <returns>The array containing the root directories in the file system.</returns>
        public override String[] GetRootDirectories()
        {
            return new String[] { DirectorySeparator.ToString() };
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
        /// The path does not exist.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for the path.</exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public override String[] GetDirectories(String path, String searchPattern, Boolean recursive)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                List<HadoopFileStatusOperationResult> result = GetFileEntryStatusListAsync(path, searchPattern, recursive).Result;

                // filter, convert and sort the result
                return result.Where(fileStatusResult => fileStatusResult.EntryType == FileSystemEntryType.Directory).
                              Select(fileStatusResult => fileStatusResult.Path).
                              OrderBy(resultPath => resultPath).ToArray();
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                            throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
                        case "FileNotFoundException":
                        case "NotFoundException":
                            throw new ArgumentException(MessagePathNotExists, "path", ex);
                        case "SecurityException":
                            throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }
                // handle the search pattern match exception
                else if (ex.InnerException is ArgumentException)
                {
                    throw ex.InnerException;
                }
                
                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Returns the directories located on the specified path asynchronously.
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
        /// The path does not exist.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for the path.</exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public async override Task<String[]> GetDirectoriesAsync(String path, String searchPattern, Boolean recursive)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                List<HadoopFileStatusOperationResult> result = await GetFileEntryStatusListAsync(path, searchPattern, recursive);

                // filter, convert and sort the result
                return result.Where(fileStatusResult => fileStatusResult.EntryType == FileSystemEntryType.Directory).
                              Select(fileStatusResult => fileStatusResult.Path).
                              OrderBy(resultPath => resultPath).ToArray();
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                            throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
                        case "FileNotFoundException":
                        case "NotFoundException":
                            throw new ArgumentException(MessagePathNotExists, "path", ex);
                        case "SecurityException":
                            throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }
                // handle the search pattern match exception
                else if (ex.InnerException is ArgumentException)
                {
                    throw ex.InnerException;
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
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
        /// The path does not exist.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for the path.</exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public override String[] GetFiles(String path, String searchPattern, Boolean recursive)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                List<HadoopFileStatusOperationResult> result = GetFileEntryStatusListAsync(path, searchPattern, recursive).Result;

                // filter, convert and sort the result
                return result.Where(fileStatusResult => fileStatusResult.EntryType == FileSystemEntryType.File).
                              Select(fileStatusResult => fileStatusResult.Path).
                              OrderBy(resultPath => resultPath).ToArray();
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                            throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
                        case "FileNotFoundException":
                        case "NotFoundException":
                            throw new ArgumentException(MessagePathNotExists, "path", ex);
                        case "SecurityException":
                            throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }
                // handle the search pattern match exception
                else if (ex.InnerException is ArgumentException)
                {
                    throw ex.InnerException;
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Returns the files located on the specified path asynchronously.
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
        /// The path does not exist.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for the path.</exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public async override Task<String[]> GetFilesAsync(String path, String searchPattern, Boolean recursive)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                List<HadoopFileStatusOperationResult> result = await GetFileEntryStatusListAsync(path, searchPattern, recursive);

                // filter, convert and sort the result
                return result.Where(fileStatusResult => fileStatusResult.EntryType == FileSystemEntryType.File).
                              Select(fileStatusResult => fileStatusResult.Path).
                              OrderBy(resultPath => resultPath).ToArray();
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                            throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
                        case "FileNotFoundException":
                        case "NotFoundException":
                            throw new ArgumentException(MessagePathNotExists, "path", ex);
                        case "SecurityException":
                            throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }
                // handle the search pattern match exception
                else if (ex.InnerException is ArgumentException)
                {
                    throw ex.InnerException;
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Returns the file system entries located on the specified path.
        /// </summary>
        /// <param name="path">The path of the directory to search.</param>
        /// <param name="searchPattern">The search string to match against the names of files in path.</param>
        /// <param name="recursive">A value that specifies whether subdirectories are included in the search.</param>
        /// <returns>An array containing the file system entry informations.</returns>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
        /// or
        /// The path does not exist.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for the path.</exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public override FileSystemEntry[] GetFileSystemEntries(String path, String searchPattern, Boolean recursive)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                List<HadoopFileStatusOperationResult> result = GetFileEntryStatusListAsync(path, searchPattern, recursive).Result;

                // convert and sort the result
                return result.Select(fileStatusResult => new FileSystemEntry { 
                    Path = fileStatusResult.Path, 
                    Name = fileStatusResult.Name, 
                    Type = fileStatusResult.EntryType, 
                    CreationTime = DateTime.MinValue, 
                    LastAccessTime = fileStatusResult.AccessTime, 
                    LastModificationTime = fileStatusResult.ModificationTime, 
                    Length = fileStatusResult.Length 
                }).OrderBy(entry => entry.Path).ToArray();
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                            throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
                        case "FileNotFoundException":
                        case "NotFoundException":
                            throw new ArgumentException(MessagePathNotExists, "path", ex);
                        case "SecurityException":
                            throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }
                // handle the search pattern match exception
                else if (ex.InnerException is ArgumentException)
                {
                    throw ex.InnerException;
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        /// <summary>
        /// Returns the file system entries located on the specified path asynchronously.
        /// </summary>
        /// <param name="path">The path of the directory to search.</param>
        /// <param name="searchPattern">The search string to match against the names of files in path.</param>
        /// <param name="recursive">A value that specifies whether subdirectories are included in the search.</param>
        /// <returns>An array containing the file system entry informations.</returns>
        /// <exception cref="System.ArgumentNullException">The path is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The path is empty, or consists only of whitespace characters.
        /// or
        /// The path is in an invalid format.
        /// or
        /// The path does not exist.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission for the path.</exception>
        /// <exception cref="ConnectionException">
        /// No connection is available to the specified path.
        /// or
        /// No connection is available to the file system.
        /// </exception>
        public async override Task<FileSystemEntry[]> GetFileSystemEntriesAsync(String path, String searchPattern, Boolean recursive)
        {
            if (path == null)
                throw new ArgumentNullException("path", MessagePathIsNull);
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException(MessagePathIsEmpty, "path");

            try
            {
                List<HadoopFileStatusOperationResult> result = await GetFileEntryStatusListAsync(path, searchPattern, recursive);

                // convert and sort the result
                return result.Select(fileStatusResult => new FileSystemEntry
                {
                    Path = fileStatusResult.Path,
                    Name = fileStatusResult.Name,
                    Type = fileStatusResult.EntryType,
                    CreationTime = DateTime.MinValue,
                    LastAccessTime = fileStatusResult.AccessTime,
                    LastModificationTime = fileStatusResult.ModificationTime,
                    Length = fileStatusResult.Length
                }).OrderBy(entry => entry.Path).ToArray();
            }
            catch (AggregateException ex)
            {
                // handle remote file system exceptions
                if (ex.InnerException is HadoopRemoteException)
                {
                    switch ((ex.InnerException as HadoopRemoteException).ExceptionName)
                    {
                        case "IllegalArgumentException":
                            throw new ArgumentException(MessagePathInvalidFormat, "path", ex);
                        case "FileNotFoundException":
                        case "NotFoundException":
                            throw new ArgumentException(MessagePathNotExists, "path", ex);
                        case "SecurityException":
                            throw new UnauthorizedAccessException(MessagePathUnauthorized, ex);
                        case "IOException":
                            throw new ConnectionException(MessageNoConnectionToPath, path, ex);
                    }
                }
                // handle the search pattern match exception
                else if (ex.InnerException is ArgumentException)
                {
                    throw ex.InnerException;
                }

                // handle unexpected exceptions
                throw new ConnectionException(MessageNoConnectionToFileSystem, ex.InnerException);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Returns the file status for the specified path asynchronously.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The file status for the specified path.</returns>
        private async Task<HadoopFileStatusOperationResult> GetFileEntryStatusAsync(String path)
        {
            HadoopFileSystemOperation operation;

            // create operation
            if (_client != null)
                operation = new HadoopFileStatusOperation(_client, Location.ToString() + RootPath + path, Authentication as IHadoopFileSystemAuthentication);
            else
                operation = new HadoopFileStatusOperation(Location.ToString() + RootPath + path, Authentication as IHadoopFileSystemAuthentication);

            // execute operation
            return (await operation.ExecuteAsync()) as HadoopFileStatusOperationResult;
        }

        /// <summary>
        ///  Returns the file status list for the specified path asynchronously.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The file status list for the specified path.</returns>
        private async Task<List<HadoopFileStatusOperationResult>> GetFileEntryStatusListAsync(String path, String searchPattern, Boolean recursive)
        {
            HadoopFileSystemOperation operation;

            // create operation
            if (_client != null)
                operation = new HadoopFileListingOperation(_client, Location.ToString() + RootPath + path, Authentication as IHadoopFileSystemAuthentication);
            else
                operation = new HadoopFileListingOperation(Location.ToString() + RootPath + path, Authentication as IHadoopFileSystemAuthentication);

            // execute operation
            HadoopFileListingOperationResult result = (await operation.ExecuteAsync()) as HadoopFileListingOperationResult;

            // correct the path
            if (path.Last() != DirectorySeparator)
                path += DirectorySeparator;

            // filter the result
            List<HadoopFileStatusOperationResult> statusList = (result as HadoopFileListingOperationResult).StatusList.
                                                                   Select(status => { status.Path = path + status.Name; return status; }).
                                                                   Where(status => IsMatch(status.Path, searchPattern)).ToList();

            // in case of recursive listing, all subdirectories need to be listed
            if (recursive)
            {
                // enqueue the starting directories
                Queue<String> directories = new Queue<String>((result as HadoopFileListingOperationResult).StatusList.
                                                                  Where(status => status.EntryType == FileSystemEntryType.Directory).
                                                                  Select(status => status.Name));

                while (directories.Count > 0)
                {
                    String directoryPath = directories.Dequeue();


                    // create operation for the inner directory
                    if (_client != null)
                        operation = new HadoopFileListingOperation(_client, Location.ToString() + RootPath + path + directoryPath, Authentication as IHadoopFileSystemAuthentication);
                    else
                        operation = new HadoopFileListingOperation(Location.ToString() + RootPath + path + directoryPath, Authentication as IHadoopFileSystemAuthentication);

                    try
                    {
                        // execute operation for inner directory
                        result = (await operation.ExecuteAsync()) as HadoopFileListingOperationResult;
                    }
                    catch (HadoopRemoteException)
                    {
                        // in case of any error, the reading should continue with the next directory
                        continue;
                    }

                    // filter the result
                    statusList.AddRange((result as HadoopFileListingOperationResult).StatusList.
                                            Select(status => { status.Path = path + directoryPath + DirectorySeparator + status.Name; return status; }).
                                            Where(status => IsMatch(directoryPath + DirectorySeparator + status.Name, searchPattern)));

                    // add inner directories to the queue
                    foreach (String innerDirectory in (result as HadoopFileListingOperationResult).StatusList.
                                                          Where(status => status.EntryType == FileSystemEntryType.Directory).
                                                          Select(status => path + directoryPath + DirectorySeparator + status.Name))
                        directories.Enqueue(innerDirectory);
                }
            }

            // filter and sort the result
            return statusList;
        }

        #endregion

        #region Private static methods

        /// <summary>
        /// Creates the location from the specified hostname and port number.
        /// </summary>
        /// <param name="hostname">The hostname.</param>
        /// <param name="portNumber">The port number.</param>
        /// <returns>The location of the file system.</returns>
        /// <exception cref="System.ArgumentNullException">The hostname is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The hostname is empty, or consists only of whitespace characters.
        /// or
        /// The hostname is in an invalid format.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The port number is less than 1.</exception>
        private static Uri CreateLocation(String hostname, Int32 portNumber)
        {
            if (hostname == null)
                throw new ArgumentNullException("hostname", "The hostname is null.");
            if (String.IsNullOrWhiteSpace(hostname))
                throw new ArgumentException("The hostname is empty, or consists only of whitespace characters.", "hostname");
            if (portNumber < 1)
                throw new ArgumentOutOfRangeException("portNumber", "The port number is less than 1.");

            if (Uri.CheckHostName(hostname) == UriHostNameType.Unknown)
                throw new ArgumentException("The hostname is in an invalid format.", "hostname");

            return new Uri(String.Format("http://{0}:{1}", hostname, portNumber));
        }

        /// <summary>
        /// Determines whether the specified name matches the pattern.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="pattern">The search pattern.</param>
        /// <returns><c>true</c> if the specified path matches the pattern; otherwise <c>false</c>.</returns>
        /// <exception cref="System.ArgumentException">The search pattern is an invalid format.</exception>
        private static Boolean IsMatch(String path, String pattern)
        {
            if (String.IsNullOrWhiteSpace(pattern))
                throw new ArgumentException(MessageInvalidSearchPattern, "searchPattern");

            pattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";

            try
            {
                return Regex.IsMatch(path, pattern, RegexOptions.CultureInvariant);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(MessageInvalidSearchPattern, "searchPattern", ex);
            }
        }

        #endregion
    }
}
