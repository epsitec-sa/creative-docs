//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

namespace Epsitec.Cresus.Core.Library.Images
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Types
		{
			internal static void _Initialize()
			{
			}
			
			//	designer:cap/9VA
			public static readonly Epsitec.Common.Types.StructuredType Image = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 0));
			//	designer:cap/9VA4
			public static readonly Epsitec.Common.Types.StructuredType ImageBlob = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 4));
			//	designer:cap/9VA1
			public static readonly Epsitec.Common.Types.StructuredType ImageCategory = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 1));
			//	designer:cap/9VA2
			public static readonly Epsitec.Common.Types.StructuredType ImageGroup = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 2));
		}
		
		public static class Fields
		{
			public static class Image
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/9VAB
				public static readonly global::Epsitec.Common.Support.Druid ImageBlob = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 11);
				//	designer:cap/9VAD
				public static readonly global::Epsitec.Common.Support.Druid ImageCategory = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 13);
				//	designer:cap/9VAC
				public static readonly global::Epsitec.Common.Support.Druid ImageGroups = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 12);
			}
			
			public static class ImageBlob
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/9VAA
				public static readonly global::Epsitec.Common.Support.Druid BitsPerPixel = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 10);
				//	designer:cap/9VA5
				public static readonly global::Epsitec.Common.Support.Druid Data = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 5);
				//	designer:cap/9VA9
				public static readonly global::Epsitec.Common.Support.Druid Dpi = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 9);
				//	designer:cap/9VA7
				public static readonly global::Epsitec.Common.Support.Druid PixelHeight = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 7);
				//	designer:cap/9VA6
				public static readonly global::Epsitec.Common.Support.Druid PixelWidth = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 6);
				//	designer:cap/9VA8
				public static readonly global::Epsitec.Common.Support.Druid ThumbnailSize = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 8);
			}
			
			public static class ImageCategory
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/9VA3
				public static readonly global::Epsitec.Common.Support.Druid CompatibleGroups = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 3);
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
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Core.Library.Images.Res.Strings.GetString (bundle, path));
			}
			
			private static global::Epsitec.Common.Types.FormattedText GetText(global::Epsitec.Common.Support.Druid druid)
			{
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Core.Library.Images.Res.Strings.GetString (druid));
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
			Res._manager.DefineDefaultModuleName ("Cresus.Core.Library.Images");
			Types._Initialize ();
			Fields.Image._Initialize ();
			Fields.ImageBlob._Initialize ();
			Fields.ImageCategory._Initialize ();
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
		private const int _moduleId = 1001;
	}
}
