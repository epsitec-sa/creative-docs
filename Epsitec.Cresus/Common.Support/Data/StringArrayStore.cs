//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 20/02/2004

namespace Epsitec.Common.Support.Data
{
	/// <summary>
	/// Summary description for StringArrayStore.
	/// </summary>
	public class StringArrayStore : ITextArrayStore
	{ 
		public StringArrayStore(string[] array)
		{
			this.array = array;
		}
		
		
		public string						this[int index]
		{
			get
			{
				return this.array[index];
			}
			set
			{
				if (this.array[index] != value)
				{
					this.array[index] = value;
					this.OnStoreChanged ();
				}
			}
		}
		
		public int							Count
		{
			get
			{
				return this.array.Length;
			}
		}
		
		#region ITextArrayStore Members
		public int GetRowCount()
		{
			return this.array.Length;
		}

		public int GetColumnCount()
		{
			return 1;
		}

		public string GetCellText(int row, int column)
		{
			if (column == 0)
			{
				return this.array[row];
			}
			
			return null;
		}

		public void SetCellText(int row, int column, string value)
		{
			if (column == 0)
			{
				this.array[row] = value;
			}
			else
			{
				throw new System.NotSupportedException (string.Format ("Cannot set cell in column {0}.", column));
			}
		}

		public void InsertRows(int row, int num)
		{
			throw new System.NotSupportedException (string.Format ("Cannot insert rows."));
		}

		public void RemoveRows(int row, int num)
		{
			throw new System.NotSupportedException (string.Format ("Cannot remove rows."));
		}

		public void MoveRow(int row, int distance)
		{
			throw new System.NotSupportedException (string.Format ("Cannot move row."));
		}
		
		public bool CheckSetRow(int row)
		{
			return (row >= 0) && (row < this.array.Length);
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
		
		
		public event Epsitec.Common.Support.EventHandler StoreChanged;
		#endregion
		
		protected virtual void OnStoreChanged()
		{
			if (this.StoreChanged != null)
			{
				this.StoreChanged (this);
			}
		}
		
		
		protected string[]					array;
	}
}
