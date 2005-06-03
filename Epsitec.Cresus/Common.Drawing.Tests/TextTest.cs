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
				this.frame1 = new SimpleTextFrame (this.Client.Width / 3, this.Client.Height, 32);
				this.frame2 = new SimpleTextFrame (this.Client.Width - this.frame1.Width, this.Client.Height, 32);
				
				this.frame1.PageNumber = 1;
				this.frame2.PageNumber = 1;
				
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
				
				double ox1 = 60;
				double oy1 = 0;
				double ox2 = 60;
				double oy2 = -1000;
					
				this.frame1.MapToView (ref ox1, ref oy1);
				this.frame1.MapToView (ref ox2, ref oy2);
				
				graphics.LineWidth = 0.25;
				graphics.AddLine (ox1, oy1, ox2, oy2);
				graphics.RenderSolid (Drawing.Color.FromName ("Blue"));
				
				ox1 = this.frame1.Width;
				ox2 = ox1;
				
				graphics.LineWidth = 0.75;
				graphics.AddLine (ox1, oy1, ox2, oy2);
				graphics.RenderSolid (Drawing.Color.FromName ("Blue"));
			}
			
			
			#region ITextRenderer Members
			public bool IsFrameAreaVisible(ITextFrame frame, double x, double y, double width, double height)
			{
				return true;
			}
			
			public void RenderBegin(Text.Layout.Context context)
			{
				ITextFrame frame = context.Frame;
				
				double ox = context.X;
				double oy = context.Y;
				double dx = context.TextWidth;
				
				this.graphics.LineWidth = 0.3;
				this.graphics.AddLine (ox, oy, ox + dx, oy);
				this.graphics.RenderSolid (Drawing.Color.FromName ("Green"));
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
			
			public void RenderEnd(Text.Layout.Context context)
			{
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
				this.window.Text = "Réglages pour TextTest/CheckPainting";
				this.window.MakeSecondaryWindow ();
				this.window.MakeFixedSizeWindow ();
				this.window.MakeToolWindow ();
				this.window.MakeButtonlessWindow ();
				
				StaticText  st1 = new StaticText (this.window.Root);
				CheckButton cb1 = new CheckButton (this.window.Root);
				CheckButton cb2 = new CheckButton (this.window.Root);
				CheckButton cb3 = new CheckButton (this.window.Root);
				CheckButton cb4 = new CheckButton (this.window.Root);
				RadioButton rb1 = new RadioButton (this.window.Root, "g1", 0);
				RadioButton rb2 = new RadioButton (this.window.Root, "g1", 1);
				
				RadioButton.Activate (this.window.Root, "g1", 0);
				
				st1.Dock = DockStyle.Top; st1.DockMargins = new Margins (4, 4, 4, 4);
				cb1.Dock = DockStyle.Top; cb1.DockMargins = new Margins (4, 4, 4, 0);
				cb2.Dock = DockStyle.Top; cb2.DockMargins = new Margins (4, 4, 0, 0);
				cb3.Dock = DockStyle.Top; cb3.DockMargins = new Margins (4, 4, 0, 0);
				cb4.Dock = DockStyle.Top; cb4.DockMargins = new Margins (4, 4, 0, 0);
				
				rb1.Dock = DockStyle.Top; rb1.DockMargins = new Margins (4, 4, 4, 0);
				rb2.Dock = DockStyle.Top; rb2.DockMargins = new Margins (4, 4, 0, 0);
				
				st1.Text = "Réglages pour le rendu du pavé de texte :";
				
				cb1.Name = "liga";			cb1.Text = "ligatures simples";		cb1.ActiveStateChanged += new Support.EventHandler (this.HandleCheckButtonActiveStateChanged);
				cb2.Name = "dlig";			cb2.Text = "ligatures avancées";	cb2.ActiveStateChanged += new Support.EventHandler (this.HandleCheckButtonActiveStateChanged);
				cb3.Name = "kern";			cb3.Text = "crénage";				cb3.ActiveStateChanged += new Support.EventHandler (this.HandleCheckButtonActiveStateChanged);
				cb4.Name = "Mgr=System";	cb4.Text = "utilise GDI";			cb4.ActiveStateChanged += new Support.EventHandler (this.HandleCheckButtonActiveStateChanged);
				
				rb1.Text = "paragraphes avec diverses justifications";			rb1.ActiveStateChanged += new Support.EventHandler (this.HandleRadioButtonActiveStateChanged);
				rb2.Text = "paragraphes avec tabulateurs";						rb2.ActiveStateChanged += new Support.EventHandler (this.HandleRadioButtonActiveStateChanged);
				
				this.window.ClientSize = new Size (260, 120);
				this.window.Owner      = owner;
				
				this.window.Show ();
			}
			
			public void GenerateText()
			{
				this.painter.ResetTextStory ();
				
				ICursor cursor = new Text.Cursors.SimpleCursor ();
				this.painter.TextStory.NewCursor (cursor);
				
				ulong[] text;
				string words;
				
				if (this.active_test == 0)
				{
					System.Collections.ArrayList properties = new System.Collections.ArrayList ();
					
					Text.Properties.FontProperty fp;
					properties.Clear ();
					fp = new Text.Properties.FontProperty ("Arial", "Regular");
					fp.Features = this.features;
					
					properties.Add (fp);
					properties.Add (new Text.Properties.FontSizeProperty (12.0, Text.Properties.SizeUnits.Points));
					properties.Add (new Text.Properties.MarginsProperty (60, 10, 10, 10, Text.Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Text.Properties.ThreeState.True));
					properties.Add (new Text.Properties.ColorProperty (Drawing.Color.FromName ("Black")));
					properties.Add (new Text.Properties.LanguageProperty ("fr-ch", 1.0));
					properties.Add (new Text.Properties.LeadingProperty (10.0, Text.Properties.SizeUnits.Points, 15.0, Text.Properties.SizeUnits.Points, 5.0, Text.Properties.SizeUnits.Points, Text.Properties.LeadingMode.Free));
					
					words = "Bonjour, ceci est un texte d'exemple permettant de vérifier le bon fonctionnement des divers algorithmes de découpe et d'affichage. Le nombre de mots moyen s'élève à environ 40 mots par paragraphe, ce qui correspond à des paragraphes de taille réduite. Quelle idée, un fjord finlandais ! Avocat.\nAWAY.\n______\n";
					
					this.painter.TextStory.ConvertToStyledText (words, properties, out text);
					this.painter.TextStory.InsertText (cursor, text);
					
					properties.Clear ();
					fp = new Text.Properties.FontProperty ("Palatino Linotype", "Regular");
					fp.Features = this.features;
					
					properties.Add (fp);
					properties.Add (new Text.Properties.FontSizeProperty (24.0, Text.Properties.SizeUnits.Points));
					properties.Add (new Text.Properties.MarginsProperty (10, 10, 10, 10, Text.Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 15, 1, Text.Properties.ThreeState.True));
					properties.Add (new Text.Properties.ColorProperty (Drawing.Color.FromName ("Blue")));
					properties.Add (new Text.Properties.LanguageProperty ("fr-ch", 1.0));
					properties.Add (new Text.Properties.LeadingProperty (24.0, Text.Properties.SizeUnits.Points, Text.Properties.LeadingMode.AlignAll));
					properties.Add (new Text.Properties.KeepProperty (3, 1, Text.Properties.ParagraphStartMode.Anywhere, Text.Properties.ThreeState.False, Text.Properties.ThreeState.True));
					
					this.painter.TextStory.ConvertToStyledText (words, properties, out text);
					this.painter.TextStory.InsertText (cursor, text);
					
					
					properties.Clear ();
					fp = new Text.Properties.FontProperty ("Times New Roman", "Italic");
					fp.Features = this.features;
					
					properties.Add (fp);
					properties.Add (new Text.Properties.FontSizeProperty (12.0, Text.Properties.SizeUnits.Points));
					properties.Add (new Text.Properties.MarginsProperty (0, 0, 0, 0, Text.Properties.SizeUnits.Points, 1.0, 1.0, 0.0, 15, 1, Text.Properties.ThreeState.False));
					properties.Add (new Text.Properties.ColorProperty (Drawing.Color.FromName ("Red")));
					properties.Add (new Text.Properties.LeadingProperty (4.0, Text.Properties.SizeUnits.Millimeters, Text.Properties.LeadingMode.AlignFirst));
					
					this.painter.TextStory.ConvertToStyledText (words, properties, out text);
					this.painter.TextStory.InsertText (cursor, text);
					
					
					properties.Clear ();
					fp = new Text.Properties.FontProperty ("Arial", "Regular");
					fp.Features = this.features;
					properties.Add (new Text.Properties.FontProperty ("Arial", "Regular"));
					properties.Add (new Text.Properties.FontSizeProperty (12.0, Text.Properties.SizeUnits.Points));
					properties.Add (new Text.Properties.MarginsProperty (0, 0, 0, 0, Text.Properties.SizeUnits.Points, 0.0, 0.0, 0.5, 15, 1, Text.Properties.ThreeState.False));
					properties.Add (new Text.Properties.ColorProperty (Drawing.Color.FromName ("Black")));
					
					this.painter.TextStory.ConvertToStyledText (words, properties, out text);
					this.painter.TextStory.InsertText (cursor, text);
					
					
					properties.Clear ();
					properties.Add (new Text.Properties.FontProperty ("Arial", "Regular"));
					properties.Add (new Text.Properties.FontSizeProperty (12.0, Text.Properties.SizeUnits.Points));
					properties.Add (new Text.Properties.MarginsProperty (0, 0, 0, 0, Text.Properties.SizeUnits.Points, 0.4, 0.0, 1.0, 15, 1, Text.Properties.ThreeState.False));
					properties.Add (new Text.Properties.ColorProperty (Drawing.Color.FromName ("Black")));
					properties.Add (new Text.Properties.LeadingProperty (double.NaN, Text.Properties.SizeUnits.None, 5.0, Text.Properties.SizeUnits.Points, 5.0, Text.Properties.SizeUnits.Points, Text.Properties.LeadingMode.Free));
					
					this.painter.TextStory.ConvertToStyledText (words, properties, out text);
					this.painter.TextStory.InsertText (cursor, text);
					
					
					words = "Une phrase contenant un \"non\u2011breaking hyphen\" mais aussi un \"soft\u2010hyphen\" au milieu du mot \"Merk\u00ADwürdig\". Voici une césure mongloienne au milieu du mot \"Abra\u1806cadabra\".";
					
					properties.Clear ();
					properties.Add (new Text.Properties.FontProperty ("Verdana", "Regular"));
					properties.Add (new Text.Properties.FontSizeProperty (16.0, Text.Properties.SizeUnits.Points));
					properties.Add (new Text.Properties.MarginsProperty (0, 0, 0, 0, Text.Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 15, 1, Text.Properties.ThreeState.True));
					properties.Add (new Text.Properties.ColorProperty (Drawing.Color.FromName ("Black")));
					
					this.painter.TextStory.ConvertToStyledText (words, properties, out text);
					this.painter.TextStory.InsertText (cursor, text);
				}
				
				if (this.active_test == 1)
				{
					double tab = 60;

					System.Collections.ArrayList properties_1 = new System.Collections.ArrayList ();
					System.Collections.ArrayList properties_2 = new System.Collections.ArrayList ();
#if true
					properties_1.Add (new Text.Properties.FontProperty ("Arial", "Regular", this.features));
					properties_1.Add (new Text.Properties.FontSizeProperty (12.0, Text.Properties.SizeUnits.Points));
					properties_1.Add (new Text.Properties.MarginsProperty (0, 0, 0, 0, Text.Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Text.Properties.ThreeState.False));
					properties_1.Add (new Text.Properties.TabProperty (tab, 0, null));
					properties_1.Add (new Text.Properties.ColorProperty (Drawing.Color.FromName ("Black")));
					
					properties_2.Add (new Text.Properties.FontProperty ("Arial", "Bold", this.features));
					properties_2.Add (new Text.Properties.FontSizeProperty (12.0, Text.Properties.SizeUnits.Points));
					properties_2.Add (new Text.Properties.MarginsProperty (0, 0, 0, 0, Text.Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Text.Properties.ThreeState.False));
					properties_2.Add (new Text.Properties.ColorProperty (Drawing.Color.FromName ("Red")));
					
					this.painter.TextStory.ConvertToStyledText ("Test:\t", properties_1, out text);
					this.painter.TextStory.InsertText (cursor, text);
					
					this.painter.TextStory.ConvertToStyledText ("Tab1", properties_2, out text);
					this.painter.TextStory.InsertText (cursor, text);
					
					this.painter.TextStory.ConvertToStyledText ("...\nCet exemple utilise un tabulateur aligné à gauche.\tTab2; enfin du texte pour la suite...\n\n", properties_1, out text);
					this.painter.TextStory.InsertText (cursor, text);
#endif
#if true
					properties_1.Clear ();
					properties_2.Clear ();
					
					properties_1.Add (new Text.Properties.FontProperty ("Arial", "Regular", this.features));
					properties_1.Add (new Text.Properties.FontSizeProperty (12.0, Text.Properties.SizeUnits.Points));
					properties_1.Add (new Text.Properties.MarginsProperty (0, 0, 0, 0, Text.Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Text.Properties.ThreeState.False));
					properties_1.Add (new Text.Properties.TabProperty (tab, 0.5, null));
					properties_1.Add (new Text.Properties.ColorProperty (Drawing.Color.FromName ("Black")));
					
					properties_2.Add (new Text.Properties.FontProperty ("Arial", "Bold", this.features));
					properties_2.Add (new Text.Properties.FontSizeProperty (12.0, Text.Properties.SizeUnits.Points));
					properties_2.Add (new Text.Properties.MarginsProperty (0, 0, 0, 0, Text.Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Text.Properties.ThreeState.False));
					properties_2.Add (new Text.Properties.ColorProperty (Drawing.Color.FromName ("Red")));
					
					this.painter.TextStory.ConvertToStyledText ("Test:\t", properties_1, out text);
					this.painter.TextStory.InsertText (cursor, text);
					
					this.painter.TextStory.ConvertToStyledText ("Tab3", properties_2, out text);
					this.painter.TextStory.InsertText (cursor, text);
					
					this.painter.TextStory.ConvertToStyledText ("...\nCet exemple utilise un tabulateur centré.\tTab4; enfin du texte pour la suite...\n\n", properties_1, out text);
					this.painter.TextStory.InsertText (cursor, text);
#endif
#if true
					properties_1.Clear ();
					properties_2.Clear ();
					
					properties_1.Add (new Text.Properties.FontProperty ("Arial", "Regular", this.features));
					properties_1.Add (new Text.Properties.FontSizeProperty (12.0, Text.Properties.SizeUnits.Points));
					properties_1.Add (new Text.Properties.MarginsProperty (0, 0, 0, 0, Text.Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 15, 1, Text.Properties.ThreeState.True));
					properties_1.Add (new Text.Properties.TabProperty (tab, 0.0, null));
					properties_1.Add (new Text.Properties.ColorProperty (Drawing.Color.FromName ("Black")));
					properties_1.Add (new Text.Properties.LanguageProperty ("fr-ch", 1.0));
					
					properties_2.Add (new Text.Properties.FontProperty ("Arial", "Bold", this.features));
					properties_2.Add (new Text.Properties.FontSizeProperty (12.0, Text.Properties.SizeUnits.Points));
					properties_2.Add (new Text.Properties.MarginsProperty (0, 0, 0, 0, Text.Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 15, 1, Text.Properties.ThreeState.False));
					properties_2.Add (new Text.Properties.ColorProperty (Drawing.Color.FromName ("Red")));
					
					this.painter.TextStory.ConvertToStyledText ("Test:\t", properties_1, out text);
					this.painter.TextStory.InsertText (cursor, text);
					
					this.painter.TextStory.ConvertToStyledText ("Tab5", properties_2, out text);
					this.painter.TextStory.InsertText (cursor, text);
					
					this.painter.TextStory.ConvertToStyledText ("...\nCet exemple utilise un tabulateur aligné à gauche dans un paragraphe justifié."
						+ "\tTab6-a; enfin du texte pour la suite..."
						+ "\tTab6-b; et aussi pour afficher la fin.\n\n", properties_1, out text);
					this.painter.TextStory.InsertText (cursor, text);
					
					properties_1.Clear ();
					properties_2.Clear ();
					
					properties_1.Add (new Text.Properties.FontProperty ("Arial", "Regular", this.features));
					properties_1.Add (new Text.Properties.FontSizeProperty (12.0, Text.Properties.SizeUnits.Points));
					properties_1.Add (new Text.Properties.MarginsProperty (0, 0, 0, 0, Text.Properties.SizeUnits.Points, 0.0, 0.0, 1.0, 15, 1, Text.Properties.ThreeState.False));
					properties_1.Add (new Text.Properties.TabProperty (tab, 0.0, null));
					properties_1.Add (new Text.Properties.ColorProperty (Drawing.Color.FromName ("Black")));
					
					properties_2.Add (new Text.Properties.FontProperty ("Arial", "Bold", this.features));
					properties_2.Add (new Text.Properties.FontSizeProperty (12.0, Text.Properties.SizeUnits.Points));
					properties_2.Add (new Text.Properties.MarginsProperty (0, 0, 0, 0, Text.Properties.SizeUnits.Points, 1.0, 0.0, 1.0, 15, 1, Text.Properties.ThreeState.False));
					properties_2.Add (new Text.Properties.ColorProperty (Drawing.Color.FromName ("Red")));
					
					this.painter.TextStory.ConvertToStyledText ("Test:\t", properties_1, out text);
					this.painter.TextStory.InsertText (cursor, text);
					
					this.painter.TextStory.ConvertToStyledText ("Tab7", properties_2, out text);
					this.painter.TextStory.InsertText (cursor, text);
					
					this.painter.TextStory.ConvertToStyledText ("...\nCet exemple utilise un tabulateur aligné à gauche dans un paragraphe aligné à droite.\tTab8; enfin du texte pour la suite...\n\n", properties_1, out text);
					this.painter.TextStory.InsertText (cursor, text);
#endif
#if true
					properties_1.Clear ();
					properties_2.Clear ();
					
					properties_1.Add (new Text.Properties.FontProperty ("Arial", "Regular", this.features));
					properties_1.Add (new Text.Properties.FontSizeProperty (12.0, Text.Properties.SizeUnits.Points));
					properties_1.Add (new Text.Properties.MarginsProperty (0, 0, 0, 0, Text.Properties.SizeUnits.Points, 0.0, 0.0, 1.0, 15, 1, Text.Properties.ThreeState.False));
					properties_1.Add (new Text.Properties.TabProperty (tab, 0.5, null));
					properties_1.Add (new Text.Properties.ColorProperty (Drawing.Color.FromName ("Black")));
					
					properties_2.Add (new Text.Properties.FontProperty ("Arial", "Bold", this.features));
					properties_2.Add (new Text.Properties.FontSizeProperty (12.0, Text.Properties.SizeUnits.Points));
					properties_2.Add (new Text.Properties.MarginsProperty (0, 0, 0, 0, Text.Properties.SizeUnits.Points, 1.0, 0.0, 1.0, 15, 1, Text.Properties.ThreeState.False));
					properties_2.Add (new Text.Properties.ColorProperty (Drawing.Color.FromName ("Red")));
					
					this.painter.TextStory.ConvertToStyledText ("Test:\t", properties_1, out text);
					this.painter.TextStory.InsertText (cursor, text);
					
					this.painter.TextStory.ConvertToStyledText ("Tab9", properties_2, out text);
					this.painter.TextStory.InsertText (cursor, text);
					
					this.painter.TextStory.ConvertToStyledText ("...\nCet exemple utilise un tabulateur centré dans un paragraphe aligné à droite.\tTab-10 centré...\n\n", properties_1, out text);
					this.painter.TextStory.InsertText (cursor, text);
#endif
				}
				
				this.painter.TextStory.RecycleCursor (cursor);
				
				this.painter.NotifyTextChanged ();
			}
			
			
			private Painter						painter;
			private Window						window;
			private string[]					features;
			private int							active_test;

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
			private void HandleRadioButtonActiveStateChanged(object sender)
			{
				RadioButton rb = sender as RadioButton;
				
				if (rb.IsActive)
				{
					this.active_test = rb.Index;
					this.GenerateText ();
				}
			}
		}
	}
}

