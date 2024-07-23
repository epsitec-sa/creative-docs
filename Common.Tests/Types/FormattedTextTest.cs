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


using Epsitec.Common.Types;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Types
{
    [TestFixture]
    public class FormattedTextTest
    {
        [Test]
        public void CheckSplit1()
        {
            FormattedText text = new FormattedText("<i>italique</i>;;<b>gras</b>");
            FormattedText[] result = text.Split(";", System.StringSplitOptions.RemoveEmptyEntries);

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(new FormattedText("<i>italique</i>"), result[0]);
            Assert.AreEqual(new FormattedText("<b>gras</b>"), result[1]);
        }
    }
}
