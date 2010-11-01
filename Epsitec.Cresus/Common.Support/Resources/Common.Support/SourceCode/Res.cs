//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

namespace Epsitec.Common.Support
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Commands
		{
			//	designer:cap/7017
			public static readonly global::Epsitec.Common.Widgets.Command TestCommand = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, 1, 7));
			
			internal static void _Initialize()
			{
			}
		}
		
		public static class CommandIds
		{
			//	designer:cap/7017
			public const long TestCommand = 0x700001000007L;
			
		}
		
		public static class Captions
		{
			//	designer:cap/7015
			public static global::Epsitec.Common.Types.Caption TestCaption1
			{
				get
				{
					return global::Epsitec.Common.Support.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 1, 5));
				}
			}
			//	designer:cap/7016
			public static global::Epsitec.Common.Types.Caption TestCaption2
			{
				get
				{
					return global::Epsitec.Common.Support.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 1, 6));
				}
			}
			//	designer:cap/7018
			public static global::Epsitec.Common.Types.Caption TestCaption3
			{
				get
				{
					return global::Epsitec.Common.Support.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 1, 8));
				}
			}
			
		}
		
		public static class CaptionIds
		{
			//	designer:cap/7015
			public static readonly global::Epsitec.Common.Support.Druid TestCaption1 = new global::Epsitec.Common.Support.Druid (_moduleId, 1, 5);
			//	designer:cap/7016
			public static readonly global::Epsitec.Common.Support.Druid TestCaption2 = new global::Epsitec.Common.Support.Druid (_moduleId, 1, 6);
			//	designer:cap/7018
			public static readonly global::Epsitec.Common.Support.Druid TestCaption3 = new global::Epsitec.Common.Support.Druid (_moduleId, 1, 8);
			
		}
		
		
		
		
		//	Code mapping for 'String' resources
		
		public static class Strings
		{
			//	designer:str/7001
			public static global::Epsitec.Common.Types.FormattedText Author
			{
				get
				{
					return global::Epsitec.Common.Support.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (1));
				}
			}
			public static class CodeGenerator
			{
				//	designer:str/701
				public static global::Epsitec.Common.Types.FormattedText SourceFileHeader
				{
					get
					{
						return global::Epsitec.Common.Support.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (16777216));
					}
				}
			}
			
			//	designer:str/7003
			public static global::Epsitec.Common.Types.FormattedText CopyrightHolder
			{
				get
				{
					return global::Epsitec.Common.Support.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (3));
				}
			}
			public static class Image
			{
				//	designer:str/7
				public static global::Epsitec.Common.Types.FormattedText Description
				{
					get
					{
						return global::Epsitec.Common.Support.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (0));
					}
				}
			}
			
			//	designer:str/7002
			public static global::Epsitec.Common.Types.FormattedText Null
			{
				get
				{
					return global::Epsitec.Common.Support.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (2));
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
			Res._manager.DefineDefaultModuleName ("Common.Support");
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
		private const int _moduleId = 7;
	}
}
