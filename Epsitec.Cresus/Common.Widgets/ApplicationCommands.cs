//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ApplicationCommands</c> class defines the commands which are the most
	/// used by an application.
	/// </summary>
	public static class ApplicationCommands
	{
		public static readonly Command Cut			= Res.Commands.Cut;
		public static readonly Command Copy			= Res.Commands.Copy;
		public static readonly Command Paste		= Res.Commands.Paste;
		public static readonly Command Delete		= Res.Commands.Delete;
		public static readonly Command SelectAll	= Res.Commands.SelectAll;
		
		//	Référence: http://winfx.msdn.microsoft.com/library/default.asp?url=/library/en-us/cpref/html/T_System_Windows_Input_ApplicationCommands_Members.asp

		
		public static readonly string Close			= "Close";
		public static readonly string	ContextMenu		= "ContextMenu";
		public static readonly string	CorrectionList	= "CorrectionList";
		public static readonly string	Find			= "Find";
		public static readonly string	Help			= "Help";
		public static readonly string	New				= "New";
		public static readonly string	Open			= "Open";
		public static readonly string	Print			= "Print";
		public static readonly string	PrintPreview	= "PrintPreview";
		public static readonly string	Properties		= "Properties";
		public static readonly string	Redo			= "Redo";
		public static readonly string	Replace			= "Replace";
		public static readonly string	Save			= "Save";
		public static readonly string	SaveAs			= "SaveAs";
		public static readonly string	Stop			= "Stop";
		public static readonly string	Undo			= "Undo";
	}
}
