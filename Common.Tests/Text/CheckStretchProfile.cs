/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using Epsitec.Common.Text;
using Epsitec.Common.Text.Layout;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Text
{
    /// <summary>
    /// Summary description for CheckStretchProfile.
    /// </summary>
    [TestFixture]
    public sealed class CheckStretchProfile
    {
        [Test]
        public static void RunTests()
        {
            StretchProfile profile = new StretchProfile();

            double p1;
            double p2;
            double p3;
            double p4;
            double p5;
            double p6;

            profile.Add(Unicode.StretchClass.NoStretch, 10.0);
            profile.Add(Unicode.StretchClass.Character, 90.0);
            profile.Add(Unicode.StretchClass.Space, 10.0);

            p1 = profile.ComputePenalty(110, 10, 2);
            p2 = profile.ComputePenalty(120, 10, 2);
            p3 = profile.ComputePenalty(150, 10, 2);
            p4 = profile.ComputePenalty(109, 10, 2);
            p5 = profile.ComputePenalty(108, 10, 2);
            p6 = profile.ComputePenalty(107, 10, 2);

            profile.Clear();
            profile.Add(Unicode.StretchClass.Character, 100.0);
            profile.Add(Unicode.StretchClass.Space, 10.0);

            p1 = profile.ComputePenalty(110, 10, 2);
            p2 = profile.ComputePenalty(120, 10, 2);
            p3 = profile.ComputePenalty(150, 10, 2);
            p4 = profile.ComputePenalty(109, 10, 2);
            p5 = profile.ComputePenalty(108, 10, 2);
            p6 = profile.ComputePenalty(107, 10, 2);

            profile.Clear();
            profile.Add(Unicode.StretchClass.Character, 90.0);
            profile.Add(Unicode.StretchClass.Space, 10.0);
            profile.Add(Unicode.StretchClass.Kashida, 10.0);

            p1 = profile.ComputePenalty(110, 10, 2);
            p2 = profile.ComputePenalty(120, 10, 2);
            p3 = profile.ComputePenalty(150, 10, 2);
            p4 = profile.ComputePenalty(109, 10, 2);
            p5 = profile.ComputePenalty(108, 10, 2);
            p6 = profile.ComputePenalty(107, 10, 2);

            double[] widths = new double[] { 10, 20, 30, 20, 10, 5, 5, 2, 8 };
            double[] scales = new double[widths.Length];
            Unicode.StretchClass[] stretch = new Unicode.StretchClass[]
            {
                Unicode.StretchClass.Character,
                Unicode.StretchClass.Character,
                Unicode.StretchClass.Character,
                Unicode.StretchClass.Character,
                Unicode.StretchClass.Character,
                Unicode.StretchClass.Space,
                Unicode.StretchClass.Space,
                Unicode.StretchClass.Kashida,
                Unicode.StretchClass.Kashida
            };

            StretchProfile.Scales classScales;
            profile.ComputeScales(120, out classScales);

            Assert.IsTrue(classScales.ScaleCharacter > 1.0009);
            Assert.IsTrue(classScales.ScaleKashida > 1.9017);
            Assert.IsTrue(classScales.ScaleSpace > 1.0901);
            Assert.IsTrue(classScales.ScaleCharacter < 1.0010);
            Assert.IsTrue(classScales.ScaleKashida < 1.9018);
            Assert.IsTrue(classScales.ScaleSpace < 1.0902);
            Assert.IsTrue(classScales.ScaleNoStretch == 1.0);

            profile.ComputeScales(150, out classScales);

            Assert.IsTrue(classScales.ScaleCharacter > 1.0036);
            Assert.IsTrue(classScales.ScaleKashida > 4.6068);
            Assert.IsTrue(classScales.ScaleSpace > 1.3606);
            Assert.IsTrue(classScales.ScaleCharacter < 1.0037);
            Assert.IsTrue(classScales.ScaleKashida < 4.6069);
            Assert.IsTrue(classScales.ScaleSpace < 1.3607);
            Assert.IsTrue(classScales.ScaleNoStretch == 1.0);

            profile.ComputeScales(108, out classScales);

            Assert.IsTrue(classScales.ScaleCharacter > 0.9999);
            Assert.IsTrue(classScales.ScaleKashida > 0.9000);
            Assert.IsTrue(classScales.ScaleSpace > 0.9000);
            Assert.IsTrue(classScales.ScaleCharacter < 1.0000);
            Assert.IsTrue(classScales.ScaleKashida < 0.9001);
            Assert.IsTrue(classScales.ScaleSpace < 0.9001);
            Assert.IsTrue(classScales.ScaleNoStretch == 1.0);
        }
    }
}
