namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Summary description for GroupBox.
	/// </summary>
	public class GroupBox : AbstractGroup
	{
		public GroupBox()
		{
		}
		
		public GroupBox(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		
		// Retourne l'alignement par défaut d'un bouton.
		public override Drawing.ContentAlignment	DefaultAlignment
		{
			get
			{
				return Drawing.ContentAlignment.TopLeft;
			}
		}
		
		public override Drawing.Rectangle			InnerBounds
		{
			get
			{
				Drawing.Rectangle frame = this.FrameRectangle;
				Drawing.Rectangle title = this.TitleRectangle;
				
				frame.Top = title.Bottom;
				frame.Deflate (2, 2);
				
				return frame;
			}
		}
		
		public Drawing.Point						TitleTextOffset
		{
			get { return new Drawing.Point (10, 0); }
		}
		
		public Drawing.Rectangle					TitleRectangle
		{
			get
			{
				TextLayout layout = this.TextLayout;
				
				if (layout == null)
				{
					return Drawing.Rectangle.Empty;
				}
				
				Drawing.Rectangle rect = layout.StandardRectangle;
				rect.RoundInflate();
				rect.Offset(this.TitleTextOffset);
				rect.Inflate(3, 0);
				return rect;
			}
		}
		
		public Drawing.Rectangle					FrameRectangle
		{
			get
			{
				Drawing.Rectangle rect  = this.Client.Bounds;
				Drawing.Rectangle title = this.TitleRectangle;
				
				if (title.IsValid)
				{
					double dy = title.Bottom + title.Top;
					rect.Top -= System.Math.Floor (rect.Height - dy/2);
				}
				
				return rect;
			}
		}
		
		
		public override Drawing.Rectangle GetShapeBounds()
		{
			Drawing.Rectangle rect = base.GetShapeBounds();
			rect.Inflate(Widgets.Adorner.Factory.Active.GeometryGroupShapeBounds);
			return rect;
		}

		// Dessine le texte.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner    adorner = Widgets.Adorner.Factory.Active;
			WidgetState state   = this.PaintState;
			
#if false
			Drawing.Rectangle rect  = this.Client.Bounds;
			Drawing.Rectangle titleRect = this.TextLayout.StandardRectangle;
			Drawing.Point pos = new Drawing.Point(10, 0);
			titleRect.Offset(pos);
			titleRect.Inflate(3, 0);
			Drawing.Rectangle frameRect = new Drawing.Rectangle();
			frameRect = rect;
			frameRect.Top -= System.Math.Floor(frameRect.Height-(titleRect.Bottom+titleRect.Top)/2);
#else
			Drawing.Rectangle frameRect = this.FrameRectangle;
			Drawing.Rectangle titleRect = this.TitleRectangle;
			Drawing.Point     pos       = this.TitleTextOffset;
#endif

			adorner.PaintGroupBox(graphics, frameRect, titleRect, state);
			adorner.PaintGeneralTextLayout(graphics, pos, this.TextLayout, state, PaintTextStyle.Group, this.BackColor);
		}
	}
}
