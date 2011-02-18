//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.GroupBox))]

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
			Types.DependencyPropertyMetadata metadata = Visual.ContentAlignmentProperty.DefaultMetadata.Clone ();

			metadata.DefineDefaultValue (Drawing.ContentAlignment.TopLeft);

			Visual.ContentAlignmentProperty.OverrideMetadata (typeof (GroupBox), metadata);
		}

		protected override Drawing.Size GetTextLayoutSize()
		{
			if (this.IsActualGeometryValid)
			{
				return base.GetTextLayoutSize ();
			}
			else
			{
				Layouts.LayoutMeasure width  = Layouts.LayoutMeasure.GetWidth (this);
				Layouts.LayoutMeasure height = Layouts.LayoutMeasure.GetHeight (this);

				double dx = ((width == null)  || double.IsNaN (width.Desired))  ? this.PreferredWidth : width.Desired;
				double dy = ((height == null) || double.IsNaN (height.Desired)) ? this.PreferredHeight : height.Desired;

				return new Drawing.Size (dx, dy);
			}
		}


		private Drawing.Rectangle GetClientBounds()
		{
			if (this.IsActualGeometryValid)
			{
				return this.Client.Bounds;
			}
			else
			{
				return new Drawing.Rectangle (Drawing.Point.Zero, this.TextLayout.LayoutSize);
			}
		}
		
		public override Drawing.Margins GetInternalPadding()
		{
			Drawing.Rectangle frame  = this.FrameRectangle;
			Drawing.Rectangle title  = this.TitleRectangle;
			Drawing.Rectangle client = this.GetClientBounds ();

			if (title.IsValid)
			{
				frame.Top = title.Bottom;
			}
			
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
				Drawing.Rectangle rect  = this.GetClientBounds ();
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
			WidgetPaintState state   = this.GetPaintState ();
			
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
			adorner.PaintGeneralTextLayout(graphics, clipRect, pos, this.TextLayout, state, PaintTextStyle.Group, TextFieldDisplayMode.Default, this.BackColor);
		}
	}
}
