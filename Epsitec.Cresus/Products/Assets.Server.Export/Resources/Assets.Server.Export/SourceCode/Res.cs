//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

namespace Epsitec.Cresus.Assets.Server.Export
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		//	Code mapping for 'String' resources
		
		public static class Strings
		{
			internal static void _Initialize()
			{
				global::System.Object.Equals (_stringsBundle, null);
			}
			
			//	designer:str/MUA0001
			public static global::Epsitec.Common.Types.FormattedText Copyright
			{
				get
				{
					return global::Epsitec.Cresus.Assets.Server.Export.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (167772160));
				}
			}
			public static class ExportEntries
			{
				public static class Description
				{
					//	designer:str/MUK0001
					public static global::Epsitec.Common.Types.FormattedText Many
					{
						get
						{
							return global::Epsitec.Cresus.Assets.Server.Export.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544320));
						}
					}
					//	designer:str/MUK1001
					public static global::Epsitec.Common.Types.FormattedText None
					{
						get
						{
							return global::Epsitec.Cresus.Assets.Server.Export.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544321));
						}
					}
				}
			}
			
			public static class ExportEntriesReport
			{
				public static class Description
				{
					//	designer:str/MUK5001
					public static global::Epsitec.Common.Types.FormattedText Many
					{
						get
						{
							return global::Epsitec.Cresus.Assets.Server.Export.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544325));
						}
					}
					//	designer:str/MUK2001
					public static global::Epsitec.Common.Types.FormattedText None
					{
						get
						{
							return global::Epsitec.Cresus.Assets.Server.Export.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544322));
						}
					}
					//	designer:str/MUK3001
					public static global::Epsitec.Common.Types.FormattedText One
					{
						get
						{
							return global::Epsitec.Cresus.Assets.Server.Export.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544323));
						}
					}
					//	designer:str/MUK6001
					public static global::Epsitec.Common.Types.FormattedText Same
					{
						get
						{
							return global::Epsitec.Cresus.Assets.Server.Export.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544326));
						}
					}
					//	designer:str/MUK4001
					public static global::Epsitec.Common.Types.FormattedText Summary
					{
						get
						{
							return global::Epsitec.Cresus.Assets.Server.Export.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (335544324));
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
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Assets.Server.Export.Res.Strings.GetString (bundle, path));
			}
			
			private static global::Epsitec.Common.Types.FormattedText GetText(global::Epsitec.Common.Support.Druid druid)
			{
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Assets.Server.Export.Res.Strings.GetString (druid));
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
			//	designer:str/MUA0001
			public static global::Epsitec.Common.Support.Druid Copyright
			{
				get
				{
					return global::Epsitec.Common.Support.Druid.FromFieldId (167772160);
				}
			}
			public static class ExportEntries
			{
				public static class Description
				{
					//	designer:str/MUK0001
					public static global::Epsitec.Common.Support.Druid Many
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544320);
						}
					}
					//	designer:str/MUK1001
					public static global::Epsitec.Common.Support.Druid None
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544321);
						}
					}
				}
			}
			
			public static class ExportEntriesReport
			{
				public static class Description
				{
					//	designer:str/MUK5001
					public static global::Epsitec.Common.Support.Druid Many
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544325);
						}
					}
					//	designer:str/MUK2001
					public static global::Epsitec.Common.Support.Druid None
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544322);
						}
					}
					//	designer:str/MUK3001
					public static global::Epsitec.Common.Support.Druid One
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544323);
						}
					}
					//	designer:str/MUK6001
					public static global::Epsitec.Common.Support.Druid Same
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544326);
						}
					}
					//	designer:str/MUK4001
					public static global::Epsitec.Common.Support.Druid Summary
					{
						get
						{
							return global::Epsitec.Common.Support.Druid.FromFieldId (335544324);
						}
					}
				}
			}
		}
		
		static Res()
		{
			Res._manager = new global::Epsitec.Common.Support.ResourceManager (typeof (Res));
			Res._manager.DefineDefaultModuleName ("Assets.Server.Export");
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
		private const int _moduleId = 2006;
	}
}
