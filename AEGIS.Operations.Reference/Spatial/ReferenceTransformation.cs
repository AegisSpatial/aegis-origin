﻿/// <copyright file="ReferenceTransformation.cs" company="Eötvös Loránd University (ELTE)">
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

using ELTE.AEGIS.Operations.Management;
using ELTE.AEGIS.Operations.Spatial.Strategy;
using ELTE.AEGIS.Reference;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ELTE.AEGIS.Operations.Spatial
{
    /// <summary>
    /// Represents a reference system transformation.
    /// </summary>
    [OperationClass("AEGIS::212901", "Reference system transformation", "1.0.0", typeof(ReferenceTransformationCertificate))]
    public class ReferenceTransformation : Operation<IGeometry, IGeometry>
    {
        #region Private fields

        private readonly IReferenceSystem _targetReferenceSystem;
        private readonly Boolean _metadataPreservation;
        private IGeometryFactory _factory;
        private ITransformationStrategy _transformation;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceTransformation" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="parameters">The parameters.</param>
        public ReferenceTransformation(IGeometry source, IDictionary<OperationParameter, Object> parameters)
            : base(source, null, ReferenceOperationMethods.ReferenceTransformation, parameters)
        {
            _targetReferenceSystem = GetParameter<IReferenceSystem>(ReferenceOperationParameters.TargetReferenceSystem);
            _metadataPreservation = GetParameter<Boolean>(OperationParameters.MetadataPreservation);
            _factory = GetParameter<IGeometryFactory>(OperationParameters.GeometryFactory);
        }

        #endregion

        #region Protected Operation methods

        /// <summary>
        /// Prepares the result of the operation.
        /// </summary>
        protected override void PrepareResult()
        {
            if (_source.ReferenceSystem != null && _targetReferenceSystem != null && !_source.ReferenceSystem.Equals(_targetReferenceSystem))
            {
                // strategy pattern
                _transformation = TransformationStrategyFactory.CreateStrategy(_source.ReferenceSystem as ReferenceSystem, _targetReferenceSystem as ReferenceSystem);
            }

            if (_factory == null)
                _factory = (IGeometryFactory)Factory.GetInstance(_source.Factory, _targetReferenceSystem);
        }

        /// <summary>
        /// Computes the result.
        /// </summary>
        protected override void ComputeResult()
        {
            _result = Compute(_source);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Computes the transformation of the source geometry.
        /// </summary>
        /// <param name="source">The source geometry.</param>
        /// <returns>The geometry in the specified reference system.</returns>
        /// <exception cref="System.InvalidOperationException">Transformation of the specified geometry is not supported.</exception>
        private IGeometry Compute(IGeometry source)
        {
            if (source == null)
                return null;

            if (source.ReferenceSystem == null || source.ReferenceSystem.Equals(_targetReferenceSystem))
                return source;

            if (source is IPoint)
                return Compute(source as IPoint);
            if (source is ILine)
                return Compute(source as ILine);
            if (source is ILinearRing)
                return Compute(source as ILinearRing);
            if (source is ILineString)
                return Compute(source as ILineString);
            if (source is IPolygon)
                return Compute(source as IPolygon);
            if (source is IMultiPoint)
                return Compute(source as IMultiPoint);
            if (source is IMultiLineString)
                return Compute(source as IMultiLineString);
            if (source is IMultiPolygon)
                return Compute(source as IMultiPolygon);
            if (source is IGeometryCollection<IGeometry>)
                return Compute(source as IGeometryCollection<IGeometry>);

            throw new InvalidOperationException("Transformation of the specified geometry is not supported.");
        }

        /// <summary>
        /// Computes the transformation of the source geometry.
        /// </summary>
        /// <param name="source">The source geometry.</param>
        /// <returns>The geometry in the specified reference system.</returns>
        private IPoint Compute(IPoint source)
        {
            return _factory.CreatePoint(Compute(source.Coordinate), _metadataPreservation ? source.Metadata : null);
        }

        /// <summary>
        /// Computes the transformation of the source geometry.
        /// </summary>
        /// <param name="source">The source geometry.</param>
        /// <returns>The geometry in the specified reference system.</returns>
        private ILine Compute(ILine source)
        {
            return _factory.CreateLine(Compute(source.StartCoordinate), Compute(source.EndCoordinate), _metadataPreservation ? source.Metadata : null);
        }

        /// <summary>
        /// Computes the transformation of the source geometry.
        /// </summary>
        /// <param name="source">The source geometry.</param>
        /// <returns>The geometry in the specified reference system.</returns>
        private ILinearRing Compute(ILinearRing source)
        {
            return _factory.CreateLinearRing(source.Coordinates.Select(coordinate => Compute(coordinate)), _metadataPreservation ? source.Metadata : null);
        }

        /// <summary>
        /// Computes the transformation of the source geometry.
        /// </summary>
        /// <param name="source">The source geometry.</param>
        /// <returns>The geometry in the specified reference system.</returns>
        private ILineString Compute(ILineString source)
        {
            return _factory.CreateLineString(source.Coordinates.Select(coordinate => Compute(coordinate)), _metadataPreservation ? source.Metadata : null);
        }

        /// <summary>
        /// Computes the transformation of the source geometry.
        /// </summary>
        /// <param name="source">The source geometry.</param>
        /// <returns>The geometry in the specified reference system.</returns>
        private IPolygon Compute(IPolygon source)
        {
            return _factory.CreatePolygon(Compute(source.Shell),
                                          source.HoleCount > 0 ? source.Holes.Select(hole => Compute(hole)) : null,
                                          _metadataPreservation ? source.Metadata : null);
        }

        /// <summary>
        /// Computes the transformation of the source geometry.
        /// </summary>
        /// <param name="source">The source geometry.</param>
        /// <returns>The geometry in the specified reference system.</returns>
        private IMultiPoint Compute(IMultiPoint source)
        {
            return _factory.CreateMultiPoint(source.Select(item => Compute(item)),
                                             _metadataPreservation ? source.Metadata : null);
        }

        /// <summary>
        /// Computes the transformation of the source geometry.
        /// </summary>
        /// <param name="source">The source geometry.</param>
        /// <returns>The geometry in the specified reference system.</returns>
        private IMultiLineString Compute(IMultiLineString source)
        {
            return _factory.CreateMultiLineString(source.Select(item => Compute(item)),
                                                  _metadataPreservation ? source.Metadata : null);
        }

        /// <summary>
        /// Computes the transformation of the source geometry.
        /// </summary>
        /// <param name="source">The source geometry.</param>
        /// <returns>The geometry in the specified reference system.</returns>
        private IMultiPolygon Compute(IMultiPolygon source)
        {
            return _factory.CreateMultiPolygon(source.Select(item => Compute(item)),
                                               _metadataPreservation ? source.Metadata : null);
        }

        /// <summary>
        /// Computes the transformation of the source geometry.
        /// </summary>
        /// <param name="source">The source geometry.</param>
        /// <returns>The geometry in the specified reference system.</returns>
        private IGeometryCollection<IGeometry> Compute(IGeometryCollection<IGeometry> source)
        {
            return _factory.CreateGeometryCollection(source.Select(item => Compute(item)),
                                                     _metadataPreservation ? source.Metadata : null);
        }
       
        /// <summary>
        /// Computes the transformation of the coordinate.
        /// </summary>
        /// <param name="source">The source coordinate.</param>
        /// <returns>The coordinate in the specified reference system.</returns>
        private Coordinate Compute(Coordinate coordinate)
        {
            return _transformation.Transform(coordinate);
        }

        #endregion
    }
}
