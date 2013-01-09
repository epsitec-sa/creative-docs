//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

namespace Epsitec.Data.Platform
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Types
		{
			internal static void _Initialize()
			{
				System.Object.Equals (MatchStreet, null);
			}
			
			//	designer:cap/MVA
			public static readonly Epsitec.Common.Types.StructuredType MatchStreet = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 0));
		}
		
		public static class Fields
		{
			public static class MatchStreet
			{
				internal static void _Initialize()
				{
					System.Object.Equals (MatchStreet.AddressPostcode, null);
				}
				
				//	designer:cap/MVA2
				public static readonly global::Epsitec.Common.Support.Druid AddressPostcode = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 2);
				//	designer:cap/MVA3
				public static readonly global::Epsitec.Common.Support.Druid AddressPostcodeSuffix = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 3);
				//	designer:cap/MVA1
				public static readonly global::Epsitec.Common.Support.Druid BasicPostcode = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 1);
				//	designer:cap/MVA4
				public static readonly global::Epsitec.Common.Support.Druid HouseNumberFrom = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 4);
				//	designer:cap/MVAB
				public static readonly global::Epsitec.Common.Support.Druid HouseNumberFromSuffix = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 11);
				//	designer:cap/MVA5
				public static readonly global::Epsitec.Common.Support.Druid HouseNumberTo = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 5);
				//	designer:cap/MVAC
				public static readonly global::Epsitec.Common.Support.Druid HouseNumberToSuffix = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 12);
				//	designer:cap/MVAA
				public static readonly global::Epsitec.Common.Support.Druid LanguageCode = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 10);
				//	designer:cap/MVA9
				public static readonly global::Epsitec.Common.Support.Druid PrepositionCode = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 9);
				//	designer:cap/MVA6
				public static readonly global::Epsitec.Common.Support.Druid StreetName = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 6);
				//	designer:cap/MVA7
				public static readonly global::Epsitec.Common.Support.Druid StreetNameRoot = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 7);
				//	designer:cap/MVA8
				public static readonly global::Epsitec.Common.Support.Druid StreetTypeCode = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 8);
			}
		}
		
		//	Code mapping for 'String' resources
		
		public static class Strings
		{
			internal static void _Initialize()
			{
				System.Object.Equals (_stringsBundle, null);
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
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Data.Platform.Res.Strings.GetString (bundle, path));
			}
			
			private static global::Epsitec.Common.Types.FormattedText GetText(global::Epsitec.Common.Support.Druid druid)
			{
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Data.Platform.Res.Strings.GetString (druid));
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
		}
		
		static Res()
		{
			Res._manager = new global::Epsitec.Common.Support.ResourceManager (typeof (Res));
			Res._manager.DefineDefaultModuleName ("Data.Platform.SwissPostMatch");
			Types._Initialize ();
			Fields.MatchStreet._Initialize ();
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
		private const int _moduleId = 1014;
	}
}
