﻿//	Automatically generated by ResGenerator, on 26.04.2007
//	Do not edit manually.

namespace Epsitec.Common.Dialogs
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Commands
		{
			public static class Dialog
			{
				internal static void _Initialize() { }
				
				public static class File
				{
					internal static void _Initialize() { }
					
					public static readonly Epsitec.Common.Widgets.Command Delete = Epsitec.Common.Widgets.Command.Get (Epsitec.Common.Support.Druid.FromLong (_moduleId, 5));
					public static readonly Epsitec.Common.Widgets.Command NavigateNext = Epsitec.Common.Widgets.Command.Get (Epsitec.Common.Support.Druid.FromLong (_moduleId, 7));
					public static readonly Epsitec.Common.Widgets.Command NavigatePrev = Epsitec.Common.Widgets.Command.Get (Epsitec.Common.Support.Druid.FromLong (_moduleId, 6));
					public static readonly Epsitec.Common.Widgets.Command NewFolder = Epsitec.Common.Widgets.Command.Get (Epsitec.Common.Support.Druid.FromLong (_moduleId, 3));
					public static readonly Epsitec.Common.Widgets.Command ParentFolder = Epsitec.Common.Widgets.Command.Get (Epsitec.Common.Support.Druid.FromLong (_moduleId, 1));
					public static readonly Epsitec.Common.Widgets.Command Refresh = Epsitec.Common.Widgets.Command.Get (Epsitec.Common.Support.Druid.FromLong (_moduleId, 20));
					public static readonly Epsitec.Common.Widgets.Command Rename = Epsitec.Common.Widgets.Command.Get (Epsitec.Common.Support.Druid.FromLong (_moduleId, 4));
					public static readonly Epsitec.Common.Widgets.Command ViewDisposition = Epsitec.Common.Widgets.Command.Get (Epsitec.Common.Support.Druid.FromLong (_moduleId, 18));
					public static readonly Epsitec.Common.Widgets.Command ViewSize = Epsitec.Common.Widgets.Command.Get (Epsitec.Common.Support.Druid.FromLong (_moduleId, 19));
					public static class Favorites
					{
						internal static void _Initialize() { }
						
						public static readonly Epsitec.Common.Widgets.Command Add = Epsitec.Common.Widgets.Command.Get (Epsitec.Common.Support.Druid.FromLong (_moduleId, 8));
						public static readonly Epsitec.Common.Widgets.Command Down = Epsitec.Common.Widgets.Command.Get (Epsitec.Common.Support.Druid.FromLong (_moduleId, 11));
						public static readonly Epsitec.Common.Widgets.Command Remove = Epsitec.Common.Widgets.Command.Get (Epsitec.Common.Support.Druid.FromLong (_moduleId, 9));
						public static readonly Epsitec.Common.Widgets.Command ToggleSize = Epsitec.Common.Widgets.Command.Get (Epsitec.Common.Support.Druid.FromLong (_moduleId, 12));
						public static readonly Epsitec.Common.Widgets.Command Up = Epsitec.Common.Widgets.Command.Get (Epsitec.Common.Support.Druid.FromLong (_moduleId, 10));
					}
				}
			}
			
			internal static void _Initialize()
			{
				Dialog.File._Initialize ();
				Dialog.File.Favorites._Initialize ();
			}
		}
		
		public static class CommandIds
		{
			public static class Dialog
			{
				
				public static class File
				{
					
					public const long Delete = 0x600000000005L;
					public const long NavigateNext = 0x600000000007L;
					public const long NavigatePrev = 0x600000000006L;
					public const long NewFolder = 0x600000000003L;
					public const long ParentFolder = 0x600000000001L;
					public const long Refresh = 0x600000000014L;
					public const long Rename = 0x600000000004L;
					public const long ViewDisposition = 0x600000000012L;
					public const long ViewSize = 0x600000000013L;
					public static class Favorites
					{
						
						public const long Add = 0x600000000008L;
						public const long Down = 0x60000000000BL;
						public const long Remove = 0x600000000009L;
						public const long ToggleSize = 0x60000000000CL;
						public const long Up = 0x60000000000AL;
					}
				}
			}
		}
		
		public static class Captions
		{
			public static class File
			{
				public static class Column
				{
					public static Epsitec.Common.Types.Caption Date { get { return Res._manager.GetCaption (Epsitec.Common.Support.Druid.FromLong (_moduleId, 15)); } }
					public static Epsitec.Common.Types.Caption Icon { get { return Res._manager.GetCaption (Epsitec.Common.Support.Druid.FromLong (_moduleId, 0)); } }
					public static Epsitec.Common.Types.Caption Info { get { return Res._manager.GetCaption (Epsitec.Common.Support.Druid.FromLong (_moduleId, 17)); } }
					public static Epsitec.Common.Types.Caption Name { get { return Res._manager.GetCaption (Epsitec.Common.Support.Druid.FromLong (_moduleId, 14)); } }
					public static Epsitec.Common.Types.Caption Size { get { return Res._manager.GetCaption (Epsitec.Common.Support.Druid.FromLong (_moduleId, 16)); } }
				}
			}
			
		}
		
		//	Code mapping for 'String' resources
		
		public static class Strings
		{
			public static class Dialog
			{
				public static class File
				{
					public static string Directory { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (10)); } }
					public static string Document { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (11)); } }
					public static string Label { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (17)); } }
					public static string Model { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (21)); } }
					public static string NewDirectoryName { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (22)); } }
					public static string NewEmptyDocument { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (35)); } }
					public static string Statistics { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (26)); } }
					public static class Button
					{
						public static string New { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (7)); } }
						public static string Open { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (8)); } }
						public static string Save { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (9)); } }
					}
					
					public static class Header
					{
						public static string Date { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (12)); } }
						public static string Description { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (13)); } }
						public static string FileName { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (14)); } }
						public static string Preview { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (15)); } }
						public static string Size { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (16)); } }
					}
					
					public static class LabelPath
					{
						public static string Open { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (19)); } }
						public static string Save { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (20)); } }
					}
					
					public static class Size
					{
						public static string Giga { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (23)); } }
						public static string Kilo { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (24)); } }
						public static string Mega { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (25)); } }
					}
					
					public static class Tooltip
					{
						public static string ExtendInclude { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (4)); } }
						public static string ExtendToolbar { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (3)); } }
						public static string PreviewSize { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (5)); } }
						public static string VisitedMenu { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (6)); } }
					}
				}
				
				public static class Generic
				{
					public static string Title { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (36)); } }
					public static class Button
					{
						public static string Cancel { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (31)); } }
						public static string Close { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (32)); } }
						public static string Help { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (33)); } }
						public static string Ok { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (34)); } }
					}
				}
				
				public static class Question
				{
					public static class Open
					{
						public static string File { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (27)); } }
					}
					
					public static class Save
					{
						public static string File { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (28)); } }
						public static string Part1 { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (29)); } }
						public static string Part2 { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (30)); } }
					}
				}
				
				public static class Tooltip
				{
					public static string Close { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (0)); } }
					public static string Help { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (1)); } }
					public static string Resize { get { return Epsitec.Common.Dialogs.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (2)); } }
				}
			}
			
			public static string GetString(params string[] path)
			{
				string field = string.Join (".", path);
				return _stringsBundle[field].AsString;
			}
			
			#region Internal Support Code
			private static string GetText(string bundle, params string[] path)
			{
				string field = string.Join (".", path);
				return _stringsBundle[field].AsString;
			}
			private static string GetText(Epsitec.Common.Support.Druid druid)
			{
				return _stringsBundle[druid].AsString;
			}
			private static Epsitec.Common.Support.ResourceBundle _stringsBundle = Res._manager.GetBundle ("Strings");
			#endregion
		}
		
		//	Code mapping for 'Panel' resources
		
		static Res()
		{
			Res.Initialize (typeof (Res), "Common.Dialogs");
		}

		public static void Initialize()
		{
		}

		private static void Initialize(System.Type type, string name)
		{
			Res._manager = new Epsitec.Common.Support.ResourceManager (type);
			Res._manager.DefineDefaultModuleName (name);
			Commands._Initialize ();
		}
		
		public static Epsitec.Common.Support.ResourceManager Manager
		{
			get { return Res._manager; }
		}
		
		public static int ModuleId
		{
			get { return _moduleId; }
		}
		
		private static Epsitec.Common.Support.ResourceManager _manager = Epsitec.Common.Support.Resources.DefaultManager;
		private const int _moduleId = 6;
	}
}
