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
