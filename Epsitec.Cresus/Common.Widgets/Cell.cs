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
		
		public Cell(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		public void Insert(Widget widget)
		{
			widget.Parent = this;
			widget.Bounds = widget.Parent.Bounds;
			
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
			
			adorner.PaintCellBackground(graphics, rect, state);
		}
		
		
		protected AbstractCellArray		cellArray;
		protected int					rankColumn;
		protected int					rankRow;
	}
}
