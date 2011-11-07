//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

namespace Epsitec.Cresus.Core.Library
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Commands
		{
			internal static void _Initialize()
			{
			}
			
			public static class Edition
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/EVA8
				public static readonly global::Epsitec.Common.Widgets.Command DiscardRecord = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 8));
				//	designer:cap/EVA7
				public static readonly global::Epsitec.Common.Widgets.Command SaveRecord = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 7));
			}
			
			public static class History
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/EVA5
				public static readonly global::Epsitec.Common.Widgets.Command NavigateBackward = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 5));
				//	designer:cap/EVA6
				public static readonly global::Epsitec.Common.Widgets.Command NavigateForward = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 6));
			}
		}
		
		public static class CommandIds
		{
			internal static void _Initialize()
			{
			}
			
			public static class Edition
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/EVA8
				public const long DiscardRecord = 0x3EE0000A000008L;
				//	designer:cap/EVA7
				public const long SaveRecord = 0x3EE0000A000007L;
			}
			
			public static class History
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/EVA5
				public const long NavigateBackward = 0x3EE0000A000005L;
				//	designer:cap/EVA6
				public const long NavigateForward = 0x3EE0000A000006L;
			}
			
		}
		
		public static class Types
		{
			internal static void _Initialize()
			{
			}
			
			//	designer:cap/EVA
			public static readonly Epsitec.Common.Types.EnumType EnumValueCardinality = (global::Epsitec.Common.Types.EnumType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 0));
			//	designer:cap/EVAN
			public static readonly Epsitec.Common.Types.EnumType MergeSettingsMode = (global::Epsitec.Common.Types.EnumType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 23));
			//	designer:cap/EVA9
			public static readonly Epsitec.Common.Types.EnumType TileEditionMode = (global::Epsitec.Common.Types.EnumType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 9));
			//	designer:cap/EVAD
			public static readonly Epsitec.Common.Types.EnumType TileVisibilityMode = (global::Epsitec.Common.Types.EnumType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 13));
			//	designer:cap/EVAI
			public static readonly Epsitec.Common.Types.EnumType UserCategory = (global::Epsitec.Common.Types.EnumType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 18));
		}
		
		public static class Values
		{
			internal static void _Initialize()
			{
			}
			
			public static class EnumValueCardinality
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/EVA4
				public static global::Epsitec.Common.Types.Caption Any
				{
					get
					{
						return global::Epsitec.Cresus.Core.Library.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 4));
					}
				}
				//	designer:cap/EVA3
				public static global::Epsitec.Common.Types.Caption AtLeastOne
				{
					get
					{
						return global::Epsitec.Cresus.Core.Library.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 3));
					}
				}
				//	designer:cap/EVA2
				public static global::Epsitec.Common.Types.Caption ExactlyOne
				{
					get
					{
						return global::Epsitec.Cresus.Core.Library.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 2));
					}
				}
				//	designer:cap/EVA1
				public static global::Epsitec.Common.Types.Caption ZeroOrOne
				{
					get
					{
						return global::Epsitec.Cresus.Core.Library.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 1));
					}
				}
			}
			
			public static class MergeSettingsMode
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/EVAP
				public static global::Epsitec.Common.Types.Caption Exclusive
				{
					get
					{
						return global::Epsitec.Cresus.Core.Library.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 25));
					}
				}
				//	designer:cap/EVAO
				public static global::Epsitec.Common.Types.Caption Inclusive
				{
					get
					{
						return global::Epsitec.Cresus.Core.Library.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 24));
					}
				}
				//	designer:cap/EVAR
				public static global::Epsitec.Common.Types.Caption Override
				{
					get
					{
						return global::Epsitec.Cresus.Core.Library.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 27));
					}
				}
			}
			
			public static class TileEditionMode
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/EVAC
				public static global::Epsitec.Common.Types.Caption ReadOnly
				{
					get
					{
						return global::Epsitec.Cresus.Core.Library.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 12));
					}
				}
				//	designer:cap/EVAB
				public static global::Epsitec.Common.Types.Caption ReadWrite
				{
					get
					{
						return global::Epsitec.Cresus.Core.Library.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 11));
					}
				}
				//	designer:cap/EVAA
				public static global::Epsitec.Common.Types.Caption Undefined
				{
					get
					{
						return global::Epsitec.Cresus.Core.Library.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 10));
					}
				}
			}
			
			public static class TileVisibilityMode
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/EVAG
				public static global::Epsitec.Common.Types.Caption Hidden
				{
					get
					{
						return global::Epsitec.Cresus.Core.Library.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 16));
					}
				}
				//	designer:cap/EVAH
				public static global::Epsitec.Common.Types.Caption NeverVisible
				{
					get
					{
						return global::Epsitec.Cresus.Core.Library.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 17));
					}
				}
				//	designer:cap/EVAE
				public static global::Epsitec.Common.Types.Caption Undefined
				{
					get
					{
						return global::Epsitec.Cresus.Core.Library.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 14));
					}
				}
				//	designer:cap/EVAF
				public static global::Epsitec.Common.Types.Caption Visible
				{
					get
					{
						return global::Epsitec.Cresus.Core.Library.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 15));
					}
				}
			}
			
			public static class UserCategory
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/EVAJ
				public static global::Epsitec.Common.Types.Caption Any
				{
					get
					{
						return global::Epsitec.Cresus.Core.Library.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 19));
					}
				}
				//	designer:cap/EVAL
				public static global::Epsitec.Common.Types.Caption Group
				{
					get
					{
						return global::Epsitec.Cresus.Core.Library.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 21));
					}
				}
				//	designer:cap/EVAM
				public static global::Epsitec.Common.Types.Caption Role
				{
					get
					{
						return global::Epsitec.Cresus.Core.Library.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 22));
					}
				}
				//	designer:cap/EVAK
				public static global::Epsitec.Common.Types.Caption User
				{
					get
					{
						return global::Epsitec.Cresus.Core.Library.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 20));
					}
				}
			}
			
		}
		
		//	Code mapping for 'String' resources
		
		public static class Strings
		{
			public static class Text
			{
				//	designer:str/EVA
				public static global::System.String Unknown
				{
					get
					{
						return global::Epsitec.Cresus.Core.Library.Res.Strings.GetString (global::Epsitec.Common.Support.Druid.FromFieldId (167772160));
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
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Core.Library.Res.Strings.GetString (bundle, path));
			}
			
			private static global::Epsitec.Common.Types.FormattedText GetText(global::Epsitec.Common.Support.Druid druid)
			{
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Core.Library.Res.Strings.GetString (druid));
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
			public static class Text
			{
				//	designer:str/EVA
				public static global::Epsitec.Common.Support.Druid Unknown
				{
					get
					{
						return global::Epsitec.Common.Support.Druid.FromFieldId (167772160);
					}
				}
			}
		}
		
		static Res()
		{
			Res._manager = new global::Epsitec.Common.Support.ResourceManager (typeof (Res));
			Res._manager.DefineDefaultModuleName ("Cresus.Core.Library");
			Commands._Initialize ();
			Commands.Edition._Initialize ();
			Commands.History._Initialize ();
			CommandIds._Initialize ();
			CommandIds.Edition._Initialize ();
			CommandIds.History._Initialize ();
			Types._Initialize ();
			Values._Initialize ();
			Values.EnumValueCardinality._Initialize ();
			Values.MergeSettingsMode._Initialize ();
			Values.TileEditionMode._Initialize ();
			Values.TileVisibilityMode._Initialize ();
			Values.UserCategory._Initialize ();
		}
		
		public static void Initialize()
		{
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
		private const int _moduleId = 1006;
	}
}
