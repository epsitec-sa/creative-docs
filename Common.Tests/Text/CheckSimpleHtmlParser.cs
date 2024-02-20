//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

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
