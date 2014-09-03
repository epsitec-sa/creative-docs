//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

namespace Epsitec.Cresus.WebCore.Server
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Types
		{
			internal static void _Initialize()
			{
				global::System.Object.Equals (LabelLayout, null);
			}
			
			//	designer:cap/A1G002
			public static readonly Epsitec.Common.Types.EnumType LabelLayout = (global::Epsitec.Common.Types.EnumType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 80, 0));
		}
		
		public static class Values
		{
			public static class LabelLayout
			{
				//	designer:cap/A1G102
				public static global::Epsitec.Common.Types.Caption Avery_3475
				{
					get
					{
						return global::Epsitec.Cresus.WebCore.Server.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 80, 1));
					}
				}
				//	designer:cap/A1A3
				public static global::Epsitec.Common.Types.Caption Sheet_A4_Simple
				{
					get
					{
						return global::Epsitec.Cresus.WebCore.Server.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 3));
					}
				}
				//	designer:cap/A1A4
				public static global::Epsitec.Common.Types.Caption Sheet_A4_SimplePP
				{
					get
					{
						return global::Epsitec.Cresus.WebCore.Server.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 4));
					}
				}
				//	designer:cap/A1A5
				public static global::Epsitec.Common.Types.Caption Sheet_A4_SimplePPPriority
				{
					get
					{
						return global::Epsitec.Cresus.WebCore.Server.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 5));
					}
				}
				//	designer:cap/A1A
				public static global::Epsitec.Common.Types.Caption Sheet_A5_Simple
				{
					get
					{
						return global::Epsitec.Cresus.WebCore.Server.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 0));
					}
				}
				//	designer:cap/A1A1
				public static global::Epsitec.Common.Types.Caption Sheet_A5_SimplePP
				{
					get
					{
						return global::Epsitec.Cresus.WebCore.Server.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 1));
					}
				}
				//	designer:cap/A1A2
				public static global::Epsitec.Common.Types.Caption Sheet_A5_SimplePPPriority
				{
					get
					{
						return global::Epsitec.Cresus.WebCore.Server.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 2));
					}
				}
			}
			
		}
		
		//	Code mapping for 'String' resources
		
		public static class Strings
		{
			internal static void _Initialize()
			{
				global::System.Object.Equals (_stringsBundle, null);
			}
			
			//	designer:str/A1G302
			public static global::Epsitec.Common.Types.FormattedText EmptyValue
			{
				get
				{
					return global::Epsitec.Cresus.WebCore.Server.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (1342177283));
				}
			}
			//	designer:str/A1G102
			public static global::Epsitec.Common.Types.FormattedText IncorrectPassword
			{
				get
				{
					return global::Epsitec.Cresus.WebCore.Server.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (1342177281));
				}
			}
			//	designer:str/A1G002
			public static global::Epsitec.Common.Types.FormattedText IncorrectUsername
			{
				get
				{
					return global::Epsitec.Cresus.WebCore.Server.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (1342177280));
				}
			}
			//	designer:str/A1G202
			public static global::Epsitec.Common.Types.FormattedText IncorrectValue
			{
				get
				{
					return global::Epsitec.Cresus.WebCore.Server.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (1342177282));
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
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.WebCore.Server.Res.Strings.GetString (bundle, path));
			}
			
			private static global::Epsitec.Common.Types.FormattedText GetText(global::Epsitec.Common.Support.Druid druid)
			{
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.WebCore.Server.Res.Strings.GetString (druid));
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
			
			private static readonly global::Epsitec.Common.Support.ResourceBundle _stringsBundle = Res._manager.GetBundleOrThrow ("Strings");
			
			#endregion
		}
		
		public static class StringIds
		{
			//	designer:str/A1G302
			public static global::Epsitec.Common.Support.Druid EmptyValue
			{
				get
				{
					return global::Epsitec.Common.Support.Druid.FromFieldId (1342177283);
				}
			}
			//	designer:str/A1G102
			public static global::Epsitec.Common.Support.Druid IncorrectPassword
			{
				get
				{
					return global::Epsitec.Common.Support.Druid.FromFieldId (1342177281);
				}
			}
			//	designer:str/A1G002
			public static global::Epsitec.Common.Support.Druid IncorrectUsername
			{
				get
				{
					return global::Epsitec.Common.Support.Druid.FromFieldId (1342177280);
				}
			}
			//	designer:str/A1G202
			public static global::Epsitec.Common.Support.Druid IncorrectValue
			{
				get
				{
					return global::Epsitec.Common.Support.Druid.FromFieldId (1342177282);
				}
			}
		}
		
		static Res()
		{
			Res._manager = new global::Epsitec.Common.Support.ResourceManager (typeof (Res));
			Res._manager.DefineDefaultModuleName ("Cresus.WebCore.Server");
			Types._Initialize ();
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
		private const int _moduleId = 42;
	}
}
