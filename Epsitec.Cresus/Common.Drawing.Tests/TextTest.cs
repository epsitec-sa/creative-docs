using NUnit.Framework;

using Epsitec.Common.Widgets;
using Epsitec.Common.Text;


namespace Epsitec.Common.Drawing
{
	[TestFixture]
	public class TextTest
	{
		[Test] public void CheckPainting()
		{
			Window  window  = new Window ();
			Painter painter = new Painter ();
			
			window.ClientSize     = new Size (500, 700);
			window.WindowLocation = ScreenInfo.Find (new Point (10, 10)).WorkingArea.TopLeft + new Point (100, -100 - window.WindowSize.Height);
			window.Text           = "TextTest/CheckPainting";
			
			painter.Parent = window.Root;
			painter.Dock   = DockStyle.Fill;
			
			Controller controller = new Controller (painter);
			
			controller.CreateWindow (window);
			controller.GenerateText ();
			
			window.Show ();
		}
		
		
		
		private class Painter : Widget, ITextRenderer
		{
			public Painter()
			{
				this.story  = new TextStory ();
				this.fitter = new TextFitter (this.story);
				this.frame1 = new SimpleTextFrame (this.Client.Width / 3, this.Client.Height);
				this.frame2 = new SimpleTextFrame (this.Client.Width - this.frame1.Width, this.Client.Height);
				
				this.fitter.FrameList.InsertAt (0, this.frame1);
				this.fitter.FrameList.InsertAt (1, this.frame2);
			}
			
			
			public TextStory					TextStory
			{
				get
				{
					return this.story;
				}
			}
			
			
			public void ResetTextStory()
			{
				this.story = new TextStory ();
				this.fitter.TextStory = this.story;
			}
			
			public void NotifyTextChanged()
			{
				this.fitter.ClearAllMarks ();
				this.fitter.GenerateAllMarks ();
				this.Invalidate ();
			}
			
			
			protected override void OnSizeChanged()
			{
				base.OnSizeChanged ();
				
				if ((this.frame1 != null) &&
					(this.frame2 != null))
				{
					this.frame1.Width  = this.Client.Width / 3;
					this.frame1.Height = this.Client.Height;
					
					this.frame2.Width  = this.Client.Width - this.frame1.Width;
					this.frame2.Height = this.Client.Height;
					this.frame2.X      = this.frame1.Width + this.frame1.X;
					
					this.fitter.ClearAllMarks ();
					this.fitter.GenerateAllMarks ();
				}
			}

			protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clip_rect)
			{
				graphics.AddFilledRectangle (0, 0, this.Width, this.Height);
				graphics.RenderSolid (Drawing.Color.FromBrightness (1.0));
				
				System.Diagnostics.Debug.WriteLine ("Paint called.");
				
				this.graphics = graphics;
				this.fitter.RenderTextFrame (this.frame1, this);
				this.fitter.RenderTextFrame (this.frame2, this);
				this.graphics = null;
				
				System.Diagnostics.Debug.WriteLine ("Paint done.");
			}
			
			
			#region ITextRenderer Members
			public bool IsFrameAreaVisible(ITextFrame frame, double x, double y, double width, double height)
			{
				return true;
			}
			
			public void Render(ITextFrame frame, Epsitec.Common.OpenType.Font font, double size, Drawing.Color color, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy)
			{
				if (font.FontManagerType == OpenType.FontManagerType.System)
				{
					Drawing.NativeTextRenderer.Draw (this.graphics.Pixmap, font, size, glyphs, x, y, color);
				}
				else
				{
					Drawing.Font drawing_font = Drawing.Font.GetFont (font.FontIdentity.InvariantFaceName, font.FontIdentity.InvariantStyleName);
					
					if (drawing_font != null)
					{
						for (int i = 0; i < glyphs.Length; i++)
						{
							if (glyphs[i] < 0xffff)
							{
								this.graphics.Rasterizer.AddGlyph (drawing_font, glyphs[i], x[i], y[i], size, sx == null ? 1.0 : sx[i], sy == null ? 1.0 : sy[i]);
							}
						}
					}
					
					this.graphics.RenderSolid (color);
				}
			}
			#endregion
			
			private TextStory					story;
			private TextFitter					fitter;
			private SimpleTextFrame				frame1, frame2;
			private Graphics					graphics;
		}
		
		private class Controller
		{
			public Controller(Painter painter)
			{
				this.painter = painter;
				this.features = new string[] { };
			}
			
			
			public void CreateWindow(Window owner)
			{
				this.window = new Window ();
				this.window.Text = "R�glages pour TextTest/CheckPainting";
				this.window.MakeSecondaryWindow ();
				this.window.MakeFixedSizeWindow ();
				this.window.MakeToolWindow ();
				this.window.MakeButtonlessWindow ();
				
				StaticText  st1 = new StaticText ();
				CheckButton cb1 = new CheckButton ();
				CheckButton cb2 = new CheckButton ();
				CheckButton cb3 = new CheckButton ();
				CheckButton cb4 = new CheckButton ();
				
				st1.Parent = this.window.Root;	st1.Dock = DockStyle.Top;	st1.DockMargins = new Margins (4, 4, 4, 4);
				cb1.Parent = this.window.Root;	cb1.Dock = DockStyle.Top;	cb1.DockMargins = new Margins (4, 4, 4, 0);
				cb2.Parent = this.window.Root;	cb2.Dock = DockStyle.Top;	cb2.DockMargins = new Margins (4, 4, 0, 0);
				cb3.Parent = this.window.Root;	cb3.Dock = DockStyle.Top;	cb3.DockMargins = new Margins (4, 4, 0, 0);
				cb4.Parent = this.window.Root;	cb4.Dock = DockStyle.Top;	cb4.DockMargins = new Margins (4, 4, 0, 0);
				
				st1.Text   = "R�glages pour le rendu du pav� de texte :";
				
				cb1.Name   = "liga";		cb1.Text = "ligatures simples";		cb1.ActiveStateChanged += new Support.EventHandler(this.HandleCheckButtonActiveStateChanged);
				cb2.Name   = "dlig";		cb2.Text = "ligatures avanc�es";	cb2.ActiveStateChanged += new Support.EventHandler(this.HandleCheckButtonActiveStateChanged);
				cb3.Name   = "kern";		cb3.Text = "cr�nage";				cb3.ActiveStateChanged += new Support.EventHandler(this.HandleCheckButtonActiveStateChanged);
				cb4.Name   = "Mgr=System";	cb4.Text = "utilise GDI";			cb4.ActiveStateChanged += new Support.EventHandler(this.HandleCheckButtonActiveStateChanged);
				
				this.window.ClientSize = new Size (240, 90);
				this.window.Owner      = owner;
				
				this.window.Show ();
			}
			
			public void GenerateText()
			{
				this.painter.ResetTextStory ();
				
				ICursor cursor = new Text.Cursors.SimpleCursor ();
				this.painter.TextStory.NewCursor (cursor);
				
				System.Collections.ArrayList properties = new System.Collections.ArrayList ();
				ulong[] text;
				string words;
				
				
				Text.Properties.FontProperty fp;
				properties.Clear ();
				fp = new Text.Properties.FontProperty ("Arial", "Regular");
				fp.Features = this.features;
				
				properties.Add (fp);
				properties.Add (new Text.Properties.FontSizeProperty (12.0, Text.Properties.FontSizeUnits.Points));
				properties.Add (new Text.Properties.MarginsProperty (60, 10, 10, 10, 0.0, 0.0, 0.0, 15, 1, false));
				
				words = "Bonjour, ceci est un texte d'exemple permettant de v�rifier le bon fonctionnement des divers algorithmes de d�coupe et d'affichage. Le nombre de mots moyen s'�l�ve � environ 40 mots par paragraphe, ce qui correspond � des paragraphes de taille r�duite. Quelle id�e, un fjord finlandais ! Avocat.\nAWAY.\n______\n";
				
				this.painter.TextStory.ConvertToStyledText (words, properties, out text);
				this.painter.TextStory.InsertText (cursor, text);
				
				properties.Clear ();
				fp = new Text.Properties.FontProperty ("Palatino Linotype", "Regular");
				fp.Features = this.features;
				
				properties.Add (fp);
				properties.Add (new Text.Properties.FontSizeProperty (24.0, Text.Properties.FontSizeUnits.Points));
				properties.Add (new Text.Properties.MarginsProperty (10, 10, 10, 10, 1.0, 0.0, 0.0, 15, 1, false));
				
				this.painter.TextStory.ConvertToStyledText (words, properties, out text);
				this.painter.TextStory.InsertText (cursor, text);
				
				
				properties.Clear ();
				fp = new Text.Properties.FontProperty ("Times New Roman", "Italic");
				fp.Features = this.features;
				
				properties.Add (fp);
				properties.Add (new Text.Properties.FontSizeProperty (12.0, Text.Properties.FontSizeUnits.Points));
				properties.Add (new Text.Properties.MarginsProperty (0, 0, 0, 0, 1.0, 1.0, 0.0, 15, 1, false));
				
				this.painter.TextStory.ConvertToStyledText (words, properties, out text);
				this.painter.TextStory.InsertText (cursor, text);
				
				
				properties.Clear ();
				fp = new Text.Properties.FontProperty ("Arial", "Regular");
				fp.Features = this.features;
				properties.Add (new Text.Properties.FontProperty ("Arial", "Regular"));
				properties.Add (new Text.Properties.FontSizeProperty (12.0, Text.Properties.FontSizeUnits.Points));
				properties.Add (new Text.Properties.MarginsProperty (0, 0, 0, 0, 0.0, 0.0, 0.5, 15, 1, false));
				
				this.painter.TextStory.ConvertToStyledText (words, properties, out text);
				this.painter.TextStory.InsertText (cursor, text);
				
				
				properties.Clear ();
				properties.Add (new Text.Properties.FontProperty ("Arial", "Regular"));
				properties.Add (new Text.Properties.FontSizeProperty (12.0, Text.Properties.FontSizeUnits.Points));
				properties.Add (new Text.Properties.MarginsProperty (0, 0, 0, 0, 0.4, 0.0, 1.0, 15, 1, false));
				
				this.painter.TextStory.ConvertToStyledText (words, properties, out text);
				this.painter.TextStory.InsertText (cursor, text);
				this.painter.TextStory.RecycleCursor (cursor);
				
				this.painter.NotifyTextChanged ();
			}
			
			
			private Painter						painter;
			private Window						window;
			private string[]					features;

			private void HandleCheckButtonActiveStateChanged(object sender)
			{
				CheckButton cb = sender as CheckButton;
				
				string f = string.Join (":", this.features);
				
				f = f.Replace (cb.Name, "").Replace ("::", ":");
				
				if (cb.IsActive)
				{
					f = string.Concat (f, ":", cb.Name);
				}
				
				while (f.EndsWith (":"))
				{
					f = f.Substring (0, f.Length-1);
				}
				
				while (f.StartsWith (":"))
				{
					f = f.Substring (1);
				}
				
				this.features = f.Split (':');
				
				System.Diagnostics.Debug.WriteLine ("Features: " + f);
				
				this.GenerateText ();
			}
		}
	}
}

