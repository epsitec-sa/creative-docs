//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Michael WALZ

using NUnit.Framework;
using Epsitec.Common.Text;
using Epsitec.Common.Text.Properties;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text.Exchange;
using Epsitec.Common.Text.Wrappers;

namespace Epsitec.Common.Tests.Text.Exchange
{
	[TestFixture] 
	public class RosettaTest
	{
		[SetUp] public void Intialize()
		{
			Widgets.Widget.Initialize ();
		}
			
		
		private static void CreateTextContext(out TextContext context)
		{
			context = new TextContext ();
			
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			string black = RichColor.ToString (RichColor.FromBrightness (0));
			
			properties.Add (new FontProperty ("Arial", "Regular", "kern", "liga"));
			properties.Add (new FontSizeProperty (12.0, SizeUnits.Points, 0.0));
			properties.Add (new MarginsProperty (0, 0, 0, 0, SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, ThreeState.True, 0, null));
			properties.Add (new FontColorProperty (black));
			properties.Add (new LanguageProperty (System.Globalization.CultureInfo.CurrentCulture.Name, 1.0));
			properties.Add (new LeadingProperty (1.0, SizeUnits.PercentNotCombining, 0.0, SizeUnits.Points, 0.0, SizeUnits.Points, AlignMode.None));
			properties.Add (new KeepProperty (2, 2, ParagraphStartMode.Anywhere, ThreeState.False, ThreeState.False));
			
			Epsitec.Common.Text.TextStyle paraStyle = context.StyleList.NewTextStyle (null, "Default", TextStyleClass.Paragraph, properties);
			Epsitec.Common.Text.TextStyle charStyle = context.StyleList.NewTextStyle (null, "Default", TextStyleClass.Text);
			
			context.DefaultParagraphStyle = paraStyle;
			context.StyleList.StyleMap.SetRank (null, paraStyle, 0);
			context.StyleList.StyleMap.SetCaption (null, paraStyle, "Normal");

			context.DefaultTextStyle = charStyle;
			context.StyleList.StyleMap.SetRank (null, charStyle, 0);
			context.StyleList.StyleMap.SetCaption (null, charStyle, "Caractères par défaut");
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
			
			TextWrapper      textWrapper = new TextWrapper ();
			ParagraphWrapper paraWrapper = new ParagraphWrapper ();
			
			RosettaTest.CreateTextContext (out context);
			RosettaTest.CreateEmptyTextStoryAndNavigator (context, out story, out navigator);
			
			textWrapper.Attach (navigator);
			paraWrapper.Attach (navigator);
			
			textWrapper.SuspendSynchronizations ();
			textWrapper.Defined.FontFace = "Times New Roman";
			textWrapper.Defined.FontStyle = "Regular";
			textWrapper.Defined.FontSize = 12.0;
			textWrapper.Defined.Units = SizeUnits.Points;
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
				Epsitec.Common.Text.TextStyle[] runStyles     = navigator.TextStyles;
				
				System.Console.Out.WriteLine ("Run: >>{0}<< with {1} properties", runText, runProperties.Length);
				
				foreach (Property p in runProperties)
				{
					if (p.WellKnownType == WellKnownType.Font)
					{
						FontProperty fontProperty = p as FontProperty;
						
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
				
				//	La façon "haut niveau" de faire :
				
				if (textWrapper.Defined.IsFontFaceDefined)
				{
					System.Console.Out.WriteLine ("- Font Face: {0}", textWrapper.Defined.FontFace, textWrapper.Defined.FontStyle, textWrapper.Defined.InvertItalic ? "(italic)" : "");
				}
				if (textWrapper.Defined.IsFontStyleDefined)
				{
					System.Console.Out.WriteLine ("- Font Style: {0}", textWrapper.Defined.FontStyle);
				}
				if (textWrapper.Defined.IsInvertItalicDefined)
				{
					System.Console.Out.WriteLine ("- Invert Italic: {0}", textWrapper.Defined.InvertItalic);
				}
				if (textWrapper.Defined.IsInvertBoldDefined)
				{
					System.Console.Out.WriteLine ("- Invert Bold: {0}", textWrapper.Defined.InvertBold);
				}
			}
		
					}
	}
}
