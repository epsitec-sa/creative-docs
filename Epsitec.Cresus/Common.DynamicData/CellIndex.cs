//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.DynamicData
{
	/// <summary>
	/// La structure CellIndex décrit l'index ligne/colonne d'une cellule dans une
	/// table 
	/// </summary>
	
	[System.Serializable]
	
	public struct CellIndex
	{
		public CellIndex(int row, int column)
		{
			this.row    = row + 1;
			this.column = column + 1;
		}
		
		
		public int							Row
		{
			get
			{
				return this.row - 1;
			}
			set
			{
				this.row = value + 1;
			}
		}
		
		public int							Column
		{
			get
			{
				return this.column - 1;
			}
			set
			{
				this.column = value + 1;
			}
		}
		
		
		private int							row;
		private int							column;
	}
}
