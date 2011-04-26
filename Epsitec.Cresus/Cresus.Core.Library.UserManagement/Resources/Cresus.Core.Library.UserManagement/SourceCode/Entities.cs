//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[AVA]", typeof (Epsitec.Cresus.Core.Entities.SoftwareUserEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[AVA1]", typeof (Epsitec.Cresus.Core.Entities.SoftwareUserGroupEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[AVA2]", typeof (Epsitec.Cresus.Core.Entities.SoftwareUserRoleEntity))]
#region Epsitec.Cresus.Core.SoftwareUser Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>SoftwareUser</c> entity.
	///	designer:cap/AVA
	///	</summary>
	public partial class SoftwareUserEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IItemCode, global::Epsitec.Cresus.Core.Entities.IDateTimeRange
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/AVA/8VA3
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
		///	designer:fld/AVA/8VA5
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
		#region IDateTimeRange Members
		///	<summary>
		///	The <c>BeginDate</c> field.
		///	designer:fld/AVA/8VAK
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAK]")]
		public global::System.DateTime? BeginDate
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IDateTimeRangeInterfaceImplementation.GetBeginDate (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IDateTimeRangeInterfaceImplementation.SetBeginDate (this, value);
			}
		}
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/AVA/8VAL
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAL]")]
		public global::System.DateTime? EndDate
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IDateTimeRangeInterfaceImplementation.GetEndDate (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IDateTimeRangeInterfaceImplementation.SetEndDate (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Person</c> field.
		///	designer:fld/AVA/AVA6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[AVA6]")]
		public global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity Person
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity> ("[AVA6]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity oldValue = this.Person;
				if (oldValue != value || !this.IsFieldDefined("[AVA6]"))
				{
					this.OnPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity> ("[AVA6]", oldValue, value);
					this.OnPersonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DisplayName</c> field.
		///	designer:fld/AVA/AVA7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[AVA7]")]
		public global::Epsitec.Common.Types.FormattedText DisplayName
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[AVA7]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.DisplayName;
				if (oldValue != value || !this.IsFieldDefined("[AVA7]"))
				{
					this.OnDisplayNameChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[AVA7]", oldValue, value);
					this.OnDisplayNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LoginPicture</c> field.
		///	designer:fld/AVA/AVA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[AVA8]")]
		public global::Epsitec.Cresus.Core.Entities.ImageEntity LoginPicture
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.ImageEntity> ("[AVA8]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ImageEntity oldValue = this.LoginPicture;
				if (oldValue != value || !this.IsFieldDefined("[AVA8]"))
				{
					this.OnLoginPictureChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.ImageEntity> ("[AVA8]", oldValue, value);
					this.OnLoginPictureChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LoginName</c> field.
		///	designer:fld/AVA/AVA9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[AVA9]")]
		public string LoginName
		{
			get
			{
				return this.GetField<string> ("[AVA9]");
			}
			set
			{
				string oldValue = this.LoginName;
				if (oldValue != value || !this.IsFieldDefined("[AVA9]"))
				{
					this.OnLoginNameChanging (oldValue, value);
					this.SetField<string> ("[AVA9]", oldValue, value);
					this.OnLoginNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LoginPasswordHash</c> field.
		///	designer:fld/AVA/AVAA
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[AVAA]")]
		public string LoginPasswordHash
		{
			get
			{
				return this.GetField<string> ("[AVAA]");
			}
			set
			{
				string oldValue = this.LoginPasswordHash;
				if (oldValue != value || !this.IsFieldDefined("[AVAA]"))
				{
					this.OnLoginPasswordHashChanging (oldValue, value);
					this.SetField<string> ("[AVAA]", oldValue, value);
					this.OnLoginPasswordHashChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AuthenticationMethod</c> field.
		///	designer:fld/AVA/AVAB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[AVAB]")]
		public global::Epsitec.Cresus.Core.Business.UserManagement.UserAuthenticationMethod AuthenticationMethod
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.UserManagement.UserAuthenticationMethod> ("[AVAB]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.UserManagement.UserAuthenticationMethod oldValue = this.AuthenticationMethod;
				if (oldValue != value || !this.IsFieldDefined("[AVAB]"))
				{
					this.OnAuthenticationMethodChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.UserManagement.UserAuthenticationMethod> ("[AVAB]", oldValue, value);
					this.OnAuthenticationMethodChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>UserGroups</c> field.
		///	designer:fld/AVA/AVAC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[AVAC]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.SoftwareUserGroupEntity> UserGroups
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.SoftwareUserGroupEntity> ("[AVAC]");
			}
		}
		///	<summary>
		///	The <c>Disabled</c> field.
		///	designer:fld/AVA/AVAD
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[AVAD]")]
		public bool Disabled
		{
			get
			{
				return this.GetField<bool> ("[AVAD]");
			}
			set
			{
				bool oldValue = this.Disabled;
				if (oldValue != value || !this.IsFieldDefined("[AVAD]"))
				{
					this.OnDisabledChanging (oldValue, value);
					this.SetField<bool> ("[AVAD]", oldValue, value);
					this.OnDisabledChanged (oldValue, value);
				}
			}
		}
		
		partial void OnPersonChanging(global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity newValue);
		partial void OnPersonChanged(global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity newValue);
		partial void OnDisplayNameChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnDisplayNameChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnLoginPictureChanging(global::Epsitec.Cresus.Core.Entities.ImageEntity oldValue, global::Epsitec.Cresus.Core.Entities.ImageEntity newValue);
		partial void OnLoginPictureChanged(global::Epsitec.Cresus.Core.Entities.ImageEntity oldValue, global::Epsitec.Cresus.Core.Entities.ImageEntity newValue);
		partial void OnLoginNameChanging(string oldValue, string newValue);
		partial void OnLoginNameChanged(string oldValue, string newValue);
		partial void OnLoginPasswordHashChanging(string oldValue, string newValue);
		partial void OnLoginPasswordHashChanged(string oldValue, string newValue);
		partial void OnAuthenticationMethodChanging(global::Epsitec.Cresus.Core.Business.UserManagement.UserAuthenticationMethod oldValue, global::Epsitec.Cresus.Core.Business.UserManagement.UserAuthenticationMethod newValue);
		partial void OnAuthenticationMethodChanged(global::Epsitec.Cresus.Core.Business.UserManagement.UserAuthenticationMethod oldValue, global::Epsitec.Cresus.Core.Business.UserManagement.UserAuthenticationMethod newValue);
		partial void OnDisabledChanging(bool oldValue, bool newValue);
		partial void OnDisabledChanged(bool oldValue, bool newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.SoftwareUserEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.SoftwareUserEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1002, 10, 0);	// [AVA]
		public static readonly string EntityStructuredTypeKey = "[AVA]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<SoftwareUserEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.SoftwareUserGroup Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>SoftwareUserGroup</c> entity.
	///	designer:cap/AVA1
	///	</summary>
	public partial class SoftwareUserGroupEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/AVA1/8VA3
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
		///	designer:fld/AVA1/8VA5
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
		///	designer:fld/AVA1/8VA7
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
		///	designer:fld/AVA1/8VA8
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
		///	The <c>Disabled</c> field.
		///	designer:fld/AVA1/AVA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[AVA3]")]
		public bool Disabled
		{
			get
			{
				return this.GetField<bool> ("[AVA3]");
			}
			set
			{
				bool oldValue = this.Disabled;
				if (oldValue != value || !this.IsFieldDefined("[AVA3]"))
				{
					this.OnDisabledChanging (oldValue, value);
					this.SetField<bool> ("[AVA3]", oldValue, value);
					this.OnDisabledChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Roles</c> field.
		///	designer:fld/AVA1/AVA4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[AVA4]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.SoftwareUserRoleEntity> Roles
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.SoftwareUserRoleEntity> ("[AVA4]");
			}
		}
		///	<summary>
		///	The <c>UserPowerLevel</c> field.
		///	designer:fld/AVA1/AVA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[AVA5]")]
		public global::Epsitec.Cresus.Core.Business.UserManagement.UserPowerLevel UserPowerLevel
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.UserManagement.UserPowerLevel> ("[AVA5]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.UserManagement.UserPowerLevel oldValue = this.UserPowerLevel;
				if (oldValue != value || !this.IsFieldDefined("[AVA5]"))
				{
					this.OnUserPowerLevelChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.UserManagement.UserPowerLevel> ("[AVA5]", oldValue, value);
					this.OnUserPowerLevelChanged (oldValue, value);
				}
			}
		}
		
		partial void OnDisabledChanging(bool oldValue, bool newValue);
		partial void OnDisabledChanged(bool oldValue, bool newValue);
		partial void OnUserPowerLevelChanging(global::Epsitec.Cresus.Core.Business.UserManagement.UserPowerLevel oldValue, global::Epsitec.Cresus.Core.Business.UserManagement.UserPowerLevel newValue);
		partial void OnUserPowerLevelChanged(global::Epsitec.Cresus.Core.Business.UserManagement.UserPowerLevel oldValue, global::Epsitec.Cresus.Core.Business.UserManagement.UserPowerLevel newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.SoftwareUserGroupEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.SoftwareUserGroupEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1002, 10, 1);	// [AVA1]
		public static readonly string EntityStructuredTypeKey = "[AVA1]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<SoftwareUserGroupEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.SoftwareUserRole Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>SoftwareUserRole</c> entity.
	///	designer:cap/AVA2
	///	</summary>
	public partial class SoftwareUserRoleEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/AVA2/8VA3
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
		///	designer:fld/AVA2/8VA5
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
		///	designer:fld/AVA2/8VA7
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
		///	designer:fld/AVA2/8VA8
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
			return global::Epsitec.Cresus.Core.Entities.SoftwareUserRoleEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.SoftwareUserRoleEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1002, 10, 2);	// [AVA2]
		public static readonly string EntityStructuredTypeKey = "[AVA2]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<SoftwareUserRoleEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

