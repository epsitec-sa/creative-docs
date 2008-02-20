//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
