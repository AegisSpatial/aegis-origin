﻿// <copyright file="IIdentifierProvider.cs" company="Eötvös Loránd University (ELTE)">
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
using System.Collections.Generic;

namespace ELTE.AEGIS.Topology
{
    /// <summary>
    /// Defines a provider for geometry identifiers.
    /// </summary>
    /// <author>Máté Cserép</author>
    public interface IIdentifierProvider
    {
        /// <summary>
        /// Retrieves the identifier from an <see cref="IGeometry"/>.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns>The identifier.</returns>
        ISet<Int32> GetIdentifiers(IGeometry geometry);
    }
}
