namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe AbstractCellArray est la classe de base pour les tableaux
	/// et les listes.
	/// </summary>
	public abstract class AbstractCellArray : AbstractGroup
	{
		public AbstractCellArray()
		{
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.SetArraySize(0, 0);
			}
			
			base.Dispose(disposing);
		}
		
		
		public int Columns
		{
			get
			{
				return this.maxColumns;
			}
		}
		
		public int Rows
		{
			get
			{
				return this.maxRows;
			}
		}
		
		public int VisibleColumns
		{
			get
			{
				return this.visibleColumns;
			}
		}
		
		public int VisibleRows
		{
			get
			{
				return this.visibleRows;
			}
		}
		
		
		public virtual Cell this[int column, int row]
		{
			get
			{
				return this.array[column, row];
			}
			
			set
			{
				if ( this.array[column, row] != null )
				{
					this.array[column, row].SetArrayRank(null, -1, -1);
				}
				
				this.array[column, row] = value;
				
				if ( this.array[column, row] != null )
				{
					this.array[column, row].SetArrayRank(this, column, row);
				}
			}
		}
		
		
		
		// Les colonnes et les lignes contenues dans le tableau peuvent être escamotées
		// pour permettre de réaliser des tables avec des éléments miniaturisables, par
		// exemple.
		// Les appels MapToVisible et MapFromVisible permettent de convertir entre des
		// positions affichées (colonne/ligne) et des positions dans la table interne.
		
		public virtual void WidthColumns(int start, int count, double width)
		{
			if ( start+count > this.maxColumns )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				if ( i >= start && i < start+count )
				{
					this.widthColumns[i] = width;
				}
			}
		}
		
		public virtual void HideColumns(int start, int count)
		{
			if ( start+count > this.maxColumns )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				if ( i >= start && i < start+count )
				{
					this.widthColumns[i] = 0;
				}
			}
		}
		
		public virtual void ShowColumns(int start, int count)
		{
			if ( start+count > this.maxColumns )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				if ( i >= start && i < start+count )
				{
					this.widthColumns[i] = this.defWidth;
				}
			}
		}
		
		public virtual void HeightRows(int start, int count, double height)
		{
			if ( start+count > this.maxRows )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			for ( int i=0 ; i<this.maxRows ; i++ )
			{
				if ( i >= start && i < start+count )
				{
					this.heightRows[i] = height;
				}
			}
		}
		
		public virtual void HideRows(int start, int count)
		{
			if ( start+count > this.maxRows )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			for ( int i=0 ; i<this.maxRows ; i++ )
			{
				if ( i >= start && i < start+count )
				{
					this.heightRows[i] = 0;
				}
			}
		}
		
		public virtual void ShowRows(int start, int count)
		{
			if ( start+count > this.maxRows )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			for ( int i=0 ; i<this.maxRows ; i++ )
			{
				if ( i >= start && i < start+count )
				{
					this.heightRows[i] = this.defHeight;
				}
			}
		}
		
		
		
		public virtual bool MapToVisible(ref int column, ref int row)
		{
			if ( column < 0 || column >= this.visibleColumns ||
				 row    < 0 || row    >= this.visibleRows    )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			bool foundColumn = false;
			bool foundRow    = false;
			
			int rankColumn = 0;
			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				if ( column == rankColumn )
				{
					column = i;
					foundColumn = true;
					break;
				}
				if ( this.widthColumns[i] != 0 )  rankColumn ++;
			}

			int rankRow = 0;
			for ( int i=0 ; i<this.maxRows ; i++ )
			{
				if ( row == rankRow )
				{
					row = i;
					foundRow = true;
					break;
				}
				if ( this.heightRows[i] != 0 )  rankRow ++;
			}
			
			if ( !foundColumn ) column = -1;
			if ( !foundRow    ) row    = -1;
			
			return ( foundColumn & foundRow );
		}
		
		public virtual bool MapFromVisible(ref int column, ref int row)
		{
			if ( column < 0 || column >= this.maxColumns ||
				 row    < 0 || row    >= this.maxRows    )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			int rankColumn = 0;
			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				if ( column == i )
				{
					if ( this.widthColumns[i] == 0 )
					{
						column = -1;
					}
					else
					{
						column = rankColumn;
					}
					break;
				}
				if ( this.widthColumns[i] != 0 )  rankColumn ++;
			}

			int rankRow = 0;
			for ( int i=0 ; i<this.maxRows ; i++ )
			{
				if ( row == i )
				{
					if ( this.heightRows[i] == 0 )
					{
						row = -1;
					}
					else
					{
						row = rankRow;
					}
					break;
				}
				if ( this.heightRows[i] != 0 )  rankRow ++;
			}
			
			return ( column != -1 & row != -1 );
		}
		
		
		public virtual void SetArraySize(int maxColumns, int maxRows)
		{
			if ( this.maxColumns == maxColumns &&
				 this.maxRows    == maxRows    )
			{
				return;
			}
			
			//	Supprime les colonnes excédentaires.
			for ( int col=maxColumns ; col<this.maxColumns ; col++ )
			{
				for ( int row=0 ; row<this.maxRows ; row++ )
				{
					Cell cell = this.array[col,row];
					
					if ( cell != null )
					{
						cell.SetArrayRank(null, -1, -1);
						this.array[col,row] = null;
					}
				}
			}
			
			//	Supprime les lignes excédentaires.
			for ( int row=maxRows ; row<this.maxRows ; row++ )
			{
				for ( int col=0 ; col<this.maxColumns ; col++ )
				{
					Cell cell = array[col,row];
					
					if ( cell != null )
					{
						cell.SetArrayRank(null, -1, -1);
						this.array[col,row] = null;
					}
				}
			}
			
			//	Alloue un nouveau tableau, puis copie le contenu de l'ancien tableau
			//	dans le nouveau. L'ancien sera perdu.
			Cell[,] newArray = (maxColumns*maxRows > 0) ? new Cell[maxColumns, maxRows] : null;
			
			int minMaxCol = System.Math.Min(maxColumns, this.maxColumns);
			int minMaxRow = System.Math.Min(maxRows,    this.maxRows   );
			
			for ( int col=0 ; col<minMaxCol ; col++ )
			{
				for ( int row=0 ; row<minMaxRow ; row++ )
				{
					newArray[col,row] = this.array[col,row];
				}
			}
			
			this.maxColumns = maxColumns;
			this.maxRows    = maxRows;
			this.array      = newArray;
			
			//	Recrée un mapping 1->1 des lignes et des colonnes.
			this.widthColumns = new double[this.maxColumns];
			this.heightRows   = new double[this.maxRows];
			
			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				this.widthColumns[i] = this.defWidth;
			}
			
			for ( int i=0 ; i<this.maxRows ; i++ )
			{
				this.heightRows[i] = this.defHeight;
			}

			this.visibleColumns = this.maxColumns;
			this.visibleRows    = this.maxRows;
		}
		
		
		protected int					maxColumns;		// nb total de colonnes
		protected int					maxRows;		// nb total de lignes
		protected int					visibleColumns;	// nb de colonnes visibles
		protected int					visibleRows;	// nb de lignes visibles
		protected Cell[,]				array;			// tableau des cellules
		protected double				defWidth = 100;
		protected double				defHeight = 20;
		protected double[]				widthColumns;	// largeurs des colonnes
		protected double[]				heightRows;		// hauteurs des lignes
	}
}
