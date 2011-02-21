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
			//	designer:cap/CVAK
			public static readonly Epsitec.Common.Types.StringType BookAccount = (global::Epsitec.Common.Types.StringType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 20));
			//	designer:cap/CVA
			public static readonly Epsitec.Common.Types.EnumType CurrencyCode = (global::Epsitec.Common.Types.EnumType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 0));
			//	designer:cap/CVAF
			public static readonly Epsitec.Common.Types.StringType IsrBankReferenceNumber = (global::Epsitec.Common.Types.StringType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 15));
			//	designer:cap/CVAA
			public static readonly Epsitec.Common.Types.StringType PostFinanceAccount = (global::Epsitec.Common.Types.StringType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 10));
			//	designer:cap/CVA9
			public static readonly Epsitec.Common.Types.StructuredType IsrDefinition = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 9));
		}
		
		public static class Values
		{
			public static class CurrencyCode
			{
				//	designer:cap/CVA2
				public static global::Epsitec.Common.Types.Caption Aud
				{
					get
					{
						return global::Epsitec.Cresus.Core.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 2));
					}
				}
				//	designer:cap/CVA5
				public static global::Epsitec.Common.Types.Caption Chf
				{
					get
					{
						return global::Epsitec.Cresus.Core.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 5));
					}
				}
				//	designer:cap/CVA3
				public static global::Epsitec.Common.Types.Caption Cny
				{
					get
					{
						return global::Epsitec.Cresus.Core.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 3));
					}
				}
				//	designer:cap/CVA8
				public static global::Epsitec.Common.Types.Caption Eur
				{
					get
					{
						return global::Epsitec.Cresus.Core.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 8));
					}
				}
				//	designer:cap/CVA6
				public static global::Epsitec.Common.Types.Caption Gbp
				{
					get
					{
						return global::Epsitec.Cresus.Core.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 6));
					}
				}
				//	designer:cap/CVA4
				public static global::Epsitec.Common.Types.Caption Jpy
				{
					get
					{
						return global::Epsitec.Cresus.Core.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 4));
					}
				}
				//	designer:cap/CVA1
				public static global::Epsitec.Common.Types.Caption None
				{
					get
					{
						return global::Epsitec.Cresus.Core.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 1));
					}
				}
				//	designer:cap/CVA7
				public static global::Epsitec.Common.Types.Caption Usd
				{
					get
					{
						return global::Epsitec.Cresus.Core.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 10, 7));
					}
				}
			}
			
		}
		
		public static class Fields
		{
			public static class IsrDefinition
			{
				//	designer:cap/CVAI
				public static readonly global::Epsitec.Common.Support.Druid BankAccount = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 18);
				//	designer:cap/CVAG
				public static readonly global::Epsitec.Common.Support.Druid BankAddressLine1 = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 16);
				//	designer:cap/CVAH
				public static readonly global::Epsitec.Common.Support.Druid BankAddressLine2 = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 17);
				//	designer:cap/CVAE
				public static readonly global::Epsitec.Common.Support.Druid BankReferenceNumberPrefix = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 14);
				//	designer:cap/CVAB
				public static readonly global::Epsitec.Common.Support.Druid Currency = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 11);
				//	designer:cap/CVAJ
				public static readonly global::Epsitec.Common.Support.Druid IncomingBookAccount = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 19);
				//	designer:cap/CVAD
				public static readonly global::Epsitec.Common.Support.Druid SubscriberAddress = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 13);
				//	designer:cap/CVAC
				public static readonly global::Epsitec.Common.Support.Druid SubscriberNumber = new global::Epsitec.Common.Support.Druid (_moduleId, 10, 12);
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
			Res._manager.DefineDefaultModuleName ("Cresus.Core.Library.Finance");
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
		private const int _moduleId = 1004;
	}
}
