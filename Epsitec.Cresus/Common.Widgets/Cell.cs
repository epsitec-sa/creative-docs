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
		
		
		public AbstractCellArray CellArray
		{
			get
			{
				return this.Parent as AbstractCellArray;
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
			this.Parent     = array;
			this.rankColumn = column;
			this.rankRow    = row;
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
		
		protected int			rankColumn;
		protected int			rankRow;
	}
}
