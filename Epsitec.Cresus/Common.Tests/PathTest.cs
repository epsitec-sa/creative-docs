using System;
using NUnit.Framework;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class PathTest
	{
		[Test] public void CheckAppendPaths()
		{
			Path p1 = new Path ();
			Path p2 = new Path ();
			
			p1.MoveTo (10, 10);
			p1.LineTo (50, 10);
			p1.LineTo (30, 60);
			p1.Close ();
			
			p2.MoveTo (20, 5);
			p2.LineTo (60, 0);
			p2.LineTo (15, 40);
			p2.Close ();
			
			Rectangle r1 = p1.ComputeBounds ();
			Rectangle r2 = p2.ComputeBounds ();
			Rectangle r  = Rectangle.Union (r1, r2);
			
			Path p = new Path ();
			
			p.Append (p1);
			Assertion.AssertEquals (r1, p.ComputeBounds ());
			
			p.Append (p2);
			Assertion.AssertEquals (r, p.ComputeBounds ());
		}
		
		[Test] public void CheckAppendGlyph()
		{
			Font font = Font.GetFont ("Times New Roman", "Italic");
			Path path = new Path ();
			
			int glyph = font.GetGlyphIndex ('f');
			
			path.Append (font, glyph, 0, 0, 12.0);
			
			Rectangle rp = path.ComputeBounds ();
			Rectangle rf = font.GetGlyphBounds (glyph);
			
			rf.Scale (12.0);
			
			Assertion.AssertEquals (rp, rf);
		}
		
		[Test] public void CheckGlyphPathToString()
		{
			Font font = Font.GetFont ("Times New Roman", "Regular");
			Path path = new Path ();
			
			int glyph = font.GetGlyphIndex ('a');
			
			path.Append (font, glyph, 0, 0, 100.0);
			
			System.Console.Out.WriteLine (path.ToString ());
		}
	}
}
