﻿/// <copyright file="Length.cs" company="Eötvös Loránd University (ELTE)">
///     Copyright (c) 2011-2016 Roberto Giachetta. Licensed under the
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
using System.Linq;

namespace ELTE.AEGIS
{
    /// <summary>
    /// Represents a length measure.
    /// </summary>
    [Serializable]
    public struct Length : IEquatable<Length>, IComparable<Length>
    {
        #region Static instances

        /// <summary>
        /// Represents the zero <see cref="Length" /> value. This field is constant.
        /// </summary>
        public static readonly Length Zero = new Length(0, UnitsOfMeasurement.Metre);

        /// <summary>
        /// Represents the unknown <see cref="Length" /> value. This field is constant.
        /// </summary>
        public static readonly Length Undefined = new Length(Double.NaN, UnitsOfMeasurement.Metre);

        /// <summary>
        /// Represents the negative infinite <see cref="Length" /> value. This field is constant.
        /// </summary>
        public static readonly Length NegativeInfinity = new Length(Double.PositiveInfinity, UnitsOfMeasurement.Metre);

        /// <summary>
        /// Represents the positive infinite <see cref="Length" /> value. This field is constant.
        /// </summary>
        public static readonly Length PositiveInfinity = new Length(Double.NegativeInfinity, UnitsOfMeasurement.Metre);

        /// <summary>
        /// Represents the smallest positive <see cref="Length" /> value that is greater than zero. This field is constant.
        /// </summary>
        public static readonly Length Epsilon = new Length(Double.Epsilon, UnitsOfMeasurement.Metre);

        #endregion

        #region Private fields

        private readonly Double _value;
        private readonly UnitOfMeasurement _unit;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the value of the length.
        /// </summary>
        public Double Value { get { return _value; } }

        /// <summary>
        /// Gets the base value (converted to radian) of the length.
        /// </summary>
        public Double BaseValue { get { return _value * _unit.BaseMultiple; } }

        /// <summary>
        /// Gets the unit of measurement of the length.
        /// </summary>
        public UnitOfMeasurement Unit { get { return _unit; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="unit">The unit of measurement.</param>
        /// <exception cref="System.ArgumentException">The unit is not a length measure.;unit</exception>
        public Length(Double value, UnitOfMeasurement unit)
        {
            if (unit != null && unit.Type != UnitQuantityType.Length)
                throw new ArgumentException("The unit is not a length measure.", "unit");

            _value = value;
            _unit = unit ?? UnitsOfMeasurement.Metre;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the value using the specified <see cref="UnitOfMeasurement" />.
        /// </summary>
        /// <param name="unit">The unit of measurement.</param>
        /// <returns>The value converted to the specified <see cref="UnitOfMeasurement" />.</returns>
        /// <exception cref="System.ArgumentNullException">The unit of measurement is null.</exception>
        /// <exception cref="System.ArgumentException">The unit is not an angular measure.</exception>
        public Double GetValue(UnitOfMeasurement unit)
        {
            if (unit == null)
                throw new ArgumentNullException("unit", "The unit of measurement is null.");

            if (unit.Type != UnitQuantityType.Length)
                throw new ArgumentException("The unit is not a length measure.", "unit");

            if (unit == _unit)
                return _value;

            return BaseValue / unit.BaseMultiple;
        }

        /// <summary>
        /// Returns the <see cref="System.String" /> equivalent of the instance.
        /// </summary>
        /// <param name="unit">The unit of measurement.</param>
        /// <returns>A <see cref="System.String" /> containing the value and unit symbol of the instance.</returns>
        /// <exception cref="System.ArgumentNullException">The unit of measurement is null.</exception>
        /// <exception cref="System.ArgumentException">The unit is not an angular measure.</exception>
        public String ToString(UnitOfMeasurement unit)
        {
            if (unit == null)
                throw new ArgumentNullException("unit", "The unit of measurement is null.");

            if (unit.Type != UnitQuantityType.Length)
                throw new ArgumentException("The unit is not a length measure.", "unit");

            if (Unit != unit)
            {
                return (BaseValue / unit.BaseMultiple).ToString() + unit.Symbol;
            }
            else
            {
                return Value.ToString() + Unit.Symbol;
            }
        }

        #endregion

        #region IEquatable methods

        /// <summary>
        /// Indicates whether this instance and a specified <see cref="Length" /> are equal.
        /// </summary>
        /// <param name="another">Another length to compare to.</param>
        /// <returns><c>true</c> if <paramref name="another" /> and this instance represent the same value; otherwise, <c>false</c>.</returns>
        public Boolean Equals(Length another)
        {
            if (ReferenceEquals(null, another)) return false;
            if (ReferenceEquals(this, another)) return true;

            return BaseValue.Equals(another.BaseValue);
        }

        #endregion

        #region IComparable methods

        /// <summary>
        /// Compares the current instance with another <see cref="Length" />.
        /// </summary>
        /// <param name="other">A length to compare with this angle.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has the following meanings: Value Meaning Less than zero
        /// This object is less than the other parameter.Zero This object is equal to
        /// other. Greater than zero This object is greater than other.
        /// </returns>
        public Int32 CompareTo(Length other)
        {
            return BaseValue.CompareTo(other.BaseValue);
        }

        #endregion

        #region Object methods

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns><c>true</c> if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, <c>false</c>.
        /// </returns>
        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return (obj is Length) && BaseValue.Equals(((Length)obj).BaseValue);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override Int32 GetHashCode()
        {
            return BaseValue.GetHashCode() ^ 625907383;
        }

        /// <summary>
        /// Returns the <see cref="System.String" /> equivalent of the instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> containing the value and unit symbol of the instance.</returns>
        public override String ToString()
        {
            return Value.ToString() + Unit.Symbol;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Sums the specified <see cref="Length" /> instances.
        /// </summary>
        /// <param name="first">The first length.</param>
        /// <param name="second">The second length.</param>
        /// <returns>The sum of the two <see cref="Length" /> instances.</returns>
        public static Length operator +(Length first, Length second)
        {
            return new Length(first.BaseValue + second.BaseValue, UnitsOfMeasurement.Metre);
        }

        /// <summary>
        /// Negates the specified <see cref="Length" /> instance.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>The negate of the <see cref="Length" /> instance.</returns>
        public static Length operator -(Length length)
        {
            return new Length(-length.Value, length.Unit);
        }

        /// <summary>
        /// Extracts the specified <see cref="Length" /> instances.
        /// </summary>
        /// <param name="first">The first length.</param>
        /// <param name="second">The second length.</param>
        /// <returns>The extract of the two <see cref="Length" /> instances.</returns>
        public static Length operator -(Length first, Length second)
        {
            return new Length(first.BaseValue - second.BaseValue, UnitsOfMeasurement.Metre);
        }

        /// <summary>
        /// Multiplies the <see cref="System.Double" /> scalar with the specified <see cref="Length" />.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="length">The length.</param>
        /// <returns>The scalar multiplication of the <see cref="Length" />.</returns>
        public static Length operator *(Double scalar, Length length)
        {
            return new Length(scalar * length.Value, length.Unit);
        }

        /// <summary>
        /// Multiplies the specified <see cref="Length" /> with the <see cref="System.Double" /> scalar.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>The scalar multiplication of the <see cref="Length" />.</returns>
        public static Length operator *(Length length, Double scalar)
        {
            return new Length(length.Value * scalar, length.Unit);
        }

        /// <summary>
        /// Divides the specified <see cref="Length" /> with the <see cref="System.Double" /> scalar.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>The scalar division of the <see cref="Length" />.</returns>
        public static Length operator /(Length length, Double scalar)
        {
            return new Length(length.Value / scalar, length.Unit);
        }

        /// <summary>
        /// Indicates whether the specified <see cref="Length" /> instances are equal.
        /// </summary>
        /// <param name="first">The first length.</param>
        /// <param name="second">The second length.</param>
        /// <returns><c>true</c> if the instances represent the same value; otherwise, <c>false</c>.</returns>
        public static Boolean operator ==(Length first, Length second)
        {
            return first.BaseValue == second.BaseValue;
        }

        /// <summary>
        /// Indicates whether the specified <see cref="Length" /> instances are not equal.
        /// </summary>
        /// <param name="first">The first length.</param>
        /// <param name="second">The second length.</param>
        /// <returns><c>true</c> if the instances do not represent the same value; otherwise, <c>false</c>.</returns>
        public static Boolean operator !=(Length first, Length second)
        {
            return first.BaseValue != second.BaseValue;
        }

        /// <summary>
        /// Indicates whether the first specified <see cref="Length" /> instance is less than the second.
        /// </summary>
        /// <param name="first">The first length.</param>
        /// <param name="second">The second length.</param>
        /// <returns><c>true</c> if the first <see cref="Length" /> instance is less than the second; otherwise, <c>false</c>.</returns>
        public static Boolean operator <(Length first, Length second)
        {
            return first.BaseValue < second.BaseValue;
        }

        /// <summary>
        /// Indicates whether the first specified <see cref="Length" /> instance is greater than the second.
        /// </summary>
        /// <param name="first">The first length.</param>
        /// <param name="second">The second length.</param>
        /// <returns><c>true</c> if the first <see cref="Length" /> instance is greater than the second; otherwise, <c>false</c>.</returns>
        public static Boolean operator >(Length x, Length y)
        {
            return x.BaseValue > y.BaseValue;
        }

        /// <summary>
        /// Indicates whether the first specified <see cref="Length" /> instance is smaller or equal to the second.
        /// </summary>
        /// <param name="first">The first length.</param>
        /// <param name="second">The second length.</param>
        /// <returns><c>true</c> if the first <see cref="Length" /> instance is smaller or equal to the second; otherwise, <c>false</c>.</returns>
        public static Boolean operator <=(Length x, Length y)
        {
            return x.BaseValue <= y.BaseValue;
        }

        /// <summary>
        /// Indicates whether the first specified <see cref="Length" /> instance is greater or equal to the second.
        /// </summary>
        /// <param name="first">The first length.</param>
        /// <param name="second">The second length.</param>
        /// <returns><c>true</c> if the first <see cref="Length" /> instance is greater or equal to the second; otherwise, <c>false</c>.</returns>
        public static Boolean operator >=(Length x, Length y)
        {
            return x.BaseValue >= y.BaseValue;
        }

        /// <summary>
        /// Converts the specified <see cref="Length" /> instance to a <see cref="System.Double" /> value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="System.Double" /> value of the specified <see cref="Length" /> instance.</returns>
        public static explicit operator Double(Length value)
        {
            return value.BaseValue;
        }

        /// <summary>
        /// Converts the specified <see cref="System.Double" /> instance to a <see cref="Length" /> value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="Length" /> value of the specified <see cref="System.Double" /> instance.</returns>
        public static explicit operator Length(Double value)
        {
            return new Length(value, UnitsOfMeasurement.Radian);
        }

        #endregion

        #region Public static factory methods

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.BritishChainBenoit1895A" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.BritishChainBenoit1895A" />.</returns>
        public static Length FromBritishChainBenoit1895A(Double value)
        {
            return new Length(value, UnitsOfMeasurement.BritishChainBenoit1895A);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.BritishChainBenoit1895B" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.BritishChainBenoit1895B" />.</returns>
        public static Length FromBritishChainBenoit1895B(Double value)
        {
            return new Length(value, UnitsOfMeasurement.BritishChainBenoit1895B);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.BritishChainSears1922" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.BritishChainSears1922" />.</returns>
        public static Length FromBritishChainSears1922(Double value)
        {
            return new Length(value, UnitsOfMeasurement.BritishChainSears1922);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.BritishChainSears1922T" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.BritishChainSears1922T" />.</returns>
        public static Length FromBritishChainSears1922T(Double value)
        {
            return new Length(value, UnitsOfMeasurement.BritishChainSears1922T);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.BritishFoot1865" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.BritishFoot1865" />.</returns>
        public static Length FromBritishFoot1865(Double value)
        {
            return new Length(value, UnitsOfMeasurement.BritishFoot1865);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.BritishFoot1936" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.BritishFoot1936" />.</returns>
        public static Length FromBritishFoot1936(Double value)
        {
            return new Length(value, UnitsOfMeasurement.BritishFoot1936);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.BritishFootBenoit1895A" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.BritishFootBenoit1895A" />.</returns>
        public static Length FromBritishFootBenoit1895A(Double value)
        {
            return new Length(value, UnitsOfMeasurement.BritishFootBenoit1895A);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.BritishFootBenoit1895B" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.BritishFootBenoit1895B" />.</returns>
        public static Length FromBritishFootBenoit1895B(Double value)
        {
            return new Length(value, UnitsOfMeasurement.BritishFootBenoit1895B);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.BritishFootSears1922" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.BritishFootSears1922" />.</returns>
        public static Length FromBritishFootSears1922(Double value)
        {
            return new Length(value, UnitsOfMeasurement.BritishFootSears1922);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.BritishFootSears1922T" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.BritishFootSears1922T" />.</returns>
        public static Length FromBritishFootSears1922T(Double value)
        {
            return new Length(value, UnitsOfMeasurement.BritishFootSears1922T);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.BritishLinkBenoit1895A" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.BritishLinkBenoit1895A" />.</returns>
        public static Length FromBritishLinkBenoit1895A(Double value)
        {
            return new Length(value, UnitsOfMeasurement.BritishLinkBenoit1895A);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.BritishLinkBenoit1895B" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.BritishLinkBenoit1895B" />.</returns>
        public static Length FromBritishLinkBenoit1895B(Double value)
        {
            return new Length(value, UnitsOfMeasurement.BritishLinkBenoit1895B);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.BritishLinkSears1922" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.BritishLinkSears1922" />.</returns>
        public static Length FromBritishLinkSears1922(Double value)
        {
            return new Length(value, UnitsOfMeasurement.BritishLinkSears1922);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.BritishLinkSears1922T" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.BritishLinkSears1922T" />.</returns>
        public static Length FromBritishLinkSears1922T(Double value)
        {
            return new Length(value, UnitsOfMeasurement.BritishLinkSears1922T);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.BritishYardBenoit1895A" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.BritishYardBenoit1895A" />.</returns>
        public static Length FromBritishYardBenoit1895A(Double value)
        {
            return new Length(value, UnitsOfMeasurement.BritishYardBenoit1895A);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.BritishYardBenoit1895B" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.BritishYardBenoit1895B" />.</returns>
        public static Length FromBritishYardBenoit1895B(Double value)
        {
            return new Length(value, UnitsOfMeasurement.BritishYardBenoit1895B);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.BritishYardSears1922" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.BritishYardSears1922" />.</returns>
        public static Length FromBritishYardSears1922(Double value)
        {
            return new Length(value, UnitsOfMeasurement.BritishYardSears1922);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.BritishYardSears1922T" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.BritishYardSears1922T" />.</returns>
        public static Length FromBritishYardSears1922T(Double value)
        {
            return new Length(value, UnitsOfMeasurement.BritishYardSears1922T);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.Chain" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.Chain" />.</returns>
        public static Length FromChain(Double value)
        {
            return new Length(value, UnitsOfMeasurement.Chain);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.ClarkesChain" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.ClarkesChain" />.</returns>
        public static Length FromClarkesChain(Double value)
        {
            return new Length(value, UnitsOfMeasurement.ClarkesChain);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.ClarkesFoot" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.ClarkesFoot" />.</returns>
        public static Length FromClarkesFoot(Double value)
        {
            return new Length(value, UnitsOfMeasurement.ClarkesFoot);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.ClarkesLink" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.ClarkesLink" />.</returns>
        public static Length FromClarkesLink(Double value)
        {
            return new Length(value, UnitsOfMeasurement.ClarkesLink);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.ClarkesYard" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.ClarkesYard" />.</returns>
        public static Length FromClarkesYard(Double value)
        {
            return new Length(value, UnitsOfMeasurement.ClarkesYard);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.Fathom" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.Fathom" />.</returns>
        public static Length FromFathom(Double value)
        {
            return new Length(value, UnitsOfMeasurement.Fathom);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.Foot" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.Foot" />.</returns>
        public static Length FromFoot(Double value)
        {
            return new Length(value, UnitsOfMeasurement.Foot);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.GermanLegalMetre" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.GermanLegalMetre" />.</returns>
        public static Length FromGermanLegalMetre(Double value)
        {
            return new Length(value, UnitsOfMeasurement.GermanLegalMetre);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.GoldCoastFoot" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.GoldCoastFoot" />.</returns>
        public static Length FromGoldCoastFoot(Double value)
        {
            return new Length(value, UnitsOfMeasurement.GoldCoastFoot);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.IndianFoot" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.IndianFoot" />.</returns>
        public static Length FromIndianFoot(Double value)
        {
            return new Length(value, UnitsOfMeasurement.IndianFoot);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.IndianFoot1937" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.IndianFoot1937" />.</returns>
        public static Length FromIndianFoot1937(Double value)
        {
            return new Length(value, UnitsOfMeasurement.IndianFoot1937);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.IndianFoot1962" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.IndianFoot1962" />.</returns>
        public static Length FromIndianFoot1962(Double value)
        {
            return new Length(value, UnitsOfMeasurement.IndianFoot1962);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.IndianFoot1975" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.IndianFoot1975" />.</returns>
        public static Length FromIndianFoot1975(Double value)
        {
            return new Length(value, UnitsOfMeasurement.IndianFoot1975);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.IndianYard" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.IndianYard" />.</returns>
        public static Length FromIndianYard(Double value)
        {
            return new Length(value, UnitsOfMeasurement.IndianYard);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.IndianYard1937" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.IndianYard1937" />.</returns>
        public static Length FromIndianYard1937(Double value)
        {
            return new Length(value, UnitsOfMeasurement.IndianYard1937);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.IndianYard1962" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.IndianYard1962" />.</returns>
        public static Length FromIndianYard1962(Double value)
        {
            return new Length(value, UnitsOfMeasurement.IndianYard1962);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.IndianYard1975" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.IndianYard1975" />.</returns>
        public static Length FromIndianYard1975(Double value)
        {
            return new Length(value, UnitsOfMeasurement.IndianYard1975);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.Kilometre" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.Kilometre" />.</returns>
        public static Length FromKilometre(Double value)
        {
            return new Length(value, UnitsOfMeasurement.Kilometre);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.Link" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.Link" />.</returns>
        public static Length FromLink(Double value)
        {
            return new Length(value, UnitsOfMeasurement.Link);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.Metre" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.Metre" />.</returns>
        public static Length FromMetre(Double value)
        {
            return new Length(value, UnitsOfMeasurement.Metre);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.NauticalMile" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.NauticalMile" />.</returns>
        public static Length FromNauticalMile(Double value)
        {
            return new Length(value, UnitsOfMeasurement.NauticalMile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.StatuteMile" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.StatuteMile" />.</returns>
        public static Length FromStatuteMile(Double value)
        {
            return new Length(value, UnitsOfMeasurement.StatuteMile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.USSurveyChain" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.USSurveyChain" />.</returns>
        public static Length FromUSSurveyChain(Double value)
        {
            return new Length(value, UnitsOfMeasurement.USSurveyChain);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.USSurveyFoot" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.USSurveyFoot" />.</returns>
        public static Length FromUSSurveyFoot(Double value)
        {
            return new Length(value, UnitsOfMeasurement.USSurveyFoot);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.USSurveyLink" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.USSurveyLink" />.</returns>
        public static Length FromUSSurveyLink(Double value)
        {
            return new Length(value, UnitsOfMeasurement.USSurveyLink);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.USSurveyMile" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.USSurveyMile" />.</returns>
        public static Length FromUSSurveyMile(Double value)
        {
            return new Length(value, UnitsOfMeasurement.USSurveyMile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> struct from the value specified in <see cref="UnitsOfMeasurement.Yard" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Length" /> instance with the value specified in <see cref="UnitsOfMeasurement.Yard" />.</returns>
        public static Length FromYard(Double value)
        {
            return new Length(value, UnitsOfMeasurement.Yard);
        }

        /// <summary>
        /// Convert the specified <see cref="Length" /> to the specified measurement unit.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="unit">The unit of measurement.</param>
        /// <returns>The <paramref name="length" /> measured in the <paramref="unit" />.</returns>
        /// <exception cref="System.ArgumentNullException">The unit of measurement is null.</exception>
        /// <exception cref="System.ArgumentException">The unit is not a length measure.</exception>
        public static Length Convert(Length length, UnitOfMeasurement unit)
        {
            if (unit == null)
                throw new ArgumentNullException("unit", "The unit of measurement is null.");

            if (unit.Type != UnitQuantityType.Length)
                throw new ArgumentException("The unit is not a length measure.", "unit");

            if (length.Unit.Equals(unit))
                return length;

            return new Length(length.BaseValue / unit.BaseMultiple, unit);
        }

        #endregion

        #region Public static methods

        /// <summary>
        /// Determines whether the specified <see cref="Length" /> instance is valid.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns><c>true</c> if <paramref name="length" /> is not an unknown value; otherwise, <c>false</c>.</returns>
        public static Boolean IsValid(Length length)
        {
            return !Double.IsNaN(length._value);
        }

        /// <summary>
        /// Determines the maximum of the specified <see cref="Length" /> instances.
        /// </summary>
        /// <param name="angles">The length values.</param>
        /// <returns>The maximum of the speficied <see cref="Length" /> instances.</returns>
        /// <exception cref="System.ArgumentException">There are no length values specified.</exception>
        public static Length Max(params Length[] lengths)
        {
            if (lengths == null || lengths.Length == 0)
                throw new ArgumentException("There are no length values specified.", "lengths");

            return new Length(lengths.Max(length => length.BaseValue), UnitsOfMeasurement.Metre);
        }

        /// <summary>
        /// Determines the minimum of the specified <see cref="Length" /> instances.
        /// </summary>
        /// <param name="lengths">The lengths.</param>
        /// <returns>The minimum of the speficied <see cref="Length" /> instances.</returns>
        /// <exception cref="System.ArgumentException">There are no length values specified.</exception>
        public static Length Min(params Length[] lengths)
        {
            if (lengths == null || lengths.Length == 0)
                throw new ArgumentException("There are no length values specified.", "lengths");

            return new Length(lengths.Min(length => length.BaseValue), UnitsOfMeasurement.Metre);
        }

        #endregion
    }
}
