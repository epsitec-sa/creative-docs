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
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAB7]", typeof (Epsitec.Aider.Entities.AiderContactEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAS7]", typeof (Epsitec.Aider.Entities.AiderDataManagerEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAV7]", typeof (Epsitec.Aider.Entities.SoftwareSessionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA08]", typeof (Epsitec.Aider.Entities.SoftwareUserEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA18]", typeof (Epsitec.Aider.Entities.SoftwareMutationLogEntryEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA28]", typeof (Epsitec.Aider.Entities.AiderDataEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA38]", typeof (Epsitec.Aider.Entities.AiderLegalPersonContactEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA48]", typeof (Epsitec.Aider.Entities.AiderGroupEventEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA78]", typeof (Epsitec.Aider.Entities.AiderCommentEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVAL8]", typeof (Epsitec.Aider.Entities.AiderPersonDataEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA69]", typeof (Epsitec.Aider.Entities.SoftwareUserGroupEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA2A]", typeof (Epsitec.Aider.Entities.AiderGroupDefEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[LVA7A]", typeof (Epsitec.Aider.Entities.AiderFunctionDefEntity))]
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
		///	The <c>PersonDateOfBirthDay</c> field.
		///	designer:fld/LVA/LVAD2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAD2]")]
		public int? PersonDateOfBirthDay
		{
			get
			{
				return this.GetField<int?> ("[LVAD2]");
			}
			set
			{
				int? oldValue = this.PersonDateOfBirthDay;
				if (oldValue != value || !this.IsFieldDefined("[LVAD2]"))
				{
					this.OnPersonDateOfBirthDayChanging (oldValue, value);
					this.SetField<int?> ("[LVAD2]", oldValue, value);
					this.OnPersonDateOfBirthDayChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PersonDateOfBirthMonth</c> field.
		///	designer:fld/LVA/LVAE2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAE2]")]
		public int? PersonDateOfBirthMonth
		{
			get
			{
				return this.GetField<int?> ("[LVAE2]");
			}
			set
			{
				int? oldValue = this.PersonDateOfBirthMonth;
				if (oldValue != value || !this.IsFieldDefined("[LVAE2]"))
				{
					this.OnPersonDateOfBirthMonthChanging (oldValue, value);
					this.SetField<int?> ("[LVAE2]", oldValue, value);
					this.OnPersonDateOfBirthMonthChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PersonDateOfBirthYear</c> field.
		///	designer:fld/LVA/LVAF2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAF2]")]
		public int? PersonDateOfBirthYear
		{
			get
			{
				return this.GetField<int?> ("[LVAF2]");
			}
			set
			{
				int? oldValue = this.PersonDateOfBirthYear;
				if (oldValue != value || !this.IsFieldDefined("[LVAF2]"))
				{
					this.OnPersonDateOfBirthYearChanging (oldValue, value);
					this.SetField<int?> ("[LVAF2]", oldValue, value);
					this.OnPersonDateOfBirthYearChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PersonDateOfBirthType</c> field.
		///	designer:fld/LVA/LVA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA5]")]
		public global::Epsitec.Aider.Enumerations.DatePrecision PersonDateOfBirthType
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.DatePrecision> ("[LVA5]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.DatePrecision oldValue = this.PersonDateOfBirthType;
				if (oldValue != value || !this.IsFieldDefined("[LVA5]"))
				{
					this.OnPersonDateOfBirthTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.DatePrecision> ("[LVA5]", oldValue, value);
					this.OnPersonDateOfBirthTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DateOfBirth</c> field.
		///	designer:fld/LVA/LVG602
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVG602]")]
		public string DateOfBirth
		{
			get
			{
				string value = default (string);
				this.GetDateOfBirth (ref value);
				return value;
			}
			set
			{
				string oldValue = this.DateOfBirth;
				if (oldValue != value || !this.IsFieldDefined("[LVG602]"))
				{
					this.OnDateOfBirthChanging (oldValue, value);
					this.SetDateOfBirth (value);
					this.OnDateOfBirthChanged (oldValue, value);
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
		partial void OnPersonDateOfBirthDayChanging(int? oldValue, int? newValue);
		partial void OnPersonDateOfBirthDayChanged(int? oldValue, int? newValue);
		partial void OnPersonDateOfBirthMonthChanging(int? oldValue, int? newValue);
		partial void OnPersonDateOfBirthMonthChanged(int? oldValue, int? newValue);
		partial void OnPersonDateOfBirthYearChanging(int? oldValue, int? newValue);
		partial void OnPersonDateOfBirthYearChanged(int? oldValue, int? newValue);
		partial void OnPersonDateOfBirthTypeChanging(global::Epsitec.Aider.Enumerations.DatePrecision oldValue, global::Epsitec.Aider.Enumerations.DatePrecision newValue);
		partial void OnPersonDateOfBirthTypeChanged(global::Epsitec.Aider.Enumerations.DatePrecision oldValue, global::Epsitec.Aider.Enumerations.DatePrecision newValue);
		partial void OnDateOfBirthChanging(string oldValue, string newValue);
		partial void OnDateOfBirthChanged(string oldValue, string newValue);
		partial void OnPersonSexChanging(global::Epsitec.Aider.Enumerations.PersonSex oldValue, global::Epsitec.Aider.Enumerations.PersonSex newValue);
		partial void OnPersonSexChanged(global::Epsitec.Aider.Enumerations.PersonSex oldValue, global::Epsitec.Aider.Enumerations.PersonSex newValue);
		partial void OnNationalityStatusChanging(global::Epsitec.Aider.Enumerations.PersonNationalityStatus oldValue, global::Epsitec.Aider.Enumerations.PersonNationalityStatus newValue);
		partial void OnNationalityStatusChanged(global::Epsitec.Aider.Enumerations.PersonNationalityStatus oldValue, global::Epsitec.Aider.Enumerations.PersonNationalityStatus newValue);
		partial void OnNationalityCountryCodeChanging(string oldValue, string newValue);
		partial void OnNationalityCountryCodeChanged(string oldValue, string newValue);
		partial void OnOriginsChanging(string oldValue, string newValue);
		partial void OnOriginsChanged(string oldValue, string newValue);
		partial void OnAdultMaritalStatusChanging(global::Epsitec.Aider.Enumerations.PersonMaritalStatus oldValue, global::Epsitec.Aider.Enumerations.PersonMaritalStatus newValue);
		partial void OnAdultMaritalStatusChanged(global::Epsitec.Aider.Enumerations.PersonMaritalStatus oldValue, global::Epsitec.Aider.Enumerations.PersonMaritalStatus newValue);
		partial void OnAddress1Changing(global::Epsitec.Aider.Entities.eCH_AddressEntity oldValue, global::Epsitec.Aider.Entities.eCH_AddressEntity newValue);
		partial void OnAddress1Changed(global::Epsitec.Aider.Entities.eCH_AddressEntity oldValue, global::Epsitec.Aider.Entities.eCH_AddressEntity newValue);
		partial void OnAddress2Changing(global::Epsitec.Aider.Entities.eCH_AddressEntity oldValue, global::Epsitec.Aider.Entities.eCH_AddressEntity newValue);
		partial void OnAddress2Changed(global::Epsitec.Aider.Entities.eCH_AddressEntity oldValue, global::Epsitec.Aider.Entities.eCH_AddressEntity newValue);
		
		partial void GetDateOfBirth(ref string value);
		partial void SetDateOfBirth(string value);
		
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
		///	The <c>Household1</c> field.
		///	designer:fld/LVAF/LVAL4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAL4]")]
		public global::Epsitec.Aider.Entities.AiderHouseholdEntity Household1
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderHouseholdEntity> ("[LVAL4]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderHouseholdEntity oldValue = this.Household1;
				if (oldValue != value || !this.IsFieldDefined("[LVAL4]"))
				{
					this.OnHousehold1Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderHouseholdEntity> ("[LVAL4]", oldValue, value);
					this.OnHousehold1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Household2</c> field.
		///	designer:fld/LVAF/LVAG9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAG9]")]
		public global::Epsitec.Aider.Entities.AiderHouseholdEntity Household2
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderHouseholdEntity> ("[LVAG9]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderHouseholdEntity oldValue = this.Household2;
				if (oldValue != value || !this.IsFieldDefined("[LVAG9]"))
				{
					this.OnHousehold2Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderHouseholdEntity> ("[LVAG9]", oldValue, value);
					this.OnHousehold2Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AdditionalAddress1</c> field.
		///	designer:fld/LVAF/LVAM4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAM4]")]
		public global::Epsitec.Aider.Entities.AiderAddressEntity AdditionalAddress1
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAM4]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderAddressEntity oldValue = this.AdditionalAddress1;
				if (oldValue != value || !this.IsFieldDefined("[LVAM4]"))
				{
					this.OnAdditionalAddress1Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAM4]", oldValue, value);
					this.OnAdditionalAddress1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AdditionalAddress2</c> field.
		///	designer:fld/LVAF/LVAU4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAU4]")]
		public global::Epsitec.Aider.Entities.AiderAddressEntity AdditionalAddress2
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAU4]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderAddressEntity oldValue = this.AdditionalAddress2;
				if (oldValue != value || !this.IsFieldDefined("[LVAU4]"))
				{
					this.OnAdditionalAddress2Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAU4]", oldValue, value);
					this.OnAdditionalAddress2Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AdditionalAddress3</c> field.
		///	designer:fld/LVAF/LVAI9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAI9]")]
		public global::Epsitec.Aider.Entities.AiderAddressEntity AdditionalAddress3
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAI9]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderAddressEntity oldValue = this.AdditionalAddress3;
				if (oldValue != value || !this.IsFieldDefined("[LVAI9]"))
				{
					this.OnAdditionalAddress3Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAI9]", oldValue, value);
					this.OnAdditionalAddress3Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AdditionalAddress4</c> field.
		///	designer:fld/LVAF/LVAJ9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAJ9]")]
		public global::Epsitec.Aider.Entities.AiderAddressEntity AdditionalAddress4
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAJ9]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderAddressEntity oldValue = this.AdditionalAddress4;
				if (oldValue != value || !this.IsFieldDefined("[LVAJ9]"))
				{
					this.OnAdditionalAddress4Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAJ9]", oldValue, value);
					this.OnAdditionalAddress4Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DateOfBirth</c> field.
		///	designer:fld/LVAF/LVAT8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAT8]")]
		public global::Epsitec.Common.Types.Date? DateOfBirth
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[LVAT8]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.DateOfBirth;
				if (oldValue != value || !this.IsFieldDefined("[LVAT8]"))
				{
					this.OnDateOfBirthChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[LVAT8]", oldValue, value);
					this.OnDateOfBirthChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DateOfDeath</c> field.
		///	designer:fld/LVAF/LVAU8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAU8]")]
		public global::Epsitec.Common.Types.Date? DateOfDeath
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[LVAU8]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.DateOfDeath;
				if (oldValue != value || !this.IsFieldDefined("[LVAU8]"))
				{
					this.OnDateOfDeathChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[LVAU8]", oldValue, value);
					this.OnDateOfDeathChanged (oldValue, value);
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
		///	The <c>Events</c> field.
		///	designer:fld/LVAF/LVAQ8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAQ8]")]
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
		[global::Epsitec.Common.Support.EntityField ("[LVAR8]")]
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
		[global::Epsitec.Common.Support.EntityField ("[LVAS8]")]
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
		[global::Epsitec.Common.Support.EntityField ("[LVGL02]")]
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
		[global::Epsitec.Common.Support.EntityField ("[LVGM02]")]
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
		[global::Epsitec.Common.Support.EntityField ("[LVGN02]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> Housemates
		{
			get
			{
				global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> value = default (global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity>);
				this.GetHousemates (ref value);
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
		partial void OnHousehold1Changing(global::Epsitec.Aider.Entities.AiderHouseholdEntity oldValue, global::Epsitec.Aider.Entities.AiderHouseholdEntity newValue);
		partial void OnHousehold1Changed(global::Epsitec.Aider.Entities.AiderHouseholdEntity oldValue, global::Epsitec.Aider.Entities.AiderHouseholdEntity newValue);
		partial void OnHousehold2Changing(global::Epsitec.Aider.Entities.AiderHouseholdEntity oldValue, global::Epsitec.Aider.Entities.AiderHouseholdEntity newValue);
		partial void OnHousehold2Changed(global::Epsitec.Aider.Entities.AiderHouseholdEntity oldValue, global::Epsitec.Aider.Entities.AiderHouseholdEntity newValue);
		partial void OnAdditionalAddress1Changing(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAdditionalAddress1Changed(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAdditionalAddress2Changing(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAdditionalAddress2Changed(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAdditionalAddress3Changing(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAdditionalAddress3Changed(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAdditionalAddress4Changing(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAdditionalAddress4Changed(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnDateOfBirthChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnDateOfBirthChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnDateOfDeathChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnDateOfDeathChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnConfessionChanging(global::Epsitec.Aider.Enumerations.PersonConfession oldValue, global::Epsitec.Aider.Enumerations.PersonConfession newValue);
		partial void OnConfessionChanged(global::Epsitec.Aider.Enumerations.PersonConfession oldValue, global::Epsitec.Aider.Enumerations.PersonConfession newValue);
		partial void OnProfessionChanging(string oldValue, string newValue);
		partial void OnProfessionChanged(string oldValue, string newValue);
		
		partial void GetEvents(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderEventParticipantEntity> value);
		partial void GetGroups(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderGroupParticipantEntity> value);
		partial void GetData(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonDataEntity> value);
		partial void GetChildren(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> value);
		partial void GetParents(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> value);
		partial void GetHousemates(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderPersonEntity> value);
		
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
		///	designer:fld/LVA22/LVA72
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA72]")]
		public string SwissZipCode
		{
			get
			{
				return this.GetField<string> ("[LVA72]");
			}
			set
			{
				string oldValue = this.SwissZipCode;
				if (oldValue != value || !this.IsFieldDefined("[LVA72]"))
				{
					this.OnSwissZipCodeChanging (oldValue, value);
					this.SetField<string> ("[LVA72]", oldValue, value);
					this.OnSwissZipCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SwissZipCodeAddOn</c> field.
		///	designer:fld/LVA22/LVA82
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA82]")]
		public string SwissZipCodeAddOn
		{
			get
			{
				return this.GetField<string> ("[LVA82]");
			}
			set
			{
				string oldValue = this.SwissZipCodeAddOn;
				if (oldValue != value || !this.IsFieldDefined("[LVA82]"))
				{
					this.OnSwissZipCodeAddOnChanging (oldValue, value);
					this.SetField<string> ("[LVA82]", oldValue, value);
					this.OnSwissZipCodeAddOnChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SwissZipCodeId</c> field.
		///	designer:fld/LVA22/LVA92
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA92]")]
		public string SwissZipCodeId
		{
			get
			{
				return this.GetField<string> ("[LVA92]");
			}
			set
			{
				string oldValue = this.SwissZipCodeId;
				if (oldValue != value || !this.IsFieldDefined("[LVA92]"))
				{
					this.OnSwissZipCodeIdChanging (oldValue, value);
					this.SetField<string> ("[LVA92]", oldValue, value);
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
		partial void OnHouseNumberChanging(string oldValue, string newValue);
		partial void OnHouseNumberChanged(string oldValue, string newValue);
		partial void OnTownChanging(string oldValue, string newValue);
		partial void OnTownChanged(string oldValue, string newValue);
		partial void OnSwissZipCodeChanging(string oldValue, string newValue);
		partial void OnSwissZipCodeChanged(string oldValue, string newValue);
		partial void OnSwissZipCodeAddOnChanging(string oldValue, string newValue);
		partial void OnSwissZipCodeAddOnChanged(string oldValue, string newValue);
		partial void OnSwissZipCodeIdChanging(string oldValue, string newValue);
		partial void OnSwissZipCodeIdChanged(string oldValue, string newValue);
		partial void OnCountryChanging(string oldValue, string newValue);
		partial void OnCountryChanged(string oldValue, string newValue);
		
		
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
		///	The <c>Head1</c> field.
		///	designer:fld/LVAI2/LVAV4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAV4]")]
		public global::Epsitec.Aider.Entities.AiderPersonEntity Head1
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVAV4]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderPersonEntity oldValue = this.Head1;
				if (oldValue != value || !this.IsFieldDefined("[LVAV4]"))
				{
					this.OnHead1Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVAV4]", oldValue, value);
					this.OnHead1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Head2</c> field.
		///	designer:fld/LVAI2/LVA05
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA05]")]
		public global::Epsitec.Aider.Entities.AiderPersonEntity Head2
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVA05]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderPersonEntity oldValue = this.Head2;
				if (oldValue != value || !this.IsFieldDefined("[LVA05]"))
				{
					this.OnHead2Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVA05]", oldValue, value);
					this.OnHead2Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Members</c> field.
		///	designer:fld/LVAI2/LVG702
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVG702]")]
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
		partial void OnAddressChanging(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAddressChanged(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnHead1Changing(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnHead1Changed(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnHead2Changing(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnHead2Changed(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		
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
		///	The <c>Type</c> field.
		///	designer:fld/LVAJ2/LVAE6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAE6]")]
		public global::Epsitec.Aider.Enumerations.AddressType Type
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.AddressType> ("[LVAE6]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.AddressType oldValue = this.Type;
				if (oldValue != value || !this.IsFieldDefined("[LVAE6]"))
				{
					this.OnTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.AddressType> ("[LVAE6]", oldValue, value);
					this.OnTypeChanged (oldValue, value);
				}
			}
		}
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
		
		partial void OnTypeChanging(global::Epsitec.Aider.Enumerations.AddressType oldValue, global::Epsitec.Aider.Enumerations.AddressType newValue);
		partial void OnTypeChanged(global::Epsitec.Aider.Enumerations.AddressType oldValue, global::Epsitec.Aider.Enumerations.AddressType newValue);
		partial void OnAddressLine1Changing(string oldValue, string newValue);
		partial void OnAddressLine1Changed(string oldValue, string newValue);
		partial void OnPostBoxChanging(string oldValue, string newValue);
		partial void OnPostBoxChanged(string oldValue, string newValue);
		partial void OnStreetChanging(string oldValue, string newValue);
		partial void OnStreetChanged(string oldValue, string newValue);
		partial void OnHouseNumberChanging(int? oldValue, int? newValue);
		partial void OnHouseNumberChanged(int? oldValue, int? newValue);
		partial void OnHouseNumberComplementChanging(string oldValue, string newValue);
		partial void OnHouseNumberComplementChanged(string oldValue, string newValue);
		partial void OnTownChanging(global::Epsitec.Aider.Entities.AiderTownEntity oldValue, global::Epsitec.Aider.Entities.AiderTownEntity newValue);
		partial void OnTownChanged(global::Epsitec.Aider.Entities.AiderTownEntity oldValue, global::Epsitec.Aider.Entities.AiderTownEntity newValue);
		partial void OnPhone1Changing(string oldValue, string newValue);
		partial void OnPhone1Changed(string oldValue, string newValue);
		partial void OnPhone2Changing(string oldValue, string newValue);
		partial void OnPhone2Changed(string oldValue, string newValue);
		partial void OnEmailChanging(string oldValue, string newValue);
		partial void OnEmailChanged(string oldValue, string newValue);
		partial void OnWebChanging(string oldValue, string newValue);
		partial void OnWebChanged(string oldValue, string newValue);
		
		
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
		///	The <c>Function</c> field.
		///	designer:fld/LVA73/LVACA
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVACA]")]
		public global::Epsitec.Aider.Entities.AiderFunctionDefEntity Function
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderFunctionDefEntity> ("[LVACA]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderFunctionDefEntity oldValue = this.Function;
				if (oldValue != value || !this.IsFieldDefined("[LVACA]"))
				{
					this.OnFunctionChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderFunctionDefEntity> ("[LVACA]", oldValue, value);
					this.OnFunctionChanged (oldValue, value);
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
		///	<summary>
		///	The <c>LegalPerson</c> field.
		///	designer:fld/LVA73/LVA0A
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA0A]")]
		public global::Epsitec.Aider.Entities.AiderLegalPersonEntity LegalPerson
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderLegalPersonEntity> ("[LVA0A]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderLegalPersonEntity oldValue = this.LegalPerson;
				if (oldValue != value || !this.IsFieldDefined("[LVA0A]"))
				{
					this.OnLegalPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderLegalPersonEntity> ("[LVA0A]", oldValue, value);
					this.OnLegalPersonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LegalPersonContact</c> field.
		///	designer:fld/LVA73/LVA1A
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA1A]")]
		public global::Epsitec.Aider.Entities.AiderLegalPersonContactEntity LegalPersonContact
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderLegalPersonContactEntity> ("[LVA1A]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderLegalPersonContactEntity oldValue = this.LegalPersonContact;
				if (oldValue != value || !this.IsFieldDefined("[LVA1A]"))
				{
					this.OnLegalPersonContactChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderLegalPersonContactEntity> ("[LVA1A]", oldValue, value);
					this.OnLegalPersonContactChanged (oldValue, value);
				}
			}
		}
		
		partial void OnValidationStateChanging(global::Epsitec.Aider.Enumerations.ValidationState oldValue, global::Epsitec.Aider.Enumerations.ValidationState newValue);
		partial void OnValidationStateChanged(global::Epsitec.Aider.Enumerations.ValidationState oldValue, global::Epsitec.Aider.Enumerations.ValidationState newValue);
		partial void OnGroupChanging(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnGroupChanged(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnFunctionChanging(global::Epsitec.Aider.Entities.AiderFunctionDefEntity oldValue, global::Epsitec.Aider.Entities.AiderFunctionDefEntity newValue);
		partial void OnFunctionChanged(global::Epsitec.Aider.Entities.AiderFunctionDefEntity oldValue, global::Epsitec.Aider.Entities.AiderFunctionDefEntity newValue);
		partial void OnPersonChanging(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnPersonChanged(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnLegalPersonChanging(global::Epsitec.Aider.Entities.AiderLegalPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderLegalPersonEntity newValue);
		partial void OnLegalPersonChanged(global::Epsitec.Aider.Entities.AiderLegalPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderLegalPersonEntity newValue);
		partial void OnLegalPersonContactChanging(global::Epsitec.Aider.Entities.AiderLegalPersonContactEntity oldValue, global::Epsitec.Aider.Entities.AiderLegalPersonContactEntity newValue);
		partial void OnLegalPersonContactChanged(global::Epsitec.Aider.Entities.AiderLegalPersonContactEntity oldValue, global::Epsitec.Aider.Entities.AiderLegalPersonContactEntity newValue);
		
		
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
		///	designer:fld/LVA54/LVAB4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAB4]")]
		public string Description
		{
			get
			{
				return this.GetField<string> ("[LVAB4]");
			}
			set
			{
				string oldValue = this.Description;
				if (oldValue != value || !this.IsFieldDefined("[LVAB4]"))
				{
					this.OnDescriptionChanging (oldValue, value);
					this.SetField<string> ("[LVAB4]", oldValue, value);
					this.OnDescriptionChanged (oldValue, value);
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
		///	The <c>Root</c> field.
		///	designer:fld/LVA54/LVAHA
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAHA]")]
		public global::Epsitec.Aider.Entities.AiderGroupEntity Root
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderGroupEntity> ("[LVAHA]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderGroupEntity oldValue = this.Root;
				if (oldValue != value || !this.IsFieldDefined("[LVAHA]"))
				{
					this.OnRootChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderGroupEntity> ("[LVAHA]", oldValue, value);
					this.OnRootChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Participants</c> field.
		///	designer:fld/LVA54/LVAJ8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAJ8]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderGroupParticipantEntity> Participants
		{
			get
			{
				global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderGroupParticipantEntity> value = default (global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderGroupParticipantEntity>);
				this.GetParticipants (ref value);
				return value;
			}
		}
		
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		partial void OnDescriptionChanging(string oldValue, string newValue);
		partial void OnDescriptionChanged(string oldValue, string newValue);
		partial void OnGroupDefChanging(global::Epsitec.Aider.Entities.AiderGroupDefEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupDefEntity newValue);
		partial void OnGroupDefChanged(global::Epsitec.Aider.Entities.AiderGroupDefEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupDefEntity newValue);
		partial void OnRootChanging(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnRootChanged(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		
		partial void GetParticipants(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderGroupParticipantEntity> value);
		
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
		///	designer:fld/LVAR6/LVAT6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAT6]")]
		public global::Epsitec.Aider.Entities.AiderAddressEntity Address
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAT6]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderAddressEntity oldValue = this.Address;
				if (oldValue != value || !this.IsFieldDefined("[LVAT6]"))
				{
					this.OnAddressChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAT6]", oldValue, value);
					this.OnAddressChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTypeChanging(global::Epsitec.Aider.Enumerations.LegalPersonType oldValue, global::Epsitec.Aider.Enumerations.LegalPersonType newValue);
		partial void OnTypeChanged(global::Epsitec.Aider.Enumerations.LegalPersonType oldValue, global::Epsitec.Aider.Enumerations.LegalPersonType newValue);
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		partial void OnRemovalReasonChanging(global::Epsitec.Aider.Enumerations.RemovalReason oldValue, global::Epsitec.Aider.Enumerations.RemovalReason newValue);
		partial void OnRemovalReasonChanged(global::Epsitec.Aider.Enumerations.RemovalReason oldValue, global::Epsitec.Aider.Enumerations.RemovalReason newValue);
		partial void OnLanguageChanging(global::Epsitec.Aider.Enumerations.Language oldValue, global::Epsitec.Aider.Enumerations.Language newValue);
		partial void OnLanguageChanged(global::Epsitec.Aider.Enumerations.Language oldValue, global::Epsitec.Aider.Enumerations.Language newValue);
		partial void OnAddressChanging(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAddressChanged(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		
		
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

#region Epsitec.Aider.AiderContact Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderContact</c> entity.
	///	designer:cap/LVAB7
	///	</summary>
	public partial class AiderContactEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IManagedItem, global::Epsitec.Aider.Entities.IComment
	{
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVAB7/LVAA8
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
		///	designer:fld/LVAB7/LVAT7
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
		///	designer:fld/LVAB7/LVAU7
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
		///	The <c>Role</c> field.
		///	designer:fld/LVAB7/LVAJ7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAJ7]")]
		public global::Epsitec.Aider.Enumerations.ContactRole Role
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.ContactRole> ("[LVAJ7]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.ContactRole oldValue = this.Role;
				if (oldValue != value || !this.IsFieldDefined("[LVAJ7]"))
				{
					this.OnRoleChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.ContactRole> ("[LVAJ7]", oldValue, value);
					this.OnRoleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MrMrs</c> field.
		///	designer:fld/LVAB7/LVAD7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAD7]")]
		public global::Epsitec.Aider.Enumerations.PersonMrMrs MrMrs
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.PersonMrMrs> ("[LVAD7]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.PersonMrMrs oldValue = this.MrMrs;
				if (oldValue != value || !this.IsFieldDefined("[LVAD7]"))
				{
					this.OnMrMrsChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.PersonMrMrs> ("[LVAD7]", oldValue, value);
					this.OnMrMrsChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Title</c> field.
		///	designer:fld/LVAB7/LVAE7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAE7]")]
		public string Title
		{
			get
			{
				return this.GetField<string> ("[LVAE7]");
			}
			set
			{
				string oldValue = this.Title;
				if (oldValue != value || !this.IsFieldDefined("[LVAE7]"))
				{
					this.OnTitleChanging (oldValue, value);
					this.SetField<string> ("[LVAE7]", oldValue, value);
					this.OnTitleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>FirstName</c> field.
		///	designer:fld/LVAB7/LVAF7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAF7]")]
		public string FirstName
		{
			get
			{
				return this.GetField<string> ("[LVAF7]");
			}
			set
			{
				string oldValue = this.FirstName;
				if (oldValue != value || !this.IsFieldDefined("[LVAF7]"))
				{
					this.OnFirstNameChanging (oldValue, value);
					this.SetField<string> ("[LVAF7]", oldValue, value);
					this.OnFirstNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LastName</c> field.
		///	designer:fld/LVAB7/LVAG7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAG7]")]
		public string LastName
		{
			get
			{
				return this.GetField<string> ("[LVAG7]");
			}
			set
			{
				string oldValue = this.LastName;
				if (oldValue != value || !this.IsFieldDefined("[LVAG7]"))
				{
					this.OnLastNameChanging (oldValue, value);
					this.SetField<string> ("[LVAG7]", oldValue, value);
					this.OnLastNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Address</c> field.
		///	designer:fld/LVAB7/LVAB8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAB8]")]
		public global::Epsitec.Aider.Entities.AiderAddressEntity Address
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAB8]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderAddressEntity oldValue = this.Address;
				if (oldValue != value || !this.IsFieldDefined("[LVAB8]"))
				{
					this.OnAddressChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAB8]", oldValue, value);
					this.OnAddressChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AdditionalAddress1</c> field.
		///	designer:fld/LVAB7/LVAC8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAC8]")]
		public global::Epsitec.Aider.Entities.AiderAddressEntity AdditionalAddress1
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAC8]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderAddressEntity oldValue = this.AdditionalAddress1;
				if (oldValue != value || !this.IsFieldDefined("[LVAC8]"))
				{
					this.OnAdditionalAddress1Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAC8]", oldValue, value);
					this.OnAdditionalAddress1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AdditionalAddress2</c> field.
		///	designer:fld/LVAB7/LVAD8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAD8]")]
		public global::Epsitec.Aider.Entities.AiderAddressEntity AdditionalAddress2
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAD8]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderAddressEntity oldValue = this.AdditionalAddress2;
				if (oldValue != value || !this.IsFieldDefined("[LVAD8]"))
				{
					this.OnAdditionalAddress2Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAD8]", oldValue, value);
					this.OnAdditionalAddress2Changed (oldValue, value);
				}
			}
		}
		
		partial void OnRoleChanging(global::Epsitec.Aider.Enumerations.ContactRole oldValue, global::Epsitec.Aider.Enumerations.ContactRole newValue);
		partial void OnRoleChanged(global::Epsitec.Aider.Enumerations.ContactRole oldValue, global::Epsitec.Aider.Enumerations.ContactRole newValue);
		partial void OnMrMrsChanging(global::Epsitec.Aider.Enumerations.PersonMrMrs oldValue, global::Epsitec.Aider.Enumerations.PersonMrMrs newValue);
		partial void OnMrMrsChanged(global::Epsitec.Aider.Enumerations.PersonMrMrs oldValue, global::Epsitec.Aider.Enumerations.PersonMrMrs newValue);
		partial void OnTitleChanging(string oldValue, string newValue);
		partial void OnTitleChanged(string oldValue, string newValue);
		partial void OnFirstNameChanging(string oldValue, string newValue);
		partial void OnFirstNameChanged(string oldValue, string newValue);
		partial void OnLastNameChanging(string oldValue, string newValue);
		partial void OnLastNameChanged(string oldValue, string newValue);
		partial void OnAddressChanging(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAddressChanged(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAdditionalAddress1Changing(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAdditionalAddress1Changed(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAdditionalAddress2Changing(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAdditionalAddress2Changed(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderContactEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderContactEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 235);	// [LVAB7]
		public static readonly string EntityStructuredTypeKey = "[LVAB7]";
		
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
		///	The <c>UserGroup</c> field.
		///	designer:fld/LVAS7/LVAA9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA9]")]
		public global::Epsitec.Aider.Entities.SoftwareUserGroupEntity UserGroup
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.SoftwareUserGroupEntity> ("[LVAA9]");
			}
			set
			{
				global::Epsitec.Aider.Entities.SoftwareUserGroupEntity oldValue = this.UserGroup;
				if (oldValue != value || !this.IsFieldDefined("[LVAA9]"))
				{
					this.OnUserGroupChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.SoftwareUserGroupEntity> ("[LVAA9]", oldValue, value);
					this.OnUserGroupChanged (oldValue, value);
				}
			}
		}
		
		partial void OnUserGroupChanging(global::Epsitec.Aider.Entities.SoftwareUserGroupEntity oldValue, global::Epsitec.Aider.Entities.SoftwareUserGroupEntity newValue);
		partial void OnUserGroupChanged(global::Epsitec.Aider.Entities.SoftwareUserGroupEntity oldValue, global::Epsitec.Aider.Entities.SoftwareUserGroupEntity newValue);
		
		
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

#region Epsitec.Aider.SoftwareUser Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>SoftwareUser</c> entity.
	///	designer:cap/LVA08
	///	</summary>
	public partial class SoftwareUserEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IDateRange, global::Epsitec.Aider.Entities.IComment
	{
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVA08/LVAA8
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
		///	designer:fld/LVA08/LVAN3
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
		///	designer:fld/LVA08/LVAO3
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
		///	The <c>UserName</c> field.
		///	designer:fld/LVA08/LVA89
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA89]")]
		public string UserName
		{
			get
			{
				return this.GetField<string> ("[LVA89]");
			}
			set
			{
				string oldValue = this.UserName;
				if (oldValue != value || !this.IsFieldDefined("[LVA89]"))
				{
					this.OnUserNameChanging (oldValue, value);
					this.SetField<string> ("[LVA89]", oldValue, value);
					this.OnUserNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Groups</c> field.
		///	designer:fld/LVA08/LVA79
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA79]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.SoftwareUserGroupEntity> Groups
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Aider.Entities.SoftwareUserGroupEntity> ("[LVA79]");
			}
		}
		
		partial void OnUserNameChanging(string oldValue, string newValue);
		partial void OnUserNameChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.SoftwareUserEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.SoftwareUserEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 256);	// [LVA08]
		public static readonly string EntityStructuredTypeKey = "[LVA08]";
		
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

#region Epsitec.Aider.AiderLegalPersonContact Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>AiderLegalPersonContact</c> entity.
	///	designer:cap/LVA38
	///	</summary>
	public partial class AiderLegalPersonContactEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IDateRange, global::Epsitec.Aider.Entities.IComment
	{
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVA38/LVAA8
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
		///	designer:fld/LVA38/LVAN3
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
		///	designer:fld/LVA38/LVAO3
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
		///	The <c>LegalPerson</c> field.
		///	designer:fld/LVA38/LVA58
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA58]")]
		public global::Epsitec.Aider.Entities.AiderLegalPersonEntity LegalPerson
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderLegalPersonEntity> ("[LVA58]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderLegalPersonEntity oldValue = this.LegalPerson;
				if (oldValue != value || !this.IsFieldDefined("[LVA58]"))
				{
					this.OnLegalPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderLegalPersonEntity> ("[LVA58]", oldValue, value);
					this.OnLegalPersonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Person</c> field.
		///	designer:fld/LVA38/LVAV9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAV9]")]
		public global::Epsitec.Aider.Entities.AiderPersonEntity Person
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVAV9]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderPersonEntity oldValue = this.Person;
				if (oldValue != value || !this.IsFieldDefined("[LVAV9]"))
				{
					this.OnPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderPersonEntity> ("[LVAV9]", oldValue, value);
					this.OnPersonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Function</c> field.
		///	designer:fld/LVA38/LVAGA
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAGA]")]
		public global::Epsitec.Aider.Entities.AiderFunctionDefEntity Function
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderFunctionDefEntity> ("[LVAGA]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderFunctionDefEntity oldValue = this.Function;
				if (oldValue != value || !this.IsFieldDefined("[LVAGA]"))
				{
					this.OnFunctionChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderFunctionDefEntity> ("[LVAGA]", oldValue, value);
					this.OnFunctionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MrMrs</c> field.
		///	designer:fld/LVA38/LVAR9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAR9]")]
		public global::Epsitec.Aider.Enumerations.PersonMrMrs MrMrs
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.PersonMrMrs> ("[LVAR9]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.PersonMrMrs oldValue = this.MrMrs;
				if (oldValue != value || !this.IsFieldDefined("[LVAR9]"))
				{
					this.OnMrMrsChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.PersonMrMrs> ("[LVAR9]", oldValue, value);
					this.OnMrMrsChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PersonSex</c> field.
		///	designer:fld/LVA38/LVAS9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAS9]")]
		public global::Epsitec.Aider.Enumerations.PersonSex PersonSex
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.PersonSex> ("[LVAS9]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.PersonSex oldValue = this.PersonSex;
				if (oldValue != value || !this.IsFieldDefined("[LVAS9]"))
				{
					this.OnPersonSexChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.PersonSex> ("[LVAS9]", oldValue, value);
					this.OnPersonSexChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Title</c> field.
		///	designer:fld/LVA38/LVAM9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAM9]")]
		public string Title
		{
			get
			{
				return this.GetField<string> ("[LVAM9]");
			}
			set
			{
				string oldValue = this.Title;
				if (oldValue != value || !this.IsFieldDefined("[LVAM9]"))
				{
					this.OnTitleChanging (oldValue, value);
					this.SetField<string> ("[LVAM9]", oldValue, value);
					this.OnTitleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>FirstName</c> field.
		///	designer:fld/LVA38/LVAT9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAT9]")]
		public string FirstName
		{
			get
			{
				return this.GetField<string> ("[LVAT9]");
			}
			set
			{
				string oldValue = this.FirstName;
				if (oldValue != value || !this.IsFieldDefined("[LVAT9]"))
				{
					this.OnFirstNameChanging (oldValue, value);
					this.SetField<string> ("[LVAT9]", oldValue, value);
					this.OnFirstNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LastName</c> field.
		///	designer:fld/LVA38/LVAU9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAU9]")]
		public string LastName
		{
			get
			{
				return this.GetField<string> ("[LVAU9]");
			}
			set
			{
				string oldValue = this.LastName;
				if (oldValue != value || !this.IsFieldDefined("[LVAU9]"))
				{
					this.OnLastNameChanging (oldValue, value);
					this.SetField<string> ("[LVAU9]", oldValue, value);
					this.OnLastNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AdditionalAddress1</c> field.
		///	designer:fld/LVA38/LVAP9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAP9]")]
		public global::Epsitec.Aider.Entities.AiderAddressEntity AdditionalAddress1
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAP9]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderAddressEntity oldValue = this.AdditionalAddress1;
				if (oldValue != value || !this.IsFieldDefined("[LVAP9]"))
				{
					this.OnAdditionalAddress1Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAP9]", oldValue, value);
					this.OnAdditionalAddress1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AdditionalAddress2</c> field.
		///	designer:fld/LVA38/LVAQ9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAQ9]")]
		public global::Epsitec.Aider.Entities.AiderAddressEntity AdditionalAddress2
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAQ9]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderAddressEntity oldValue = this.AdditionalAddress2;
				if (oldValue != value || !this.IsFieldDefined("[LVAQ9]"))
				{
					this.OnAdditionalAddress2Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderAddressEntity> ("[LVAQ9]", oldValue, value);
					this.OnAdditionalAddress2Changed (oldValue, value);
				}
			}
		}
		
		partial void OnLegalPersonChanging(global::Epsitec.Aider.Entities.AiderLegalPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderLegalPersonEntity newValue);
		partial void OnLegalPersonChanged(global::Epsitec.Aider.Entities.AiderLegalPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderLegalPersonEntity newValue);
		partial void OnPersonChanging(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnPersonChanged(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnFunctionChanging(global::Epsitec.Aider.Entities.AiderFunctionDefEntity oldValue, global::Epsitec.Aider.Entities.AiderFunctionDefEntity newValue);
		partial void OnFunctionChanged(global::Epsitec.Aider.Entities.AiderFunctionDefEntity oldValue, global::Epsitec.Aider.Entities.AiderFunctionDefEntity newValue);
		partial void OnMrMrsChanging(global::Epsitec.Aider.Enumerations.PersonMrMrs oldValue, global::Epsitec.Aider.Enumerations.PersonMrMrs newValue);
		partial void OnMrMrsChanged(global::Epsitec.Aider.Enumerations.PersonMrMrs oldValue, global::Epsitec.Aider.Enumerations.PersonMrMrs newValue);
		partial void OnPersonSexChanging(global::Epsitec.Aider.Enumerations.PersonSex oldValue, global::Epsitec.Aider.Enumerations.PersonSex newValue);
		partial void OnPersonSexChanged(global::Epsitec.Aider.Enumerations.PersonSex oldValue, global::Epsitec.Aider.Enumerations.PersonSex newValue);
		partial void OnTitleChanging(string oldValue, string newValue);
		partial void OnTitleChanged(string oldValue, string newValue);
		partial void OnFirstNameChanging(string oldValue, string newValue);
		partial void OnFirstNameChanged(string oldValue, string newValue);
		partial void OnLastNameChanging(string oldValue, string newValue);
		partial void OnLastNameChanged(string oldValue, string newValue);
		partial void OnAdditionalAddress1Changing(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAdditionalAddress1Changed(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAdditionalAddress2Changing(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAdditionalAddress2Changed(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.AiderLegalPersonContactEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.AiderLegalPersonContactEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 259);	// [LVA38]
		public static readonly string EntityStructuredTypeKey = "[LVA38]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AiderLegalPersonContactEntity>
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
		
		partial void OnTextChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		
		
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

#region Epsitec.Aider.SoftwareUserGroup Entity
namespace Epsitec.Aider.Entities
{
	///	<summary>
	///	The <c>SoftwareUserGroup</c> entity.
	///	designer:cap/LVA69
	///	</summary>
	public partial class SoftwareUserGroupEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IDateRange, global::Epsitec.Aider.Entities.IComment
	{
		#region IComment Members
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVA69/LVAA8
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
		///	designer:fld/LVA69/LVAN3
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
		///	designer:fld/LVA69/LVAO3
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
		///	The <c>GroupName</c> field.
		///	designer:fld/LVA69/LVA99
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA99]")]
		public string GroupName
		{
			get
			{
				return this.GetField<string> ("[LVA99]");
			}
			set
			{
				string oldValue = this.GroupName;
				if (oldValue != value || !this.IsFieldDefined("[LVA99]"))
				{
					this.OnGroupNameChanging (oldValue, value);
					this.SetField<string> ("[LVA99]", oldValue, value);
					this.OnGroupNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Users</c> field.
		///	designer:fld/LVA69/LVAB9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAB9]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.SoftwareUserEntity> Users
		{
			get
			{
				global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.SoftwareUserEntity> value = default (global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.SoftwareUserEntity>);
				this.GetUsers (ref value);
				return value;
			}
		}
		
		partial void OnGroupNameChanging(string oldValue, string newValue);
		partial void OnGroupNameChanged(string oldValue, string newValue);
		
		partial void GetUsers(ref global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.SoftwareUserEntity> value);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Aider.Entities.SoftwareUserGroupEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Aider.Entities.SoftwareUserGroupEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1013, 10, 294);	// [LVA69]
		public static readonly string EntityStructuredTypeKey = "[LVA69]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<SoftwareUserGroupEntity>
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
		///	The <c>Level</c> field.
		///	designer:fld/LVA2A/LVAAA
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAAA]")]
		public global::Epsitec.Aider.Enumerations.GroupLevel Level
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.GroupLevel> ("[LVAAA]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.GroupLevel oldValue = this.Level;
				if (oldValue != value || !this.IsFieldDefined("[LVAAA]"))
				{
					this.OnLevelChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.GroupLevel> ("[LVAAA]", oldValue, value);
					this.OnLevelChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Type</c> field.
		///	designer:fld/LVA2A/LVA1B
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA1B]")]
		public global::Epsitec.Aider.Enumerations.GroupType Type
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Enumerations.GroupType> ("[LVA1B]");
			}
			set
			{
				global::Epsitec.Aider.Enumerations.GroupType oldValue = this.Type;
				if (oldValue != value || !this.IsFieldDefined("[LVA1B]"))
				{
					this.OnTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Enumerations.GroupType> ("[LVA1B]", oldValue, value);
					this.OnTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Category</c> field.
		///	designer:fld/LVA2A/LVA2B
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA2B]")]
		public global::Epsitec.Common.Types.UnresolvedEnum Category
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.UnresolvedEnum> ("[LVA2B]");
			}
			set
			{
				global::Epsitec.Common.Types.UnresolvedEnum oldValue = this.Category;
				if (oldValue != value || !this.IsFieldDefined("[LVA2B]"))
				{
					this.OnCategoryChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.UnresolvedEnum> ("[LVA2B]", oldValue, value);
					this.OnCategoryChanged (oldValue, value);
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
		///	<summary>
		///	The <c>MinOccurs</c> field.
		///	designer:fld/LVA2A/LVA5A
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA5A]")]
		public int MinOccurs
		{
			get
			{
				return this.GetField<int> ("[LVA5A]");
			}
			set
			{
				int oldValue = this.MinOccurs;
				if (oldValue != value || !this.IsFieldDefined("[LVA5A]"))
				{
					this.OnMinOccursChanging (oldValue, value);
					this.SetField<int> ("[LVA5A]", oldValue, value);
					this.OnMinOccursChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MaxOccurs</c> field.
		///	designer:fld/LVA2A/LVA6A
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA6A]")]
		public int MaxOccurs
		{
			get
			{
				return this.GetField<int> ("[LVA6A]");
			}
			set
			{
				int oldValue = this.MaxOccurs;
				if (oldValue != value || !this.IsFieldDefined("[LVA6A]"))
				{
					this.OnMaxOccursChanging (oldValue, value);
					this.SetField<int> ("[LVA6A]", oldValue, value);
					this.OnMaxOccursChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Functions</c> field.
		///	designer:fld/LVA2A/LVA9A
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA9A]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Aider.Entities.AiderFunctionDefEntity> Functions
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Aider.Entities.AiderFunctionDefEntity> ("[LVA9A]");
			}
		}
		
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		partial void OnLevelChanging(global::Epsitec.Aider.Enumerations.GroupLevel oldValue, global::Epsitec.Aider.Enumerations.GroupLevel newValue);
		partial void OnLevelChanged(global::Epsitec.Aider.Enumerations.GroupLevel oldValue, global::Epsitec.Aider.Enumerations.GroupLevel newValue);
		partial void OnTypeChanging(global::Epsitec.Aider.Enumerations.GroupType oldValue, global::Epsitec.Aider.Enumerations.GroupType newValue);
		partial void OnTypeChanged(global::Epsitec.Aider.Enumerations.GroupType oldValue, global::Epsitec.Aider.Enumerations.GroupType newValue);
		partial void OnCategoryChanging(global::Epsitec.Common.Types.UnresolvedEnum oldValue, global::Epsitec.Common.Types.UnresolvedEnum newValue);
		partial void OnCategoryChanged(global::Epsitec.Common.Types.UnresolvedEnum oldValue, global::Epsitec.Common.Types.UnresolvedEnum newValue);
		partial void OnMinOccursChanging(int oldValue, int newValue);
		partial void OnMinOccursChanged(int oldValue, int newValue);
		partial void OnMaxOccursChanging(int oldValue, int newValue);
		partial void OnMaxOccursChanged(int oldValue, int newValue);
		
		
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
		///	<summary>
		///	The <c>Type</c> field.
		///	designer:fld/LVA7A/LVAFA
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAFA]")]
		public global::Epsitec.Common.Types.UnresolvedEnum Type
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.UnresolvedEnum> ("[LVAFA]");
			}
			set
			{
				global::Epsitec.Common.Types.UnresolvedEnum oldValue = this.Type;
				if (oldValue != value || !this.IsFieldDefined("[LVAFA]"))
				{
					this.OnTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.UnresolvedEnum> ("[LVAFA]", oldValue, value);
					this.OnTypeChanged (oldValue, value);
				}
			}
		}
		
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		partial void OnTypeChanging(global::Epsitec.Common.Types.UnresolvedEnum oldValue, global::Epsitec.Common.Types.UnresolvedEnum newValue);
		partial void OnTypeChanged(global::Epsitec.Common.Types.UnresolvedEnum oldValue, global::Epsitec.Common.Types.UnresolvedEnum newValue);
		
		
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

