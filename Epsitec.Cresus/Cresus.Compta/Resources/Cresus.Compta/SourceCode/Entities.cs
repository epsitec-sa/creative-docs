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
#region Epsitec.Cresus.Compta.Compta Entity
namespace Epsitec.Cresus.Compta.Entities
{
	///	<summary>
	///	The <c>Compta</c> entity.
	///	designer:cap/OVK
	///	</summary>
	public partial class ComptaEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.INameDescription
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/OVK/8VA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA3]")]
		public bool IsArchive
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.ILifetimeInterfaceImplementation.GetIsArchive (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ILifetimeInterfaceImplementation.SetIsArchive (this, value);
			}
		}
		#endregion
		#region INameDescription Members
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/OVK/8VA7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA7]")]
		public global::Epsitec.Common.Types.FormattedText Name
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.GetName (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.SetName (this, value);
			}
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/OVK/8VA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA8]")]
		public global::Epsitec.Common.Types.FormattedText Description
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.GetDescription (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.SetDescription (this, value);
			}
		}
		#endregion
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
		///	The <c>DernièrePièce</c> field.
		///	designer:fld/OVK/OVK2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK2]")]
		public global::Epsitec.Common.Types.FormattedText DernièrePièce
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[OVK2]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.DernièrePièce;
				if (oldValue != value || !this.IsFieldDefined("[OVK2]"))
				{
					this.OnDernièrePièceChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[OVK2]", oldValue, value);
					this.OnDernièrePièceChanged (oldValue, value);
				}
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
		
		partial void OnDernièrePièceChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnDernièrePièceChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		
		
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
	public partial class ComptaCompteEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/OVK3/8VA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA3]")]
		public bool IsArchive
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.ILifetimeInterfaceImplementation.GetIsArchive (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ILifetimeInterfaceImplementation.SetIsArchive (this, value);
			}
		}
		#endregion
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
		///	The <c>BudgetPrécédent</c> field.
		///	designer:fld/OVK3/OVK71
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK71]")]
		public global::System.Decimal? BudgetPrécédent
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[OVK71]");
			}
			set
			{
				global::System.Decimal? oldValue = this.BudgetPrécédent;
				if (oldValue != value || !this.IsFieldDefined("[OVK71]"))
				{
					this.OnBudgetPrécédentChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[OVK71]", oldValue, value);
					this.OnBudgetPrécédentChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Budget</c> field.
		///	designer:fld/OVK3/OVK81
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK81]")]
		public global::System.Decimal? Budget
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[OVK81]");
			}
			set
			{
				global::System.Decimal? oldValue = this.Budget;
				if (oldValue != value || !this.IsFieldDefined("[OVK81]"))
				{
					this.OnBudgetChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[OVK81]", oldValue, value);
					this.OnBudgetChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>BudgetFutur</c> field.
		///	designer:fld/OVK3/OVK91
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK91]")]
		public global::System.Decimal? BudgetFutur
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[OVK91]");
			}
			set
			{
				global::System.Decimal? oldValue = this.BudgetFutur;
				if (oldValue != value || !this.IsFieldDefined("[OVK91]"))
				{
					this.OnBudgetFuturChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[OVK91]", oldValue, value);
					this.OnBudgetFuturChanged (oldValue, value);
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
		partial void OnMonnaieChanging(string oldValue, string newValue);
		partial void OnMonnaieChanged(string oldValue, string newValue);
		partial void OnBudgetPrécédentChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnBudgetPrécédentChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnBudgetChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnBudgetChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnBudgetFuturChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnBudgetFuturChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
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
	public partial class ComptaEcritureEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/OVKB/8VA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA3]")]
		public bool IsArchive
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.ILifetimeInterfaceImplementation.GetIsArchive (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ILifetimeInterfaceImplementation.SetIsArchive (this, value);
			}
		}
		#endregion
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
		///	The <c>Montant</c> field.
		///	designer:fld/OVKB/OVKI
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKI]")]
		public global::System.Decimal Montant
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[OVKI]");
			}
			set
			{
				global::System.Decimal oldValue = this.Montant;
				if (oldValue != value || !this.IsFieldDefined("[OVKI]"))
				{
					this.OnMontantChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[OVKI]", oldValue, value);
					this.OnMontantChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TypeTVA</c> field.
		///	designer:fld/OVKB/OVKJ
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKJ]")]
		public string TypeTVA
		{
			get
			{
				return this.GetField<string> ("[OVKJ]");
			}
			set
			{
				string oldValue = this.TypeTVA;
				if (oldValue != value || !this.IsFieldDefined("[OVKJ]"))
				{
					this.OnTypeTVAChanging (oldValue, value);
					this.SetField<string> ("[OVKJ]", oldValue, value);
					this.OnTypeTVAChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>NuméroTVA</c> field.
		///	designer:fld/OVKB/OVKK
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKK]")]
		public string NuméroTVA
		{
			get
			{
				return this.GetField<string> ("[OVKK]");
			}
			set
			{
				string oldValue = this.NuméroTVA;
				if (oldValue != value || !this.IsFieldDefined("[OVKK]"))
				{
					this.OnNuméroTVAChanging (oldValue, value);
					this.SetField<string> ("[OVKK]", oldValue, value);
					this.OnNuméroTVAChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CodeTVA</c> field.
		///	designer:fld/OVKB/OVKL
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKL]")]
		public string CodeTVA
		{
			get
			{
				return this.GetField<string> ("[OVKL]");
			}
			set
			{
				string oldValue = this.CodeTVA;
				if (oldValue != value || !this.IsFieldDefined("[OVKL]"))
				{
					this.OnCodeTVAChanging (oldValue, value);
					this.SetField<string> ("[OVKL]", oldValue, value);
					this.OnCodeTVAChanged (oldValue, value);
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
		partial void OnMontantChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnMontantChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnTypeTVAChanging(string oldValue, string newValue);
		partial void OnTypeTVAChanged(string oldValue, string newValue);
		partial void OnNuméroTVAChanging(string oldValue, string newValue);
		partial void OnNuméroTVAChanged(string oldValue, string newValue);
		partial void OnCodeTVAChanging(string oldValue, string newValue);
		partial void OnCodeTVAChanged(string oldValue, string newValue);
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
	public partial class ComptaJournalEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.INameDescription
	{
		#region INameDescription Members
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/OVK23/8VA7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA7]")]
		public global::Epsitec.Common.Types.FormattedText Name
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.GetName (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.SetName (this, value);
			}
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/OVK23/8VA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA8]")]
		public global::Epsitec.Common.Types.FormattedText Description
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.GetDescription (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.INameDescriptionInterfaceImplementation.SetDescription (this, value);
			}
		}
		#endregion
		
		
		
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
	public partial class ComptaPériodeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/OVKC3/8VA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA3]")]
		public bool IsArchive
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.ILifetimeInterfaceImplementation.GetIsArchive (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ILifetimeInterfaceImplementation.SetIsArchive (this, value);
			}
		}
		#endregion
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
		
		partial void OnDateDébutChanging(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDateDébutChanged(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDateFinChanging(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDateFinChanged(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDescriptionChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnDescriptionChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnDernièreDateChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnDernièreDateChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		
		
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
		///	The <c>Permanant</c> field.
		///	designer:fld/OVKN3/OVKP3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKP3]")]
		public bool Permanant
		{
			get
			{
				return this.GetField<bool> ("[OVKP3]");
			}
			set
			{
				bool oldValue = this.Permanant;
				if (oldValue != value || !this.IsFieldDefined("[OVKP3]"))
				{
					this.OnPermanantChanging (oldValue, value);
					this.SetField<bool> ("[OVKP3]", oldValue, value);
					this.OnPermanantChanged (oldValue, value);
				}
			}
		}
		
		partial void OnLibelléChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnLibelléChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnPermanantChanging(bool oldValue, bool newValue);
		partial void OnPermanantChanged(bool oldValue, bool newValue);
		
		
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
	public partial class ComptaModèleEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/OVKT3/8VA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA3]")]
		public bool IsArchive
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.ILifetimeInterfaceImplementation.GetIsArchive (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ILifetimeInterfaceImplementation.SetIsArchive (this, value);
			}
		}
		#endregion
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

