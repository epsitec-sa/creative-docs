using NUnit.Framework;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class FontTests
	{
		public FontTests()
		{
			Font.Initialise ();
		}
		[Test] public void CheckInit()
		{
			int n = Font.Count;
			System.Console.Out.WriteLine (n.ToString () + " fonts found");
			
			for (int i = 0; i < n; i++)
			{
				Font font = Font.GetFont (i);
				Assertion.AssertNotNull (font);
				
				if (font.GetGlyphIndex ('e') == 0)
				{
					System.Console.WriteLine (font.FullName + " (" + font.LocalStyleName + ") is not a Latin font");
				}
				else
				{
					System.Console.Out.WriteLine (font.FullName + " (" + font.LocalStyleName + ") / e=" + font.GetGlyphIndex ('e').ToString ());
					System.Console.Out.WriteLine ("  A=" + font.Ascender + " D=" + font.Descender + " H=" + font.LineHeight + " w=" + font.GetCharAdvance ('e'));
				}
				Font find = Font.GetFont (font.FaceName, font.StyleName, font.OpticalName);
				
				Assertion.AssertNotNull (find);
				Assertion.Assert (find.Handle == font.Handle);
			}
			
			Assertion.AssertNull (Font.GetFont (n+1));
		}
	}
}
