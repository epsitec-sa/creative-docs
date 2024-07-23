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

using Epsitec.Common.OpenType;
using NUnit.Framework;

namespace Epsitec.Common.Tests.OpenType
{
    public class FontNameTest
    {
        [Test]
        public void FontNameWithoutStyleHasNormalStyle() { 
            FontName fn = new FontName("Crazy");
            Assert.AreEqual("Crazy", fn.FaceName);
            Assert.AreEqual("Normal", fn.StyleName);
            Assert.AreEqual("Crazy", fn.FullName);
        }

        [Test]
        public void FontNameWithBoldStyle() { 
            FontName fn = new FontName("Crazy", "Bold");
            Assert.AreEqual("Crazy", fn.FaceName);
            Assert.AreEqual("Bold", fn.StyleName);
            Assert.AreEqual("Crazy Bold", fn.FullName);
        }

        [Test]
        public void FontNameWithBoldItalicStyle() { 
            FontName fn = new FontName("Crazy", "Bold Italic");
            Assert.AreEqual("Crazy", fn.FaceName);
            Assert.AreEqual("Bold Italic", fn.StyleName);
            Assert.AreEqual("Crazy Bold Italic", fn.FullName);
        }

        [Test]
        public void FontNameFilterInvalidStyles() { 
            FontName fn = new FontName("Crazy", "Whatever Italic Nonsense Bold Thing");
            Assert.AreEqual("Crazy", fn.FaceName);
            Assert.AreEqual("Bold Italic", fn.StyleName);
            Assert.AreEqual("Crazy Bold Italic", fn.FullName);
        }
    }
}
