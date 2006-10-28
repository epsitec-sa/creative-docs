namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe IconSeparator permet de dessiner des s�parations utiles
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
			Types.DependencyPropertyMetadata metadataDx = Visual.PreferredWidthProperty.DefaultMetadata.Clone ();
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataDx.DefineDefaultValue (12.0);
			metadataDy.DefineDefaultValue (12.0);

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
