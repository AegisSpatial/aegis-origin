﻿/// <copyright file="ReferenceSystem.cs" company="Eötvös Loránd University (ELTE)">
///     Copyright (c) 2011-2014 Roberto Giachetta. Licensed under the
///     Educational Community License, Version 2.0 (the "License"); you may
///     not use this file except in compliance with the License. You may
///     obtain a copy of the License at
///     http://www.osedu.org/licenses/ECL-2.0
///
///     Unless required by applicable law or agreed to in writing,
///     software distributed under the License is distributed on an "AS IS"
///     BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
///     or implied. See the License for the specific language governing
///     permissions and limitations under the License.
/// </copyright>
/// <author>Roberto Giachetta</author>

using System;

namespace ELTE.AEGIS.Reference
{
    /// <summary>
    /// Represents a general reference system.
    /// </summary>
    /// <remarks>
    /// A reference system contains the metadata required to interpret spatial location or temporal position information unambiguously.
    /// </remarks>
    public abstract class ReferenceSystem : IdentifiedObject, IReferenceSystem
    {
        #region Private fields

        private readonly String _scope;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the dimension of the reference system.
        /// </summary>
        /// <value>The dimension of the reference system.</value>
        public abstract Int32 Dimension { get; }

        /// <summary>
        /// Gets the scope of the reference system.
        /// </summary>
        /// <value>Description of usage, or limitations of usage, for whichh this reference system is valid.</value>
        public String Scope { get { return _scope; } }

        /// <summary>
        /// Gets the type of the reference system.
        /// </summary>
        /// <value>The type of the reference system.</value>
        public abstract ReferenceSystemType Type { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceSystem" /> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="remarks">The remarks.</param>
        /// <param name="aliases">The aliases.</param>
        /// <param name="scope">The scope.</param>
        /// <exception cref="System.ArgumentNullException">The identifier is null.</exception>
        protected ReferenceSystem(String identifier, String name, String remarks, String[] aliases, String scope) 
            : base(identifier, name, remarks, aliases) 
        {
            _scope = scope ?? String.Empty;
        }

        #endregion
    }
}
