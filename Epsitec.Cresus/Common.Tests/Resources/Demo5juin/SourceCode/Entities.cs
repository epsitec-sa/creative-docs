//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[6305]", typeof (Demo.Demo5juin.Entities.MonnaieEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[6307]", typeof (Demo.Demo5juin.Entities.CodeTvaEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[6308]", typeof (Demo.Demo5juin.Entities.PrixEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[6309]", typeof (Demo.Demo5juin.Entities.PrixComposeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[630Q]", typeof (Demo.Demo5juin.Entities.ArticleEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[630R]", typeof (Demo.Demo5juin.Entities.StockEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[630S]", typeof (Demo.Demo5juin.Entities.ArticleStockEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[630T]", typeof (Demo.Demo5juin.Entities.MouvementStockEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[630U]", typeof (Demo.Demo5juin.Entities.EmplacementStockEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[630V]", typeof (Demo.Demo5juin.Entities.UniteEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[63001]", typeof (Demo.Demo5juin.Entities.PositionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[63011]", typeof (Demo.Demo5juin.Entities.RabaisSurArticleEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[63021]", typeof (Demo.Demo5juin.Entities.FactureEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[63031]", typeof (Demo.Demo5juin.Entities.RappelEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[63041]", typeof (Demo.Demo5juin.Entities.MoyenDePaiementEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[63051]", typeof (Demo.Demo5juin.Entities.AffaireEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[63061]", typeof (Demo.Demo5juin.Entities.PaiementEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[63071]", typeof (Demo.Demo5juin.Entities.PrixSimpleEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[63081]", typeof (Demo.Demo5juin.Entities.AdresseEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[631]", typeof (Demo.Demo5juin.Entities.ArticleVisserieEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[6321]", typeof (Demo.Demo5juin.Entities.AdressePlusEntity))]
#region Demo.Demo5juin.Monnaie Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>Monnaie</c> entity.
	///	designer:cap/6305
	///	</summary>
	public partial class MonnaieEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Désignation</c> field.
		///	designer:fld/6305/630A
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630A]")]
		public string Désignation
		{
			get
			{
				return this.GetField<string> ("[630A]");
			}
			set
			{
				string oldValue = this.Désignation;
				if (oldValue != value || !this.IsFieldDefined("[630A]"))
				{
					this.OnDésignationChanging (oldValue, value);
					this.SetField<string> ("[630A]", oldValue, value);
					this.OnDésignationChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TauxChangeVersChf</c> field.
		///	designer:fld/6305/630B
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630B]")]
		public global::System.Decimal TauxChangeVersChf
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[630B]");
			}
			set
			{
				global::System.Decimal oldValue = this.TauxChangeVersChf;
				if (oldValue != value || !this.IsFieldDefined("[630B]"))
				{
					this.OnTauxChangeVersChfChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[630B]", oldValue, value);
					this.OnTauxChangeVersChfChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Date</c> field.
		///	designer:fld/6305/630C
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630C]")]
		public global::Epsitec.Common.Types.Date Date
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date> ("[630C]");
			}
			set
			{
				global::Epsitec.Common.Types.Date oldValue = this.Date;
				if (oldValue != value || !this.IsFieldDefined("[630C]"))
				{
					this.OnDateChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date> ("[630C]", oldValue, value);
					this.OnDateChanged (oldValue, value);
				}
			}
		}
		
		partial void OnDésignationChanging(string oldValue, string newValue);
		partial void OnDésignationChanged(string oldValue, string newValue);
		partial void OnTauxChangeVersChfChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnTauxChangeVersChfChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnDateChanging(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDateChanged(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.MonnaieEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.MonnaieEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 0, 5);	// [6305]
		public static readonly string EntityStructuredTypeKey = "[6305]";
	}
}
#endregion

#region Demo.Demo5juin.CodeTva Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>CodeTva</c> entity.
	///	designer:cap/6307
	///	</summary>
	public partial class CodeTvaEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Désignation</c> field.
		///	designer:fld/6307/630D
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630D]")]
		public string Désignation
		{
			get
			{
				return this.GetField<string> ("[630D]");
			}
			set
			{
				string oldValue = this.Désignation;
				if (oldValue != value || !this.IsFieldDefined("[630D]"))
				{
					this.OnDésignationChanging (oldValue, value);
					this.SetField<string> ("[630D]", oldValue, value);
					this.OnDésignationChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Taux</c> field.
		///	designer:fld/6307/630E
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630E]")]
		public global::System.Decimal Taux
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[630E]");
			}
			set
			{
				global::System.Decimal oldValue = this.Taux;
				if (oldValue != value || !this.IsFieldDefined("[630E]"))
				{
					this.OnTauxChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[630E]", oldValue, value);
					this.OnTauxChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DateDébutValidité</c> field.
		///	designer:fld/6307/630F
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630F]")]
		public global::Epsitec.Common.Types.Date DateDébutValidité
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date> ("[630F]");
			}
			set
			{
				global::Epsitec.Common.Types.Date oldValue = this.DateDébutValidité;
				if (oldValue != value || !this.IsFieldDefined("[630F]"))
				{
					this.OnDateDébutValiditéChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date> ("[630F]", oldValue, value);
					this.OnDateDébutValiditéChanged (oldValue, value);
				}
			}
		}
		
		partial void OnDésignationChanging(string oldValue, string newValue);
		partial void OnDésignationChanged(string oldValue, string newValue);
		partial void OnTauxChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnTauxChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnDateDébutValiditéChanging(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDateDébutValiditéChanged(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.CodeTvaEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.CodeTvaEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 0, 7);	// [6307]
		public static readonly string EntityStructuredTypeKey = "[6307]";
	}
}
#endregion

#region Demo.Demo5juin.Prix Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>Prix</c> entity.
	///	designer:cap/6308
	///	</summary>
	public partial class PrixEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Monnaie</c> field.
		///	designer:fld/6308/630G
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630G]")]
		public global::Demo.Demo5juin.Entities.MonnaieEntity Monnaie
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.MonnaieEntity> ("[630G]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.MonnaieEntity oldValue = this.Monnaie;
				if (oldValue != value || !this.IsFieldDefined("[630G]"))
				{
					this.OnMonnaieChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.MonnaieEntity> ("[630G]", oldValue, value);
					this.OnMonnaieChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Ht</c> field.
		///	designer:fld/6308/630H
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630H]")]
		public global::System.Decimal Ht
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[630H]");
			}
			set
			{
				global::System.Decimal oldValue = this.Ht;
				if (oldValue != value || !this.IsFieldDefined("[630H]"))
				{
					this.OnHtChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[630H]", oldValue, value);
					this.OnHtChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CodeTva</c> field.
		///	designer:fld/6308/630I
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630I]")]
		public global::Demo.Demo5juin.Entities.CodeTvaEntity CodeTva
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.CodeTvaEntity> ("[630I]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.CodeTvaEntity oldValue = this.CodeTva;
				if (oldValue != value || !this.IsFieldDefined("[630I]"))
				{
					this.OnCodeTvaChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.CodeTvaEntity> ("[630I]", oldValue, value);
					this.OnCodeTvaChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ArrondiTva</c> field.
		///	designer:fld/6308/630J
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630J]")]
		public global::System.Decimal ArrondiTva
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[630J]");
			}
			set
			{
				global::System.Decimal oldValue = this.ArrondiTva;
				if (oldValue != value || !this.IsFieldDefined("[630J]"))
				{
					this.OnArrondiTvaChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[630J]", oldValue, value);
					this.OnArrondiTvaChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MontantTva</c> field.
		///	designer:fld/6308/630K
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630K]")]
		public global::System.Decimal MontantTva
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[630K]");
			}
			set
			{
				global::System.Decimal oldValue = this.MontantTva;
				if (oldValue != value || !this.IsFieldDefined("[630K]"))
				{
					this.OnMontantTvaChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[630K]", oldValue, value);
					this.OnMontantTvaChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Ttc</c> field.
		///	designer:fld/6308/630L
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630L]")]
		public global::System.Decimal Ttc
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[630L]");
			}
			set
			{
				global::System.Decimal oldValue = this.Ttc;
				if (oldValue != value || !this.IsFieldDefined("[630L]"))
				{
					this.OnTtcChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[630L]", oldValue, value);
					this.OnTtcChanged (oldValue, value);
				}
			}
		}
		
		partial void OnMonnaieChanging(global::Demo.Demo5juin.Entities.MonnaieEntity oldValue, global::Demo.Demo5juin.Entities.MonnaieEntity newValue);
		partial void OnMonnaieChanged(global::Demo.Demo5juin.Entities.MonnaieEntity oldValue, global::Demo.Demo5juin.Entities.MonnaieEntity newValue);
		partial void OnHtChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnHtChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnCodeTvaChanging(global::Demo.Demo5juin.Entities.CodeTvaEntity oldValue, global::Demo.Demo5juin.Entities.CodeTvaEntity newValue);
		partial void OnCodeTvaChanged(global::Demo.Demo5juin.Entities.CodeTvaEntity oldValue, global::Demo.Demo5juin.Entities.CodeTvaEntity newValue);
		partial void OnArrondiTvaChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnArrondiTvaChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnMontantTvaChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnMontantTvaChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnTtcChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnTtcChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.PrixEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.PrixEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 0, 8);	// [6308]
		public static readonly string EntityStructuredTypeKey = "[6308]";
	}
}
#endregion

#region Demo.Demo5juin.PrixCompose Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>PrixCompose</c> entity.
	///	designer:cap/6309
	///	</summary>
	public partial class PrixComposeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Prix</c> field.
		///	designer:fld/6309/630M
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630M]")]
		public global::System.Collections.Generic.IList<global::Demo.Demo5juin.Entities.PrixEntity> Prix
		{
			get
			{
				return this.GetFieldCollection<global::Demo.Demo5juin.Entities.PrixEntity> ("[630M]");
			}
		}
		///	<summary>
		///	The <c>TotalHt</c> field.
		///	designer:fld/6309/630N
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630N]")]
		public global::System.Decimal TotalHt
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[630N]");
			}
			set
			{
				global::System.Decimal oldValue = this.TotalHt;
				if (oldValue != value || !this.IsFieldDefined("[630N]"))
				{
					this.OnTotalHtChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[630N]", oldValue, value);
					this.OnTotalHtChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TotalTtc</c> field.
		///	designer:fld/6309/630O
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630O]")]
		public global::System.Decimal TotalTtc
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[630O]");
			}
			set
			{
				global::System.Decimal oldValue = this.TotalTtc;
				if (oldValue != value || !this.IsFieldDefined("[630O]"))
				{
					this.OnTotalTtcChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[630O]", oldValue, value);
					this.OnTotalTtcChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TotalTva</c> field.
		///	designer:fld/6309/630P
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630P]")]
		public global::System.Decimal TotalTva
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[630P]");
			}
			set
			{
				global::System.Decimal oldValue = this.TotalTva;
				if (oldValue != value || !this.IsFieldDefined("[630P]"))
				{
					this.OnTotalTvaChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[630P]", oldValue, value);
					this.OnTotalTvaChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTotalHtChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnTotalHtChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnTotalTtcChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnTotalTtcChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnTotalTvaChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnTotalTvaChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.PrixComposeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.PrixComposeEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 0, 9);	// [6309]
		public static readonly string EntityStructuredTypeKey = "[6309]";
	}
}
#endregion

#region Demo.Demo5juin.Article Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>Article</c> entity.
	///	designer:cap/630Q
	///	</summary>
	public partial class ArticleEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Numéro</c> field.
		///	designer:fld/630Q/63091
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63091]")]
		public string Numéro
		{
			get
			{
				return this.GetField<string> ("[63091]");
			}
			set
			{
				string oldValue = this.Numéro;
				if (oldValue != value || !this.IsFieldDefined("[63091]"))
				{
					this.OnNuméroChanging (oldValue, value);
					this.SetField<string> ("[63091]", oldValue, value);
					this.OnNuméroChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Désignation</c> field.
		///	designer:fld/630Q/630A1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630A1]")]
		public string Désignation
		{
			get
			{
				return this.GetField<string> ("[630A1]");
			}
			set
			{
				string oldValue = this.Désignation;
				if (oldValue != value || !this.IsFieldDefined("[630A1]"))
				{
					this.OnDésignationChanging (oldValue, value);
					this.SetField<string> ("[630A1]", oldValue, value);
					this.OnDésignationChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PrixVente</c> field.
		///	designer:fld/630Q/630B1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630B1]")]
		public global::Demo.Demo5juin.Entities.PrixEntity PrixVente
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.PrixEntity> ("[630B1]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.PrixEntity oldValue = this.PrixVente;
				if (oldValue != value || !this.IsFieldDefined("[630B1]"))
				{
					this.OnPrixVenteChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.PrixEntity> ("[630B1]", oldValue, value);
					this.OnPrixVenteChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PrixAchat</c> field.
		///	designer:fld/630Q/6313
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[6313]")]
		public global::Demo.Demo5juin.Entities.PrixEntity PrixAchat
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.PrixEntity> ("[6313]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.PrixEntity oldValue = this.PrixAchat;
				if (oldValue != value || !this.IsFieldDefined("[6313]"))
				{
					this.OnPrixAchatChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.PrixEntity> ("[6313]", oldValue, value);
					this.OnPrixAchatChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Fournisseur</c> field.
		///	designer:fld/630Q/630C1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630C1]")]
		public global::Demo.Demo5juin.Entities.AdresseEntity Fournisseur
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.AdresseEntity> ("[630C1]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.AdresseEntity oldValue = this.Fournisseur;
				if (oldValue != value || !this.IsFieldDefined("[630C1]"))
				{
					this.OnFournisseurChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.AdresseEntity> ("[630C1]", oldValue, value);
					this.OnFournisseurChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Unité</c> field.
		///	designer:fld/630Q/630D1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630D1]")]
		public global::Demo.Demo5juin.Entities.UniteEntity Unité
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.UniteEntity> ("[630D1]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.UniteEntity oldValue = this.Unité;
				if (oldValue != value || !this.IsFieldDefined("[630D1]"))
				{
					this.OnUnitéChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.UniteEntity> ("[630D1]", oldValue, value);
					this.OnUnitéChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ArticlesEnStock</c> field.
		///	designer:fld/630Q/630E1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630E1]")]
		public global::System.Collections.Generic.IList<global::Demo.Demo5juin.Entities.ArticleStockEntity> ArticlesEnStock
		{
			get
			{
				return this.GetFieldCollection<global::Demo.Demo5juin.Entities.ArticleStockEntity> ("[630E1]");
			}
		}
		///	<summary>
		///	The <c>QuantitéEnStock</c> field.
		///	designer:fld/630Q/630F1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630F1]")]
		public int QuantitéEnStock
		{
			get
			{
				return this.GetField<int> ("[630F1]");
			}
			set
			{
				int oldValue = this.QuantitéEnStock;
				if (oldValue != value || !this.IsFieldDefined("[630F1]"))
				{
					this.OnQuantitéEnStockChanging (oldValue, value);
					this.SetField<int> ("[630F1]", oldValue, value);
					this.OnQuantitéEnStockChanged (oldValue, value);
				}
			}
		}
		
		partial void OnNuméroChanging(string oldValue, string newValue);
		partial void OnNuméroChanged(string oldValue, string newValue);
		partial void OnDésignationChanging(string oldValue, string newValue);
		partial void OnDésignationChanged(string oldValue, string newValue);
		partial void OnPrixVenteChanging(global::Demo.Demo5juin.Entities.PrixEntity oldValue, global::Demo.Demo5juin.Entities.PrixEntity newValue);
		partial void OnPrixVenteChanged(global::Demo.Demo5juin.Entities.PrixEntity oldValue, global::Demo.Demo5juin.Entities.PrixEntity newValue);
		partial void OnPrixAchatChanging(global::Demo.Demo5juin.Entities.PrixEntity oldValue, global::Demo.Demo5juin.Entities.PrixEntity newValue);
		partial void OnPrixAchatChanged(global::Demo.Demo5juin.Entities.PrixEntity oldValue, global::Demo.Demo5juin.Entities.PrixEntity newValue);
		partial void OnFournisseurChanging(global::Demo.Demo5juin.Entities.AdresseEntity oldValue, global::Demo.Demo5juin.Entities.AdresseEntity newValue);
		partial void OnFournisseurChanged(global::Demo.Demo5juin.Entities.AdresseEntity oldValue, global::Demo.Demo5juin.Entities.AdresseEntity newValue);
		partial void OnUnitéChanging(global::Demo.Demo5juin.Entities.UniteEntity oldValue, global::Demo.Demo5juin.Entities.UniteEntity newValue);
		partial void OnUnitéChanged(global::Demo.Demo5juin.Entities.UniteEntity oldValue, global::Demo.Demo5juin.Entities.UniteEntity newValue);
		partial void OnQuantitéEnStockChanging(int oldValue, int newValue);
		partial void OnQuantitéEnStockChanged(int oldValue, int newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.ArticleEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.ArticleEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 0, 26);	// [630Q]
		public static readonly string EntityStructuredTypeKey = "[630Q]";
	}
}
#endregion

#region Demo.Demo5juin.Stock Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>Stock</c> entity.
	///	designer:cap/630R
	///	</summary>
	public partial class StockEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Articles</c> field.
		///	designer:fld/630R/630G1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630G1]")]
		public global::System.Collections.Generic.IList<global::Demo.Demo5juin.Entities.ArticleStockEntity> Articles
		{
			get
			{
				return this.GetFieldCollection<global::Demo.Demo5juin.Entities.ArticleStockEntity> ("[630G1]");
			}
		}
		///	<summary>
		///	The <c>Désignation</c> field.
		///	designer:fld/630R/630H1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630H1]")]
		public string Désignation
		{
			get
			{
				return this.GetField<string> ("[630H1]");
			}
			set
			{
				string oldValue = this.Désignation;
				if (oldValue != value || !this.IsFieldDefined("[630H1]"))
				{
					this.OnDésignationChanging (oldValue, value);
					this.SetField<string> ("[630H1]", oldValue, value);
					this.OnDésignationChanged (oldValue, value);
				}
			}
		}
		
		partial void OnDésignationChanging(string oldValue, string newValue);
		partial void OnDésignationChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.StockEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.StockEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 0, 27);	// [630R]
		public static readonly string EntityStructuredTypeKey = "[630R]";
	}
}
#endregion

#region Demo.Demo5juin.ArticleStock Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>ArticleStock</c> entity.
	///	designer:cap/630S
	///	</summary>
	public partial class ArticleStockEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Article</c> field.
		///	designer:fld/630S/630I1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630I1]")]
		public global::Demo.Demo5juin.Entities.ArticleEntity Article
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.ArticleEntity> ("[630I1]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.ArticleEntity oldValue = this.Article;
				if (oldValue != value || !this.IsFieldDefined("[630I1]"))
				{
					this.OnArticleChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.ArticleEntity> ("[630I1]", oldValue, value);
					this.OnArticleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Emplacement</c> field.
		///	designer:fld/630S/630J1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630J1]")]
		public global::Demo.Demo5juin.Entities.EmplacementStockEntity Emplacement
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.EmplacementStockEntity> ("[630J1]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.EmplacementStockEntity oldValue = this.Emplacement;
				if (oldValue != value || !this.IsFieldDefined("[630J1]"))
				{
					this.OnEmplacementChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.EmplacementStockEntity> ("[630J1]", oldValue, value);
					this.OnEmplacementChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Mouvements</c> field.
		///	designer:fld/630S/630K1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630K1]")]
		public global::Demo.Demo5juin.Entities.MouvementStockEntity Mouvements
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.MouvementStockEntity> ("[630K1]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.MouvementStockEntity oldValue = this.Mouvements;
				if (oldValue != value || !this.IsFieldDefined("[630K1]"))
				{
					this.OnMouvementsChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.MouvementStockEntity> ("[630K1]", oldValue, value);
					this.OnMouvementsChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TotalValeurAchat</c> field.
		///	designer:fld/630S/630L1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630L1]")]
		public global::Demo.Demo5juin.Entities.PrixComposeEntity TotalValeurAchat
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.PrixComposeEntity> ("[630L1]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.PrixComposeEntity oldValue = this.TotalValeurAchat;
				if (oldValue != value || !this.IsFieldDefined("[630L1]"))
				{
					this.OnTotalValeurAchatChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.PrixComposeEntity> ("[630L1]", oldValue, value);
					this.OnTotalValeurAchatChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TotalValeurVente</c> field.
		///	designer:fld/630S/630M1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630M1]")]
		public global::Demo.Demo5juin.Entities.PrixComposeEntity TotalValeurVente
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.PrixComposeEntity> ("[630M1]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.PrixComposeEntity oldValue = this.TotalValeurVente;
				if (oldValue != value || !this.IsFieldDefined("[630M1]"))
				{
					this.OnTotalValeurVenteChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.PrixComposeEntity> ("[630M1]", oldValue, value);
					this.OnTotalValeurVenteChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>QuantitéEnStock</c> field.
		///	designer:fld/630S/630N1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630N1]")]
		public int QuantitéEnStock
		{
			get
			{
				return this.GetField<int> ("[630N1]");
			}
			set
			{
				int oldValue = this.QuantitéEnStock;
				if (oldValue != value || !this.IsFieldDefined("[630N1]"))
				{
					this.OnQuantitéEnStockChanging (oldValue, value);
					this.SetField<int> ("[630N1]", oldValue, value);
					this.OnQuantitéEnStockChanged (oldValue, value);
				}
			}
		}
		
		partial void OnArticleChanging(global::Demo.Demo5juin.Entities.ArticleEntity oldValue, global::Demo.Demo5juin.Entities.ArticleEntity newValue);
		partial void OnArticleChanged(global::Demo.Demo5juin.Entities.ArticleEntity oldValue, global::Demo.Demo5juin.Entities.ArticleEntity newValue);
		partial void OnEmplacementChanging(global::Demo.Demo5juin.Entities.EmplacementStockEntity oldValue, global::Demo.Demo5juin.Entities.EmplacementStockEntity newValue);
		partial void OnEmplacementChanged(global::Demo.Demo5juin.Entities.EmplacementStockEntity oldValue, global::Demo.Demo5juin.Entities.EmplacementStockEntity newValue);
		partial void OnMouvementsChanging(global::Demo.Demo5juin.Entities.MouvementStockEntity oldValue, global::Demo.Demo5juin.Entities.MouvementStockEntity newValue);
		partial void OnMouvementsChanged(global::Demo.Demo5juin.Entities.MouvementStockEntity oldValue, global::Demo.Demo5juin.Entities.MouvementStockEntity newValue);
		partial void OnTotalValeurAchatChanging(global::Demo.Demo5juin.Entities.PrixComposeEntity oldValue, global::Demo.Demo5juin.Entities.PrixComposeEntity newValue);
		partial void OnTotalValeurAchatChanged(global::Demo.Demo5juin.Entities.PrixComposeEntity oldValue, global::Demo.Demo5juin.Entities.PrixComposeEntity newValue);
		partial void OnTotalValeurVenteChanging(global::Demo.Demo5juin.Entities.PrixComposeEntity oldValue, global::Demo.Demo5juin.Entities.PrixComposeEntity newValue);
		partial void OnTotalValeurVenteChanged(global::Demo.Demo5juin.Entities.PrixComposeEntity oldValue, global::Demo.Demo5juin.Entities.PrixComposeEntity newValue);
		partial void OnQuantitéEnStockChanging(int oldValue, int newValue);
		partial void OnQuantitéEnStockChanged(int oldValue, int newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.ArticleStockEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.ArticleStockEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 0, 28);	// [630S]
		public static readonly string EntityStructuredTypeKey = "[630S]";
	}
}
#endregion

#region Demo.Demo5juin.MouvementStock Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>MouvementStock</c> entity.
	///	designer:cap/630T
	///	</summary>
	public partial class MouvementStockEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>PrixAchatOuVente</c> field.
		///	designer:fld/630T/630O1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630O1]")]
		public global::Demo.Demo5juin.Entities.PrixEntity PrixAchatOuVente
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.PrixEntity> ("[630O1]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.PrixEntity oldValue = this.PrixAchatOuVente;
				if (oldValue != value || !this.IsFieldDefined("[630O1]"))
				{
					this.OnPrixAchatOuVenteChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.PrixEntity> ("[630O1]", oldValue, value);
					this.OnPrixAchatOuVenteChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Opération</c> field.
		///	designer:fld/630T/630P1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630P1]")]
		public int Opération
		{
			get
			{
				return this.GetField<int> ("[630P1]");
			}
			set
			{
				int oldValue = this.Opération;
				if (oldValue != value || !this.IsFieldDefined("[630P1]"))
				{
					this.OnOpérationChanging (oldValue, value);
					this.SetField<int> ("[630P1]", oldValue, value);
					this.OnOpérationChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ModifQuantité</c> field.
		///	designer:fld/630T/630Q1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630Q1]")]
		public int ModifQuantité
		{
			get
			{
				return this.GetField<int> ("[630Q1]");
			}
			set
			{
				int oldValue = this.ModifQuantité;
				if (oldValue != value || !this.IsFieldDefined("[630Q1]"))
				{
					this.OnModifQuantitéChanging (oldValue, value);
					this.SetField<int> ("[630Q1]", oldValue, value);
					this.OnModifQuantitéChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Date</c> field.
		///	designer:fld/630T/630R1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630R1]")]
		public global::Epsitec.Common.Types.Date Date
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date> ("[630R1]");
			}
			set
			{
				global::Epsitec.Common.Types.Date oldValue = this.Date;
				if (oldValue != value || !this.IsFieldDefined("[630R1]"))
				{
					this.OnDateChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date> ("[630R1]", oldValue, value);
					this.OnDateChanged (oldValue, value);
				}
			}
		}
		
		partial void OnPrixAchatOuVenteChanging(global::Demo.Demo5juin.Entities.PrixEntity oldValue, global::Demo.Demo5juin.Entities.PrixEntity newValue);
		partial void OnPrixAchatOuVenteChanged(global::Demo.Demo5juin.Entities.PrixEntity oldValue, global::Demo.Demo5juin.Entities.PrixEntity newValue);
		partial void OnOpérationChanging(int oldValue, int newValue);
		partial void OnOpérationChanged(int oldValue, int newValue);
		partial void OnModifQuantitéChanging(int oldValue, int newValue);
		partial void OnModifQuantitéChanged(int oldValue, int newValue);
		partial void OnDateChanging(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDateChanged(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.MouvementStockEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.MouvementStockEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 0, 29);	// [630T]
		public static readonly string EntityStructuredTypeKey = "[630T]";
	}
}
#endregion

#region Demo.Demo5juin.EmplacementStock Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>EmplacementStock</c> entity.
	///	designer:cap/630U
	///	</summary>
	public partial class EmplacementStockEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Stock</c> field.
		///	designer:fld/630U/630S1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630S1]")]
		public global::Demo.Demo5juin.Entities.StockEntity Stock
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.StockEntity> ("[630S1]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.StockEntity oldValue = this.Stock;
				if (oldValue != value || !this.IsFieldDefined("[630S1]"))
				{
					this.OnStockChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.StockEntity> ("[630S1]", oldValue, value);
					this.OnStockChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Etage</c> field.
		///	designer:fld/630U/630T1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630T1]")]
		public string Etage
		{
			get
			{
				return this.GetField<string> ("[630T1]");
			}
			set
			{
				string oldValue = this.Etage;
				if (oldValue != value || !this.IsFieldDefined("[630T1]"))
				{
					this.OnEtageChanging (oldValue, value);
					this.SetField<string> ("[630T1]", oldValue, value);
					this.OnEtageChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Allée</c> field.
		///	designer:fld/630U/630U1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630U1]")]
		public string Allée
		{
			get
			{
				return this.GetField<string> ("[630U1]");
			}
			set
			{
				string oldValue = this.Allée;
				if (oldValue != value || !this.IsFieldDefined("[630U1]"))
				{
					this.OnAlléeChanging (oldValue, value);
					this.SetField<string> ("[630U1]", oldValue, value);
					this.OnAlléeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Casier</c> field.
		///	designer:fld/630U/630V1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630V1]")]
		public string Casier
		{
			get
			{
				return this.GetField<string> ("[630V1]");
			}
			set
			{
				string oldValue = this.Casier;
				if (oldValue != value || !this.IsFieldDefined("[630V1]"))
				{
					this.OnCasierChanging (oldValue, value);
					this.SetField<string> ("[630V1]", oldValue, value);
					this.OnCasierChanged (oldValue, value);
				}
			}
		}
		
		partial void OnStockChanging(global::Demo.Demo5juin.Entities.StockEntity oldValue, global::Demo.Demo5juin.Entities.StockEntity newValue);
		partial void OnStockChanged(global::Demo.Demo5juin.Entities.StockEntity oldValue, global::Demo.Demo5juin.Entities.StockEntity newValue);
		partial void OnEtageChanging(string oldValue, string newValue);
		partial void OnEtageChanged(string oldValue, string newValue);
		partial void OnAlléeChanging(string oldValue, string newValue);
		partial void OnAlléeChanged(string oldValue, string newValue);
		partial void OnCasierChanging(string oldValue, string newValue);
		partial void OnCasierChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.EmplacementStockEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.EmplacementStockEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 0, 30);	// [630U]
		public static readonly string EntityStructuredTypeKey = "[630U]";
	}
}
#endregion

#region Demo.Demo5juin.Unite Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>Unite</c> entity.
	///	designer:cap/630V
	///	</summary>
	public partial class UniteEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Désignation</c> field.
		///	designer:fld/630V/63002
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63002]")]
		public string Désignation
		{
			get
			{
				return this.GetField<string> ("[63002]");
			}
			set
			{
				string oldValue = this.Désignation;
				if (oldValue != value || !this.IsFieldDefined("[63002]"))
				{
					this.OnDésignationChanging (oldValue, value);
					this.SetField<string> ("[63002]", oldValue, value);
					this.OnDésignationChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/630V/63012
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63012]")]
		public int Code
		{
			get
			{
				return this.GetField<int> ("[63012]");
			}
			set
			{
				int oldValue = this.Code;
				if (oldValue != value || !this.IsFieldDefined("[63012]"))
				{
					this.OnCodeChanging (oldValue, value);
					this.SetField<int> ("[63012]", oldValue, value);
					this.OnCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>UnitéDeBase</c> field.
		///	designer:fld/630V/63022
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63022]")]
		public global::Demo.Demo5juin.Entities.UniteEntity UnitéDeBase
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.UniteEntity> ("[63022]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.UniteEntity oldValue = this.UnitéDeBase;
				if (oldValue != value || !this.IsFieldDefined("[63022]"))
				{
					this.OnUnitéDeBaseChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.UniteEntity> ("[63022]", oldValue, value);
					this.OnUnitéDeBaseChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MultipleUnitéDeBase</c> field.
		///	designer:fld/630V/63032
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63032]")]
		public global::System.Decimal MultipleUnitéDeBase
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[63032]");
			}
			set
			{
				global::System.Decimal oldValue = this.MultipleUnitéDeBase;
				if (oldValue != value || !this.IsFieldDefined("[63032]"))
				{
					this.OnMultipleUnitéDeBaseChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[63032]", oldValue, value);
					this.OnMultipleUnitéDeBaseChanged (oldValue, value);
				}
			}
		}
		
		partial void OnDésignationChanging(string oldValue, string newValue);
		partial void OnDésignationChanged(string oldValue, string newValue);
		partial void OnCodeChanging(int oldValue, int newValue);
		partial void OnCodeChanged(int oldValue, int newValue);
		partial void OnUnitéDeBaseChanging(global::Demo.Demo5juin.Entities.UniteEntity oldValue, global::Demo.Demo5juin.Entities.UniteEntity newValue);
		partial void OnUnitéDeBaseChanged(global::Demo.Demo5juin.Entities.UniteEntity oldValue, global::Demo.Demo5juin.Entities.UniteEntity newValue);
		partial void OnMultipleUnitéDeBaseChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnMultipleUnitéDeBaseChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.UniteEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.UniteEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 0, 31);	// [630V]
		public static readonly string EntityStructuredTypeKey = "[630V]";
	}
}
#endregion

#region Demo.Demo5juin.Position Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>Position</c> entity.
	///	designer:cap/63001
	///	</summary>
	public partial class PositionEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Article</c> field.
		///	designer:fld/63001/63042
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63042]")]
		public global::Demo.Demo5juin.Entities.ArticleEntity Article
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.ArticleEntity> ("[63042]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.ArticleEntity oldValue = this.Article;
				if (oldValue != value || !this.IsFieldDefined("[63042]"))
				{
					this.OnArticleChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.ArticleEntity> ("[63042]", oldValue, value);
					this.OnArticleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Quantité</c> field.
		///	designer:fld/63001/63052
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63052]")]
		public global::System.Decimal? Quantité
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[63052]");
			}
			set
			{
				global::System.Decimal? oldValue = this.Quantité;
				if (oldValue != value || !this.IsFieldDefined("[63052]"))
				{
					this.OnQuantitéChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[63052]", oldValue, value);
					this.OnQuantitéChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Unité</c> field.
		///	designer:fld/63001/63062
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63062]")]
		public global::Demo.Demo5juin.Entities.UniteEntity Unité
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.UniteEntity> ("[63062]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.UniteEntity oldValue = this.Unité;
				if (oldValue != value || !this.IsFieldDefined("[63062]"))
				{
					this.OnUnitéChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.UniteEntity> ("[63062]", oldValue, value);
					this.OnUnitéChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Rabais</c> field.
		///	designer:fld/63001/63072
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63072]")]
		public global::System.Collections.Generic.IList<global::Demo.Demo5juin.Entities.RabaisSurArticleEntity> Rabais
		{
			get
			{
				return this.GetFieldCollection<global::Demo.Demo5juin.Entities.RabaisSurArticleEntity> ("[63072]");
			}
		}
		
		partial void OnArticleChanging(global::Demo.Demo5juin.Entities.ArticleEntity oldValue, global::Demo.Demo5juin.Entities.ArticleEntity newValue);
		partial void OnArticleChanged(global::Demo.Demo5juin.Entities.ArticleEntity oldValue, global::Demo.Demo5juin.Entities.ArticleEntity newValue);
		partial void OnQuantitéChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnQuantitéChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnUnitéChanging(global::Demo.Demo5juin.Entities.UniteEntity oldValue, global::Demo.Demo5juin.Entities.UniteEntity newValue);
		partial void OnUnitéChanged(global::Demo.Demo5juin.Entities.UniteEntity oldValue, global::Demo.Demo5juin.Entities.UniteEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.PositionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.PositionEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 0, 32);	// [63001]
		public static readonly string EntityStructuredTypeKey = "[63001]";
	}
}
#endregion

#region Demo.Demo5juin.RabaisSurArticle Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>RabaisSurArticle</c> entity.
	///	designer:cap/63011
	///	</summary>
	public partial class RabaisSurArticleEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Pourcent</c> field.
		///	designer:fld/63011/63082
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63082]")]
		public global::System.Decimal Pourcent
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[63082]");
			}
			set
			{
				global::System.Decimal oldValue = this.Pourcent;
				if (oldValue != value || !this.IsFieldDefined("[63082]"))
				{
					this.OnPourcentChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[63082]", oldValue, value);
					this.OnPourcentChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CodeRaison</c> field.
		///	designer:fld/63011/63092
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63092]")]
		public int CodeRaison
		{
			get
			{
				return this.GetField<int> ("[63092]");
			}
			set
			{
				int oldValue = this.CodeRaison;
				if (oldValue != value || !this.IsFieldDefined("[63092]"))
				{
					this.OnCodeRaisonChanging (oldValue, value);
					this.SetField<int> ("[63092]", oldValue, value);
					this.OnCodeRaisonChanged (oldValue, value);
				}
			}
		}
		
		partial void OnPourcentChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnPourcentChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnCodeRaisonChanging(int oldValue, int newValue);
		partial void OnCodeRaisonChanged(int oldValue, int newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.RabaisSurArticleEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.RabaisSurArticleEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 0, 33);	// [63011]
		public static readonly string EntityStructuredTypeKey = "[63011]";
	}
}
#endregion

#region Demo.Demo5juin.Facture Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>Facture</c> entity.
	///	designer:cap/63021
	///	</summary>
	public partial class FactureEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Numéro</c> field.
		///	designer:fld/63021/630A2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630A2]")]
		public string Numéro
		{
			get
			{
				return this.GetField<string> ("[630A2]");
			}
			set
			{
				string oldValue = this.Numéro;
				if (oldValue != value || !this.IsFieldDefined("[630A2]"))
				{
					this.OnNuméroChanging (oldValue, value);
					this.SetField<string> ("[630A2]", oldValue, value);
					this.OnNuméroChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Affaire</c> field.
		///	designer:fld/63021/630B2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630B2]")]
		public global::Demo.Demo5juin.Entities.AffaireEntity Affaire
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.AffaireEntity> ("[630B2]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.AffaireEntity oldValue = this.Affaire;
				if (oldValue != value || !this.IsFieldDefined("[630B2]"))
				{
					this.OnAffaireChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.AffaireEntity> ("[630B2]", oldValue, value);
					this.OnAffaireChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DateTravail</c> field.
		///	designer:fld/63021/630C2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630C2]")]
		public global::Epsitec.Common.Types.Date DateTravail
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date> ("[630C2]");
			}
			set
			{
				global::Epsitec.Common.Types.Date oldValue = this.DateTravail;
				if (oldValue != value || !this.IsFieldDefined("[630C2]"))
				{
					this.OnDateTravailChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date> ("[630C2]", oldValue, value);
					this.OnDateTravailChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DateFacture</c> field.
		///	designer:fld/63021/630D2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630D2]")]
		public global::Epsitec.Common.Types.Date DateFacture
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date> ("[630D2]");
			}
			set
			{
				global::Epsitec.Common.Types.Date oldValue = this.DateFacture;
				if (oldValue != value || !this.IsFieldDefined("[630D2]"))
				{
					this.OnDateFactureChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date> ("[630D2]", oldValue, value);
					this.OnDateFactureChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DateEchéance</c> field.
		///	designer:fld/63021/630E2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630E2]")]
		public global::Epsitec.Common.Types.Date DateEchéance
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date> ("[630E2]");
			}
			set
			{
				global::Epsitec.Common.Types.Date oldValue = this.DateEchéance;
				if (oldValue != value || !this.IsFieldDefined("[630E2]"))
				{
					this.OnDateEchéanceChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date> ("[630E2]", oldValue, value);
					this.OnDateEchéanceChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AdresseFacturation</c> field.
		///	designer:fld/63021/630F2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630F2]")]
		public global::Demo.Demo5juin.Entities.AdressePlusEntity AdresseFacturation
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.AdressePlusEntity> ("[630F2]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.AdressePlusEntity oldValue = this.AdresseFacturation;
				if (oldValue != value || !this.IsFieldDefined("[630F2]"))
				{
					this.OnAdresseFacturationChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.AdressePlusEntity> ("[630F2]", oldValue, value);
					this.OnAdresseFacturationChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AdresseLivraison</c> field.
		///	designer:fld/63021/630G2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630G2]")]
		public global::Demo.Demo5juin.Entities.AdressePlusEntity AdresseLivraison
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.AdressePlusEntity> ("[630G2]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.AdressePlusEntity oldValue = this.AdresseLivraison;
				if (oldValue != value || !this.IsFieldDefined("[630G2]"))
				{
					this.OnAdresseLivraisonChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.AdressePlusEntity> ("[630G2]", oldValue, value);
					this.OnAdresseLivraisonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Positions</c> field.
		///	designer:fld/63021/630H2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630H2]")]
		public global::System.Collections.Generic.IList<global::Demo.Demo5juin.Entities.PositionEntity> Positions
		{
			get
			{
				return this.GetFieldCollection<global::Demo.Demo5juin.Entities.PositionEntity> ("[630H2]");
			}
		}
		///	<summary>
		///	The <c>FraisDePort</c> field.
		///	designer:fld/63021/630I2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630I2]")]
		public global::Demo.Demo5juin.Entities.PrixEntity FraisDePort
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.PrixEntity> ("[630I2]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.PrixEntity oldValue = this.FraisDePort;
				if (oldValue != value || !this.IsFieldDefined("[630I2]"))
				{
					this.OnFraisDePortChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.PrixEntity> ("[630I2]", oldValue, value);
					this.OnFraisDePortChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MoyenDePaiement</c> field.
		///	designer:fld/63021/630J2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630J2]")]
		public global::Demo.Demo5juin.Entities.MoyenDePaiementEntity MoyenDePaiement
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.MoyenDePaiementEntity> ("[630J2]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.MoyenDePaiementEntity oldValue = this.MoyenDePaiement;
				if (oldValue != value || !this.IsFieldDefined("[630J2]"))
				{
					this.OnMoyenDePaiementChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.MoyenDePaiementEntity> ("[630J2]", oldValue, value);
					this.OnMoyenDePaiementChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Remise</c> field.
		///	designer:fld/63021/630K2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630K2]")]
		public global::Demo.Demo5juin.Entities.PrixEntity Remise
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.PrixEntity> ("[630K2]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.PrixEntity oldValue = this.Remise;
				if (oldValue != value || !this.IsFieldDefined("[630K2]"))
				{
					this.OnRemiseChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.PrixEntity> ("[630K2]", oldValue, value);
					this.OnRemiseChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TotalFacturé</c> field.
		///	designer:fld/63021/630L2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630L2]")]
		public global::Demo.Demo5juin.Entities.PrixComposeEntity TotalFacturé
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.PrixComposeEntity> ("[630L2]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.PrixComposeEntity oldValue = this.TotalFacturé;
				if (oldValue != value || !this.IsFieldDefined("[630L2]"))
				{
					this.OnTotalFacturéChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.PrixComposeEntity> ("[630L2]", oldValue, value);
					this.OnTotalFacturéChanged (oldValue, value);
				}
			}
		}
		
		partial void OnNuméroChanging(string oldValue, string newValue);
		partial void OnNuméroChanged(string oldValue, string newValue);
		partial void OnAffaireChanging(global::Demo.Demo5juin.Entities.AffaireEntity oldValue, global::Demo.Demo5juin.Entities.AffaireEntity newValue);
		partial void OnAffaireChanged(global::Demo.Demo5juin.Entities.AffaireEntity oldValue, global::Demo.Demo5juin.Entities.AffaireEntity newValue);
		partial void OnDateTravailChanging(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDateTravailChanged(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDateFactureChanging(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDateFactureChanged(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDateEchéanceChanging(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDateEchéanceChanged(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnAdresseFacturationChanging(global::Demo.Demo5juin.Entities.AdressePlusEntity oldValue, global::Demo.Demo5juin.Entities.AdressePlusEntity newValue);
		partial void OnAdresseFacturationChanged(global::Demo.Demo5juin.Entities.AdressePlusEntity oldValue, global::Demo.Demo5juin.Entities.AdressePlusEntity newValue);
		partial void OnAdresseLivraisonChanging(global::Demo.Demo5juin.Entities.AdressePlusEntity oldValue, global::Demo.Demo5juin.Entities.AdressePlusEntity newValue);
		partial void OnAdresseLivraisonChanged(global::Demo.Demo5juin.Entities.AdressePlusEntity oldValue, global::Demo.Demo5juin.Entities.AdressePlusEntity newValue);
		partial void OnFraisDePortChanging(global::Demo.Demo5juin.Entities.PrixEntity oldValue, global::Demo.Demo5juin.Entities.PrixEntity newValue);
		partial void OnFraisDePortChanged(global::Demo.Demo5juin.Entities.PrixEntity oldValue, global::Demo.Demo5juin.Entities.PrixEntity newValue);
		partial void OnMoyenDePaiementChanging(global::Demo.Demo5juin.Entities.MoyenDePaiementEntity oldValue, global::Demo.Demo5juin.Entities.MoyenDePaiementEntity newValue);
		partial void OnMoyenDePaiementChanged(global::Demo.Demo5juin.Entities.MoyenDePaiementEntity oldValue, global::Demo.Demo5juin.Entities.MoyenDePaiementEntity newValue);
		partial void OnRemiseChanging(global::Demo.Demo5juin.Entities.PrixEntity oldValue, global::Demo.Demo5juin.Entities.PrixEntity newValue);
		partial void OnRemiseChanged(global::Demo.Demo5juin.Entities.PrixEntity oldValue, global::Demo.Demo5juin.Entities.PrixEntity newValue);
		partial void OnTotalFacturéChanging(global::Demo.Demo5juin.Entities.PrixComposeEntity oldValue, global::Demo.Demo5juin.Entities.PrixComposeEntity newValue);
		partial void OnTotalFacturéChanged(global::Demo.Demo5juin.Entities.PrixComposeEntity oldValue, global::Demo.Demo5juin.Entities.PrixComposeEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.FactureEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.FactureEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 0, 34);	// [63021]
		public static readonly string EntityStructuredTypeKey = "[63021]";
	}
}
#endregion

#region Demo.Demo5juin.Rappel Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>Rappel</c> entity.
	///	designer:cap/63031
	///	</summary>
	public partial class RappelEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Nième</c> field.
		///	designer:fld/63031/630M2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630M2]")]
		public int Nième
		{
			get
			{
				return this.GetField<int> ("[630M2]");
			}
			set
			{
				int oldValue = this.Nième;
				if (oldValue != value || !this.IsFieldDefined("[630M2]"))
				{
					this.OnNièmeChanging (oldValue, value);
					this.SetField<int> ("[630M2]", oldValue, value);
					this.OnNièmeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Texte</c> field.
		///	designer:fld/63031/630N2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630N2]")]
		public string Texte
		{
			get
			{
				return this.GetField<string> ("[630N2]");
			}
			set
			{
				string oldValue = this.Texte;
				if (oldValue != value || !this.IsFieldDefined("[630N2]"))
				{
					this.OnTexteChanging (oldValue, value);
					this.SetField<string> ("[630N2]", oldValue, value);
					this.OnTexteChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Facture</c> field.
		///	designer:fld/63031/630O2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630O2]")]
		public global::Demo.Demo5juin.Entities.FactureEntity Facture
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.FactureEntity> ("[630O2]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.FactureEntity oldValue = this.Facture;
				if (oldValue != value || !this.IsFieldDefined("[630O2]"))
				{
					this.OnFactureChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.FactureEntity> ("[630O2]", oldValue, value);
					this.OnFactureChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>FraisRappel</c> field.
		///	designer:fld/63031/630P2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630P2]")]
		public global::Demo.Demo5juin.Entities.PrixEntity FraisRappel
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.PrixEntity> ("[630P2]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.PrixEntity oldValue = this.FraisRappel;
				if (oldValue != value || !this.IsFieldDefined("[630P2]"))
				{
					this.OnFraisRappelChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.PrixEntity> ("[630P2]", oldValue, value);
					this.OnFraisRappelChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TotalRappel</c> field.
		///	designer:fld/63031/630Q2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630Q2]")]
		public global::Demo.Demo5juin.Entities.PrixComposeEntity TotalRappel
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.PrixComposeEntity> ("[630Q2]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.PrixComposeEntity oldValue = this.TotalRappel;
				if (oldValue != value || !this.IsFieldDefined("[630Q2]"))
				{
					this.OnTotalRappelChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.PrixComposeEntity> ("[630Q2]", oldValue, value);
					this.OnTotalRappelChanged (oldValue, value);
				}
			}
		}
		
		partial void OnNièmeChanging(int oldValue, int newValue);
		partial void OnNièmeChanged(int oldValue, int newValue);
		partial void OnTexteChanging(string oldValue, string newValue);
		partial void OnTexteChanged(string oldValue, string newValue);
		partial void OnFactureChanging(global::Demo.Demo5juin.Entities.FactureEntity oldValue, global::Demo.Demo5juin.Entities.FactureEntity newValue);
		partial void OnFactureChanged(global::Demo.Demo5juin.Entities.FactureEntity oldValue, global::Demo.Demo5juin.Entities.FactureEntity newValue);
		partial void OnFraisRappelChanging(global::Demo.Demo5juin.Entities.PrixEntity oldValue, global::Demo.Demo5juin.Entities.PrixEntity newValue);
		partial void OnFraisRappelChanged(global::Demo.Demo5juin.Entities.PrixEntity oldValue, global::Demo.Demo5juin.Entities.PrixEntity newValue);
		partial void OnTotalRappelChanging(global::Demo.Demo5juin.Entities.PrixComposeEntity oldValue, global::Demo.Demo5juin.Entities.PrixComposeEntity newValue);
		partial void OnTotalRappelChanged(global::Demo.Demo5juin.Entities.PrixComposeEntity oldValue, global::Demo.Demo5juin.Entities.PrixComposeEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.RappelEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.RappelEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 0, 35);	// [63031]
		public static readonly string EntityStructuredTypeKey = "[63031]";
	}
}
#endregion

#region Demo.Demo5juin.MoyenDePaiement Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>MoyenDePaiement</c> entity.
	///	designer:cap/63041
	///	</summary>
	public partial class MoyenDePaiementEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Désignation</c> field.
		///	designer:fld/63041/630R2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630R2]")]
		public string Désignation
		{
			get
			{
				return this.GetField<string> ("[630R2]");
			}
			set
			{
				string oldValue = this.Désignation;
				if (oldValue != value || !this.IsFieldDefined("[630R2]"))
				{
					this.OnDésignationChanging (oldValue, value);
					this.SetField<string> ("[630R2]", oldValue, value);
					this.OnDésignationChanged (oldValue, value);
				}
			}
		}
		
		partial void OnDésignationChanging(string oldValue, string newValue);
		partial void OnDésignationChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.MoyenDePaiementEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.MoyenDePaiementEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 0, 36);	// [63041]
		public static readonly string EntityStructuredTypeKey = "[63041]";
	}
}
#endregion

#region Demo.Demo5juin.Affaire Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>Affaire</c> entity.
	///	designer:cap/63051
	///	</summary>
	public partial class AffaireEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Client</c> field.
		///	designer:fld/63051/630S2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630S2]")]
		public string Client
		{
			get
			{
				return this.GetField<string> ("[630S2]");
			}
			set
			{
				string oldValue = this.Client;
				if (oldValue != value || !this.IsFieldDefined("[630S2]"))
				{
					this.OnClientChanging (oldValue, value);
					this.SetField<string> ("[630S2]", oldValue, value);
					this.OnClientChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Désignation</c> field.
		///	designer:fld/63051/630T2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630T2]")]
		public string Désignation
		{
			get
			{
				return this.GetField<string> ("[630T2]");
			}
			set
			{
				string oldValue = this.Désignation;
				if (oldValue != value || !this.IsFieldDefined("[630T2]"))
				{
					this.OnDésignationChanging (oldValue, value);
					this.SetField<string> ("[630T2]", oldValue, value);
					this.OnDésignationChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Facture</c> field.
		///	designer:fld/63051/630U2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630U2]")]
		public global::Demo.Demo5juin.Entities.FactureEntity Facture
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.FactureEntity> ("[630U2]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.FactureEntity oldValue = this.Facture;
				if (oldValue != value || !this.IsFieldDefined("[630U2]"))
				{
					this.OnFactureChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.FactureEntity> ("[630U2]", oldValue, value);
					this.OnFactureChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Rappels</c> field.
		///	designer:fld/63051/630V2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630V2]")]
		public global::System.Collections.Generic.IList<global::Demo.Demo5juin.Entities.RappelEntity> Rappels
		{
			get
			{
				return this.GetFieldCollection<global::Demo.Demo5juin.Entities.RappelEntity> ("[630V2]");
			}
		}
		///	<summary>
		///	The <c>Paiements</c> field.
		///	designer:fld/63051/63003
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63003]")]
		public global::System.Collections.Generic.IList<global::Demo.Demo5juin.Entities.PaiementEntity> Paiements
		{
			get
			{
				return this.GetFieldCollection<global::Demo.Demo5juin.Entities.PaiementEntity> ("[63003]");
			}
		}
		///	<summary>
		///	The <c>SoldeDû</c> field.
		///	designer:fld/63051/63013
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63013]")]
		public global::System.Collections.Generic.IList<global::Demo.Demo5juin.Entities.PrixSimpleEntity> SoldeDû
		{
			get
			{
				return this.GetFieldCollection<global::Demo.Demo5juin.Entities.PrixSimpleEntity> ("[63013]");
			}
		}
		///	<summary>
		///	The <c>SoldeCadeau</c> field.
		///	designer:fld/63051/63023
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63023]")]
		public global::System.Collections.Generic.IList<global::Demo.Demo5juin.Entities.PrixSimpleEntity> SoldeCadeau
		{
			get
			{
				return this.GetFieldCollection<global::Demo.Demo5juin.Entities.PrixSimpleEntity> ("[63023]");
			}
		}
		
		partial void OnClientChanging(string oldValue, string newValue);
		partial void OnClientChanged(string oldValue, string newValue);
		partial void OnDésignationChanging(string oldValue, string newValue);
		partial void OnDésignationChanged(string oldValue, string newValue);
		partial void OnFactureChanging(global::Demo.Demo5juin.Entities.FactureEntity oldValue, global::Demo.Demo5juin.Entities.FactureEntity newValue);
		partial void OnFactureChanged(global::Demo.Demo5juin.Entities.FactureEntity oldValue, global::Demo.Demo5juin.Entities.FactureEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.AffaireEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.AffaireEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 0, 37);	// [63051]
		public static readonly string EntityStructuredTypeKey = "[63051]";
	}
}
#endregion

#region Demo.Demo5juin.Paiement Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>Paiement</c> entity.
	///	designer:cap/63061
	///	</summary>
	public partial class PaiementEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Moyen</c> field.
		///	designer:fld/63061/63033
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63033]")]
		public global::Demo.Demo5juin.Entities.MoyenDePaiementEntity Moyen
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.MoyenDePaiementEntity> ("[63033]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.MoyenDePaiementEntity oldValue = this.Moyen;
				if (oldValue != value || !this.IsFieldDefined("[63033]"))
				{
					this.OnMoyenChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.MoyenDePaiementEntity> ("[63033]", oldValue, value);
					this.OnMoyenChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Valeur</c> field.
		///	designer:fld/63061/63043
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63043]")]
		public global::Demo.Demo5juin.Entities.PrixSimpleEntity Valeur
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.PrixSimpleEntity> ("[63043]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.PrixSimpleEntity oldValue = this.Valeur;
				if (oldValue != value || !this.IsFieldDefined("[63043]"))
				{
					this.OnValeurChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.PrixSimpleEntity> ("[63043]", oldValue, value);
					this.OnValeurChanged (oldValue, value);
				}
			}
		}
		
		partial void OnMoyenChanging(global::Demo.Demo5juin.Entities.MoyenDePaiementEntity oldValue, global::Demo.Demo5juin.Entities.MoyenDePaiementEntity newValue);
		partial void OnMoyenChanged(global::Demo.Demo5juin.Entities.MoyenDePaiementEntity oldValue, global::Demo.Demo5juin.Entities.MoyenDePaiementEntity newValue);
		partial void OnValeurChanging(global::Demo.Demo5juin.Entities.PrixSimpleEntity oldValue, global::Demo.Demo5juin.Entities.PrixSimpleEntity newValue);
		partial void OnValeurChanged(global::Demo.Demo5juin.Entities.PrixSimpleEntity oldValue, global::Demo.Demo5juin.Entities.PrixSimpleEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.PaiementEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.PaiementEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 0, 38);	// [63061]
		public static readonly string EntityStructuredTypeKey = "[63061]";
	}
}
#endregion

#region Demo.Demo5juin.PrixSimple Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>PrixSimple</c> entity.
	///	designer:cap/63071
	///	</summary>
	public partial class PrixSimpleEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Montant</c> field.
		///	designer:fld/63071/63053
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63053]")]
		public global::System.Decimal Montant
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[63053]");
			}
			set
			{
				global::System.Decimal oldValue = this.Montant;
				if (oldValue != value || !this.IsFieldDefined("[63053]"))
				{
					this.OnMontantChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[63053]", oldValue, value);
					this.OnMontantChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Monnaie</c> field.
		///	designer:fld/63071/63063
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63063]")]
		public global::Demo.Demo5juin.Entities.MonnaieEntity Monnaie
		{
			get
			{
				return this.GetField<global::Demo.Demo5juin.Entities.MonnaieEntity> ("[63063]");
			}
			set
			{
				global::Demo.Demo5juin.Entities.MonnaieEntity oldValue = this.Monnaie;
				if (oldValue != value || !this.IsFieldDefined("[63063]"))
				{
					this.OnMonnaieChanging (oldValue, value);
					this.SetField<global::Demo.Demo5juin.Entities.MonnaieEntity> ("[63063]", oldValue, value);
					this.OnMonnaieChanged (oldValue, value);
				}
			}
		}
		
		partial void OnMontantChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnMontantChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnMonnaieChanging(global::Demo.Demo5juin.Entities.MonnaieEntity oldValue, global::Demo.Demo5juin.Entities.MonnaieEntity newValue);
		partial void OnMonnaieChanged(global::Demo.Demo5juin.Entities.MonnaieEntity oldValue, global::Demo.Demo5juin.Entities.MonnaieEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.PrixSimpleEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.PrixSimpleEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 0, 39);	// [63071]
		public static readonly string EntityStructuredTypeKey = "[63071]";
	}
}
#endregion

#region Demo.Demo5juin.Adresse Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>Adresse</c> entity.
	///	designer:cap/63081
	///	</summary>
	public partial class AdresseEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Désignation</c> field.
		///	designer:fld/63081/63073
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63073]")]
		public string Désignation
		{
			get
			{
				return this.GetField<string> ("[63073]");
			}
			set
			{
				string oldValue = this.Désignation;
				if (oldValue != value || !this.IsFieldDefined("[63073]"))
				{
					this.OnDésignationChanging (oldValue, value);
					this.SetField<string> ("[63073]", oldValue, value);
					this.OnDésignationChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Rue</c> field.
		///	designer:fld/63081/63083
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63083]")]
		public string Rue
		{
			get
			{
				return this.GetField<string> ("[63083]");
			}
			set
			{
				string oldValue = this.Rue;
				if (oldValue != value || !this.IsFieldDefined("[63083]"))
				{
					this.OnRueChanging (oldValue, value);
					this.SetField<string> ("[63083]", oldValue, value);
					this.OnRueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Numéro</c> field.
		///	designer:fld/63081/63093
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[63093]")]
		public string Numéro
		{
			get
			{
				return this.GetField<string> ("[63093]");
			}
			set
			{
				string oldValue = this.Numéro;
				if (oldValue != value || !this.IsFieldDefined("[63093]"))
				{
					this.OnNuméroChanging (oldValue, value);
					this.SetField<string> ("[63093]", oldValue, value);
					this.OnNuméroChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Case</c> field.
		///	designer:fld/63081/630A3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630A3]")]
		public string Case
		{
			get
			{
				return this.GetField<string> ("[630A3]");
			}
			set
			{
				string oldValue = this.Case;
				if (oldValue != value || !this.IsFieldDefined("[630A3]"))
				{
					this.OnCaseChanging (oldValue, value);
					this.SetField<string> ("[630A3]", oldValue, value);
					this.OnCaseChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Ville</c> field.
		///	designer:fld/63081/630B3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630B3]")]
		public string Ville
		{
			get
			{
				return this.GetField<string> ("[630B3]");
			}
			set
			{
				string oldValue = this.Ville;
				if (oldValue != value || !this.IsFieldDefined("[630B3]"))
				{
					this.OnVilleChanging (oldValue, value);
					this.SetField<string> ("[630B3]", oldValue, value);
					this.OnVilleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Npa</c> field.
		///	designer:fld/63081/630C3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630C3]")]
		public string Npa
		{
			get
			{
				return this.GetField<string> ("[630C3]");
			}
			set
			{
				string oldValue = this.Npa;
				if (oldValue != value || !this.IsFieldDefined("[630C3]"))
				{
					this.OnNpaChanging (oldValue, value);
					this.SetField<string> ("[630C3]", oldValue, value);
					this.OnNpaChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Etat</c> field.
		///	designer:fld/63081/630D3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630D3]")]
		public string Etat
		{
			get
			{
				return this.GetField<string> ("[630D3]");
			}
			set
			{
				string oldValue = this.Etat;
				if (oldValue != value || !this.IsFieldDefined("[630D3]"))
				{
					this.OnEtatChanging (oldValue, value);
					this.SetField<string> ("[630D3]", oldValue, value);
					this.OnEtatChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Pays</c> field.
		///	designer:fld/63081/630E3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[630E3]")]
		public string Pays
		{
			get
			{
				return this.GetField<string> ("[630E3]");
			}
			set
			{
				string oldValue = this.Pays;
				if (oldValue != value || !this.IsFieldDefined("[630E3]"))
				{
					this.OnPaysChanging (oldValue, value);
					this.SetField<string> ("[630E3]", oldValue, value);
					this.OnPaysChanged (oldValue, value);
				}
			}
		}
		
		partial void OnDésignationChanging(string oldValue, string newValue);
		partial void OnDésignationChanged(string oldValue, string newValue);
		partial void OnRueChanging(string oldValue, string newValue);
		partial void OnRueChanged(string oldValue, string newValue);
		partial void OnNuméroChanging(string oldValue, string newValue);
		partial void OnNuméroChanged(string oldValue, string newValue);
		partial void OnCaseChanging(string oldValue, string newValue);
		partial void OnCaseChanged(string oldValue, string newValue);
		partial void OnVilleChanging(string oldValue, string newValue);
		partial void OnVilleChanged(string oldValue, string newValue);
		partial void OnNpaChanging(string oldValue, string newValue);
		partial void OnNpaChanged(string oldValue, string newValue);
		partial void OnEtatChanging(string oldValue, string newValue);
		partial void OnEtatChanged(string oldValue, string newValue);
		partial void OnPaysChanging(string oldValue, string newValue);
		partial void OnPaysChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.AdresseEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.AdresseEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 0, 40);	// [63081]
		public static readonly string EntityStructuredTypeKey = "[63081]";
	}
}
#endregion

#region Demo.Demo5juin.ArticleVisserie Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>ArticleVisserie</c> entity.
	///	designer:cap/631
	///	</summary>
	public partial class ArticleVisserieEntity : global::Demo.Demo5juin.Entities.ArticleEntity
	{
		///	<summary>
		///	The <c>Longueur</c> field.
		///	designer:fld/631/6311
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[6311]")]
		public int Longueur
		{
			get
			{
				return this.GetField<int> ("[6311]");
			}
			set
			{
				int oldValue = this.Longueur;
				if (oldValue != value || !this.IsFieldDefined("[6311]"))
				{
					this.OnLongueurChanging (oldValue, value);
					this.SetField<int> ("[6311]", oldValue, value);
					this.OnLongueurChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Dimension</c> field.
		///	designer:fld/631/6312
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[6312]")]
		public string Dimension
		{
			get
			{
				return this.GetField<string> ("[6312]");
			}
			set
			{
				string oldValue = this.Dimension;
				if (oldValue != value || !this.IsFieldDefined("[6312]"))
				{
					this.OnDimensionChanging (oldValue, value);
					this.SetField<string> ("[6312]", oldValue, value);
					this.OnDimensionChanged (oldValue, value);
				}
			}
		}
		
		partial void OnLongueurChanging(int oldValue, int newValue);
		partial void OnLongueurChanged(int oldValue, int newValue);
		partial void OnDimensionChanging(string oldValue, string newValue);
		partial void OnDimensionChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.ArticleVisserieEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.ArticleVisserieEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 1, 0);	// [631]
		public static readonly new string EntityStructuredTypeKey = "[631]";
	}
}
#endregion

#region Demo.Demo5juin.AdressePlus Entity
namespace Demo.Demo5juin.Entities
{
	///	<summary>
	///	The <c>AdressePlus</c> entity.
	///	designer:cap/6321
	///	</summary>
	public partial class AdressePlusEntity : global::Demo.Demo5juin.Entities.AdresseEntity
	{
		///	<summary>
		///	The <c>Téléphone</c> field.
		///	designer:fld/6321/6322
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[6322]")]
		public string Téléphone
		{
			get
			{
				return this.GetField<string> ("[6322]");
			}
			set
			{
				string oldValue = this.Téléphone;
				if (oldValue != value || !this.IsFieldDefined("[6322]"))
				{
					this.OnTéléphoneChanging (oldValue, value);
					this.SetField<string> ("[6322]", oldValue, value);
					this.OnTéléphoneChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTéléphoneChanging(string oldValue, string newValue);
		partial void OnTéléphoneChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Demo.Demo5juin.Entities.AdressePlusEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Demo.Demo5juin.Entities.AdressePlusEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (102, 2, 1);	// [6321]
		public static readonly new string EntityStructuredTypeKey = "[6321]";
	}
}
#endregion

