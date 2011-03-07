namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Cell implémente un conteneur pour peupler des tableaux et
	/// des grilles.
	/// </summary>
	public class Cell : AbstractGroup
	{
		public Cell()
		{
			this.InheritsParentFocus = true;
		}
		
		public Cell(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		public void Insert(Widget widget)
		{
			widget.SetParent (this);

			if ((widget.Dock == DockStyle.None) &&
				(widget.Anchor == AnchorStyles.None))
			{
				widget.SetManualBounds(widget.Parent.ActualBounds);
			}
			
			if ( this.cellArray != null )
			{
				this.cellArray.NotifyCellChanged(this);
			}
		}
		
		public void Remove(Widget widget)
		{
			this.Children.Remove(widget);
			
			if ( this.cellArray != null )
			{
				this.cellArray.NotifyCellChanged(this);
			}
		}
		
		public void Clear()
		{
			this.Children.Clear ();
			
			if ( this.cellArray != null )
			{
				this.cellArray.NotifyCellChanged(this);
			}
		}
		
		
		public AbstractCellArray CellArray
		{
			get
			{
				return this.cellArray;
			}
		}
		
		
		public int						RankColumn
		{
			get
			{
				return this.rankColumn;
			}
		}
		
		public int						RankRow
		{
			get
			{
				return this.rankRow;
			}
		}


		public bool						IsHilite
		{
			get
			{
				return this.isHilite;
			}
			
			set
			{
				if ( this.isHilite != value )
				{
					this.isHilite = value;
					this.Invalidate();
				}
			}
		}
		
		public bool						IsFlyOver
		{
			get
			{
				return this.isFlyOver;
			}
			
			set
			{
				if ( this.isFlyOver != value )
				{
					this.isFlyOver = value;
					this.Invalidate();
				}
			}
		}
		
		
		internal void SetArrayRank(AbstractCellArray array, int column, int row)
		{
			this.cellArray  = array;
			this.rankColumn = column;
			this.rankRow    = row;
		}
		

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetPaintState       state = this.GetPaintState ();
			
			adorner.PaintCellBackground(graphics, rect, state);

			if (!this.BackColor.IsEmpty)
			{
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (this.BackColor);
			}

			if ( this.isHilite )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.cellArray.HiliteColor);
			}

			if ( this.isFlyOver )
			{
				Drawing.Color color = Drawing.Color.FromAlphaColor(0.2, adorner.ColorCaption);
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(color);
			}
		}
		
		
		protected AbstractCellArray		cellArray;
		protected int					rankColumn;
		protected int					rankRow;
		protected bool					isHilite;
		protected bool					isFlyOver;
	}
}
