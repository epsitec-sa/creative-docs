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
		FieldMatch Match(System.Data.DataRow row);
		FieldMatch Match(System.Data.DataColumn column);
	}
}
