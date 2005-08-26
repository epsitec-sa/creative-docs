//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// L'interface IDialogDesigner permet d'interagir avec l'éditeur
	/// d'interfaces graphiques. Il y a exactement un IDialogDesigner
	/// par dialogue en cours d'édition.
	/// </summary>
	public interface IDialogDesigner : System.IDisposable
	{
		Widgets.Window				DialogWindow		{ get; set; }
		Types.IDataGraph			DialogData			{ get; set; }
		Support.CommandDispatcher	DialogCommands		{ get; set; }
		string						ResourceName		{ get; set; }
		Support.ResourceManager		ResourceManager		{ get; set; }
		Script.ScriptWrapper		DialogScript		{ get; set; }
		Common.UI.InterfaceType		InterfaceType		{ get; }
		bool						IsEditOnlyInterface	{ get; set; }
		
		void StartDesign();
		
		event Support.EventHandler	Disposed;
	}
}
