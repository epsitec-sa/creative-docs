//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Tests
{
	/// <summary>
	/// Summary description for CheckSimpleHtmlParser.
	/// </summary>
	public sealed class CheckSimpleHtmlParser
	{
		public static void RunTests()
		{
			Text.TextStory       story  = new TextStory ();
			Cursors.SimpleCursor cursor = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			Support.SimpleHtmlParser parser = new Support.SimpleHtmlParser (story, cursor);
			
			parser.Parse (@"Abc<b>def<i>123</i>456</b>ghi&#160;<a href=""xyz"">link</a><br />");
			
			Debug.Expect.Exception (new Debug.Method (CheckSimpleHtmlParser.RunEx1), typeof (System.FormatException));
			Debug.Expect.Exception (new Debug.Method (CheckSimpleHtmlParser.RunEx2), typeof (System.FormatException));
		}
		
		
		private static void RunEx1()
		{
			Text.TextStory       story  = new TextStory ();
			Cursors.SimpleCursor cursor = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			Support.SimpleHtmlParser parser = new Support.SimpleHtmlParser (story, cursor);
			
			parser.Parse (@"Abc&gt;&amp;&lt;&#160;<b&lt;");
		}
		
		private static void RunEx2()
		{
			Text.TextStory       story  = new TextStory ();
			Cursors.SimpleCursor cursor = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			Support.SimpleHtmlParser parser = new Support.SimpleHtmlParser (story, cursor);
			
			parser.Parse (@"Abc<b>def<i>123</b>456</i>ghi <a href=""xyz"">link</a><br />");
		}
	}
}
