//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Core
{
	public class Hack
	{
		public void PopulateUsers()
		{
			var role = this.DataContext.CreateEntity<SoftwareUserRoleEntity> ();

			role.Code = "?";
			role.Name = "Principal";

			var groupSystem     = this.CreateUserGroup (role, "Système", Business.UserManagement.UserPowerLevel.System);
			var groupDev        = this.CreateUserGroup (role, "Développeurs", Business.UserManagement.UserPowerLevel.Developer);
			var groupAdmin      = this.CreateUserGroup (role, "Administrateurs", Business.UserManagement.UserPowerLevel.Administrator);
			var groupPowerUser  = this.CreateUserGroup (role, "Utilisateurs avec pouvoir", Business.UserManagement.UserPowerLevel.PowerUser);
			var groupStandard   = this.CreateUserGroup (role, "Utilisateurs standards", Business.UserManagement.UserPowerLevel.Standard);
			var groupRestricted = this.CreateUserGroup (role, "Utilisateurs restreints", Business.UserManagement.UserPowerLevel.Restricted);

#if false
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 1", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 2", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 3", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 4", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 5", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 6", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 7", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 8", Business.UserManagement.UserPowerLevel.Restricted);
#endif

			var userStandard1 = this.CreateUser (groupDev, "Pierre Arnaud", "arnaud", "smaky", Business.UserManagement.UserAuthenticationMethod.System);
			var userStandard2 = this.CreateUser (groupDev, "Marc Bettex", "Marc", "tiger", Business.UserManagement.UserAuthenticationMethod.System);
			var userStandard3 = this.CreateUser (groupDev, "Daniel Roux", "Daniel", "blupi", Business.UserManagement.UserAuthenticationMethod.System);
			var userEpsitec   = this.CreateUser (groupDev, "Epsitec", "Epsitec", "admin", Business.UserManagement.UserAuthenticationMethod.Password);

			userStandard1.UserGroups.Add (groupStandard);
			userStandard2.UserGroups.Add (groupStandard);
			userStandard3.UserGroups.Add (groupStandard);
			userEpsitec.UserGroups.Add (groupAdmin);

			this.DataContext.SaveChanges ();
		}

		private SoftwareUserGroupEntity CreateUserGroup(SoftwareUserRoleEntity role, string name, Business.UserManagement.UserPowerLevel level)
		{
			var group = this.DataContext.CreateEntity<SoftwareUserGroupEntity> ();
			var logic = new Logic (group, null);

			logic.ApplyRules (RuleType.Setup, group);

			group.Code           = "?";
			group.Name           = name;
			group.UserPowerLevel = level;
			group.Roles.Add (role);

			return group;
		}

		private SoftwareUserEntity CreateUser(SoftwareUserGroupEntity group, FormattedText displayName, string userLogin, string userPassword, Business.UserManagement.UserAuthenticationMethod am)
		{
			var user = this.DataContext.CreateEntity<SoftwareUserEntity> ();
			var logic = new Logic (user, null);

			logic.ApplyRules (RuleType.Setup, user);

			user.AuthenticationMethod = am;
			user.DisplayName = displayName;
			user.LoginName = userLogin;
			user.UserGroups.Add (group);
			user.SetPassword (userPassword);

			FormattedText[] p = displayName.Split (" ");
			if (p.Length == 2)
			{
				var person = this.SearchNaturalPerson (p[0].ToString (), p[1].ToString ());
				if (person.IsNotNull ())
				{
					user.Person = person;
				}
			}

			return user;
		}

		private NaturalPersonEntity SearchNaturalPerson(string firstName, string lastName)
		{
			var example = new NaturalPersonEntity ();
			example.Firstname = firstName;
			example.Lastname = lastName;

			return this.DataContext.GetByExample<NaturalPersonEntity> (example).FirstOrDefault ();
		}

	}
}
#region Epsitec.Cresus.Core.SoftwareUser Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>SoftwareUser</c> entity.
	///	designer:cap/L0AGF
	///	</summary>
	public partial class SoftwareUserEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.IDateTimeRange, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IItemCode
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/L0AGF/L0AB5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AB5]")]
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
		///	designer:fld/L0AGF/L0AD3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AD3]")]
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
		///	designer:fld/L0AGF/L0AU4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AU4]")]
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
		///	designer:fld/L0AGF/L0AV4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AV4]")]
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
		///	designer:fld/L0AGF/L0AHF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AHF]")]
		public global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity Person
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity> ("[L0AHF]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity oldValue = this.Person;
				if (oldValue != value || !this.IsFieldDefined ("[L0AHF]"))
				{
					this.OnPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity> ("[L0AHF]", oldValue, value);
					this.OnPersonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DisplayName</c> field.
		///	designer:fld/L0AGF/L0ASG
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0ASG]")]
		public global::Epsitec.Common.Types.FormattedText DisplayName
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[L0ASG]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.DisplayName;
				if (oldValue != value || !this.IsFieldDefined ("[L0ASG]"))
				{
					this.OnDisplayNameChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[L0ASG]", oldValue, value);
					this.OnDisplayNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LoginPicture</c> field.
		///	designer:fld/L0AGF/L0A8O
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A8O]")]
		public global::Epsitec.Cresus.Core.Entities.ImageEntity LoginPicture
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.ImageEntity> ("[L0A8O]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ImageEntity oldValue = this.LoginPicture;
				if (oldValue != value || !this.IsFieldDefined ("[L0A8O]"))
				{
					this.OnLoginPictureChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.ImageEntity> ("[L0A8O]", oldValue, value);
					this.OnLoginPictureChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LoginName</c> field.
		///	designer:fld/L0AGF/L0AIF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AIF]")]
		public string LoginName
		{
			get
			{
				return this.GetField<string> ("[L0AIF]");
			}
			set
			{
				string oldValue = this.LoginName;
				if (oldValue != value || !this.IsFieldDefined ("[L0AIF]"))
				{
					this.OnLoginNameChanging (oldValue, value);
					this.SetField<string> ("[L0AIF]", oldValue, value);
					this.OnLoginNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LoginPasswordHash</c> field.
		///	designer:fld/L0AGF/L0AJF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AJF]")]
		public string LoginPasswordHash
		{
			get
			{
				return this.GetField<string> ("[L0AJF]");
			}
			set
			{
				string oldValue = this.LoginPasswordHash;
				if (oldValue != value || !this.IsFieldDefined ("[L0AJF]"))
				{
					this.OnLoginPasswordHashChanging (oldValue, value);
					this.SetField<string> ("[L0AJF]", oldValue, value);
					this.OnLoginPasswordHashChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AuthenticationMethod</c> field.
		///	designer:fld/L0AGF/L0APG
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0APG]")]
		public global::Epsitec.Cresus.Core.Business.UserManagement.UserAuthenticationMethod? AuthenticationMethod
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.UserManagement.UserAuthenticationMethod?> ("[L0APG]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.UserManagement.UserAuthenticationMethod? oldValue = this.AuthenticationMethod;
				if (oldValue != value || !this.IsFieldDefined ("[L0APG]"))
				{
					this.OnAuthenticationMethodChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.UserManagement.UserAuthenticationMethod?> ("[L0APG]", oldValue, value);
					this.OnAuthenticationMethodChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>UserGroups</c> field.
		///	designer:fld/L0AGF/L0ANF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0ANF]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.SoftwareUserGroupEntity> UserGroups
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.SoftwareUserGroupEntity> ("[L0ANF]");
			}
		}
		///	<summary>
		///	The <c>Disabled</c> field.
		///	designer:fld/L0AGF/L0AOF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AOF]")]
		public bool Disabled
		{
			get
			{
				return this.GetField<bool> ("[L0AOF]");
			}
			set
			{
				bool oldValue = this.Disabled;
				if (oldValue != value || !this.IsFieldDefined ("[L0AOF]"))
				{
					this.OnDisabledChanging (oldValue, value);
					this.SetField<bool> ("[L0AOF]", oldValue, value);
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
		partial void OnAuthenticationMethodChanging(global::Epsitec.Cresus.Core.Business.UserManagement.UserAuthenticationMethod? oldValue, global::Epsitec.Cresus.Core.Business.UserManagement.UserAuthenticationMethod? newValue);
		partial void OnAuthenticationMethodChanged(global::Epsitec.Cresus.Core.Business.UserManagement.UserAuthenticationMethod? oldValue, global::Epsitec.Cresus.Core.Business.UserManagement.UserAuthenticationMethod? newValue);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 496);	// [L0AGF]
		public static readonly new string EntityStructuredTypeKey = "[L0AGF]";
	}
}
#endregion

#region Epsitec.Cresus.Core.SoftwareUserGroup Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>SoftwareUserGroup</c> entity.
	///	designer:cap/L0AKF
	///	</summary>
	public partial class SoftwareUserGroupEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IItemCode, global::Epsitec.Cresus.Core.Entities.INameDescription
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/L0AKF/L0AB5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AB5]")]
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
		///	designer:fld/L0AKF/L0AD3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AD3]")]
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
		///	designer:fld/L0AKF/L0AUN
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AUN]")]
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
		///	designer:fld/L0AKF/L0AVN
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AVN]")]
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
		///	designer:fld/L0AKF/L0APF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0APF]")]
		public bool Disabled
		{
			get
			{
				return this.GetField<bool> ("[L0APF]");
			}
			set
			{
				bool oldValue = this.Disabled;
				if (oldValue != value || !this.IsFieldDefined ("[L0APF]"))
				{
					this.OnDisabledChanging (oldValue, value);
					this.SetField<bool> ("[L0APF]", oldValue, value);
					this.OnDisabledChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Roles</c> field.
		///	designer:fld/L0AKF/L0ATF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0ATF]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.SoftwareUserRoleEntity> Roles
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.SoftwareUserRoleEntity> ("[L0ATF]");
			}
		}
		///	<summary>
		///	The <c>UserPowerLevel</c> field.
		///	designer:fld/L0AKF/L0AQG
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AQG]")]
		public global::Epsitec.Cresus.Core.Business.UserManagement.UserPowerLevel UserPowerLevel
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.UserManagement.UserPowerLevel> ("[L0AQG]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.UserManagement.UserPowerLevel oldValue = this.UserPowerLevel;
				if (oldValue != value || !this.IsFieldDefined ("[L0AQG]"))
				{
					this.OnUserPowerLevelChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.UserManagement.UserPowerLevel> ("[L0AQG]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 500);	// [L0AKF]
		public static readonly new string EntityStructuredTypeKey = "[L0AKF]";
	}
}
#endregion

#region Epsitec.Cresus.Core.SoftwareUserRole Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>SoftwareUserRole</c> entity.
	///	designer:cap/L0AQF
	///	</summary>
	public partial class SoftwareUserRoleEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IItemCode, global::Epsitec.Cresus.Core.Entities.INameDescription
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/L0AQF/L0AB5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AB5]")]
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
		///	designer:fld/L0AQF/L0AD3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AD3]")]
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
		///	designer:fld/L0AQF/L0AUN
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AUN]")]
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
		///	designer:fld/L0AQF/L0AVN
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AVN]")]
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 506);	// [L0AQF]
		public static readonly new string EntityStructuredTypeKey = "[L0AQF]";
	}
}
#endregion
