//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe DataTable stocke une collection de DataSets.
	/// </summary>
	public class DataTable : DataRecord
	{
		public DataTable()
		{
			this.type = new DataType ();
		}
		
		public override bool					IsTable
		{
			get { return true; }
		}
	}
}
