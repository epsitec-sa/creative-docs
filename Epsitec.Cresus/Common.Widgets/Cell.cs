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
		}
		
		public Cell(Widget widget)
		{
			this.Insert(widget);
		}
		
		public void Insert(Widget widget)
		{
			this.Children.Add(widget);

			Drawing.Rectangle rect = widget.Parent.Bounds;
			rect.Left += 1;
			rect.Top  -= 1;  // laisse la place pour la grille
			widget.Bounds = rect;
			
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
		
		
		public AbstractCellArray CellArray
		{
			get
			{
				return this.cellArray;
			}
		}
		
		
		public int RankColumn
		{
			get
			{
				return this.rankColumn;
			}
		}
		
		public int RankRow
		{
			get
			{
				return this.rankRow;
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
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetState       state = this.PaintState;
			Direction         dir   = this.RootDirection;
			
			adorner.PaintCellBackground(graphics, rect, state, dir);
		}
		
#if false
		// Dessine la cellule.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state = this.PaintState;
			Direction         dir   = this.RootDirection;
			
			if ( this.IsSelected )
			{
				Drawing.Rectangle[] rects = new Drawing.Rectangle[1];
				rects[0] = rect;
				Drawing.Point pos = new Drawing.Point(0,0);
				adorner.PaintTextSelectionBackground(graphics, pos, rects);
			}
		}
#endif
		
		protected AbstractCellArray		cellArray;
		protected int					rankColumn;
		protected int					rankRow;
	}
}
