namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe IconSeparator permet de dessiner des séparations utiles
	/// pour remplir une ToolBar.
	/// </summary>
	public class IconSeparator : Widget
	{
		public IconSeparator()
		{
		}
		
		public IconSeparator(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		public IconSeparator(double breadth)
		{
			this.Breadth = breadth;
		}
		
		
		static IconSeparator()
		{
			Helpers.VisualPropertyMetadata metadataDx = new Helpers.VisualPropertyMetadata (12.0, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			Helpers.VisualPropertyMetadata metadataDy = new Helpers.VisualPropertyMetadata (12.0, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);

			Visual.PreferredWidthProperty.OverrideMetadata (typeof (IconSeparator), metadataDx);
			Visual.PreferredHeightProperty.OverrideMetadata (typeof (IconSeparator), metadataDy);
		}
		
		public bool IsHorizontal
		{
			get
			{
				return this.isHorizontal;
			}

			set
			{
				if ( this.isHorizontal != value )
				{
					this.isHorizontal = value;
				}
			}
		}
		
		public double Breadth
		{
			get
			{
				return this.breadth;
			}

			set
			{
				if ( this.breadth != value )
				{
					this.breadth = value;
					this.PreferredWidth = value;
					this.PreferredHeight = value;
				}
			}
		}
		
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetPaintState       state = this.PaintState;
			
			if ( this.isHorizontal )
			{
				adorner.PaintSeparatorBackground(graphics, rect, state, Direction.Right, true);
			}
			else
			{
				adorner.PaintSeparatorBackground(graphics, rect, state, Direction.Down, true);
			}
		}

		
		protected double				breadth = 12;
		protected bool					isHorizontal = true;
	}
}
