//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[FVA]", typeof (Epsitec.Cresus.Core.Entities.AbstractPersonEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[FVA1]", typeof (Epsitec.Cresus.Core.Entities.AbstractContactEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[FVA4]", typeof (Epsitec.Cresus.Core.Entities.ContactGroupEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[FVA5]", typeof (Epsitec.Cresus.Core.Entities.NaturalPersonEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[FVA6]", typeof (Epsitec.Cresus.Core.Entities.LegalPersonEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[FVAA]", typeof (Epsitec.Cresus.Core.Entities.PersonTitleEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[FVAD]", typeof (Epsitec.Cresus.Core.Entities.PersonGenderEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[FVAL]", typeof (Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[FVAR]", typeof (Epsitec.Cresus.Core.Entities.MailContactEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[FVA21]", typeof (Epsitec.Cresus.Core.Entities.LocationEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[FVA31]", typeof (Epsitec.Cresus.Core.Entities.StateProvinceCountyEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[FVA41]", typeof (Epsitec.Cresus.Core.Entities.CountryEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[FVAK1]", typeof (Epsitec.Cresus.Core.Entities.TelecomTypeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[FVAL1]", typeof (Epsitec.Cresus.Core.Entities.TelecomContactEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[FVAP1]", typeof (Epsitec.Cresus.Core.Entities.UriContactEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[FVAQ1]", typeof (Epsitec.Cresus.Core.Entities.UriTypeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[FVA72]", typeof (Epsitec.Cresus.Core.Entities.PeopleEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[FVA92]", typeof (Epsitec.Cresus.Core.Entities.PeopleGroupEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[FVAI2]", typeof (Epsitec.Cresus.Core.Entities.ContactPersonGroupEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[FVAJ2]", typeof (Epsitec.Cresus.Core.Entities.ContactPersonEntity))]
#region Epsitec.Cresus.Core.AbstractPerson Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>AbstractPerson</c> entity.
	///	designer:cap/FVA
	///	</summary>
	public partial class AbstractPersonEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/FVA/8VA3
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
		///	The <c>Contacts</c> field.
		///	designer:fld/FVA/FVA2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVA2]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.AbstractContactEntity> Contacts
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.AbstractContactEntity> ("[FVA2]");
			}
		}
		///	<summary>
		///	The <c>PreferredLanguage</c> field.
		///	designer:fld/FVA/FVA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVA3]")]
		public global::Epsitec.Cresus.Core.Entities.LanguageEntity PreferredLanguage
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.LanguageEntity> ("[FVA3]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.LanguageEntity oldValue = this.PreferredLanguage;
				if (oldValue != value || !this.IsFieldDefined("[FVA3]"))
				{
					this.OnPreferredLanguageChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.LanguageEntity> ("[FVA3]", oldValue, value);
					this.OnPreferredLanguageChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DisplayName1</c> field.
		///	designer:fld/FVA/FVA23
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVA23]")]
		public string DisplayName1
		{
			get
			{
				return this.GetField<string> ("[FVA23]");
			}
			set
			{
				string oldValue = this.DisplayName1;
				if (oldValue != value || !this.IsFieldDefined("[FVA23]"))
				{
					this.OnDisplayName1Changing (oldValue, value);
					this.SetField<string> ("[FVA23]", oldValue, value);
					this.OnDisplayName1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DisplayName2</c> field.
		///	designer:fld/FVA/FVA33
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVA33]")]
		public string DisplayName2
		{
			get
			{
				return this.GetField<string> ("[FVA33]");
			}
			set
			{
				string oldValue = this.DisplayName2;
				if (oldValue != value || !this.IsFieldDefined("[FVA33]"))
				{
					this.OnDisplayName2Changing (oldValue, value);
					this.SetField<string> ("[FVA33]", oldValue, value);
					this.OnDisplayName2Changed (oldValue, value);
				}
			}
		}
		
		partial void OnPreferredLanguageChanging(global::Epsitec.Cresus.Core.Entities.LanguageEntity oldValue, global::Epsitec.Cresus.Core.Entities.LanguageEntity newValue);
		partial void OnPreferredLanguageChanged(global::Epsitec.Cresus.Core.Entities.LanguageEntity oldValue, global::Epsitec.Cresus.Core.Entities.LanguageEntity newValue);
		partial void OnDisplayName1Changing(string oldValue, string newValue);
		partial void OnDisplayName1Changed(string oldValue, string newValue);
		partial void OnDisplayName2Changing(string oldValue, string newValue);
		partial void OnDisplayName2Changed(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 0);	// [FVA]
		public static readonly string EntityStructuredTypeKey = "[FVA]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AbstractPersonEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Unknown)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.AbstractContact Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>AbstractContact</c> entity.
	///	designer:cap/FVA1
	///	</summary>
	public partial class AbstractContactEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.IComments
	{
		#region IComments Members
		///	<summary>
		///	The <c>Comments</c> field.
		///	designer:fld/FVA1/8VAT
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAT]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.CommentEntity> Comments
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.ICommentsInterfaceImplementation.GetComments (this);
			}
		}
		#endregion
		///	<summary>
		///	The <c>ContactGroups</c> field.
		///	designer:fld/FVA1/FVA7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVA7]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.ContactGroupEntity> ContactGroups
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.ContactGroupEntity> ("[FVA7]");
			}
		}
		
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.AbstractContactEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.AbstractContactEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 1);	// [FVA1]
		public static readonly string EntityStructuredTypeKey = "[FVA1]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AbstractContactEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.ContactGroup Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ContactGroup</c> entity.
	///	designer:cap/FVA4
	///	</summary>
	public partial class ContactGroupEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory, global::Epsitec.Cresus.Core.Entities.IItemRank
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/FVA4/8VA1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA1]")]
		public int? Rank
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IItemRankInterfaceImplementation.GetRank (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IItemRankInterfaceImplementation.SetRank (this, value);
			}
		}
		#endregion
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/FVA4/8VA3
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
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/FVA4/8VA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA5]")]
		public string Code
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.GetCode (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.SetCode (this, value);
			}
		}
		#endregion
		#region INameDescription Members
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/FVA4/8VA7
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
		///	designer:fld/FVA4/8VA8
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
		///	The <c>ContactGroupType</c> field.
		///	designer:fld/FVA4/FVA42
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVA42]")]
		public global::Epsitec.Cresus.Core.Business.ContactGroupType ContactGroupType
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.ContactGroupType> ("[FVA42]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.ContactGroupType oldValue = this.ContactGroupType;
				if (oldValue != value || !this.IsFieldDefined("[FVA42]"))
				{
					this.OnContactGroupTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.ContactGroupType> ("[FVA42]", oldValue, value);
					this.OnContactGroupTypeChanged (oldValue, value);
				}
			}
		}
		
		partial void OnContactGroupTypeChanging(global::Epsitec.Cresus.Core.Business.ContactGroupType oldValue, global::Epsitec.Cresus.Core.Business.ContactGroupType newValue);
		partial void OnContactGroupTypeChanged(global::Epsitec.Cresus.Core.Business.ContactGroupType oldValue, global::Epsitec.Cresus.Core.Business.ContactGroupType newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ContactGroupEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ContactGroupEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 4);	// [FVA4]
		public static readonly string EntityStructuredTypeKey = "[FVA4]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ContactGroupEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.NaturalPerson Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>NaturalPerson</c> entity.
	///	designer:cap/FVA5
	///	</summary>
	public partial class NaturalPersonEntity : global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity
	{
		///	<summary>
		///	Titre utilisé dans des formules de politesse (par ex. <i>Monsieur</i> ou <i>Madame</i>)
		///	designer:fld/FVA5/FVAF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAF]")]
		public global::Epsitec.Cresus.Core.Entities.PersonTitleEntity Title
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PersonTitleEntity> ("[FVAF]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PersonTitleEntity oldValue = this.Title;
				if (oldValue != value || !this.IsFieldDefined("[FVAF]"))
				{
					this.OnTitleChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PersonTitleEntity> ("[FVAF]", oldValue, value);
					this.OnTitleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Firstname</c> field.
		///	designer:fld/FVA5/FVAG
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAG]")]
		public string Firstname
		{
			get
			{
				return this.GetField<string> ("[FVAG]");
			}
			set
			{
				string oldValue = this.Firstname;
				if (oldValue != value || !this.IsFieldDefined("[FVAG]"))
				{
					this.OnFirstnameChanging (oldValue, value);
					this.SetField<string> ("[FVAG]", oldValue, value);
					this.OnFirstnameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Lastname</c> field.
		///	designer:fld/FVA5/FVAH
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAH]")]
		public string Lastname
		{
			get
			{
				return this.GetField<string> ("[FVAH]");
			}
			set
			{
				string oldValue = this.Lastname;
				if (oldValue != value || !this.IsFieldDefined("[FVAH]"))
				{
					this.OnLastnameChanging (oldValue, value);
					this.SetField<string> ("[FVAH]", oldValue, value);
					this.OnLastnameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Gender</c> field.
		///	designer:fld/FVA5/FVAI
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAI]")]
		public global::Epsitec.Cresus.Core.Entities.PersonGenderEntity Gender
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PersonGenderEntity> ("[FVAI]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PersonGenderEntity oldValue = this.Gender;
				if (oldValue != value || !this.IsFieldDefined("[FVAI]"))
				{
					this.OnGenderChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PersonGenderEntity> ("[FVAI]", oldValue, value);
					this.OnGenderChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DateOfBirth</c> field.
		///	designer:fld/FVA5/FVAJ
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAJ]")]
		public global::Epsitec.Common.Types.Date? DateOfBirth
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[FVAJ]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.DateOfBirth;
				if (oldValue != value || !this.IsFieldDefined("[FVAJ]"))
				{
					this.OnDateOfBirthChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[FVAJ]", oldValue, value);
					this.OnDateOfBirthChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Pictures</c> field.
		///	designer:fld/FVA5/FVAK
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAK]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.ImageEntity> Pictures
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.ImageEntity> ("[FVAK]");
			}
		}
		
		partial void OnTitleChanging(global::Epsitec.Cresus.Core.Entities.PersonTitleEntity oldValue, global::Epsitec.Cresus.Core.Entities.PersonTitleEntity newValue);
		partial void OnTitleChanged(global::Epsitec.Cresus.Core.Entities.PersonTitleEntity oldValue, global::Epsitec.Cresus.Core.Entities.PersonTitleEntity newValue);
		partial void OnFirstnameChanging(string oldValue, string newValue);
		partial void OnFirstnameChanged(string oldValue, string newValue);
		partial void OnLastnameChanging(string oldValue, string newValue);
		partial void OnLastnameChanged(string oldValue, string newValue);
		partial void OnGenderChanging(global::Epsitec.Cresus.Core.Entities.PersonGenderEntity oldValue, global::Epsitec.Cresus.Core.Entities.PersonGenderEntity newValue);
		partial void OnGenderChanged(global::Epsitec.Cresus.Core.Entities.PersonGenderEntity oldValue, global::Epsitec.Cresus.Core.Entities.PersonGenderEntity newValue);
		partial void OnDateOfBirthChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnDateOfBirthChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 5);	// [FVA5]
		public static readonly new string EntityStructuredTypeKey = "[FVA5]";
		
		#region Repository Class
		public new partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<NaturalPersonEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.LegalPerson Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>LegalPerson</c> entity.
	///	designer:cap/FVA6
	///	</summary>
	public partial class LegalPersonEntity : global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity
	{
		///	<summary>
		///	The <c>LegalPersonType</c> field.
		///	designer:fld/FVA6/FVAN
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAN]")]
		public global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity LegalPersonType
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity> ("[FVAN]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity oldValue = this.LegalPersonType;
				if (oldValue != value || !this.IsFieldDefined("[FVAN]"))
				{
					this.OnLegalPersonTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity> ("[FVAN]", oldValue, value);
					this.OnLegalPersonTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/FVA6/FVAO
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAO]")]
		public global::Epsitec.Common.Types.FormattedText Name
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[FVAO]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[FVAO]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[FVAO]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ShortName</c> field.
		///	designer:fld/FVA6/FVAP
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAP]")]
		public global::Epsitec.Common.Types.FormattedText ShortName
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[FVAP]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.ShortName;
				if (oldValue != value || !this.IsFieldDefined("[FVAP]"))
				{
					this.OnShortNameChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[FVAP]", oldValue, value);
					this.OnShortNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Complement</c> field.
		///	designer:fld/FVA6/FVAQ
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAQ]")]
		public global::Epsitec.Common.Types.FormattedText Complement
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[FVAQ]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Complement;
				if (oldValue != value || !this.IsFieldDefined("[FVAQ]"))
				{
					this.OnComplementChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[FVAQ]", oldValue, value);
					this.OnComplementChanged (oldValue, value);
				}
			}
		}
		
		partial void OnLegalPersonTypeChanging(global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity oldValue, global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity newValue);
		partial void OnLegalPersonTypeChanged(global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity oldValue, global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity newValue);
		partial void OnNameChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnNameChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnShortNameChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnShortNameChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnComplementChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnComplementChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.LegalPersonEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.LegalPersonEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 6);	// [FVA6]
		public static readonly new string EntityStructuredTypeKey = "[FVA6]";
		
		#region Repository Class
		public new partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<LegalPersonEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.PersonTitle Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>PersonTitle</c> entity.
	///	designer:cap/FVAA
	///	</summary>
	public partial class PersonTitleEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IItemRank
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/FVAA/8VA1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA1]")]
		public int? Rank
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IItemRankInterfaceImplementation.GetRank (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IItemRankInterfaceImplementation.SetRank (this, value);
			}
		}
		#endregion
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/FVAA/8VA3
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
		///	The <c>ShortName</c> field.
		///	designer:fld/FVAA/FVAB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAB]")]
		public global::Epsitec.Common.Types.FormattedText ShortName
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[FVAB]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.ShortName;
				if (oldValue != value || !this.IsFieldDefined("[FVAB]"))
				{
					this.OnShortNameChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[FVAB]", oldValue, value);
					this.OnShortNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/FVAA/FVAC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAC]")]
		public global::Epsitec.Common.Types.FormattedText Name
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[FVAC]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[FVAC]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[FVAC]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CompatibleGenders</c> field.
		///	designer:fld/FVAA/FVAE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAE]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.PersonGenderEntity> CompatibleGenders
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.PersonGenderEntity> ("[FVAE]");
			}
		}
		
		partial void OnShortNameChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnShortNameChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnNameChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnNameChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.PersonTitleEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.PersonTitleEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 10);	// [FVAA]
		public static readonly string EntityStructuredTypeKey = "[FVAA]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<PersonTitleEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.PersonGender Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>PersonGender</c> entity.
	///	designer:cap/FVAD
	///	</summary>
	public partial class PersonGenderEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory, global::Epsitec.Cresus.Core.Entities.IItemRank
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/FVAD/8VA1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA1]")]
		public int? Rank
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IItemRankInterfaceImplementation.GetRank (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IItemRankInterfaceImplementation.SetRank (this, value);
			}
		}
		#endregion
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/FVAD/8VA3
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
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/FVAD/8VA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA5]")]
		public string Code
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.GetCode (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.SetCode (this, value);
			}
		}
		#endregion
		#region INameDescription Members
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/FVAD/8VA7
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
		///	designer:fld/FVAD/8VA8
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
			return global::Epsitec.Cresus.Core.Entities.PersonGenderEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.PersonGenderEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 13);	// [FVAD]
		public static readonly string EntityStructuredTypeKey = "[FVAD]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<PersonGenderEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.LegalPersonType Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>LegalPersonType</c> entity.
	///	designer:cap/FVAL
	///	</summary>
	public partial class LegalPersonTypeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/FVAL/8VA3
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
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/FVAL/8VA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA5]")]
		public string Code
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.GetCode (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.SetCode (this, value);
			}
		}
		#endregion
		#region INameDescription Members
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/FVAL/8VA7
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
		///	designer:fld/FVAL/8VA8
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
		///	The <c>ShortName</c> field.
		///	designer:fld/FVAL/FVAM
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAM]")]
		public global::Epsitec.Common.Types.FormattedText ShortName
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[FVAM]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.ShortName;
				if (oldValue != value || !this.IsFieldDefined("[FVAM]"))
				{
					this.OnShortNameChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[FVAM]", oldValue, value);
					this.OnShortNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnShortNameChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnShortNameChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 21);	// [FVAL]
		public static readonly string EntityStructuredTypeKey = "[FVAL]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<LegalPersonTypeEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.MailContact Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>MailContact</c> entity.
	///	designer:cap/FVAR
	///	</summary>
	public partial class MailContactEntity : global::Epsitec.Cresus.Core.Entities.AbstractContactEntity
	{
		///	<summary>
		///	The <c>PersonAddress</c> field.
		///	designer:fld/FVAR/FVAH2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAH2]")]
		public global::Epsitec.Common.Types.FormattedText PersonAddress
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[FVAH2]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.PersonAddress;
				if (oldValue != value || !this.IsFieldDefined("[FVAH2]"))
				{
					this.OnPersonAddressChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[FVAH2]", oldValue, value);
					this.OnPersonAddressChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Complement</c> field.
		///	designer:fld/FVAR/FVAB2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAB2]")]
		public string Complement
		{
			get
			{
				return this.GetField<string> ("[FVAB2]");
			}
			set
			{
				string oldValue = this.Complement;
				if (oldValue != value || !this.IsFieldDefined("[FVAB2]"))
				{
					this.OnComplementChanging (oldValue, value);
					this.SetField<string> ("[FVAB2]", oldValue, value);
					this.OnComplementChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>StreetName</c> field.
		///	designer:fld/FVAR/FVAC2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAC2]")]
		public string StreetName
		{
			get
			{
				return this.GetField<string> ("[FVAC2]");
			}
			set
			{
				string oldValue = this.StreetName;
				if (oldValue != value || !this.IsFieldDefined("[FVAC2]"))
				{
					this.OnStreetNameChanging (oldValue, value);
					this.SetField<string> ("[FVAC2]", oldValue, value);
					this.OnStreetNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>HouseNumberPrefix</c> field.
		///	designer:fld/FVAR/FVA13
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVA13]")]
		public string HouseNumberPrefix
		{
			get
			{
				return this.GetField<string> ("[FVA13]");
			}
			set
			{
				string oldValue = this.HouseNumberPrefix;
				if (oldValue != value || !this.IsFieldDefined("[FVA13]"))
				{
					this.OnHouseNumberPrefixChanging (oldValue, value);
					this.SetField<string> ("[FVA13]", oldValue, value);
					this.OnHouseNumberPrefixChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>HouseNumber</c> field.
		///	designer:fld/FVAR/FVAU2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAU2]")]
		public int? HouseNumber
		{
			get
			{
				return this.GetField<int?> ("[FVAU2]");
			}
			set
			{
				int? oldValue = this.HouseNumber;
				if (oldValue != value || !this.IsFieldDefined("[FVAU2]"))
				{
					this.OnHouseNumberChanging (oldValue, value);
					this.SetField<int?> ("[FVAU2]", oldValue, value);
					this.OnHouseNumberChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>HouseNumberSuffix</c> field.
		///	designer:fld/FVAR/FVAT2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAT2]")]
		public string HouseNumberSuffix
		{
			get
			{
				return this.GetField<string> ("[FVAT2]");
			}
			set
			{
				string oldValue = this.HouseNumberSuffix;
				if (oldValue != value || !this.IsFieldDefined("[FVAT2]"))
				{
					this.OnHouseNumberSuffixChanging (oldValue, value);
					this.SetField<string> ("[FVAT2]", oldValue, value);
					this.OnHouseNumberSuffixChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PostBoxPrefix</c> field.
		///	designer:fld/FVAR/FVAD2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAD2]")]
		public string PostBoxPrefix
		{
			get
			{
				return this.GetField<string> ("[FVAD2]");
			}
			set
			{
				string oldValue = this.PostBoxPrefix;
				if (oldValue != value || !this.IsFieldDefined("[FVAD2]"))
				{
					this.OnPostBoxPrefixChanging (oldValue, value);
					this.SetField<string> ("[FVAD2]", oldValue, value);
					this.OnPostBoxPrefixChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PostBoxNumber</c> field.
		///	designer:fld/FVAR/FVAV2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAV2]")]
		public int? PostBoxNumber
		{
			get
			{
				return this.GetField<int?> ("[FVAV2]");
			}
			set
			{
				int? oldValue = this.PostBoxNumber;
				if (oldValue != value || !this.IsFieldDefined("[FVAV2]"))
				{
					this.OnPostBoxNumberChanging (oldValue, value);
					this.SetField<int?> ("[FVAV2]", oldValue, value);
					this.OnPostBoxNumberChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PostBoxSuffix</c> field.
		///	designer:fld/FVAR/FVA03
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVA03]")]
		public string PostBoxSuffix
		{
			get
			{
				return this.GetField<string> ("[FVA03]");
			}
			set
			{
				string oldValue = this.PostBoxSuffix;
				if (oldValue != value || !this.IsFieldDefined("[FVA03]"))
				{
					this.OnPostBoxSuffixChanging (oldValue, value);
					this.SetField<string> ("[FVA03]", oldValue, value);
					this.OnPostBoxSuffixChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>EditionStreetAndHouseNumber</c> field.
		///	designer:fld/FVAR/FVA43
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVA43]")]
		public string EditionStreetAndHouseNumber
		{
			get
			{
				string value = default (string);
				this.GetEditionStreetAndHouseNumber (ref value);
				return value;
			}
			set
			{
				string oldValue = this.EditionStreetAndHouseNumber;
				if (oldValue != value || !this.IsFieldDefined("[FVA43]"))
				{
					this.OnEditionStreetAndHouseNumberChanging (oldValue, value);
					this.SetEditionStreetAndHouseNumber (value);
					this.OnEditionStreetAndHouseNumberChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>EditionPostBoxNumber</c> field.
		///	designer:fld/FVAR/FVA53
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVA53]")]
		public string EditionPostBoxNumber
		{
			get
			{
				string value = default (string);
				this.GetEditionPostBoxNumber (ref value);
				return value;
			}
			set
			{
				string oldValue = this.EditionPostBoxNumber;
				if (oldValue != value || !this.IsFieldDefined("[FVA53]"))
				{
					this.OnEditionPostBoxNumberChanging (oldValue, value);
					this.SetEditionPostBoxNumber (value);
					this.OnEditionPostBoxNumberChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Location</c> field.
		///	designer:fld/FVAR/FVAE2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAE2]")]
		public global::Epsitec.Cresus.Core.Entities.LocationEntity Location
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.LocationEntity> ("[FVAE2]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.LocationEntity oldValue = this.Location;
				if (oldValue != value || !this.IsFieldDefined("[FVAE2]"))
				{
					this.OnLocationChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.LocationEntity> ("[FVAE2]", oldValue, value);
					this.OnLocationChanged (oldValue, value);
				}
			}
		}
		
		partial void OnPersonAddressChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnPersonAddressChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnComplementChanging(string oldValue, string newValue);
		partial void OnComplementChanged(string oldValue, string newValue);
		partial void OnStreetNameChanging(string oldValue, string newValue);
		partial void OnStreetNameChanged(string oldValue, string newValue);
		partial void OnHouseNumberPrefixChanging(string oldValue, string newValue);
		partial void OnHouseNumberPrefixChanged(string oldValue, string newValue);
		partial void OnHouseNumberChanging(int? oldValue, int? newValue);
		partial void OnHouseNumberChanged(int? oldValue, int? newValue);
		partial void OnHouseNumberSuffixChanging(string oldValue, string newValue);
		partial void OnHouseNumberSuffixChanged(string oldValue, string newValue);
		partial void OnPostBoxPrefixChanging(string oldValue, string newValue);
		partial void OnPostBoxPrefixChanged(string oldValue, string newValue);
		partial void OnPostBoxNumberChanging(int? oldValue, int? newValue);
		partial void OnPostBoxNumberChanged(int? oldValue, int? newValue);
		partial void OnPostBoxSuffixChanging(string oldValue, string newValue);
		partial void OnPostBoxSuffixChanged(string oldValue, string newValue);
		partial void OnEditionStreetAndHouseNumberChanging(string oldValue, string newValue);
		partial void OnEditionStreetAndHouseNumberChanged(string oldValue, string newValue);
		partial void OnEditionPostBoxNumberChanging(string oldValue, string newValue);
		partial void OnEditionPostBoxNumberChanged(string oldValue, string newValue);
		partial void OnLocationChanging(global::Epsitec.Cresus.Core.Entities.LocationEntity oldValue, global::Epsitec.Cresus.Core.Entities.LocationEntity newValue);
		partial void OnLocationChanged(global::Epsitec.Cresus.Core.Entities.LocationEntity oldValue, global::Epsitec.Cresus.Core.Entities.LocationEntity newValue);
		
		partial void GetEditionStreetAndHouseNumber(ref string value);
		partial void SetEditionStreetAndHouseNumber(string value);
		partial void GetEditionPostBoxNumber(ref string value);
		partial void SetEditionPostBoxNumber(string value);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.MailContactEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.MailContactEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 27);	// [FVAR]
		public static readonly new string EntityStructuredTypeKey = "[FVAR]";
		
		#region Repository Class
		public new partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<MailContactEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.Location Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>Location</c> entity.
	///	designer:cap/FVA21
	///	</summary>
	public partial class LocationEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IItemCode
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/FVA21/8VA3
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
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/FVA21/8VA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA5]")]
		public string Code
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.GetCode (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.SetCode (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>PostalCode</c> field.
		///	designer:fld/FVA21/FVA71
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVA71]")]
		public string PostalCode
		{
			get
			{
				return this.GetField<string> ("[FVA71]");
			}
			set
			{
				string oldValue = this.PostalCode;
				if (oldValue != value || !this.IsFieldDefined("[FVA71]"))
				{
					this.OnPostalCodeChanging (oldValue, value);
					this.SetField<string> ("[FVA71]", oldValue, value);
					this.OnPostalCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/FVA21/FVA81
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVA81]")]
		public global::Epsitec.Common.Types.FormattedText Name
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[FVA81]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[FVA81]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[FVA81]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Country</c> field.
		///	designer:fld/FVA21/FVA91
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVA91]")]
		public global::Epsitec.Cresus.Core.Entities.CountryEntity Country
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.CountryEntity> ("[FVA91]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.CountryEntity oldValue = this.Country;
				if (oldValue != value || !this.IsFieldDefined("[FVA91]"))
				{
					this.OnCountryChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.CountryEntity> ("[FVA91]", oldValue, value);
					this.OnCountryChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Region</c> field.
		///	designer:fld/FVA21/FVAA1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAA1]")]
		public global::Epsitec.Cresus.Core.Entities.StateProvinceCountyEntity Region
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.StateProvinceCountyEntity> ("[FVAA1]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.StateProvinceCountyEntity oldValue = this.Region;
				if (oldValue != value || !this.IsFieldDefined("[FVAA1]"))
				{
					this.OnRegionChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.StateProvinceCountyEntity> ("[FVAA1]", oldValue, value);
					this.OnRegionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Language1</c> field.
		///	designer:fld/FVA21/FVA52
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVA52]")]
		public global::Epsitec.Cresus.Core.Entities.LanguageEntity Language1
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.LanguageEntity> ("[FVA52]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.LanguageEntity oldValue = this.Language1;
				if (oldValue != value || !this.IsFieldDefined("[FVA52]"))
				{
					this.OnLanguage1Changing (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.LanguageEntity> ("[FVA52]", oldValue, value);
					this.OnLanguage1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Language2</c> field.
		///	designer:fld/FVA21/FVA62
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVA62]")]
		public global::Epsitec.Cresus.Core.Entities.LanguageEntity Language2
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.LanguageEntity> ("[FVA62]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.LanguageEntity oldValue = this.Language2;
				if (oldValue != value || !this.IsFieldDefined("[FVA62]"))
				{
					this.OnLanguage2Changing (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.LanguageEntity> ("[FVA62]", oldValue, value);
					this.OnLanguage2Changed (oldValue, value);
				}
			}
		}
		
		partial void OnPostalCodeChanging(string oldValue, string newValue);
		partial void OnPostalCodeChanged(string oldValue, string newValue);
		partial void OnNameChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnNameChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnCountryChanging(global::Epsitec.Cresus.Core.Entities.CountryEntity oldValue, global::Epsitec.Cresus.Core.Entities.CountryEntity newValue);
		partial void OnCountryChanged(global::Epsitec.Cresus.Core.Entities.CountryEntity oldValue, global::Epsitec.Cresus.Core.Entities.CountryEntity newValue);
		partial void OnRegionChanging(global::Epsitec.Cresus.Core.Entities.StateProvinceCountyEntity oldValue, global::Epsitec.Cresus.Core.Entities.StateProvinceCountyEntity newValue);
		partial void OnRegionChanged(global::Epsitec.Cresus.Core.Entities.StateProvinceCountyEntity oldValue, global::Epsitec.Cresus.Core.Entities.StateProvinceCountyEntity newValue);
		partial void OnLanguage1Changing(global::Epsitec.Cresus.Core.Entities.LanguageEntity oldValue, global::Epsitec.Cresus.Core.Entities.LanguageEntity newValue);
		partial void OnLanguage1Changed(global::Epsitec.Cresus.Core.Entities.LanguageEntity oldValue, global::Epsitec.Cresus.Core.Entities.LanguageEntity newValue);
		partial void OnLanguage2Changing(global::Epsitec.Cresus.Core.Entities.LanguageEntity oldValue, global::Epsitec.Cresus.Core.Entities.LanguageEntity newValue);
		partial void OnLanguage2Changed(global::Epsitec.Cresus.Core.Entities.LanguageEntity oldValue, global::Epsitec.Cresus.Core.Entities.LanguageEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.LocationEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.LocationEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 34);	// [FVA21]
		public static readonly string EntityStructuredTypeKey = "[FVA21]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<LocationEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.StateProvinceCounty Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>StateProvinceCounty</c> entity.
	///	designer:cap/FVA31
	///	</summary>
	public partial class StateProvinceCountyEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/FVA31/8VA3
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
		///	The <c>RegionCode</c> field.
		///	designer:fld/FVA31/FVAB1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAB1]")]
		public string RegionCode
		{
			get
			{
				return this.GetField<string> ("[FVAB1]");
			}
			set
			{
				string oldValue = this.RegionCode;
				if (oldValue != value || !this.IsFieldDefined("[FVAB1]"))
				{
					this.OnRegionCodeChanging (oldValue, value);
					this.SetField<string> ("[FVAB1]", oldValue, value);
					this.OnRegionCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/FVA31/FVAC1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAC1]")]
		public global::Epsitec.Common.Types.FormattedText Name
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[FVAC1]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[FVAC1]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[FVAC1]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Country</c> field.
		///	designer:fld/FVA31/FVAD1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAD1]")]
		public global::Epsitec.Cresus.Core.Entities.CountryEntity Country
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.CountryEntity> ("[FVAD1]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.CountryEntity oldValue = this.Country;
				if (oldValue != value || !this.IsFieldDefined("[FVAD1]"))
				{
					this.OnCountryChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.CountryEntity> ("[FVAD1]", oldValue, value);
					this.OnCountryChanged (oldValue, value);
				}
			}
		}
		
		partial void OnRegionCodeChanging(string oldValue, string newValue);
		partial void OnRegionCodeChanged(string oldValue, string newValue);
		partial void OnNameChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnNameChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnCountryChanging(global::Epsitec.Cresus.Core.Entities.CountryEntity oldValue, global::Epsitec.Cresus.Core.Entities.CountryEntity newValue);
		partial void OnCountryChanged(global::Epsitec.Cresus.Core.Entities.CountryEntity oldValue, global::Epsitec.Cresus.Core.Entities.CountryEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.StateProvinceCountyEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.StateProvinceCountyEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 35);	// [FVA31]
		public static readonly string EntityStructuredTypeKey = "[FVA31]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<StateProvinceCountyEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Unknown)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.Country Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>Country</c> entity.
	///	designer:cap/FVA41
	///	</summary>
	public partial class CountryEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IPreferred
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/FVA41/8VA3
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
		#region IPreferred Members
		///	<summary>
		///	The <c>IsPreferred</c> field.
		///	designer:fld/FVA41/8VAE2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAE2]")]
		public bool IsPreferred
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IPreferredInterfaceImplementation.GetIsPreferred (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IPreferredInterfaceImplementation.SetIsPreferred (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>CountryCode</c> field.
		///	designer:fld/FVA41/FVA51
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVA51]")]
		public string CountryCode
		{
			get
			{
				return this.GetField<string> ("[FVA51]");
			}
			set
			{
				string oldValue = this.CountryCode;
				if (oldValue != value || !this.IsFieldDefined("[FVA51]"))
				{
					this.OnCountryCodeChanging (oldValue, value);
					this.SetField<string> ("[FVA51]", oldValue, value);
					this.OnCountryCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/FVA41/FVA61
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVA61]")]
		public global::Epsitec.Common.Types.FormattedText Name
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[FVA61]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[FVA61]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[FVA61]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnCountryCodeChanging(string oldValue, string newValue);
		partial void OnCountryCodeChanged(string oldValue, string newValue);
		partial void OnNameChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnNameChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.CountryEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.CountryEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 36);	// [FVA41]
		public static readonly string EntityStructuredTypeKey = "[FVA41]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<CountryEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.TelecomType Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>TelecomType</c> entity.
	///	designer:cap/FVAK1
	///	</summary>
	public partial class TelecomTypeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory, global::Epsitec.Cresus.Core.Entities.IItemRank
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/FVAK1/8VA1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA1]")]
		public int? Rank
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IItemRankInterfaceImplementation.GetRank (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IItemRankInterfaceImplementation.SetRank (this, value);
			}
		}
		#endregion
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/FVAK1/8VA3
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
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/FVAK1/8VA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA5]")]
		public string Code
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.GetCode (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.SetCode (this, value);
			}
		}
		#endregion
		#region INameDescription Members
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/FVAK1/8VA7
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
		///	designer:fld/FVAK1/8VA8
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
			return global::Epsitec.Cresus.Core.Entities.TelecomTypeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.TelecomTypeEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 52);	// [FVAK1]
		public static readonly string EntityStructuredTypeKey = "[FVAK1]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<TelecomTypeEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.TelecomContact Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>TelecomContact</c> entity.
	///	designer:cap/FVAL1
	///	</summary>
	public partial class TelecomContactEntity : global::Epsitec.Cresus.Core.Entities.AbstractContactEntity
	{
		///	<summary>
		///	The <c>TelecomType</c> field.
		///	designer:fld/FVAL1/FVAM1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAM1]")]
		public global::Epsitec.Cresus.Core.Entities.TelecomTypeEntity TelecomType
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.TelecomTypeEntity> ("[FVAM1]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.TelecomTypeEntity oldValue = this.TelecomType;
				if (oldValue != value || !this.IsFieldDefined("[FVAM1]"))
				{
					this.OnTelecomTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.TelecomTypeEntity> ("[FVAM1]", oldValue, value);
					this.OnTelecomTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Number</c> field.
		///	designer:fld/FVAL1/FVAN1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAN1]")]
		public string Number
		{
			get
			{
				return this.GetField<string> ("[FVAN1]");
			}
			set
			{
				string oldValue = this.Number;
				if (oldValue != value || !this.IsFieldDefined("[FVAN1]"))
				{
					this.OnNumberChanging (oldValue, value);
					this.SetField<string> ("[FVAN1]", oldValue, value);
					this.OnNumberChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Extension</c> field.
		///	designer:fld/FVAL1/FVAO1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAO1]")]
		public string Extension
		{
			get
			{
				return this.GetField<string> ("[FVAO1]");
			}
			set
			{
				string oldValue = this.Extension;
				if (oldValue != value || !this.IsFieldDefined("[FVAO1]"))
				{
					this.OnExtensionChanging (oldValue, value);
					this.SetField<string> ("[FVAO1]", oldValue, value);
					this.OnExtensionChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTelecomTypeChanging(global::Epsitec.Cresus.Core.Entities.TelecomTypeEntity oldValue, global::Epsitec.Cresus.Core.Entities.TelecomTypeEntity newValue);
		partial void OnTelecomTypeChanged(global::Epsitec.Cresus.Core.Entities.TelecomTypeEntity oldValue, global::Epsitec.Cresus.Core.Entities.TelecomTypeEntity newValue);
		partial void OnNumberChanging(string oldValue, string newValue);
		partial void OnNumberChanged(string oldValue, string newValue);
		partial void OnExtensionChanging(string oldValue, string newValue);
		partial void OnExtensionChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.TelecomContactEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.TelecomContactEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 53);	// [FVAL1]
		public static readonly new string EntityStructuredTypeKey = "[FVAL1]";
		
		#region Repository Class
		public new partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<TelecomContactEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.UriContact Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>UriContact</c> entity.
	///	designer:cap/FVAP1
	///	</summary>
	public partial class UriContactEntity : global::Epsitec.Cresus.Core.Entities.AbstractContactEntity
	{
		///	<summary>
		///	The <c>UriType</c> field.
		///	designer:fld/FVAP1/FVAS1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAS1]")]
		public global::Epsitec.Cresus.Core.Entities.UriTypeEntity UriType
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.UriTypeEntity> ("[FVAS1]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.UriTypeEntity oldValue = this.UriType;
				if (oldValue != value || !this.IsFieldDefined("[FVAS1]"))
				{
					this.OnUriTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.UriTypeEntity> ("[FVAS1]", oldValue, value);
					this.OnUriTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Uri</c> field.
		///	designer:fld/FVAP1/FVAT1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAT1]")]
		public string Uri
		{
			get
			{
				return this.GetField<string> ("[FVAT1]");
			}
			set
			{
				string oldValue = this.Uri;
				if (oldValue != value || !this.IsFieldDefined("[FVAT1]"))
				{
					this.OnUriChanging (oldValue, value);
					this.SetField<string> ("[FVAT1]", oldValue, value);
					this.OnUriChanged (oldValue, value);
				}
			}
		}
		
		partial void OnUriTypeChanging(global::Epsitec.Cresus.Core.Entities.UriTypeEntity oldValue, global::Epsitec.Cresus.Core.Entities.UriTypeEntity newValue);
		partial void OnUriTypeChanged(global::Epsitec.Cresus.Core.Entities.UriTypeEntity oldValue, global::Epsitec.Cresus.Core.Entities.UriTypeEntity newValue);
		partial void OnUriChanging(string oldValue, string newValue);
		partial void OnUriChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.UriContactEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.UriContactEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 57);	// [FVAP1]
		public static readonly new string EntityStructuredTypeKey = "[FVAP1]";
		
		#region Repository Class
		public new partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<UriContactEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.UriType Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	Définition du type de ressource Internet (mail, http, ftp, etc.)
	///	designer:cap/FVAQ1
	///	</summary>
	public partial class UriTypeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/FVAQ1/8VA3
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
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/FVAQ1/8VA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA5]")]
		public string Code
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.GetCode (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.SetCode (this, value);
			}
		}
		#endregion
		#region INameDescription Members
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/FVAQ1/8VA7
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
		///	designer:fld/FVAQ1/8VA8
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
		///	Protocole Internet utilisé (mailto, http, ftp, etc.)
		///	designer:fld/FVAQ1/FVAR1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAR1]")]
		public string Protocol
		{
			get
			{
				return this.GetField<string> ("[FVAR1]");
			}
			set
			{
				string oldValue = this.Protocol;
				if (oldValue != value || !this.IsFieldDefined("[FVAR1]"))
				{
					this.OnProtocolChanging (oldValue, value);
					this.SetField<string> ("[FVAR1]", oldValue, value);
					this.OnProtocolChanged (oldValue, value);
				}
			}
		}
		
		partial void OnProtocolChanging(string oldValue, string newValue);
		partial void OnProtocolChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.UriTypeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.UriTypeEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 58);	// [FVAQ1]
		public static readonly string EntityStructuredTypeKey = "[FVAQ1]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<UriTypeEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.People Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>People</c> entity.
	///	designer:cap/FVA72
	///	</summary>
	public partial class PeopleEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IItemCode, global::Epsitec.Cresus.Core.Entities.IReferenceNumber, global::Epsitec.Cresus.Core.Entities.IDateRange
	{
		#region IReferenceNumber Members
		///	<summary>
		///	The <c>IdA</c> field.
		///	designer:fld/FVA72/8VA11
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA11]")]
		public string IdA
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.GetIdA (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.SetIdA (this, value);
			}
		}
		#endregion
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/FVA72/8VA3
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
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/FVA72/8VA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA5]")]
		public string Code
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.GetCode (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.SetCode (this, value);
			}
		}
		#endregion
		#region IDateRange Members
		///	<summary>
		///	The <c>BeginDate</c> field.
		///	designer:fld/FVA72/8VAO
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
		#region IReferenceNumber Members
		///	<summary>
		///	The <c>IdB</c> field.
		///	designer:fld/FVA72/8VA21
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA21]")]
		public string IdB
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.GetIdB (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.SetIdB (this, value);
			}
		}
		#endregion
		#region IDateRange Members
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/FVA72/8VAP
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
		#region IReferenceNumber Members
		///	<summary>
		///	The <c>IdC</c> field.
		///	designer:fld/FVA72/8VA31
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA31]")]
		public string IdC
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.GetIdC (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.SetIdC (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>NaturalPerson</c> field.
		///	designer:fld/FVA72/FVA82
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVA82]")]
		public global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity NaturalPerson
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity> ("[FVA82]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity oldValue = this.NaturalPerson;
				if (oldValue != value || !this.IsFieldDefined("[FVA82]"))
				{
					this.OnNaturalPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity> ("[FVA82]", oldValue, value);
					this.OnNaturalPersonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PeopleGroups</c> field.
		///	designer:fld/FVA72/FVAA2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAA2]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.PeopleGroupEntity> PeopleGroups
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.PeopleGroupEntity> ("[FVAA2]");
			}
		}
		
		partial void OnNaturalPersonChanging(global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity newValue);
		partial void OnNaturalPersonChanged(global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.PeopleEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.PeopleEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 71);	// [FVA72]
		public static readonly string EntityStructuredTypeKey = "[FVA72]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<PeopleEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.PeopleGroup Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>PeopleGroup</c> entity.
	///	designer:cap/FVA92
	///	</summary>
	public partial class PeopleGroupEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/FVA92/8VA3
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
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/FVA92/8VA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA5]")]
		public string Code
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.GetCode (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.SetCode (this, value);
			}
		}
		#endregion
		#region INameDescription Members
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/FVA92/8VA7
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
		///	designer:fld/FVA92/8VA8
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
			return global::Epsitec.Cresus.Core.Entities.PeopleGroupEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.PeopleGroupEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 73);	// [FVA92]
		public static readonly string EntityStructuredTypeKey = "[FVA92]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<PeopleGroupEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.ContactPersonGroup Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ContactPersonGroup</c> entity.
	///	designer:cap/FVAI2
	///	</summary>
	public partial class ContactPersonGroupEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/FVAI2/8VA3
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
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/FVAI2/8VA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA5]")]
		public string Code
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.GetCode (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IItemCodeInterfaceImplementation.SetCode (this, value);
			}
		}
		#endregion
		#region INameDescription Members
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/FVAI2/8VA7
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
		///	designer:fld/FVAI2/8VA8
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
			return global::Epsitec.Cresus.Core.Entities.ContactPersonGroupEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ContactPersonGroupEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 82);	// [FVAI2]
		public static readonly string EntityStructuredTypeKey = "[FVAI2]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ContactPersonGroupEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.ContactPerson Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ContactPerson</c> entity.
	///	designer:cap/FVAJ2
	///	</summary>
	public partial class ContactPersonEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.INameDescription
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/FVAJ2/8VA3
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
		///	designer:fld/FVAJ2/8VA7
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
		///	designer:fld/FVAJ2/8VA8
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
		///	The <c>Person</c> field.
		///	designer:fld/FVAJ2/FVAK2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAK2]")]
		public global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity Person
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity> ("[FVAK2]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity oldValue = this.Person;
				if (oldValue != value || !this.IsFieldDefined("[FVAK2]"))
				{
					this.OnPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity> ("[FVAK2]", oldValue, value);
					this.OnPersonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Groups</c> field.
		///	designer:fld/FVAJ2/FVAL2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[FVAL2]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.ContactPersonGroupEntity> Groups
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.ContactPersonGroupEntity> ("[FVAL2]");
			}
		}
		
		partial void OnPersonChanging(global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity newValue);
		partial void OnPersonChanged(global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ContactPersonEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ContactPersonEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1007, 10, 83);	// [FVAJ2]
		public static readonly string EntityStructuredTypeKey = "[FVAJ2]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ContactPersonEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

