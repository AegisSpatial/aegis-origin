﻿/// <copyright file="FilterFactoryTest.cs" company="Eötvös Loránd University (ELTE)">
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

using ELTE.AEGIS.Numerics;
using ELTE.AEGIS.Operations.Spectral.Filtering;
using NUnit.Framework;
using System;
using System.Linq;

namespace ELTE.AEGIS.Tests.Operations.Spectral.Filtering
{
    /// <summary>
    /// Test fixture for <see cref="FilterFactory"/> class.
    /// </summary>
    [TestFixture]
    public class FilterFactoryTest
    {
        [TestCase]
        public void FilterFactoryCreateBoxFilterTest()
        {
            // test case 1: bad radius
            Assert.Throws<ArgumentOutOfRangeException>(() => FilterFactory.CreateBoxFilter(0));

            // test case 1: good radius
            Filter boxFilter = FilterFactory.CreateBoxFilter(2);

            Assert.AreEqual(boxFilter.Radius, 2);
            Assert.AreEqual(boxFilter.Factor, 25);
            Assert.AreEqual(boxFilter.Offset, 0);
            Assert.IsTrue(boxFilter.Kernel.All(value => value == 1));
        }

        [TestCase]
        public void FilterFactoryCreateGaussianFilterTest()
        {
            // test case 1: bad radius
            Assert.Throws<ArgumentOutOfRangeException>(() => FilterFactory.CreateGaussianFilter(0));

            // test case 1: good radius
            Filter gaussianFilter = FilterFactory.CreateGaussianFilter(1);

            Assert.AreEqual(gaussianFilter.Radius, 1);
            Assert.AreEqual(gaussianFilter.Factor, gaussianFilter.Kernel.Sum());
            Assert.AreEqual(gaussianFilter.Offset, 0);

            for (Int32 rowIndex = 0; rowIndex < gaussianFilter.Kernel.NumberOfRows; rowIndex++)
                for (Int32 columnIndex = 0; columnIndex < gaussianFilter.Kernel.NumberOfColumns; columnIndex++)
                    Assert.AreEqual(gaussianFilter.Kernel[rowIndex, columnIndex], 1 / (2 * Math.PI * 0.25) * Math.Exp(-(Calculator.Square(rowIndex - 1) + Calculator.Square(columnIndex - 1)) / 0.5), 0.0001);
        }
    }
}
