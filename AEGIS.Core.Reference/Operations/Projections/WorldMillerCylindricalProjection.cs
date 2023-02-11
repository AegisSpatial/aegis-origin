﻿// <copyright file="WorldMillerCylindricalProjection.cs" company="Eötvös Loránd University (ELTE)">
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

using ELTE.AEGIS.Management;
using ELTE.AEGIS.Numerics;
using System;
using System.Collections.Generic;

namespace ELTE.AEGIS.Reference.Operations.Projections
{
    /// <summary>
    /// Represents a Miller Cylindrical Projection.
    /// </summary>
    /// <author>Péter Rónai</author>
    [CoordinateOperationMethodImplementation("ESRI::54002", "Miller Cylindrical Projection")]
    public class WorldMillerCylindricalProjection : CoordinateProjection
    {
        #region Private fields

        private readonly Double _falseEasting;
        private readonly Double _falseNorthing;
        private readonly Double _longitudeOfNaturalOrigin;
        private readonly Double _sphereRadius;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldMillerCylindricalProjection"/> class.
        /// </summary>
        /// <param name="identifier">The identifier of the operation.</param>
        /// <param name="name">The name of the operation.</param>
        /// <param name="parameters">The parameters of the operation.</param>
        /// <param name="ellipsoid">The ellipsoid.</param>
        /// <param name="areaOfUse">The area of use where the operation is applicable.</param>
        /// <exception cref="System.ArgumentNullException">
        /// The defined operation method requires parameters.
        /// or
        /// The ellipsoid is null.
        /// or
        /// The area of use is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The parameters do not contain a required parameter value.
        /// or
        /// The parameter is not an angular value as required by the method.
        /// or
        /// The parameter is not a length value as required by the method.
        /// or
        /// The parameter is not a double precision floating-point number as required by the method.
        /// or
        /// The parameter does not have the same measurement unit as the ellipsoid.
        /// </exception>
        public WorldMillerCylindricalProjection(String identifier, String name, IDictionary<CoordinateOperationParameter, Object> parameters, Ellipsoid ellipsoid, AreaOfUse areaOfUse) :
            base(identifier, name, CoordinateOperationMethods.WorldMillerCylindricalProjection, parameters, ellipsoid, areaOfUse)
        {
            _falseNorthing = ((Length)_parameters[CoordinateOperationParameters.FalseNorthing]).Value;
            _falseEasting = ((Length)_parameters[CoordinateOperationParameters.FalseEasting]).Value;
            _longitudeOfNaturalOrigin = ((Angle)_parameters[CoordinateOperationParameters.LongitudeOfNaturalOrigin]).BaseValue;
            _sphereRadius = ellipsoid.SemiMajorAxis.BaseValue;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Computes the forward transformation.
        /// </summary>
        /// <param name="coordinate">The coordinate.</param>
        /// <returns>The transformed coordinate.</returns>
        protected override Coordinate ComputeForward(GeoCoordinate coordinate)
        {
            Double latitude = coordinate.Latitude.BaseValue;
            Double longitude = coordinate.Longitude.BaseValue;
            Double x = _sphereRadius * ComputeLongitudeDelta(longitude);
            Double y = _sphereRadius * (Calculator.Asinh(Math.Tan(0.8 * latitude))) / 0.8;
            return new Coordinate(_falseEasting + x, _falseNorthing + y);
        }

        /// <summary>
        /// Computes the reverse transformation.
        /// </summary>
        /// <param name="coordinate">The coordinate.</param>
        /// <returns>The transformed coordinate.</returns>
        protected override GeoCoordinate ComputeReverse(Coordinate coordinate)
        {
            Double x = coordinate.X - _falseEasting;
            Double y = coordinate.Y - _falseNorthing;
            Double latitude = Math.Atan(Math.Sinh(0.8 * y / _sphereRadius)) / 0.8;
            Double longitude = _longitudeOfNaturalOrigin + x / _sphereRadius;
            return new GeoCoordinate(latitude, longitude);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Computes the longitude delta.
        /// </summary>
        /// <param name="longitude">The longitude.</param>
        /// <returns>The longitude delta.</returns>
        private Double ComputeLongitudeDelta(Double longitude)
        {
            Angle threshold = Angle.FromDegree(180);
            Angle diff = Angle.FromRadian(longitude) - Angle.FromRadian(_longitudeOfNaturalOrigin);
            Angle correction = Angle.FromDegree(diff > threshold ? -360 : (diff < -threshold ? 360 : 0));
            return (diff + correction).BaseValue;
        }

        #endregion
    }
}
