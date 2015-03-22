﻿/// <copyright file="IGeometryFactory.cs" company="Eötvös Loránd University (ELTE)">
///     Copyright (c) 2011-2015 Roberto Giachetta. Licensed under the
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

using ELTE.AEGIS.Geometry;
using System;
using System.Collections.Generic;

namespace ELTE.AEGIS
{
    /// <summary>
    /// Defines behavior for factories producing <see cref="IGeometry" /> instances.
    /// </summary>
    [FactoryContract(Product = typeof(IGeometry), DefaultBehavior = typeof(GeometryFactory))]
    public interface IGeometryFactory : IFactory
    {
        #region Properties

        /// <summary>
        /// Gets the precision model used by the factory.
        /// </summary>
        /// <value>The precision model used by the factory.</value>
        PrecisionModel PrecisionModel { get; }

        /// <summary>
        /// Gets the reference system used by the factory.
        /// </summary>
        /// <value>The reference system used by the factory.</value>
        IReferenceSystem ReferenceSystem { get; }

        #endregion

        #region Factory methods for points

        /// <summary>
        /// Creates a point.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The point with the specified X, Y coordinates.</returns>
        IPoint CreatePoint(Double x, Double y);

        /// <summary>
        /// Creates a point.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The point with the specified X, Y coordinates and metadata.</returns>
        IPoint CreatePoint(Double x, Double y, IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a point.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        /// <returns>The point with the specified X, Y, Z coordinates.</returns>
        IPoint CreatePoint(Double x, Double y, Double z);

        /// <summary>
        /// Creates a point.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The point with the specified X, Y, Z coordinates and metadata.</returns>
        IPoint CreatePoint(Double x, Double y, Double z, IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a point.
        /// </summary>
        /// <param name="coordinate">The coordinate of the point.</param>
        /// <returns>The point with the specified coordinate.</returns>
        IPoint CreatePoint(Coordinate coordinate);

        /// <summary>
        /// Creates a point.
        /// </summary>
        /// <param name="coordinate">The coordinate of the point.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The point with the specified coordinate and metadata.</returns>
        IPoint CreatePoint(Coordinate coordinate, IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a point.
        /// </summary>
        /// <param name="other">The other point.</param>
        /// <returns>A point that matches <paramref name="other" />.</returns>
        /// <exception cref="System.ArgumentNullException">The other point is null.</exception>
        IPoint CreatePoint(IPoint other);

        #endregion

        #region Factory methods for line strings

        /// <summary>
        /// Creates a line string.
        /// </summary>
        /// <returns>An empty line string.</returns>
        ILineString CreateLineString();

        /// <summary>
        /// Creates a line string.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>An empty line string containing the specified metadata.</returns>
        ILineString CreateLineString(IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a line string.
        /// </summary>
        /// <param name="source">The source coordinates.</param>
        /// <returns>A line string containing the specified coordinates.</returns>
        ILineString CreateLineString(params Coordinate[] source);

        /// <summary>
        /// Creates a line string.
        /// </summary>
        /// <param name="source">The source points.</param>
        /// <returns>A line string containing the specified points.</returns>
        ILineString CreateLineString(params IPoint[] source);

        /// <summary>
        /// Creates a line string.
        /// </summary>
        /// <param name="source">The source coordinates.</param>
        /// <returns>A line string containing the specified coordinates.</returns>
        ILineString CreateLineString(IEnumerable<Coordinate> source);

        /// <summary>
        /// Creates a line string.
        /// </summary>
        /// <param name="source">The source coordinates.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>A line string containing the specified coordinates and metadata.</returns>
        ILineString CreateLineString(IEnumerable<Coordinate> source, IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a line string.
        /// </summary>
        /// <param name="source">The source points.</param>
        /// <returns>A line string containing the specified points.</returns>
        ILineString CreateLineString(IEnumerable<IPoint> source);

        /// <summary>
        /// Creates a line string.
        /// </summary>
        /// <param name="source">The points.</param>
        /// <returns>A line string containing the specified points and metadata.</returns>
        ILineString CreateLineString(IEnumerable<IPoint> source, IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a line string.
        /// </summary>
        /// <param name="source">The other line string.</param>
        /// <returns>A line string that matches <paramref name="source" />.</returns>
        /// <exception cref="System.ArgumentNullException">The other line string is null.</exception>
        ILineString CreateLineString(ILineString source);

        #endregion

        #region Factory methods for lines

        /// <summary>
        /// Creates a line.
        /// </summary>
        /// <param name="start">The startint coordinate.</param>
        /// <param name="end">The ending coordinate.</param>
        /// <returns>A line containing the specified coordinates.</returns>
        ILine CreateLine(Coordinate start, Coordinate end);

        /// <summary>
        /// Creates a line.
        /// </summary>
        /// <param name="start">The startint coordinate.</param>
        /// <param name="end">The ending coordinate.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>A line containing the specified coordinates.</returns>
        ILine CreateLine(Coordinate start, Coordinate end, IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a line.
        /// </summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        /// <returns>A line containing the specified points.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// The start point is null.
        /// or
        /// The end point is null.
        /// </exception>
        ILine CreateLine(IPoint start, IPoint end);

        /// <summary>
        /// Creates a line.
        /// </summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        /// <returns>A line containing the specified points.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// The start point is null.
        /// or
        /// The end point is null.
        /// </exception>
        ILine CreateLine(IPoint start, IPoint end, IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a line.
        /// </summary>
        /// <param name="other">The other line.</param>
        /// <returns>A line that matches <paramref name="other" />.</returns>
        /// <exception cref="System.ArgumentNullException">The other line is null.</exception>
        ILine CreateLine(ILine other);

        #endregion

        #region Factory methods for linear rings

        /// <summary>
        /// Creates a linear ring.
        /// </summary>
        /// <param name="source">The source coordinates.</param>
        /// <returns>A linear ring containing the specified coordinates.</returns>
        /// <exception cref="System.ArgumentNullException">The source is null.</exception>
        /// <exception cref="System.ArgumentException">The source is empty.</exception>
        ILinearRing CreateLinearRing(params Coordinate[] source);

        /// <summary>
        /// Creates a linear ring.
        /// </summary>
        /// <param name="source">The source points.</param>
        /// <returns>A linear ring containing the specified points.</returns>
        /// <exception cref="System.ArgumentNullException">The source is null.</exception>
        /// <exception cref="System.ArgumentException">The source is empty.</exception>
        ILinearRing CreateLinearRing(params IPoint[] source);

        /// <summary>
        /// Creates a linear ring.
        /// </summary>
        /// <param name="source">The source coordinates.</param>
        /// <returns>A linear ring containing the specified coordinates.</returns>
        /// <exception cref="System.ArgumentNullException">The source is null.</exception>
        /// <exception cref="System.ArgumentException">The source is empty.</exception>
        ILinearRing CreateLinearRing(IEnumerable<Coordinate> source);

        /// <summary>
        /// Creates a linear ring.
        /// </summary>
        /// <param name="source">The source coordinates.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>A linear ring containing the specified coordinates and metadata.</returns>
        /// <exception cref="System.ArgumentNullException">The source is null.</exception>
        /// <exception cref="System.ArgumentException">The source is empty.</exception>
        ILinearRing CreateLinearRing(IEnumerable<Coordinate> source, IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a linear ring.
        /// </summary>
        /// <param name="source">The source points.</param>
        /// <returns>A linear ring containing the specified points.</returns>
        /// <exception cref="System.ArgumentNullException">The source is null.</exception>
        /// <exception cref="System.ArgumentException">The source is empty.</exception>
        ILinearRing CreateLinearRing(IEnumerable<IPoint> source);

        /// <summary>
        /// Creates a linear ring.
        /// </summary>
        /// <param name="source">The points.</param>
        /// <returns>A linear ring containing the specified points and metadata.</returns>
        /// <exception cref="System.ArgumentNullException">The source is null.</exception>
        /// <exception cref="System.ArgumentException">The source is empty.</exception>
        ILinearRing CreateLinearRing(IEnumerable<IPoint> source, IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a linear ring.
        /// </summary>
        /// <param name="other">The other linear ring.</param>
        /// <returns>A linear ring that matches <paramref name="other" />.</returns>
        /// <exception cref="System.ArgumentNullException">The other linear ring is null.</exception>
        ILinearRing CreateLinearRing(ILinearRing other);

        #endregion

        #region Factory methods for polygons

        /// <summary>
        /// Creates a polygon.
        /// </summary>
        /// <param name="shell">The coordinates of the shell.</param>
        /// <returns>A polygon containing the specified coordinates.</returns>
        /// <exception cref="System.ArgumentNullException">The shell is null.</exception>
        /// <exception cref="System.ArgumentException">The shell is empty.</exception>
        IPolygon CreatePolygon(params Coordinate[] shell);

        /// <summary>
        /// Creates a polygon.
        /// </summary>
        /// <param name="shell">The points of the shell.</param>
        /// <returns>A polygon containing the specified points.</returns>
        /// <exception cref="System.ArgumentNullException">The shell is null.</exception>
        /// <exception cref="System.ArgumentException">The shell is empty.</exception>
        IPolygon CreatePolygon(params IPoint[] shell);

        /// <summary>
        /// Creates a polygon.
        /// </summary>
        /// <param name="shell">The coordinates of the shell.</param>
        /// <returns>A polygon containing the specified coordinates.</returns>
        /// <exception cref="System.ArgumentNullException">The shell is null.</exception>
        /// <exception cref="System.ArgumentException">The shell is empty.</exception>
        IPolygon CreatePolygon(IEnumerable<Coordinate> shell);

        /// <summary>
        /// Creates a polygon.
        /// </summary>
        /// <param name="shell">The coordinates of the shell.</param>
        /// <param name="holes">The coordinates of the holes.</param>
        /// <returns>A polygon containing the specified coordinates.</returns>
        /// <exception cref="System.ArgumentNullException">The shell is null.</exception>
        /// <exception cref="System.ArgumentException">The shell is empty.</exception>
        IPolygon CreatePolygon(IEnumerable<Coordinate> shell, IEnumerable<IEnumerable<Coordinate>> holes);

        /// <summary>
        /// Creates a polygon.
        /// </summary>
        /// <param name="shell">The coordinates of the shell.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>A polygon containing the specified coordinates and metadata.</returns>
        /// <exception cref="System.ArgumentNullException">The shell is null.</exception>
        /// <exception cref="System.ArgumentException">The shell is empty.</exception>
        IPolygon CreatePolygon(IEnumerable<Coordinate> shell, IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a polygon.
        /// </summary>
        /// <param name="shell">The coordinates of the shell.</param>
        /// <param name="holes">The coordinates of the holes.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>A polygon containing the specified coordinates and metadata.</returns>
        /// <exception cref="System.ArgumentNullException">The shell is null.</exception>
        /// <exception cref="System.ArgumentException">The shell is empty.</exception>
        IPolygon CreatePolygon(IEnumerable<Coordinate> shell, IEnumerable<IEnumerable<Coordinate>> holes, IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a polygon.
        /// </summary>
        /// <param name="other">The other polygon.</param>
        /// <returns>A polygon that matches <paramref name="other" />.</returns>
        /// <exception cref="System.ArgumentNullException">The other polygon is null.</exception>
        IPolygon CreatePolygon(IPolygon other);

        #endregion

        #region Factory methods for triangles

        /// <summary>
        /// Creates a triangle.
        /// </summary>
        /// <param name="first">The first coordinate.</param>
        /// <param name="second">The second coordinate.</param>
        /// <param name="third">The third coordinate.</param>
        /// <returns>The triangle containing the specified coordinates.</returns>
        ITriangle CreateTriangle(Coordinate first, Coordinate second, Coordinate third);

        /// <summary>
        /// Creates a triangle.
        /// </summary>
        /// <param name="first">The first coordinate.</param>
        /// <param name="second">The second coordinate.</param>
        /// <param name="third">The third coordinate.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The triangle containing the specified coordinates and metadata.</returns>
        ITriangle CreateTriangle(Coordinate first, Coordinate second, Coordinate third, IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a triangle.
        /// </summary>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point.</param>
        /// <param name="third">The third point.</param>
        /// <returns>The triangle containing the specified points.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// The first point is null.
        /// or
        /// The second point is null.
        /// or
        /// The third point is null.
        /// </exception>
        ITriangle CreateTriangle(IPoint first, IPoint second, IPoint third);

        /// <summary>
        /// Creates a triangle.
        /// </summary>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point.</param>
        /// <param name="third">The third point.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The triangle containing the specified points and metadata.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// The first point is null.
        /// or
        /// The second point is null.
        /// or
        /// The third point is null.
        /// </exception>
        ITriangle CreateTriangle(IPoint first, IPoint second, IPoint third, IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a triangle.
        /// </summary>
        /// <param name="other">The other triangle.</param>
        /// <returns>A triangle that matches <paramref name="other" />.</returns>
        /// <exception cref="System.ArgumentNullException">The other triangle is null.</exception>
        ITriangle CreateTriangle(ITriangle other);

        #endregion

        #region Factory methods for geometry collections

        /// <summary>
        /// Creates a geometry collection.
        /// </summary>
        /// <returns>The empty geometry collection.</returns>
        IGeometryCollection<IGeometry> CreateGeometryCollection();
        
        /// <summary>
        /// Creates a geometry collection.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The empty geometry collection with the specified metadata.</returns>
        IGeometryCollection<IGeometry> CreateGeometryCollection(IDictionary<String, Object> metadata);
        
        /// <summary>
        /// Creates a geometry collection.
        /// </summary>
        /// <param name="geometries">The source geometries.</param>
        /// <returns>The geometry collection containing the specified geometries.</returns>
        IGeometryCollection<IGeometry> CreateGeometryCollection(IEnumerable<IGeometry> geometries);
        
        /// <summary>
        /// Creates a geometry collection.
        /// </summary>
        /// <param name="geometries">The source geometries.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The geometry collection containing the specified geometries and metadata.</returns>
        IGeometryCollection<IGeometry> CreateGeometryCollection(IEnumerable<IGeometry> geometries, IDictionary<String, Object> metadata);
        
        /// <summary>
        /// Creates a geometry collection.
        /// </summary>
        /// <param name="other">The other geometry collection.</param>
        /// <returns>A geometry collection that matches <paramref name="other" />.</returns>
        /// <exception cref="System.ArgumentNullException">The other geometry collection is null.</exception>
        IGeometryCollection<IGeometry> CreateGeometryCollection(IGeometryCollection<IGeometry> other);

        /// <summary>
        /// Creates a geometry collection.
        /// </summary>
        /// <returns>The empty geometry collection.</returns>
        IGeometryCollection<T> CreateGeometryCollection<T>() where T : IGeometry;

        /// <summary>
        /// Creates a geometry collection.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The empty geometry collection with the specified metadata.</returns>
        IGeometryCollection<T> CreateGeometryCollection<T>(IDictionary<String, Object> metadata) where T : IGeometry;

        /// <summary>
        /// Creates a geometry collection.
        /// </summary>
        /// <param name="geometries">The source geometries.</param>
        /// <returns>The geometry collection containing the specified geometries.</returns>
        IGeometryCollection<T> CreateGeometryCollection<T>(IEnumerable<T> geometries) where T : IGeometry;

        /// <summary>
        /// Creates a geometry collection.
        /// </summary>
        /// <param name="geometries">The source geometries.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The geometry collection containing the specified geometries and metadata.</returns>
        IGeometryCollection<T> CreateGeometryCollection<T>(IEnumerable<T> geometries, IDictionary<String, Object> metadata) where T : IGeometry;

        /// <summary>
        /// Creates a geometry collection.
        /// </summary>
        /// <param name="other">The other geometry collection.</param>
        /// <returns>A geometry collection that matches <paramref name="other" />.</returns>
        /// <exception cref="System.ArgumentNullException">The other geometry collection is null.</exception>
        IGeometryCollection<T> CreateGeometryCollection<T>(IGeometryCollection<T> other) where T : IGeometry;

        #endregion

        #region Factory methods for multi points

        /// <summary>
        /// Creates a multi point.
        /// </summary>
        /// <returns>The empty multi point.</returns>
        IMultiPoint CreateMultiPoint();
        
        /// <summary>
        /// Creates a multi point.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The empty multi point with the specified metadata.</returns>
        IMultiPoint CreateMultiPoint(IDictionary<String, Object> metadata);
        
        /// <summary>
        /// Creates a multi point.
        /// </summary>
        /// <param name="points">The source coordinates.</param>
        /// <returns>The multi point containing the specified points.</returns>
        IMultiPoint CreateMultiPoint(IEnumerable<Coordinate> points);

        /// <summary>
        /// Creates a multi point.
        /// </summary>
        /// <param name="points">The source coordinates.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The multi point containing the specified points and metadata.</returns>
        IMultiPoint CreateMultiPoint(IEnumerable<Coordinate> points, IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a multi point.
        /// </summary>
        /// <param name="points">The source points.</param>
        /// <returns>The multi point containing the specified points.</returns>
        IMultiPoint CreateMultiPoint(IEnumerable<IPoint> points);

        /// <summary>
        /// Creates a multi point.
        /// </summary>
        /// <param name="points">The source points.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The multi point containing the specified points and metadata.</returns>
        IMultiPoint CreateMultiPoint(IEnumerable<IPoint> points, IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a multi point.
        /// </summary>
        /// <param name="other">The other multi point.</param>
        /// <returns>A multi point that matches <paramref name="other" />.</returns>
        /// <exception cref="System.ArgumentNullException">The other multi point is null.</exception>
        IMultiPoint CreateMultiPoint(IMultiPoint other);

        #endregion

        #region Factory methods for multi line strings

        /// <summary>
        /// Creates a multi line string.
        /// </summary>
        /// <returns>The empty multi line string.</returns>
        IMultiLineString CreateMultiLineString();

        /// <summary>
        /// Creates a multi line string.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The empty multi line string with the specified metadata.</returns>
        IMultiLineString CreateMultiLineString(IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a multi line string.
        /// </summary>
        /// <param name="line strings">The source line strings.</param>
        /// <returns>The multi line string containing the specified line strings.</returns>
        IMultiLineString CreateMultiLineString(IEnumerable<ILineString> lineStrings);

        /// <summary>
        /// Creates a multi line string.
        /// </summary>
        /// <param name="line strings">The source line strings.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The multi line string containing the specified line strings and metadata.</returns>
        IMultiLineString CreateMultiLineString(IEnumerable<ILineString> lineStrings, IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a multi line string.
        /// </summary>
        /// <param name="other">The other multi line string.</param>
        /// <returns>A multi line string that matches <paramref name="other" />.</returns>
        /// <exception cref="System.ArgumentNullException">The other multi line string is null.</exception>
        IMultiLineString CreateMultiLineString(IMultiLineString other);

        #endregion

        #region Factory methods for multi polygons

        /// <summary>
        /// Creates a multi polygon.
        /// </summary>
        /// <returns>The empty multi polygon.</returns>
        IMultiPolygon CreateMultiPolygon();

        /// <summary>
        /// Creates a multi polygon.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The empty multi polygon with the specified metadata.</returns>
        IMultiPolygon CreateMultiPolygon(IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a multi polygon.
        /// </summary>
        /// <param name="polygons">The source polygons.</param>
        /// <returns>The multi polygon containing the specified polygons.</returns>
        IMultiPolygon CreateMultiPolygon(IEnumerable<IPolygon> polygons);

        /// <summary>
        /// Creates a multi polygon.
        /// </summary>
        /// <param name="polygons">The source polygons.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The multi polygon containing the specified polygons and metadata.</returns>
        IMultiPolygon CreateMultiPolygon(IEnumerable<IPolygon> polygons, IDictionary<String, Object> metadata);

        /// <summary>
        /// Creates a multi polygon.
        /// </summary>
        /// <param name="other">The other multi polygon.</param>
        /// <returns>A multi polygon that matches <paramref name="other" />.</returns>
        /// <exception cref="System.ArgumentNullException">The other multi polygon is null.</exception>
        IMultiPolygon CreateMultiPolygon(IMultiPolygon other);

        #endregion

        #region Factory methods for geometries

        /// <summary>
        /// Creates a geometry matching another geometry.
        /// </summary>
        /// <param name="other">The other geometry.</param>
        /// <returns>The produced geometry matching <see cref="other" />.</returns>
        /// <exception cref="System.ArgumentNullException">The other geometry is null.</exception>
        /// <exception cref="System.ArgumentException">The type of the other geometry is not supported.</exception>
        IGeometry CreateGeometry(IGeometry other);

        #endregion
    }
}
