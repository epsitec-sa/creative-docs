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
using Epsitec.Common.Text.Cursors;
using Epsitec.Common.Text.Support;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Text
{
    /// <summary>
    /// Summary description for CheckSimpleHtmlParser.
    /// </summary>
    [TestFixture]
    public sealed class CheckSimpleHtmlParser
    {
        public static void TestParsingWorksOnValidData()
        {
            TextStory story = new TextStory();
            SimpleCursor cursor = new SimpleCursor();

            story.NewCursor(cursor);

            SimpleHtmlParser parser = new SimpleHtmlParser(story, cursor);

            parser.Parse(@"Abc<b>def<i>123</i>456</b>ghi&#160;<a href=""xyz"">link</a><br />");
        }

        [Test]
        public static void RunEx1()
        {
            Assert.Throws<System.FormatException>(() =>
            {
                TextStory story = new TextStory();
                SimpleCursor cursor = new SimpleCursor();

                story.NewCursor(cursor);

                SimpleHtmlParser parser = new SimpleHtmlParser(story, cursor);

                parser.Parse(@"Abc&gt;&amp;&lt;&#160;<b&lt;");
            });
        }

        [Test]
        public static void RunEx2()
        {
            Assert.Throws<System.FormatException>(() =>
            {
                TextStory story = new TextStory();
                SimpleCursor cursor = new SimpleCursor();

                story.NewCursor(cursor);

                SimpleHtmlParser parser = new SimpleHtmlParser(story, cursor);

                parser.Parse(@"Abc<b>def<i>123</b>456</i>ghi <a href=""xyz"">link</a><br />");
            });
        }
    }
}
