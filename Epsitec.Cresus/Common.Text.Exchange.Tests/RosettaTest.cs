//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Michael WALZ

using NUnit.Framework;

namespace Epsitec.Common.Text.Exchange
{
	[TestFixture] 
	public class RosettaTest
	{
		[Test] public void TestCtmlToHtmlConversion()
		{
			Rosetta rosetta = new Rosetta ();
			
			Text.TextStory       story  = new Text.TextStory ();
			TextNavigator        navigator = new TextNavigator (story) ;

			
			ulong[] text;
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			
			story.ConvertToStyledText ("Affiche", properties, out text);
			
			ICursor cursor = new Cursors.SimpleCursor ();
			story.NewCursor (cursor);
			
			string text_1 = "Hello you world !\n";
			string text_2 = "wonderful ";
			
			ulong[] utf32_text_1;
			ulong[] utf32_text_2;
			
			TextConverter.ConvertFromString (text_1, out utf32_text_1);
			TextConverter.ConvertFromString (text_2, out utf32_text_2);
			
			story.InsertText (cursor, utf32_text_1);

			story.InsertText (cursor, text);


			navigator.MoveTo(0,0) ;

			while (true)
			{
				int runLength = navigator.GetRunLength(1000000) ;

				if (runLength == 0)
					break ;

				string thetext = navigator.ReadText(runLength) ;
				Property[] theproperties = navigator.TextProperties ;
				TextStyle[] styles		= navigator.TextStyles ;

				navigator.MoveTo(TextNavigator.Target.CharacterNext, runLength) ;
			}
		
			
			string ctml = "<run>Hello, </run><run><rundef><b/></rundef>world</run>.";
			string html = "Hello, <b>world</b>.";
			
//			Assert.AreEqual (rosetta.ConvertCtmlToHtml (ctml), html);
		}
	}
}
