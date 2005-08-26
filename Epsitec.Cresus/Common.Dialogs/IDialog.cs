//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// L'interface IDialog est commune à tous les dialogues.
	/// </summary>
	public interface IDialog
	{
		void OpenDialog();
		
		Common.Widgets.Window	Owner	{ get; set; }
		DialogResult			Result	{ get; }
	}
}
