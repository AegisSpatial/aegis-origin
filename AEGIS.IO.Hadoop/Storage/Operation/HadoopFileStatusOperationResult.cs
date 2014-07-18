﻿/// <copyright file="HadoopFileStatusOperationResult.cs" company="Eötvös Loránd University (ELTE)">
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

using System;

namespace ELTE.AEGIS.IO.Storage.Operation
{
    /// <summary>
    /// Represents a Hadoop file system operation result containing a file status.
    /// </summary>
    public class HadoopFileStatusOperationResult : HadoopFileSystemOperationResult
    {
        #region Public properties

        /// <summary>
        /// Gets the path of the entry.
        /// </summary>
        /// <value>The full path of the entry.</value>

        public String Path { get; set; }

        /// <summary>
        /// Gets the name of the entry.
        /// </summary>
        /// <value>The name of the entry.</value>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the time of the last access.
        /// </summary>
        /// <value>The time of the last access.</value>
        public DateTime AccessTime { get; set; }

        /// <summary>
        /// Gets or sets the time of the last modification.
        /// </summary>
        /// <value>The time of the last modification.</value>
        public DateTime ModificationTime { get; set; }

        /// <summary>
        /// Gets or sets the length of the file.
        /// </summary>
        /// <value>The length of the file in bytes.</value>
        public Int64 Length { get; set; }

        /// <summary>
        /// Gets or sets the size of the blocks.
        /// </summary>
        /// <value>The size of the blocks in bytes.</value>
        public Int64 BlockSize { get; set; }

        /// <summary>
        /// Gets or sets the type of the entry.
        /// </summary>
        /// <value>The type of the entry.</value>
        public FileSystemEntryType EntryType { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HadoopFileStatusOperationResult"/> class.
        /// </summary>
        public HadoopFileStatusOperationResult() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HadoopBooleanOperationResult" /> class.
        /// </summary>
        /// <param name="request">The request of the operation.</param>
        /// <param name="path">The path of the entry.</param>
        /// <param name="name">The name of the entry.</param>
        /// <param name="accessTime">The time of the last access.</param>
        /// <param name="modificationTime">The time of the last modification.</param>
        /// <param name="length">The length of the file.</param>
        /// <param name="blockSize">The size of the block.</param>
        /// <param name="entryType">the type of the entry.</param>
        public HadoopFileStatusOperationResult(String request, String path, String name, DateTime accessTime, DateTime modificationTime, Int64 length, Int64 blockSize, FileSystemEntryType entryType)
            : base(request) 
        {
            Path = path;
            Name = name;
            AccessTime = accessTime;
            ModificationTime = modificationTime;
            Length = length;
            BlockSize = blockSize;
            EntryType = entryType;
        }

        #endregion
    }
}
