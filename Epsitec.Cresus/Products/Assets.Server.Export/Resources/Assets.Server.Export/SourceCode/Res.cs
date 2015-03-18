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
