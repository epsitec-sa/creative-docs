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
		public static readonly Command Undo			= Res.Commands.Undo;
		public static readonly Command Redo			= Res.Commands.Redo;
		public static readonly Command Find			= Res.Commands.Find;

		public static readonly Command Help			= Res.Commands.Help;
		public static readonly Command Quit			= Res.Commands.Quit;
		
		public static readonly Command New			= Res.Commands.New;
		public static readonly Command Open			= Res.Commands.Open;
		public static readonly Command Save			= Res.Commands.Save;
		public static readonly Command SaveAs		= Res.Commands.SaveAs;
		public static readonly Command Close		= Res.Commands.Close;
		public static readonly Command Print		= Res.Commands.Print;
		public static readonly Command PrintPreview = Res.Commands.PrintPreview;
		public static readonly Command Properties	= Res.Commands.Properties;
		public static readonly Command Stop			= Res.Commands.Stop;
		public static readonly Command Refresh		= Res.Commands.Refresh;

		public static class Id
		{
			public const long Cut			= Res.Commands.Cut_Id;
			public const long Copy			= Res.Commands.Copy_Id;
			public const long Paste			= Res.Commands.Paste_Id;
			public const long Delete		= Res.Commands.Delete_Id;
			public const long SelectAll		= Res.Commands.SelectAll_Id;
			public const long Undo			= Res.Commands.Undo_Id;
			public const long Redo			= Res.Commands.Redo_Id;
			public const long Find			= Res.Commands.Find_Id;

			public const long Help			= Res.Commands.Help_Id;
			public const long Quit			= Res.Commands.Quit_Id;

			public const long New			= Res.Commands.New_Id;
			public const long Open			= Res.Commands.Open_Id;
			public const long Save			= Res.Commands.Save_Id;
			public const long SaveAs		= Res.Commands.SaveAs_Id;
			public const long Close			= Res.Commands.Close_Id;
			public const long Print			= Res.Commands.Print_Id;
			public const long PrintPreview	= Res.Commands.PrintPreview_Id;
			public const long Properties	= Res.Commands.Properties_Id;
			public const long Stop			= Res.Commands.Stop_Id;
			public const long Refresh		= Res.Commands.Refresh_Id;
		}
		
		//	Référence: http://winfx.msdn.microsoft.com/library/default.asp?url=/library/en-us/cpref/html/T_System_Windows_Input_ApplicationCommands_Members.asp

		
		public static readonly string	ContextMenu		= "ContextMenu";
		public static readonly string	CorrectionList	= "CorrectionList";
		public static readonly string	Replace			= "Replace";
	}
}
