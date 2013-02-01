//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

namespace Epsitec.Cresus.Database
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Types
		{
			public static class Num
			{
				internal static void _Initialize()
				{
					System.Object.Equals (Num.CollectionRank, null);
				}
				
				//	designer:cap/K01
				public static readonly Epsitec.Common.Types.IntegerType CollectionRank = (global::Epsitec.Common.Types.IntegerType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 1, 0));
				//	designer:cap/K004
				public static readonly Epsitec.Common.Types.LongIntegerType KeyId = (global::Epsitec.Common.Types.LongIntegerType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 4));
				//	designer:cap/K005
				public static readonly Epsitec.Common.Types.IntegerType KeyStatus = (global::Epsitec.Common.Types.IntegerType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 5));
				//	designer:cap/K007
				public static readonly Epsitec.Common.Types.LongIntegerType NullableKeyId = (global::Epsitec.Common.Types.LongIntegerType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 7));
				//	designer:cap/K006
				public static readonly Epsitec.Common.Types.IntegerType ReqExecState = (global::Epsitec.Common.Types.IntegerType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 6));
			}
			
			public static class Other
			{
				internal static void _Initialize()
				{
					System.Object.Equals (Other.DateTime, null);
				}
				
				//	designer:cap/K00D
				public static readonly Epsitec.Common.Types.DateTimeType DateTime = (global::Epsitec.Common.Types.DateTimeType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 13));
				//	designer:cap/K00C
				public static readonly Epsitec.Common.Types.BinaryType ReqData = (global::Epsitec.Common.Types.BinaryType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 12));
			}
			
			public static class Str
			{
				internal static void _Initialize()
				{
					System.Object.Equals (Str.InfoXml, null);
				}
				
				//	designer:cap/K009
				public static readonly Epsitec.Common.Types.StringType InfoXml = (global::Epsitec.Common.Types.StringType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 9));
				//	designer:cap/K008
				public static readonly Epsitec.Common.Types.StringType Name = (global::Epsitec.Common.Types.StringType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 8));
				public static class Dict
				{
					internal static void _Initialize()
					{
						System.Object.Equals (Str.Dict.Key, null);
					}
					
					//	designer:cap/K00A
					public static readonly Epsitec.Common.Types.StringType Key = (global::Epsitec.Common.Types.StringType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 10));
					//	designer:cap/K00B
					public static readonly Epsitec.Common.Types.StringType Value = (global::Epsitec.Common.Types.StringType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 0, 11));
				}
			}
		}
		
		//	Code mapping for 'String' resources
		
		public static class Strings
		{
			internal static void _Initialize()
			{
				System.Object.Equals (_stringsBundle, null);
			}
			
			//	designer:str/K
			public static global::Epsitec.Common.Types.FormattedText New
			{
				get
				{
					return global::Epsitec.Cresus.Database.Res.Strings.GetText (global::Epsitec.Common.Support.Druid.FromFieldId (0));
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
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Database.Res.Strings.GetString (bundle, path));
			}
			
			private static global::Epsitec.Common.Types.FormattedText GetText(global::Epsitec.Common.Support.Druid druid)
			{
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Database.Res.Strings.GetString (druid));
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
			//	designer:str/K
			public static global::Epsitec.Common.Support.Druid New
			{
				get
				{
					return global::Epsitec.Common.Support.Druid.FromFieldId (0);
				}
			}
		}
		
		static Res()
		{
			Res._manager = new global::Epsitec.Common.Support.ResourceManager (typeof (Res));
			Res._manager.DefineDefaultModuleName ("Cresus.Database");
			Types.Num._Initialize ();
			Types.Other._Initialize ();
			Types.Str._Initialize ();
			Types.Str.Dict._Initialize ();
			Strings._Initialize ();
		}
		
		public static void Initialize()
		{
			System.Object.Equals (Res._manager, null);
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
		private const int _moduleId = 20;
	}
}
