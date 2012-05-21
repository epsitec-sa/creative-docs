//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

namespace Epsitec.Common.Tests.Vs
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Types
		{
			internal static void _Initialize()
			{
			}
			
			//	designer:cap/I1A5
			public static readonly Epsitec.Common.Types.StructuredType CollectionData = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 5));
			//	designer:cap/I1A3
			public static readonly Epsitec.Common.Types.StructuredType ReferenceData = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 3));
			//	designer:cap/I1A
			public static readonly Epsitec.Common.Types.StructuredType ValueData = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 0));
		}
		
		public static class Fields
		{
			public static class CollectionData
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/I1A6
				public static readonly global::Epsitec.Common.Support.Druid Collection = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 6);
			}
			
			public static class ReferenceData
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/I1A4
				public static readonly global::Epsitec.Common.Support.Druid Reference = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 4);
			}
			
			public static class ValueData
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/I1A2
				public static readonly global::Epsitec.Common.Support.Druid NullableValue = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 2);
				//	designer:cap/I1A1
				public static readonly global::Epsitec.Common.Support.Druid Value = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 1);
			}
		}
		
		//	Code mapping for 'String' resources
		
		public static class Strings
		{
			
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
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Common.Tests.Vs.Res.Strings.GetString (bundle, path));
			}
			
			private static global::Epsitec.Common.Types.FormattedText GetText(global::Epsitec.Common.Support.Druid druid)
			{
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Common.Tests.Vs.Res.Strings.GetString (druid));
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
		
		static Res()
		{
			Res._manager = new global::Epsitec.Common.Support.ResourceManager (typeof (Res));
			Res._manager.DefineDefaultModuleName ("Common.Tests.Vs");
			Types._Initialize ();
			Fields.CollectionData._Initialize ();
			Fields.ReferenceData._Initialize ();
			Fields.ValueData._Initialize ();
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
		private const int _moduleId = 50;
	}
}
