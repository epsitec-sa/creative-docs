//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 22/03/2004

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// L'interface IDialog est commune à tous les dialogues.
	/// </summary>
	public interface IDialog
	{
		void Show();
		Common.Widgets.Window Owner { get; set; }
	}
}
