//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.UI.Data
{
	/// <summary>
	/// L'énumération Representation détermine comment Widgets.DataWidget représente
	/// une donnée.
	/// </summary>
	public enum Representation
	{
		None,
		
		Automatic,
		
		TextField,
		
		NumericUpDown,
		
		ComboConstantList,
		ComboEditableList,
		
		RadioList,
		RadioRows,
		RadioColumns,
	}
}
