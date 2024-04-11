using Epsitec.Common.OpenType;
using NUnit.Framework;

namespace Epsitec.Common.Tests.OpenType
{
    public class FontCollectionTest
    {
        [Test]
        public void ConstructorReturnsWithoutErrors() { 
            FontCollection collection = new FontCollection();
            Assert.IsNotNull(collection);
        }

        [Test]
        public void GetFontReturnsNonNullFontOnExistingFontName() { 
            FontCollection collection = new FontCollection();
            Font font = collection.CreateFont("Arial");
            Assert.IsNotNull(font);
        }

        [Test]
        public void GetFontFallbackOnNonexistingFontName() { 
            FontCollection collection = new FontCollection();
            Font font = collection.CreateFont("uaieteunnaiuietsanutnaetsra");
            Assert.IsNotNull(font);
        }
    }
}
