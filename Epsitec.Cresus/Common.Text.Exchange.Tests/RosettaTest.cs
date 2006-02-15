//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Michael WALZ

using NUnit.Framework;

namespace Epsitec.Common.Text.Exchange
{
	[TestFixture] 
	public class RosettaTest
	{
		[SetUp] public void Intialize()
		{
			Widgets.Widget.Initialise ();
		}
			
		
		private static void CreateTextContext(out TextContext context)
		{
			context = new TextContext ();
			
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			string black = Drawing.RichColor.ToString (Drawing.RichColor.FromBrightness (0));
			
			properties.Add (new Properties.FontProperty ("Arial", "Regular", "kern", "liga"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points, 0.0));
			properties.Add (new Properties.MarginsProperty (0, 0, 0, 0, Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Properties.ThreeState.True, 0, null));
			properties.Add (new Properties.FontColorProperty (black));
			properties.Add (new Properties.LanguageProperty (System.Globalization.CultureInfo.CurrentCulture.Name, 1.0));
			properties.Add (new Properties.LeadingProperty (1.0, Properties.SizeUnits.PercentNotCombining, 0.0, Properties.SizeUnits.Points, 0.0, Properties.SizeUnits.Points, Properties.AlignMode.None));
			properties.Add (new Properties.KeepProperty (2, 2, Properties.ParagraphStartMode.Anywhere, Properties.ThreeState.False, Properties.ThreeState.False));
			
			TextStyle paraStyle = context.StyleList.NewTextStyle (null, "Default", TextStyleClass.Paragraph, properties);
			TextStyle charStyle = context.StyleList.NewTextStyle (null, "Default", TextStyleClass.Text);
			
			context.DefaultParagraphStyle = paraStyle;
			context.StyleList.StyleMap.SetRank (null, paraStyle, 0);
			context.StyleList.StyleMap.SetCaption (null, paraStyle, "Normal");

			context.DefaultTextStyle = charStyle;
			context.StyleList.StyleMap.SetRank (null, charStyle, 0);
			context.StyleList.StyleMap.SetCaption (null, charStyle, "Caract�res par d�faut");
		}
		
		private static void CreateEmptyTextStoryAndNavigator(TextContext context, out TextStory story, out TextNavigator navigator)
		{
			System.Diagnostics.Debug.Assert (context != null);
			
			story = new TextStory (context);
			navigator = new TextNavigator (story);
			
			story.DisableOpletQueue();
			
			navigator.Insert (Unicode.Code.EndOfText);
			navigator.MoveTo (TextNavigator.Target.TextStart, 0);
			
			story.EnableOpletQueue();
		}
		
		
		[Test] public void TestCtmlToHtmlConversion()
		{
			Rosetta rosetta = new Rosetta ();
			
			TextContext     context;
			TextStory       story;
			TextNavigator   navigator;
			
			Wrappers.TextWrapper      textWrapper = new Wrappers.TextWrapper ();
			Wrappers.ParagraphWrapper paraWrapper = new Wrappers.ParagraphWrapper ();
			
			RosettaTest.CreateTextContext (out context);
			RosettaTest.CreateEmptyTextStoryAndNavigator (context, out story, out navigator);
			
			textWrapper.Attach (navigator);
			paraWrapper.Attach (navigator);
			
			textWrapper.SuspendSynchronizations ();
			textWrapper.Defined.FontFace = "Times New Roman";
			textWrapper.Defined.FontStyle = "Regular";
			textWrapper.Defined.FontSize = 12.0;
			textWrapper.Defined.Units = Properties.SizeUnits.Points;
			textWrapper.ResumeSynchronizations ();
			
			navigator.Insert ("Hello you world !");
			navigator.Insert (Unicode.Code.ParagraphSeparator);
			navigator.Insert ("The End.");
			navigator.MoveTo (TextNavigator.Target.LineStart, 0);
			navigator.MoveTo (TextNavigator.Target.CharacterPrevious, 1);
			navigator.MoveTo (TextNavigator.Target.WordStart, 2);
			
			textWrapper.SuspendSynchronizations ();
			textWrapper.Defined.InvertItalic = true;
			textWrapper.ResumeSynchronizations ();
			
			navigator.Insert ("wonderful ");
			
			textWrapper.SuspendSynchronizations ();
			textWrapper.Defined.ClearInvertItalic ();
			textWrapper.ResumeSynchronizations ();
			
			navigator.Insert ("and beautiful ");
			
			navigator.MoveTo(0,0) ;

			while (true)
			{
				int runLength = navigator.GetRunLength (1000000);

				if (runLength == 0)
				{
					break ;
				}
				
				string      runText       = navigator.ReadText (runLength);
				
				navigator.MoveTo (TextNavigator.Target.CharacterNext, runLength);
				
				Property[]  runProperties = navigator.AccumulatedTextProperties;
				TextStyle[] runStyles     = navigator.TextStyles;
				
				System.Console.Out.WriteLine ("Run: >>{0}<< with {1} properties", runText, runProperties.Length);
				
				foreach (Property p in runProperties)
				{
					if (p.WellKnownType == Properties.WellKnownType.Font)
					{
						Properties.FontProperty fontProperty = p as Properties.FontProperty;
						
						string fontFace = fontProperty.FaceName;
						string fontStyle = fontProperty.StyleName;
						
						fontStyle = OpenType.FontCollection.GetStyleHash (fontStyle);
						
						System.Console.Out.WriteLine ("- Font: {0} {1}", fontFace, fontStyle);
					}
					else
					{
//-						System.Console.Out.WriteLine ("- {0}", p.WellKnownType);
					}
				}
			}
		
			
//-			string ctml = "<run>Hello, </run><run><rundef><b/></rundef>world</run>.";
//-			string html = "Hello, <b>world</b>.";
			
//-			Assert.AreEqual (rosetta.ConvertCtmlToHtml (ctml), html);
		}
	}
}
