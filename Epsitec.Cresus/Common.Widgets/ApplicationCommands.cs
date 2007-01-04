//	Copyright � 2005-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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

		public static readonly Command Clear		= Res.Commands.Clear;
		public static readonly Command Accept		= Res.Commands.Accept;
		public static readonly Command Reject		= Res.Commands.Reject;

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
			public const long Cut			= Res.CommandIds.Cut;
			public const long Copy			= Res.CommandIds.Copy;
			public const long Paste			= Res.CommandIds.Paste;
			public const long Delete		= Res.CommandIds.Delete;
			public const long SelectAll		= Res.CommandIds.SelectAll;
			public const long Undo			= Res.CommandIds.Undo;
			public const long Redo			= Res.CommandIds.Redo;
			public const long Find			= Res.CommandIds.Find;

			public const long Clear			= Res.CommandIds.Clear;
			public const long Accept		= Res.CommandIds.Accept;
			public const long Reject		= Res.CommandIds.Reject;
			
			public const long Help			= Res.CommandIds.Help;
			public const long Quit			= Res.CommandIds.Quit;

			public const long New			= Res.CommandIds.New;
			public const long Open			= Res.CommandIds.Open;
			public const long Save			= Res.CommandIds.Save;
			public const long SaveAs		= Res.CommandIds.SaveAs;
			public const long Close			= Res.CommandIds.Close;
			public const long Print			= Res.CommandIds.Print;
			public const long PrintPreview	= Res.CommandIds.PrintPreview;
			public const long Properties	= Res.CommandIds.Properties;
			public const long Stop			= Res.CommandIds.Stop;
			public const long Refresh		= Res.CommandIds.Refresh;
		}
		
		//	R�f�rence: http://winfx.msdn.microsoft.com/library/default.asp?url=/library/en-us/cpref/html/T_System_Windows_Input_ApplicationCommands_Members.asp

		
		public static readonly string	ContextMenu		= "ContextMenu";
		public static readonly string	CorrectionList	= "CorrectionList";
		public static readonly string	Replace			= "Replace";
	}
}
