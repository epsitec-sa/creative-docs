//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.DynamicData
{
	/// <summary>
	/// L'interface IDynamicField permet de générer des champs dynamiquement,
	/// en relation avec System.Data.DataTable.
	/// </summary>
	public interface IDynamicField
	{
		FieldMatchResult Match(System.Data.DataRow row);
		FieldMatchResult Match(System.Data.DataColumn column);
		
		bool Match(System.Data.DataRow row, System.Data.DataColumn column);
	}
}
