//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.UI.Data
{
	/// <summary>
	/// L'�num�ration Representation d�termine comment Widgets.DataWidget repr�sente
	/// une donn�e.
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
