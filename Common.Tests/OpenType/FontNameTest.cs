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
