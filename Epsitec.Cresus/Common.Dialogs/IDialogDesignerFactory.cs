//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// L'interface IDialogDesignerFactory permet d'instancier des
	/// �diteurs de dialogues correspondant � IDialogDesigner.
	/// </summary>
	public interface IDialogDesignerFactory
	{
		IDialogDesigner CreateDialogDesigner(UI.InterfaceType type);
	}
}
