// <copyright file="ChangesetType.cs" company="E�tv�s Lor�nd University (ELTE)">
//     Copyright (c) 2011-2023 Roberto Giachetta. Licensed under the
//     Educational Community License, Version 2.0 (the "License"); you may
//     not use this file except in compliance with the License. You may
//     obtain a copy of the License at
//     http://opensource.org/licenses/ECL-2.0
// 
//     Unless required by applicable law or agreed to in writing,
//     software distributed under the License is distributed on an "AS IS"
//     BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
//     or implied. See the License for the specific language governing
//     permissions and limitations under the License.
// </copyright>

using System;

namespace ELTE.AEGIS.Versioning
{
    /// <summary>
    /// Defines the type of a changeset.
    /// </summary>
    /// <author>M�t� Cser�p</author>
    [Flags]
    public enum ChangesetType
    {
        /// <summary>
        /// Supports forward change tracking.
        /// </summary>
        Forward = 1,

        /// <summary>
        /// Supports reverse change tracking.
        /// </summary>
        Reverse = 2,

        /// <summary>
        /// Supports both forward and reverse change tracking.
        /// </summary>
        Dual    = 3
    }
}