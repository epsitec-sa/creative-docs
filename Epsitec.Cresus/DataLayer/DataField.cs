//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe DataField stocke une donnée unique.
	/// </summary>
	public class DataField : DataRecord
	{
		public DataField()
		{
		}
		
		public override bool					IsField
		{
			get { return true; }
		}
	}
}
