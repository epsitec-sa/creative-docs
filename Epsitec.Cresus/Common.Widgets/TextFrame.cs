//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextFrame permet de représenter du texte en associant un widget
	/// à un frame (cf. Common.Text pour les concepts utilisés).
	/// </summary>
	[Support.SuppressBundleSupport]
	public class TextFrame : Widget, Epsitec.Common.Text.ITextRenderer
	{
		public TextFrame()
		{
			this.AutoFocus = true;
			this.AutoDoubleClick = true;
			this.InternalState |= InternalState.Focusable;
			
			this.oplet_queue = new Epsitec.Common.Support.OpletQueue ();
			
			this.text_context   = new Epsitec.Common.Text.TextContext ();
			this.text_story     = new Epsitec.Common.Text.TextStory (this.oplet_queue, this.text_context);
			this.text_fitter    = new Epsitec.Common.Text.TextFitter (this.text_story);
			this.text_navigator = new Epsitec.Common.Text.TextNavigator (this.text_fitter);
			this.text_frame     = new Epsitec.Common.Text.SimpleTextFrame (this.DefaultWidth, this.DefaultHeight);
			
			this.navigator = new TextNavigator2 ();
			
			this.navigator.TextNavigator = this.text_navigator;
			
			this.text_fitter.FrameList.Add (this.text_frame);
			
			this.text_fitter.ClearAllMarks ();
			this.text_fitter.GenerateAllMarks ();
			
			this.navigator.TextChanged += new Support.EventHandler (this.HandleTextChanged);
			this.navigator.CursorMoved += new Support.EventHandler (this.HandleCursorMoved);
			
			this.marker_selected = this.text_context.Markers.Selected;
		}
		
		public TextFrame(TextFrame frame) : this (frame.text_story, frame.text_fitter, frame.text_navigator)
		{
		}
		
		public TextFrame(Text.TextStory story, Text.TextFitter fitter, Text.TextNavigator navigator)
		{
			this.AutoFocus = true;
			this.AutoDoubleClick = true;
			this.InternalState |= InternalState.Focusable;
			
			this.oplet_queue = story.OpletQueue;
			
			this.text_context   = story.TextContext;
			this.text_story     = story;
			this.text_fitter    = fitter;
			this.text_navigator = navigator;
			this.text_frame     = new Epsitec.Common.Text.SimpleTextFrame (this.DefaultWidth, this.DefaultHeight);
			
			this.navigator = new TextNavigator2 ();
			
			this.navigator.TextNavigator = this.text_navigator;
			
			this.text_fitter.FrameList.Add (this.text_frame);
			
			this.text_fitter.ClearAllMarks ();
			this.text_fitter.GenerateAllMarks ();
			
			this.navigator.TextChanged += new Support.EventHandler (this.HandleTextChanged);
			this.navigator.CursorMoved += new Support.EventHandler (this.HandleCursorMoved);
			
			this.marker_selected = this.text_context.Markers.Selected;
		}
		
		
		public override Support.OpletQueue		OpletQueue
		{
			get
			{
				return this.oplet_queue;
			}
			set
			{
				throw new System.InvalidOperationException ("Cannot set OpletQueue on Widget.");
			}
		}
		
		
		public Epsitec.Common.Text.TextStory	TextStory
		{
			get
			{
				return this.text_story;
			}
		}
		
		public TextNavigator2					TextNavigator
		{
			get
			{
				return this.navigator;
			}
		}
		
		
		protected override void  SetBoundsOverride(Epsitec.Common.Drawing.Rectangle oldRect, Epsitec.Common.Drawing.Rectangle newRect)
		{
 			 base.SetBoundsOverride(oldRect, newRect);
			
			if ((this.text_fitter != null) &&
				(this.text_frame != null))
			{
				this.text_frame.Width  = this.Client.Size.Width;
				this.text_frame.Height = this.Client.Size.Height;
				
				this.text_fitter.ClearAllMarks ();
				this.text_fitter.GenerateAllMarks ();
			}
		}
		
		protected override void ProcessMessage(Message message, Epsitec.Common.Drawing.Point pos)
		{
			if (this.navigator.ProcessMessage (message, pos, this.text_frame))
			{
				this.Invalidate ();
				return;
			}
			
			base.ProcessMessage (message, pos);
		}
		
		protected override void PaintBackgroundImplementation(Epsitec.Common.Drawing.Graphics graphics, Epsitec.Common.Drawing.Rectangle clip_rect)
		{
			graphics.AddFilledRectangle (0, 0, this.Width, this.Height);
			graphics.RenderSolid (Drawing.Color.FromBrightness (1.0));
			
			this.has_selection = false;
			
//-			System.Diagnostics.Debug.WriteLine ("Paint started.");
			this.graphics = graphics;
			this.text_fitter.RenderTextFrame (this.text_frame, this);
			this.graphics = null;
//-			System.Diagnostics.Debug.WriteLine ("Paint done.");
			
			if (this.has_selection == false)
			{
				Text.ITextFrame frame;
				double cx, cy, ascender, descender, angle;
				
				this.text_navigator.GetCursorGeometry (out frame, out cx, out cy, out ascender, out descender, out angle);
				
				if (frame == this.text_frame)
				{
					double tan = System.Math.Tan (System.Math.PI / 2 - angle);
					
					double x1 = cx + descender * tan;
					double x2 = cx + ascender  * tan;
					double y1 = cy + descender;
					double y2 = cy + ascender;
					
					graphics.LineWidth = 2.0;
					graphics.AddLine (x1, y1, x2, y2);
					graphics.RenderSolid (Drawing.Color.FromRgb (1.0, 0.0, 0.0));
				}
			}
		}
		
		
		#region ITextRenderer Members
		public bool IsFrameAreaVisible(Epsitec.Common.Text.ITextFrame frame, double x, double y, double width, double height)
		{
			return true;
		}
		
		public void RenderStartParagraph(Text.Layout.Context context)
		{
		}
		
		public void RenderStartLine(Text.Layout.Context context)
		{
			double ox = context.LineCurrentX;
			double oy = context.LineBaseY;
			double dx = context.TextWidth;
			
			this.graphics.LineWidth = 0.3;
			this.graphics.AddLine (ox, oy, ox + dx, oy);
			this.graphics.RenderSolid (Drawing.Color.FromName ("Green"));
			
			context.DisableSimpleRendering ();
		}
		
		public void RenderTab(Text.Layout.Context layout, string tag, double tab_origin, double tab_stop, ulong tab_code, bool is_tab_defined, bool is_tab_auto)
		{
		}
			
		public void Render(Text.Layout.Context layout, Epsitec.Common.OpenType.Font font, double size, string color, Text.Layout.TextToGlyphMapping mapping, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy, bool is_last_run)
		{
			Text.ITextFrame frame = layout.Frame;
			
			System.Diagnostics.Debug.Assert (mapping != null);
			
			//	Vérifions d'abord que le mapping du texte vers les glyphes est
			//	correct et correspond à quelque chose de valide :
			
			int  offset = 0;
			bool is_in_selection = false;
			
			double sel_x = 0;
			
			System.Collections.ArrayList sel_rect_list = null;
			Drawing.Rectangle            sel_bbox      = Drawing.Rectangle.Empty;
			
			int[]    c_array;
			ulong[]  t_array;
			ushort[] g_array;
			
			double x1 = 0;
			double x2 = 0;
			
			while (mapping.GetNextMapping (out c_array, out g_array, out t_array))
			{
				int num_glyphs = g_array.Length;
				int num_chars  = c_array.Length;
				
				System.Diagnostics.Debug.Assert ((num_glyphs == 1) || (num_chars == 1));
				
				x1 = x[offset+0];
				x2 = x[offset+num_glyphs];
				
				for (int i = 0; i < num_chars; i++)
				{
					if ((t_array[i] & this.marker_selected) != 0)
					{
						//	Le caractère considéré est sélectionné.
						
						if (is_in_selection == false)
						{
							//	C'est le premier caractère d'une tranche. Il faut
							//	mémoriser son début :
							
							double xx = x1 + ((x2 - x1) * i) / num_chars;
							is_in_selection = true;
							sel_x = xx;
						}
					}
					else
					{
						if (is_in_selection)
						{
							//	Nous avons quitté une tranche sélectionnée. Il faut
							//	mémoriser sa fin :
							
							double xx = x1 + ((x2 - x1) * i) / num_chars;
							is_in_selection = false;
							
							if (xx > sel_x)
							{
								if (sel_rect_list == null)
								{
									sel_rect_list = new System.Collections.ArrayList ();
								}
								
								double dx = xx - sel_x;
								double dy = layout.LineY2 - layout.LineY1;
								
								Drawing.Rectangle rect = new Drawing.Rectangle (sel_x, layout.LineY1, dx, dy);
								
								sel_bbox = Drawing.Rectangle.Union (sel_bbox, rect);
								
								double px1 = rect.Left;
								double px2 = rect.Right;
								double py1 = rect.Bottom;
								double py2 = rect.Top;
								
								this.graphics.Rasterizer.Transform.TransformDirect (ref px1, ref py1);
								this.graphics.Rasterizer.Transform.TransformDirect (ref px2, ref py2);
								
								sel_rect_list.Add (Drawing.Rectangle.FromPoints (px1, py1, px2, py2));
							}
						}
					}
				}
				
				offset += num_glyphs;
			}
			
			if (is_in_selection)
			{
				//	Nous avons quitté une tranche sélectionnée. Il faut
				//	mémoriser sa fin :
				
				double xx = x2;
				is_in_selection = false;
				
				if (xx > sel_x)
				{
					if (sel_rect_list == null)
					{
						sel_rect_list = new System.Collections.ArrayList ();
					}
					
					double dx = xx - sel_x;
					double dy = layout.LineY2 - layout.LineY1;
					
					Drawing.Rectangle rect = new Drawing.Rectangle (sel_x, layout.LineY1, dx, dy);
					
					sel_bbox = Drawing.Rectangle.Union (sel_bbox, rect);
					
					double px1 = rect.Left;
					double px2 = rect.Right;
					double py1 = rect.Bottom;
					double py2 = rect.Top;
					
					this.graphics.Rasterizer.Transform.TransformDirect (ref px1, ref py1);
					this.graphics.Rasterizer.Transform.TransformDirect (ref px2, ref py2);
					
					sel_rect_list.Add (Drawing.Rectangle.FromPoints (px1, py1, px2, py2));
				}
			}
			
			if (font.FontManagerType == OpenType.FontManagerType.System)
			{
				Drawing.NativeTextRenderer.Draw (this.graphics.Pixmap, font, size, glyphs, x, y, Drawing.Color.FromName (color));
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
				
				this.graphics.RenderSolid (Drawing.Color.FromName (color));
			}
			
			if (sel_rect_list != null)
			{
				this.has_selection = true;
				
				Drawing.Rectangle save_clip = this.graphics.SaveClippingRectangle ();
				
				this.graphics.SetClippingRectangles (sel_rect_list);
				this.graphics.AddFilledRectangle (sel_bbox);
				this.graphics.RenderSolid (Drawing.Color.FromName ("Highlight"));
				
				if (font.FontManagerType == OpenType.FontManagerType.System)
				{
					Drawing.NativeTextRenderer.Draw (this.graphics.Pixmap, font, size, glyphs, x, y, Drawing.Color.FromName (color));
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
					
					this.graphics.RenderSolid (Drawing.Color.FromName ("HighlightText"));
				}
				
				this.graphics.RestoreClippingRectangle (save_clip);
			}
		}
		
		public void Render(Epsitec.Common.Text.Layout.Context layout, Epsitec.Common.Text.IGlyphRenderer glyph_renderer, string color, double x, double y, bool is_last_run)
		{
			glyph_renderer.RenderGlyph (layout.Frame, x, y);
		}
		
		public void RenderEndLine(Text.Layout.Context context)
		{
		}
		
		public void RenderEndParagraph(Text.Layout.Context context)
		{
			Text.Layout.XlineRecord[] records = context.XlineRecords;
			
			double x1 = 0;
			double y1 = 0;
			
			//	Dans ce test, la couleur est stockée directement comme LineStyle pour la propriété
			//	"underline".
			
			string color = "Yellow";
			
			if (records.Length > 0)
			{
				for (int i = 0; i < records.Length; i++)
				{
					if ((records[i].Type == Common.Text.Layout.XlineRecord.RecordType.LineEnd) ||
						(records[i].Xlines.Length == 0))
					{
						this.graphics.LineWidth = 1.0;
						this.graphics.AddLine (x1, y1, records[i].X, records[i].Y + records[i].Descender * 0.8);
						this.graphics.RenderSolid (Drawing.Color.FromName (color));
					}
					
					x1 = records[i].X;
					y1 = records[i].Y + records[i].Descender * 0.8;
					
					if (records[i].Xlines.Length > 0)
					{
						color = records[i].Xlines[0].DrawStyle;
					}
				}
			}
		}
		#endregion
		
		private void HandleTextChanged(object sender)
		{
			this.text_fitter.ClearAllMarks ();
			this.text_fitter.GenerateAllMarks ();
			this.Invalidate ();
		}
		
		private void HandleCursorMoved(object sender)
		{
			this.Invalidate ();
		}
		
		
		private Drawing.Graphics				graphics;
		private bool							has_selection;
		private ulong							marker_selected;
		
		private Support.OpletQueue				oplet_queue;
		private Common.Text.TextContext			text_context;
		private Common.Text.TextStory			text_story;
		private Common.Text.TextFitter			text_fitter;
		private Common.Text.TextNavigator		text_navigator;
		private Common.Text.SimpleTextFrame		text_frame;
		
		private TextNavigator2					navigator;
	}
}
