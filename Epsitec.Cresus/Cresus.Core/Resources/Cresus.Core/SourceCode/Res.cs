﻿//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

namespace Epsitec.Cresus.Core
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Commands
		{
			public static class Test
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/L0A
				public static readonly global::Epsitec.Common.Widgets.Command Crash = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 0));
			}
			
			internal static void _Initialize()
			{
				Test._Initialize ();
			}
		}
		
		public static class CommandIds
		{
			public static class Test
			{
				
				//	designer:cap/L0A
				public const long Crash = 0x150000A000000L;
			}
		}
		
		//	Code mapping for 'String' resources
		
		public static class Strings
		{
			public static class Error
			{
				//	designer:str/L0A
				public static global::Epsitec.Common.Types.FormattedText CannotConnectToLocalDatabase
				{
					get
					{
						return global::Epsitec.Cresus.Core.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (167772160));
					}
				}
			}
			
			public static class Hint
			{
				public static class Error
				{
					//	designer:str/L0A1
					public static global::Epsitec.Common.Types.FormattedText CannotConnectToLocalDatabase
					{
						get
						{
							return global::Epsitec.Cresus.Core.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (167772161));
						}
					}
				}
			}
			
			//	designer:str/L01
			public static global::Epsitec.Common.Types.FormattedText ProductName
			{
				get
				{
					return global::Epsitec.Cresus.Core.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (16777216));
				}
			}
			
			public static global::Epsitec.Common.Types.FormattedText GetText(params string[] path)
			{
				string field = string.Join (".", path);
				return new global::Epsitec.Common.Types.FormattedText (_stringsBundle[field].AsString);
			}
			
			#region Internal Support Code
			
			private static global::Epsitec.Common.Types.FormattedText GetText(string bundle, params string[] path)
			{
				string field = string.Join (".", path);
				return new global::Epsitec.Common.Types.FormattedText (_stringsBundle[field].AsString);
			}
			
			private static global::Epsitec.Common.Types.FormattedText GetText(global::Epsitec.Common.Support.Druid druid)
			{
				return new global::Epsitec.Common.Types.FormattedText (_stringsBundle[druid].AsString);
			}
			
			private static readonly global::Epsitec.Common.Support.ResourceBundle _stringsBundle = Res._manager.GetBundle ("Strings");
			
			#endregion
		}
		
		static Res()
		{
			Res._manager = new global::Epsitec.Common.Support.ResourceManager (typeof (Res));
			Res._manager.DefineDefaultModuleName ("Cresus.Core");
			Commands._Initialize ();
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
		private const int _moduleId = 21;
	}
}
