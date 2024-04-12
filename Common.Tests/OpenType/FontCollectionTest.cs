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
            Font font = collection.CreateFont("AwesomeFont");
            Assert.IsNotNull(font);
        }

        [Test]
        public void GetFontFallbackOnNonexistingFontName()
        {
            FontCollection collection = new FontCollection();
            var fids = DummyFontIdentities();
            collection.Initialize(fids);
            Assert.Throws<FontNotFoundException>(() =>
            {
                Font font = collection.CreateFont("uaieteunnaiuietsanutnaetsra");
            });
        }

        public IEnumerable<FontIdentity> DummyFontIdentities()
        {
            /*
            var fonts = Platform.FontFinder.FindFonts();
            foreach (string fontpath in fonts)
            {
                FontStyle fontStyle = fontStyleGetter(fontpath);
                FontName fontName = new FontName(Path.GetFileName(fontpath), fontStyle);
                FontIdentity fontIdentity = new FontIdentity(fontpath, fontName);
                this.fontDict[fontIdentity.Name] = fontIdentity;
            }
            */
            yield return new FontIdentity("some/path/to/awesome.ttf", new FontName("AwesomeFont"));
            yield return new FontIdentity("some/path/to/anotherfont.ttf", new FontName("AnotherFont"));
        }

    }
}
