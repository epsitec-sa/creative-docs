﻿//	Automatically generated by ResGenerator, on 12.04.2006 07:31:28
//	Do not edit manually.

namespace Epsitec.Common.Designer
{
	public sealed class Res
	{
		public sealed class Strings
		{
			public sealed class Action
			{
				public static string Clipboard { get { return GetText ("Strings", "Action", "Clipboard"); } }
				public static string Close { get { return GetText ("Strings", "Action", "Close"); } }
				public static string CloseAll { get { return GetText ("Strings", "Action", "CloseAll"); } }
				public static string Copy { get { return GetText ("Strings", "Action", "Copy"); } }
				public static string Cut { get { return GetText ("Strings", "Action", "Cut"); } }
				public static string Delete { get { return GetText ("Strings", "Action", "Delete"); } }
				public static string Duplicate { get { return GetText ("Strings", "Action", "Duplicate"); } }
				public static string FileMain { get { return GetText ("Strings", "Action", "FileMain"); } }
				public static string LastFiles { get { return GetText ("Strings", "Action", "LastFiles"); } }
				public static string New { get { return GetText ("Strings", "Action", "New"); } }
				public static string Open { get { return GetText ("Strings", "Action", "Open"); } }
				public static string Paste { get { return GetText ("Strings", "Action", "Paste"); } }
				public static string Quit { get { return GetText ("Strings", "Action", "Quit"); } }
				public static string Save { get { return GetText ("Strings", "Action", "Save"); } }
				public static string SaveAs { get { return GetText ("Strings", "Action", "SaveAs"); } }
			}
			
			public sealed class Application
			{
				public static string Title { get { return GetText ("Strings", "Application", "Title"); } }
			}
			
			public sealed class Culture
			{
				public static string ch { get { return GetText ("Strings", "Culture", "ch"); } }
				public static string de { get { return GetText ("Strings", "Culture", "de"); } }
				public static string en { get { return GetText ("Strings", "Culture", "en"); } }
				public static string fr { get { return GetText ("Strings", "Culture", "fr"); } }
			}
			
			public sealed class Misc
			{
				public static string Copy { get { return GetText ("Strings", "Misc", "Copy"); } }
				public static string CopyOf { get { return GetText ("Strings", "Misc", "CopyOf"); } }
				public static string Extract { get { return GetText ("Strings", "Misc", "Extract"); } }
				public static string ExtractOf { get { return GetText ("Strings", "Misc", "ExtractOf"); } }
				public static string NoTitle { get { return GetText ("Strings", "Misc", "NoTitle"); } }
			}
			
			public sealed class Ribbon
			{
				public static string Main { get { return GetText ("Strings", "Ribbon", "Main"); } }
				public static string Oper { get { return GetText ("Strings", "Ribbon", "Oper"); } }
			}
			
			public sealed class String
			{
				public static string About { get { return GetText ("Strings", "String", "About"); } }
				public static string Edit { get { return GetText ("Strings", "String", "Edit"); } }
			}
			
			public static string GetString(params string[] path)
			{
				string field = string.Join (".", path);
				return _bundle[field].AsString;
			}
			
			#region Internal Support Code
			private static string GetText(string bundle, params string[] path)
			{
				string field = string.Join (".", path);
				return _bundle[field].AsString;
			}
			private static Epsitec.Common.Support.ResourceBundle _bundle = _manager.GetBundle ("Strings");
			#endregion
		}
		
		public static void Initialise(System.Type type, string name)
		{
			_manager = new Epsitec.Common.Support.ResourceManager (type);
			_manager.SetupApplication (name);
		}
		
		private static Epsitec.Common.Support.ResourceManager _manager = Epsitec.Common.Support.Resources.DefaultManager;
	}
}
