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
		IDialogDesigner CreateDialogDesigner(DesignerType type);
	}
	
	
	public enum DesignerType
	{
		Generic,				//	designer g�n�rique (capable de cr�er toute forme d'interface)
		DialogWindow			//	designer permettant d'�diter un dialogue ou une fen�tre
	}
}
