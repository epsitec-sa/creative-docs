//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[OVK]", typeof (Epsitec.Cresus.Compta.Entities.ComptabilitéEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[OVK3]", typeof (Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[OVKB]", typeof (Epsitec.Cresus.Compta.Entities.ComptabilitéEcritureEntity))]
#region Epsitec.Cresus.Compta.Comptabilité Entity
namespace Epsitec.Cresus.Compta.Entities
{
	///	<summary>
	///	The <c>Comptabilité</c> entity.
	///	designer:cap/OVK
	///	</summary>
	public partial class ComptabilitéEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IDateRange, global::Epsitec.Cresus.Core.Entities.INameDescription
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
		#endregion
		#region IDateRange Members
		///	<summary>
		///	The <c>BeginDate</c> field.
		///	designer:fld/OVK/8VAO
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAO]")]
		public global::Epsitec.Common.Types.Date? BeginDate
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IDateRangeInterfaceImplementation.GetBeginDate (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IDateRangeInterfaceImplementation.SetBeginDate (this, value);
			}
		}
		#endregion
		#region INameDescription Members
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
		#region IDateRange Members
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/OVK/8VAP
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAP]")]
		public global::Epsitec.Common.Types.Date? EndDate
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IDateRangeInterfaceImplementation.GetEndDate (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IDateRangeInterfaceImplementation.SetEndDate (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>PlanComptable</c> field.
		///	designer:fld/OVK/OVKN
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKN]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity> PlanComptable
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity> ("[OVKN]");
			}
		}
		///	<summary>
		///	The <c>Journal</c> field.
		///	designer:fld/OVK/OVKO
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKO]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Compta.Entities.ComptabilitéEcritureEntity> Journal
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Compta.Entities.ComptabilitéEcritureEntity> ("[OVKO]");
			}
		}
		///	<summary>
		///	The <c>DernièreDate</c> field.
		///	designer:fld/OVK/OVK1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK1]")]
		public global::Epsitec.Common.Types.Date? DernièreDate
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[OVK1]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.DernièreDate;
				if (oldValue != value || !this.IsFieldDefined("[OVK1]"))
				{
					this.OnDernièreDateChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[OVK1]", oldValue, value);
					this.OnDernièreDateChanged (oldValue, value);
				}
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
		
		partial void OnDernièreDateChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnDernièreDateChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnDernièrePièceChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnDernièrePièceChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptabilitéEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptabilitéEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1016, 20, 0);	// [OVK]
		public static readonly string EntityStructuredTypeKey = "[OVK]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ComptabilitéEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Compta.ComptabilitéCompte Entity
namespace Epsitec.Cresus.Compta.Entities
{
	///	<summary>
	///	The <c>ComptabilitéCompte</c> entity.
	///	designer:cap/OVK3
	///	</summary>
	public partial class ComptabilitéCompteEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime
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
		///	The <c>Groupe</c> field.
		///	designer:fld/OVK3/OVK6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK6]")]
		public global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity Groupe
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity> ("[OVK6]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity oldValue = this.Groupe;
				if (oldValue != value || !this.IsFieldDefined("[OVK6]"))
				{
					this.OnGroupeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity> ("[OVK6]", oldValue, value);
					this.OnGroupeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CompteOuvBoucl</c> field.
		///	designer:fld/OVK3/OVK7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVK7]")]
		public global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity CompteOuvBoucl
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity> ("[OVK7]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity oldValue = this.CompteOuvBoucl;
				if (oldValue != value || !this.IsFieldDefined("[OVK7]"))
				{
					this.OnCompteOuvBouclChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity> ("[OVK7]", oldValue, value);
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
		partial void OnGroupeChanging(global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity newValue);
		partial void OnGroupeChanged(global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity newValue);
		partial void OnCompteOuvBouclChanging(global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity newValue);
		partial void OnCompteOuvBouclChanged(global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity newValue);
		partial void OnIndexOuvBouclChanging(int oldValue, int newValue);
		partial void OnIndexOuvBouclChanged(int oldValue, int newValue);
		partial void OnMonnaieChanging(string oldValue, string newValue);
		partial void OnMonnaieChanged(string oldValue, string newValue);
		partial void OnNiveauChanging(int oldValue, int newValue);
		partial void OnNiveauChanged(int oldValue, int newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1016, 20, 3);	// [OVK3]
		public static readonly string EntityStructuredTypeKey = "[OVK3]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ComptabilitéCompteEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Compta.ComptabilitéEcriture Entity
namespace Epsitec.Cresus.Compta.Entities
{
	///	<summary>
	///	The <c>ComptabilitéEcriture</c> entity.
	///	designer:cap/OVKB
	///	</summary>
	public partial class ComptabilitéEcritureEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime
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
		///	The <c>IndexMulti</c> field.
		///	designer:fld/OVKB/OVKD
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKD]")]
		public int IndexMulti
		{
			get
			{
				return this.GetField<int> ("[OVKD]");
			}
			set
			{
				int oldValue = this.IndexMulti;
				if (oldValue != value || !this.IsFieldDefined("[OVKD]"))
				{
					this.OnIndexMultiChanging (oldValue, value);
					this.SetField<int> ("[OVKD]", oldValue, value);
					this.OnIndexMultiChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Débit</c> field.
		///	designer:fld/OVKB/OVKE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKE]")]
		public global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity Débit
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity> ("[OVKE]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity oldValue = this.Débit;
				if (oldValue != value || !this.IsFieldDefined("[OVKE]"))
				{
					this.OnDébitChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity> ("[OVKE]", oldValue, value);
					this.OnDébitChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Crédit</c> field.
		///	designer:fld/OVKB/OVKF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[OVKF]")]
		public global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity Crédit
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity> ("[OVKF]");
			}
			set
			{
				global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity oldValue = this.Crédit;
				if (oldValue != value || !this.IsFieldDefined("[OVKF]"))
				{
					this.OnCréditChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity> ("[OVKF]", oldValue, value);
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
		
		partial void OnDateChanging(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDateChanged(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnIndexMultiChanging(int oldValue, int newValue);
		partial void OnIndexMultiChanged(int oldValue, int newValue);
		partial void OnDébitChanging(global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity newValue);
		partial void OnDébitChanged(global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity newValue);
		partial void OnCréditChanging(global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity newValue);
		partial void OnCréditChanged(global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity oldValue, global::Epsitec.Cresus.Compta.Entities.ComptabilitéCompteEntity newValue);
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
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptabilitéEcritureEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Compta.Entities.ComptabilitéEcritureEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1016, 20, 11);	// [OVKB]
		public static readonly string EntityStructuredTypeKey = "[OVKB]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ComptabilitéEcritureEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

