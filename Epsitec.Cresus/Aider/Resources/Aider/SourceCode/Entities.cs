//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA]", typeof (Epsitec.Aider.Entities.eCH_PersonEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAF]", typeof (Epsitec.Aider.Entities.AiderPersonEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAG]", typeof (Epsitec.Aider.Entities.eCH_ReportedPersonEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA22]", typeof (Epsitec.Aider.Entities.eCH_AddressEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAI2]", typeof (Epsitec.Aider.Entities.AiderHouseholdEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAJ2]", typeof (Epsitec.Aider.Entities.AiderAddressEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAV2]", typeof (Epsitec.Aider.Entities.AiderPersonRelationshipEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA73]", typeof (Epsitec.Aider.Entities.AiderGroupParticipantEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA93]", typeof (Epsitec.Aider.Entities.AiderEventEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAA3]", typeof (Epsitec.Aider.Entities.AiderEventParticipantEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAV3]", typeof (Epsitec.Aider.Entities.AiderPlaceEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA54]", typeof (Epsitec.Aider.Entities.AiderGroupEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAN4]", typeof (Epsitec.Aider.Entities.AiderGroupRelationshipEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA65]", typeof (Epsitec.Aider.Entities.AiderTownEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAB5]", typeof (Epsitec.Aider.Entities.AiderCountryEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAF5]", typeof (Epsitec.Aider.Entities.AiderGroupPlaceEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAQ5]", typeof (Epsitec.Aider.Entities.AiderPlacePersonEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAR6]", typeof (Epsitec.Aider.Entities.AiderLegalPersonEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAS7]", typeof (Epsitec.Aider.Entities.AiderDataManagerEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAV7]", typeof (Epsitec.Aider.Entities.SoftwareSessionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA18]", typeof (Epsitec.Aider.Entities.SoftwareMutationLogEntryEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA28]", typeof (Epsitec.Aider.Entities.AiderDataEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA48]", typeof (Epsitec.Aider.Entities.AiderGroupEventEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA78]", typeof (Epsitec.Aider.Entities.AiderCommentEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAL8]", typeof (Epsitec.Aider.Entities.AiderPersonDataEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA2A]", typeof (Epsitec.Aider.Entities.AiderGroupDefEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA7A]", typeof (Epsitec.Aider.Entities.AiderFunctionDefEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAFB]", typeof (Epsitec.Aider.Entities.AiderWarningActionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVALB]", typeof (Epsitec.Aider.Entities.AiderPersonWarningEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVACC]", typeof (Epsitec.Aider.Entities.AiderUserScopeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAHC]", typeof (Epsitec.Aider.Entities.AiderUserEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAKC]", typeof (Epsitec.Aider.Entities.AiderUserRoleEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVARD]", typeof (Epsitec.Aider.Entities.AiderContactEntity))]
#region Epsitec.Aider.eCH_Person Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>eCH_Person</c> entity.
	///	designer:cap/LVA
	///	</summary>
	public partial class eCH_PersonEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>CreationDate</c> field.
		///	designer:fld/LVA/LVAI6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAI6]")]
		public global::Epsitec.Common.Types.Date CreationDate
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date> ("[LVAI6]");
			}
			set
			{
				global::Epsitec.Common.Types.Date oldValue = this.CreationDate;
				if (oldValue != value || !this.IsFieldDefined("[LVAI6]"))
				{
					this.OnCreationDateChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date> ("[LVAI6]", oldValue, value);
					this.OnCreationDateChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ReportedPerson1</c> field.
		///	designer:fld/LVA/LVAG2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAG2]")]
		public global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity ReportedPerson1
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity> ("[LVAG2]");
			}
			set
			{
				global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity oldValue = this.ReportedPerson1;
				if (oldValue != value || !this.IsFieldDefined("[LVAG2]"))
				{
					this.OnReportedPerson1Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity> ("[LVAG2]", oldValue, value);
					this.OnReportedPerson1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ReportedPerson2</c> field.
		///	designer:fld/LVA/LVAH2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAH2]")]
		public global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity ReportedPerson2
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity> ("[LVAH2]");
			}
			set
			{
				global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity oldValue = this.ReportedPerson2;
				if (oldValue != value || !this.IsFieldDefined("[LVAH2]"))
				{
					this.OnReportedPerson2Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity> ("[LVAH2]", oldValue, value);
					this.OnReportedPerson2Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DataSource</c> field.
		///	designer:fld/LVA/LVAA
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA]")]
		public global::Epsitec.Aider.Enumerations.DataSource DataSource
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.DataSource> ("[LVAA]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.DataSource oldValue = this.DataSource;
				if (oldValue != value || !this.IsFieldDefined("[LVAA]"))
				{
					this.OnDataSourceChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.DataSource> ("[LVAA]", oldValue, value);
					this.OnDataSourceChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DeclarationStatus</c> field.
		///	designer:fld/LVA/LVAS2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAS2]")]
		public global::Epsitec.Aider.Enumerations.PersonDeclarationStatus DeclarationStatus
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.PersonDeclarationStatus> ("[LVAS2]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.PersonDeclarationStatus oldValue = this.DeclarationStatus;
				if (oldValue != value || !this.IsFieldDefined("[LVAS2]"))
				{
					this.OnDeclarationStatusChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.PersonDeclarationStatus> ("[LVAS2]", oldValue, value);
					this.OnDeclarationStatusChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>RemovalReason</c> field.
		///	designer:fld/LVA/LVAQ6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAQ6]")]
		public global::Epsitec.Aider.Enumerations.RemovalReason RemovalReason
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.RemovalReason> ("[LVAQ6]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.RemovalReason oldValue = this.RemovalReason;
				if (oldValue != value || !this.IsFieldDefined("[LVAQ6]"))
				{
					this.OnRemovalReasonChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.RemovalReason> ("[LVAQ6]", oldValue, value);
					this.OnRemovalReasonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PersonId</c> field.
		///	designer:fld/LVA/LVA1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA1]")]
		public string PersonId
		{
			get
			{
				return this.GetField<string> ("[LVA1]");
			}
			set
			{
				string oldValue = this.PersonId;
				if (oldValue != value || !this.IsFieldDefined("[LVA1]"))
				{
					this.OnPersonIdChanging (oldValue, value);
					this.SetField<string> ("[LVA1]", oldValue, value);
					this.OnPersonIdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PersonOfficialName</c> field.
		///	designer:fld/LVA/LVA2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA2]")]
		public string PersonOfficialName
		{
			get
			{
				return this.GetField<string> ("[LVA2]");
			}
			set
			{
				string oldValue = this.PersonOfficialName;
				if (oldValue != value || !this.IsFieldDefined("[LVA2]"))
				{
					this.OnPersonOfficialNameChanging (oldValue, value);
					this.SetField<string> ("[LVA2]", oldValue, value);
					this.OnPersonOfficialNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PersonFirstNames</c> field.
		///	designer:fld/LVA/LVA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA3]")]
		public string PersonFirstNames
		{
			get
			{
				return this.GetField<string> ("[LVA3]");
			}
			set
			{
				string oldValue = this.PersonFirstNames;
				if (oldValue != value || !this.IsFieldDefined("[LVA3]"))
				{
					this.OnPersonFirstNamesChanging (oldValue, value);
					this.SetField<string> ("[LVA3]", oldValue, value);
					this.OnPersonFirstNamesChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PersonDateOfBirth</c> field.
		///	designer:fld/LVA/LVA8B
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA8B]")]
		public global::Epsitec.Common.Types.Date? PersonDateOfBirth
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[LVA8B]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.PersonDateOfBirth;
				if (oldValue != value || !this.IsFieldDefined("[LVA8B]"))
				{
					this.OnPersonDateOfBirthChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[LVA8B]", oldValue, value);
					this.OnPersonDateOfBirthChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PersonDateOfDeath</c> field.
		///	designer:fld/LVA/LVA9B
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA9B]")]
		public global::Epsitec.Common.Types.Date? PersonDateOfDeath
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[LVA9B]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.PersonDateOfDeath;
				if (oldValue != value || !this.IsFieldDefined("[LVA9B]"))
				{
					this.OnPersonDateOfDeathChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[LVA9B]", oldValue, value);
					this.OnPersonDateOfDeathChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PersonSex</c> field.
		///	designer:fld/LVA/LVA6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA6]")]
		public global::Epsitec.Aider.Enumerations.PersonSex PersonSex
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.PersonSex> ("[LVA6]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.PersonSex oldValue = this.PersonSex;
				if (oldValue != value || !this.IsFieldDefined("[LVA6]"))
				{
					this.OnPersonSexChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.PersonSex> ("[LVA6]", oldValue, value);
					this.OnPersonSexChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>NationalityStatus</c> field.
		///	designer:fld/LVA/LVA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA8]")]
		public global::Epsitec.Aider.Enumerations.PersonNationalityStatus NationalityStatus
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.PersonNationalityStatus> ("[LVA8]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.PersonNationalityStatus oldValue = this.NationalityStatus;
				if (oldValue != value || !this.IsFieldDefined("[LVA8]"))
				{
					this.OnNationalityStatusChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.PersonNationalityStatus> ("[LVA8]", oldValue, value);
					this.OnNationalityStatusChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>NationalityCountryCode</c> field.
		///	designer:fld/LVA/LVA7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA7]")]
		public string NationalityCountryCode
		{
			get
			{
				return this.GetField<string> ("[LVA7]");
			}
			set
			{
				string oldValue = this.NationalityCountryCode;
				if (oldValue != value || !this.IsFieldDefined("[LVA7]"))
				{
					this.OnNationalityCountryCodeChanging (oldValue, value);
					this.SetField<string> ("[LVA7]", oldValue, value);
					this.OnNationalityCountryCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Nationality</c> field.
		///	designer:fld/LVA/LVACB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVACB]", IsVirtual=true)]
		public global::Epsitec.Aider.Entities.AiderCountryEntity Nationality
		{
			get
			{
				global::Epsitec.Aider.Entities.AiderCountryEntity value = default (global::Epsitec.Aider.Entities.AiderCountryEntity);
				this.GetNationality (ref value);
				return value;
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderCountryEntity oldValue = this.Nationality;
				if (oldValue != value || !this.IsFieldDefined("[LVACB]"))
				{
					this.OnNationalityChanging (oldValue, value);
					this.SetNationality (value);
					this.OnNationalityChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Origins</c> field.
		///	designer:fld/LVA/LVAB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAB]")]
		public string Origins
		{
			get
			{
				return this.GetField<string> ("[LVAB]");
			}
			set
			{
				string oldValue = this.Origins;
				if (oldValue != value || !this.IsFieldDefined("[LVAB]"))
				{
					this.OnOriginsChanging (oldValue, value);
					this.SetField<string> ("[LVAB]", oldValue, value);
					this.OnOriginsChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AdultMaritalStatus</c> field.
		///	designer:fld/LVA/LVA9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA9]")]
		public global::Epsitec.Aider.Enumerations.PersonMaritalStatus AdultMaritalStatus
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.PersonMaritalStatus> ("[LVA9]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.PersonMaritalStatus oldValue = this.AdultMaritalStatus;
				if (oldValue != value || !this.IsFieldDefined("[LVA9]"))
				{
					this.OnAdultMaritalStatusChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.PersonMaritalStatus> ("[LVA9]", oldValue, value);
					this.OnAdultMaritalStatusChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Address1</c> field.
		///	designer:fld/LVA/LVAC2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAC2]")]
		public global::Epsitec.Aider.Entities.eCH_AddressEntity Address1
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.eCH_AddressEntity> ("[LVAC2]");
			}
			set
			{
				global::Epsitec.Aider.Entities.eCH_AddressEntity oldValue = this.Address1;
				if (oldValue != value || !this.IsFieldDefined("[LVAC2]"))
				{
					this.OnAddress1Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.eCH_AddressEntity> ("[LVAC2]", oldValue, value);
					this.OnAddress1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Address2</c> field.
		///	designer:fld/LVA/LVG802
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVG802]")]
		public global::Epsitec.Aider.Entities.eCH_AddressEntity Address2
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.eCH_AddressEntity> ("[LVG802]");
			}
			set
			{
				global::Epsitec.Aider.Entities.eCH_AddressEntity oldValue = this.Address2;
				if (oldValue != value || !this.IsFieldDefined("[LVG802]"))
				{
					this.OnAddress2Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.eCH_AddressEntity> ("[LVG802]", oldValue, value);
					this.OnAddress2Changed (oldValue, value);
				}
			}
		}
		
		partial void OnCreationDateChanging(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnCreationDateChanged(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnReportedPerson1Changing(global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity oldValue, global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity newValue);
		partial void OnReportedPerson1Changed(global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity oldValue, global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity newValue);
		partial void OnReportedPerson2Changing(global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity oldValue, global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity newValue);
		partial void OnReportedPerson2Changed(global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity oldValue, global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity newValue);
		partial void OnDataSourceChanging(global::Epsitec.Aider.Enumerations.DataSource oldValue, global::Epsitec.Aider.Enumerations.DataSource newValue);
		partial void OnDataSourceChanged(global::Epsitec.Aider.Enumerations.DataSource oldValue, global::Epsitec.Aider.Enumerations.DataSource newValue);
		partial void OnDeclarationStatusChanging(global::Epsitec.Aider.Enumerations.PersonDeclarationStatus oldValue, global::Epsitec.Aider.Enumerations.PersonDeclarationStatus newValue);
		partial void OnDeclarationStatusChanged(global::Epsitec.Aider.Enumerations.PersonDeclarationStatus oldValue, global::Epsitec.Aider.Enumerations.PersonDeclarationStatus newValue);
		partial void OnRemovalReasonChanging(global::Epsitec.Aider.Enumerations.RemovalReason oldValue, global::Epsitec.Aider.Enumerations.RemovalReason newValue);
		partial void OnRemovalReasonChanged(global::Epsitec.Aider.Enumerations.RemovalReason oldValue, global::Epsitec.Aider.Enumerations.RemovalReason newValue);
		partial void OnPersonIdChanging(string oldValue, string newValue);
		partial void OnPersonIdChanged(string oldValue, string newValue);
		partial void OnPersonOfficialNameChanging(string oldValue, string newValue);
		partial void OnPersonOfficialNameChanged(string oldValue, string newValue);
		partial void OnPersonFirstNamesChanging(string oldValue, string newValue);
		partial void OnPersonFirstNamesChanged(string oldValue, string newValue);
		partial void OnPersonDateOfBirthChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnPersonDateOfBirthChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnPersonDateOfDeathChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnPersonDateOfDeathChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnPersonSexChanging(global::Epsitec.Aider.Enumerations.PersonSex oldValue, global::Epsitec.Aider.Enumerations.PersonSex newValue);
		partial void OnPersonSexChanged(global::Epsitec.Aider.Enumerations.PersonSex oldValue, global::Epsitec.Aider.Enumerations.PersonSex newValue);
		partial void OnNationalityStatusChanging(global::Epsitec.Aider.Enumerations.PersonNationalityStatus oldValue, global::Epsitec.Aider.Enumerations.PersonNationalityStatus newValue);
		partial void OnNationalityStatusChanged(global::Epsitec.Aider.Enumerations.PersonNationalityStatus oldValue, global::Epsitec.Aider.Enumerations.PersonNationalityStatus newValue);
		partial void OnNationalityCountryCodeChanging(string oldValue, string newValue);
		partial void OnNationalityCountryCodeChanged(string oldValue, string newValue);
		partial void OnNationalityChanging(global::Epsitec.Aider.Entities.AiderCountryEntity oldValue, global::Epsitec.Aider.Entities.AiderCountryEntity newValue);
		partial void OnNationalityChanged(global::Epsitec.Aider.Entities.AiderCountryEntity oldValue, global::Epsitec.Aider.Entities.AiderCountryEntity newValue);
		partial void OnOriginsChanging(string oldValue, string newValue);
		partial void OnOriginsChanged(string oldValue, string newValue);
		partial void OnAdultMaritalStatusChanging(global::Epsitec.Aider.Enumerations.PersonMaritalStatus oldValue, global::Epsitec.Aider.Enumerations.PersonMaritalStatus newValue);
		partial void OnAdultMaritalStatusChanged(global::Epsitec.Aider.Enumerations.PersonMaritalStatus oldValue, global::Epsitec.Aider.Enumerations.PersonMaritalStatus newValue);
		partial void OnAddress1Changing(global::Epsitec.Aider.Entities.eCH_AddressEntity oldValue, global::Epsitec.Aider.Entities.eCH_AddressEntity newValue);
		partial void OnAddress1Changed(global::Epsitec.Aider.Entities.eCH_AddressEntity oldValue, global::Epsitec.Aider.Entities.eCH_AddressEntity newValue);
		partial void OnAddress2Changing(global::Epsitec.Aider.Entities.eCH_AddressEntity oldValue, global::Epsitec.Aider.Entities.eCH_AddressEntity newValue);
		partial void OnAddress2Changed(global::Epsitec.Aider.Entities.eCH_AddressEntity oldValue, global::Epsitec.Aider.Entities.eCH_AddressEntity newValue);
		
		partial void GetNationality(ref global::Epsitec.Aider.Entities.AiderCountryEntity value);
		partial void SetNationality(global::Epsitec.Aider.Entities.AiderCountryEntity value);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.eCH_PersonEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.eCH_PersonEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 0);	// [LVA]
		public static readonly string EntityStructuredTypeKey = "[LVA]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<eCH_PersonEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderPerson Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderPerson</c> entity.
	///	designer:cap/LVAF
	///	</summary>
	public partial class AiderPersonEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IManagedItem, global::Epsitec.Aider.Entities.IComment
	{
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVAF/LVAA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA8]")]
		public global::Epsitec.Aider.Entities.AiderCommentEntity Comment
		{
			get
			{
				return global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.GetComment (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.SetComment (this, value);
			}
		}
		#endregion
		#region IManagedItem Members
		///	<summary>
		///	The <c>DataManager</c> field.
		///	designer:fld/LVAF/LVAT7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAT7]")]
		public global::Epsitec.Aider.Entities.AiderDataManagerEntity DataManager
		{
			get
			{
				return global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.GetDataManager (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.SetDataManager (this, value);
			}
		}
		///	<summary>
		///	The <c>ValidationState</c> field.
		///	designer:fld/LVAF/LVAU7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAU7]")]
		public global::Epsitec.Aider.Enumerations.ValidationState ValidationState
		{
			get
			{
				return global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.GetValidationState (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.SetValidationState (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>eCH_Person</c> field.
		///	designer:fld/LVAF/LVAU1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAU1]")]
		public global::Epsitec.Aider.Entities.eCH_PersonEntity eCH_Person
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.eCH_PersonEntity> ("[LVAU1]");
			}
			set
			{
				global::Epsitec.Aider.Entities.eCH_PersonEntity oldValue = this.eCH_Person;
				if (oldValue != value || !this.IsFieldDefined("[LVAU1]"))
				{
					this.OneCH_PersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.eCH_PersonEntity> ("[LVAU1]", oldValue, value);
					this.OneCH_PersonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CodeId</c> field.
		///	designer:fld/LVAF/LVAM2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAM2]")]
		public string CodeId
		{
			get
			{
				return this.GetField<string> ("[LVAM2]");
			}
			set
			{
				string oldValue = this.CodeId;
				if (oldValue != value || !this.IsFieldDefined("[LVAM2]"))
				{
					this.OnCodeIdChanging (oldValue, value);
					this.SetField<string> ("[LVAM2]", oldValue, value);
					this.OnCodeIdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MrMrs</c> field.
		///	designer:fld/LVAF/LVAT
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAT]")]
		public global::Epsitec.Aider.Enumerations.PersonMrMrs MrMrs
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.PersonMrMrs> ("[LVAT]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.PersonMrMrs oldValue = this.MrMrs;
				if (oldValue != value || !this.IsFieldDefined("[LVAT]"))
				{
					this.OnMrMrsChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.PersonMrMrs> ("[LVAT]", oldValue, value);
					this.OnMrMrsChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Title</c> field.
		///	designer:fld/LVAF/LVAU
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAU]")]
		public string Title
		{
			get
			{
				return this.GetField<string> ("[LVAU]");
			}
			set
			{
				string oldValue = this.Title;
				if (oldValue != value || !this.IsFieldDefined("[LVAU]"))
				{
					this.OnTitleChanging (oldValue, value);
					this.SetField<string> ("[LVAU]", oldValue, value);
					this.OnTitleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CallName</c> field.
		///	designer:fld/LVAF/LVAK2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAK2]")]
		public string CallName
		{
			get
			{
				return this.GetField<string> ("[LVAK2]");
			}
			set
			{
				string oldValue = this.CallName;
				if (oldValue != value || !this.IsFieldDefined("[LVAK2]"))
				{
					this.OnCallNameChanging (oldValue, value);
					this.SetField<string> ("[LVAK2]", oldValue, value);
					this.OnCallNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>OriginalName</c> field.
		///	designer:fld/LVAF/LVAL2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAL2]")]
		public string OriginalName
		{
			get
			{
				return this.GetField<string> ("[LVAL2]");
			}
			set
			{
				string oldValue = this.OriginalName;
				if (oldValue != value || !this.IsFieldDefined("[LVAL2]"))
				{
					this.OnOriginalNameChanging (oldValue, value);
					this.SetField<string> ("[LVAL2]", oldValue, value);
					this.OnOriginalNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DisplayName</c> field.
		///	designer:fld/LVAF/LVAI4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAI4]")]
		public string DisplayName
		{
			get
			{
				return this.GetField<string> ("[LVAI4]");
			}
			set
			{
				string oldValue = this.DisplayName;
				if (oldValue != value || !this.IsFieldDefined("[LVAI4]"))
				{
					this.OnDisplayNameChanging (oldValue, value);
					this.SetField<string> ("[LVAI4]", oldValue, value);
					this.OnDisplayNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Language</c> field.
		///	designer:fld/LVAF/LVAO7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAO7]")]
		public global::Epsitec.Aider.Enumerations.Language Language
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.Language> ("[LVAO7]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.Language oldValue = this.Language;
				if (oldValue != value || !this.IsFieldDefined("[LVAO7]"))
				{
					this.OnLanguageChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.Language> ("[LVAO7]", oldValue, value);
					this.OnLanguageChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Confession</c> field.
		///	designer:fld/LVAF/LVAI5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAI5]")]
		public global::Epsitec.Aider.Enumerations.PersonConfession Confession
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.PersonConfession> ("[LVAI5]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.PersonConfession oldValue = this.Confession;
				if (oldValue != value || !this.IsFieldDefined("[LVAI5]"))
				{
					this.OnConfessionChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.PersonConfession> ("[LVAI5]", oldValue, value);
					this.OnConfessionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Visibility</c> field.
		///	designer:fld/LVAF/LVAVE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAVE]")]
		public global::Epsitec.Aider.Enumerations.PersonVisibilityStatus Visibility
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.PersonVisibilityStatus> ("[LVAVE]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.PersonVisibilityStatus oldValue = this.Visibility;
				if (oldValue != value || !this.IsFieldDefined("[LVAVE]"))
				{
					this.OnVisibilityChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.PersonVisibilityStatus> ("[LVAVE]", oldValue, value);
					this.OnVisibilityChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Profession</c> field.
		///	designer:fld/LVAF/LVAP7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAP7]")]
		public string Profession
		{
			get
			{
				return this.GetField<string> ("[LVAP7]");
			}
			set
			{
				string oldValue = this.Profession;
				if (oldValue != value || !this.IsFieldDefined("[LVAP7]"))
				{
					this.OnProfessionChanging (oldValue, value);
					this.SetField<string> ("[LVAP7]", oldValue, value);
					this.OnProfessionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>BirthdayDay</c> field.
		///	designer:fld/LVAF/LVANE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVANE]")]
		public int BirthdayDay
		{
			get
			{
				return this.GetField<int> ("[LVANE]");
			}
			set
			{
				int oldValue = this.BirthdayDay;
				if (oldValue != value || !this.IsFieldDefined("[LVANE]"))
				{
					this.OnBirthdayDayChanging (oldValue, value);
					this.SetField<int> ("[LVANE]", oldValue, value);
					this.OnBirthdayDayChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>BirthdayMonth</c> field.
		///	designer:fld/LVAF/LVAOE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAOE]")]
		public int BirthdayMonth
		{
			get
			{
				return this.GetField<int> ("[LVAOE]");
			}
			set
			{
				int oldValue = this.BirthdayMonth;
				if (oldValue != value || !this.IsFieldDefined("[LVAOE]"))
				{
					this.OnBirthdayMonthChanging (oldValue, value);
					this.SetField<int> ("[LVAOE]", oldValue, value);
					this.OnBirthdayMonthChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>BirthdayYear</c> field.
		///	designer:fld/LVAF/LVAPE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAPE]")]
		public int BirthdayYear
		{
			get
			{
				return this.GetField<int> ("[LVAPE]");
			}
			set
			{
				int oldValue = this.BirthdayYear;
				if (oldValue != value || !this.IsFieldDefined("[LVAPE]"))
				{
					this.OnBirthdayYearChanging (oldValue, value);
					this.SetField<int> ("[LVAPE]", oldValue, value);
					this.OnBirthdayYearChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Parish</c> field.
		///	designer:fld/LVAF/LVADB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVADB]")]
		public global::Epsitec.Aider.Entities.AiderGroupParticipantEntity Parish
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderGroupParticipantEntity> ("[LVADB]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderGroupParticipantEntity oldValue = this.Parish;
				if (oldValue != value || !this.IsFieldDefined("[LVADB]"))
				{
					this.OnParishChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderGroupParticipantEntity> ("[LVADB]", oldValue, value);
					this.OnParishChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ParishGroup</c> field.
		///	designer:fld/LVAF/LVG622
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVG622]", IsVirtual=true)]
		public global::Epsitec.Aider.Entities.AiderGroupEntity ParishGroup
		{
			get
			{
				global::Epsitec.Aider.Entities.AiderGroupEntity value = default (global::Epsitec.Aider.Entities.AiderGroupEntity);
				this.GetParishGroup (ref value);
				return value;
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderGroupEntity oldValue = this.ParishGroup;
				if (oldValue != value || !this.IsFieldDefined("[LVG622]"))
				{
					this.OnParishGroupChanging (oldValue, value);
					this.SetParishGroup (value);
					this.OnParishGroupChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Events</c> field.
		///	designer:fld/LVAF/LVAQ8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAQ8]", IsVirtual=true)]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderEventParticipantEntity> Events
		{
			get
			{
				global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderEventParticipantEntity> value = default (global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderEventParticipantEntity>);
				this.GetEvents (ref value);
				return value;
			}
		}
		///	<summary>
		///	The <c>Groups</c> field.
		///	designer:fld/LVAF/LVAR8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAR8]", IsVirtual=true)]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderGroupParticipantEntity> Groups
		{
			get
			{
				global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderGroupParticipantEntity> value = default (global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderGroupParticipantEntity>);
				this.GetGroups (ref value);
				return value;
			}
		}
		///	<summary>
		///	The <c>Data</c> field.
		///	designer:fld/LVAF/LVAS8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAS8]", IsVirtual=true)]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonDataEntity> Data
		{
			get
			{
				global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonDataEntity> value = default (global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonDataEntity>);
				this.GetData (ref value);
				return value;
			}
		}
		///	<summary>
		///	The <c>Children</c> field.
		///	designer:fld/LVAF/LVGL02
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVGL02]", IsVirtual=true)]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> Children
		{
			get
			{
				global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> value = default (global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity>);
				this.GetChildren (ref value);
				return value;
			}
		}
		///	<summary>
		///	The <c>Parents</c> field.
		///	designer:fld/LVAF/LVGM02
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVGM02]", IsVirtual=true)]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> Parents
		{
			get
			{
				global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> value = default (global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity>);
				this.GetParents (ref value);
				return value;
			}
		}
		///	<summary>
		///	The <c>Housemates</c> field.
		///	designer:fld/LVAF/LVGN02
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVGN02]", IsVirtual=true)]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> Housemates
		{
			get
			{
				global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> value = default (global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity>);
				this.GetHousemates (ref value);
				return value;
			}
		}
		///	<summary>
		///	The <c>Relationships</c> field.
		///	designer:fld/LVAF/LVAAB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAAB]", IsVirtual=true)]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonRelationshipEntity> Relationships
		{
			get
			{
				global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonRelationshipEntity> value = default (global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonRelationshipEntity>);
				this.GetRelationships (ref value);
				return value;
			}
		}
		///	<summary>
		///	The <c>Warnings</c> field.
		///	designer:fld/LVAF/LVAOB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAOB]", IsVirtual=true)]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonWarningEntity> Warnings
		{
			get
			{
				global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonWarningEntity> value = default (global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonWarningEntity>);
				this.GetWarnings (ref value);
				return value;
			}
		}
		///	<summary>
		///	The <c>Households</c> field.
		///	designer:fld/LVAF/LVALE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVALE]", IsVirtual=true)]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderHouseholdEntity> Households
		{
			get
			{
				global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderHouseholdEntity> value = default (global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderHouseholdEntity>);
				this.GetHouseholds (ref value);
				return value;
			}
		}
		///	<summary>
		///	The <c>Contacts</c> field.
		///	designer:fld/LVAF/LVAIE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAIE]", IsVirtual=true)]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderContactEntity> Contacts
		{
			get
			{
				global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderContactEntity> value = default (global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderContactEntity>);
				this.GetContacts (ref value);
				return value;
			}
		}
		///	<summary>
		///	The <c>AdditionalAddresses</c> field.
		///	designer:fld/LVAF/LVAME
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAME]", IsVirtual=true)]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderContactEntity> AdditionalAddresses
		{
			get
			{
				global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderContactEntity> value = default (global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderContactEntity>);
				this.GetAdditionalAddresses (ref value);
				return value;
			}
		}
		
		partial void OneCH_PersonChanging(global::Epsitec.Aider.Entities.eCH_PersonEntity oldValue, global::Epsitec.Aider.Entities.eCH_PersonEntity newValue);
		partial void OneCH_PersonChanged(global::Epsitec.Aider.Entities.eCH_PersonEntity oldValue, global::Epsitec.Aider.Entities.eCH_PersonEntity newValue);
		partial void OnCodeIdChanging(string oldValue, string newValue);
		partial void OnCodeIdChanged(string oldValue, string newValue);
		partial void OnMrMrsChanging(global::Epsitec.Aider.Enumerations.PersonMrMrs oldValue, global::Epsitec.Aider.Enumerations.PersonMrMrs newValue);
		partial void OnMrMrsChanged(global::Epsitec.Aider.Enumerations.PersonMrMrs oldValue, global::Epsitec.Aider.Enumerations.PersonMrMrs newValue);
		partial void OnTitleChanging(string oldValue, string newValue);
		partial void OnTitleChanged(string oldValue, string newValue);
		partial void OnCallNameChanging(string oldValue, string newValue);
		partial void OnCallNameChanged(string oldValue, string newValue);
		partial void OnOriginalNameChanging(string oldValue, string newValue);
		partial void OnOriginalNameChanged(string oldValue, string newValue);
		partial void OnDisplayNameChanging(string oldValue, string newValue);
		partial void OnDisplayNameChanged(string oldValue, string newValue);
		partial void OnLanguageChanging(global::Epsitec.Aider.Enumerations.Language oldValue, global::Epsitec.Aider.Enumerations.Language newValue);
		partial void OnLanguageChanged(global::Epsitec.Aider.Enumerations.Language oldValue, global::Epsitec.Aider.Enumerations.Language newValue);
		partial void OnConfessionChanging(global::Epsitec.Aider.Enumerations.PersonConfession oldValue, global::Epsitec.Aider.Enumerations.PersonConfession newValue);
		partial void OnConfessionChanged(global::Epsitec.Aider.Enumerations.PersonConfession oldValue, global::Epsitec.Aider.Enumerations.PersonConfession newValue);
		partial void OnVisibilityChanging(global::Epsitec.Aider.Enumerations.PersonVisibilityStatus oldValue, global::Epsitec.Aider.Enumerations.PersonVisibilityStatus newValue);
		partial void OnVisibilityChanged(global::Epsitec.Aider.Enumerations.PersonVisibilityStatus oldValue, global::Epsitec.Aider.Enumerations.PersonVisibilityStatus newValue);
		partial void OnProfessionChanging(string oldValue, string newValue);
		partial void OnProfessionChanged(string oldValue, string newValue);
		partial void OnBirthdayDayChanging(int oldValue, int newValue);
		partial void OnBirthdayDayChanged(int oldValue, int newValue);
		partial void OnBirthdayMonthChanging(int oldValue, int newValue);
		partial void OnBirthdayMonthChanged(int oldValue, int newValue);
		partial void OnBirthdayYearChanging(int oldValue, int newValue);
		partial void OnBirthdayYearChanged(int oldValue, int newValue);
		partial void OnParishChanging(global::Epsitec.Aider.Entities.AiderGroupParticipantEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupParticipantEntity newValue);
		partial void OnParishChanged(global::Epsitec.Aider.Entities.AiderGroupParticipantEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupParticipantEntity newValue);
		partial void OnParishGroupChanging(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnParishGroupChanged(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		
		partial void GetParishGroup(ref global::Epsitec.Aider.Entities.AiderGroupEntity value);
		partial void SetParishGroup(global::Epsitec.Aider.Entities.AiderGroupEntity value);
		partial void GetEvents(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderEventParticipantEntity> value);
		partial void GetGroups(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderGroupParticipantEntity> value);
		partial void GetData(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonDataEntity> value);
		partial void GetChildren(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> value);
		partial void GetParents(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> value);
		partial void GetHousemates(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> value);
		partial void GetRelationships(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonRelationshipEntity> value);
		partial void GetWarnings(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonWarningEntity> value);
		partial void GetHouseholds(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderHouseholdEntity> value);
		partial void GetContacts(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderContactEntity> value);
		partial void GetAdditionalAddresses(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderContactEntity> value);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderPersonEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderPersonEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 15);	// [LVAF]
		public static readonly string EntityStructuredTypeKey = "[LVAF]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderPersonEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.eCH_ReportedPerson Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>eCH_ReportedPerson</c> entity.
	///	designer:cap/LVAG
	///	</summary>
	public partial class eCH_ReportedPersonEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Adult1</c> field.
		///	designer:fld/LVAG/LVAH
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAH]")]
		public global::Epsitec.Aider.Entities.eCH_PersonEntity Adult1
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.eCH_PersonEntity> ("[LVAH]");
			}
			set
			{
				global::Epsitec.Aider.Entities.eCH_PersonEntity oldValue = this.Adult1;
				if (oldValue != value || !this.IsFieldDefined("[LVAH]"))
				{
					this.OnAdult1Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.eCH_PersonEntity> ("[LVAH]", oldValue, value);
					this.OnAdult1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Adult2</c> field.
		///	designer:fld/LVAG/LVAI
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAI]")]
		public global::Epsitec.Aider.Entities.eCH_PersonEntity Adult2
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.eCH_PersonEntity> ("[LVAI]");
			}
			set
			{
				global::Epsitec.Aider.Entities.eCH_PersonEntity oldValue = this.Adult2;
				if (oldValue != value || !this.IsFieldDefined("[LVAI]"))
				{
					this.OnAdult2Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.eCH_PersonEntity> ("[LVAI]", oldValue, value);
					this.OnAdult2Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Children</c> field.
		///	designer:fld/LVAG/LVAJ
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAJ]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.eCH_PersonEntity> Children
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Aider.Entities.eCH_PersonEntity> ("[LVAJ]");
			}
		}
		///	<summary>
		///	The <c>Address</c> field.
		///	designer:fld/LVAG/LVAB2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAB2]")]
		public global::Epsitec.Aider.Entities.eCH_AddressEntity Address
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.eCH_AddressEntity> ("[LVAB2]");
			}
			set
			{
				global::Epsitec.Aider.Entities.eCH_AddressEntity oldValue = this.Address;
				if (oldValue != value || !this.IsFieldDefined("[LVAB2]"))
				{
					this.OnAddressChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.eCH_AddressEntity> ("[LVAB2]", oldValue, value);
					this.OnAddressChanged (oldValue, value);
				}
			}
		}
		
		partial void OnAdult1Changing(global::Epsitec.Aider.Entities.eCH_PersonEntity oldValue, global::Epsitec.Aider.Entities.eCH_PersonEntity newValue);
		partial void OnAdult1Changed(global::Epsitec.Aider.Entities.eCH_PersonEntity oldValue, global::Epsitec.Aider.Entities.eCH_PersonEntity newValue);
		partial void OnAdult2Changing(global::Epsitec.Aider.Entities.eCH_PersonEntity oldValue, global::Epsitec.Aider.Entities.eCH_PersonEntity newValue);
		partial void OnAdult2Changed(global::Epsitec.Aider.Entities.eCH_PersonEntity oldValue, global::Epsitec.Aider.Entities.eCH_PersonEntity newValue);
		partial void OnAddressChanging(global::Epsitec.Aider.Entities.eCH_AddressEntity oldValue, global::Epsitec.Aider.Entities.eCH_AddressEntity newValue);
		partial void OnAddressChanged(global::Epsitec.Aider.Entities.eCH_AddressEntity oldValue, global::Epsitec.Aider.Entities.eCH_AddressEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 16);	// [LVAG]
		public static readonly string EntityStructuredTypeKey = "[LVAG]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<eCH_ReportedPersonEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.eCH_Address Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>eCH_Address</c> entity.
	///	designer:cap/LVA22
	///	</summary>
	public partial class eCH_AddressEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>AddressLine1</c> field.
		///	designer:fld/LVA22/LVA32
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA32]")]
		public string AddressLine1
		{
			get
			{
				return this.GetField<string> ("[LVA32]");
			}
			set
			{
				string oldValue = this.AddressLine1;
				if (oldValue != value || !this.IsFieldDefined("[LVA32]"))
				{
					this.OnAddressLine1Changing (oldValue, value);
					this.SetField<string> ("[LVA32]", oldValue, value);
					this.OnAddressLine1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Street</c> field.
		///	designer:fld/LVA22/LVA42
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA42]")]
		public string Street
		{
			get
			{
				return this.GetField<string> ("[LVA42]");
			}
			set
			{
				string oldValue = this.Street;
				if (oldValue != value || !this.IsFieldDefined("[LVA42]"))
				{
					this.OnStreetChanging (oldValue, value);
					this.SetField<string> ("[LVA42]", oldValue, value);
					this.OnStreetChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>StreetUserFriendly</c> field.
		///	designer:fld/LVA22/LVAKD
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAKD]", IsVirtual=true)]
		public string StreetUserFriendly
		{
			get
			{
				string value = default (string);
				this.GetStreetUserFriendly (ref value);
				return value;
			}
			set
			{
				string oldValue = this.StreetUserFriendly;
				if (oldValue != value || !this.IsFieldDefined("[LVAKD]"))
				{
					this.OnStreetUserFriendlyChanging (oldValue, value);
					this.SetStreetUserFriendly (value);
					this.OnStreetUserFriendlyChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>HouseNumber</c> field.
		///	designer:fld/LVA22/LVA52
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA52]")]
		public string HouseNumber
		{
			get
			{
				return this.GetField<string> ("[LVA52]");
			}
			set
			{
				string oldValue = this.HouseNumber;
				if (oldValue != value || !this.IsFieldDefined("[LVA52]"))
				{
					this.OnHouseNumberChanging (oldValue, value);
					this.SetField<string> ("[LVA52]", oldValue, value);
					this.OnHouseNumberChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Town</c> field.
		///	designer:fld/LVA22/LVA62
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA62]")]
		public string Town
		{
			get
			{
				return this.GetField<string> ("[LVA62]");
			}
			set
			{
				string oldValue = this.Town;
				if (oldValue != value || !this.IsFieldDefined("[LVA62]"))
				{
					this.OnTownChanging (oldValue, value);
					this.SetField<string> ("[LVA62]", oldValue, value);
					this.OnTownChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SwissZipCode</c> field.
		///	designer:fld/LVA22/LVAPB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAPB]")]
		public int SwissZipCode
		{
			get
			{
				return this.GetField<int> ("[LVAPB]");
			}
			set
			{
				int oldValue = this.SwissZipCode;
				if (oldValue != value || !this.IsFieldDefined("[LVAPB]"))
				{
					this.OnSwissZipCodeChanging (oldValue, value);
					this.SetField<int> ("[LVAPB]", oldValue, value);
					this.OnSwissZipCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SwissZipCodeAddOn</c> field.
		///	designer:fld/LVA22/LVAQB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAQB]")]
		public int SwissZipCodeAddOn
		{
			get
			{
				return this.GetField<int> ("[LVAQB]");
			}
			set
			{
				int oldValue = this.SwissZipCodeAddOn;
				if (oldValue != value || !this.IsFieldDefined("[LVAQB]"))
				{
					this.OnSwissZipCodeAddOnChanging (oldValue, value);
					this.SetField<int> ("[LVAQB]", oldValue, value);
					this.OnSwissZipCodeAddOnChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SwissZipCodeId</c> field.
		///	designer:fld/LVA22/LVARB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVARB]")]
		public int SwissZipCodeId
		{
			get
			{
				return this.GetField<int> ("[LVARB]");
			}
			set
			{
				int oldValue = this.SwissZipCodeId;
				if (oldValue != value || !this.IsFieldDefined("[LVARB]"))
				{
					this.OnSwissZipCodeIdChanging (oldValue, value);
					this.SetField<int> ("[LVARB]", oldValue, value);
					this.OnSwissZipCodeIdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Country</c> field.
		///	designer:fld/LVA22/LVAA2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA2]")]
		public string Country
		{
			get
			{
				return this.GetField<string> ("[LVAA2]");
			}
			set
			{
				string oldValue = this.Country;
				if (oldValue != value || !this.IsFieldDefined("[LVAA2]"))
				{
					this.OnCountryChanging (oldValue, value);
					this.SetField<string> ("[LVAA2]", oldValue, value);
					this.OnCountryChanged (oldValue, value);
				}
			}
		}
		
		partial void OnAddressLine1Changing(string oldValue, string newValue);
		partial void OnAddressLine1Changed(string oldValue, string newValue);
		partial void OnStreetChanging(string oldValue, string newValue);
		partial void OnStreetChanged(string oldValue, string newValue);
		partial void OnStreetUserFriendlyChanging(string oldValue, string newValue);
		partial void OnStreetUserFriendlyChanged(string oldValue, string newValue);
		partial void OnHouseNumberChanging(string oldValue, string newValue);
		partial void OnHouseNumberChanged(string oldValue, string newValue);
		partial void OnTownChanging(string oldValue, string newValue);
		partial void OnTownChanged(string oldValue, string newValue);
		partial void OnSwissZipCodeChanging(int oldValue, int newValue);
		partial void OnSwissZipCodeChanged(int oldValue, int newValue);
		partial void OnSwissZipCodeAddOnChanging(int oldValue, int newValue);
		partial void OnSwissZipCodeAddOnChanged(int oldValue, int newValue);
		partial void OnSwissZipCodeIdChanging(int oldValue, int newValue);
		partial void OnSwissZipCodeIdChanged(int oldValue, int newValue);
		partial void OnCountryChanging(string oldValue, string newValue);
		partial void OnCountryChanged(string oldValue, string newValue);
		
		partial void GetStreetUserFriendly(ref string value);
		partial void SetStreetUserFriendly(string value);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.eCH_AddressEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.eCH_AddressEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 66);	// [LVA22]
		public static readonly string EntityStructuredTypeKey = "[LVA22]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<eCH_AddressEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderHousehold Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderHousehold</c> entity.
	///	designer:cap/LVAI2
	///	</summary>
	public partial class AiderHouseholdEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IManagedItem, global::Epsitec.Aider.Entities.IComment
	{
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVAI2/LVAA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA8]")]
		public global::Epsitec.Aider.Entities.AiderCommentEntity Comment
		{
			get
			{
				return global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.GetComment (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.SetComment (this, value);
			}
		}
		#endregion
		#region IManagedItem Members
		///	<summary>
		///	The <c>DataManager</c> field.
		///	designer:fld/LVAI2/LVAT7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAT7]")]
		public global::Epsitec.Aider.Entities.AiderDataManagerEntity DataManager
		{
			get
			{
				return global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.GetDataManager (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.SetDataManager (this, value);
			}
		}
		///	<summary>
		///	The <c>ValidationState</c> field.
		///	designer:fld/LVAI2/LVAU7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAU7]")]
		public global::Epsitec.Aider.Enumerations.ValidationState ValidationState
		{
			get
			{
				return global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.GetValidationState (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.SetValidationState (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>HouseholdMrMrs</c> field.
		///	designer:fld/LVAI2/LVAG8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAG8]")]
		public global::Epsitec.Aider.Enumerations.HouseholdMrMrs HouseholdMrMrs
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.HouseholdMrMrs> ("[LVAG8]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.HouseholdMrMrs oldValue = this.HouseholdMrMrs;
				if (oldValue != value || !this.IsFieldDefined("[LVAG8]"))
				{
					this.OnHouseholdMrMrsChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.HouseholdMrMrs> ("[LVAG8]", oldValue, value);
					this.OnHouseholdMrMrsChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>HouseholdName</c> field.
		///	designer:fld/LVAI2/LVAH9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAH9]")]
		public string HouseholdName
		{
			get
			{
				return this.GetField<string> ("[LVAH9]");
			}
			set
			{
				string oldValue = this.HouseholdName;
				if (oldValue != value || !this.IsFieldDefined("[LVAH9]"))
				{
					this.OnHouseholdNameChanging (oldValue, value);
					this.SetField<string> ("[LVAH9]", oldValue, value);
					this.OnHouseholdNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DisplayName</c> field.
		///	designer:fld/LVAI2/LVAGE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAGE]")]
		public string DisplayName
		{
			get
			{
				return this.GetField<string> ("[LVAGE]");
			}
			set
			{
				string oldValue = this.DisplayName;
				if (oldValue != value || !this.IsFieldDefined("[LVAGE]"))
				{
					this.OnDisplayNameChanging (oldValue, value);
					this.SetField<string> ("[LVAGE]", oldValue, value);
					this.OnDisplayNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DisplayAddress</c> field.
		///	designer:fld/LVAI2/LVA5F
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA5F]")]
		public string DisplayAddress
		{
			get
			{
				return this.GetField<string> ("[LVA5F]");
			}
			set
			{
				string oldValue = this.DisplayAddress;
				if (oldValue != value || !this.IsFieldDefined("[LVA5F]"))
				{
					this.OnDisplayAddressChanging (oldValue, value);
					this.SetField<string> ("[LVA5F]", oldValue, value);
					this.OnDisplayAddressChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DisplayZipCode</c> field.
		///	designer:fld/LVAI2/LVA6F
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA6F]")]
		public string DisplayZipCode
		{
			get
			{
				return this.GetField<string> ("[LVA6F]");
			}
			set
			{
				string oldValue = this.DisplayZipCode;
				if (oldValue != value || !this.IsFieldDefined("[LVA6F]"))
				{
					this.OnDisplayZipCodeChanging (oldValue, value);
					this.SetField<string> ("[LVA6F]", oldValue, value);
					this.OnDisplayZipCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DisplayVisibility</c> field.
		///	designer:fld/LVAI2/LVA8F
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA8F]")]
		public global::Epsitec.Aider.Enumerations.PersonVisibilityStatus DisplayVisibility
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.PersonVisibilityStatus> ("[LVA8F]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.PersonVisibilityStatus oldValue = this.DisplayVisibility;
				if (oldValue != value || !this.IsFieldDefined("[LVA8F]"))
				{
					this.OnDisplayVisibilityChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.PersonVisibilityStatus> ("[LVA8F]", oldValue, value);
					this.OnDisplayVisibilityChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ParishGroupPathCache</c> field.
		///	designer:fld/LVAI2/LVAAF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAAF]")]
		public string ParishGroupPathCache
		{
			get
			{
				return this.GetField<string> ("[LVAAF]");
			}
			set
			{
				string oldValue = this.ParishGroupPathCache;
				if (oldValue != value || !this.IsFieldDefined("[LVAAF]"))
				{
					this.OnParishGroupPathCacheChanging (oldValue, value);
					this.SetField<string> ("[LVAAF]", oldValue, value);
					this.OnParishGroupPathCacheChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Address</c> field.
		///	designer:fld/LVAI2/LVAT2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAT2]")]
		public global::Epsitec.Aider.Entities.AiderAddressEntity Address
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAT2]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderAddressEntity oldValue = this.Address;
				if (oldValue != value || !this.IsFieldDefined("[LVAT2]"))
				{
					this.OnAddressChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAT2]", oldValue, value);
					this.OnAddressChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Members</c> field.
		///	designer:fld/LVAI2/LVG702
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVG702]", IsVirtual=true)]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> Members
		{
			get
			{
				global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> value = default (global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity>);
				this.GetMembers (ref value);
				return value;
			}
		}
		
		partial void OnHouseholdMrMrsChanging(global::Epsitec.Aider.Enumerations.HouseholdMrMrs oldValue, global::Epsitec.Aider.Enumerations.HouseholdMrMrs newValue);
		partial void OnHouseholdMrMrsChanged(global::Epsitec.Aider.Enumerations.HouseholdMrMrs oldValue, global::Epsitec.Aider.Enumerations.HouseholdMrMrs newValue);
		partial void OnHouseholdNameChanging(string oldValue, string newValue);
		partial void OnHouseholdNameChanged(string oldValue, string newValue);
		partial void OnDisplayNameChanging(string oldValue, string newValue);
		partial void OnDisplayNameChanged(string oldValue, string newValue);
		partial void OnDisplayAddressChanging(string oldValue, string newValue);
		partial void OnDisplayAddressChanged(string oldValue, string newValue);
		partial void OnDisplayZipCodeChanging(string oldValue, string newValue);
		partial void OnDisplayZipCodeChanged(string oldValue, string newValue);
		partial void OnDisplayVisibilityChanging(global::Epsitec.Aider.Enumerations.PersonVisibilityStatus oldValue, global::Epsitec.Aider.Enumerations.PersonVisibilityStatus newValue);
		partial void OnDisplayVisibilityChanged(global::Epsitec.Aider.Enumerations.PersonVisibilityStatus oldValue, global::Epsitec.Aider.Enumerations.PersonVisibilityStatus newValue);
		partial void OnParishGroupPathCacheChanging(string oldValue, string newValue);
		partial void OnParishGroupPathCacheChanged(string oldValue, string newValue);
		partial void OnAddressChanging(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAddressChanged(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		
		partial void GetMembers(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> value);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderHouseholdEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderHouseholdEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 82);	// [LVAI2]
		public static readonly string EntityStructuredTypeKey = "[LVAI2]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderHouseholdEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderAddress Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderAddress</c> entity.
	///	designer:cap/LVAJ2
	///	</summary>
	public partial class AiderAddressEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IComment
	{
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVAJ2/LVAA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA8]")]
		public global::Epsitec.Aider.Entities.AiderCommentEntity Comment
		{
			get
			{
				return global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.GetComment (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.SetComment (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>AddressLine1</c> field.
		///	designer:fld/LVAJ2/LVA15
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA15]")]
		public string AddressLine1
		{
			get
			{
				return this.GetField<string> ("[LVA15]");
			}
			set
			{
				string oldValue = this.AddressLine1;
				if (oldValue != value || !this.IsFieldDefined("[LVA15]"))
				{
					this.OnAddressLine1Changing (oldValue, value);
					this.SetField<string> ("[LVA15]", oldValue, value);
					this.OnAddressLine1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PostBox</c> field.
		///	designer:fld/LVAJ2/LVAP5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAP5]")]
		public string PostBox
		{
			get
			{
				return this.GetField<string> ("[LVAP5]");
			}
			set
			{
				string oldValue = this.PostBox;
				if (oldValue != value || !this.IsFieldDefined("[LVAP5]"))
				{
					this.OnPostBoxChanging (oldValue, value);
					this.SetField<string> ("[LVAP5]", oldValue, value);
					this.OnPostBoxChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Street</c> field.
		///	designer:fld/LVAJ2/LVA25
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA25]")]
		public string Street
		{
			get
			{
				return this.GetField<string> ("[LVA25]");
			}
			set
			{
				string oldValue = this.Street;
				if (oldValue != value || !this.IsFieldDefined("[LVA25]"))
				{
					this.OnStreetChanging (oldValue, value);
					this.SetField<string> ("[LVA25]", oldValue, value);
					this.OnStreetChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>StreetUserFriendly</c> field.
		///	designer:fld/LVAJ2/LVAJD
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAJD]", IsVirtual=true)]
		public string StreetUserFriendly
		{
			get
			{
				string value = default (string);
				this.GetStreetUserFriendly (ref value);
				return value;
			}
			set
			{
				string oldValue = this.StreetUserFriendly;
				if (oldValue != value || !this.IsFieldDefined("[LVAJD]"))
				{
					this.OnStreetUserFriendlyChanging (oldValue, value);
					this.SetStreetUserFriendly (value);
					this.OnStreetUserFriendlyChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>HouseNumber</c> field.
		///	designer:fld/LVAJ2/LVA35
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA35]")]
		public int? HouseNumber
		{
			get
			{
				return this.GetField<int?> ("[LVA35]");
			}
			set
			{
				int? oldValue = this.HouseNumber;
				if (oldValue != value || !this.IsFieldDefined("[LVA35]"))
				{
					this.OnHouseNumberChanging (oldValue, value);
					this.SetField<int?> ("[LVA35]", oldValue, value);
					this.OnHouseNumberChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>HouseNumberComplement</c> field.
		///	designer:fld/LVAJ2/LVA45
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA45]")]
		public string HouseNumberComplement
		{
			get
			{
				return this.GetField<string> ("[LVA45]");
			}
			set
			{
				string oldValue = this.HouseNumberComplement;
				if (oldValue != value || !this.IsFieldDefined("[LVA45]"))
				{
					this.OnHouseNumberComplementChanging (oldValue, value);
					this.SetField<string> ("[LVA45]", oldValue, value);
					this.OnHouseNumberComplementChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>HouseNumberAndComplement</c> field.
		///	designer:fld/LVAJ2/LVAMD
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAMD]", IsVirtual=true)]
		public string HouseNumberAndComplement
		{
			get
			{
				string value = default (string);
				this.GetHouseNumberAndComplement (ref value);
				return value;
			}
			set
			{
				string oldValue = this.HouseNumberAndComplement;
				if (oldValue != value || !this.IsFieldDefined("[LVAMD]"))
				{
					this.OnHouseNumberAndComplementChanging (oldValue, value);
					this.SetHouseNumberAndComplement (value);
					this.OnHouseNumberAndComplementChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Town</c> field.
		///	designer:fld/LVAJ2/LVA55
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA55]")]
		public global::Epsitec.Aider.Entities.AiderTownEntity Town
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderTownEntity> ("[LVA55]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderTownEntity oldValue = this.Town;
				if (oldValue != value || !this.IsFieldDefined("[LVA55]"))
				{
					this.OnTownChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderTownEntity> ("[LVA55]", oldValue, value);
					this.OnTownChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Phone1</c> field.
		///	designer:fld/LVAJ2/LVAE5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAE5]")]
		public string Phone1
		{
			get
			{
				return this.GetField<string> ("[LVAE5]");
			}
			set
			{
				string oldValue = this.Phone1;
				if (oldValue != value || !this.IsFieldDefined("[LVAE5]"))
				{
					this.OnPhone1Changing (oldValue, value);
					this.SetField<string> ("[LVAE5]", oldValue, value);
					this.OnPhone1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Phone2</c> field.
		///	designer:fld/LVAJ2/LVAM5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAM5]")]
		public string Phone2
		{
			get
			{
				return this.GetField<string> ("[LVAM5]");
			}
			set
			{
				string oldValue = this.Phone2;
				if (oldValue != value || !this.IsFieldDefined("[LVAM5]"))
				{
					this.OnPhone2Changing (oldValue, value);
					this.SetField<string> ("[LVAM5]", oldValue, value);
					this.OnPhone2Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Mobile</c> field.
		///	designer:fld/LVAJ2/LVA6B
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA6B]")]
		public string Mobile
		{
			get
			{
				return this.GetField<string> ("[LVA6B]");
			}
			set
			{
				string oldValue = this.Mobile;
				if (oldValue != value || !this.IsFieldDefined("[LVA6B]"))
				{
					this.OnMobileChanging (oldValue, value);
					this.SetField<string> ("[LVA6B]", oldValue, value);
					this.OnMobileChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Fax</c> field.
		///	designer:fld/LVAJ2/LVA5B
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA5B]")]
		public string Fax
		{
			get
			{
				return this.GetField<string> ("[LVA5B]");
			}
			set
			{
				string oldValue = this.Fax;
				if (oldValue != value || !this.IsFieldDefined("[LVA5B]"))
				{
					this.OnFaxChanging (oldValue, value);
					this.SetField<string> ("[LVA5B]", oldValue, value);
					this.OnFaxChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Email</c> field.
		///	designer:fld/LVAJ2/LVAN5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAN5]")]
		public string Email
		{
			get
			{
				return this.GetField<string> ("[LVAN5]");
			}
			set
			{
				string oldValue = this.Email;
				if (oldValue != value || !this.IsFieldDefined("[LVAN5]"))
				{
					this.OnEmailChanging (oldValue, value);
					this.SetField<string> ("[LVAN5]", oldValue, value);
					this.OnEmailChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Web</c> field.
		///	designer:fld/LVAJ2/LVAI8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAI8]")]
		public string Web
		{
			get
			{
				return this.GetField<string> ("[LVAI8]");
			}
			set
			{
				string oldValue = this.Web;
				if (oldValue != value || !this.IsFieldDefined("[LVAI8]"))
				{
					this.OnWebChanging (oldValue, value);
					this.SetField<string> ("[LVAI8]", oldValue, value);
					this.OnWebChanged (oldValue, value);
				}
			}
		}
		
		partial void OnAddressLine1Changing(string oldValue, string newValue);
		partial void OnAddressLine1Changed(string oldValue, string newValue);
		partial void OnPostBoxChanging(string oldValue, string newValue);
		partial void OnPostBoxChanged(string oldValue, string newValue);
		partial void OnStreetChanging(string oldValue, string newValue);
		partial void OnStreetChanged(string oldValue, string newValue);
		partial void OnStreetUserFriendlyChanging(string oldValue, string newValue);
		partial void OnStreetUserFriendlyChanged(string oldValue, string newValue);
		partial void OnHouseNumberChanging(int? oldValue, int? newValue);
		partial void OnHouseNumberChanged(int? oldValue, int? newValue);
		partial void OnHouseNumberComplementChanging(string oldValue, string newValue);
		partial void OnHouseNumberComplementChanged(string oldValue, string newValue);
		partial void OnHouseNumberAndComplementChanging(string oldValue, string newValue);
		partial void OnHouseNumberAndComplementChanged(string oldValue, string newValue);
		partial void OnTownChanging(global::Epsitec.Aider.Entities.AiderTownEntity oldValue, global::Epsitec.Aider.Entities.AiderTownEntity newValue);
		partial void OnTownChanged(global::Epsitec.Aider.Entities.AiderTownEntity oldValue, global::Epsitec.Aider.Entities.AiderTownEntity newValue);
		partial void OnPhone1Changing(string oldValue, string newValue);
		partial void OnPhone1Changed(string oldValue, string newValue);
		partial void OnPhone2Changing(string oldValue, string newValue);
		partial void OnPhone2Changed(string oldValue, string newValue);
		partial void OnMobileChanging(string oldValue, string newValue);
		partial void OnMobileChanged(string oldValue, string newValue);
		partial void OnFaxChanging(string oldValue, string newValue);
		partial void OnFaxChanged(string oldValue, string newValue);
		partial void OnEmailChanging(string oldValue, string newValue);
		partial void OnEmailChanged(string oldValue, string newValue);
		partial void OnWebChanging(string oldValue, string newValue);
		partial void OnWebChanged(string oldValue, string newValue);
		
		partial void GetStreetUserFriendly(ref string value);
		partial void SetStreetUserFriendly(string value);
		partial void GetHouseNumberAndComplement(ref string value);
		partial void SetHouseNumberAndComplement(string value);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderAddressEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderAddressEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 83);	// [LVAJ2]
		public static readonly string EntityStructuredTypeKey = "[LVAJ2]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderAddressEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderPersonRelationship Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderPersonRelationship</c> entity.
	///	designer:cap/LVAV2
	///	</summary>
	public partial class AiderPersonRelationshipEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IDateRange
	{
		#region IDateRange Members
		///	<summary>
		///	The <c>StartDate</c> field.
		///	designer:fld/LVAV2/LVAN3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAN3]")]
		public global::Epsitec.Common.Types.Date? StartDate
		{
			get
			{
				return global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.GetStartDate (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.SetStartDate (this, value);
			}
		}
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/LVAV2/LVAO3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAO3]")]
		public global::Epsitec.Common.Types.Date? EndDate
		{
			get
			{
				return global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.GetEndDate (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.SetEndDate (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Type</c> field.
		///	designer:fld/LVAV2/LVAL3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAL3]")]
		public global::Epsitec.Aider.Enumerations.PersonRelationshipType Type
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.PersonRelationshipType> ("[LVAL3]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.PersonRelationshipType oldValue = this.Type;
				if (oldValue != value || !this.IsFieldDefined("[LVAL3]"))
				{
					this.OnTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.PersonRelationshipType> ("[LVAL3]", oldValue, value);
					this.OnTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Person1</c> field.
		///	designer:fld/LVAV2/LVA03
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA03]")]
		public global::Epsitec.Aider.Entities.AiderPersonEntity Person1
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVA03]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderPersonEntity oldValue = this.Person1;
				if (oldValue != value || !this.IsFieldDefined("[LVA03]"))
				{
					this.OnPerson1Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVA03]", oldValue, value);
					this.OnPerson1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Person2</c> field.
		///	designer:fld/LVAV2/LVA13
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA13]")]
		public global::Epsitec.Aider.Entities.AiderPersonEntity Person2
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVA13]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderPersonEntity oldValue = this.Person2;
				if (oldValue != value || !this.IsFieldDefined("[LVA13]"))
				{
					this.OnPerson2Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVA13]", oldValue, value);
					this.OnPerson2Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVAV2/LVAG4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAG4]")]
		public string Comment
		{
			get
			{
				return this.GetField<string> ("[LVAG4]");
			}
			set
			{
				string oldValue = this.Comment;
				if (oldValue != value || !this.IsFieldDefined("[LVAG4]"))
				{
					this.OnCommentChanging (oldValue, value);
					this.SetField<string> ("[LVAG4]", oldValue, value);
					this.OnCommentChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTypeChanging(global::Epsitec.Aider.Enumerations.PersonRelationshipType oldValue, global::Epsitec.Aider.Enumerations.PersonRelationshipType newValue);
		partial void OnTypeChanged(global::Epsitec.Aider.Enumerations.PersonRelationshipType oldValue, global::Epsitec.Aider.Enumerations.PersonRelationshipType newValue);
		partial void OnPerson1Changing(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnPerson1Changed(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnPerson2Changing(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnPerson2Changed(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnCommentChanging(string oldValue, string newValue);
		partial void OnCommentChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderPersonRelationshipEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderPersonRelationshipEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 95);	// [LVAV2]
		public static readonly string EntityStructuredTypeKey = "[LVAV2]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderPersonRelationshipEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderGroupParticipant Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderGroupParticipant</c> entity.
	///	designer:cap/LVA73
	///	</summary>
	public partial class AiderGroupParticipantEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IDateRange, global::Epsitec.Aider.Entities.IComment
	{
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVA73/LVAA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA8]")]
		public global::Epsitec.Aider.Entities.AiderCommentEntity Comment
		{
			get
			{
				return global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.GetComment (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.SetComment (this, value);
			}
		}
		#endregion
		#region IDateRange Members
		///	<summary>
		///	The <c>StartDate</c> field.
		///	designer:fld/LVA73/LVAN3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAN3]")]
		public global::Epsitec.Common.Types.Date? StartDate
		{
			get
			{
				return global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.GetStartDate (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.SetStartDate (this, value);
			}
		}
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/LVA73/LVAO3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAO3]")]
		public global::Epsitec.Common.Types.Date? EndDate
		{
			get
			{
				return global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.GetEndDate (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.SetEndDate (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>ValidationState</c> field.
		///	designer:fld/LVA73/LVAK8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAK8]")]
		public global::Epsitec.Aider.Enumerations.ValidationState ValidationState
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.ValidationState> ("[LVAK8]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.ValidationState oldValue = this.ValidationState;
				if (oldValue != value || !this.IsFieldDefined("[LVAK8]"))
				{
					this.OnValidationStateChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.ValidationState> ("[LVAK8]", oldValue, value);
					this.OnValidationStateChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Group</c> field.
		///	designer:fld/LVA73/LVA84
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA84]")]
		public global::Epsitec.Aider.Entities.AiderGroupEntity Group
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderGroupEntity> ("[LVA84]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderGroupEntity oldValue = this.Group;
				if (oldValue != value || !this.IsFieldDefined("[LVA84]"))
				{
					this.OnGroupChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderGroupEntity> ("[LVA84]", oldValue, value);
					this.OnGroupChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Person</c> field.
		///	designer:fld/LVA73/LVA94
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA94]")]
		public global::Epsitec.Aider.Entities.AiderPersonEntity Person
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVA94]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderPersonEntity oldValue = this.Person;
				if (oldValue != value || !this.IsFieldDefined("[LVA94]"))
				{
					this.OnPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVA94]", oldValue, value);
					this.OnPersonChanged (oldValue, value);
				}
			}
		}
		
		partial void OnValidationStateChanging(global::Epsitec.Aider.Enumerations.ValidationState oldValue, global::Epsitec.Aider.Enumerations.ValidationState newValue);
		partial void OnValidationStateChanged(global::Epsitec.Aider.Enumerations.ValidationState oldValue, global::Epsitec.Aider.Enumerations.ValidationState newValue);
		partial void OnGroupChanging(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnGroupChanged(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnPersonChanging(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnPersonChanged(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderGroupParticipantEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderGroupParticipantEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 103);	// [LVA73]
		public static readonly string EntityStructuredTypeKey = "[LVA73]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderGroupParticipantEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderEvent Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderEvent</c> entity.
	///	designer:cap/LVA93
	///	</summary>
	public partial class AiderEventEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IManagedItem, global::Epsitec.Aider.Entities.IComment
	{
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVA93/LVAA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA8]")]
		public global::Epsitec.Aider.Entities.AiderCommentEntity Comment
		{
			get
			{
				return global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.GetComment (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.SetComment (this, value);
			}
		}
		#endregion
		#region IManagedItem Members
		///	<summary>
		///	The <c>DataManager</c> field.
		///	designer:fld/LVA93/LVAT7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAT7]")]
		public global::Epsitec.Aider.Entities.AiderDataManagerEntity DataManager
		{
			get
			{
				return global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.GetDataManager (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.SetDataManager (this, value);
			}
		}
		///	<summary>
		///	The <c>ValidationState</c> field.
		///	designer:fld/LVA93/LVAU7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAU7]")]
		public global::Epsitec.Aider.Enumerations.ValidationState ValidationState
		{
			get
			{
				return global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.GetValidationState (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.SetValidationState (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Date</c> field.
		///	designer:fld/LVA93/LVAQ3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAQ3]")]
		public global::Epsitec.Common.Types.Date? Date
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[LVAQ3]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.Date;
				if (oldValue != value || !this.IsFieldDefined("[LVAQ3]"))
				{
					this.OnDateChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[LVAQ3]", oldValue, value);
					this.OnDateChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Type</c> field.
		///	designer:fld/LVA93/LVA04
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA04]")]
		public global::Epsitec.Aider.Enumerations.EventType Type
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.EventType> ("[LVA04]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.EventType oldValue = this.Type;
				if (oldValue != value || !this.IsFieldDefined("[LVA04]"))
				{
					this.OnTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.EventType> ("[LVA04]", oldValue, value);
					this.OnTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Place</c> field.
		///	designer:fld/LVA93/LVA14
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA14]")]
		public global::Epsitec.Aider.Entities.AiderPlaceEntity Place
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderPlaceEntity> ("[LVA14]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderPlaceEntity oldValue = this.Place;
				if (oldValue != value || !this.IsFieldDefined("[LVA14]"))
				{
					this.OnPlaceChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderPlaceEntity> ("[LVA14]", oldValue, value);
					this.OnPlaceChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Group</c> field.
		///	designer:fld/LVA93/LVABA
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVABA]")]
		public global::Epsitec.Aider.Entities.AiderGroupEntity Group
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderGroupEntity> ("[LVABA]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderGroupEntity oldValue = this.Group;
				if (oldValue != value || !this.IsFieldDefined("[LVABA]"))
				{
					this.OnGroupChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderGroupEntity> ("[LVABA]", oldValue, value);
					this.OnGroupChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/LVA93/LVAR3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAR3]")]
		public string Description
		{
			get
			{
				return this.GetField<string> ("[LVAR3]");
			}
			set
			{
				string oldValue = this.Description;
				if (oldValue != value || !this.IsFieldDefined("[LVAR3]"))
				{
					this.OnDescriptionChanging (oldValue, value);
					this.SetField<string> ("[LVAR3]", oldValue, value);
					this.OnDescriptionChanged (oldValue, value);
				}
			}
		}
		
		partial void OnDateChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnDateChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnTypeChanging(global::Epsitec.Aider.Enumerations.EventType oldValue, global::Epsitec.Aider.Enumerations.EventType newValue);
		partial void OnTypeChanged(global::Epsitec.Aider.Enumerations.EventType oldValue, global::Epsitec.Aider.Enumerations.EventType newValue);
		partial void OnPlaceChanging(global::Epsitec.Aider.Entities.AiderPlaceEntity oldValue, global::Epsitec.Aider.Entities.AiderPlaceEntity newValue);
		partial void OnPlaceChanged(global::Epsitec.Aider.Entities.AiderPlaceEntity oldValue, global::Epsitec.Aider.Entities.AiderPlaceEntity newValue);
		partial void OnGroupChanging(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnGroupChanged(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnDescriptionChanging(string oldValue, string newValue);
		partial void OnDescriptionChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderEventEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderEventEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 105);	// [LVA93]
		public static readonly string EntityStructuredTypeKey = "[LVA93]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderEventEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderEventParticipant Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderEventParticipant</c> entity.
	///	designer:cap/LVAA3
	///	</summary>
	public partial class AiderEventParticipantEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IComment
	{
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVAA3/LVAA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA8]")]
		public global::Epsitec.Aider.Entities.AiderCommentEntity Comment
		{
			get
			{
				return global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.GetComment (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.SetComment (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Role</c> field.
		///	designer:fld/LVAA3/LVAD3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAD3]")]
		public global::Epsitec.Aider.Enumerations.EventParticipantRole Role
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.EventParticipantRole> ("[LVAD3]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.EventParticipantRole oldValue = this.Role;
				if (oldValue != value || !this.IsFieldDefined("[LVAD3]"))
				{
					this.OnRoleChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.EventParticipantRole> ("[LVAD3]", oldValue, value);
					this.OnRoleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Person</c> field.
		///	designer:fld/LVAA3/LVAC3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAC3]")]
		public global::Epsitec.Aider.Entities.AiderPersonEntity Person
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVAC3]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderPersonEntity oldValue = this.Person;
				if (oldValue != value || !this.IsFieldDefined("[LVAC3]"))
				{
					this.OnPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVAC3]", oldValue, value);
					this.OnPersonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Event</c> field.
		///	designer:fld/LVAA3/LVAB3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAB3]")]
		public global::Epsitec.Aider.Entities.AiderEventEntity Event
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderEventEntity> ("[LVAB3]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderEventEntity oldValue = this.Event;
				if (oldValue != value || !this.IsFieldDefined("[LVAB3]"))
				{
					this.OnEventChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderEventEntity> ("[LVAB3]", oldValue, value);
					this.OnEventChanged (oldValue, value);
				}
			}
		}
		
		partial void OnRoleChanging(global::Epsitec.Aider.Enumerations.EventParticipantRole oldValue, global::Epsitec.Aider.Enumerations.EventParticipantRole newValue);
		partial void OnRoleChanged(global::Epsitec.Aider.Enumerations.EventParticipantRole oldValue, global::Epsitec.Aider.Enumerations.EventParticipantRole newValue);
		partial void OnPersonChanging(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnPersonChanged(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnEventChanging(global::Epsitec.Aider.Entities.AiderEventEntity oldValue, global::Epsitec.Aider.Entities.AiderEventEntity newValue);
		partial void OnEventChanged(global::Epsitec.Aider.Entities.AiderEventEntity oldValue, global::Epsitec.Aider.Entities.AiderEventEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderEventParticipantEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderEventParticipantEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 106);	// [LVAA3]
		public static readonly string EntityStructuredTypeKey = "[LVAA3]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderEventParticipantEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.IDateRange Interface
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>IDateRange</c> entity.
	///	designer:cap/LVAM3
	///	</summary>
	public interface IDateRange
	{
		///	<summary>
		///	The <c>StartDate</c> field.
		///	designer:fld/LVAM3/LVAN3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAN3]")]
		global::Epsitec.Common.Types.Date? StartDate
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/LVAM3/LVAO3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAO3]")]
		global::Epsitec.Common.Types.Date? EndDate
		{
			get;
			set;
		}
	}
	public static partial class IDateRangeInterfaceImplementation
	{
		public static global::Epsitec.Common.Types.Date? GetStartDate(global::Epsitec.Aider.Entities.IDateRange obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::Epsitec.Common.Types.Date?> ("[LVAN3]");
		}
		public static void SetStartDate(global::Epsitec.Aider.Entities.IDateRange obj, global::Epsitec.Common.Types.Date? value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::Epsitec.Common.Types.Date? oldValue = obj.StartDate;
			if (oldValue != value || !entity.IsFieldDefined("[LVAN3]"))
			{
				IDateRangeInterfaceImplementation.OnStartDateChanging (obj, oldValue, value);
				entity.SetField<global::Epsitec.Common.Types.Date?> ("[LVAN3]", oldValue, value);
				IDateRangeInterfaceImplementation.OnStartDateChanged (obj, oldValue, value);
			}
		}
		static partial void OnStartDateChanged(global::Epsitec.Aider.Entities.IDateRange obj, global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		static partial void OnStartDateChanging(global::Epsitec.Aider.Entities.IDateRange obj, global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		public static global::Epsitec.Common.Types.Date? GetEndDate(global::Epsitec.Aider.Entities.IDateRange obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::Epsitec.Common.Types.Date?> ("[LVAO3]");
		}
		public static void SetEndDate(global::Epsitec.Aider.Entities.IDateRange obj, global::Epsitec.Common.Types.Date? value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::Epsitec.Common.Types.Date? oldValue = obj.EndDate;
			if (oldValue != value || !entity.IsFieldDefined("[LVAO3]"))
			{
				IDateRangeInterfaceImplementation.OnEndDateChanging (obj, oldValue, value);
				entity.SetField<global::Epsitec.Common.Types.Date?> ("[LVAO3]", oldValue, value);
				IDateRangeInterfaceImplementation.OnEndDateChanged (obj, oldValue, value);
			}
		}
		static partial void OnEndDateChanged(global::Epsitec.Aider.Entities.IDateRange obj, global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		static partial void OnEndDateChanging(global::Epsitec.Aider.Entities.IDateRange obj, global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
	}
}
#endregion

#region Epsitec.Aider.AiderPlace Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderPlace</c> entity.
	///	designer:cap/LVAV3
	///	</summary>
	public partial class AiderPlaceEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IManagedItem, global::Epsitec.Aider.Entities.IComment
	{
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVAV3/LVAA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA8]")]
		public global::Epsitec.Aider.Entities.AiderCommentEntity Comment
		{
			get
			{
				return global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.GetComment (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.SetComment (this, value);
			}
		}
		#endregion
		#region IManagedItem Members
		///	<summary>
		///	The <c>DataManager</c> field.
		///	designer:fld/LVAV3/LVAT7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAT7]")]
		public global::Epsitec.Aider.Entities.AiderDataManagerEntity DataManager
		{
			get
			{
				return global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.GetDataManager (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.SetDataManager (this, value);
			}
		}
		///	<summary>
		///	The <c>ValidationState</c> field.
		///	designer:fld/LVAV3/LVAU7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAU7]")]
		public global::Epsitec.Aider.Enumerations.ValidationState ValidationState
		{
			get
			{
				return global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.GetValidationState (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.SetValidationState (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/LVAV3/LVA34
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA34]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[LVA34]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[LVA34]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[LVA34]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/LVAV3/LVA44
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA44]")]
		public string Description
		{
			get
			{
				return this.GetField<string> ("[LVA44]");
			}
			set
			{
				string oldValue = this.Description;
				if (oldValue != value || !this.IsFieldDefined("[LVA44]"))
				{
					this.OnDescriptionChanging (oldValue, value);
					this.SetField<string> ("[LVA44]", oldValue, value);
					this.OnDescriptionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Address</c> field.
		///	designer:fld/LVAV3/LVA24
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA24]")]
		public global::Epsitec.Aider.Entities.AiderAddressEntity Address
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVA24]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderAddressEntity oldValue = this.Address;
				if (oldValue != value || !this.IsFieldDefined("[LVA24]"))
				{
					this.OnAddressChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVA24]", oldValue, value);
					this.OnAddressChanged (oldValue, value);
				}
			}
		}
		
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		partial void OnDescriptionChanging(string oldValue, string newValue);
		partial void OnDescriptionChanged(string oldValue, string newValue);
		partial void OnAddressChanging(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAddressChanged(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderPlaceEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderPlaceEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 127);	// [LVAV3]
		public static readonly string EntityStructuredTypeKey = "[LVAV3]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderPlaceEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderGroup Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderGroup</c> entity.
	///	designer:cap/LVA54
	///	</summary>
	public partial class AiderGroupEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IDateRange, global::Epsitec.Aider.Entities.IManagedItem, global::Epsitec.Aider.Entities.IComment
	{
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVA54/LVAA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA8]")]
		public global::Epsitec.Aider.Entities.AiderCommentEntity Comment
		{
			get
			{
				return global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.GetComment (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.SetComment (this, value);
			}
		}
		#endregion
		#region IDateRange Members
		///	<summary>
		///	The <c>StartDate</c> field.
		///	designer:fld/LVA54/LVAN3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAN3]")]
		public global::Epsitec.Common.Types.Date? StartDate
		{
			get
			{
				return global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.GetStartDate (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.SetStartDate (this, value);
			}
		}
		#endregion
		#region IManagedItem Members
		///	<summary>
		///	The <c>DataManager</c> field.
		///	designer:fld/LVA54/LVAT7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAT7]")]
		public global::Epsitec.Aider.Entities.AiderDataManagerEntity DataManager
		{
			get
			{
				return global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.GetDataManager (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.SetDataManager (this, value);
			}
		}
		#endregion
		#region IDateRange Members
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/LVA54/LVAO3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAO3]")]
		public global::Epsitec.Common.Types.Date? EndDate
		{
			get
			{
				return global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.GetEndDate (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.SetEndDate (this, value);
			}
		}
		#endregion
		#region IManagedItem Members
		///	<summary>
		///	The <c>ValidationState</c> field.
		///	designer:fld/LVA54/LVAU7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAU7]")]
		public global::Epsitec.Aider.Enumerations.ValidationState ValidationState
		{
			get
			{
				return global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.GetValidationState (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.SetValidationState (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/LVA54/LVAA4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA4]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[LVAA4]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[LVAA4]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[LVAA4]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/LVA54/LVAND
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAND]")]
		public string Description
		{
			get
			{
				return this.GetField<string> ("[LVAND]");
			}
			set
			{
				string oldValue = this.Description;
				if (oldValue != value || !this.IsFieldDefined("[LVAND]"))
				{
					this.OnDescriptionChanging (oldValue, value);
					this.SetField<string> ("[LVAND]", oldValue, value);
					this.OnDescriptionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>GroupLevel</c> field.
		///	designer:fld/LVA54/LVAED
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAED]")]
		public int GroupLevel
		{
			get
			{
				return this.GetField<int> ("[LVAED]");
			}
			set
			{
				int oldValue = this.GroupLevel;
				if (oldValue != value || !this.IsFieldDefined("[LVAED]"))
				{
					this.OnGroupLevelChanging (oldValue, value);
					this.SetField<int> ("[LVAED]", oldValue, value);
					this.OnGroupLevelChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>GroupDef</c> field.
		///	designer:fld/LVA54/LVADA
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVADA]")]
		public global::Epsitec.Aider.Entities.AiderGroupDefEntity GroupDef
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderGroupDefEntity> ("[LVADA]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderGroupDefEntity oldValue = this.GroupDef;
				if (oldValue != value || !this.IsFieldDefined("[LVADA]"))
				{
					this.OnGroupDefChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderGroupDefEntity> ("[LVADA]", oldValue, value);
					this.OnGroupDefChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Path</c> field.
		///	designer:fld/LVA54/LVAPC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAPC]")]
		public string Path
		{
			get
			{
				return this.GetField<string> ("[LVAPC]");
			}
			set
			{
				string oldValue = this.Path;
				if (oldValue != value || !this.IsFieldDefined("[LVAPC]"))
				{
					this.OnPathChanging (oldValue, value);
					this.SetField<string> ("[LVAPC]", oldValue, value);
					this.OnPathChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Participants</c> field.
		///	designer:fld/LVA54/LVAJ8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAJ8]", IsVirtual=true)]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> Participants
		{
			get
			{
				global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> value = default (global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity>);
				this.GetParticipants (ref value);
				return value;
			}
		}
		///	<summary>
		///	The <c>Subgroups</c> field.
		///	designer:fld/LVA54/LVA7D
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA7D]", IsVirtual=true)]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderGroupEntity> Subgroups
		{
			get
			{
				global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderGroupEntity> value = default (global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderGroupEntity>);
				this.GetSubgroups (ref value);
				return value;
			}
		}
		
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		partial void OnDescriptionChanging(string oldValue, string newValue);
		partial void OnDescriptionChanged(string oldValue, string newValue);
		partial void OnGroupLevelChanging(int oldValue, int newValue);
		partial void OnGroupLevelChanged(int oldValue, int newValue);
		partial void OnGroupDefChanging(global::Epsitec.Aider.Entities.AiderGroupDefEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupDefEntity newValue);
		partial void OnGroupDefChanged(global::Epsitec.Aider.Entities.AiderGroupDefEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupDefEntity newValue);
		partial void OnPathChanging(string oldValue, string newValue);
		partial void OnPathChanged(string oldValue, string newValue);
		
		partial void GetParticipants(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> value);
		partial void GetSubgroups(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderGroupEntity> value);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderGroupEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderGroupEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 133);	// [LVA54]
		public static readonly string EntityStructuredTypeKey = "[LVA54]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderGroupEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderGroupRelationship Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderGroupRelationship</c> entity.
	///	designer:cap/LVAN4
	///	</summary>
	public partial class AiderGroupRelationshipEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IDateRange, global::Epsitec.Aider.Entities.IComment
	{
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVAN4/LVAA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA8]")]
		public global::Epsitec.Aider.Entities.AiderCommentEntity Comment
		{
			get
			{
				return global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.GetComment (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.SetComment (this, value);
			}
		}
		#endregion
		#region IDateRange Members
		///	<summary>
		///	The <c>StartDate</c> field.
		///	designer:fld/LVAN4/LVAN3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAN3]")]
		public global::Epsitec.Common.Types.Date? StartDate
		{
			get
			{
				return global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.GetStartDate (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.SetStartDate (this, value);
			}
		}
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/LVAN4/LVAO3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAO3]")]
		public global::Epsitec.Common.Types.Date? EndDate
		{
			get
			{
				return global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.GetEndDate (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.SetEndDate (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Type</c> field.
		///	designer:fld/LVAN4/LVAT4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAT4]")]
		public global::Epsitec.Aider.Enumerations.GroupRelationshipType Type
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.GroupRelationshipType> ("[LVAT4]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.GroupRelationshipType oldValue = this.Type;
				if (oldValue != value || !this.IsFieldDefined("[LVAT4]"))
				{
					this.OnTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.GroupRelationshipType> ("[LVAT4]", oldValue, value);
					this.OnTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Group1</c> field.
		///	designer:fld/LVAN4/LVAO4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAO4]")]
		public global::Epsitec.Aider.Entities.AiderGroupEntity Group1
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderGroupEntity> ("[LVAO4]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderGroupEntity oldValue = this.Group1;
				if (oldValue != value || !this.IsFieldDefined("[LVAO4]"))
				{
					this.OnGroup1Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderGroupEntity> ("[LVAO4]", oldValue, value);
					this.OnGroup1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Group2</c> field.
		///	designer:fld/LVAN4/LVAP4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAP4]")]
		public global::Epsitec.Aider.Entities.AiderGroupEntity Group2
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderGroupEntity> ("[LVAP4]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderGroupEntity oldValue = this.Group2;
				if (oldValue != value || !this.IsFieldDefined("[LVAP4]"))
				{
					this.OnGroup2Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderGroupEntity> ("[LVAP4]", oldValue, value);
					this.OnGroup2Changed (oldValue, value);
				}
			}
		}
		
		partial void OnTypeChanging(global::Epsitec.Aider.Enumerations.GroupRelationshipType oldValue, global::Epsitec.Aider.Enumerations.GroupRelationshipType newValue);
		partial void OnTypeChanged(global::Epsitec.Aider.Enumerations.GroupRelationshipType oldValue, global::Epsitec.Aider.Enumerations.GroupRelationshipType newValue);
		partial void OnGroup1Changing(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnGroup1Changed(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnGroup2Changing(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnGroup2Changed(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderGroupRelationshipEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderGroupRelationshipEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 151);	// [LVAN4]
		public static readonly string EntityStructuredTypeKey = "[LVAN4]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderGroupRelationshipEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderTown Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderTown</c> entity.
	///	designer:cap/LVA65
	///	</summary>
	public partial class AiderTownEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>SwissZipCode</c> field.
		///	designer:fld/LVA65/LVA75
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA75]")]
		public int? SwissZipCode
		{
			get
			{
				return this.GetField<int?> ("[LVA75]");
			}
			set
			{
				int? oldValue = this.SwissZipCode;
				if (oldValue != value || !this.IsFieldDefined("[LVA75]"))
				{
					this.OnSwissZipCodeChanging (oldValue, value);
					this.SetField<int?> ("[LVA75]", oldValue, value);
					this.OnSwissZipCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SwissZipCodeId</c> field.
		///	designer:fld/LVA65/LVA85
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA85]")]
		public int? SwissZipCodeId
		{
			get
			{
				return this.GetField<int?> ("[LVA85]");
			}
			set
			{
				int? oldValue = this.SwissZipCodeId;
				if (oldValue != value || !this.IsFieldDefined("[LVA85]"))
				{
					this.OnSwissZipCodeIdChanging (oldValue, value);
					this.SetField<int?> ("[LVA85]", oldValue, value);
					this.OnSwissZipCodeIdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SwissCantonCode</c> field.
		///	designer:fld/LVA65/LVALD
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVALD]")]
		public string SwissCantonCode
		{
			get
			{
				return this.GetField<string> ("[LVALD]");
			}
			set
			{
				string oldValue = this.SwissCantonCode;
				if (oldValue != value || !this.IsFieldDefined("[LVALD]"))
				{
					this.OnSwissCantonCodeChanging (oldValue, value);
					this.SetField<string> ("[LVALD]", oldValue, value);
					this.OnSwissCantonCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ZipCode</c> field.
		///	designer:fld/LVA65/LVAH8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAH8]")]
		public string ZipCode
		{
			get
			{
				return this.GetField<string> ("[LVAH8]");
			}
			set
			{
				string oldValue = this.ZipCode;
				if (oldValue != value || !this.IsFieldDefined("[LVAH8]"))
				{
					this.OnZipCodeChanging (oldValue, value);
					this.SetField<string> ("[LVAH8]", oldValue, value);
					this.OnZipCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/LVA65/LVA95
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA95]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[LVA95]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[LVA95]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[LVA95]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Country</c> field.
		///	designer:fld/LVA65/LVAA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA5]")]
		public global::Epsitec.Aider.Entities.AiderCountryEntity Country
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderCountryEntity> ("[LVAA5]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderCountryEntity oldValue = this.Country;
				if (oldValue != value || !this.IsFieldDefined("[LVAA5]"))
				{
					this.OnCountryChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderCountryEntity> ("[LVAA5]", oldValue, value);
					this.OnCountryChanged (oldValue, value);
				}
			}
		}
		
		partial void OnSwissZipCodeChanging(int? oldValue, int? newValue);
		partial void OnSwissZipCodeChanged(int? oldValue, int? newValue);
		partial void OnSwissZipCodeIdChanging(int? oldValue, int? newValue);
		partial void OnSwissZipCodeIdChanged(int? oldValue, int? newValue);
		partial void OnSwissCantonCodeChanging(string oldValue, string newValue);
		partial void OnSwissCantonCodeChanged(string oldValue, string newValue);
		partial void OnZipCodeChanging(string oldValue, string newValue);
		partial void OnZipCodeChanged(string oldValue, string newValue);
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		partial void OnCountryChanging(global::Epsitec.Aider.Entities.AiderCountryEntity oldValue, global::Epsitec.Aider.Entities.AiderCountryEntity newValue);
		partial void OnCountryChanged(global::Epsitec.Aider.Entities.AiderCountryEntity oldValue, global::Epsitec.Aider.Entities.AiderCountryEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderTownEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderTownEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 166);	// [LVA65]
		public static readonly string EntityStructuredTypeKey = "[LVA65]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderTownEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderCountry Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderCountry</c> entity.
	///	designer:cap/LVAB5
	///	</summary>
	public partial class AiderCountryEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>IsoCode</c> field.
		///	designer:fld/LVAB5/LVAC5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAC5]")]
		public string IsoCode
		{
			get
			{
				return this.GetField<string> ("[LVAC5]");
			}
			set
			{
				string oldValue = this.IsoCode;
				if (oldValue != value || !this.IsFieldDefined("[LVAC5]"))
				{
					this.OnIsoCodeChanging (oldValue, value);
					this.SetField<string> ("[LVAC5]", oldValue, value);
					this.OnIsoCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/LVAB5/LVAD5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAD5]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[LVAD5]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[LVAD5]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[LVAD5]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnIsoCodeChanging(string oldValue, string newValue);
		partial void OnIsoCodeChanged(string oldValue, string newValue);
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderCountryEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderCountryEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 171);	// [LVAB5]
		public static readonly string EntityStructuredTypeKey = "[LVAB5]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderCountryEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderGroupPlace Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderGroupPlace</c> entity.
	///	designer:cap/LVAF5
	///	</summary>
	public partial class AiderGroupPlaceEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IDateRange
	{
		#region IDateRange Members
		///	<summary>
		///	The <c>StartDate</c> field.
		///	designer:fld/LVAF5/LVAN3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAN3]")]
		public global::Epsitec.Common.Types.Date? StartDate
		{
			get
			{
				return global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.GetStartDate (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.SetStartDate (this, value);
			}
		}
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/LVAF5/LVAO3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAO3]")]
		public global::Epsitec.Common.Types.Date? EndDate
		{
			get
			{
				return global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.GetEndDate (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.SetEndDate (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Place</c> field.
		///	designer:fld/LVAF5/LVAG5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAG5]")]
		public global::Epsitec.Aider.Entities.AiderPlaceEntity Place
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderPlaceEntity> ("[LVAG5]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderPlaceEntity oldValue = this.Place;
				if (oldValue != value || !this.IsFieldDefined("[LVAG5]"))
				{
					this.OnPlaceChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderPlaceEntity> ("[LVAG5]", oldValue, value);
					this.OnPlaceChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Group</c> field.
		///	designer:fld/LVAF5/LVAH5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAH5]")]
		public global::Epsitec.Aider.Entities.AiderGroupEntity Group
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderGroupEntity> ("[LVAH5]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderGroupEntity oldValue = this.Group;
				if (oldValue != value || !this.IsFieldDefined("[LVAH5]"))
				{
					this.OnGroupChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderGroupEntity> ("[LVAH5]", oldValue, value);
					this.OnGroupChanged (oldValue, value);
				}
			}
		}
		
		partial void OnPlaceChanging(global::Epsitec.Aider.Entities.AiderPlaceEntity oldValue, global::Epsitec.Aider.Entities.AiderPlaceEntity newValue);
		partial void OnPlaceChanged(global::Epsitec.Aider.Entities.AiderPlaceEntity oldValue, global::Epsitec.Aider.Entities.AiderPlaceEntity newValue);
		partial void OnGroupChanging(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnGroupChanged(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderGroupPlaceEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderGroupPlaceEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 175);	// [LVAF5]
		public static readonly string EntityStructuredTypeKey = "[LVAF5]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderGroupPlaceEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderPlacePerson Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderPlacePerson</c> entity.
	///	designer:cap/LVAQ5
	///	</summary>
	public partial class AiderPlacePersonEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.IDateRange, global::Epsitec.Aider.Entities.IComment
	{
		#region IDateRange Members
		///	<summary>
		///	The <c>BeginDate</c> field.
		///	designer:fld/LVAQ5/8VAO
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
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVAQ5/LVAA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA8]")]
		public global::Epsitec.Aider.Entities.AiderCommentEntity Comment
		{
			get
			{
				return global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.GetComment (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.SetComment (this, value);
			}
		}
		#endregion
		#region IDateRange Members
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/LVAQ5/8VAP
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
		///	The <c>Function</c> field.
		///	designer:fld/LVAQ5/LVAEA
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAEA]")]
		public global::Epsitec.Aider.Entities.AiderFunctionDefEntity Function
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderFunctionDefEntity> ("[LVAEA]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderFunctionDefEntity oldValue = this.Function;
				if (oldValue != value || !this.IsFieldDefined("[LVAEA]"))
				{
					this.OnFunctionChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderFunctionDefEntity> ("[LVAEA]", oldValue, value);
					this.OnFunctionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Person</c> field.
		///	designer:fld/LVAQ5/LVAS5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAS5]")]
		public global::Epsitec.Aider.Entities.AiderPersonEntity Person
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVAS5]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderPersonEntity oldValue = this.Person;
				if (oldValue != value || !this.IsFieldDefined("[LVAS5]"))
				{
					this.OnPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVAS5]", oldValue, value);
					this.OnPersonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Place</c> field.
		///	designer:fld/LVAQ5/LVAR5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAR5]")]
		public global::Epsitec.Aider.Entities.AiderPlaceEntity Place
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderPlaceEntity> ("[LVAR5]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderPlaceEntity oldValue = this.Place;
				if (oldValue != value || !this.IsFieldDefined("[LVAR5]"))
				{
					this.OnPlaceChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderPlaceEntity> ("[LVAR5]", oldValue, value);
					this.OnPlaceChanged (oldValue, value);
				}
			}
		}
		
		partial void OnFunctionChanging(global::Epsitec.Aider.Entities.AiderFunctionDefEntity oldValue, global::Epsitec.Aider.Entities.AiderFunctionDefEntity newValue);
		partial void OnFunctionChanged(global::Epsitec.Aider.Entities.AiderFunctionDefEntity oldValue, global::Epsitec.Aider.Entities.AiderFunctionDefEntity newValue);
		partial void OnPersonChanging(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnPersonChanged(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnPlaceChanging(global::Epsitec.Aider.Entities.AiderPlaceEntity oldValue, global::Epsitec.Aider.Entities.AiderPlaceEntity newValue);
		partial void OnPlaceChanged(global::Epsitec.Aider.Entities.AiderPlaceEntity oldValue, global::Epsitec.Aider.Entities.AiderPlaceEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderPlacePersonEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderPlacePersonEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 186);	// [LVAQ5]
		public static readonly string EntityStructuredTypeKey = "[LVAQ5]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderPlacePersonEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderLegalPerson Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderLegalPerson</c> entity.
	///	designer:cap/LVAR6
	///	</summary>
	public partial class AiderLegalPersonEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IManagedItem, global::Epsitec.Aider.Entities.IComment
	{
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVAR6/LVAA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA8]")]
		public global::Epsitec.Aider.Entities.AiderCommentEntity Comment
		{
			get
			{
				return global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.GetComment (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.SetComment (this, value);
			}
		}
		#endregion
		#region IManagedItem Members
		///	<summary>
		///	The <c>DataManager</c> field.
		///	designer:fld/LVAR6/LVAT7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAT7]")]
		public global::Epsitec.Aider.Entities.AiderDataManagerEntity DataManager
		{
			get
			{
				return global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.GetDataManager (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.SetDataManager (this, value);
			}
		}
		///	<summary>
		///	The <c>ValidationState</c> field.
		///	designer:fld/LVAR6/LVAU7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAU7]")]
		public global::Epsitec.Aider.Enumerations.ValidationState ValidationState
		{
			get
			{
				return global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.GetValidationState (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.SetValidationState (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>RemovalReason</c> field.
		///	designer:fld/LVAR6/LVA17
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA17]")]
		public global::Epsitec.Aider.Enumerations.RemovalReason RemovalReason
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.RemovalReason> ("[LVA17]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.RemovalReason oldValue = this.RemovalReason;
				if (oldValue != value || !this.IsFieldDefined("[LVA17]"))
				{
					this.OnRemovalReasonChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.RemovalReason> ("[LVA17]", oldValue, value);
					this.OnRemovalReasonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Visibility</c> field.
		///	designer:fld/LVAR6/LVA2F
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA2F]")]
		public global::Epsitec.Aider.Enumerations.PersonVisibilityStatus Visibility
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.PersonVisibilityStatus> ("[LVA2F]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.PersonVisibilityStatus oldValue = this.Visibility;
				if (oldValue != value || !this.IsFieldDefined("[LVA2F]"))
				{
					this.OnVisibilityChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.PersonVisibilityStatus> ("[LVA2F]", oldValue, value);
					this.OnVisibilityChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Type</c> field.
		///	designer:fld/LVAR6/LVAS6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAS6]")]
		public global::Epsitec.Aider.Enumerations.LegalPersonType Type
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.LegalPersonType> ("[LVAS6]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.LegalPersonType oldValue = this.Type;
				if (oldValue != value || !this.IsFieldDefined("[LVAS6]"))
				{
					this.OnTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.LegalPersonType> ("[LVAS6]", oldValue, value);
					this.OnTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/LVAR6/LVAV8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAV8]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[LVAV8]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[LVAV8]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[LVAV8]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DisplayZipCode</c> field.
		///	designer:fld/LVAR6/LVAEF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAEF]")]
		public string DisplayZipCode
		{
			get
			{
				return this.GetField<string> ("[LVAEF]");
			}
			set
			{
				string oldValue = this.DisplayZipCode;
				if (oldValue != value || !this.IsFieldDefined("[LVAEF]"))
				{
					this.OnDisplayZipCodeChanging (oldValue, value);
					this.SetField<string> ("[LVAEF]", oldValue, value);
					this.OnDisplayZipCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DisplayAddress</c> field.
		///	designer:fld/LVAR6/LVAFF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAFF]")]
		public string DisplayAddress
		{
			get
			{
				return this.GetField<string> ("[LVAFF]");
			}
			set
			{
				string oldValue = this.DisplayAddress;
				if (oldValue != value || !this.IsFieldDefined("[LVAFF]"))
				{
					this.OnDisplayAddressChanging (oldValue, value);
					this.SetField<string> ("[LVAFF]", oldValue, value);
					this.OnDisplayAddressChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Language</c> field.
		///	designer:fld/LVAR6/LVAN7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAN7]")]
		public global::Epsitec.Aider.Enumerations.Language Language
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.Language> ("[LVAN7]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.Language oldValue = this.Language;
				if (oldValue != value || !this.IsFieldDefined("[LVAN7]"))
				{
					this.OnLanguageChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.Language> ("[LVAN7]", oldValue, value);
					this.OnLanguageChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Address</c> field.
		///	designer:fld/LVAR6/LVADF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVADF]")]
		public global::Epsitec.Aider.Entities.AiderAddressEntity Address
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVADF]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderAddressEntity oldValue = this.Address;
				if (oldValue != value || !this.IsFieldDefined("[LVADF]"))
				{
					this.OnAddressChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVADF]", oldValue, value);
					this.OnAddressChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ParishGroup</c> field.
		///	designer:fld/LVAR6/LVACF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVACF]", IsVirtual=true)]
		public global::Epsitec.Aider.Entities.AiderGroupEntity ParishGroup
		{
			get
			{
				global::Epsitec.Aider.Entities.AiderGroupEntity value = default (global::Epsitec.Aider.Entities.AiderGroupEntity);
				this.GetParishGroup (ref value);
				return value;
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderGroupEntity oldValue = this.ParishGroup;
				if (oldValue != value || !this.IsFieldDefined("[LVACF]"))
				{
					this.OnParishGroupChanging (oldValue, value);
					this.SetParishGroup (value);
					this.OnParishGroupChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ParishGroupPathCache</c> field.
		///	designer:fld/LVAR6/LVABF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVABF]")]
		public string ParishGroupPathCache
		{
			get
			{
				return this.GetField<string> ("[LVABF]");
			}
			set
			{
				string oldValue = this.ParishGroupPathCache;
				if (oldValue != value || !this.IsFieldDefined("[LVABF]"))
				{
					this.OnParishGroupPathCacheChanging (oldValue, value);
					this.SetField<string> ("[LVABF]", oldValue, value);
					this.OnParishGroupPathCacheChanged (oldValue, value);
				}
			}
		}
		
		partial void OnRemovalReasonChanging(global::Epsitec.Aider.Enumerations.RemovalReason oldValue, global::Epsitec.Aider.Enumerations.RemovalReason newValue);
		partial void OnRemovalReasonChanged(global::Epsitec.Aider.Enumerations.RemovalReason oldValue, global::Epsitec.Aider.Enumerations.RemovalReason newValue);
		partial void OnVisibilityChanging(global::Epsitec.Aider.Enumerations.PersonVisibilityStatus oldValue, global::Epsitec.Aider.Enumerations.PersonVisibilityStatus newValue);
		partial void OnVisibilityChanged(global::Epsitec.Aider.Enumerations.PersonVisibilityStatus oldValue, global::Epsitec.Aider.Enumerations.PersonVisibilityStatus newValue);
		partial void OnTypeChanging(global::Epsitec.Aider.Enumerations.LegalPersonType oldValue, global::Epsitec.Aider.Enumerations.LegalPersonType newValue);
		partial void OnTypeChanged(global::Epsitec.Aider.Enumerations.LegalPersonType oldValue, global::Epsitec.Aider.Enumerations.LegalPersonType newValue);
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		partial void OnDisplayZipCodeChanging(string oldValue, string newValue);
		partial void OnDisplayZipCodeChanged(string oldValue, string newValue);
		partial void OnDisplayAddressChanging(string oldValue, string newValue);
		partial void OnDisplayAddressChanged(string oldValue, string newValue);
		partial void OnLanguageChanging(global::Epsitec.Aider.Enumerations.Language oldValue, global::Epsitec.Aider.Enumerations.Language newValue);
		partial void OnLanguageChanged(global::Epsitec.Aider.Enumerations.Language oldValue, global::Epsitec.Aider.Enumerations.Language newValue);
		partial void OnAddressChanging(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAddressChanged(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnParishGroupChanging(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnParishGroupChanged(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnParishGroupPathCacheChanging(string oldValue, string newValue);
		partial void OnParishGroupPathCacheChanged(string oldValue, string newValue);
		
		partial void GetParishGroup(ref global::Epsitec.Aider.Entities.AiderGroupEntity value);
		partial void SetParishGroup(global::Epsitec.Aider.Entities.AiderGroupEntity value);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderLegalPersonEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderLegalPersonEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 219);	// [LVAR6]
		public static readonly string EntityStructuredTypeKey = "[LVAR6]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderLegalPersonEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.IManagedItem Interface
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>IManagedItem</c> entity.
	///	designer:cap/LVAQ7
	///	</summary>
	public interface IManagedItem
	{
		///	<summary>
		///	The <c>DataManager</c> field.
		///	designer:fld/LVAQ7/LVAT7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAT7]")]
		global::Epsitec.Aider.Entities.AiderDataManagerEntity DataManager
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>ValidationState</c> field.
		///	designer:fld/LVAQ7/LVAU7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAU7]")]
		global::Epsitec.Aider.Enumerations.ValidationState ValidationState
		{
			get;
			set;
		}
	}
	public static partial class IManagedItemInterfaceImplementation
	{
		public static global::Epsitec.Aider.Entities.AiderDataManagerEntity GetDataManager(global::Epsitec.Aider.Entities.IManagedItem obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::Epsitec.Aider.Entities.AiderDataManagerEntity> ("[LVAT7]");
		}
		public static void SetDataManager(global::Epsitec.Aider.Entities.IManagedItem obj, global::Epsitec.Aider.Entities.AiderDataManagerEntity value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::Epsitec.Aider.Entities.AiderDataManagerEntity oldValue = obj.DataManager;
			if (oldValue != value || !entity.IsFieldDefined("[LVAT7]"))
			{
				IManagedItemInterfaceImplementation.OnDataManagerChanging (obj, oldValue, value);
				entity.SetField<global::Epsitec.Aider.Entities.AiderDataManagerEntity> ("[LVAT7]", oldValue, value);
				IManagedItemInterfaceImplementation.OnDataManagerChanged (obj, oldValue, value);
			}
		}
		static partial void OnDataManagerChanged(global::Epsitec.Aider.Entities.IManagedItem obj, global::Epsitec.Aider.Entities.AiderDataManagerEntity oldValue, global::Epsitec.Aider.Entities.AiderDataManagerEntity newValue);
		static partial void OnDataManagerChanging(global::Epsitec.Aider.Entities.IManagedItem obj, global::Epsitec.Aider.Entities.AiderDataManagerEntity oldValue, global::Epsitec.Aider.Entities.AiderDataManagerEntity newValue);
		public static global::Epsitec.Aider.Enumerations.ValidationState GetValidationState(global::Epsitec.Aider.Entities.IManagedItem obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::Epsitec.Aider.Enumerations.ValidationState> ("[LVAU7]");
		}
		public static void SetValidationState(global::Epsitec.Aider.Entities.IManagedItem obj, global::Epsitec.Aider.Enumerations.ValidationState value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::Epsitec.Aider.Enumerations.ValidationState oldValue = obj.ValidationState;
			if (oldValue != value || !entity.IsFieldDefined("[LVAU7]"))
			{
				IManagedItemInterfaceImplementation.OnValidationStateChanging (obj, oldValue, value);
				entity.SetField<global::Epsitec.Aider.Enumerations.ValidationState> ("[LVAU7]", oldValue, value);
				IManagedItemInterfaceImplementation.OnValidationStateChanged (obj, oldValue, value);
			}
		}
		static partial void OnValidationStateChanged(global::Epsitec.Aider.Entities.IManagedItem obj, global::Epsitec.Aider.Enumerations.ValidationState oldValue, global::Epsitec.Aider.Enumerations.ValidationState newValue);
		static partial void OnValidationStateChanging(global::Epsitec.Aider.Entities.IManagedItem obj, global::Epsitec.Aider.Enumerations.ValidationState oldValue, global::Epsitec.Aider.Enumerations.ValidationState newValue);
	}
}
#endregion

#region Epsitec.Aider.AiderDataManager Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderDataManager</c> entity.
	///	designer:cap/LVAS7
	///	</summary>
	public partial class AiderDataManagerEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>User</c> field.
		///	designer:fld/LVAS7/LVASC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVASC]")]
		public global::Epsitec.Aider.Entities.AiderUserEntity User
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderUserEntity> ("[LVASC]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderUserEntity oldValue = this.User;
				if (oldValue != value || !this.IsFieldDefined("[LVASC]"))
				{
					this.OnUserChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderUserEntity> ("[LVASC]", oldValue, value);
					this.OnUserChanged (oldValue, value);
				}
			}
		}
		
		partial void OnUserChanging(global::Epsitec.Aider.Entities.AiderUserEntity oldValue, global::Epsitec.Aider.Entities.AiderUserEntity newValue);
		partial void OnUserChanged(global::Epsitec.Aider.Entities.AiderUserEntity oldValue, global::Epsitec.Aider.Entities.AiderUserEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderDataManagerEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderDataManagerEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 252);	// [LVAS7]
		public static readonly string EntityStructuredTypeKey = "[LVAS7]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderDataManagerEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.SoftwareSession Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>SoftwareSession</c> entity.
	///	designer:cap/LVAV7
	///	</summary>
	public partial class SoftwareSessionEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IDateRange
	{
		#region IDateRange Members
		///	<summary>
		///	The <c>StartDate</c> field.
		///	designer:fld/LVAV7/LVAN3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAN3]")]
		public global::Epsitec.Common.Types.Date? StartDate
		{
			get
			{
				return global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.GetStartDate (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.SetStartDate (this, value);
			}
		}
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/LVAV7/LVAO3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAO3]")]
		public global::Epsitec.Common.Types.Date? EndDate
		{
			get
			{
				return global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.GetEndDate (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.SetEndDate (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>SessionData</c> field.
		///	designer:fld/LVAV7/LVAC9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAC9]")]
		public string SessionData
		{
			get
			{
				return this.GetField<string> ("[LVAC9]");
			}
			set
			{
				string oldValue = this.SessionData;
				if (oldValue != value || !this.IsFieldDefined("[LVAC9]"))
				{
					this.OnSessionDataChanging (oldValue, value);
					this.SetField<string> ("[LVAC9]", oldValue, value);
					this.OnSessionDataChanged (oldValue, value);
				}
			}
		}
		
		partial void OnSessionDataChanging(string oldValue, string newValue);
		partial void OnSessionDataChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.SoftwareSessionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.SoftwareSessionEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 255);	// [LVAV7]
		public static readonly string EntityStructuredTypeKey = "[LVAV7]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<SoftwareSessionEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.SoftwareMutationLogEntry Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>SoftwareMutationLogEntry</c> entity.
	///	designer:cap/LVA18
	///	</summary>
	public partial class SoftwareMutationLogEntryEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Session</c> field.
		///	designer:fld/LVA18/LVAD9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAD9]")]
		public global::Epsitec.Aider.Entities.SoftwareSessionEntity Session
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.SoftwareSessionEntity> ("[LVAD9]");
			}
			set
			{
				global::Epsitec.Aider.Entities.SoftwareSessionEntity oldValue = this.Session;
				if (oldValue != value || !this.IsFieldDefined("[LVAD9]"))
				{
					this.OnSessionChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.SoftwareSessionEntity> ("[LVAD9]", oldValue, value);
					this.OnSessionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>EntityId</c> field.
		///	designer:fld/LVA18/LVAE9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAE9]")]
		public string EntityId
		{
			get
			{
				return this.GetField<string> ("[LVAE9]");
			}
			set
			{
				string oldValue = this.EntityId;
				if (oldValue != value || !this.IsFieldDefined("[LVAE9]"))
				{
					this.OnEntityIdChanging (oldValue, value);
					this.SetField<string> ("[LVAE9]", oldValue, value);
					this.OnEntityIdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SerializedChanges</c> field.
		///	designer:fld/LVA18/LVAF9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAF9]")]
		public global::System.Byte[] SerializedChanges
		{
			get
			{
				return this.GetField<global::System.Byte[]> ("[LVAF9]");
			}
			set
			{
				global::System.Byte[] oldValue = this.SerializedChanges;
				if (oldValue != value || !this.IsFieldDefined("[LVAF9]"))
				{
					this.OnSerializedChangesChanging (oldValue, value);
					this.SetField<global::System.Byte[]> ("[LVAF9]", oldValue, value);
					this.OnSerializedChangesChanged (oldValue, value);
				}
			}
		}
		
		partial void OnSessionChanging(global::Epsitec.Aider.Entities.SoftwareSessionEntity oldValue, global::Epsitec.Aider.Entities.SoftwareSessionEntity newValue);
		partial void OnSessionChanged(global::Epsitec.Aider.Entities.SoftwareSessionEntity oldValue, global::Epsitec.Aider.Entities.SoftwareSessionEntity newValue);
		partial void OnEntityIdChanging(string oldValue, string newValue);
		partial void OnEntityIdChanged(string oldValue, string newValue);
		partial void OnSerializedChangesChanging(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		partial void OnSerializedChangesChanged(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.SoftwareMutationLogEntryEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.SoftwareMutationLogEntryEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 257);	// [LVA18]
		public static readonly string EntityStructuredTypeKey = "[LVA18]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<SoftwareMutationLogEntryEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderData Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderData</c> entity.
	///	designer:cap/LVA28
	///	</summary>
	public partial class AiderDataEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IDateRange, global::Epsitec.Aider.Entities.IComment
	{
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVA28/LVAA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA8]")]
		public global::Epsitec.Aider.Entities.AiderCommentEntity Comment
		{
			get
			{
				return global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.GetComment (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.SetComment (this, value);
			}
		}
		#endregion
		#region IDateRange Members
		///	<summary>
		///	The <c>StartDate</c> field.
		///	designer:fld/LVA28/LVAN3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAN3]")]
		public global::Epsitec.Common.Types.Date? StartDate
		{
			get
			{
				return global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.GetStartDate (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.SetStartDate (this, value);
			}
		}
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/LVA28/LVAO3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAO3]")]
		public global::Epsitec.Common.Types.Date? EndDate
		{
			get
			{
				return global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.GetEndDate (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.SetEndDate (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>MimeType</c> field.
		///	designer:fld/LVA28/LVAN8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAN8]")]
		public string MimeType
		{
			get
			{
				return this.GetField<string> ("[LVAN8]");
			}
			set
			{
				string oldValue = this.MimeType;
				if (oldValue != value || !this.IsFieldDefined("[LVAN8]"))
				{
					this.OnMimeTypeChanging (oldValue, value);
					this.SetField<string> ("[LVAN8]", oldValue, value);
					this.OnMimeTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DocumentName</c> field.
		///	designer:fld/LVA28/LVAO8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAO8]")]
		public string DocumentName
		{
			get
			{
				return this.GetField<string> ("[LVAO8]");
			}
			set
			{
				string oldValue = this.DocumentName;
				if (oldValue != value || !this.IsFieldDefined("[LVAO8]"))
				{
					this.OnDocumentNameChanging (oldValue, value);
					this.SetField<string> ("[LVAO8]", oldValue, value);
					this.OnDocumentNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DocumentData</c> field.
		///	designer:fld/LVA28/LVAP8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAP8]")]
		public global::System.Byte[] DocumentData
		{
			get
			{
				return this.GetField<global::System.Byte[]> ("[LVAP8]");
			}
			set
			{
				global::System.Byte[] oldValue = this.DocumentData;
				if (oldValue != value || !this.IsFieldDefined("[LVAP8]"))
				{
					this.OnDocumentDataChanging (oldValue, value);
					this.SetField<global::System.Byte[]> ("[LVAP8]", oldValue, value);
					this.OnDocumentDataChanged (oldValue, value);
				}
			}
		}
		
		partial void OnMimeTypeChanging(string oldValue, string newValue);
		partial void OnMimeTypeChanged(string oldValue, string newValue);
		partial void OnDocumentNameChanging(string oldValue, string newValue);
		partial void OnDocumentNameChanged(string oldValue, string newValue);
		partial void OnDocumentDataChanging(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		partial void OnDocumentDataChanged(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderDataEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderDataEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 258);	// [LVA28]
		public static readonly string EntityStructuredTypeKey = "[LVA28]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderDataEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderGroupEvent Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderGroupEvent</c> entity.
	///	designer:cap/LVA48
	///	</summary>
	public partial class AiderGroupEventEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Event</c> field.
		///	designer:fld/LVA48/LVAE8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAE8]")]
		public global::Epsitec.Aider.Entities.AiderEventEntity Event
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderEventEntity> ("[LVAE8]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderEventEntity oldValue = this.Event;
				if (oldValue != value || !this.IsFieldDefined("[LVAE8]"))
				{
					this.OnEventChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderEventEntity> ("[LVAE8]", oldValue, value);
					this.OnEventChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Group</c> field.
		///	designer:fld/LVA48/LVAF8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAF8]")]
		public global::Epsitec.Aider.Entities.AiderGroupEntity Group
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderGroupEntity> ("[LVAF8]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderGroupEntity oldValue = this.Group;
				if (oldValue != value || !this.IsFieldDefined("[LVAF8]"))
				{
					this.OnGroupChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderGroupEntity> ("[LVAF8]", oldValue, value);
					this.OnGroupChanged (oldValue, value);
				}
			}
		}
		
		partial void OnEventChanging(global::Epsitec.Aider.Entities.AiderEventEntity oldValue, global::Epsitec.Aider.Entities.AiderEventEntity newValue);
		partial void OnEventChanged(global::Epsitec.Aider.Entities.AiderEventEntity oldValue, global::Epsitec.Aider.Entities.AiderEventEntity newValue);
		partial void OnGroupChanging(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnGroupChanged(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderGroupEventEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderGroupEventEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 260);	// [LVA48]
		public static readonly string EntityStructuredTypeKey = "[LVA48]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderGroupEventEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderComment Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderComment</c> entity.
	///	designer:cap/LVA78
	///	</summary>
	public partial class AiderCommentEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Text</c> field.
		///	designer:fld/LVA78/LVA88
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA88]")]
		public global::Epsitec.Common.Types.FormattedText Text
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[LVA88]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Text;
				if (oldValue != value || !this.IsFieldDefined("[LVA88]"))
				{
					this.OnTextChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[LVA88]", oldValue, value);
					this.OnTextChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SystemText</c> field.
		///	designer:fld/LVA78/LVA1F
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA1F]")]
		public string SystemText
		{
			get
			{
				return this.GetField<string> ("[LVA1F]");
			}
			set
			{
				string oldValue = this.SystemText;
				if (oldValue != value || !this.IsFieldDefined("[LVA1F]"))
				{
					this.OnSystemTextChanging (oldValue, value);
					this.SetField<string> ("[LVA1F]", oldValue, value);
					this.OnSystemTextChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTextChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnSystemTextChanging(string oldValue, string newValue);
		partial void OnSystemTextChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderCommentEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderCommentEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 263);	// [LVA78]
		public static readonly string EntityStructuredTypeKey = "[LVA78]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderCommentEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.IComment Interface
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>IComment</c> entity.
	///	designer:cap/LVA98
	///	</summary>
	public interface IComment
	{
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVA98/LVAA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA8]")]
		global::Epsitec.Aider.Entities.AiderCommentEntity Comment
		{
			get;
			set;
		}
	}
	public static partial class ICommentInterfaceImplementation
	{
		public static global::Epsitec.Aider.Entities.AiderCommentEntity GetComment(global::Epsitec.Aider.Entities.IComment obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::Epsitec.Aider.Entities.AiderCommentEntity> ("[LVAA8]");
		}
		public static void SetComment(global::Epsitec.Aider.Entities.IComment obj, global::Epsitec.Aider.Entities.AiderCommentEntity value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::Epsitec.Aider.Entities.AiderCommentEntity oldValue = obj.Comment;
			if (oldValue != value || !entity.IsFieldDefined("[LVAA8]"))
			{
				ICommentInterfaceImplementation.OnCommentChanging (obj, oldValue, value);
				entity.SetField<global::Epsitec.Aider.Entities.AiderCommentEntity> ("[LVAA8]", oldValue, value);
				ICommentInterfaceImplementation.OnCommentChanged (obj, oldValue, value);
			}
		}
		static partial void OnCommentChanged(global::Epsitec.Aider.Entities.IComment obj, global::Epsitec.Aider.Entities.AiderCommentEntity oldValue, global::Epsitec.Aider.Entities.AiderCommentEntity newValue);
		static partial void OnCommentChanging(global::Epsitec.Aider.Entities.IComment obj, global::Epsitec.Aider.Entities.AiderCommentEntity oldValue, global::Epsitec.Aider.Entities.AiderCommentEntity newValue);
	}
}
#endregion

#region Epsitec.Aider.AiderPersonData Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderPersonData</c> entity.
	///	designer:cap/LVAL8
	///	</summary>
	public partial class AiderPersonDataEntity : global::Epsitec.Aider.Entities.AiderDataEntity
	{
		///	<summary>
		///	The <c>Person</c> field.
		///	designer:fld/LVAL8/LVAM8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAM8]")]
		public global::Epsitec.Aider.Entities.AiderPersonEntity Person
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVAM8]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderPersonEntity oldValue = this.Person;
				if (oldValue != value || !this.IsFieldDefined("[LVAM8]"))
				{
					this.OnPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVAM8]", oldValue, value);
					this.OnPersonChanged (oldValue, value);
				}
			}
		}
		
		partial void OnPersonChanging(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnPersonChanged(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderPersonDataEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderPersonDataEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 277);	// [LVAL8]
		public static readonly new string EntityStructuredTypeKey = "[LVAL8]";
		
		#region Repository Class
		public new partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderPersonDataEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderGroupDef Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderGroupDef</c> entity.
	///	designer:cap/LVA2A
	///	</summary>
	public partial class AiderGroupDefEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IManagedItem, global::Epsitec.Aider.Entities.IComment
	{
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVA2A/LVAA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA8]")]
		public global::Epsitec.Aider.Entities.AiderCommentEntity Comment
		{
			get
			{
				return global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.GetComment (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.SetComment (this, value);
			}
		}
		#endregion
		#region IManagedItem Members
		///	<summary>
		///	The <c>DataManager</c> field.
		///	designer:fld/LVA2A/LVAT7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAT7]")]
		public global::Epsitec.Aider.Entities.AiderDataManagerEntity DataManager
		{
			get
			{
				return global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.GetDataManager (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.SetDataManager (this, value);
			}
		}
		///	<summary>
		///	The <c>ValidationState</c> field.
		///	designer:fld/LVA2A/LVAU7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAU7]")]
		public global::Epsitec.Aider.Enumerations.ValidationState ValidationState
		{
			get
			{
				return global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.GetValidationState (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.SetValidationState (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/LVA2A/LVA3A
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA3A]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[LVA3A]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[LVA3A]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[LVA3A]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/LVA2A/LVAOD
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAOD]")]
		public string Description
		{
			get
			{
				return this.GetField<string> ("[LVAOD]");
			}
			set
			{
				string oldValue = this.Description;
				if (oldValue != value || !this.IsFieldDefined("[LVAOD]"))
				{
					this.OnDescriptionChanging (oldValue, value);
					this.SetField<string> ("[LVAOD]", oldValue, value);
					this.OnDescriptionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PathTemplate</c> field.
		///	designer:fld/LVA2A/LVARC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVARC]")]
		public string PathTemplate
		{
			get
			{
				return this.GetField<string> ("[LVARC]");
			}
			set
			{
				string oldValue = this.PathTemplate;
				if (oldValue != value || !this.IsFieldDefined("[LVARC]"))
				{
					this.OnPathTemplateChanging (oldValue, value);
					this.SetField<string> ("[LVARC]", oldValue, value);
					this.OnPathTemplateChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>NodeType</c> field.
		///	designer:fld/LVA2A/LVA1B
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA1B]")]
		public global::Epsitec.Aider.Enumerations.GroupNodeType NodeType
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.GroupNodeType> ("[LVA1B]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.GroupNodeType oldValue = this.NodeType;
				if (oldValue != value || !this.IsFieldDefined("[LVA1B]"))
				{
					this.OnNodeTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.GroupNodeType> ("[LVA1B]", oldValue, value);
					this.OnNodeTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Classification</c> field.
		///	designer:fld/LVA2A/LVA1C
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA1C]")]
		public global::Epsitec.Aider.Enumerations.GroupClassification Classification
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.GroupClassification> ("[LVA1C]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.GroupClassification oldValue = this.Classification;
				if (oldValue != value || !this.IsFieldDefined("[LVA1C]"))
				{
					this.OnClassificationChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.GroupClassification> ("[LVA1C]", oldValue, value);
					this.OnClassificationChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Mutability</c> field.
		///	designer:fld/LVA2A/LVA2B
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA2B]")]
		public global::Epsitec.Aider.Enumerations.Mutability Mutability
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.Mutability> ("[LVA2B]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.Mutability oldValue = this.Mutability;
				if (oldValue != value || !this.IsFieldDefined("[LVA2B]"))
				{
					this.OnMutabilityChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.Mutability> ("[LVA2B]", oldValue, value);
					this.OnMutabilityChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Subgroups</c> field.
		///	designer:fld/LVA2A/LVA4A
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA4A]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderGroupDefEntity> Subgroups
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Aider.Entities.AiderGroupDefEntity> ("[LVA4A]");
			}
		}
		
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		partial void OnDescriptionChanging(string oldValue, string newValue);
		partial void OnDescriptionChanged(string oldValue, string newValue);
		partial void OnPathTemplateChanging(string oldValue, string newValue);
		partial void OnPathTemplateChanged(string oldValue, string newValue);
		partial void OnNodeTypeChanging(global::Epsitec.Aider.Enumerations.GroupNodeType oldValue, global::Epsitec.Aider.Enumerations.GroupNodeType newValue);
		partial void OnNodeTypeChanged(global::Epsitec.Aider.Enumerations.GroupNodeType oldValue, global::Epsitec.Aider.Enumerations.GroupNodeType newValue);
		partial void OnClassificationChanging(global::Epsitec.Aider.Enumerations.GroupClassification oldValue, global::Epsitec.Aider.Enumerations.GroupClassification newValue);
		partial void OnClassificationChanged(global::Epsitec.Aider.Enumerations.GroupClassification oldValue, global::Epsitec.Aider.Enumerations.GroupClassification newValue);
		partial void OnMutabilityChanging(global::Epsitec.Aider.Enumerations.Mutability oldValue, global::Epsitec.Aider.Enumerations.Mutability newValue);
		partial void OnMutabilityChanged(global::Epsitec.Aider.Enumerations.Mutability oldValue, global::Epsitec.Aider.Enumerations.Mutability newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderGroupDefEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderGroupDefEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 322);	// [LVA2A]
		public static readonly string EntityStructuredTypeKey = "[LVA2A]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderGroupDefEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderFunctionDef Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderFunctionDef</c> entity.
	///	designer:cap/LVA7A
	///	</summary>
	public partial class AiderFunctionDefEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IManagedItem, global::Epsitec.Aider.Entities.IComment
	{
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVA7A/LVAA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA8]")]
		public global::Epsitec.Aider.Entities.AiderCommentEntity Comment
		{
			get
			{
				return global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.GetComment (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.SetComment (this, value);
			}
		}
		#endregion
		#region IManagedItem Members
		///	<summary>
		///	The <c>DataManager</c> field.
		///	designer:fld/LVA7A/LVAT7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAT7]")]
		public global::Epsitec.Aider.Entities.AiderDataManagerEntity DataManager
		{
			get
			{
				return global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.GetDataManager (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.SetDataManager (this, value);
			}
		}
		///	<summary>
		///	The <c>ValidationState</c> field.
		///	designer:fld/LVA7A/LVAU7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAU7]")]
		public global::Epsitec.Aider.Enumerations.ValidationState ValidationState
		{
			get
			{
				return global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.GetValidationState (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IManagedItemInterfaceImplementation.SetValidationState (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Id</c> field.
		///	designer:fld/LVA7A/LVA6D
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA6D]")]
		public string Id
		{
			get
			{
				return this.GetField<string> ("[LVA6D]");
			}
			set
			{
				string oldValue = this.Id;
				if (oldValue != value || !this.IsFieldDefined("[LVA6D]"))
				{
					this.OnIdChanging (oldValue, value);
					this.SetField<string> ("[LVA6D]", oldValue, value);
					this.OnIdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Type</c> field.
		///	designer:fld/LVA7A/LVAFA
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAFA]")]
		public global::Epsitec.Aider.Enumerations.FunctionType Type
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.FunctionType> ("[LVAFA]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.FunctionType oldValue = this.Type;
				if (oldValue != value || !this.IsFieldDefined("[LVAFA]"))
				{
					this.OnTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.FunctionType> ("[LVAFA]", oldValue, value);
					this.OnTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/LVA7A/LVA8A
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA8A]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[LVA8A]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[LVA8A]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[LVA8A]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnIdChanging(string oldValue, string newValue);
		partial void OnIdChanged(string oldValue, string newValue);
		partial void OnTypeChanging(global::Epsitec.Aider.Enumerations.FunctionType oldValue, global::Epsitec.Aider.Enumerations.FunctionType newValue);
		partial void OnTypeChanged(global::Epsitec.Aider.Enumerations.FunctionType oldValue, global::Epsitec.Aider.Enumerations.FunctionType newValue);
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderFunctionDefEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderFunctionDefEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 327);	// [LVA7A]
		public static readonly string EntityStructuredTypeKey = "[LVA7A]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderFunctionDefEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.IAiderWarning Interface
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>IAiderWarning</c> entity.
	///	designer:cap/LVAEB
	///	</summary>
	public interface IAiderWarning : global::Epsitec.Aider.Entities.IDateRange
	{
		///	<summary>
		///	The <c>HideUntilDate</c> field.
		///	designer:fld/LVAEB/LVAAC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAAC]")]
		global::Epsitec.Common.Types.Date? HideUntilDate
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>Title</c> field.
		///	designer:fld/LVAEB/LVAKB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAKB]")]
		global::Epsitec.Common.Types.FormattedText Title
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/LVAEB/LVANB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVANB]")]
		global::Epsitec.Common.Types.FormattedText Description
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>Actions</c> field.
		///	designer:fld/LVAEB/LVAJB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAJB]")]
		global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderWarningActionEntity> Actions
		{
			get;
		}
		///	<summary>
		///	The <c>WarningType</c> field.
		///	designer:fld/LVAEB/LVA7C
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA7C]")]
		global::Epsitec.Aider.Enumerations.WarningType WarningType
		{
			get;
			set;
		}
	}
	public static partial class IAiderWarningInterfaceImplementation
	{
		public static global::Epsitec.Common.Types.Date? GetHideUntilDate(global::Epsitec.Aider.Entities.IAiderWarning obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::Epsitec.Common.Types.Date?> ("[LVAAC]");
		}
		public static void SetHideUntilDate(global::Epsitec.Aider.Entities.IAiderWarning obj, global::Epsitec.Common.Types.Date? value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::Epsitec.Common.Types.Date? oldValue = obj.HideUntilDate;
			if (oldValue != value || !entity.IsFieldDefined("[LVAAC]"))
			{
				IAiderWarningInterfaceImplementation.OnHideUntilDateChanging (obj, oldValue, value);
				entity.SetField<global::Epsitec.Common.Types.Date?> ("[LVAAC]", oldValue, value);
				IAiderWarningInterfaceImplementation.OnHideUntilDateChanged (obj, oldValue, value);
			}
		}
		static partial void OnHideUntilDateChanged(global::Epsitec.Aider.Entities.IAiderWarning obj, global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		static partial void OnHideUntilDateChanging(global::Epsitec.Aider.Entities.IAiderWarning obj, global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		public static global::Epsitec.Common.Types.FormattedText GetTitle(global::Epsitec.Aider.Entities.IAiderWarning obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::Epsitec.Common.Types.FormattedText> ("[LVAKB]");
		}
		public static void SetTitle(global::Epsitec.Aider.Entities.IAiderWarning obj, global::Epsitec.Common.Types.FormattedText value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::Epsitec.Common.Types.FormattedText oldValue = obj.Title;
			if (oldValue != value || !entity.IsFieldDefined("[LVAKB]"))
			{
				IAiderWarningInterfaceImplementation.OnTitleChanging (obj, oldValue, value);
				entity.SetField<global::Epsitec.Common.Types.FormattedText> ("[LVAKB]", oldValue, value);
				IAiderWarningInterfaceImplementation.OnTitleChanged (obj, oldValue, value);
			}
		}
		static partial void OnTitleChanged(global::Epsitec.Aider.Entities.IAiderWarning obj, global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		static partial void OnTitleChanging(global::Epsitec.Aider.Entities.IAiderWarning obj, global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		public static global::Epsitec.Common.Types.FormattedText GetDescription(global::Epsitec.Aider.Entities.IAiderWarning obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::Epsitec.Common.Types.FormattedText> ("[LVANB]");
		}
		public static void SetDescription(global::Epsitec.Aider.Entities.IAiderWarning obj, global::Epsitec.Common.Types.FormattedText value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::Epsitec.Common.Types.FormattedText oldValue = obj.Description;
			if (oldValue != value || !entity.IsFieldDefined("[LVANB]"))
			{
				IAiderWarningInterfaceImplementation.OnDescriptionChanging (obj, oldValue, value);
				entity.SetField<global::Epsitec.Common.Types.FormattedText> ("[LVANB]", oldValue, value);
				IAiderWarningInterfaceImplementation.OnDescriptionChanged (obj, oldValue, value);
			}
		}
		static partial void OnDescriptionChanged(global::Epsitec.Aider.Entities.IAiderWarning obj, global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		static partial void OnDescriptionChanging(global::Epsitec.Aider.Entities.IAiderWarning obj, global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		public static global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderWarningActionEntity> GetActions(global::Epsitec.Aider.Entities.IAiderWarning obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetFieldCollection<global::Epsitec.Aider.Entities.AiderWarningActionEntity> ("[LVAJB]");
		}
		public static global::Epsitec.Aider.Enumerations.WarningType GetWarningType(global::Epsitec.Aider.Entities.IAiderWarning obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::Epsitec.Aider.Enumerations.WarningType> ("[LVA7C]");
		}
		public static void SetWarningType(global::Epsitec.Aider.Entities.IAiderWarning obj, global::Epsitec.Aider.Enumerations.WarningType value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::Epsitec.Aider.Enumerations.WarningType oldValue = obj.WarningType;
			if (oldValue != value || !entity.IsFieldDefined("[LVA7C]"))
			{
				IAiderWarningInterfaceImplementation.OnWarningTypeChanging (obj, oldValue, value);
				entity.SetField<global::Epsitec.Aider.Enumerations.WarningType> ("[LVA7C]", oldValue, value);
				IAiderWarningInterfaceImplementation.OnWarningTypeChanged (obj, oldValue, value);
			}
		}
		static partial void OnWarningTypeChanged(global::Epsitec.Aider.Entities.IAiderWarning obj, global::Epsitec.Aider.Enumerations.WarningType oldValue, global::Epsitec.Aider.Enumerations.WarningType newValue);
		static partial void OnWarningTypeChanging(global::Epsitec.Aider.Entities.IAiderWarning obj, global::Epsitec.Aider.Enumerations.WarningType oldValue, global::Epsitec.Aider.Enumerations.WarningType newValue);
	}
}
#endregion

#region Epsitec.Aider.AiderWarningAction Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderWarningAction</c> entity.
	///	designer:cap/LVAFB
	///	</summary>
	public partial class AiderWarningActionEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>IconUri</c> field.
		///	designer:fld/LVAFB/LVAHB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAHB]")]
		public string IconUri
		{
			get
			{
				return this.GetField<string> ("[LVAHB]");
			}
			set
			{
				string oldValue = this.IconUri;
				if (oldValue != value || !this.IsFieldDefined("[LVAHB]"))
				{
					this.OnIconUriChanging (oldValue, value);
					this.SetField<string> ("[LVAHB]", oldValue, value);
					this.OnIconUriChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Title</c> field.
		///	designer:fld/LVAFB/LVAGB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAGB]")]
		public global::Epsitec.Common.Types.FormattedText Title
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[LVAGB]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Title;
				if (oldValue != value || !this.IsFieldDefined("[LVAGB]"))
				{
					this.OnTitleChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[LVAGB]", oldValue, value);
					this.OnTitleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SerializedData</c> field.
		///	designer:fld/LVAFB/LVAIB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAIB]")]
		public global::System.Byte[] SerializedData
		{
			get
			{
				return this.GetField<global::System.Byte[]> ("[LVAIB]");
			}
			set
			{
				global::System.Byte[] oldValue = this.SerializedData;
				if (oldValue != value || !this.IsFieldDefined("[LVAIB]"))
				{
					this.OnSerializedDataChanging (oldValue, value);
					this.SetField<global::System.Byte[]> ("[LVAIB]", oldValue, value);
					this.OnSerializedDataChanged (oldValue, value);
				}
			}
		}
		
		partial void OnIconUriChanging(string oldValue, string newValue);
		partial void OnIconUriChanged(string oldValue, string newValue);
		partial void OnTitleChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTitleChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnSerializedDataChanging(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		partial void OnSerializedDataChanged(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderWarningActionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderWarningActionEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 367);	// [LVAFB]
		public static readonly string EntityStructuredTypeKey = "[LVAFB]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderWarningActionEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderPersonWarning Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderPersonWarning</c> entity.
	///	designer:cap/LVALB
	///	</summary>
	public partial class AiderPersonWarningEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IAiderWarning
	{
		#region IDateRange Members
		///	<summary>
		///	The <c>StartDate</c> field.
		///	designer:fld/LVALB/LVAN3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAN3]")]
		public global::Epsitec.Common.Types.Date? StartDate
		{
			get
			{
				return global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.GetStartDate (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.SetStartDate (this, value);
			}
		}
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/LVALB/LVAO3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAO3]")]
		public global::Epsitec.Common.Types.Date? EndDate
		{
			get
			{
				return global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.GetEndDate (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IDateRangeInterfaceImplementation.SetEndDate (this, value);
			}
		}
		#endregion
		#region IAiderWarning Members
		///	<summary>
		///	The <c>HideUntilDate</c> field.
		///	designer:fld/LVALB/LVAAC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAAC]")]
		public global::Epsitec.Common.Types.Date? HideUntilDate
		{
			get
			{
				return global::Epsitec.Aider.Entities.IAiderWarningInterfaceImplementation.GetHideUntilDate (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IAiderWarningInterfaceImplementation.SetHideUntilDate (this, value);
			}
		}
		///	<summary>
		///	The <c>Title</c> field.
		///	designer:fld/LVALB/LVAKB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAKB]")]
		public global::Epsitec.Common.Types.FormattedText Title
		{
			get
			{
				return global::Epsitec.Aider.Entities.IAiderWarningInterfaceImplementation.GetTitle (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IAiderWarningInterfaceImplementation.SetTitle (this, value);
			}
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/LVALB/LVANB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVANB]")]
		public global::Epsitec.Common.Types.FormattedText Description
		{
			get
			{
				return global::Epsitec.Aider.Entities.IAiderWarningInterfaceImplementation.GetDescription (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IAiderWarningInterfaceImplementation.SetDescription (this, value);
			}
		}
		///	<summary>
		///	The <c>Actions</c> field.
		///	designer:fld/LVALB/LVAJB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAJB]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderWarningActionEntity> Actions
		{
			get
			{
				return global::Epsitec.Aider.Entities.IAiderWarningInterfaceImplementation.GetActions (this);
			}
		}
		///	<summary>
		///	The <c>WarningType</c> field.
		///	designer:fld/LVALB/LVA7C
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA7C]")]
		public global::Epsitec.Aider.Enumerations.WarningType WarningType
		{
			get
			{
				return global::Epsitec.Aider.Entities.IAiderWarningInterfaceImplementation.GetWarningType (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.IAiderWarningInterfaceImplementation.SetWarningType (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Person</c> field.
		///	designer:fld/LVALB/LVAMB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAMB]")]
		public global::Epsitec.Aider.Entities.AiderPersonEntity Person
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVAMB]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderPersonEntity oldValue = this.Person;
				if (oldValue != value || !this.IsFieldDefined("[LVAMB]"))
				{
					this.OnPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVAMB]", oldValue, value);
					this.OnPersonChanged (oldValue, value);
				}
			}
		}
		
		partial void OnPersonChanging(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnPersonChanged(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderPersonWarningEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderPersonWarningEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 373);	// [LVALB]
		public static readonly string EntityStructuredTypeKey = "[LVALB]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderPersonWarningEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderUserScope Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderUserScope</c> entity.
	///	designer:cap/LVACC
	///	</summary>
	public partial class AiderUserScopeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IComment
	{
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVACC/LVAA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA8]")]
		public global::Epsitec.Aider.Entities.AiderCommentEntity Comment
		{
			get
			{
				return global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.GetComment (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.SetComment (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Mutability</c> field.
		///	designer:fld/LVACC/LVAFD
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAFD]")]
		public global::Epsitec.Aider.Enumerations.Mutability Mutability
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.Mutability> ("[LVAFD]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.Mutability oldValue = this.Mutability;
				if (oldValue != value || !this.IsFieldDefined("[LVAFD]"))
				{
					this.OnMutabilityChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.Mutability> ("[LVAFD]", oldValue, value);
					this.OnMutabilityChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/LVACC/LVAFC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAFC]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[LVAFC]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[LVAFC]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[LVAFC]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>GroupPath</c> field.
		///	designer:fld/LVACC/LVAQC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAQC]")]
		public string GroupPath
		{
			get
			{
				return this.GetField<string> ("[LVAQC]");
			}
			set
			{
				string oldValue = this.GroupPath;
				if (oldValue != value || !this.IsFieldDefined("[LVAQC]"))
				{
					this.OnGroupPathChanging (oldValue, value);
					this.SetField<string> ("[LVAQC]", oldValue, value);
					this.OnGroupPathChanged (oldValue, value);
				}
			}
		}
		
		partial void OnMutabilityChanging(global::Epsitec.Aider.Enumerations.Mutability oldValue, global::Epsitec.Aider.Enumerations.Mutability newValue);
		partial void OnMutabilityChanged(global::Epsitec.Aider.Enumerations.Mutability oldValue, global::Epsitec.Aider.Enumerations.Mutability newValue);
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		partial void OnGroupPathChanging(string oldValue, string newValue);
		partial void OnGroupPathChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderUserScopeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderUserScopeEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 396);	// [LVACC]
		public static readonly string EntityStructuredTypeKey = "[LVACC]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderUserScopeEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderUser Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderUser</c> entity.
	///	designer:cap/LVAHC
	///	</summary>
	public partial class AiderUserEntity : global::Epsitec.Cresus.Core.Entities.SoftwareUserEntity
	{
		///	<summary>
		///	The <c>Person</c> field.
		///	designer:fld/LVAHC/LVAIC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAIC]")]
		public global::Epsitec.Aider.Entities.AiderPersonEntity Person
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVAIC]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderPersonEntity oldValue = this.Person;
				if (oldValue != value || !this.IsFieldDefined("[LVAIC]"))
				{
					this.OnPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVAIC]", oldValue, value);
					this.OnPersonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Role</c> field.
		///	designer:fld/LVAHC/LVALC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVALC]")]
		public global::Epsitec.Aider.Entities.AiderUserRoleEntity Role
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderUserRoleEntity> ("[LVALC]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderUserRoleEntity oldValue = this.Role;
				if (oldValue != value || !this.IsFieldDefined("[LVALC]"))
				{
					this.OnRoleChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderUserRoleEntity> ("[LVALC]", oldValue, value);
					this.OnRoleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CustomScopes</c> field.
		///	designer:fld/LVAHC/LVAJC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAJC]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderUserScopeEntity> CustomScopes
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Aider.Entities.AiderUserScopeEntity> ("[LVAJC]");
			}
		}
		///	<summary>
		///	The <c>PreferredScope</c> field.
		///	designer:fld/LVAHC/LVAGD
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAGD]")]
		public global::Epsitec.Aider.Entities.AiderUserScopeEntity PreferredScope
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderUserScopeEntity> ("[LVAGD]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderUserScopeEntity oldValue = this.PreferredScope;
				if (oldValue != value || !this.IsFieldDefined("[LVAGD]"))
				{
					this.OnPreferredScopeChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderUserScopeEntity> ("[LVAGD]", oldValue, value);
					this.OnPreferredScopeChanged (oldValue, value);
				}
			}
		}
		
		partial void OnPersonChanging(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnPersonChanged(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnRoleChanging(global::Epsitec.Aider.Entities.AiderUserRoleEntity oldValue, global::Epsitec.Aider.Entities.AiderUserRoleEntity newValue);
		partial void OnRoleChanged(global::Epsitec.Aider.Entities.AiderUserRoleEntity oldValue, global::Epsitec.Aider.Entities.AiderUserRoleEntity newValue);
		partial void OnPreferredScopeChanging(global::Epsitec.Aider.Entities.AiderUserScopeEntity oldValue, global::Epsitec.Aider.Entities.AiderUserScopeEntity newValue);
		partial void OnPreferredScopeChanged(global::Epsitec.Aider.Entities.AiderUserScopeEntity oldValue, global::Epsitec.Aider.Entities.AiderUserScopeEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderUserEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderUserEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 401);	// [LVAHC]
		public static readonly new string EntityStructuredTypeKey = "[LVAHC]";
		
		#region Repository Class
		public new partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderUserEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderUserRole Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderUserRole</c> entity.
	///	designer:cap/LVAKC
	///	</summary>
	public partial class AiderUserRoleEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IComment
	{
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVAKC/LVAA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA8]")]
		public global::Epsitec.Aider.Entities.AiderCommentEntity Comment
		{
			get
			{
				return global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.GetComment (this);
			}
			set
			{
				global::Epsitec.Aider.Entities.ICommentInterfaceImplementation.SetComment (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/LVAKC/LVAMC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAMC]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[LVAMC]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[LVAMC]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[LVAMC]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultScopes</c> field.
		///	designer:fld/LVAKC/LVANC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVANC]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderUserScopeEntity> DefaultScopes
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Aider.Entities.AiderUserScopeEntity> ("[LVANC]");
			}
		}
		
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderUserRoleEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderUserRoleEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 404);	// [LVAKC]
		public static readonly string EntityStructuredTypeKey = "[LVAKC]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderUserRoleEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Aider.AiderContact Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderContact</c> entity.
	///	designer:cap/LVARD
	///	</summary>
	public partial class AiderContactEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>ContactType</c> field.
		///	designer:fld/LVARD/LVA4E
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA4E]")]
		public global::Epsitec.Aider.Enumerations.ContactType ContactType
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.ContactType> ("[LVA4E]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.ContactType oldValue = this.ContactType;
				if (oldValue != value || !this.IsFieldDefined("[LVA4E]"))
				{
					this.OnContactTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.ContactType> ("[LVA4E]", oldValue, value);
					this.OnContactTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Person</c> field.
		///	designer:fld/LVARD/LVA5E
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA5E]")]
		public global::Epsitec.Aider.Entities.AiderPersonEntity Person
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVA5E]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderPersonEntity oldValue = this.Person;
				if (oldValue != value || !this.IsFieldDefined("[LVA5E]"))
				{
					this.OnPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVA5E]", oldValue, value);
					this.OnPersonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LegalPerson</c> field.
		///	designer:fld/LVARD/LVA7E
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA7E]")]
		public global::Epsitec.Aider.Entities.AiderLegalPersonEntity LegalPerson
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderLegalPersonEntity> ("[LVA7E]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderLegalPersonEntity oldValue = this.LegalPerson;
				if (oldValue != value || !this.IsFieldDefined("[LVA7E]"))
				{
					this.OnLegalPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderLegalPersonEntity> ("[LVA7E]", oldValue, value);
					this.OnLegalPersonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LegalPersonContactRole</c> field.
		///	designer:fld/LVARD/LVAAE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAAE]")]
		public global::Epsitec.Aider.Enumerations.ContactRole LegalPersonContactRole
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.ContactRole> ("[LVAAE]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.ContactRole oldValue = this.LegalPersonContactRole;
				if (oldValue != value || !this.IsFieldDefined("[LVAAE]"))
				{
					this.OnLegalPersonContactRoleChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.ContactRole> ("[LVAAE]", oldValue, value);
					this.OnLegalPersonContactRoleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Household</c> field.
		///	designer:fld/LVARD/LVA6E
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA6E]")]
		public global::Epsitec.Aider.Entities.AiderHouseholdEntity Household
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderHouseholdEntity> ("[LVA6E]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderHouseholdEntity oldValue = this.Household;
				if (oldValue != value || !this.IsFieldDefined("[LVA6E]"))
				{
					this.OnHouseholdChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderHouseholdEntity> ("[LVA6E]", oldValue, value);
					this.OnHouseholdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>HouseholdRole</c> field.
		///	designer:fld/LVARD/LVA9E
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA9E]")]
		public global::Epsitec.Aider.Enumerations.HouseholdRole HouseholdRole
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.HouseholdRole> ("[LVA9E]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.HouseholdRole oldValue = this.HouseholdRole;
				if (oldValue != value || !this.IsFieldDefined("[LVA9E]"))
				{
					this.OnHouseholdRoleChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.HouseholdRole> ("[LVA9E]", oldValue, value);
					this.OnHouseholdRoleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Address</c> field.
		///	designer:fld/LVARD/LVA8E
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA8E]")]
		public global::Epsitec.Aider.Entities.AiderAddressEntity Address
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVA8E]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderAddressEntity oldValue = this.Address;
				if (oldValue != value || !this.IsFieldDefined("[LVA8E]"))
				{
					this.OnAddressChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVA8E]", oldValue, value);
					this.OnAddressChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AddressType</c> field.
		///	designer:fld/LVARD/LVABE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVABE]")]
		public global::Epsitec.Aider.Enumerations.AddressType AddressType
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.AddressType> ("[LVABE]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.AddressType oldValue = this.AddressType;
				if (oldValue != value || !this.IsFieldDefined("[LVABE]"))
				{
					this.OnAddressTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.AddressType> ("[LVABE]", oldValue, value);
					this.OnAddressTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DisplayName</c> field.
		///	designer:fld/LVARD/LVACE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVACE]")]
		public string DisplayName
		{
			get
			{
				return this.GetField<string> ("[LVACE]");
			}
			set
			{
				string oldValue = this.DisplayName;
				if (oldValue != value || !this.IsFieldDefined("[LVACE]"))
				{
					this.OnDisplayNameChanging (oldValue, value);
					this.SetField<string> ("[LVACE]", oldValue, value);
					this.OnDisplayNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DisplayAddress</c> field.
		///	designer:fld/LVARD/LVADE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVADE]")]
		public string DisplayAddress
		{
			get
			{
				return this.GetField<string> ("[LVADE]");
			}
			set
			{
				string oldValue = this.DisplayAddress;
				if (oldValue != value || !this.IsFieldDefined("[LVADE]"))
				{
					this.OnDisplayAddressChanging (oldValue, value);
					this.SetField<string> ("[LVADE]", oldValue, value);
					this.OnDisplayAddressChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DisplayZipCode</c> field.
		///	designer:fld/LVARD/LVAEE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAEE]")]
		public string DisplayZipCode
		{
			get
			{
				return this.GetField<string> ("[LVAEE]");
			}
			set
			{
				string oldValue = this.DisplayZipCode;
				if (oldValue != value || !this.IsFieldDefined("[LVAEE]"))
				{
					this.OnDisplayZipCodeChanging (oldValue, value);
					this.SetField<string> ("[LVAEE]", oldValue, value);
					this.OnDisplayZipCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DisplayVisibility</c> field.
		///	designer:fld/LVARD/LVA7F
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA7F]")]
		public global::Epsitec.Aider.Enumerations.PersonVisibilityStatus DisplayVisibility
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.PersonVisibilityStatus> ("[LVA7F]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.PersonVisibilityStatus oldValue = this.DisplayVisibility;
				if (oldValue != value || !this.IsFieldDefined("[LVA7F]"))
				{
					this.OnDisplayVisibilityChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.PersonVisibilityStatus> ("[LVA7F]", oldValue, value);
					this.OnDisplayVisibilityChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ParishGroupPathCache</c> field.
		///	designer:fld/LVARD/LVAKE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAKE]")]
		public string ParishGroupPathCache
		{
			get
			{
				return this.GetField<string> ("[LVAKE]");
			}
			set
			{
				string oldValue = this.ParishGroupPathCache;
				if (oldValue != value || !this.IsFieldDefined("[LVAKE]"))
				{
					this.OnParishGroupPathCacheChanging (oldValue, value);
					this.SetField<string> ("[LVAKE]", oldValue, value);
					this.OnParishGroupPathCacheChanged (oldValue, value);
				}
			}
		}
		
		partial void OnContactTypeChanging(global::Epsitec.Aider.Enumerations.ContactType oldValue, global::Epsitec.Aider.Enumerations.ContactType newValue);
		partial void OnContactTypeChanged(global::Epsitec.Aider.Enumerations.ContactType oldValue, global::Epsitec.Aider.Enumerations.ContactType newValue);
		partial void OnPersonChanging(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnPersonChanged(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnLegalPersonChanging(global::Epsitec.Aider.Entities.AiderLegalPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderLegalPersonEntity newValue);
		partial void OnLegalPersonChanged(global::Epsitec.Aider.Entities.AiderLegalPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderLegalPersonEntity newValue);
		partial void OnLegalPersonContactRoleChanging(global::Epsitec.Aider.Enumerations.ContactRole oldValue, global::Epsitec.Aider.Enumerations.ContactRole newValue);
		partial void OnLegalPersonContactRoleChanged(global::Epsitec.Aider.Enumerations.ContactRole oldValue, global::Epsitec.Aider.Enumerations.ContactRole newValue);
		partial void OnHouseholdChanging(global::Epsitec.Aider.Entities.AiderHouseholdEntity oldValue, global::Epsitec.Aider.Entities.AiderHouseholdEntity newValue);
		partial void OnHouseholdChanged(global::Epsitec.Aider.Entities.AiderHouseholdEntity oldValue, global::Epsitec.Aider.Entities.AiderHouseholdEntity newValue);
		partial void OnHouseholdRoleChanging(global::Epsitec.Aider.Enumerations.HouseholdRole oldValue, global::Epsitec.Aider.Enumerations.HouseholdRole newValue);
		partial void OnHouseholdRoleChanged(global::Epsitec.Aider.Enumerations.HouseholdRole oldValue, global::Epsitec.Aider.Enumerations.HouseholdRole newValue);
		partial void OnAddressChanging(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAddressChanged(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAddressTypeChanging(global::Epsitec.Aider.Enumerations.AddressType oldValue, global::Epsitec.Aider.Enumerations.AddressType newValue);
		partial void OnAddressTypeChanged(global::Epsitec.Aider.Enumerations.AddressType oldValue, global::Epsitec.Aider.Enumerations.AddressType newValue);
		partial void OnDisplayNameChanging(string oldValue, string newValue);
		partial void OnDisplayNameChanged(string oldValue, string newValue);
		partial void OnDisplayAddressChanging(string oldValue, string newValue);
		partial void OnDisplayAddressChanged(string oldValue, string newValue);
		partial void OnDisplayZipCodeChanging(string oldValue, string newValue);
		partial void OnDisplayZipCodeChanged(string oldValue, string newValue);
		partial void OnDisplayVisibilityChanging(global::Epsitec.Aider.Enumerations.PersonVisibilityStatus oldValue, global::Epsitec.Aider.Enumerations.PersonVisibilityStatus newValue);
		partial void OnDisplayVisibilityChanged(global::Epsitec.Aider.Enumerations.PersonVisibilityStatus oldValue, global::Epsitec.Aider.Enumerations.PersonVisibilityStatus newValue);
		partial void OnParishGroupPathCacheChanging(string oldValue, string newValue);
		partial void OnParishGroupPathCacheChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderContactEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderContactEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 443);	// [LVARD]
		public static readonly string EntityStructuredTypeKey = "[LVARD]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderContactEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

