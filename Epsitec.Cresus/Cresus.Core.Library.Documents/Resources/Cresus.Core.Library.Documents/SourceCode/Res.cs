//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

namespace Epsitec.Cresus.Core
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Types
		{
			internal static void _Initialize()
			{
			}
			
			//	designer:cap/HVA4
			public static readonly Epsitec.Common.Types.StructuredType AbstractDocument = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 4));
			//	designer:cap/HVA1
			public static readonly Epsitec.Common.Types.StructuredType DocumentCategory = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 1));
			//	designer:cap/HVA
			public static readonly Epsitec.Common.Types.StructuredType DocumentMetadata = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 0));
			//	designer:cap/HVA2
			public static readonly Epsitec.Common.Types.StructuredType DocumentOptions = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 2));
			//	designer:cap/HVA3
			public static readonly Epsitec.Common.Types.StructuredType DocumentPrintingUnits = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 3));
			//	designer:cap/HVAC
			public static readonly Epsitec.Common.Types.StructuredType SerializedDocumentBlob = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 12));
		}
		
		public static class Fields
		{
			public static class DocumentCategory
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/HVA8
				public static readonly global::Epsitec.Common.Support.Druid DocumentOptions = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 8);
				//	designer:cap/HVA9
				public static readonly global::Epsitec.Common.Support.Druid DocumentPrintingUnits = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 9);
				//	designer:cap/HVA7
				public static readonly global::Epsitec.Common.Support.Druid DocumentType = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 7);
			}
			
			public static class DocumentMetadata
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/HVA6
				public static readonly global::Epsitec.Common.Support.Druid DocumentCategory = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 6);
				//	designer:cap/HVA5
				public static readonly global::Epsitec.Common.Support.Druid DocumentTitle = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 5);
				//	designer:cap/HVAE
				public static readonly global::Epsitec.Common.Support.Druid SerializedDocumentVersions = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 14);
			}
			
			public static class DocumentOptions
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/HVAA
				public static readonly global::Epsitec.Common.Support.Druid SerializedData = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 10);
			}
			
			public static class DocumentPrintingUnits
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/HVAB
				public static readonly global::Epsitec.Common.Support.Druid SerializedData = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 11);
			}
			
			public static class SerializedDocumentBlob
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/HVAD
				public static readonly global::Epsitec.Common.Support.Druid Data = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 13);
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
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Core.Res.Strings.GetString (bundle, path));
			}
			
			private static global::Epsitec.Common.Types.FormattedText GetText(global::Epsitec.Common.Support.Druid druid)
			{
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Core.Res.Strings.GetString (druid));
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
			Res._manager.DefineDefaultModuleName ("Cresus.Core.Library.Documents");
			Types._Initialize ();
			Fields.DocumentCategory._Initialize ();
			Fields.DocumentMetadata._Initialize ();
			Fields.DocumentOptions._Initialize ();
			Fields.DocumentPrintingUnits._Initialize ();
			Fields.SerializedDocumentBlob._Initialize ();
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
		private const int _moduleId = 1009;
	}
}
