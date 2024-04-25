using System.Collections.Generic;
using Epsitec.Common.OpenType;
using NUnit.Framework;

namespace Epsitec.Common.Tests.OpenType
{
    public class FontCollectionTest
    {
        [Test]
        public void ConstructorReturnsWithoutErrors()
        {
            FontCollection collection = new FontCollection();
            Assert.IsNotNull(collection);
        }

        [Test]
        public void GetFontReturnsNonNullFontOnExistingFontName()
        {
            FontCollection collection = new FontCollection();
            var fids = DummyFontIdentities();
            collection.Initialize(fids);
            // In the tests, we don't have actual font files, so we still get an exception
            // we can still distinguish a font missing from the database (NoMatchingFontException)
            // from a font in the database that doesn't have a valid font file (FontFileNotFoundException)
            Assert.Throws<FontFileNotFoundException>(() =>
            {
                Font font = collection.CreateFont("AwesomeFont");
            });
        }

        [Test]
        public void GetFontRaisesNoMatchingFontExceptionOnEmptyCollection()
        {
            FontCollection collection = new FontCollection();
            var fids = DummyFontIdentities();
            Assert.Throws<NoMatchingFontException>(() =>
            {
                Font font = collection.CreateFont("AwesomeFont");
            });
        }

        public IEnumerable<FontIdentity> DummyFontIdentities()
        {
            yield return new FontIdentity("some/path/to/awesome.ttf", new FontName("AwesomeFont"));
            yield return new FontIdentity(
                "some/path/to/anotherfont.ttf",
                new FontName("AnotherFont")
            );
        }
    }
}
