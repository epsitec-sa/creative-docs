//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

namespace Epsitec.Cresus.Compta
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Types
		{
			internal static void _Initialize()
			{
			}
			
			public static class Enum
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/OVKP
				public static readonly Epsitec.Common.Types.EnumType CatégorieDeCompte = (global::Epsitec.Common.Types.EnumType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 25));
				//	designer:cap/OVK01
				public static readonly Epsitec.Common.Types.EnumType TypeDeCompte = (global::Epsitec.Common.Types.EnumType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 32));
			}
			
			//	designer:cap/OVK
			public static readonly Epsitec.Common.Types.StructuredType Comptabilité = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 0));
			//	designer:cap/OVK3
			public static readonly Epsitec.Common.Types.StructuredType ComptabilitéCompte = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 3));
			//	designer:cap/OVKB
			public static readonly Epsitec.Common.Types.StructuredType ComptabilitéEcriture = (global::Epsitec.Common.Types.StructuredType) global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 11));
		}
		
		public static class Values
		{
			internal static void _Initialize()
			{
			}
			
			public static class Enum
			{
				internal static void _Initialize()
				{
				}
				
				public static class CatégorieDeCompte
				{
					internal static void _Initialize()
					{
					}
					
					//	designer:cap/OVKR
					public static global::Epsitec.Common.Types.Caption Actif
					{
						get
						{
							return global::Epsitec.Cresus.Compta.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 27));
						}
					}
					//	designer:cap/OVKT
					public static global::Epsitec.Common.Types.Caption Charge
					{
						get
						{
							return global::Epsitec.Cresus.Compta.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 29));
						}
					}
					//	designer:cap/OVKV
					public static global::Epsitec.Common.Types.Caption Exploitation
					{
						get
						{
							return global::Epsitec.Cresus.Compta.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 31));
						}
					}
					//	designer:cap/OVKQ
					public static global::Epsitec.Common.Types.Caption Inconnu
					{
						get
						{
							return global::Epsitec.Cresus.Compta.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 26));
						}
					}
					//	designer:cap/OVKS
					public static global::Epsitec.Common.Types.Caption Passif
					{
						get
						{
							return global::Epsitec.Cresus.Compta.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 28));
						}
					}
					//	designer:cap/OVKU
					public static global::Epsitec.Common.Types.Caption Produit
					{
						get
						{
							return global::Epsitec.Cresus.Compta.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 30));
						}
					}
				}
				
				public static class TypeDeCompte
				{
					internal static void _Initialize()
					{
					}
					
					//	designer:cap/OVK41
					public static global::Epsitec.Common.Types.Caption Bloqué
					{
						get
						{
							return global::Epsitec.Cresus.Compta.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 36));
						}
					}
					//	designer:cap/OVK31
					public static global::Epsitec.Common.Types.Caption Groupe
					{
						get
						{
							return global::Epsitec.Cresus.Compta.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 35));
						}
					}
					//	designer:cap/OVK11
					public static global::Epsitec.Common.Types.Caption Normal
					{
						get
						{
							return global::Epsitec.Cresus.Compta.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 33));
						}
					}
					//	designer:cap/OVK21
					public static global::Epsitec.Common.Types.Caption Titre
					{
						get
						{
							return global::Epsitec.Cresus.Compta.Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, 20, 34));
						}
					}
				}
			}
			
		}
		
		public static class Fields
		{
			public static class Comptabilité
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/OVK1
				public static readonly global::Epsitec.Common.Support.Druid DernièreDate = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 1);
				//	designer:cap/OVK2
				public static readonly global::Epsitec.Common.Support.Druid DernièrePièce = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 2);
				//	designer:cap/OVKO
				public static readonly global::Epsitec.Common.Support.Druid Journal = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 24);
				//	designer:cap/OVKN
				public static readonly global::Epsitec.Common.Support.Druid PlanComptable = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 23);
			}
			
			public static class ComptabilitéCompte
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/OVK81
				public static readonly global::Epsitec.Common.Support.Druid Budget = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 40);
				//	designer:cap/OVK91
				public static readonly global::Epsitec.Common.Support.Druid BudgetFutur = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 41);
				//	designer:cap/OVK71
				public static readonly global::Epsitec.Common.Support.Druid BudgetPrécédent = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 39);
				//	designer:cap/OVK51
				public static readonly global::Epsitec.Common.Support.Druid Catégorie = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 37);
				//	designer:cap/OVK7
				public static readonly global::Epsitec.Common.Support.Druid CompteOuvBoucl = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 7);
				//	designer:cap/OVK6
				public static readonly global::Epsitec.Common.Support.Druid Groupe = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 6);
				//	designer:cap/OVK8
				public static readonly global::Epsitec.Common.Support.Druid IndexOuvBoucl = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 8);
				//	designer:cap/OVK9
				public static readonly global::Epsitec.Common.Support.Druid Monnaie = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 9);
				//	designer:cap/OVKA
				public static readonly global::Epsitec.Common.Support.Druid Niveau = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 10);
				//	designer:cap/OVK4
				public static readonly global::Epsitec.Common.Support.Druid Numéro = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 4);
				//	designer:cap/OVK5
				public static readonly global::Epsitec.Common.Support.Druid Titre = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 5);
				//	designer:cap/OVK61
				public static readonly global::Epsitec.Common.Support.Druid Type = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 38);
			}
			
			public static class ComptabilitéEcriture
			{
				internal static void _Initialize()
				{
				}
				
				//	designer:cap/OVKM
				public static readonly global::Epsitec.Common.Support.Druid CodeAnalytique = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 22);
				//	designer:cap/OVKL
				public static readonly global::Epsitec.Common.Support.Druid CodeTVA = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 21);
				//	designer:cap/OVKF
				public static readonly global::Epsitec.Common.Support.Druid Crédit = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 15);
				//	designer:cap/OVKC
				public static readonly global::Epsitec.Common.Support.Druid Date = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 12);
				//	designer:cap/OVKE
				public static readonly global::Epsitec.Common.Support.Druid Débit = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 14);
				//	designer:cap/OVKH
				public static readonly global::Epsitec.Common.Support.Druid Libellé = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 17);
				//	designer:cap/OVKI
				public static readonly global::Epsitec.Common.Support.Druid Montant = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 18);
				//	designer:cap/OVKD
				public static readonly global::Epsitec.Common.Support.Druid MultiId = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 13);
				//	designer:cap/OVKK
				public static readonly global::Epsitec.Common.Support.Druid NuméroTVA = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 20);
				//	designer:cap/OVKG
				public static readonly global::Epsitec.Common.Support.Druid Pièce = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 16);
				//	designer:cap/OVKJ
				public static readonly global::Epsitec.Common.Support.Druid TypeTVA = new global::Epsitec.Common.Support.Druid (_moduleId, 20, 19);
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
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Compta.Res.Strings.GetString (bundle, path));
			}
			
			private static global::Epsitec.Common.Types.FormattedText GetText(global::Epsitec.Common.Support.Druid druid)
			{
				return new global::Epsitec.Common.Types.FormattedText (global::Epsitec.Cresus.Compta.Res.Strings.GetString (druid));
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
			Res._manager.DefineDefaultModuleName ("Cresus.Compta");
			Types._Initialize ();
			Types.Enum._Initialize ();
			Values._Initialize ();
			Values.Enum.CatégorieDeCompte._Initialize ();
			Values.Enum.TypeDeCompte._Initialize ();
			Fields.Comptabilité._Initialize ();
			Fields.ComptabilitéCompte._Initialize ();
			Fields.ComptabilitéEcriture._Initialize ();
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
		private const int _moduleId = 1016;
	}
}
