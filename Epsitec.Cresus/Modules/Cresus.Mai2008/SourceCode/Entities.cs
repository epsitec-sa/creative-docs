//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[9VA]", typeof (Epsitec.Cresus.Mai2008.Entities.FactureEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[9VAF]", typeof (Epsitec.Cresus.Mai2008.Entities.ArticleEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[9VAG]", typeof (Epsitec.Cresus.Mai2008.Entities.LigneFactureEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[9VAU]", typeof (Epsitec.Cresus.Mai2008.Entities.ClientEntity))]
#region Epsitec.Cresus.Mai2008.Facture Entity
namespace Epsitec.Cresus.Mai2008.Entities
{
	///	<summary>
	///	The <c>Facture</c> entity.
	///	designer:cap/9VA
	///	</summary>
	public partial class FactureEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Objet</c> field.
		///	designer:fld/9VA/9VA2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VA2]")]
		public global::Epsitec.Common.Types.FormattedText Objet
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[9VA2]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Objet;
				if (oldValue != value)
				{
					this.OnObjetChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[9VA2]", oldValue, value);
					this.OnObjetChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AdresseFacturation</c> field.
		///	designer:fld/9VA/9VA1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VA1]")]
		public global::Epsitec.Cresus.Mai2008.Entities.ClientEntity AdresseFacturation
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Mai2008.Entities.ClientEntity> ("[9VA1]");
			}
			set
			{
				global::Epsitec.Cresus.Mai2008.Entities.ClientEntity oldValue = this.AdresseFacturation;
				if (oldValue != value)
				{
					this.OnAdresseFacturationChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Mai2008.Entities.ClientEntity> ("[9VA1]", oldValue, value);
					this.OnAdresseFacturationChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Lignes</c> field.
		///	designer:fld/9VA/9VAN
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VAN]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Mai2008.Entities.LigneFactureEntity> Lignes
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Mai2008.Entities.LigneFactureEntity> ("[9VAN]");
			}
		}
		
		partial void OnObjetChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnObjetChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnAdresseFacturationChanging(global::Epsitec.Cresus.Mai2008.Entities.ClientEntity oldValue, global::Epsitec.Cresus.Mai2008.Entities.ClientEntity newValue);
		partial void OnAdresseFacturationChanged(global::Epsitec.Cresus.Mai2008.Entities.ClientEntity oldValue, global::Epsitec.Cresus.Mai2008.Entities.ClientEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Mai2008.Entities.FactureEntity.EntityStructuredTypeId;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1001, 10, 0);	// [9VA]
	}
}
#endregion

#region Epsitec.Cresus.Mai2008.Article Entity
namespace Epsitec.Cresus.Mai2008.Entities
{
	///	<summary>
	///	The <c>Article</c> entity.
	///	designer:cap/9VAF
	///	</summary>
	public partial class ArticleEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Numéro</c> field.
		///	designer:fld/9VAF/9VAK
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VAK]")]
		public string Numéro
		{
			get
			{
				return this.GetField<string> ("[9VAK]");
			}
			set
			{
				string oldValue = this.Numéro;
				if (oldValue != value)
				{
					this.OnNuméroChanging (oldValue, value);
					this.SetField<string> ("[9VAK]", oldValue, value);
					this.OnNuméroChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Désignation</c> field.
		///	designer:fld/9VAF/9VAL
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VAL]")]
		public string Désignation
		{
			get
			{
				return this.GetField<string> ("[9VAL]");
			}
			set
			{
				string oldValue = this.Désignation;
				if (oldValue != value)
				{
					this.OnDésignationChanging (oldValue, value);
					this.SetField<string> ("[9VAL]", oldValue, value);
					this.OnDésignationChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Prix</c> field.
		///	designer:fld/9VAF/9VAM
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VAM]")]
		public global::System.Decimal Prix
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[9VAM]");
			}
			set
			{
				global::System.Decimal oldValue = this.Prix;
				if (oldValue != value)
				{
					this.OnPrixChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[9VAM]", oldValue, value);
					this.OnPrixChanged (oldValue, value);
				}
			}
		}
		
		partial void OnNuméroChanging(string oldValue, string newValue);
		partial void OnNuméroChanged(string oldValue, string newValue);
		partial void OnDésignationChanging(string oldValue, string newValue);
		partial void OnDésignationChanged(string oldValue, string newValue);
		partial void OnPrixChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnPrixChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Mai2008.Entities.ArticleEntity.EntityStructuredTypeId;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1001, 10, 15);	// [9VAF]
	}
}
#endregion

#region Epsitec.Cresus.Mai2008.LigneFacture Entity
namespace Epsitec.Cresus.Mai2008.Entities
{
	///	<summary>
	///	The <c>LigneFacture</c> entity.
	///	designer:cap/9VAG
	///	</summary>
	public partial class LigneFactureEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Quantité</c> field.
		///	designer:fld/9VAG/9VAI
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VAI]")]
		public global::System.Decimal Quantité
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[9VAI]");
			}
			set
			{
				global::System.Decimal oldValue = this.Quantité;
				if (oldValue != value)
				{
					this.OnQuantitéChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[9VAI]", oldValue, value);
					this.OnQuantitéChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Article</c> field.
		///	designer:fld/9VAG/9VAH
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VAH]")]
		public global::Epsitec.Cresus.Mai2008.Entities.ArticleEntity Article
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Mai2008.Entities.ArticleEntity> ("[9VAH]");
			}
			set
			{
				global::Epsitec.Cresus.Mai2008.Entities.ArticleEntity oldValue = this.Article;
				if (oldValue != value)
				{
					this.OnArticleChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Mai2008.Entities.ArticleEntity> ("[9VAH]", oldValue, value);
					this.OnArticleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Prix</c> field.
		///	designer:fld/9VAG/9VAJ
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VAJ]")]
		public virtual global::System.Decimal Prix
		{
			get
			{
				return global::Epsitec.Common.Support.EntityEngine.AbstractEntity.GetCalculation<global::Epsitec.Cresus.Mai2008.Entities.LigneFactureEntity, global::System.Decimal> (this, "[9VAJ]", global::Epsitec.Cresus.Mai2008.Entities.LigneFactureEntity.FuncPrix, global::Epsitec.Cresus.Mai2008.Entities.LigneFactureEntity.ExprPrix);
			}
			set
			{
				global::Epsitec.Common.Support.EntityEngine.AbstractEntity.SetCalculation<global::Epsitec.Cresus.Mai2008.Entities.LigneFactureEntity, global::System.Decimal> (this, "[9VAJ]", value);
			}
		}
		private static readonly global::System.Func<global::Epsitec.Cresus.Mai2008.Entities.LigneFactureEntity, global::System.Decimal> FuncPrix = ligne => ligne.Quantité * (ligne.Article == null ? 0 : ligne.Article.Prix); // λ [9VAG] [9VAJ]
		private static readonly global::System.Linq.Expressions.Expression<global::System.Func<global::Epsitec.Cresus.Mai2008.Entities.LigneFactureEntity, global::System.Decimal>> ExprPrix = ligne => ligne.Quantité * (ligne.Article == null ? 0 : ligne.Article.Prix); // λ [9VAG] [9VAJ]
		
		partial void OnQuantitéChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnQuantitéChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnArticleChanging(global::Epsitec.Cresus.Mai2008.Entities.ArticleEntity oldValue, global::Epsitec.Cresus.Mai2008.Entities.ArticleEntity newValue);
		partial void OnArticleChanged(global::Epsitec.Cresus.Mai2008.Entities.ArticleEntity oldValue, global::Epsitec.Cresus.Mai2008.Entities.ArticleEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Mai2008.Entities.LigneFactureEntity.EntityStructuredTypeId;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1001, 10, 16);	// [9VAG]
	}
}
#endregion

#region Epsitec.Cresus.Mai2008.Client Entity
namespace Epsitec.Cresus.Mai2008.Entities
{
	///	<summary>
	///	The <c>Client</c> entity.
	///	designer:cap/9VAU
	///	</summary>
	public partial class ClientEntity : global::Epsitec.Cresus.AddressBook.Entities.AdressePersonneEntity
	{
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Mai2008.Entities.ClientEntity.EntityStructuredTypeId;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1001, 10, 30);	// [9VAU]
	}
}
#endregion

