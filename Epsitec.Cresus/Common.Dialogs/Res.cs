﻿//	Automatically generated by ResGenerator, on 13.09.2007
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
					
					public static readonly global::Epsitec.Common.Widgets.Command Delete = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 5));
					public static readonly global::Epsitec.Common.Widgets.Command NavigateNext = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 7));
					public static readonly global::Epsitec.Common.Widgets.Command NavigatePrev = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 6));
					public static readonly global::Epsitec.Common.Widgets.Command NewFolder = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 3));
					public static readonly global::Epsitec.Common.Widgets.Command ParentFolder = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 1));
					public static readonly global::Epsitec.Common.Widgets.Command Refresh = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 20));
					public static readonly global::Epsitec.Common.Widgets.Command Rename = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 4));
					public static readonly global::Epsitec.Common.Widgets.Command ViewDisposition = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 18));
					public static readonly global::Epsitec.Common.Widgets.Command ViewSize = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 19));
					public static class Favorites
					{
						internal static void _Initialize() { }
						
						public static readonly global::Epsitec.Common.Widgets.Command Add = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 8));
						public static readonly global::Epsitec.Common.Widgets.Command Down = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 11));
						public static readonly global::Epsitec.Common.Widgets.Command Remove = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 9));
						public static readonly global::Epsitec.Common.Widgets.Command ToggleSize = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 12));
						public static readonly global::Epsitec.Common.Widgets.Command Up = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 10));
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
					public static global::Epsitec.Common.Types.Caption Date { get { return Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 15)); } }
					public static global::Epsitec.Common.Types.Caption Icon { get { return Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 0)); } }
					public static global::Epsitec.Common.Types.Caption Info { get { return Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 17)); } }
					public static global::Epsitec.Common.Types.Caption Name { get { return Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 14)); } }
					public static global::Epsitec.Common.Types.Caption Size { get { return Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 16)); } }
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
					public static string Directory { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (10)); } }
					public static string Document { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (11)); } }
					public static string Label { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (17)); } }
					public static string Model { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (21)); } }
					public static string NewDirectoryName { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (22)); } }
					public static string NewEmptyDocument { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (35)); } }
					public static string Statistics { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (26)); } }
					public static class Button
					{
						public static string New { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (7)); } }
						public static string Open { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (8)); } }
						public static string Save { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (9)); } }
					}
					
					public static class Header
					{
						public static string Date { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (12)); } }
						public static string Description { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (13)); } }
						public static string FileName { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (14)); } }
						public static string Preview { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (15)); } }
						public static string Size { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (16)); } }
					}
					
					public static class LabelPath
					{
						public static string Open { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (19)); } }
						public static string Save { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (20)); } }
					}
					
					public static class Size
					{
						public static string Giga { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (23)); } }
						public static string Kilo { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (24)); } }
						public static string Mega { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (25)); } }
					}
					
					public static class Tooltip
					{
						public static string ExtendInclude { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (4)); } }
						public static string ExtendToolbar { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (3)); } }
						public static string PreviewSize { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (5)); } }
						public static string VisitedMenu { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (6)); } }
					}
				}
				
				public static class Generic
				{
					public static string Title { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (36)); } }
					public static class Button
					{
						public static string Cancel { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (31)); } }
						public static string Close { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (32)); } }
						public static string Help { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (33)); } }
						public static string Ok { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (34)); } }
					}
				}
				
				public static class Question
				{
					public static class Open
					{
						public static string File { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (27)); } }
					}
					
					public static class Save
					{
						public static string File { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (28)); } }
						public static string Part1 { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (29)); } }
						public static string Part2 { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (30)); } }
					}
				}
				
				public static class Tooltip
				{
					public static string Close { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (0)); } }
					public static string Help { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (1)); } }
					public static string Resize { get { return global::Epsitec.Common.Dialogs.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (2)); } }
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
			private static string GetText(global::Epsitec.Common.Support.Druid druid)
			{
				return _stringsBundle[druid].AsString;
			}
			private static global::Epsitec.Common.Support.ResourceBundle _stringsBundle = Res._manager.GetBundle ("Strings");
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
			Res._manager = new global::Epsitec.Common.Support.ResourceManager (type);
			Res._manager.DefineDefaultModuleName (name);
			Commands._Initialize ();
		}
		
		public static global::Epsitec.Common.Support.ResourceManager Manager
		{
			get { return Res._manager; }
		}
		
		public static int ModuleId
		{
			get { return _moduleId; }
		}
		
		private static global::Epsitec.Common.Support.ResourceManager _manager = global::Epsitec.Common.Support.Resources.DefaultManager;
		private const int _moduleId = 6;
	}
}
