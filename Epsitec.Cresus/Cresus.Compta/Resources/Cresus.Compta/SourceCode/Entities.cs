//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[OVK]", typeof (Epsitec.Cresus.Compta.Entities.ComptaEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[OVK3]", typeof (Epsitec.Cresus.Compta.Entities.ComptaCompteEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[OVKB]", typeof (Epsitec.Cresus.Compta.Entities.ComptaEcritureEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[OVK23]", typeof (Epsitec.Cresus.Compta.Entities.ComptaJournalEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[OVKC3]", typeof (Epsitec.Cresus.Compta.Entities.ComptaPériodeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[OVKN3]", typeof (Epsitec.Cresus.Compta.Entities.ComptaLibelléEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[OVKT3]", typeof (Epsitec.Cresus.Compta.Entities.ComptaModèleEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[OVKK4]", typeof (Epsitec.Cresus.Compta.Entities.ComptaBudgetEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[OVK75]", typeof (Epsitec.Cresus.Compta.Entities.ComptaUtilisateurEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[OVKB5]", typeof (Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[OVKD6]", typeof (Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[OVK07]", typeof (Epsitec.Cresus.Compta.Entities.ComptaTauxTVAEntity))]
#region Epsitec.Cresus.Compta.Compta Entity
namespace Epsitec.Cresus.Compta.Entities
{
	///	<summary>
	///	The <c>Compta</c> entity.
	///	designer:cap/OVK
	///	</summary>
	public partial class ComptaEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Nom</c> field.
		///	designer:fld/OVK/OVKI4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKI4]")]
		public global::Epsitec.Common.Types.FormattedText Nom
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVKI4]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Nom;
				if (oldValue != value || !this.IsFieldDefined("[OVKI4]"))
				{
					this.OnNomChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVKI4]", oldValue, value);
					this.OnNomChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/OVK/OVKJ4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKJ4]")]
		public global::Epsitec.Common.Types.FormattedText Description
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVKJ4]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Description;
				if (oldValue != value || !this.IsFieldDefined("[OVKJ4]"))
				{
					this.OnDescriptionChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVKJ4]", oldValue, value);
					this.OnDescriptionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PlanComptable</c> field.
		///	designer:fld/OVK/OVKN
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKN]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity> PlanComptable
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity> ("[OVKN]");
			}
		}
		///	<summary>
		///	The <c>Périodes</c> field.
		///	designer:fld/OVK/OVKF3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKF3]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Compta.Entities.ComptaPériodeEntity> Périodes
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Compta.Entities.ComptaPériodeEntity> ("[OVKF3]");
			}
		}
		///	<summary>
		///	The <c>Journaux</c> field.
		///	designer:fld/OVK/OVKH3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKH3]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Compta.Entities.ComptaJournalEntity> Journaux
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Compta.Entities.ComptaJournalEntity> ("[OVKH3]");
			}
		}
		///	<summary>
		///	The <c>Libellés</c> field.
		///	designer:fld/OVK/OVKQ3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKQ3]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Compta.Entities.ComptaLibelléEntity> Libellés
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Compta.Entities.ComptaLibelléEntity> ("[OVKQ3]");
			}
		}
		///	<summary>
		///	The <c>Modèles</c> field.
		///	designer:fld/OVK/OVK54
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK54]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Compta.Entities.ComptaModèleEntity> Modèles
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Compta.Entities.ComptaModèleEntity> ("[OVK54]");
			}
		}
		///	<summary>
		///	The <c>PiècesGenerator</c> field.
		///	designer:fld/OVK/OVKH5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKH5]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity> PiècesGenerator
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity> ("[OVKH5]");
			}
		}
		///	<summary>
		///	The <c>CodesTVA</c> field.
		///	designer:fld/OVK/OVKN6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKN6]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity> CodesTVA
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity> ("[OVKN6]");
			}
		}
		///	<summary>
		///	The <c>TauxTVA</c> field.
		///	designer:fld/OVK/OVK57
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK57]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Compta.Entities.ComptaTauxTVAEntity> TauxTVA
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Compta.Entities.ComptaTauxTVAEntity> ("[OVK57]");
			}
		}
		///	<summary>
		///	The <c>Utilisateurs</c> field.
		///	designer:fld/OVK/OVKI5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKI5]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Compta.Entities.ComptaUtilisateurEntity> Utilisateurs
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Compta.Entities.ComptaUtilisateurEntity> ("[OVKI5]");
			}
		}
		
		partial void OnNomChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnNomChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnDescriptionChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnDescriptionChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1016, 20, 0);	// [OVK]
		public static readonly string EntityStructuredTypeKey = "[OVK]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ComptaEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Compta.ComptaCompte Entity
namespace Epsitec.Cresus.Compta.Entities
{
	///	<summary>
	///	The <c>ComptaCompte</c> entity.
	///	designer:cap/OVK3
	///	</summary>
	public partial class ComptaCompteEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Numéro</c> field.
		///	designer:fld/OVK3/OVK4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK4]")]
		public global::Epsitec.Common.Types.FormattedText Numéro
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVK4]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Numéro;
				if (oldValue != value || !this.IsFieldDefined("[OVK4]"))
				{
					this.OnNuméroChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVK4]", oldValue, value);
					this.OnNuméroChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Titre</c> field.
		///	designer:fld/OVK3/OVK5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK5]")]
		public global::Epsitec.Common.Types.FormattedText Titre
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVK5]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Titre;
				if (oldValue != value || !this.IsFieldDefined("[OVK5]"))
				{
					this.OnTitreChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVK5]", oldValue, value);
					this.OnTitreChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Catégorie</c> field.
		///	designer:fld/OVK3/OVKI2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKI2]")]
		public global::Epsitec.Cresus.Compta.CatégorieDeCompte Catégorie
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.CatégorieDeCompte> ("[OVKI2]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.CatégorieDeCompte oldValue = this.Catégorie;
				if (oldValue != value || !this.IsFieldDefined("[OVKI2]"))
				{
					this.OnCatégorieChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.CatégorieDeCompte> ("[OVKI2]", oldValue, value);
					this.OnCatégorieChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Type</c> field.
		///	designer:fld/OVK3/OVKJ2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKJ2]")]
		public global::Epsitec.Cresus.Compta.TypeDeCompte Type
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.TypeDeCompte> ("[OVKJ2]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.TypeDeCompte oldValue = this.Type;
				if (oldValue != value || !this.IsFieldDefined("[OVKJ2]"))
				{
					this.OnTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.TypeDeCompte> ("[OVKJ2]", oldValue, value);
					this.OnTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Groupe</c> field.
		///	designer:fld/OVK3/OVK6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK6]")]
		public global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity Groupe
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity> ("[OVK6]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue = this.Groupe;
				if (oldValue != value || !this.IsFieldDefined("[OVK6]"))
				{
					this.OnGroupeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity> ("[OVK6]", oldValue, value);
					this.OnGroupeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CompteOuvBoucl</c> field.
		///	designer:fld/OVK3/OVK7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK7]")]
		public global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity CompteOuvBoucl
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity> ("[OVK7]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue = this.CompteOuvBoucl;
				if (oldValue != value || !this.IsFieldDefined("[OVK7]"))
				{
					this.OnCompteOuvBouclChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity> ("[OVK7]", oldValue, value);
					this.OnCompteOuvBouclChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>IndexOuvBoucl</c> field.
		///	designer:fld/OVK3/OVK8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK8]")]
		public int IndexOuvBoucl
		{
			get
			{
				return this.GetField<int> ("[OVK8]");
			}
			set
			{
				int oldValue = this.IndexOuvBoucl;
				if (oldValue != value || !this.IsFieldDefined("[OVK8]"))
				{
					this.OnIndexOuvBouclChanging (oldValue, value);
					this.SetField<int> ("[OVK8]", oldValue, value);
					this.OnIndexOuvBouclChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CodeTVA</c> field.
		///	designer:fld/OVK3/OVKO6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKO6]")]
		public global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity CodeTVA
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity> ("[OVKO6]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity oldValue = this.CodeTVA;
				if (oldValue != value || !this.IsFieldDefined("[OVKO6]"))
				{
					this.OnCodeTVAChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity> ("[OVKO6]", oldValue, value);
					this.OnCodeTVAChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Monnaie</c> field.
		///	designer:fld/OVK3/OVK9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK9]")]
		public string Monnaie
		{
			get
			{
				return this.GetField<string> ("[OVK9]");
			}
			set
			{
				string oldValue = this.Monnaie;
				if (oldValue != value || !this.IsFieldDefined("[OVK9]"))
				{
					this.OnMonnaieChanging (oldValue, value);
					this.SetField<string> ("[OVK9]", oldValue, value);
					this.OnMonnaieChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Niveau</c> field.
		///	designer:fld/OVK3/OVKA
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKA]")]
		public int Niveau
		{
			get
			{
				return this.GetField<int> ("[OVKA]");
			}
			set
			{
				int oldValue = this.Niveau;
				if (oldValue != value || !this.IsFieldDefined("[OVKA]"))
				{
					this.OnNiveauChanging (oldValue, value);
					this.SetField<int> ("[OVKA]", oldValue, value);
					this.OnNiveauChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Budgets</c> field.
		///	designer:fld/OVK3/OVKQ4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKQ4]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Compta.Entities.ComptaBudgetEntity> Budgets
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Compta.Entities.ComptaBudgetEntity> ("[OVKQ4]");
			}
		}
		
		partial void OnNuméroChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnNuméroChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTitreChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTitreChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnCatégorieChanging(global::Epsitec.Cresus.Compta.CatégorieDeCompte oldValue, global::Epsitec.Cresus.Compta.CatégorieDeCompte newValue);
		partial void OnCatégorieChanged(global::Epsitec.Cresus.Compta.CatégorieDeCompte oldValue, global::Epsitec.Cresus.Compta.CatégorieDeCompte newValue);
		partial void OnTypeChanging(global::Epsitec.Cresus.Compta.TypeDeCompte oldValue, global::Epsitec.Cresus.Compta.TypeDeCompte newValue);
		partial void OnTypeChanged(global::Epsitec.Cresus.Compta.TypeDeCompte oldValue, global::Epsitec.Cresus.Compta.TypeDeCompte newValue);
		partial void OnGroupeChanging(global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity newValue);
		partial void OnGroupeChanged(global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity newValue);
		partial void OnCompteOuvBouclChanging(global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity newValue);
		partial void OnCompteOuvBouclChanged(global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity newValue);
		partial void OnIndexOuvBouclChanging(int oldValue, int newValue);
		partial void OnIndexOuvBouclChanged(int oldValue, int newValue);
		partial void OnCodeTVAChanging(global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity newValue);
		partial void OnCodeTVAChanged(global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity newValue);
		partial void OnMonnaieChanging(string oldValue, string newValue);
		partial void OnMonnaieChanged(string oldValue, string newValue);
		partial void OnNiveauChanging(int oldValue, int newValue);
		partial void OnNiveauChanged(int oldValue, int newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1016, 20, 3);	// [OVK3]
		public static readonly string EntityStructuredTypeKey = "[OVK3]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ComptaCompteEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Compta.ComptaEcriture Entity
namespace Epsitec.Cresus.Compta.Entities
{
	///	<summary>
	///	The <c>ComptaEcriture</c> entity.
	///	designer:cap/OVKB
	///	</summary>
	public partial class ComptaEcritureEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Date</c> field.
		///	designer:fld/OVKB/OVKC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKC]")]
		public global::Epsitec.Common.Types.Date Date
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date> ("[OVKC]");
			}
			set
			{
				global::Epsitec.Common.Types.Date oldValue = this.Date;
				if (oldValue != value || !this.IsFieldDefined("[OVKC]"))
				{
					this.OnDateChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date> ("[OVKC]", oldValue, value);
					this.OnDateChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MultiId</c> field.
		///	designer:fld/OVKB/OVKD
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKD]")]
		public int MultiId
		{
			get
			{
				return this.GetField<int> ("[OVKD]");
			}
			set
			{
				int oldValue = this.MultiId;
				if (oldValue != value || !this.IsFieldDefined("[OVKD]"))
				{
					this.OnMultiIdChanging (oldValue, value);
					this.SetField<int> ("[OVKD]", oldValue, value);
					this.OnMultiIdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TotalAutomatique</c> field.
		///	designer:fld/OVKB/OVKA1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKA1]")]
		public bool TotalAutomatique
		{
			get
			{
				return this.GetField<bool> ("[OVKA1]");
			}
			set
			{
				bool oldValue = this.TotalAutomatique;
				if (oldValue != value || !this.IsFieldDefined("[OVKA1]"))
				{
					this.OnTotalAutomatiqueChanging (oldValue, value);
					this.SetField<bool> ("[OVKA1]", oldValue, value);
					this.OnTotalAutomatiqueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Débit</c> field.
		///	designer:fld/OVKB/OVKE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKE]")]
		public global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity Débit
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity> ("[OVKE]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue = this.Débit;
				if (oldValue != value || !this.IsFieldDefined("[OVKE]"))
				{
					this.OnDébitChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity> ("[OVKE]", oldValue, value);
					this.OnDébitChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Crédit</c> field.
		///	designer:fld/OVKB/OVKF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKF]")]
		public global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity Crédit
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity> ("[OVKF]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue = this.Crédit;
				if (oldValue != value || !this.IsFieldDefined("[OVKF]"))
				{
					this.OnCréditChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity> ("[OVKF]", oldValue, value);
					this.OnCréditChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Pièce</c> field.
		///	designer:fld/OVKB/OVKG
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKG]")]
		public global::Epsitec.Common.Types.FormattedText Pièce
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVKG]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Pièce;
				if (oldValue != value || !this.IsFieldDefined("[OVKG]"))
				{
					this.OnPièceChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVKG]", oldValue, value);
					this.OnPièceChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Libellé</c> field.
		///	designer:fld/OVKB/OVKH
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKH]")]
		public global::Epsitec.Common.Types.FormattedText Libellé
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVKH]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Libellé;
				if (oldValue != value || !this.IsFieldDefined("[OVKH]"))
				{
					this.OnLibelléChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVKH]", oldValue, value);
					this.OnLibelléChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MontantHT</c> field.
		///	designer:fld/OVKB/OVKQ6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKQ6]")]
		public global::System.Decimal? MontantHT
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[OVKQ6]");
			}
			set
			{
				global::System.Decimal? oldValue = this.MontantHT;
				if (oldValue != value || !this.IsFieldDefined("[OVKQ6]"))
				{
					this.OnMontantHTChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[OVKQ6]", oldValue, value);
					this.OnMontantHTChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MontantTVA</c> field.
		///	designer:fld/OVKB/OVKP6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKP6]")]
		public global::System.Decimal? MontantTVA
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[OVKP6]");
			}
			set
			{
				global::System.Decimal? oldValue = this.MontantTVA;
				if (oldValue != value || !this.IsFieldDefined("[OVKP6]"))
				{
					this.OnMontantTVAChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[OVKP6]", oldValue, value);
					this.OnMontantTVAChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MontantTTC</c> field.
		///	designer:fld/OVKB/OVKI
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKI]")]
		public global::System.Decimal MontantTTC
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[OVKI]");
			}
			set
			{
				global::System.Decimal oldValue = this.MontantTTC;
				if (oldValue != value || !this.IsFieldDefined("[OVKI]"))
				{
					this.OnMontantTTCChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[OVKI]", oldValue, value);
					this.OnMontantTTCChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CodeTVA</c> field.
		///	designer:fld/OVKB/OVKL
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKL]")]
		public global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity CodeTVA
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity> ("[OVKL]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity oldValue = this.CodeTVA;
				if (oldValue != value || !this.IsFieldDefined("[OVKL]"))
				{
					this.OnCodeTVAChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity> ("[OVKL]", oldValue, value);
					this.OnCodeTVAChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TauxTVA</c> field.
		///	designer:fld/OVKB/OVKU6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKU6]")]
		public global::System.Decimal? TauxTVA
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[OVKU6]");
			}
			set
			{
				global::System.Decimal? oldValue = this.TauxTVA;
				if (oldValue != value || !this.IsFieldDefined("[OVKU6]"))
				{
					this.OnTauxTVAChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[OVKU6]", oldValue, value);
					this.OnTauxTVAChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CodeAnalytique</c> field.
		///	designer:fld/OVKB/OVKM
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKM]")]
		public string CodeAnalytique
		{
			get
			{
				return this.GetField<string> ("[OVKM]");
			}
			set
			{
				string oldValue = this.CodeAnalytique;
				if (oldValue != value || !this.IsFieldDefined("[OVKM]"))
				{
					this.OnCodeAnalytiqueChanging (oldValue, value);
					this.SetField<string> ("[OVKM]", oldValue, value);
					this.OnCodeAnalytiqueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Journal</c> field.
		///	designer:fld/OVKB/OVK43
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK43]")]
		public global::Epsitec.Cresus.Compta.Entities.ComptaJournalEntity Journal
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.Entities.ComptaJournalEntity> ("[OVK43]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.Entities.ComptaJournalEntity oldValue = this.Journal;
				if (oldValue != value || !this.IsFieldDefined("[OVK43]"))
				{
					this.OnJournalChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.Entities.ComptaJournalEntity> ("[OVK43]", oldValue, value);
					this.OnJournalChanged (oldValue, value);
				}
			}
		}
		
		partial void OnDateChanging(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDateChanged(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnMultiIdChanging(int oldValue, int newValue);
		partial void OnMultiIdChanged(int oldValue, int newValue);
		partial void OnTotalAutomatiqueChanging(bool oldValue, bool newValue);
		partial void OnTotalAutomatiqueChanged(bool oldValue, bool newValue);
		partial void OnDébitChanging(global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity newValue);
		partial void OnDébitChanged(global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity newValue);
		partial void OnCréditChanging(global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity newValue);
		partial void OnCréditChanged(global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity newValue);
		partial void OnPièceChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnPièceChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnLibelléChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnLibelléChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnMontantHTChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnMontantHTChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnMontantTVAChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnMontantTVAChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnMontantTTCChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnMontantTTCChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnCodeTVAChanging(global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity newValue);
		partial void OnCodeTVAChanged(global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity newValue);
		partial void OnTauxTVAChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnTauxTVAChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnCodeAnalytiqueChanging(string oldValue, string newValue);
		partial void OnCodeAnalytiqueChanged(string oldValue, string newValue);
		partial void OnJournalChanging(global::Epsitec.Cresus.Compta.Entities.ComptaJournalEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaJournalEntity newValue);
		partial void OnJournalChanged(global::Epsitec.Cresus.Compta.Entities.ComptaJournalEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaJournalEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaEcritureEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaEcritureEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1016, 20, 11);	// [OVKB]
		public static readonly string EntityStructuredTypeKey = "[OVKB]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ComptaEcritureEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Compta.ComptaJournal Entity
namespace Epsitec.Cresus.Compta.Entities
{
	///	<summary>
	///	The <c>ComptaJournal</c> entity.
	///	designer:cap/OVK23
	///	</summary>
	public partial class ComptaJournalEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Id</c> field.
		///	designer:fld/OVK23/OVK35
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK35]")]
		public int Id
		{
			get
			{
				return this.GetField<int> ("[OVK35]");
			}
			set
			{
				int oldValue = this.Id;
				if (oldValue != value || !this.IsFieldDefined("[OVK35]"))
				{
					this.OnIdChanging (oldValue, value);
					this.SetField<int> ("[OVK35]", oldValue, value);
					this.OnIdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Nom</c> field.
		///	designer:fld/OVK23/OVKG4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKG4]")]
		public global::Epsitec.Common.Types.FormattedText Nom
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVKG4]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Nom;
				if (oldValue != value || !this.IsFieldDefined("[OVKG4]"))
				{
					this.OnNomChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVKG4]", oldValue, value);
					this.OnNomChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/OVK23/OVKH4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKH4]")]
		public global::Epsitec.Common.Types.FormattedText Description
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVKH4]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Description;
				if (oldValue != value || !this.IsFieldDefined("[OVKH4]"))
				{
					this.OnDescriptionChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVKH4]", oldValue, value);
					this.OnDescriptionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PiècesGenerator</c> field.
		///	designer:fld/OVK23/OVKK5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKK5]")]
		public global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity PiècesGenerator
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity> ("[OVKK5]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity oldValue = this.PiècesGenerator;
				if (oldValue != value || !this.IsFieldDefined("[OVKK5]"))
				{
					this.OnPiècesGeneratorChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity> ("[OVKK5]", oldValue, value);
					this.OnPiècesGeneratorChanged (oldValue, value);
				}
			}
		}
		
		partial void OnIdChanging(int oldValue, int newValue);
		partial void OnIdChanged(int oldValue, int newValue);
		partial void OnNomChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnNomChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnDescriptionChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnDescriptionChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnPiècesGeneratorChanging(global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity newValue);
		partial void OnPiècesGeneratorChanged(global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaJournalEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaJournalEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1016, 20, 98);	// [OVK23]
		public static readonly string EntityStructuredTypeKey = "[OVK23]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ComptaJournalEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Compta.ComptaPériode Entity
namespace Epsitec.Cresus.Compta.Entities
{
	///	<summary>
	///	The <c>ComptaPériode</c> entity.
	///	designer:cap/OVKC3
	///	</summary>
	public partial class ComptaPériodeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>DateDébut</c> field.
		///	designer:fld/OVKC3/OVKJ3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKJ3]")]
		public global::Epsitec.Common.Types.Date DateDébut
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date> ("[OVKJ3]");
			}
			set
			{
				global::Epsitec.Common.Types.Date oldValue = this.DateDébut;
				if (oldValue != value || !this.IsFieldDefined("[OVKJ3]"))
				{
					this.OnDateDébutChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date> ("[OVKJ3]", oldValue, value);
					this.OnDateDébutChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DateFin</c> field.
		///	designer:fld/OVKC3/OVKK3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKK3]")]
		public global::Epsitec.Common.Types.Date DateFin
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date> ("[OVKK3]");
			}
			set
			{
				global::Epsitec.Common.Types.Date oldValue = this.DateFin;
				if (oldValue != value || !this.IsFieldDefined("[OVKK3]"))
				{
					this.OnDateFinChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date> ("[OVKK3]", oldValue, value);
					this.OnDateFinChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/OVKC3/OVKL3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKL3]")]
		public global::Epsitec.Common.Types.FormattedText Description
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVKL3]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Description;
				if (oldValue != value || !this.IsFieldDefined("[OVKL3]"))
				{
					this.OnDescriptionChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVKL3]", oldValue, value);
					this.OnDescriptionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Journal</c> field.
		///	designer:fld/OVKC3/OVKD3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKD3]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Compta.Entities.ComptaEcritureEntity> Journal
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Compta.Entities.ComptaEcritureEntity> ("[OVKD3]");
			}
		}
		///	<summary>
		///	The <c>DernièreDate</c> field.
		///	designer:fld/OVKC3/OVKE3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKE3]")]
		public global::Epsitec.Common.Types.Date? DernièreDate
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[OVKE3]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.DernièreDate;
				if (oldValue != value || !this.IsFieldDefined("[OVKE3]"))
				{
					this.OnDernièreDateChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[OVKE3]", oldValue, value);
					this.OnDernièreDateChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PiècesGenerator</c> field.
		///	designer:fld/OVKC3/OVKN5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKN5]")]
		public global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity PiècesGenerator
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity> ("[OVKN5]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity oldValue = this.PiècesGenerator;
				if (oldValue != value || !this.IsFieldDefined("[OVKN5]"))
				{
					this.OnPiècesGeneratorChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity> ("[OVKN5]", oldValue, value);
					this.OnPiècesGeneratorChanged (oldValue, value);
				}
			}
		}
		
		partial void OnDateDébutChanging(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDateDébutChanged(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDateFinChanging(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDateFinChanged(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDescriptionChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnDescriptionChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnDernièreDateChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnDernièreDateChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnPiècesGeneratorChanging(global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity newValue);
		partial void OnPiècesGeneratorChanged(global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaPériodeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaPériodeEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1016, 20, 108);	// [OVKC3]
		public static readonly string EntityStructuredTypeKey = "[OVKC3]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ComptaPériodeEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Compta.ComptaLibellé Entity
namespace Epsitec.Cresus.Compta.Entities
{
	///	<summary>
	///	The <c>ComptaLibellé</c> entity.
	///	designer:cap/OVKN3
	///	</summary>
	public partial class ComptaLibelléEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Libellé</c> field.
		///	designer:fld/OVKN3/OVKO3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKO3]")]
		public global::Epsitec.Common.Types.FormattedText Libellé
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVKO3]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Libellé;
				if (oldValue != value || !this.IsFieldDefined("[OVKO3]"))
				{
					this.OnLibelléChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVKO3]", oldValue, value);
					this.OnLibelléChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Permanent</c> field.
		///	designer:fld/OVKN3/OVKP3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKP3]")]
		public bool Permanent
		{
			get
			{
				return this.GetField<bool> ("[OVKP3]");
			}
			set
			{
				bool oldValue = this.Permanent;
				if (oldValue != value || !this.IsFieldDefined("[OVKP3]"))
				{
					this.OnPermanentChanging (oldValue, value);
					this.SetField<bool> ("[OVKP3]", oldValue, value);
					this.OnPermanentChanged (oldValue, value);
				}
			}
		}
		
		partial void OnLibelléChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnLibelléChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnPermanentChanging(bool oldValue, bool newValue);
		partial void OnPermanentChanged(bool oldValue, bool newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaLibelléEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaLibelléEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1016, 20, 119);	// [OVKN3]
		public static readonly string EntityStructuredTypeKey = "[OVKN3]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ComptaLibelléEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Compta.ComptaModèle Entity
namespace Epsitec.Cresus.Compta.Entities
{
	///	<summary>
	///	The <c>ComptaModèle</c> entity.
	///	designer:cap/OVKT3
	///	</summary>
	public partial class ComptaModèleEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/OVKT3/OVKU3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKU3]")]
		public string Code
		{
			get
			{
				return this.GetField<string> ("[OVKU3]");
			}
			set
			{
				string oldValue = this.Code;
				if (oldValue != value || !this.IsFieldDefined("[OVKU3]"))
				{
					this.OnCodeChanging (oldValue, value);
					this.SetField<string> ("[OVKU3]", oldValue, value);
					this.OnCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Raccourci</c> field.
		///	designer:fld/OVKT3/OVKV3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKV3]")]
		public string Raccourci
		{
			get
			{
				return this.GetField<string> ("[OVKV3]");
			}
			set
			{
				string oldValue = this.Raccourci;
				if (oldValue != value || !this.IsFieldDefined("[OVKV3]"))
				{
					this.OnRaccourciChanging (oldValue, value);
					this.SetField<string> ("[OVKV3]", oldValue, value);
					this.OnRaccourciChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Débit</c> field.
		///	designer:fld/OVKT3/OVK04
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK04]")]
		public global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity Débit
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity> ("[OVK04]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue = this.Débit;
				if (oldValue != value || !this.IsFieldDefined("[OVK04]"))
				{
					this.OnDébitChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity> ("[OVK04]", oldValue, value);
					this.OnDébitChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Crédit</c> field.
		///	designer:fld/OVKT3/OVK14
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK14]")]
		public global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity Crédit
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity> ("[OVK14]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue = this.Crédit;
				if (oldValue != value || !this.IsFieldDefined("[OVK14]"))
				{
					this.OnCréditChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity> ("[OVK14]", oldValue, value);
					this.OnCréditChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Pièce</c> field.
		///	designer:fld/OVKT3/OVK24
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK24]")]
		public global::Epsitec.Common.Types.FormattedText Pièce
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVK24]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Pièce;
				if (oldValue != value || !this.IsFieldDefined("[OVK24]"))
				{
					this.OnPièceChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVK24]", oldValue, value);
					this.OnPièceChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Libellé</c> field.
		///	designer:fld/OVKT3/OVK34
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK34]")]
		public global::Epsitec.Common.Types.FormattedText Libellé
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVK34]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Libellé;
				if (oldValue != value || !this.IsFieldDefined("[OVK34]"))
				{
					this.OnLibelléChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVK34]", oldValue, value);
					this.OnLibelléChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Montant</c> field.
		///	designer:fld/OVKT3/OVK44
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK44]")]
		public global::System.Decimal? Montant
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[OVK44]");
			}
			set
			{
				global::System.Decimal? oldValue = this.Montant;
				if (oldValue != value || !this.IsFieldDefined("[OVK44]"))
				{
					this.OnMontantChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[OVK44]", oldValue, value);
					this.OnMontantChanged (oldValue, value);
				}
			}
		}
		
		partial void OnCodeChanging(string oldValue, string newValue);
		partial void OnCodeChanged(string oldValue, string newValue);
		partial void OnRaccourciChanging(string oldValue, string newValue);
		partial void OnRaccourciChanged(string oldValue, string newValue);
		partial void OnDébitChanging(global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity newValue);
		partial void OnDébitChanged(global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity newValue);
		partial void OnCréditChanging(global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity newValue);
		partial void OnCréditChanged(global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity newValue);
		partial void OnPièceChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnPièceChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnLibelléChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnLibelléChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnMontantChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnMontantChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaModèleEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaModèleEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1016, 20, 125);	// [OVKT3]
		public static readonly string EntityStructuredTypeKey = "[OVKT3]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ComptaModèleEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Compta.ComptaBudget Entity
namespace Epsitec.Cresus.Compta.Entities
{
	///	<summary>
	///	The <c>ComptaBudget</c> entity.
	///	designer:cap/OVKK4
	///	</summary>
	public partial class ComptaBudgetEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Période</c> field.
		///	designer:fld/OVKK4/OVKP4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKP4]")]
		public global::Epsitec.Cresus.Compta.Entities.ComptaPériodeEntity Période
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.Entities.ComptaPériodeEntity> ("[OVKP4]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.Entities.ComptaPériodeEntity oldValue = this.Période;
				if (oldValue != value || !this.IsFieldDefined("[OVKP4]"))
				{
					this.OnPériodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.Entities.ComptaPériodeEntity> ("[OVKP4]", oldValue, value);
					this.OnPériodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Montant</c> field.
		///	designer:fld/OVKK4/OVKM4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKM4]")]
		public global::System.Decimal Montant
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[OVKM4]");
			}
			set
			{
				global::System.Decimal oldValue = this.Montant;
				if (oldValue != value || !this.IsFieldDefined("[OVKM4]"))
				{
					this.OnMontantChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[OVKM4]", oldValue, value);
					this.OnMontantChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/OVKK4/OVKO4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKO4]")]
		public global::Epsitec.Common.Types.FormattedText Description
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVKO4]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Description;
				if (oldValue != value || !this.IsFieldDefined("[OVKO4]"))
				{
					this.OnDescriptionChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVKO4]", oldValue, value);
					this.OnDescriptionChanged (oldValue, value);
				}
			}
		}
		
		partial void OnPériodeChanging(global::Epsitec.Cresus.Compta.Entities.ComptaPériodeEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaPériodeEntity newValue);
		partial void OnPériodeChanged(global::Epsitec.Cresus.Compta.Entities.ComptaPériodeEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaPériodeEntity newValue);
		partial void OnMontantChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnMontantChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnDescriptionChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnDescriptionChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaBudgetEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaBudgetEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1016, 20, 148);	// [OVKK4]
		public static readonly string EntityStructuredTypeKey = "[OVKK4]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ComptaBudgetEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Compta.ComptaUtilisateur Entity
namespace Epsitec.Cresus.Compta.Entities
{
	///	<summary>
	///	The <c>ComptaUtilisateur</c> entity.
	///	designer:cap/OVK75
	///	</summary>
	public partial class ComptaUtilisateurEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Utilisateur</c> field.
		///	designer:fld/OVK75/OVK85
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK85]")]
		public global::Epsitec.Common.Types.FormattedText Utilisateur
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVK85]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Utilisateur;
				if (oldValue != value || !this.IsFieldDefined("[OVK85]"))
				{
					this.OnUtilisateurChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVK85]", oldValue, value);
					this.OnUtilisateurChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>NomComplet</c> field.
		///	designer:fld/OVK75/OVKO5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKO5]")]
		public global::Epsitec.Common.Types.FormattedText NomComplet
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVKO5]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.NomComplet;
				if (oldValue != value || !this.IsFieldDefined("[OVKO5]"))
				{
					this.OnNomCompletChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVKO5]", oldValue, value);
					this.OnNomCompletChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>IdentitéWindows</c> field.
		///	designer:fld/OVK75/OVK26
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK26]")]
		public bool IdentitéWindows
		{
			get
			{
				return this.GetField<bool> ("[OVK26]");
			}
			set
			{
				bool oldValue = this.IdentitéWindows;
				if (oldValue != value || !this.IsFieldDefined("[OVK26]"))
				{
					this.OnIdentitéWindowsChanging (oldValue, value);
					this.SetField<bool> ("[OVK26]", oldValue, value);
					this.OnIdentitéWindowsChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Désactivé</c> field.
		///	designer:fld/OVK75/OVK36
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK36]")]
		public bool Désactivé
		{
			get
			{
				return this.GetField<bool> ("[OVK36]");
			}
			set
			{
				bool oldValue = this.Désactivé;
				if (oldValue != value || !this.IsFieldDefined("[OVK36]"))
				{
					this.OnDésactivéChanging (oldValue, value);
					this.SetField<bool> ("[OVK36]", oldValue, value);
					this.OnDésactivéChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DateDébut</c> field.
		///	designer:fld/OVK75/OVK46
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK46]")]
		public global::Epsitec.Common.Types.Date? DateDébut
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[OVK46]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.DateDébut;
				if (oldValue != value || !this.IsFieldDefined("[OVK46]"))
				{
					this.OnDateDébutChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[OVK46]", oldValue, value);
					this.OnDateDébutChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DateFin</c> field.
		///	designer:fld/OVK75/OVK56
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK56]")]
		public global::Epsitec.Common.Types.Date? DateFin
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[OVK56]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.DateFin;
				if (oldValue != value || !this.IsFieldDefined("[OVK56]"))
				{
					this.OnDateFinChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[OVK56]", oldValue, value);
					this.OnDateFinChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MotDePasse</c> field.
		///	designer:fld/OVK75/OVK95
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK95]")]
		public string MotDePasse
		{
			get
			{
				return this.GetField<string> ("[OVK95]");
			}
			set
			{
				string oldValue = this.MotDePasse;
				if (oldValue != value || !this.IsFieldDefined("[OVK95]"))
				{
					this.OnMotDePasseChanging (oldValue, value);
					this.SetField<string> ("[OVK95]", oldValue, value);
					this.OnMotDePasseChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Admin</c> field.
		///	designer:fld/OVK75/OVKU5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKU5]")]
		public bool Admin
		{
			get
			{
				return this.GetField<bool> ("[OVKU5]");
			}
			set
			{
				bool oldValue = this.Admin;
				if (oldValue != value || !this.IsFieldDefined("[OVKU5]"))
				{
					this.OnAdminChanging (oldValue, value);
					this.SetField<bool> ("[OVKU5]", oldValue, value);
					this.OnAdminChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Présentations</c> field.
		///	designer:fld/OVK75/OVKV5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKV5]")]
		public string Présentations
		{
			get
			{
				return this.GetField<string> ("[OVKV5]");
			}
			set
			{
				string oldValue = this.Présentations;
				if (oldValue != value || !this.IsFieldDefined("[OVKV5]"))
				{
					this.OnPrésentationsChanging (oldValue, value);
					this.SetField<string> ("[OVKV5]", oldValue, value);
					this.OnPrésentationsChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PiècesGenerator</c> field.
		///	designer:fld/OVK75/OVKJ5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKJ5]")]
		public global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity PiècesGenerator
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity> ("[OVKJ5]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity oldValue = this.PiècesGenerator;
				if (oldValue != value || !this.IsFieldDefined("[OVKJ5]"))
				{
					this.OnPiècesGeneratorChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity> ("[OVKJ5]", oldValue, value);
					this.OnPiècesGeneratorChanged (oldValue, value);
				}
			}
		}
		
		partial void OnUtilisateurChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnUtilisateurChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnNomCompletChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnNomCompletChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnIdentitéWindowsChanging(bool oldValue, bool newValue);
		partial void OnIdentitéWindowsChanged(bool oldValue, bool newValue);
		partial void OnDésactivéChanging(bool oldValue, bool newValue);
		partial void OnDésactivéChanged(bool oldValue, bool newValue);
		partial void OnDateDébutChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnDateDébutChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnDateFinChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnDateFinChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnMotDePasseChanging(string oldValue, string newValue);
		partial void OnMotDePasseChanged(string oldValue, string newValue);
		partial void OnAdminChanging(bool oldValue, bool newValue);
		partial void OnAdminChanged(bool oldValue, bool newValue);
		partial void OnPrésentationsChanging(string oldValue, string newValue);
		partial void OnPrésentationsChanged(string oldValue, string newValue);
		partial void OnPiècesGeneratorChanging(global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity newValue);
		partial void OnPiècesGeneratorChanged(global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaUtilisateurEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaUtilisateurEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1016, 20, 167);	// [OVK75]
		public static readonly string EntityStructuredTypeKey = "[OVK75]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ComptaUtilisateurEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Compta.ComptaPiècesGenerator Entity
namespace Epsitec.Cresus.Compta.Entities
{
	///	<summary>
	///	The <c>ComptaPiècesGenerator</c> entity.
	///	designer:cap/OVKB5
	///	</summary>
	public partial class ComptaPiècesGeneratorEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Nom</c> field.
		///	designer:fld/OVKB5/OVKC5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKC5]")]
		public global::Epsitec.Common.Types.FormattedText Nom
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVKC5]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Nom;
				if (oldValue != value || !this.IsFieldDefined("[OVKC5]"))
				{
					this.OnNomChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVKC5]", oldValue, value);
					this.OnNomChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Préfixe</c> field.
		///	designer:fld/OVKB5/OVK86
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK86]")]
		public global::Epsitec.Common.Types.FormattedText Préfixe
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVK86]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Préfixe;
				if (oldValue != value || !this.IsFieldDefined("[OVK86]"))
				{
					this.OnPréfixeChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVK86]", oldValue, value);
					this.OnPréfixeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Suffixe</c> field.
		///	designer:fld/OVKB5/OVK96
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK96]")]
		public global::Epsitec.Common.Types.FormattedText Suffixe
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVK96]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Suffixe;
				if (oldValue != value || !this.IsFieldDefined("[OVK96]"))
				{
					this.OnSuffixeChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVK96]", oldValue, value);
					this.OnSuffixeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Format</c> field.
		///	designer:fld/OVKB5/OVK66
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK66]")]
		public global::Epsitec.Common.Types.FormattedText Format
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVK66]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Format;
				if (oldValue != value || !this.IsFieldDefined("[OVK66]"))
				{
					this.OnFormatChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVK66]", oldValue, value);
					this.OnFormatChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Numéro</c> field.
		///	designer:fld/OVKB5/OVKE5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKE5]")]
		public int Numéro
		{
			get
			{
				return this.GetField<int> ("[OVKE5]");
			}
			set
			{
				int oldValue = this.Numéro;
				if (oldValue != value || !this.IsFieldDefined("[OVKE5]"))
				{
					this.OnNuméroChanging (oldValue, value);
					this.SetField<int> ("[OVKE5]", oldValue, value);
					this.OnNuméroChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Incrément</c> field.
		///	designer:fld/OVKB5/OVKG5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKG5]")]
		public int Incrément
		{
			get
			{
				return this.GetField<int> ("[OVKG5]");
			}
			set
			{
				int oldValue = this.Incrément;
				if (oldValue != value || !this.IsFieldDefined("[OVKG5]"))
				{
					this.OnIncrémentChanging (oldValue, value);
					this.SetField<int> ("[OVKG5]", oldValue, value);
					this.OnIncrémentChanged (oldValue, value);
				}
			}
		}
		
		partial void OnNomChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnNomChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnPréfixeChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnPréfixeChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnSuffixeChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnSuffixeChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnFormatChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnFormatChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnNuméroChanging(int oldValue, int newValue);
		partial void OnNuméroChanged(int oldValue, int newValue);
		partial void OnIncrémentChanging(int oldValue, int newValue);
		partial void OnIncrémentChanged(int oldValue, int newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaPiècesGeneratorEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1016, 20, 171);	// [OVKB5]
		public static readonly string EntityStructuredTypeKey = "[OVKB5]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ComptaPiècesGeneratorEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Compta.ComptaCodeTVA Entity
namespace Epsitec.Cresus.Compta.Entities
{
	///	<summary>
	///	The <c>ComptaCodeTVA</c> entity.
	///	designer:cap/OVKD6
	///	</summary>
	public partial class ComptaCodeTVAEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/OVKD6/OVKE6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKE6]")]
		public global::Epsitec.Common.Types.FormattedText Code
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVKE6]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Code;
				if (oldValue != value || !this.IsFieldDefined("[OVKE6]"))
				{
					this.OnCodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVKE6]", oldValue, value);
					this.OnCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/OVKD6/OVKF6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKF6]")]
		public global::Epsitec.Common.Types.FormattedText Description
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVKF6]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Description;
				if (oldValue != value || !this.IsFieldDefined("[OVKF6]"))
				{
					this.OnDescriptionChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVKF6]", oldValue, value);
					this.OnDescriptionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Taux</c> field.
		///	designer:fld/OVKD6/OVKG6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKG6]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Compta.Entities.ComptaTauxTVAEntity> Taux
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Compta.Entities.ComptaTauxTVAEntity> ("[OVKG6]");
			}
		}
		///	<summary>
		///	The <c>Compte</c> field.
		///	designer:fld/OVKD6/OVKH6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKH6]")]
		public global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity Compte
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity> ("[OVKH6]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue = this.Compte;
				if (oldValue != value || !this.IsFieldDefined("[OVKH6]"))
				{
					this.OnCompteChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity> ("[OVKH6]", oldValue, value);
					this.OnCompteChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Déduction</c> field.
		///	designer:fld/OVKD6/OVKI6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKI6]")]
		public global::System.Decimal Déduction
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[OVKI6]");
			}
			set
			{
				global::System.Decimal oldValue = this.Déduction;
				if (oldValue != value || !this.IsFieldDefined("[OVKI6]"))
				{
					this.OnDéductionChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[OVKI6]", oldValue, value);
					this.OnDéductionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Chiffre</c> field.
		///	designer:fld/OVKD6/OVKK6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKK6]")]
		public int? Chiffre
		{
			get
			{
				return this.GetField<int?> ("[OVKK6]");
			}
			set
			{
				int? oldValue = this.Chiffre;
				if (oldValue != value || !this.IsFieldDefined("[OVKK6]"))
				{
					this.OnChiffreChanging (oldValue, value);
					this.SetField<int?> ("[OVKK6]", oldValue, value);
					this.OnChiffreChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MontantFictif</c> field.
		///	designer:fld/OVKD6/OVKM6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKM6]")]
		public global::System.Decimal? MontantFictif
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[OVKM6]");
			}
			set
			{
				global::System.Decimal? oldValue = this.MontantFictif;
				if (oldValue != value || !this.IsFieldDefined("[OVKM6]"))
				{
					this.OnMontantFictifChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[OVKM6]", oldValue, value);
					this.OnMontantFictifChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ParDéfaut</c> field.
		///	designer:fld/OVKD6/OVKT6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKT6]")]
		public bool ParDéfaut
		{
			get
			{
				return this.GetField<bool> ("[OVKT6]");
			}
			set
			{
				bool oldValue = this.ParDéfaut;
				if (oldValue != value || !this.IsFieldDefined("[OVKT6]"))
				{
					this.OnParDéfautChanging (oldValue, value);
					this.SetField<bool> ("[OVKT6]", oldValue, value);
					this.OnParDéfautChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Désactivé</c> field.
		///	designer:fld/OVKD6/OVKJ6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKJ6]")]
		public bool Désactivé
		{
			get
			{
				return this.GetField<bool> ("[OVKJ6]");
			}
			set
			{
				bool oldValue = this.Désactivé;
				if (oldValue != value || !this.IsFieldDefined("[OVKJ6]"))
				{
					this.OnDésactivéChanging (oldValue, value);
					this.SetField<bool> ("[OVKJ6]", oldValue, value);
					this.OnDésactivéChanged (oldValue, value);
				}
			}
		}
		
		partial void OnCodeChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnCodeChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnDescriptionChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnDescriptionChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnCompteChanging(global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity newValue);
		partial void OnCompteChanged(global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptaCompteEntity newValue);
		partial void OnDéductionChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnDéductionChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnChiffreChanging(int? oldValue, int? newValue);
		partial void OnChiffreChanged(int? oldValue, int? newValue);
		partial void OnMontantFictifChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnMontantFictifChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnParDéfautChanging(bool oldValue, bool newValue);
		partial void OnParDéfautChanged(bool oldValue, bool newValue);
		partial void OnDésactivéChanging(bool oldValue, bool newValue);
		partial void OnDésactivéChanged(bool oldValue, bool newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaCodeTVAEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1016, 20, 205);	// [OVKD6]
		public static readonly string EntityStructuredTypeKey = "[OVKD6]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ComptaCodeTVAEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Compta.ComptaTauxTVA Entity
namespace Epsitec.Cresus.Compta.Entities
{
	///	<summary>
	///	The <c>ComptaTauxTVA</c> entity.
	///	designer:cap/OVK07
	///	</summary>
	public partial class ComptaTauxTVAEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Nom</c> field.
		///	designer:fld/OVK07/OVK17
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK17]")]
		public global::Epsitec.Common.Types.FormattedText Nom
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVK17]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Nom;
				if (oldValue != value || !this.IsFieldDefined("[OVK17]"))
				{
					this.OnNomChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVK17]", oldValue, value);
					this.OnNomChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DateDébut</c> field.
		///	designer:fld/OVK07/OVK27
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK27]")]
		public global::Epsitec.Common.Types.Date? DateDébut
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[OVK27]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.DateDébut;
				if (oldValue != value || !this.IsFieldDefined("[OVK27]"))
				{
					this.OnDateDébutChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[OVK27]", oldValue, value);
					this.OnDateDébutChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DateFin</c> field.
		///	designer:fld/OVK07/OVK37
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK37]")]
		public global::Epsitec.Common.Types.Date? DateFin
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[OVK37]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.DateFin;
				if (oldValue != value || !this.IsFieldDefined("[OVK37]"))
				{
					this.OnDateFinChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[OVK37]", oldValue, value);
					this.OnDateFinChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Taux</c> field.
		///	designer:fld/OVK07/OVK47
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK47]")]
		public global::System.Decimal Taux
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[OVK47]");
			}
			set
			{
				global::System.Decimal oldValue = this.Taux;
				if (oldValue != value || !this.IsFieldDefined("[OVK47]"))
				{
					this.OnTauxChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[OVK47]", oldValue, value);
					this.OnTauxChanged (oldValue, value);
				}
			}
		}
		
		partial void OnNomChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnNomChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnDateDébutChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnDateDébutChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnDateFinChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnDateFinChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnTauxChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnTauxChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaTauxTVAEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptaTauxTVAEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1016, 20, 224);	// [OVK07]
		public static readonly string EntityStructuredTypeKey = "[OVK07]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ComptaTauxTVAEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

