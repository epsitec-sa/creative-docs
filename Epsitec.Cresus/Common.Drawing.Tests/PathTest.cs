using NUnit.Framework;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Drawing
{
	[TestFixture]
	public class PathTest
	{
		[Test] public void CheckAppendPaths()
		{
			Path p1  = new Path ();
			Path p1r = new Path ();
			Path p2  = new Path ();
			
			p1.MoveTo (10, 10);
			p1.LineTo (50, 10);
			p1.LineTo (30, 60);
			p1.Close ();
			
			p1r.MoveTo (10, 10);
			p1r.LineTo (30, 60);
			p1r.LineTo (50, 10);
			p1r.Close ();
			
			p2.MoveTo (20, 5);
			p2.LineTo (60, 0);
			p2.LineTo (15, 40);
			p2.Close ();
			
			Rectangle r1 = p1.ComputeBounds ();
			Rectangle r2 = p2.ComputeBounds ();
			Rectangle r  = Rectangle.Union (r1, r2);
			
			System.Console.Out.WriteLine ("Path1:");
			System.Console.Out.WriteLine (p1.ToString ());
			System.Console.Out.WriteLine ("Path2:");
			System.Console.Out.WriteLine (p2.ToString ());
			
			Path p = new Path ();
			
			p.Append (p1);
			Assertion.AssertEquals (r1, p.ComputeBounds ());
			
			System.Console.Out.WriteLine ("Path: p1");
			System.Console.Out.WriteLine (p.ToString ());
			
			p.Append (p2);
			
			System.Console.Out.WriteLine ("Path: p1 UNION p2");
			System.Console.Out.WriteLine (p.ToString ());
			
			Assertion.AssertEquals (r, p.ComputeBounds ());
			
			Path pp1 = new Path();
			Path pp2 = new Path();
			
			pp1.Append(p1,  1, 2);
			pp2.Append(p1r, 1, 2);
			
			System.Console.Out.WriteLine ("Path: direct, bold 2");
			System.Console.Out.WriteLine (pp1.ToString ());
			
			System.Console.Out.WriteLine ("Path: reversed, bold 2");
			System.Console.Out.WriteLine (pp2.ToString ());
		}
		
		[Test] public void CheckAppendGlyph()
		{
			Font font = Font.GetFont ("Times New Roman", "Italic");
			Path path = new Path ();
			
			int glyph = font.GetGlyphIndex ('f');
			
			path.Append (font, glyph, 0, 0, 12.0);
			
			System.Console.Out.WriteLine ("Glyph path:");
			System.Console.Out.WriteLine (path.ToString ());
			
			Rectangle rp = path.ComputeBounds ();
			Rectangle rf = font.GetGlyphBounds (glyph);
			
			rf.Scale (12.0);
			
			Assertion.AssertEquals (rp, rf);
		}
		
		[Test] public void CheckGlyphPathToStringTimes()
		{
			Font font = Font.GetFont ("Times New Roman", "Regular");
			Path path = new Path ();
			
			int glyph = font.GetGlyphIndex ('a');
			
			path.Append (font, glyph, 0, 0, 100.0);
			
			System.Console.Out.WriteLine (path.ToString ());
		}
		
		[Test] public void CheckGlyphPathToStringTahoma()
		{
			Font font = Font.GetFont ("Tahoma", "Regular");
			Path path = new Path ();
			
			int glyph = font.GetGlyphIndex ('i');
			
			path.Append (font, glyph, 0, 0, 100.0);
			
			System.Console.Out.WriteLine (path.ToString ());
		}
		
		[Test] public void CheckComputeBounds()
		{
			Path path = new Path ();
			Rectangle bounds;
			
			path.MoveTo (110, 110);
			path.LineTo (110, 210);
			path.LineTo (210, 160);
			path.Close ();
			
			bounds = path.ComputeBounds ();
			Assertion.AssertEquals (110, bounds.Left);
			Assertion.AssertEquals (110, bounds.Bottom);
			Assertion.AssertEquals (210, bounds.Right);
			Assertion.AssertEquals (210, bounds.Top);
			
			path.MoveTo (120, 125);
			path.LineTo (195, 160);
			path.LineTo (120, 195);
			path.Close ();
			
			bounds = path.ComputeBounds ();
			Assertion.AssertEquals (110, bounds.Left);
			Assertion.AssertEquals (110, bounds.Bottom);
			Assertion.AssertEquals (210, bounds.Right);
			Assertion.AssertEquals (210, bounds.Top);
		}
	}
}
