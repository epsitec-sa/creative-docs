//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// L'interface IDialogDesignerFactory permet d'instancier des
	/// éditeurs de dialogues correspondant à IDialogDesigner.
	/// </summary>
	public interface IDialogDesignerFactory
	{
		IDialogDesigner CreateDialogDesigner(DesignerType type);
	}
	
	
	public enum DesignerType
	{
		Generic,				//	designer générique (capable de créer toute forme d'interface)
		DialogWindow			//	designer permettant d'éditer un dialogue ou une fenêtre
	}
}
