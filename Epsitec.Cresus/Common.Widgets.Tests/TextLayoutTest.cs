using NUnit.Framework;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	[TestFixture]
	public class TextLayoutTest
	{
		[Test] public void CheckPaint()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(300, 200);
			window.Text = "CheckPaint";
			window.Root.PaintForeground += new PaintEventHandler(CheckPaint_Paint1);
			window.Root.Invalidate();
			window.Show();
		}

		[Test] public void CheckPaintJustif()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(300, 200);
			window.Text = "CheckPaintJustif";
			window.Root.PaintForeground += new PaintEventHandler(CheckPaint_PaintJustif1);
			window.Root.Invalidate();
			window.Show();
		}

		[Test] public void CheckPaintUnderline()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(300, 200);
			window.Text = "CheckPaintUnderline";
			window.Root.PaintForeground += new PaintEventHandler(CheckPaint_PaintUnderline1);
			window.Root.Invalidate();
			window.Show();
		}

		[Test] public void CheckPaintWave()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(300, 200);
			window.Text = "CheckPaintWave";
			window.Root.PaintForeground += new PaintEventHandler(CheckPaint_PaintWave1);
			window.Root.Invalidate();
			window.Show();
		}

		[Test] public void CheckRectangle()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(300, 200);
			window.Text = "CheckRectangle";
			window.Root.PaintForeground += new PaintEventHandler(CheckRectangle_Paint1);
			window.Root.Invalidate();
			window.Show();
		}

		[Test] public void CheckImage()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(300, 200);
			window.Text = "CheckImage";
			window.Root.PaintForeground += new PaintEventHandler(CheckImage_Paint1);
			window.Root.Invalidate ();
			window.Show();
		}
		
		[Test] public void CheckAnchorAsStaticText()
		{
			Window window = new Window ();
			window.Text = "CheckAnchorAsStaticText";
			
			window.ClientSize = new Size (240, 180);
			window.Root.DockPadding = new Margins (10, 10, 10, 10);
			
			StaticText text = new StaticText ();
			text.Dock = DockStyle.Fill;
			text.Text = @"abracadabra abracadabra<br/><a href=""x"">abc <img src=""file:images/icon.png"" width=""5"" height=""4""/> def</a><br/>abracadabra abracadabra <a href=""y"">bla bla bla&#160;!</a>";
			text.Parent = window.Root;
			text.BackColor = Color.FromRGB (1.0, 1.0, 1.0);
			
			window.Show ();
		}

		[Test] public void CheckJustif()
		{
			TextLayout layout = this.NewTextLayout();

			int n = System.Convert.ToByte("17", 16);

			layout.Text = "Normal, <b>gras</b>, <i><i>italique</i></i>, <b><i>italique+gras</b></i>, <font face=\"Arial\">arial <font size=\"20\">grand</font></font>, fin";
			layout.DebugDumpJustif(System.Console.Out);

			layout.Text = "<font size=\"12\">Premier deuxi�me troisi�me quatri�me cinqui�me sixi�me septi�me huiti�me neuvi�me et dixi�me.</font>";
			layout.DebugDumpJustif(System.Console.Out);

			layout.Text = "Ceciestuntr�slongtextesansespacesjustepourvoir.";
			layout.DebugDumpJustif(System.Console.Out);

			layout.Text = "1.<br/><br/><b><i>3.</b></i><br/><br/>5.<br/><br/>";
			layout.DebugDumpJustif(System.Console.Out);

			layout.Text = "Juste quelques lignes termin�es par un br.<br/>";
			layout.DebugDumpJustif(System.Console.Out);
		}

		
		[Test] public void CheckBasicLayout()
		{
			TextLayout layout = this.NewTextLayout();
			
			Assert.IsTrue(layout.TotalLineCount > 0);
			Assert.IsTrue(layout.VisibleLineCount > 0);
			Assert.IsTrue(layout.VisibleLineCount <= layout.TotalLineCount);
			
			Assert.AreEqual(layout.TotalRectangle.IsEmpty, false);
			Assert.IsTrue(layout.TotalRectangle.Width <= layout.LayoutSize.Width);
			Assert.IsTrue(layout.TotalRectangle.Width >= layout.FontSize);
			Assert.IsTrue(layout.TotalRectangle.Height >= layout.FontSize);
			
			Assert.AreEqual(layout.VisibleRectangle.IsEmpty, false);
			Assert.IsTrue(layout.VisibleRectangle.Width <= layout.LayoutSize.Width);
			Assert.IsTrue(layout.VisibleRectangle.Width >= layout.FontSize);
			Assert.IsTrue(layout.VisibleRectangle.Height >= layout.FontSize);
			Assert.IsTrue(layout.VisibleRectangle.Height <= layout.TotalRectangle.Height);
		}
		
		[Test] public void CheckTextConversions()
		{
			string t11 = "(a < b) && (b > c)\nSecond line";
			string t12 = "(a &lt; b) &amp;&amp; (b &gt; c)<br/>Second line";
			string t13 = TextLayout.ConvertToTaggedText(t11);
			string t14 = TextLayout.ConvertToSimpleText(t13);
			Assert.AreEqual(t12, t13);
			Assert.AreEqual(t11, t14);

			string t21 = "Hel&lo && <bye>";
			string t22 = "Hel<m>l</m>o &amp; &lt;bye&gt;";
			string t23 = TextLayout.ConvertToTaggedText(t21, true);
			string t24 = TextLayout.ConvertToSimpleText(t23);
			string t25 = "Hello & <bye>";
			Assert.AreEqual(t22, t23);
			Assert.AreEqual(t25, t24);

			string t36 = "This is an image <img src=\"x.jpg\" width=\"5\" height=\"4\"/> !";
			string t37 = "This is an image xyz !";
			string t38 = TextLayout.ConvertToSimpleText(t36, "xyz");
			Assert.AreEqual(t37, t38);
		}
		
		[Test] public void CheckMnemonic()
		{
			Assert.AreEqual('L', TextLayout.ExtractMnemonic("Hel<m>l</m>o"));
			Assert.AreEqual('\0', TextLayout.ExtractMnemonic("Hello"));
		}
		
		[Test] public void CheckTextManipulation()
		{
			TextLayout layout = this.NewTextLayout();
			
			string reference = "Link, Bold text, normal text, italic text...\nAnd some <more> text, \ufffc nice & clean.";
			string text = TextLayout.ConvertToSimpleText(layout.Text);
			Assert.AreEqual(reference, text);

			Assert.AreEqual(12, layout.FindOffsetFromIndex(0));
			Assert.AreEqual(13, layout.FindOffsetFromIndex(1));
			Assert.AreEqual(20, layout.FindOffsetFromIndex(4));
			Assert.AreEqual(21, layout.FindOffsetFromIndex(5));
			Assert.AreEqual(74, layout.FindOffsetFromIndex(44));
			Assert.AreEqual(79, layout.FindOffsetFromIndex(45));
			Assert.AreEqual(88, layout.FindOffsetFromIndex(54));
			Assert.AreEqual(92, layout.FindOffsetFromIndex(55));
//?			Assert.AreEqual(layout.Text.Length, layout.FindOffsetFromIndex(reference.Length));
			
			Assert.AreEqual(0,  layout.FindIndexFromOffset(12));
			Assert.AreEqual(1,  layout.FindIndexFromOffset(13));
			Assert.AreEqual(4,  layout.FindIndexFromOffset(16));
			Assert.AreEqual(4,  layout.FindIndexFromOffset(20));
			Assert.AreEqual(5,  layout.FindIndexFromOffset(21));
			Assert.AreEqual(44, layout.FindIndexFromOffset(74));
			Assert.AreEqual(45, layout.FindIndexFromOffset(79));
			Assert.AreEqual(54, layout.FindIndexFromOffset(88));
			Assert.AreEqual(55, layout.FindIndexFromOffset(92));
//?			Assert.AreEqual(reference.Length, layout.FindIndexFromOffset(layout.Text.Length));
		}
		
		[Test] public void CheckEntityChar()
		{
			int index = 0;
			string text = "A&lt;&amp;&gt;.&quot;&#160;";
			
			Assert.AreEqual('A', TextLayout.AnalyseEntityChar(text, ref index));
			Assert.AreEqual('<', TextLayout.AnalyseEntityChar(text, ref index));
			Assert.AreEqual('&', TextLayout.AnalyseEntityChar(text, ref index));
			Assert.AreEqual('>', TextLayout.AnalyseEntityChar(text, ref index));
			Assert.AreEqual('.', TextLayout.AnalyseEntityChar(text, ref index));
			Assert.AreEqual('"', TextLayout.AnalyseEntityChar(text, ref index));
			Assert.AreEqual(160, TextLayout.AnalyseEntityChar(text, ref index));
			Assert.AreEqual(text.Length, index);
		}
		
		[Test] public void CheckAnchor()
		{
			TextLayout layout = this.NewTextLayout();
			layout.Text = "<a href=\"x\">Link</a>";
			TextLayout.SelectedArea[] areas = layout.FindTextRange(new Drawing.Point (), 0, 20);
			Assert.IsNotNull(areas);
			Assert.AreEqual(1, areas.Length);
			
			double x = (areas[0].Rect.Left + areas[0].Rect.Right) / 2;
			double y = (areas[0].Rect.Bottom + areas[0].Rect.Top) / 2;
			string anchor = layout.DetectAnchor(new Point(x, y));
			Assert.IsNotNull(anchor);
			Assert.AreEqual("x", anchor);
		}
		
		[Test] public void CheckAnalyseTagsAtOffset()
		{
			TextLayout layout = this.NewTextLayout();
			string[] tags;
			layout.AnalyseTagsAtOffset(30, out tags);
			Assert.AreEqual(tags.Length, 1);
			Assert.AreEqual(tags[0], "<b>");

			layout.Text = "<b><u>abc</u></b> <b><i>abc</i></b> abc";
			layout.AnalyseTagsAtOffset(6, out tags);
			Assert.AreEqual(tags.Length, 2);
			Assert.AreEqual(tags[0], "<b>");
			Assert.AreEqual(tags[1], "<u>");
			layout.AnalyseTagsAtOffset(24, out tags);
			Assert.AreEqual(tags.Length, 2);
			Assert.AreEqual(tags[0], "<b>");
			Assert.AreEqual(tags[1], "<i>");
			layout.AnalyseTagsAtOffset(27, out tags);
			Assert.AreEqual(tags.Length, 2);
			Assert.AreEqual(tags[0], "<b>");
			Assert.AreEqual(tags[1], "<i>");
			layout.AnalyseTagsAtOffset(31, out tags);
			Assert.AreEqual(tags.Length, 1);
			Assert.AreEqual(tags[0], "<b>");
			layout.AnalyseTagsAtOffset(35, out tags);
			Assert.AreEqual(tags.Length, 0);
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
				
				Assert.AreEqual(expected_tags[i], tag);
				i++;
			}
			
			text  = "<img src=\"x\">";
			index = 0;
			Assert.AreEqual(TextLayout.Tag.SyntaxError, TextLayout.ParseTag(text, ref index, out parameters));
			Assert.IsNull(parameters);
			Assert.AreEqual(text.Length, index);
			
			text  = "<img src=\"x\"/>";
			index = 0;
			Assert.AreEqual(TextLayout.Tag.Image, TextLayout.ParseTag(text, ref index, out parameters));
			Assert.IsNotNull(parameters);
			Assert.AreEqual("x", parameters["src"]);
			Assert.AreEqual(text.Length, index);
			
			text  = "<font face=\"Arial Bold\" size=\"2\" color=\"#FFCC00\">";
			index = 0;
			Assert.AreEqual(TextLayout.Tag.Font, TextLayout.ParseTag(text, ref index, out parameters));
			Assert.IsNotNull(parameters);
			Assert.AreEqual(parameters["face"], "Arial Bold");
			Assert.AreEqual(parameters["size"], "2");
			Assert.AreEqual(parameters["color"], "#FFCC00");
			Assert.AreEqual(text.Length, index);
		}
		
		[Test] public void CheckCheckSyntax()
		{
			int offsetError;

			// Textes tordus mais corrects.
			Assert.IsTrue(TextLayout.CheckSyntax("<a href=\"x\">Link</a>", out offsetError));
			Assert.IsTrue(TextLayout.CheckSyntax("<b><i></b></i>", out offsetError));
			Assert.IsTrue(TextLayout.CheckSyntax("Premi�re<br/>Deuxi�me", out offsetError));
			Assert.IsTrue(TextLayout.CheckSyntax("<img src=\"x\"/>", out offsetError));
			Assert.IsTrue(TextLayout.CheckSyntax("A&lt;&amp;&gt;.&quot;&#160;", out offsetError));

			// Textes faux qui doivent �tre rejet�s.
			Assert.IsTrue(!TextLayout.CheckSyntax("<bold", out offsetError));
			Assert.IsTrue(!TextLayout.CheckSyntax("&quot", out offsetError));
			Assert.IsTrue(!TextLayout.CheckSyntax("<b>bold", out offsetError));
			Assert.IsTrue(!TextLayout.CheckSyntax("bold</b>", out offsetError));
			Assert.IsTrue(!TextLayout.CheckSyntax("Premi�re<br>Deuxi�me", out offsetError));
			Assert.IsTrue(!TextLayout.CheckSyntax("<img src=\"x\">", out offsetError));
			Assert.IsTrue(!TextLayout.CheckSyntax("ab&quott;cd", out offsetError));
			Assert.IsTrue(!TextLayout.CheckSyntax("<x>", out offsetError));
		}
			
		[Test] public void CheckTextBrBr()
		{
			Window window = new Window ();
			window.Text = "CheckTextBrBr";
			window.ClientSize = new Size (200, 100);
			window.Root.DockPadding = new Margins (5, 5, 5, 5);
			TextFieldMulti text = new TextFieldMulti ();
			text.Dock = DockStyle.Fill;
			//?text.Text = "1.<br/><br/>3.";
			text.Text = "1.<br/><br/><b><i>3.</b></i><br/><br/>5.<br/><br/>";
			text.Parent = window.Root;
			window.Show ();
		}
		
		private void CheckPaint_Paint1(object sender, PaintEventArgs e)
		{
			TextLayout layout = new TextLayout();

			//?			layout.Text = "Ceci est un petit texte ridicule, juste pour essayer !";
			//?			layout.Text = "Normal <b>bold <i>bold-italic </b>italic </i>normal.";
			//?			layout.Text = "<b>Premi�re ligne</b> assez longue pour n�cessiter une coupure.<br/><b>Deuxi�me ligne</b> assez longue pour n�cessiter une coupure.";
			layout.Text = @"Ceci est un <a href=""x"">petit texte <b>ridicule</b></a>, juste pour <font color=""#ff0000"">tester</font> le comportement de la <font size=""20"">classe</font> <font face=""Courier New"">TextLayout</font>, mes premiers pas en &quot;C#&quot;&#160;!<br/>Et voil� une image <img src=""file:images/icon.png""/> simple.";
			layout.Font = Font.GetFont("Tahoma", "Regular");
			layout.FontSize = 11.0;
			layout.Alignment = ContentAlignment.MiddleCenter;
			//?layout.JustifMode = TextJustifMode.All;
			layout.LayoutSize = new Size(200, 100);

			Point pos = new Point(20, 20);

			//?			e.Graphics.RotateTransform (5, 0, 0);
			e.Graphics.ScaleTransform (1.2, 1.2, 0, 0);
			e.Graphics.AddFilledRectangle(pos.X, pos.Y, layout.LayoutSize.Width, layout.LayoutSize.Height);
			e.Graphics.RenderSolid(Color.FromBrightness(1));

			TextLayout.SelectedArea[] areas = layout.FindTextRange(pos, 0, layout.Text.Length);
			for ( int i=0 ; i<areas.Length ; i++ )
			{
				e.Graphics.Align(ref areas[i].Rect);
				e.Graphics.AddFilledRectangle(areas[i].Rect.Left, areas[i].Rect.Bottom, areas[i].Rect.Width, areas[i].Rect.Height);
				e.Graphics.RenderSolid(Color.FromRGB(0,1,0));
			}

			layout.Paint(pos, e.Graphics, e.ClipRectangle, Color.Empty, GlyphPaintStyle.Normal);
		}

		private void CheckPaint_PaintJustif1(object sender, PaintEventArgs e)
		{
			TextLayout layout = new TextLayout();

			layout.Text = @"Voil� une image <img src=""file:images/icon.png""/> <w>simple</w>, suivie d'une deuxi�me <img src=""file:images/icon.png""/> et une troisi�me <img src=""file:images/icon.png""/>.<br/><br/>On donnait ce jour-l� un grand d�ner, o�, pour la premi�re fois, je vis avec beaucoup d'�tonnement le ma�tre d'h�tel servir l'�p�e au c�t� et le chapeau sur la t�te.";
			layout.Font = Font.GetFont("Tahoma", "Regular");
			layout.FontSize = 11.0;
			layout.Alignment = ContentAlignment.MiddleLeft;
			layout.JustifMode = TextJustifMode.AllButLast;
			layout.LayoutSize = new Size(260, 150);

			Point pos = new Point(20, 20);
			e.Graphics.AddFilledRectangle(pos.X, pos.Y, layout.LayoutSize.Width, layout.LayoutSize.Height);
			e.Graphics.RenderSolid(Color.FromBrightness(1));

			TextLayout.SelectedArea[] areas = layout.FindTextRange(pos, 0, layout.Text.Length);
			for ( int i=0 ; i<areas.Length ; i++ )
			{
				e.Graphics.Align(ref areas[i].Rect);
				e.Graphics.AddFilledRectangle(areas[i].Rect.Left, areas[i].Rect.Bottom, areas[i].Rect.Width, areas[i].Rect.Height);
				e.Graphics.RenderSolid(Color.FromRGB(0,1,0));
			}

			layout.Paint(pos, e.Graphics, e.ClipRectangle, Color.Empty, GlyphPaintStyle.Normal);
		}

		private void CheckPaint_PaintUnderline1(object sender, PaintEventArgs e)
		{
			TextLayout layout = new TextLayout();

			layout.Text = @"On <u>donnait</u> ce jour-l� <u>un grand d�ner</u>, o�, pour la <u>premi�re <b>fois</b></u>, je vis avec beaucoup <u>d'�tonnement le ma�tre d'h�tel servir l'�p�e au c�t� et le chapeau sur la t�te</u>.";
			layout.Font = Font.GetFont("Tahoma", "Regular");
			layout.FontSize = 11.0;
			layout.Alignment = ContentAlignment.MiddleLeft;
			layout.JustifMode = TextJustifMode.AllButLast;
			layout.LayoutSize = new Size(150, 150);

			Point pos = new Point(20, 20);
			e.Graphics.AddFilledRectangle(pos.X, pos.Y, layout.LayoutSize.Width, layout.LayoutSize.Height);
			e.Graphics.RenderSolid(Color.FromBrightness(1));

			TextLayout.SelectedArea[] areas = layout.FindTextRange(pos, 0, layout.Text.Length);
			for ( int i=0 ; i<areas.Length ; i++ )
			{
				e.Graphics.Align(ref areas[i].Rect);
				e.Graphics.AddFilledRectangle(areas[i].Rect.Left, areas[i].Rect.Bottom, areas[i].Rect.Width, areas[i].Rect.Height);
				e.Graphics.RenderSolid(Color.FromRGB(0,1,0));
			}

			layout.Paint(pos, e.Graphics, e.ClipRectangle, Color.Empty, GlyphPaintStyle.Normal);
		}

		private void CheckPaint_PaintWave1(object sender, PaintEventArgs e)
		{
			TextLayout layout = new TextLayout();

			layout.Text = @"On <w>donnait</w> ce jour-l� <w color=""#0000FF"">un grand d�ner</w>, o�, pour la <w>premi�re <b>fois</b></w>, je vis avec beaucoup <w>d'�tonnement le ma�tre d'h�tel servir l'�p�e au c�t� et le chapeau sur la t�te</w>.";
			layout.Font = Font.GetFont("Tahoma", "Regular");
			layout.FontSize = 11.0;
			layout.Alignment = ContentAlignment.MiddleLeft;
			layout.JustifMode = TextJustifMode.AllButLast;
			layout.LayoutSize = new Size(150, 150);

			Point pos = new Point(20, 20);
			e.Graphics.AddFilledRectangle(pos.X, pos.Y, layout.LayoutSize.Width, layout.LayoutSize.Height);
			e.Graphics.RenderSolid(Color.FromBrightness(1));

#if false
			TextLayout.SelectedArea[] areas = layout.FindTextRange(pos, 0, layout.Text.Length);
			for ( int i=0 ; i<areas.Length ; i++ )
			{
				e.Graphics.Align(ref areas[i].Rect);
				e.Graphics.AddFilledRectangle(areas[i].Rect.Left, areas[i].Rect.Bottom, areas[i].Rect.Width, areas[i].Rect.Height);
				e.Graphics.RenderSolid(Color.FromRGB(0,1,0));
			}
#endif

			layout.Paint(pos, e.Graphics, e.ClipRectangle, Color.Empty, GlyphPaintStyle.Normal);
		}

		private void CheckRectangle_Paint1(object sender, PaintEventArgs e)
		{
			TextLayout layout = new TextLayout();

			layout.Text = @"Juste <font size=""30"">trois</font> lignes de texte <font size=""10"">(et une image <img src=""file:images/icon.png""/>)</font> pour rigoler !";
			layout.Font = Font.GetFont("Tahoma", "Regular");
			layout.FontSize = 20.0;
			layout.Alignment = ContentAlignment.MiddleCenter;
			layout.LayoutSize = new Size(200, 100);

			Point pos = new Point(50, 50);

			e.Graphics.AddFilledRectangle(pos.X, pos.Y, layout.LayoutSize.Width, layout.LayoutSize.Height);
			e.Graphics.RenderSolid(Color.FromBrightness(1));

			layout.Paint(pos, e.Graphics, e.ClipRectangle, Color.Empty, GlyphPaintStyle.Normal);

			Rectangle bounds = layout.TotalRectangle;
			bounds.Offset(pos);
			e.Graphics.AddRectangle(bounds);
			e.Graphics.RenderSolid(Color.FromRGB(1,0,0));
		}

		private void CheckImage_Paint1(object sender, PaintEventArgs e)
		{
			TextLayout layout = new TextLayout();

			layout.Text = @"abracadabra&#8212;abracadabra<br/><a href=""x"">abc <img src=""file:images/icon.png"" width=""5"" height=""4""/> def</a><br/>abracadabra abracadabra <a href=""y"">bla bla bla&#160;!</a>";
			layout.Font = Font.GetFont("Tahoma", "Regular");
			layout.FontSize = 15.0;
			layout.Alignment = ContentAlignment.MiddleCenter;
			layout.LayoutSize = new Size(200, 100);

			Point pos = new Point(50, 50);

			e.Graphics.AddFilledRectangle(pos.X, pos.Y, layout.LayoutSize.Width, layout.LayoutSize.Height);
			e.Graphics.RenderSolid(Color.FromBrightness(1));

			layout.Paint(pos, e.Graphics, e.ClipRectangle, Color.Empty, GlyphPaintStyle.Normal);
		}

		
		private TextLayout NewTextLayout()
		{
			TextLayout layout = new TextLayout();
			
			layout.Text       = @"<a href=""x"">Link</a>, <b>Bold text</b>, normal text, <i>italic text</i>...<br/>And some &lt;more&gt; text, <img src=""file:images/icon.png""/> nice &amp; clean.";
			layout.Font       = Font.GetFont("Tahoma", "Regular");
			layout.FontSize   = 11.0;
			layout.LayoutSize = new Size(100, 50);
			
			return layout;
		}
	}
}
