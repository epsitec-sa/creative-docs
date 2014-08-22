//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

namespace Epsitec.Cresus.Assets.App
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Commands
		{
			public static class Edit
			{
				internal static void _Initialize()
				{
					global::System.Object.Equals (Edit.Accept, null);
				}
				
				//	designer:cap/JUK6001
				public static readonly global::Epsitec.Common.Widgets.Command Accept = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 6));
				//	designer:cap/JUK7001
				public static readonly global::Epsitec.Common.Widgets.Command Cancel = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 7));
			}
			
			public static class View
			{
				internal static void _Initialize()
				{
					global::System.Object.Equals (View.Categories, null);
				}
				
				//	designer:cap/JUK1001
				public static readonly global::Epsitec.Common.Widgets.Command Categories = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 1));
				//	designer:cap/JUK3001
				public static readonly global::Epsitec.Common.Widgets.Command Events = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 3));
				//	designer:cap/JUK2001
				public static readonly global::Epsitec.Common.Widgets.Command Groups = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 2));
				//	designer:cap/JUK0001
				public static readonly global::Epsitec.Common.Widgets.Command Objects = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 0));
				//	designer:cap/JUK4001
				public static readonly global::Epsitec.Common.Widgets.Command Reports = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 4));
				//	designer:cap/JUK5001
				public static readonly global::Epsitec.Common.Widgets.Command Settings = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 5));
			}
		}
		
		public static class CommandIds
		{
			public static class Edit
			{
				//	designer:cap/JUK6001
				public const long Accept = 0x7D300014000006L;
				//	designer:cap/JUK7001
				public const long Cancel = 0x7D300014000007L;
			}
			
			public static class View
			{
				//	designer:cap/JUK1001
				public const long Categories = 0x7D300014000001L;
				//	designer:cap/JUK3001
				public const long Events = 0x7D300014000003L;
				//	designer:cap/JUK2001
				public const long Groups = 0x7D300014000002L;
				//	designer:cap/JUK0001
				public const long Objects = 0x7D300014000000L;
				//	designer:cap/JUK4001
				public const long Reports = 0x7D300014000004L;
				//	designer:cap/JUK5001
				public const long Settings = 0x7D300014000005L;
			}
			
		}
		
		//	Code mapping for 'String' resources
		
		public static class Strings
		{
			internal static void _Initialize()
			{
				global::System.Object.Equals (_stringsBundle, null);
			}
			
			public static class Popup
			{
				public static class NewMandat
				{
					//	designer:str/JUK0001
					public static global::Epsitec.Common.Types.FormattedText Title
					{
						get
						{
							return global::Epsitec.Cresus.Assets.App.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544320));
						}
					}
				}
			}
			
			public static global::Epsitec.Common.Types.FormattedText GetText(params string[] path)
			{
				string field = string.Join (".", path);
				return new global::Epsitec.Common.Types.FormattedText (_stringsBundle[field].AsString);
			}
			
			public static global::System.String GetString(params string[] path)
			{
				string field = string.Join (".", path);
				return _stringsBundle[field].AsString;
			}
			
			#region Internal Support Code
			
			private static global::Epsitec.Common.Types.FormattedText GetText(string bundle, params string[] path)
			{
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Assets.App.Res.Strings.GetString (bundle, path));
			}
			
			private static global::Epsitec.Common.Types.FormattedText GetText(global::Epsitec.Common.Support.Druid druid)
			{
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Assets.App.Res.Strings.GetString (druid));
			}
			
			private static global::System.String GetString(string bundle, params string[] path)
			{
				string field = string.Join (".", path);
				return _stringsBundle[field].AsString;
			}
			
			private static global::System.String GetString(global::Epsitec.Common.Support.Druid druid)
			{
				return _stringsBundle[druid].AsString;
			}
			
			private static readonly global::Epsitec.Common.Support.ResourceBundle _stringsBundle = Res._manager.GetBundle ("Strings");
			
			#endregion
		}
		
		public static class StringIds
		{
			public static class Popup
			{
				public static class NewMandat
				{
					//	designer:str/JUK0001
					public static global::Epsitec.Common.Support.Druid Title
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544320);
						}
					}
				}
			}
		}
		
		static Res()
		{
			Res._manager = new global::Epsitec.Common.Support.ResourceManager (typeof (Res));
			Res._manager.DefineDefaultModuleName ("Assets.App");
			Commands.Edit._Initialize ();
			Commands.View._Initialize ();
			Strings._Initialize ();
		}
		
		public static void Initialize()
		{
			global::System.Object.Equals (Res._manager, null);
		}
		
		public static global::Epsitec.Common.Support.ResourceManager Manager
		{
			get
			{
				return Res._manager;
			}
		}
		public static int ModuleId
		{
			get
			{
				return Res._moduleId;
			}
		}
		private static readonly global::Epsitec.Common.Support.ResourceManager _manager;
		private const int _moduleId = 2003;
	}
}
