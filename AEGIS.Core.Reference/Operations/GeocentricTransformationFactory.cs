﻿/// <copyright file="GeocentricTransformationFactory.cs" company="Eötvös Loránd University (ELTE)">
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

using ELTE.AEGIS.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ELTE.AEGIS.Reference.Operations
{
    /// <summary>
    /// Provides factory methods for creating <see cref="GeocentricTransformation" /> instances.
    /// </summary>
    [IdentifiedObjectFactory(typeof(GeocentricTransformation))]
    public static class GeocentricTransformationFactory
    {
        #region Query fields

        private static Dictionary<CoordinateOperationMethod, Type> _operations;

        #endregion

        #region Query methods

        /// <summary>
        /// Returns all <see cref="GeocentricTransformation" /> instances matching a specified identifier.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns>The list containing the <see cref="GeocentricTransformation" /> instances that match the specified identifier.</returns>
        /// <exception cref="System.ArgumentNullException">The name is null.</exception>
        /// <exception cref="System.ArgumentException">The identifier is empty.;identifier</exception>
        public static IList<GeocentricTransformation> FromIdentifier(String identifier)
        {
            if (identifier == null)
                throw new ArgumentNullException("identifier", "The identifier is null.");           
            if (String.IsNullOrEmpty(identifier))
                throw new ArgumentException("The identifier is empty.", "identifier");

            // identifier correction
            identifier = Regex.Escape(identifier);

            // query methods with the appropriate attribute
            MethodInfo[] methods = typeof(GeocentricTransformationFactory).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(method =>
                {
                    Object attribute = method.GetCustomAttributes(typeof(IdentifiedObjectFactoryMethodAttribute), false).FirstOrDefault(); 
                    if (attribute == null)
                        return false;

                    return Regex.IsMatch((attribute as IdentifiedObjectFactoryMethodAttribute).Identifier, identifier, RegexOptions.IgnoreCase);
                }).ToArray();

            List<GeocentricTransformation> operations = new List<GeocentricTransformation>();

            // invoke methods and gather return values
            foreach (MethodInfo method in methods)
            {
                operations.Add(typeof(GeocentricTransformationFactory).InvokeMember(method.Name, BindingFlags.Public | BindingFlags.Static, null, null, null) as GeocentricTransformation);
            }

            return operations;
        }

        /// <summary>
        /// Returns all <see cref="GeocentricTransformation" /> instances matching a specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The list containing the <see cref="GeocentricTransformation" /> instances that match the specified name.</returns>
        /// <exception cref="System.ArgumentNullException">The name is null.</exception>
        /// <exception cref="System.ArgumentException">The name is empty.;name</exception>
        public static IList<GeocentricTransformation> FromName(String name)
        {
            if (name == null)
                throw new ArgumentNullException("name", "The name is null.");
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("The name is empty.", "name");

            // name correction
            name = Regex.Escape(name);

            // query methods with the appropriate attribute
            MethodInfo[] methods = typeof(GeocentricTransformationFactory).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(method =>
            {
                Object attribute = method.GetCustomAttributes(typeof(IdentifiedObjectFactoryMethodAttribute), false).FirstOrDefault();
                if (attribute == null)
                    return false;

                return Regex.IsMatch((attribute as IdentifiedObjectFactoryMethodAttribute).Name, name, RegexOptions.IgnoreCase);
            }).ToArray();

            List<GeocentricTransformation> operations = new List<GeocentricTransformation>();

            // invoke methods and gather return values
            foreach (MethodInfo method in methods)
            {
                operations.Add(typeof(GeocentricTransformationFactory).InvokeMember(method.Name, BindingFlags.Public | BindingFlags.Static, null, null, null) as GeocentricTransformation);
            }

            return operations;
        }

        /// <summary>
        /// Returns all <see cref="GeocentricTransformation" /> instances matching a specified method identifier.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The list containing the <see cref="GeocentricTransformation" /> instances that match the specified identifier.</returns>
        /// <exception cref="System.ArgumentNullException">The name is null.</exception>
        /// <exception cref="System.ArgumentException">The identifier is empty.;identifier</exception>
        public static IList<GeocentricTransformation> FromMethodIdentifier(String identifier, IDictionary<CoordinateOperationParameter, Object> parameters)
        {
            if (identifier == null)
                throw new ArgumentNullException("identifier", "The identifier is null.");
            if (String.IsNullOrEmpty(identifier))
                throw new ArgumentException("The identifier is empty.", "identifier");

            if (_operations == null)
                LoadOperations();

            // identifier correction
            identifier = Regex.Escape(identifier);

            List<GeocentricTransformation> operations = new List<GeocentricTransformation>();

            // query types with the specified identifier
            foreach (CoordinateOperationMethod method in _operations.Keys.Where(m => Regex.IsMatch(m.Identifier, identifier, RegexOptions.IgnoreCase)))
            {
                if (!_operations.ContainsKey(method))
                    continue;
                Boolean hasParameters = true;
                foreach (CoordinateOperationParameter parameter in method.Parameters)
                    if (!parameters.ContainsKey(parameter))
                        hasParameters = false;

                if (!hasParameters)
                    continue;

                try
                {
                    // create instances
                    operations.Add(Activator.CreateInstance(_operations[method], method.Identifier, method.Name, parameters) as GeocentricTransformation);
                }
                catch (ArgumentException) { }
            }

            return operations;
        }

        /// <summary>
        /// Returns all <see cref="GeocentricTransformation" /> instances matching a specified method name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The list containing the <see cref="GeocentricTransformation" /> instances that match the specified name.</returns>
        /// <exception cref="System.ArgumentNullException">The name is null.</exception>
        /// <exception cref="System.ArgumentException">The name is empty.;name</exception>
        public static IList<GeocentricTransformation> FromMethodName(String name, IDictionary<CoordinateOperationParameter, Object> parameters)
        {
            if (name == null)
                throw new ArgumentNullException("name", "The name is null.");
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("The name is empty.", "name");

            if (_operations == null)
                LoadOperations();

            // name correction
            name = Regex.Escape(name);

            List<GeocentricTransformation> operations = new List<GeocentricTransformation>();

            // query types with the specified name
            foreach (CoordinateOperationMethod method in _operations.Keys.Where(m => Regex.IsMatch(m.Name, name, RegexOptions.IgnoreCase) || m.Aliases != null && m.Aliases.Any(alias => Regex.IsMatch(alias, name, RegexOptions.IgnoreCase))))
            {
                if (!_operations.ContainsKey(method))
                    continue;
                Boolean hasParameters = true;
                foreach (CoordinateOperationParameter parameter in method.Parameters)
                    if (!parameters.ContainsKey(parameter))
                        hasParameters = false;

                if (!hasParameters)
                    continue;

                try
                {
                    // create instances
                    operations.Add(Activator.CreateInstance(_operations[method], method.Identifier, method.Name, parameters) as GeocentricTransformation);
                }   
                catch (ArgumentException) { }
            }

            return operations;
        }

        /// <summary>
        /// Returns the <see cref="GeocentricTransformation" /> instance for the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The <see cref="GeocentricTransformation" /> instance implementing the method.</returns>
        /// <exception cref="System.ArgumentNullException">The method is null.</exception>
        public static GeocentricTransformation FromMethod(CoordinateOperationMethod method, IDictionary<CoordinateOperationParameter, Object> parameters)
        {
            if (method == null)
                throw new ArgumentNullException("method", "The method is null.");

            if (_operations == null)
                LoadOperations();

            if (!_operations.ContainsKey(method))
                return null;

            // create instance
            return Activator.CreateInstance(_operations[method], method.Identifier, method.Name, parameters) as GeocentricTransformation;
        }

        /// <summary>
        /// Loads the operations.
        /// </summary>
        private static void LoadOperations()
        {            
            _operations = new Dictionary<CoordinateOperationMethod,Type>();

            // collect all geocentric transformation types within the assembly that matches the specified name
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(type => type.IsSubclassOf(typeof(GeocentricTransformation))))
            {
                // query the attribute of the type
                Object attribute = type.GetCustomAttributes(typeof(CoordinateOperationMethodImplementationAttribute), false).FirstOrDefault();
                if (attribute == null)
                    continue;

                // query the method of the transformation
                CoordinateOperationMethod method = CoordinateOperationMethods.FromIdentifier((attribute as CoordinateOperationMethodImplementationAttribute).Identifier).FirstOrDefault();
                if (method == null)
                    continue;

                _operations.Add(method, type);
            }
        }

        #endregion
    }
}
