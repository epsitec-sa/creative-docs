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
			if (disposing)
			{
				this.SetArraySize (0, 0);
			}
			
			base.Dispose (disposing);
		}
		
		
		public int						Columns
		{
			get { return this.max_columns; }
		}
		
		public int						Rows
		{
			get { return this.max_rows; }
		}
		
		
		public virtual Cell				this[int column, int row]
		{
			get
			{
				return this.array[column, row];
			}
			
			set
			{
				if (this.array[column, row] != null)
				{
					this.array[column, row].SetArrayRank (null, -1, -1);
				}
				
				this.array[column, row] = value;
				
				if (this.array[column, row] != null)
				{
					this.array[column, row].SetArrayRank (this, column, row);
				}
			}
		}
		
		
		
		/*
		 * Les colonnes et les lignes contenues dans le tableau peuvent être escamotées
		 * pour permettre de réaliser des tables avec des éléments miniaturisables, par
		 * exemple.
		 * 
		 * Les appels MapToVisible et MapFromVisible permettent de convertir entre des
		 * positions affichées (colonne/ligne) et des positions dans la table interne.
		 */
		
		public virtual void HideColumns(int start, int count)
		{
			if (start + count > this.max_columns)
			{
				throw new System.ArgumentOutOfRangeException ();
			}
			
			//	TODO: passe en revue les colonnes spécifiées et cache toutes les cellules
			//	concernées. De plus, ajuste map_columns[] pour que les colonnes cachées
			//	réfèrent sur -1, et que les colonnes visibles pointent sur une séquence
			//	continue de 0..n.
		}
		
		public virtual void ShowColumns(int start, int count)
		{
			if (start + count > this.max_columns)
			{
				throw new System.ArgumentOutOfRangeException ();
			}
			
			//	TODO: passe en revue les colonnes spécifiées et montre toutes les cellules
			//	concernées. De plus, ajuste map_columns[] pour que les colonnes visibles
			//	pointent sur une séquence continue de 0..n.
		}
		
		public virtual void HideRows(int start, int count)
		{
			if (start + count > this.max_rows)
			{
				throw new System.ArgumentOutOfRangeException ();
			}
			
			//	TODO: voir HideColumns
		}
		
		public virtual void ShowRows(int start, int count)
		{
			if (start + count > this.max_rows)
			{
				throw new System.ArgumentOutOfRangeException ();
			}
			
			//	TODO: voir ShowColumns
		}
		
		
		
		public virtual bool MapToVisible(ref int column, ref int row)
		{
			if ((column >= this.max_columns) ||
				(row >= this.max_rows) ||
				(column < 0) ||
				(row < 0))
			{
				throw new System.ArgumentOutOfRangeException ();
			}
			
			column = this.map_columns[column];
			row    = this.map_rows[row];
			
			return (column >= 0) & (row >= 0);
		}
		
		public virtual bool MapFromVisible(ref int column, ref int row)
		{
			bool found_column = false;
			bool found_row    = false;
			
			for (int i = 0; i < this.max_columns; i++)
			{
				if (this.map_columns[i] == column)
				{
					column = i;
					found_column = true;
					break;
				}
			}
			
			for (int i = 0; i < this.max_rows; i++)
			{
				if (this.map_rows[i] == row)
				{
					row = i;
					found_row = true;
					break;
				}
			}
			
			if (!found_column) column = -1;
			if (!found_row) row = -1;
			
			return found_column & found_row;
		}
		
		
		public virtual void SetArraySize(int max_columns, int max_rows)
		{
			if ((this.max_columns == max_columns) &&
				(this.max_rows == max_rows))
			{
				return;
			}
			
			//	Supprime les colonnes excédentaires.
			
			for (int col = max_columns; col < this.max_columns; col++)
			{
				for (int row = 0; row < this.max_rows; row++)
				{
					Cell cell = this.array[col,row];
					
					if (cell != null)
					{
						cell.SetArrayRank (null, -1, -1);
						this.array[col,row] = null;
					}
				}
			}
			
			//	Supprime les lignes excédentaires.
			
			for (int row = max_rows; row < this.max_rows; row++)
			{
				for (int col = 0; col < this.max_columns; col++)
				{
					Cell cell = array[col,row];
					
					if (cell != null)
					{
						cell.SetArrayRank (null, -1, -1);
						this.array[col,row] = null;
					}
				}
			}
			
			//	Alloue un nouveau tableau, puis copie le contenu de l'ancien tableau
			//	dans le nouveau. L'ancien sera perdu.
			
			Cell[,] new_array = (max_columns * max_rows > 0) ? new Cell[max_columns, max_rows] : null;
			
			int min_max_col = System.Math.Min (max_columns, this.max_columns);
			int min_max_row = System.Math.Min (max_rows, this.max_rows);
			
			for (int col = 0; col < min_max_col; col++)
			{
				for (int row = 0; row < min_max_row; row++)
				{
					new_array[col,row] = this.array[col,row];
				}
			}
			
			this.max_columns = max_columns;
			this.max_rows    = max_rows;
			this.array       = new_array;
			
			//	Recrée un mapping 1->1 des lignes et des colonnes.
			
			this.map_columns = new int[this.max_columns];
			this.map_rows    = new int[this.max_rows];
			
			for (int i = 0; i < this.max_columns; i++)
			{
				this.map_columns[i] = i;
			}
			
			for (int i = 0; i < this.max_rows; i++)
			{
				this.map_rows[i] = i;
			}
		}
		
		
		protected int					max_columns;
		protected int					max_rows;
		protected Cell[,]				array;
		protected int[]					map_columns;
		protected int[]					map_rows;
	}
}
