using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class TextTest
	{
		[Test] public void CheckGenerator()
		{
			Common.Text.Tests.CheckGenerator.RunTests ();
		}
		
		[Test] public void CheckInternalCursor()
		{
			Common.Text.Tests.CheckInternalCursor.RunTests ();
		}
		
		[Test] public void CheckInternalCursorIdArray()
		{
			Common.Text.Tests.CheckInternalCursorIdArray.RunTests ();
		}
		
		[Test] public void CheckInternalCursorTable()
		{
			Common.Text.Tests.CheckInternalCursorTable.RunTests ();
		}
		
		[Test] public void CheckLayout()
		{
			Common.Text.Tests.CheckLayout.RunTests ();
		}
		
		[Test] public void CheckNavigator()
		{
			Common.Text.Tests.CheckNavigator.RunTests ();
		}
		
		[Test] public void CheckParagraphManager()
		{
			Common.Text.Tests.CheckParagraphManager.RunTests ();
		}
		
		[Test] public void CheckProperties()
		{
			Common.Text.Tests.CheckProperties.RunTests ();
		}
		
		[Test] public void CheckSerializerSupport()
		{
			Common.Text.Tests.CheckSerializerSupport.RunTests ();
		}
		
		[Test] public void CheckSimpleHtmlParser()
		{
			Common.Text.Tests.CheckSimpleHtmlParser.RunTests ();
		}
		
		[Test] public void CheckTextConverter()
		{
			Common.Text.Tests.CheckTextConverter.RunTests ();
		}
		
		[Test] public void CheckTextFitter()
		{
			Common.Text.Tests.CheckTextFitter.RunTests ();
		}
		
		[Test] public void CheckTextStory()
		{
			Common.Text.Tests.CheckTextStory.RunTests ();
		}
		
		[Test] public void CheckTextTable()
		{
			Common.Text.Tests.CheckTextTable.RunTests ();
		}
		
		[Test] public void CheckUnicode()
		{
			Common.Text.Tests.CheckUnicode.RunTests ();
		}
		
		[Test] public void CheckTextNavigator()
		{
			Text.TextStory       story  = new Text.TextStory ();
			Text.TextFitter      fitter = new Text.TextFitter (story);
			Text.SimpleTextFrame frame  = new Text.SimpleTextFrame (100, 1000);
			
			fitter.FrameList.Add (frame);
			
			Text.TextNavigator navigator = new Text.TextNavigator (story, fitter);
			
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			properties.Add (new Text.Properties.FontProperty ("Verdana", "Regular"));
			properties.Add (new Text.Properties.FontSizeProperty (14.0, Text.Properties.SizeUnits.Points));
			
			Text.TextStyle default_style = story.StyleList.NewTextStyle ("Default", Text.TextStyleClass.Paragraph, properties);
			
			story.TextContext.DefaultStyle = default_style;
			
			Assert.AreEqual (0, navigator.TextLength);
			Assert.AreEqual (0, navigator.CursorPosition);
			
			navigator.Insert ("abc\ndef");
			
			Assert.AreEqual (7, navigator.TextLength);
			Assert.AreEqual (7, navigator.CursorPosition);
			
			navigator.MoveTo (Text.TextNavigator.Target.TextStart, 0);
			
			Assert.AreEqual (0, navigator.CursorPosition);
			
			navigator.MoveTo (Text.TextNavigator.Target.ParagraphStart, 0);
			
			Assert.AreEqual (0, navigator.CursorPosition);
			
			navigator.MoveTo (Text.TextNavigator.Target.ParagraphEnd, 0);
			
			Assert.AreEqual (3, navigator.CursorPosition);
			
			navigator.MoveTo (Text.TextNavigator.Target.ParagraphEnd, 0);
			
			Assert.AreEqual (3, navigator.CursorPosition);
			
			navigator.MoveTo (Text.TextNavigator.Target.ParagraphEnd, 1);
			
			Assert.AreEqual (7, navigator.CursorPosition);
			
			navigator.MoveTo (Text.TextNavigator.Target.ParagraphStart, 1);
			
			Assert.AreEqual (0, navigator.CursorPosition);
			
			navigator.MoveTo (Text.TextNavigator.Target.ParagraphEnd, 1);
			navigator.MoveTo (Text.TextNavigator.Target.ParagraphStart, 0);
			
			Assert.AreEqual (4, navigator.CursorPosition);
			
			navigator.MoveTo (Text.TextNavigator.Target.PreviousCharacter, 1);
			Assert.AreEqual (3, navigator.CursorPosition);
			
			navigator.Insert (" xyz   qrs");
			Assert.AreEqual (13, navigator.CursorPosition);
			
			navigator.MoveTo (Text.TextNavigator.Target.WordEnd, 0);
			Assert.AreEqual (13, navigator.CursorPosition);
			
			navigator.MoveTo (Text.TextNavigator.Target.WordStart, 0);
			Assert.AreEqual (10, navigator.CursorPosition);
			
			navigator.MoveTo (Text.TextNavigator.Target.WordStart, 0);
			Assert.AreEqual (10, navigator.CursorPosition);
			
			navigator.MoveTo (Text.TextNavigator.Target.WordStart, 1);
			Assert.AreEqual (4, navigator.CursorPosition);
			
			navigator.MoveTo (Text.TextNavigator.Target.WordEnd, 0);
			Assert.AreEqual (4, navigator.CursorPosition);
			
			navigator.MoveTo (Text.TextNavigator.Target.PreviousCharacter, 2);
			Assert.AreEqual (2, navigator.CursorPosition);
			navigator.MoveTo (Text.TextNavigator.Target.WordEnd, 0);
			Assert.AreEqual (4, navigator.CursorPosition);
			
			navigator.MoveTo (Text.TextNavigator.Target.WordEnd, 1);
			Assert.AreEqual (10, navigator.CursorPosition);
			
			navigator.MoveTo (Text.TextNavigator.Target.WordEnd, 1);
			Assert.AreEqual (13, navigator.CursorPosition);
			
			navigator.MoveTo (Text.TextNavigator.Target.TextStart, 0);
			navigator.MoveTo (Text.TextNavigator.Target.ParagraphEnd, 0);
			navigator.Insert (". " + "Just for fun, " + "some more " + "text in order           " + "to be able to " + "test the line " + "navigation " + "algorithm.");
			
			fitter.ClearAllMarks ();
			fitter.GenerateAllMarks ();
			
			int para_count;
			int line_count;
			
			fitter.GetStatistics (out para_count, out line_count);
			
			Assert.AreEqual (2, para_count);
			Assert.AreEqual (9, line_count);
			Assert.AreEqual (116, navigator.TextLength);
			
			navigator.MoveTo (Text.TextNavigator.Target.TextStart, 0);
			
			Assert.IsTrue (navigator.IsLineStart (0));
			Assert.IsTrue (navigator.IsLineStart (15));
			Assert.IsTrue (navigator.IsLineStart (29));
			Assert.IsTrue (navigator.IsLineStart (39));
			Assert.IsTrue (navigator.IsLineStart (63));
			Assert.IsTrue (navigator.IsLineStart (77));
			Assert.IsTrue (navigator.IsLineStart (91));
			Assert.IsTrue (navigator.IsLineStart (102));
			Assert.IsTrue (navigator.IsLineStart (113));
		}
	}
}
