using NUnit.Framework;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class TextLayoutTest
	{
		[Test] public void CheckBasicLayout()
		{
			TextLayout layout = this.NewTextLayout ();
			
			Assertion.Assert (layout.TotalLineCount > 0);
			Assertion.Assert (layout.VisibleLineCount > 0);
			Assertion.Assert (layout.VisibleLineCount <= layout.TotalLineCount);
			
			Assertion.AssertEquals (false, layout.TotalRectangle.IsEmpty);
			Assertion.Assert (layout.TotalRectangle.Width <= layout.LayoutSize.Width);
			Assertion.Assert (layout.TotalRectangle.Width >= layout.FontSize);
			Assertion.Assert (layout.TotalRectangle.Height >= layout.FontSize);
			
			Assertion.AssertEquals (false, layout.VisibleRectangle.IsEmpty);
			Assertion.Assert (layout.VisibleRectangle.Width <= layout.LayoutSize.Width);
			Assertion.Assert (layout.VisibleRectangle.Width >= layout.FontSize);
			Assertion.Assert (layout.VisibleRectangle.Height >= layout.FontSize);
			Assertion.Assert (layout.VisibleRectangle.Height <= layout.TotalRectangle.Height);
		}
		
		[Test] public void CheckTextManipulation()
		{
			TextLayout layout = this.NewTextLayout ();
			
			string reference = "Link, Bold text, normal text, italic text...\nAnd some <more> text, nice & clean.";
			string text = layout.CleanText;
			int pos_0   = layout.FindOffsetFromIndex (0);
			int pos_1   = layout.FindOffsetFromIndex (1);
			int pos_4   = layout.FindOffsetFromIndex (4);
			int pos_end = layout.FindOffsetFromIndex (reference.Length);
			
			Assertion.AssertEquals (reference, text);
			Assertion.AssertEquals (12, pos_0);
			Assertion.AssertEquals (13, pos_1);
			Assertion.AssertEquals (20, pos_4);
			Assertion.AssertEquals (layout.Text.Length, pos_end);
			
			Assertion.AssertEquals (pos_0,   layout.FindIndexFromOffset (12));
			Assertion.AssertEquals (pos_1,   layout.FindIndexFromOffset (13));
			Assertion.AssertEquals (pos_4,   layout.FindIndexFromOffset (20));
			Assertion.AssertEquals (pos_end, layout.FindIndexFromOffset (layout.Text.Length));
		}
		
		[Test] public void CheckMetaChar()
		{
			int index = 0;
			string text = "A&lt;&Amp;&GT;.&quot;&nbsp;";
			
			Assertion.AssertEquals ('A', TextLayout.AnalyseMetaChar (text, ref index));
			Assertion.AssertEquals ('<', TextLayout.AnalyseMetaChar (text, ref index));
			Assertion.AssertEquals ('&', TextLayout.AnalyseMetaChar (text, ref index));
			Assertion.AssertEquals ('>', TextLayout.AnalyseMetaChar (text, ref index));
			Assertion.AssertEquals ('.', TextLayout.AnalyseMetaChar (text, ref index));
			Assertion.AssertEquals ('"', TextLayout.AnalyseMetaChar (text, ref index));
			Assertion.AssertEquals (160, TextLayout.AnalyseMetaChar (text, ref index));
			Assertion.AssertEquals (text.Length, index);
		}
		
		[Test] public void CheckAnchor()
		{
			TextLayout layout = this.NewTextLayout ();
			Rectangle[] rects = layout.FindTextRange (0, 4);
			
			Assertion.AssertNotNull (rects);
			Assertion.AssertEquals (1, rects.Length);
			
			double x = (rects[0].Left + rects[0].Right) / 2;
			double y = (rects[0].Bottom + rects[0].Top) / 2;
			
			string anchor = layout.DetectAnchor (new Point (x, y));
			
			Assertion.AssertNotNull (anchor);
			Assertion.AssertEquals ("x", anchor);
		}
		
		private TextLayout NewTextLayout()
		{
			TextLayout layout = new TextLayout ();
			
			layout.Text       = "<a href='x'>Link</a>, <b>Bold text</b>, normal text, <i>italic text</i>...<br/>And some &lt;more&gt; text, nice &amp; clean.";
			layout.Font       = Font.GetFont ("Tahoma", "Regular");
			layout.FontSize   = 11.0;
			layout.LayoutSize = new Size (100, 50);
			
			return layout;
		}
	}
}
