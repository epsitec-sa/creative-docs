//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script.Helpers
{
	/// <summary>
	/// Summary description for ParameterInfoStore.
	/// </summary>
	public class ParameterInfoStore : Support.Data.ITextArrayStore
	{
		public ParameterInfoStore()
		{
			this.hash_dir[Source.ParameterDirection.In.ToString ()]    = Source.ParameterDirection.In;
			this.hash_dir[Source.ParameterDirection.Out.ToString ()]   = Source.ParameterDirection.Out;
			this.hash_dir[Source.ParameterDirection.InOut.ToString ()] = Source.ParameterDirection.InOut;
			
			Types.IntegerType type_int     = new Types.IntegerType ();
			Types.DecimalType type_decimal = new Types.DecimalType ();
			Types.StringType  type_string  = new Types.StringType ();
			
			this.hash_type[type_int.Name]     = type_int;
			this.hash_type[type_decimal.Name] = type_decimal;
			this.hash_type[type_string.Name]  = type_string;
		}
		
		
		public void SetContents(Source.ParameterInfo[] infos)
		{
			this.list.Clear ();
			this.list.AddRange (infos);
		}
		
		public Source.ParameterInfo[] GetContents()
		{
			Source.ParameterInfo[] infos = new Source.ParameterInfo[this.list.Count];
			this.list.CopyTo (infos, 0);
			
			return infos;
		}
		
		
		public void InsertRows(int row, int num)
		{
			this.changing++;
			
			while (num-- > 0)
			{
				int id = 0;
				string name;
				
			again:
				name = "arg" + id.ToString (System.Globalization.CultureInfo.InvariantCulture);
				
				foreach (Source.ParameterInfo iter in this.list)
				{
					if (name == iter.Name)
					{
						id++;
						goto again;
					}
				}
				
				this.list.Insert (row, new Source.ParameterInfo (Source.ParameterDirection.In, new Types.IntegerType (), name));
				
				row++;
			}
			
			this.changing--;
			this.OnStoreContentsChanged ();
		}
		
		public void RemoveRows(int row, int num)
		{
			this.changing++;
			
			while (num-- > 0)
			{
				this.list.RemoveAt (row);
			}
			
			this.changing--;
			this.OnStoreContentsChanged ();
		}
		
		
		public void MoveRow(int row, int distance)
		{
			this.changing++;
			
			object value = this.list[row];
			
			this.list.RemoveAt (row);
			this.list.Insert (row+distance, value);
			
			this.changing--;
			this.OnStoreContentsChanged ();
		}
		
		
		public void   SetCellText(int row, int column, string value)
		{
			this.changing++;
			
			Source.ParameterInfo info = this.list[row] as Source.ParameterInfo;
			
			string dir  = info.Direction.ToString ();
			string type = info.Type.Name;
			string name = info.Name;
			
			switch (column)
			{
				case 0:	dir  = value; break;
				case 1: type = value; break;
				case 2: name = value; break;
			}
			
			info.DefineDirection ((Source.ParameterDirection) this.hash_dir[dir]);
			info.DefineType ((Types.INamedType) this.hash_type[type]);
			info.DefineName (name);
			
			this.changing--;
			this.OnStoreContentsChanged ();
		}
		
		public string GetCellText(int row, int column)
		{
			Source.ParameterInfo info = this.list[row] as Source.ParameterInfo;
			
			switch (column)
			{
				case 0: return info.Direction.ToString ();
				case 1: return info.Type == null ? "" : info.Type.Name;
				case 2: return info.Name == null ? "" : info.Name;
			}
			
			return null;
		}

		
		public int GetColumnCount()
		{
			return 3;
		}
		
		public int GetRowCount()
		{
			return list.Count;
		}
		
		
		public bool CheckInsertRows(int row, int num)
		{
			if ((row >= 0) &&
				(row <= this.list.Count) &&
				(num >= 1))
			{
				return true;
			}
			
			return false;
		}
		
		public bool CheckSetRow(int row)
		{
			if ((row >= 0) &&
				(row < this.list.Count))
			{
				return true;
			}
			
			return false;
		}
		
		public bool CheckMoveRow(int row, int distance)
		{
			int a = row;
			int b = row + distance;
			
			if ((a >= 0) && (a < this.list.Count) &&
				(b >= 0) && (b < this.list.Count))
			{
				return true;
			}
			
			return false;
		}
		
		
		public bool CheckEnabledCell(int row, int column)
		{
			return true;
		}
		
		public bool CheckRemoveRows(int row, int num)
		{
			if ((row >= 0) &&
				(row + num <= this.list.Count) &&
				(num >= 1))
			{
				return true;
			}
			
			return false;
		}
		
		
		
		
		protected virtual void OnStoreContentsChanged()
		{
			if (this.changing == 0)
			{
				if (this.StoreContentsChanged != null)
				{
					this.StoreContentsChanged (this);
				}
			}
		}
		
		
		public event Support.EventHandler		StoreContentsChanged;
		
		
		private int								changing;
		private System.Collections.ArrayList	list      = new System.Collections.ArrayList ();
		private System.Collections.Hashtable	hash_dir  = new System.Collections.Hashtable ();
		private System.Collections.Hashtable	hash_type = new System.Collections.Hashtable ();
	}
}
