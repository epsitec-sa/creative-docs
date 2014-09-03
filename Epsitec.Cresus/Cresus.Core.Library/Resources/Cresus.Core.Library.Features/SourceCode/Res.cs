﻿//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

namespace Epsitec.Cresus.Core.Library.Features
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Types
		{
			internal static void _Initialize()
			{
				global::System.Object.Equals (ProductCustomization, null);
			}
			
			//	designer:cap/JVA3
			public static readonly Epsitec.Common.Types.StructuredType ProductCustomization = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 3));
			//	designer:cap/JVA
			public static readonly Epsitec.Common.Types.StructuredType ProductFeature = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 0));
			//	designer:cap/JVA5
			public static readonly Epsitec.Common.Types.StructuredType ProductSettings = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 5));
		}
		
		public static class Fields
		{
			public static class ProductCustomization
			{
				internal static void _Initialize()
				{
					global::System.Object.Equals (ProductCustomization.Settings, null);
				}
				
				//	designer:cap/JVA4
				public static readonly global::Epsitec.Common.Support.Druid Settings = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 4);
			}
			
			public static class ProductFeature
			{
				internal static void _Initialize()
				{
					global::System.Object.Equals (ProductFeature.DisabledSettings, null);
				}
				
				//	designer:cap/JVA2
				public static readonly global::Epsitec.Common.Support.Druid DisabledSettings = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 2);
				//	designer:cap/JVA1
				public static readonly global::Epsitec.Common.Support.Druid EnabledSettings = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 1);
			}
			
			public static class ProductSettings
			{
				internal static void _Initialize()
				{
					global::System.Object.Equals (ProductSettings.LicensedFeatures, null);
				}
				
				//	designer:cap/JVA6
				public static readonly global::Epsitec.Common.Support.Druid LicensedFeatures = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 6);
			}
		}
		
		//	Code mapping for 'String' resources
		
		public static class Strings
		{
			internal static void _Initialize()
			{
				global::System.Object.Equals (_stringsBundle, null);
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
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Core.Library.Features.Res.Strings.GetString (bundle, path));
			}
			
			private static global::Epsitec.Common.Types.FormattedText GetText(global::Epsitec.Common.Support.Druid druid)
			{
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Core.Library.Features.Res.Strings.GetString (druid));
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
		}
		
		static Res()
		{
			Res._manager = new global::Epsitec.Common.Support.ResourceManager (typeof (Res));
			Res._manager.DefineDefaultModuleName ("Cresus.Core.Library.Features");
			Types._Initialize ();
			Fields.ProductCustomization._Initialize ();
			Fields.ProductFeature._Initialize ();
			Fields.ProductSettings._Initialize ();
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
		private const int _moduleId = 1011;
	}
}
