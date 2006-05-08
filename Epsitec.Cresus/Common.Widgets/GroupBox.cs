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

		
		static GroupBox()
		{
			Helpers.VisualPropertyMetadata metadata = new Helpers.VisualPropertyMetadata (Drawing.ContentAlignment.TopLeft, Helpers.VisualPropertyMetadataOptions.AffectsTextLayout);
			Visual.ContentAlignmentProperty.OverrideMetadata (typeof (GroupBox), metadata);
		}
		
		public override Drawing.Margins GetInternalPadding()
		{
			Drawing.Rectangle frame  = this.FrameRectangle;
			Drawing.Rectangle title  = this.TitleRectangle;
			Drawing.Rectangle client = this.Client.Bounds;
			
			frame.Top = title.Bottom;
			frame.Deflate (2, 2);
			
			return new Drawing.Margins (frame.Left - client.Left, client.Right - frame.Right, client.Top - frame.Top, frame.Bottom - client.Bottom);
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


		public override Drawing.Margins GetShapeMargins()
		{
			return Widgets.Adorners.Factory.Active.GeometryGroupShapeMargins;
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			//	Dessine le texte.
			IAdorner    adorner = Widgets.Adorners.Factory.Active;
			WidgetPaintState state   = this.PaintState;
			
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
			adorner.PaintGeneralTextLayout(graphics, clipRect, pos, this.TextLayout, state, PaintTextStyle.Group, TextDisplayMode.Default, this.BackColor);
		}
	}
}
