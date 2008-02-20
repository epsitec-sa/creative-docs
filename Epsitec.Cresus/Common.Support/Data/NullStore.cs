//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support.Data
{
	/// <summary>
	/// La classe NullStore retourne toujours null, pour tous les éléments du
	/// pseudo-tableau.
	/// </summary>
	public class NullStore : ITextArrayStore
	{
		public NullStore(int rows, int columns)
		{
			this.rows    = rows;
			this.columns = columns;
		}
		
		
		#region ITextArrayStore Members
		public int GetRowCount()
		{
			return this.rows;
		}

		public int GetColumnCount()
		{
			return this.columns;
		}

		
		public string GetCellText(int row, int column)
		{
			return null;
		}

		public void SetCellText(int row, int column, string value)
		{
			throw new System.NotSupportedException ("Cannot SetCellText.");
		}

		
		public void InsertRows(int row, int num)
		{
			throw new System.NotSupportedException ("Cannot InsertRows.");
		}

		public void RemoveRows(int row, int num)
		{
			throw new System.NotSupportedException ("Cannot RemoveRows.");
		}

		public void MoveRow(int row, int distance)
		{
			throw new System.NotSupportedException ("Cannot MoveRow.");
		}

		
		public bool CheckSetRow(int row)
		{
			return false;
		}
		
		public bool CheckInsertRows(int row, int num)
		{
			return false;
		}

		public bool CheckRemoveRows(int row, int num)
		{
			return false;
		}

		public bool CheckMoveRow(int row, int distance)
		{
			return false;
		}
		
		public bool CheckEnabledCell(int row, int column)
		{
			return true;
		}
		
		
		public event Support.EventHandler		StoreContentsChanged;
		#endregion
		
		protected virtual void OnStoreContentsChanged()
		{
			if (this.StoreContentsChanged != null)
			{
				this.StoreContentsChanged (this);
			}
		}
		
		
		public static readonly NullStore		Empty = new NullStore (0, 0);
		
		protected int							rows;
		protected int							columns;
	}
}
