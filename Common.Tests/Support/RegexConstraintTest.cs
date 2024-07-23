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

using Epsitec.Common.Support;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Support
{
    [TestFixture]
    public class RegexConstraintTest
    {
        [Test]
        public void CheckConstraint()
        {
            RegexConstraint c1 = new RegexConstraint("a*", RegexFactory.Options.IgnoreCase);
            RegexConstraint c2 = new RegexConstraint(RegexFactory.LocalizedDecimalNum);
            RegexConstraint c3 = new RegexConstraint(PredefinedRegex.AlphaNum);

            Assert.IsTrue(c1.IsValidValue("abc"));
            Assert.IsTrue(c2.IsValidValue((10.5M).ToString()));
            Assert.IsTrue(c3.IsValidValue("abc"));

            Assert.IsFalse(c1.IsValidValue("xyz"));
            Assert.IsFalse(c2.IsValidValue("abc"));
            Assert.IsFalse(c3.IsValidValue("10"));
        }

        [Test]
        public void CheckToStringFromString()
        {
            RegexConstraint c1 = new RegexConstraint("a*", RegexFactory.Options.IgnoreCase);
            RegexConstraint c2 = new RegexConstraint(RegexFactory.LocalizedDecimalNum);
            RegexConstraint c3 = new RegexConstraint(PredefinedRegex.AlphaNum);

            string s1 = c1.ToString();
            string s2 = c2.ToString();
            string s3 = c3.ToString();

            c1 = RegexConstraint.FromString(s1);
            c2 = RegexConstraint.FromString(s2);
            c3 = RegexConstraint.FromString(s3);

            Assert.IsTrue(c1.IsValidValue("abc"));
            Assert.IsTrue(c2.IsValidValue((10.5M).ToString()));
            Assert.IsTrue(c3.IsValidValue("abc"));

            Assert.IsFalse(c1.IsValidValue("xyz"));
            Assert.IsFalse(c2.IsValidValue("abc"));
            Assert.IsFalse(c3.IsValidValue("10"));

            Assert.AreEqual("a*", c1.Pattern);
            Assert.AreEqual(RegexFactory.Options.IgnoreCase, c1.PatternOptions);
            Assert.AreEqual(RegexFactory.LocalizedDecimalNum.ToString(), c2.Regex.ToString());
            Assert.AreEqual(PredefinedRegex.AlphaNum, c3.PredefinedRegex);
        }
    }
}
