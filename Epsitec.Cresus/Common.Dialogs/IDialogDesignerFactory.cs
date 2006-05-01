//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// L'interface IDialogDesignerFactory permet d'instancier des
	/// éditeurs de dialogues correspondant à IDialogDesigner.
	/// </summary>
	public interface IDialogDesignerFactory
	{
		IDialogDesigner CreateDialogDesigner();
	}
}
