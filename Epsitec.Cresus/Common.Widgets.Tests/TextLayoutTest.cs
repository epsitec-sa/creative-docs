using NUnit.Framework;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class TextLayoutTest
	{
		[Test] public void CheckPaint()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(300, 200);
			window.Text = "CheckPaint";
			window.Root.PaintForeground += new PaintEventHandler(CheckPaint_Paint1);
			window.Root.Invalidate();
			window.Show();
		}

		private void CheckPaint_Paint1(object sender, PaintEventArgs e)
		{
			TextLayout layout = new TextLayout();

//?			layout.Text = "Ceci est un petit texte ridicule, juste pour essayer !";
//?			layout.Text = "Normal <b>bold <i>bold-italic </b>italic </i>normal.";
//?			layout.Text = "<b>Première ligne</b> assez longue pour nécessiter une coupure.<br/><b>Deuxième ligne</b> assez longue pour nécessiter une coupure.";
			layout.Text = @"Ceci est un <a href=""x"">petit texte <b>ridicule</b></a>, juste pour <font color=""#ff0000"">tester</font> le comportement de la <font size=""20"">classe</font> <font face=""Courier New"">TextLayout</font>, mes premiers pas en &quot;C#&quot;&nbsp;!<br/>Et voilà une image <img src=""file:images\icon.png""/> simple.";
			layout.Font = Font.GetFont("Tahoma", "Regular");
			layout.FontSize = 11.0;
			layout.Alignment = ContentAlignment.MiddleCenter;
			layout.LayoutSize = new Size(200, 100);

			Point pos = new Point(20, 20);

			e.Graphics.RotateTransform (5, 0, 0);
			e.Graphics.ScaleTransform (1.2, 1.2, 0, 0);
			e.Graphics.AddFilledRectangle(pos.X, pos.Y, layout.LayoutSize.Width, layout.LayoutSize.Height);
			e.Graphics.RenderSolid(Color.FromBrightness(1));

//?			Rectangle[] rects = layout.FindTextRange(0, 97);
			Rectangle[] rects = layout.FindTextRange(0, layout.Text.Length);
			for ( int i=0 ; i<rects.Length ; i++ )
			{
				rects[i].Offset(pos);
				e.Graphics.AddFilledRectangle(rects[i].Left, rects[i].Bottom, rects[i].Width, rects[i].Height);
				e.Graphics.RenderSolid(Color.FromRGB(0,1,0));
			}

//?			layout.Paint(pos, e.Graphics, e.ClipRectangle, Color.FromRGB(0,1,0));
			layout.Paint(pos, e.Graphics, e.ClipRectangle, Color.Empty);
		}

		[Test] public void CheckRectangle()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(300, 200);
			window.Text = "CheckRectangle";
			window.Root.PaintForeground += new PaintEventHandler(CheckRectangle_Paint1);
			window.Root.Invalidate();
			window.Show();
		}

		private void CheckRectangle_Paint1(object sender, PaintEventArgs e)
		{
			TextLayout layout = new TextLayout();

			layout.Text = @"Juste <font size=""30"">trois</font> lignes de texte <font size=""10"">(et une image <img src=""file:images\icon.png""/>)</font> pour rigoler !";
			layout.Font = Font.GetFont("Tahoma", "Regular");
			layout.FontSize = 20.0;
			layout.Alignment = ContentAlignment.MiddleCenter;
			layout.LayoutSize = new Size(200, 100);

			Point pos = new Point(50, 50);

			e.Graphics.AddFilledRectangle(pos.X, pos.Y, layout.LayoutSize.Width, layout.LayoutSize.Height);
			e.Graphics.RenderSolid(Color.FromBrightness(1));

			layout.Paint(pos, e.Graphics, e.ClipRectangle, Color.Empty);

			Rectangle bounds = layout.TotalRectangle;
			bounds.Offset(pos);
			e.Graphics.AddRectangle(bounds);
			e.Graphics.RenderSolid(Color.FromRGB(1,0,0));
		}

		[Test] public void CheckImage()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(300, 200);
			window.Text = "CheckImage";
			window.Root.PaintForeground += new PaintEventHandler(CheckImage_Paint1);
			window.Root.Invalidate ();
			window.Show();
		}
		
		[Test] public void CheckAnchorAsStaticText()
		{
			WindowFrame window = new WindowFrame ();
			window.Text = "CheckAnchorAsStaticText";
			
			window.ClientSize = new System.Drawing.Size (240, 180);
			window.Root.DockMargins = new Margins (10, 10, 10, 10);
			
			StaticText text = new StaticText ();
			text.Dock = DockStyle.Fill;
			text.Text = @"abracadabra abracadabra<br/><a href=""x"">abc <img src=""file:images\icon.png"" width=""5"" height=""4""/> def</a><br/>abracadabra abracadabra <a href=""y"">bla bla bla&nbsp;!</a>";
			text.Parent = window.Root;
			text.BackColor = Color.FromRGB (1.0, 1.0, 1.0);
			
			window.Show ();
		}

		private void CheckImage_Paint1(object sender, PaintEventArgs e)
		{
			TextLayout layout = new TextLayout();

			layout.Text = @"abracadabra abracadabra<br/><a href=""x"">abc <img src=""file:images\icon.png"" width=""5"" height=""4""/> def</a><br/>abracadabra abracadabra <a href=""y"">bla bla bla&nbsp;!</a>";
			layout.Font = Font.GetFont("Tahoma", "Regular");
			layout.FontSize = 15.0;
			layout.Alignment = ContentAlignment.MiddleCenter;
			layout.LayoutSize = new Size(200, 100);

			Point pos = new Point(50, 50);

			e.Graphics.AddFilledRectangle(pos.X, pos.Y, layout.LayoutSize.Width, layout.LayoutSize.Height);
			e.Graphics.RenderSolid(Color.FromBrightness(1));

			layout.Paint(pos, e.Graphics, e.ClipRectangle, Color.Empty);
		}

		[Test] public void CheckJustif()
		{
			TextLayout layout = this.NewTextLayout();

			int n = System.Convert.ToByte("17", 16);

			layout.Text = "Normal, <b>gras</b>, <i><i>italique</i></i>, <b><i>italique+gras</b></i>, <font face=\"Arial\">arial <font size=\"20\">grand</font></font>, fin";
			layout.JustifConsoleOut();

			layout.Text = "<font size=\"12\">Premier deuxième troisième quatrième cinquième sixième septième huitième neuvième et dixième.</font>";
			layout.JustifConsoleOut();

			layout.Text = "Ceciestuntrèslongtextesansespacesjustepourvoir.";
			layout.JustifConsoleOut();
		}

		
		[Test] public void CheckBasicLayout()
		{
			TextLayout layout = this.NewTextLayout();
			
			Assertion.Assert(layout.TotalLineCount > 0);
			Assertion.Assert(layout.VisibleLineCount > 0);
			Assertion.Assert(layout.VisibleLineCount <= layout.TotalLineCount);
			
			Assertion.AssertEquals(layout.TotalRectangle.IsEmpty, false);
			Assertion.Assert(layout.TotalRectangle.Width <= layout.LayoutSize.Width);
			Assertion.Assert(layout.TotalRectangle.Width >= layout.FontSize);
			Assertion.Assert(layout.TotalRectangle.Height >= layout.FontSize);
			
			Assertion.AssertEquals(layout.VisibleRectangle.IsEmpty, false);
			Assertion.Assert(layout.VisibleRectangle.Width <= layout.LayoutSize.Width);
			Assertion.Assert(layout.VisibleRectangle.Width >= layout.FontSize);
			Assertion.Assert(layout.VisibleRectangle.Height >= layout.FontSize);
			Assertion.Assert(layout.VisibleRectangle.Height <= layout.TotalRectangle.Height);
		}
		
		[Test] public void CheckTextConversions()
		{
			string t11 = "(a < b) && (b > c)\nSecond line";
			string t12 = "(a &lt; b) &amp;&amp; (b &gt; c)<br/>Second line";
			string t13 = TextLayout.ConvertToTaggedText(t11);
			string t14 = TextLayout.ConvertToSimpleText(t13);
			Assertion.AssertEquals(t12, t13);
			Assertion.AssertEquals(t11, t14);

			string t21 = "Hel&lo && <bye>";
			string t22 = "Hel<m>l</m>o &amp; &lt;bye&gt;";
			string t23 = TextLayout.ConvertToTaggedText(t21, true);
			string t24 = TextLayout.ConvertToSimpleText(t23);
			string t25 = "Hello & <bye>";
			Assertion.AssertEquals(t22, t23);
			Assertion.AssertEquals(t25, t24);

			string t36 = "This is an image <img src=\"x.jpg\" width=\"5\" height=\"4\"/> !";
			string t37 = "This is an image xyz !";
			string t38 = TextLayout.ConvertToSimpleText(t36, "xyz");
			Assertion.AssertEquals(t37, t38);
		}
		
		[Test] public void CheckMnemonic()
		{
			Assertion.AssertEquals('L', TextLayout.ExtractMnemonic("Hel<m>l</m>o"));
			Assertion.AssertEquals('\0', TextLayout.ExtractMnemonic("Hello"));
		}
		
		[Test] public void CheckTextManipulation()
		{
			TextLayout layout = this.NewTextLayout();
			
			string reference = "Link, Bold text, normal text, italic text...\nAnd some <more> text,  nice & clean.";
			string text = TextLayout.ConvertToSimpleText(layout.Text);
			Assertion.AssertEquals(reference, text);

			Assertion.AssertEquals(12, layout.FindOffsetFromIndex(0));
			Assertion.AssertEquals(13, layout.FindOffsetFromIndex(1));
			Assertion.AssertEquals(20, layout.FindOffsetFromIndex(4));
			Assertion.AssertEquals(21, layout.FindOffsetFromIndex(5));
			Assertion.AssertEquals(74, layout.FindOffsetFromIndex(44));
			Assertion.AssertEquals(79, layout.FindOffsetFromIndex(45));
			Assertion.AssertEquals(88, layout.FindOffsetFromIndex(54));
			Assertion.AssertEquals(92, layout.FindOffsetFromIndex(55));
//?			Assertion.AssertEquals(layout.Text.Length, layout.FindOffsetFromIndex(reference.Length));
			
			Assertion.AssertEquals(0,  layout.FindIndexFromOffset(12));
			Assertion.AssertEquals(1,  layout.FindIndexFromOffset(13));
			Assertion.AssertEquals(4,  layout.FindIndexFromOffset(16));
			Assertion.AssertEquals(4,  layout.FindIndexFromOffset(20));
			Assertion.AssertEquals(5,  layout.FindIndexFromOffset(21));
			Assertion.AssertEquals(44, layout.FindIndexFromOffset(74));
			Assertion.AssertEquals(45, layout.FindIndexFromOffset(79));
			Assertion.AssertEquals(54, layout.FindIndexFromOffset(88));
			Assertion.AssertEquals(55, layout.FindIndexFromOffset(92));
//?			Assertion.AssertEquals(reference.Length, layout.FindIndexFromOffset(layout.Text.Length));
		}
		
		[Test] public void CheckMetaChar()
		{
			int index = 0;
			string text = "A&lt;&Amp;&GT;.&quot;&nbsp;";
			
			Assertion.AssertEquals('A', TextLayout.AnalyseMetaChar(text, ref index));
			Assertion.AssertEquals('<', TextLayout.AnalyseMetaChar(text, ref index));
			Assertion.AssertEquals('&', TextLayout.AnalyseMetaChar(text, ref index));
			Assertion.AssertEquals('>', TextLayout.AnalyseMetaChar(text, ref index));
			Assertion.AssertEquals('.', TextLayout.AnalyseMetaChar(text, ref index));
			Assertion.AssertEquals('"', TextLayout.AnalyseMetaChar(text, ref index));
			Assertion.AssertEquals(160, TextLayout.AnalyseMetaChar(text, ref index));
			Assertion.AssertEquals(text.Length, index);
		}
		
		[Test] public void CheckAnchor()
		{
			TextLayout layout = this.NewTextLayout();
			layout.Text = "<a href=\"x\">Link</a>";
			Rectangle[] rects = layout.FindTextRange(0, 20);
			Assertion.AssertNotNull(rects);
			Assertion.AssertEquals(1, rects.Length);
			
			double x = (rects[0].Left + rects[0].Right) / 2;
			double y = (rects[0].Bottom + rects[0].Top) / 2;
			string anchor = layout.DetectAnchor(new Point(x, y));
			Assertion.AssertNotNull(anchor);
			Assertion.AssertEquals("x", anchor);
		}
		
		[Test] public void CheckAnalyseTagsAtOffset()
		{
			TextLayout layout = this.NewTextLayout();
			string[] tags;
			layout.AnalyseTagsAtOffset(30, out tags);
			Assertion.AssertEquals(tags.Length, 1);
			Assertion.AssertEquals(tags[0], "<b>");

			layout.Text = "<b><u>abc</u></b> <b><i>abc</i></b> abc";
			layout.AnalyseTagsAtOffset(6, out tags);
			Assertion.AssertEquals(tags.Length, 2);
			Assertion.AssertEquals(tags[0], "<b>");
			Assertion.AssertEquals(tags[1], "<u>");
			layout.AnalyseTagsAtOffset(24, out tags);
			Assertion.AssertEquals(tags.Length, 2);
			Assertion.AssertEquals(tags[0], "<b>");
			Assertion.AssertEquals(tags[1], "<i>");
			layout.AnalyseTagsAtOffset(27, out tags);
			Assertion.AssertEquals(tags.Length, 2);
			Assertion.AssertEquals(tags[0], "<b>");
			Assertion.AssertEquals(tags[1], "<i>");
			layout.AnalyseTagsAtOffset(31, out tags);
			Assertion.AssertEquals(tags.Length, 1);
			Assertion.AssertEquals(tags[0], "<b>");
			layout.AnalyseTagsAtOffset(35, out tags);
			Assertion.AssertEquals(tags.Length, 0);
		}

		[Test] public void CheckParseTags()
		{
			TextLayout layout = this.NewTextLayout();
			TextLayout.Tag[] expected_tags = new TextLayout.Tag[]
				{
					TextLayout.Tag.Anchor, TextLayout.Tag.AnchorEnd,
					TextLayout.Tag.Bold, TextLayout.Tag.BoldEnd,
					TextLayout.Tag.Italic, TextLayout.Tag.ItalicEnd,
					TextLayout.Tag.LineBreak
				};
			
			System.Collections.Hashtable parameters;

			int    index = 0;
			string text  = layout.Text;
			
			for ( int i=0 ; i<expected_tags.Length ; )
			{
				TextLayout.Tag tag = TextLayout.ParseTag(text, ref index, out parameters);
				
				if ( tag == TextLayout.Tag.None )
				{
					continue;
				}
				
				Assertion.AssertEquals(expected_tags[i], tag);
				i++;
			}
			
			text  = "<img src=\"x\">";
			index = 0;
			Assertion.AssertEquals(TextLayout.Tag.SyntaxError, TextLayout.ParseTag(text, ref index, out parameters));
			Assertion.AssertNull(parameters);
			Assertion.AssertEquals(text.Length, index);
			
			text  = "<img src=\"x\"/>";
			index = 0;
			Assertion.AssertEquals(TextLayout.Tag.Image, TextLayout.ParseTag(text, ref index, out parameters));
			Assertion.AssertNotNull(parameters);
			Assertion.AssertEquals("x", parameters["src"]);
			Assertion.AssertEquals(text.Length, index);
			
			text  = "<font face=\"Arial Bold\" size=\"2\" color=\"#FFCC00\">";
			index = 0;
			Assertion.AssertEquals(TextLayout.Tag.Font, TextLayout.ParseTag(text, ref index, out parameters));
			Assertion.AssertNotNull(parameters);
			Assertion.AssertEquals(parameters["face"], "Arial Bold");
			Assertion.AssertEquals(parameters["size"], "2");
			Assertion.AssertEquals(parameters["color"], "#FFCC00");
			Assertion.AssertEquals(text.Length, index);
		}
		
		[Test] public void CheckCheckSyntax()
		{
			int offsetError;

			// Textes tordus mais corrects.
			Assertion.Assert(TextLayout.CheckSyntax("<a href=\"x\">Link</a>", out offsetError));
			Assertion.Assert(TextLayout.CheckSyntax("<b><i></B></I>", out offsetError));
			Assertion.Assert(TextLayout.CheckSyntax("Première<BR/>Deuxième", out offsetError));
			Assertion.Assert(TextLayout.CheckSyntax("<img src=\"x\"/>", out offsetError));
			Assertion.Assert(TextLayout.CheckSyntax("A&lt;&Amp;&GT;.&quot;&nbsp;", out offsetError));

			// Textes faux qui doivent être rejetés.
			Assertion.Assert(!TextLayout.CheckSyntax("<bold", out offsetError));
			Assertion.Assert(!TextLayout.CheckSyntax("&quot", out offsetError));
			Assertion.Assert(!TextLayout.CheckSyntax("<b>bold", out offsetError));
			Assertion.Assert(!TextLayout.CheckSyntax("bold</b>", out offsetError));
			Assertion.Assert(!TextLayout.CheckSyntax("Première<br>Deuxième", out offsetError));
			Assertion.Assert(!TextLayout.CheckSyntax("<img src=\"x\">", out offsetError));
			Assertion.Assert(!TextLayout.CheckSyntax("ab&quott;cd", out offsetError));
			Assertion.Assert(!TextLayout.CheckSyntax("<x>", out offsetError));
		}
			
		private TextLayout NewTextLayout()
		{
			TextLayout layout = new TextLayout();
			
			layout.Text       = @"<a href=""x"">Link</a>, <b>Bold text</b>, normal text, <i>italic text</i>...<br/>And some &lt;more&gt; text, <img src=""file:images\icon.png""/> nice &amp; clean.";
			layout.Font       = Font.GetFont("Tahoma", "Regular");
			layout.FontSize   = 11.0;
			layout.LayoutSize = new Size(100, 50);
			
			return layout;
		}
	}
}
