//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextFrame permet de représenter du texte en associant un widget
	/// à un frame (cf. Common.Text pour les concepts utilisés).
	/// </summary>
	public class TextFrame : Widget, Epsitec.Common.Text.ITextRenderer
	{
		public TextFrame()
		{
			this.AutoFocus = true;
			this.AutoDoubleClick = true;
			this.InternalState |= WidgetInternalState.Focusable;
			
			this.opletQueue = new Epsitec.Common.Support.OpletQueue ();
			
			this.textContext   = new Epsitec.Common.Text.TextContext ();
			this.textStory     = new Epsitec.Common.Text.TextStory (this.opletQueue, this.textContext);
			this.textFitter    = new Epsitec.Common.Text.TextFitter (this.textStory);
			this.textNavigator = new Epsitec.Common.Text.TextNavigator (this.textFitter);
			this.textFrame     = new Epsitec.Common.Text.SimpleTextFrame (this.PreferredWidth, this.PreferredHeight);
			
			this.navigator = new TextNavigator2 ();
			
			this.navigator.TextNavigator = this.textNavigator;
			
			this.textFitter.FrameList.Add (this.textFrame);
			
			this.textFitter.ClearAllMarks ();
			this.textFitter.GenerateAllMarks ();
			
			this.navigator.TextChanged += this.HandleTextChanged;
			this.navigator.CursorMoved += this.HandleCursorMoved;

			CommandDispatcher dispatcher = new CommandDispatcher ("TextFrame", CommandDispatcherLevel.Secondary, CommandDispatcherOptions.AutoForwardCommands);
			dispatcher.OpletQueue = this.opletQueue;

			CommandDispatcher.SetDispatcher (this, dispatcher);
			
			this.markerSelected = this.textContext.Markers.Selected;
		}
		
		public TextFrame(TextFrame frame) : this (frame.textStory, frame.textFitter, frame.textNavigator)
		{
		}
		
		public TextFrame(Text.TextStory story, Text.TextFitter fitter, Text.TextNavigator navigator)
		{
			this.AutoFocus = true;
			this.AutoDoubleClick = true;
			this.InternalState |= WidgetInternalState.Focusable;
			
			this.opletQueue = story.OpletQueue;
			
			this.textContext   = story.TextContext;
			this.textStory     = story;
			this.textFitter    = fitter;
			this.textNavigator = navigator;
			this.textFrame     = new Epsitec.Common.Text.SimpleTextFrame (this.PreferredWidth, this.PreferredHeight);
			
			this.navigator = new TextNavigator2 ();
			
			this.navigator.TextNavigator = this.textNavigator;
			
			this.textFitter.FrameList.Add (this.textFrame);
			
			this.textFitter.ClearAllMarks ();
			this.textFitter.GenerateAllMarks ();
			
			this.navigator.TextChanged += this.HandleTextChanged;
			this.navigator.CursorMoved += this.HandleCursorMoved;

			CommandDispatcher dispatcher = new CommandDispatcher ("TextFrame", CommandDispatcherLevel.Secondary, CommandDispatcherOptions.AutoForwardCommands);
			
			dispatcher.OpletQueue = this.opletQueue;

			CommandDispatcher.SetDispatcher (this, dispatcher);
			
			this.markerSelected = this.textContext.Markers.Selected;
		}
		
		
		public Epsitec.Common.Text.TextStory	TextStory
		{
			get
			{
				return this.textStory;
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
			
			if ((this.textFitter != null) &&
				(this.textFrame != null))
			{
				this.textFrame.Width  = this.Client.Size.Width;
				this.textFrame.Height = this.Client.Size.Height;
				
				this.textFitter.ClearAllMarks ();
				this.textFitter.GenerateAllMarks ();
			}
		}
		
		protected override void ProcessMessage(Message message, Epsitec.Common.Drawing.Point pos)
		{
			if (this.navigator.ProcessMessage (message, pos, this.textFrame))
			{
				this.Invalidate ();
				return;
			}
			
			base.ProcessMessage (message, pos);
		}
		
		protected override void PaintBackgroundImplementation(Epsitec.Common.Drawing.Graphics graphics, Epsitec.Common.Drawing.Rectangle clipRect)
		{
			graphics.AddFilledRectangle (this.ActualBounds);
			graphics.RenderSolid (Drawing.Color.FromBrightness (1.0));
			
			this.hasSelection = false;
			
//-			System.Diagnostics.Debug.WriteLine ("Paint started.");
			this.graphics = graphics;
			this.textFitter.RenderTextFrame (this.textFrame, this);
			this.graphics = null;
//-			System.Diagnostics.Debug.WriteLine ("Paint done.");
			
			if (this.hasSelection == false)
			{
				Text.ITextFrame frame;
				double cx, cy, ascender, descender, angle;
				
				this.textNavigator.GetCursorGeometry (out frame, out cx, out cy, out ascender, out descender, out angle);
				
				if (frame == this.textFrame)
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
		
		public void RenderTab(Text.Layout.Context layout, string tag, double tabOrigin, double tabStop, ulong tabCode, bool isTabDefined, bool isTabAuto)
		{
		}
			
		public void Render(Text.Layout.Context layout, Epsitec.Common.OpenType.Font font, double size, string color, Text.Layout.TextToGlyphMapping mapping, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy, bool isLastRun)
		{
			Text.ITextFrame frame = layout.Frame;
			
			System.Diagnostics.Debug.Assert (mapping != null);
			
			//	Vérifions d'abord que le mapping du texte vers les glyphes est
			//	correct et correspond à quelque chose de valide :
			
			int  offset = 0;
			bool isInSelection = false;
			
			double selX = 0;
			
			System.Collections.ArrayList selRectList  = null;
			Drawing.Rectangle            selBbox      = Drawing.Rectangle.Empty;
			
			int[]    cArray;
			ulong[]  tArray;
			ushort[] gArray;
			
			double x1 = 0;
			double x2 = 0;
			
			while (mapping.GetNextMapping (out cArray, out gArray, out tArray))
			{
				int numGlyphs = gArray.Length;
				int numChars  = cArray.Length;
				
				System.Diagnostics.Debug.Assert ((numGlyphs == 1) || (numChars == 1));
				
				x1 = x[offset+0];
				x2 = x[offset+numGlyphs];
				
				for (int i = 0; i < numChars; i++)
				{
					if ((tArray[i] & this.markerSelected) != 0)
					{
						//	Le caractère considéré est sélectionné.
						
						if (isInSelection == false)
						{
							//	C'est le premier caractère d'une tranche. Il faut
							//	mémoriser son début :
							
							double xx = x1 + ((x2 - x1) * i) / numChars;
							isInSelection = true;
							selX = xx;
						}
					}
					else
					{
						if (isInSelection)
						{
							//	Nous avons quitté une tranche sélectionnée. Il faut
							//	mémoriser sa fin :
							
							double xx = x1 + ((x2 - x1) * i) / numChars;
							isInSelection = false;
							
							if (xx > selX)
							{
								if (selRectList == null)
								{
									selRectList = new System.Collections.ArrayList ();
								}
								
								double dx = xx - selX;
								double dy = layout.LineY2 - layout.LineY1;
								
								Drawing.Rectangle rect = new Drawing.Rectangle (selX, layout.LineY1, dx, dy);
								
								selBbox = Drawing.Rectangle.Union (selBbox, rect);
								
								double px1 = rect.Left;
								double px2 = rect.Right;
								double py1 = rect.Bottom;
								double py2 = rect.Top;
								
								this.graphics.Rasterizer.Transform.TransformDirect (ref px1, ref py1);
								this.graphics.Rasterizer.Transform.TransformDirect (ref px2, ref py2);
								
								selRectList.Add (Drawing.Rectangle.FromPoints (px1, py1, px2, py2));
							}
						}
					}
				}
				
				offset += numGlyphs;
			}
			
			if (isInSelection)
			{
				//	Nous avons quitté une tranche sélectionnée. Il faut
				//	mémoriser sa fin :
				
				double xx = x2;
				isInSelection = false;
				
				if (xx > selX)
				{
					if (selRectList == null)
					{
						selRectList = new System.Collections.ArrayList ();
					}
					
					double dx = xx - selX;
					double dy = layout.LineY2 - layout.LineY1;
					
					Drawing.Rectangle rect = new Drawing.Rectangle (selX, layout.LineY1, dx, dy);
					
					selBbox = Drawing.Rectangle.Union (selBbox, rect);
					
					double px1 = rect.Left;
					double px2 = rect.Right;
					double py1 = rect.Bottom;
					double py2 = rect.Top;
					
					this.graphics.Rasterizer.Transform.TransformDirect (ref px1, ref py1);
					this.graphics.Rasterizer.Transform.TransformDirect (ref px2, ref py2);
					
					selRectList.Add (Drawing.Rectangle.FromPoints (px1, py1, px2, py2));
				}
			}
			
			if (font.FontManagerType == OpenType.FontManagerType.System)
			{
				Drawing.NativeTextRenderer.Draw (this.graphics.Pixmap, font, size, glyphs, x, y, Drawing.Color.FromName (color));
			}
			else
			{
				Drawing.Font drawingFont = Drawing.Font.GetFont (font.FontIdentity.InvariantFaceName, font.FontIdentity.InvariantStyleName);
				
				if (drawingFont != null)
				{
					for (int i = 0; i < glyphs.Length; i++)
					{
						if (glyphs[i] < 0xffff)
						{
							this.graphics.Rasterizer.AddGlyph (drawingFont, glyphs[i], x[i], y[i], size, sx == null ? 1.0 : sx[i], sy == null ? 1.0 : sy[i]);
						}
					}
				}
				
				this.graphics.RenderSolid (Drawing.Color.FromName (color));
			}
			
			if (selRectList != null)
			{
				this.hasSelection = true;
				
				Drawing.Rectangle saveClip = this.graphics.SaveClippingRectangle ();
				
				this.graphics.SetClippingRectangles (selRectList);
				this.graphics.AddFilledRectangle (selBbox);
				this.graphics.RenderSolid (Drawing.Color.FromName ("Highlight"));
				
				if (font.FontManagerType == OpenType.FontManagerType.System)
				{
					Drawing.NativeTextRenderer.Draw (this.graphics.Pixmap, font, size, glyphs, x, y, Drawing.Color.FromName (color));
				}
				else
				{
					Drawing.Font drawingFont = Drawing.Font.GetFont (font.FontIdentity.InvariantFaceName, font.FontIdentity.InvariantStyleName);
					
					if (drawingFont != null)
					{
						for (int i = 0; i < glyphs.Length; i++)
						{
							if (glyphs[i] < 0xffff)
							{
								this.graphics.Rasterizer.AddGlyph (drawingFont, glyphs[i], x[i], y[i], size, sx == null ? 1.0 : sx[i], sy == null ? 1.0 : sy[i]);
							}
						}
					}
					
					this.graphics.RenderSolid (Drawing.Color.FromName ("HighlightText"));
				}
				
				this.graphics.RestoreClippingRectangle (saveClip);
			}
		}
		
		public void Render(Epsitec.Common.Text.Layout.Context layout, Epsitec.Common.Text.IGlyphRenderer glyphRenderer, string color, double x, double y, bool isLastRun)
		{
			glyphRenderer.RenderGlyph (layout.Frame, x, y);
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
			this.textFitter.ClearAllMarks ();
			this.textFitter.GenerateAllMarks ();
			this.Invalidate ();
		}
		
		private void HandleCursorMoved(object sender)
		{
			this.Invalidate ();
		}
		
		
		private Drawing.Graphics				graphics;
		private bool							hasSelection;
		private ulong							markerSelected;
		
		private Support.OpletQueue				opletQueue;
		private Common.Text.TextContext			textContext;
		private Common.Text.TextStory			textStory;
		private Common.Text.TextFitter			textFitter;
		private Common.Text.TextNavigator		textNavigator;
		private Common.Text.SimpleTextFrame		textFrame;
		
		private TextNavigator2					navigator;
	}
}
