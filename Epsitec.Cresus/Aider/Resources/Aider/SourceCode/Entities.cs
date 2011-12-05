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
		public global::Epsitec.Aider.eCH.DataSource DataSource
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.eCH.DataSource> ("[LVAA]");
			}
			set
			{
				global::Epsitec.Aider.eCH.DataSource oldValue = this.DataSource;
				if (oldValue != value || !this.IsFieldDefined("[LVAA]"))
				{
					this.OnDataSourceChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.eCH.DataSource> ("[LVAA]", oldValue, value);
					this.OnDataSourceChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DeclarationStatus</c> field.
		///	designer:fld/LVA/LVAS2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAS2]")]
		public global::Epsitec.Aider.eCH.PersonDeclarationStatus DeclarationStatus
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.eCH.PersonDeclarationStatus> ("[LVAS2]");
			}
			set
			{
				global::Epsitec.Aider.eCH.PersonDeclarationStatus oldValue = this.DeclarationStatus;
				if (oldValue != value || !this.IsFieldDefined("[LVAS2]"))
				{
					this.OnDeclarationStatusChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.eCH.PersonDeclarationStatus> ("[LVAS2]", oldValue, value);
					this.OnDeclarationStatusChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>RemovalReason</c> field.
		///	designer:fld/LVA/LVAQ6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAQ6]")]
		public global::Epsitec.Aider.eCH.RemovalReason RemovalReason
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.eCH.RemovalReason> ("[LVAQ6]");
			}
			set
			{
				global::Epsitec.Aider.eCH.RemovalReason oldValue = this.RemovalReason;
				if (oldValue != value || !this.IsFieldDefined("[LVAQ6]"))
				{
					this.OnRemovalReasonChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.eCH.RemovalReason> ("[LVAQ6]", oldValue, value);
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
		///	designer:fld/LVA/LVA4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA4]")]
		public global::Epsitec.Common.Types.Date? PersonDateOfBirth
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[LVA4]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.PersonDateOfBirth;
				if (oldValue != value || !this.IsFieldDefined("[LVA4]"))
				{
					this.OnPersonDateOfBirthChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[LVA4]", oldValue, value);
					this.OnPersonDateOfBirthChanged (oldValue, value);
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
		public global::Epsitec.Aider.eCH.DatePrecision PersonDateOfBirthType
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.eCH.DatePrecision> ("[LVA5]");
			}
			set
			{
				global::Epsitec.Aider.eCH.DatePrecision oldValue = this.PersonDateOfBirthType;
				if (oldValue != value || !this.IsFieldDefined("[LVA5]"))
				{
					this.OnPersonDateOfBirthTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.eCH.DatePrecision> ("[LVA5]", oldValue, value);
					this.OnPersonDateOfBirthTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PersonSex</c> field.
		///	designer:fld/LVA/LVA6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA6]")]
		public global::Epsitec.Aider.eCH.PersonSex PersonSex
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.eCH.PersonSex> ("[LVA6]");
			}
			set
			{
				global::Epsitec.Aider.eCH.PersonSex oldValue = this.PersonSex;
				if (oldValue != value || !this.IsFieldDefined("[LVA6]"))
				{
					this.OnPersonSexChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.eCH.PersonSex> ("[LVA6]", oldValue, value);
					this.OnPersonSexChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>NationalityStatus</c> field.
		///	designer:fld/LVA/LVA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA8]")]
		public global::Epsitec.Aider.eCH.PersonNationalityStatus NationalityStatus
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.eCH.PersonNationalityStatus> ("[LVA8]");
			}
			set
			{
				global::Epsitec.Aider.eCH.PersonNationalityStatus oldValue = this.NationalityStatus;
				if (oldValue != value || !this.IsFieldDefined("[LVA8]"))
				{
					this.OnNationalityStatusChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.eCH.PersonNationalityStatus> ("[LVA8]", oldValue, value);
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
		public global::Epsitec.Aider.eCH.PersonMaritalStatus AdultMaritalStatus
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.eCH.PersonMaritalStatus> ("[LVA9]");
			}
			set
			{
				global::Epsitec.Aider.eCH.PersonMaritalStatus oldValue = this.AdultMaritalStatus;
				if (oldValue != value || !this.IsFieldDefined("[LVA9]"))
				{
					this.OnAdultMaritalStatusChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.eCH.PersonMaritalStatus> ("[LVA9]", oldValue, value);
					this.OnAdultMaritalStatusChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Address</c> field.
		///	designer:fld/LVA/LVAC2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAC2]")]
		public global::Epsitec.Aider.Entities.eCH_AddressEntity Address
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.eCH_AddressEntity> ("[LVAC2]");
			}
			set
			{
				global::Epsitec.Aider.Entities.eCH_AddressEntity oldValue = this.Address;
				if (oldValue != value || !this.IsFieldDefined("[LVAC2]"))
				{
					this.OnAddressChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.eCH_AddressEntity> ("[LVAC2]", oldValue, value);
					this.OnAddressChanged (oldValue, value);
				}
			}
		}
		
		partial void OnCreationDateChanging(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnCreationDateChanged(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnReportedPerson1Changing(global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity oldValue, global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity newValue);
		partial void OnReportedPerson1Changed(global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity oldValue, global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity newValue);
		partial void OnReportedPerson2Changing(global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity oldValue, global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity newValue);
		partial void OnReportedPerson2Changed(global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity oldValue, global::Epsitec.Aider.Entities.eCH_ReportedPersonEntity newValue);
		partial void OnDataSourceChanging(global::Epsitec.Aider.eCH.DataSource oldValue, global::Epsitec.Aider.eCH.DataSource newValue);
		partial void OnDataSourceChanged(global::Epsitec.Aider.eCH.DataSource oldValue, global::Epsitec.Aider.eCH.DataSource newValue);
		partial void OnDeclarationStatusChanging(global::Epsitec.Aider.eCH.PersonDeclarationStatus oldValue, global::Epsitec.Aider.eCH.PersonDeclarationStatus newValue);
		partial void OnDeclarationStatusChanged(global::Epsitec.Aider.eCH.PersonDeclarationStatus oldValue, global::Epsitec.Aider.eCH.PersonDeclarationStatus newValue);
		partial void OnRemovalReasonChanging(global::Epsitec.Aider.eCH.RemovalReason oldValue, global::Epsitec.Aider.eCH.RemovalReason newValue);
		partial void OnRemovalReasonChanged(global::Epsitec.Aider.eCH.RemovalReason oldValue, global::Epsitec.Aider.eCH.RemovalReason newValue);
		partial void OnPersonIdChanging(string oldValue, string newValue);
		partial void OnPersonIdChanged(string oldValue, string newValue);
		partial void OnPersonOfficialNameChanging(string oldValue, string newValue);
		partial void OnPersonOfficialNameChanged(string oldValue, string newValue);
		partial void OnPersonFirstNamesChanging(string oldValue, string newValue);
		partial void OnPersonFirstNamesChanged(string oldValue, string newValue);
		partial void OnPersonDateOfBirthChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnPersonDateOfBirthChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnPersonDateOfBirthDayChanging(int? oldValue, int? newValue);
		partial void OnPersonDateOfBirthDayChanged(int? oldValue, int? newValue);
		partial void OnPersonDateOfBirthMonthChanging(int? oldValue, int? newValue);
		partial void OnPersonDateOfBirthMonthChanged(int? oldValue, int? newValue);
		partial void OnPersonDateOfBirthYearChanging(int? oldValue, int? newValue);
		partial void OnPersonDateOfBirthYearChanged(int? oldValue, int? newValue);
		partial void OnPersonDateOfBirthTypeChanging(global::Epsitec.Aider.eCH.DatePrecision oldValue, global::Epsitec.Aider.eCH.DatePrecision newValue);
		partial void OnPersonDateOfBirthTypeChanged(global::Epsitec.Aider.eCH.DatePrecision oldValue, global::Epsitec.Aider.eCH.DatePrecision newValue);
		partial void OnPersonSexChanging(global::Epsitec.Aider.eCH.PersonSex oldValue, global::Epsitec.Aider.eCH.PersonSex newValue);
		partial void OnPersonSexChanged(global::Epsitec.Aider.eCH.PersonSex oldValue, global::Epsitec.Aider.eCH.PersonSex newValue);
		partial void OnNationalityStatusChanging(global::Epsitec.Aider.eCH.PersonNationalityStatus oldValue, global::Epsitec.Aider.eCH.PersonNationalityStatus newValue);
		partial void OnNationalityStatusChanged(global::Epsitec.Aider.eCH.PersonNationalityStatus oldValue, global::Epsitec.Aider.eCH.PersonNationalityStatus newValue);
		partial void OnNationalityCountryCodeChanging(string oldValue, string newValue);
		partial void OnNationalityCountryCodeChanged(string oldValue, string newValue);
		partial void OnOriginsChanging(string oldValue, string newValue);
		partial void OnOriginsChanged(string oldValue, string newValue);
		partial void OnAdultMaritalStatusChanging(global::Epsitec.Aider.eCH.PersonMaritalStatus oldValue, global::Epsitec.Aider.eCH.PersonMaritalStatus newValue);
		partial void OnAdultMaritalStatusChanged(global::Epsitec.Aider.eCH.PersonMaritalStatus oldValue, global::Epsitec.Aider.eCH.PersonMaritalStatus newValue);
		partial void OnAddressChanging(global::Epsitec.Aider.Entities.eCH_AddressEntity oldValue, global::Epsitec.Aider.Entities.eCH_AddressEntity newValue);
		partial void OnAddressChanged(global::Epsitec.Aider.Entities.eCH_AddressEntity oldValue, global::Epsitec.Aider.Entities.eCH_AddressEntity newValue);
		
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
	public partial class AiderPersonEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
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
		///	The <c>ValidationState</c> field.
		///	designer:fld/LVAF/LVAH6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAH6]")]
		public global::Epsitec.Aider.ValidationState ValidationState
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.ValidationState> ("[LVAH6]");
			}
			set
			{
				global::Epsitec.Aider.ValidationState oldValue = this.ValidationState;
				if (oldValue != value || !this.IsFieldDefined("[LVAH6]"))
				{
					this.OnValidationStateChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.ValidationState> ("[LVAH6]", oldValue, value);
					this.OnValidationStateChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MrMrs</c> field.
		///	designer:fld/LVAF/LVAT
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAT]")]
		public global::Epsitec.Aider.eCH.PersonMrMrs MrMrs
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.eCH.PersonMrMrs> ("[LVAT]");
			}
			set
			{
				global::Epsitec.Aider.eCH.PersonMrMrs oldValue = this.MrMrs;
				if (oldValue != value || !this.IsFieldDefined("[LVAT]"))
				{
					this.OnMrMrsChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.eCH.PersonMrMrs> ("[LVAT]", oldValue, value);
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
		///	The <c>Household</c> field.
		///	designer:fld/LVAF/LVAL4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAL4]")]
		public global::Epsitec.Aider.Entities.AiderHouseholdEntity Household
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderHouseholdEntity> ("[LVAL4]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderHouseholdEntity oldValue = this.Household;
				if (oldValue != value || !this.IsFieldDefined("[LVAL4]"))
				{
					this.OnHouseholdChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderHouseholdEntity> ("[LVAL4]", oldValue, value);
					this.OnHouseholdChanged (oldValue, value);
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
		///	The <c>Confession</c> field.
		///	designer:fld/LVAF/LVAI5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAI5]")]
		public global::Epsitec.Aider.PersonConfession Confession
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.PersonConfession> ("[LVAI5]");
			}
			set
			{
				global::Epsitec.Aider.PersonConfession oldValue = this.Confession;
				if (oldValue != value || !this.IsFieldDefined("[LVAI5]"))
				{
					this.OnConfessionChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.PersonConfession> ("[LVAI5]", oldValue, value);
					this.OnConfessionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Language</c> field.
		///	designer:fld/LVAF/LVAO7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAO7]")]
		public global::Epsitec.Aider.Language Language
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Language> ("[LVAO7]");
			}
			set
			{
				global::Epsitec.Aider.Language oldValue = this.Language;
				if (oldValue != value || !this.IsFieldDefined("[LVAO7]"))
				{
					this.OnLanguageChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Language> ("[LVAO7]", oldValue, value);
					this.OnLanguageChanged (oldValue, value);
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
		///	The <c>Comment</c> field.
		///	designer:fld/LVAF/LVAJ6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAJ6]")]
		public string Comment
		{
			get
			{
				return this.GetField<string> ("[LVAJ6]");
			}
			set
			{
				string oldValue = this.Comment;
				if (oldValue != value || !this.IsFieldDefined("[LVAJ6]"))
				{
					this.OnCommentChanging (oldValue, value);
					this.SetField<string> ("[LVAJ6]", oldValue, value);
					this.OnCommentChanged (oldValue, value);
				}
			}
		}
		
		partial void OneCH_PersonChanging(global::Epsitec.Aider.Entities.eCH_PersonEntity oldValue, global::Epsitec.Aider.Entities.eCH_PersonEntity newValue);
		partial void OneCH_PersonChanged(global::Epsitec.Aider.Entities.eCH_PersonEntity oldValue, global::Epsitec.Aider.Entities.eCH_PersonEntity newValue);
		partial void OnCodeIdChanging(string oldValue, string newValue);
		partial void OnCodeIdChanged(string oldValue, string newValue);
		partial void OnValidationStateChanging(global::Epsitec.Aider.ValidationState oldValue, global::Epsitec.Aider.ValidationState newValue);
		partial void OnValidationStateChanged(global::Epsitec.Aider.ValidationState oldValue, global::Epsitec.Aider.ValidationState newValue);
		partial void OnMrMrsChanging(global::Epsitec.Aider.eCH.PersonMrMrs oldValue, global::Epsitec.Aider.eCH.PersonMrMrs newValue);
		partial void OnMrMrsChanged(global::Epsitec.Aider.eCH.PersonMrMrs oldValue, global::Epsitec.Aider.eCH.PersonMrMrs newValue);
		partial void OnTitleChanging(string oldValue, string newValue);
		partial void OnTitleChanged(string oldValue, string newValue);
		partial void OnCallNameChanging(string oldValue, string newValue);
		partial void OnCallNameChanged(string oldValue, string newValue);
		partial void OnOriginalNameChanging(string oldValue, string newValue);
		partial void OnOriginalNameChanged(string oldValue, string newValue);
		partial void OnDisplayNameChanging(string oldValue, string newValue);
		partial void OnDisplayNameChanged(string oldValue, string newValue);
		partial void OnHouseholdChanging(global::Epsitec.Aider.Entities.AiderHouseholdEntity oldValue, global::Epsitec.Aider.Entities.AiderHouseholdEntity newValue);
		partial void OnHouseholdChanged(global::Epsitec.Aider.Entities.AiderHouseholdEntity oldValue, global::Epsitec.Aider.Entities.AiderHouseholdEntity newValue);
		partial void OnAdditionalAddress1Changing(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAdditionalAddress1Changed(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAdditionalAddress2Changing(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAdditionalAddress2Changed(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnConfessionChanging(global::Epsitec.Aider.PersonConfession oldValue, global::Epsitec.Aider.PersonConfession newValue);
		partial void OnConfessionChanged(global::Epsitec.Aider.PersonConfession oldValue, global::Epsitec.Aider.PersonConfession newValue);
		partial void OnLanguageChanging(global::Epsitec.Aider.Language oldValue, global::Epsitec.Aider.Language newValue);
		partial void OnLanguageChanged(global::Epsitec.Aider.Language oldValue, global::Epsitec.Aider.Language newValue);
		partial void OnProfessionChanging(string oldValue, string newValue);
		partial void OnProfessionChanged(string oldValue, string newValue);
		partial void OnCommentChanging(string oldValue, string newValue);
		partial void OnCommentChanged(string oldValue, string newValue);
		
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
	public partial class AiderHouseholdEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
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
		
		partial void OnAddressChanging(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAddressChanged(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnHead1Changing(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnHead1Changed(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnHead2Changing(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnHead2Changed(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		
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
	public partial class AiderAddressEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Type</c> field.
		///	designer:fld/LVAJ2/LVAE6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAE6]")]
		public global::Epsitec.Aider.AddressType Type
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.AddressType> ("[LVAE6]");
			}
			set
			{
				global::Epsitec.Aider.AddressType oldValue = this.Type;
				if (oldValue != value || !this.IsFieldDefined("[LVAE6]"))
				{
					this.OnTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.AddressType> ("[LVAE6]", oldValue, value);
					this.OnTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/LVAJ2/LVAO5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAO5]")]
		public string Description
		{
			get
			{
				return this.GetField<string> ("[LVAO5]");
			}
			set
			{
				string oldValue = this.Description;
				if (oldValue != value || !this.IsFieldDefined("[LVAO5]"))
				{
					this.OnDescriptionChanging (oldValue, value);
					this.SetField<string> ("[LVAO5]", oldValue, value);
					this.OnDescriptionChanged (oldValue, value);
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
		
		partial void OnTypeChanging(global::Epsitec.Aider.AddressType oldValue, global::Epsitec.Aider.AddressType newValue);
		partial void OnTypeChanged(global::Epsitec.Aider.AddressType oldValue, global::Epsitec.Aider.AddressType newValue);
		partial void OnDescriptionChanging(string oldValue, string newValue);
		partial void OnDescriptionChanged(string oldValue, string newValue);
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
		public global::Epsitec.Aider.PersonRelationshipType Type
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.PersonRelationshipType> ("[LVAL3]");
			}
			set
			{
				global::Epsitec.Aider.PersonRelationshipType oldValue = this.Type;
				if (oldValue != value || !this.IsFieldDefined("[LVAL3]"))
				{
					this.OnTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.PersonRelationshipType> ("[LVAL3]", oldValue, value);
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
		
		partial void OnTypeChanging(global::Epsitec.Aider.PersonRelationshipType oldValue, global::Epsitec.Aider.PersonRelationshipType newValue);
		partial void OnTypeChanged(global::Epsitec.Aider.PersonRelationshipType oldValue, global::Epsitec.Aider.PersonRelationshipType newValue);
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
	public partial class AiderGroupParticipantEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IDateRange
	{
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
		///	The <c>Role</c> field.
		///	designer:fld/LVA73/LVAI3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAI3]")]
		public global::Epsitec.Aider.GroupParticipantRole Role
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.GroupParticipantRole> ("[LVAI3]");
			}
			set
			{
				global::Epsitec.Aider.GroupParticipantRole oldValue = this.Role;
				if (oldValue != value || !this.IsFieldDefined("[LVAI3]"))
				{
					this.OnRoleChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.GroupParticipantRole> ("[LVAI3]", oldValue, value);
					this.OnRoleChanged (oldValue, value);
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
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVA73/LVAF4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAF4]")]
		public string Comment
		{
			get
			{
				return this.GetField<string> ("[LVAF4]");
			}
			set
			{
				string oldValue = this.Comment;
				if (oldValue != value || !this.IsFieldDefined("[LVAF4]"))
				{
					this.OnCommentChanging (oldValue, value);
					this.SetField<string> ("[LVAF4]", oldValue, value);
					this.OnCommentChanged (oldValue, value);
				}
			}
		}
		
		partial void OnRoleChanging(global::Epsitec.Aider.GroupParticipantRole oldValue, global::Epsitec.Aider.GroupParticipantRole newValue);
		partial void OnRoleChanged(global::Epsitec.Aider.GroupParticipantRole oldValue, global::Epsitec.Aider.GroupParticipantRole newValue);
		partial void OnGroupChanging(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnGroupChanged(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnPersonChanging(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnPersonChanged(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnCommentChanging(string oldValue, string newValue);
		partial void OnCommentChanged(string oldValue, string newValue);
		
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
	public partial class AiderEventEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
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
		public global::Epsitec.Aider.EventType Type
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.EventType> ("[LVA04]");
			}
			set
			{
				global::Epsitec.Aider.EventType oldValue = this.Type;
				if (oldValue != value || !this.IsFieldDefined("[LVA04]"))
				{
					this.OnTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.EventType> ("[LVA04]", oldValue, value);
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
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVA93/LVAS3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAS3]")]
		public string Comment
		{
			get
			{
				return this.GetField<string> ("[LVAS3]");
			}
			set
			{
				string oldValue = this.Comment;
				if (oldValue != value || !this.IsFieldDefined("[LVAS3]"))
				{
					this.OnCommentChanging (oldValue, value);
					this.SetField<string> ("[LVAS3]", oldValue, value);
					this.OnCommentChanged (oldValue, value);
				}
			}
		}
		
		partial void OnDateChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnDateChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnTypeChanging(global::Epsitec.Aider.EventType oldValue, global::Epsitec.Aider.EventType newValue);
		partial void OnTypeChanged(global::Epsitec.Aider.EventType oldValue, global::Epsitec.Aider.EventType newValue);
		partial void OnPlaceChanging(global::Epsitec.Aider.Entities.AiderPlaceEntity oldValue, global::Epsitec.Aider.Entities.AiderPlaceEntity newValue);
		partial void OnPlaceChanged(global::Epsitec.Aider.Entities.AiderPlaceEntity oldValue, global::Epsitec.Aider.Entities.AiderPlaceEntity newValue);
		partial void OnDescriptionChanging(string oldValue, string newValue);
		partial void OnDescriptionChanged(string oldValue, string newValue);
		partial void OnCommentChanging(string oldValue, string newValue);
		partial void OnCommentChanged(string oldValue, string newValue);
		
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
	public partial class AiderEventParticipantEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Role</c> field.
		///	designer:fld/LVAA3/LVAD3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAD3]")]
		public global::Epsitec.Aider.EventParticipantRole Role
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.EventParticipantRole> ("[LVAD3]");
			}
			set
			{
				global::Epsitec.Aider.EventParticipantRole oldValue = this.Role;
				if (oldValue != value || !this.IsFieldDefined("[LVAD3]"))
				{
					this.OnRoleChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.EventParticipantRole> ("[LVAD3]", oldValue, value);
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
		///	<summary>
		///	The <c>Comment</c> field.
		///	designer:fld/LVAA3/LVAE4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAE4]")]
		public string Comment
		{
			get
			{
				return this.GetField<string> ("[LVAE4]");
			}
			set
			{
				string oldValue = this.Comment;
				if (oldValue != value || !this.IsFieldDefined("[LVAE4]"))
				{
					this.OnCommentChanging (oldValue, value);
					this.SetField<string> ("[LVAE4]", oldValue, value);
					this.OnCommentChanged (oldValue, value);
				}
			}
		}
		
		partial void OnRoleChanging(global::Epsitec.Aider.EventParticipantRole oldValue, global::Epsitec.Aider.EventParticipantRole newValue);
		partial void OnRoleChanged(global::Epsitec.Aider.EventParticipantRole oldValue, global::Epsitec.Aider.EventParticipantRole newValue);
		partial void OnPersonChanging(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnPersonChanged(global::Epsitec.Aider.Entities.AiderPersonEntity oldValue, global::Epsitec.Aider.Entities.AiderPersonEntity newValue);
		partial void OnEventChanging(global::Epsitec.Aider.Entities.AiderEventEntity oldValue, global::Epsitec.Aider.Entities.AiderEventEntity newValue);
		partial void OnEventChanged(global::Epsitec.Aider.Entities.AiderEventEntity oldValue, global::Epsitec.Aider.Entities.AiderEventEntity newValue);
		partial void OnCommentChanging(string oldValue, string newValue);
		partial void OnCommentChanged(string oldValue, string newValue);
		
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
	public partial class AiderPlaceEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
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
	public partial class AiderGroupEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IDateRange
	{
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
		///	<summary>
		///	The <c>Type</c> field.
		///	designer:fld/LVA54/LVAC4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAC4]")]
		public global::Epsitec.Aider.GroupType Type
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.GroupType> ("[LVAC4]");
			}
			set
			{
				global::Epsitec.Aider.GroupType oldValue = this.Type;
				if (oldValue != value || !this.IsFieldDefined("[LVAC4]"))
				{
					this.OnTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.GroupType> ("[LVAC4]", oldValue, value);
					this.OnTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Level</c> field.
		///	designer:fld/LVA54/LVAF6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAF6]")]
		public global::Epsitec.Aider.GroupLevel Level
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.GroupLevel> ("[LVAF6]");
			}
			set
			{
				global::Epsitec.Aider.GroupLevel oldValue = this.Level;
				if (oldValue != value || !this.IsFieldDefined("[LVAF6]"))
				{
					this.OnLevelChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.GroupLevel> ("[LVAF6]", oldValue, value);
					this.OnLevelChanged (oldValue, value);
				}
			}
		}
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
		///	The <c>Comment</c> field.
		///	designer:fld/LVA54/LVAD4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAD4]")]
		public string Comment
		{
			get
			{
				return this.GetField<string> ("[LVAD4]");
			}
			set
			{
				string oldValue = this.Comment;
				if (oldValue != value || !this.IsFieldDefined("[LVAD4]"))
				{
					this.OnCommentChanging (oldValue, value);
					this.SetField<string> ("[LVAD4]", oldValue, value);
					this.OnCommentChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTypeChanging(global::Epsitec.Aider.GroupType oldValue, global::Epsitec.Aider.GroupType newValue);
		partial void OnTypeChanged(global::Epsitec.Aider.GroupType oldValue, global::Epsitec.Aider.GroupType newValue);
		partial void OnLevelChanging(global::Epsitec.Aider.GroupLevel oldValue, global::Epsitec.Aider.GroupLevel newValue);
		partial void OnLevelChanged(global::Epsitec.Aider.GroupLevel oldValue, global::Epsitec.Aider.GroupLevel newValue);
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		partial void OnDescriptionChanging(string oldValue, string newValue);
		partial void OnDescriptionChanged(string oldValue, string newValue);
		partial void OnCommentChanging(string oldValue, string newValue);
		partial void OnCommentChanged(string oldValue, string newValue);
		
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
	public partial class AiderGroupRelationshipEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Aider.Entities.IDateRange
	{
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
		public global::Epsitec.Aider.GroupRelationshipType Type
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.GroupRelationshipType> ("[LVAT4]");
			}
			set
			{
				global::Epsitec.Aider.GroupRelationshipType oldValue = this.Type;
				if (oldValue != value || !this.IsFieldDefined("[LVAT4]"))
				{
					this.OnTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.GroupRelationshipType> ("[LVAT4]", oldValue, value);
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
		///	<summary>
		///	The <c>Omment</c> field.
		///	designer:fld/LVAN4/LVAQ4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAQ4]")]
		public string Omment
		{
			get
			{
				return this.GetField<string> ("[LVAQ4]");
			}
			set
			{
				string oldValue = this.Omment;
				if (oldValue != value || !this.IsFieldDefined("[LVAQ4]"))
				{
					this.OnOmmentChanging (oldValue, value);
					this.SetField<string> ("[LVAQ4]", oldValue, value);
					this.OnOmmentChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTypeChanging(global::Epsitec.Aider.GroupRelationshipType oldValue, global::Epsitec.Aider.GroupRelationshipType newValue);
		partial void OnTypeChanged(global::Epsitec.Aider.GroupRelationshipType oldValue, global::Epsitec.Aider.GroupRelationshipType newValue);
		partial void OnGroup1Changing(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnGroup1Changed(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnGroup2Changing(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnGroup2Changed(global::Epsitec.Aider.Entities.AiderGroupEntity oldValue, global::Epsitec.Aider.Entities.AiderGroupEntity newValue);
		partial void OnOmmentChanging(string oldValue, string newValue);
		partial void OnOmmentChanged(string oldValue, string newValue);
		
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
	public partial class AiderGroupPlaceEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
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
	public partial class AiderPlacePersonEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.IDateRange
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
		///	The <c>Role</c> field.
		///	designer:fld/LVAQ5/LVAG6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAG6]")]
		public global::Epsitec.Aider.PlacePersonRole Role
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.PlacePersonRole> ("[LVAG6]");
			}
			set
			{
				global::Epsitec.Aider.PlacePersonRole oldValue = this.Role;
				if (oldValue != value || !this.IsFieldDefined("[LVAG6]"))
				{
					this.OnRoleChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.PlacePersonRole> ("[LVAG6]", oldValue, value);
					this.OnRoleChanged (oldValue, value);
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
		
		partial void OnRoleChanging(global::Epsitec.Aider.PlacePersonRole oldValue, global::Epsitec.Aider.PlacePersonRole newValue);
		partial void OnRoleChanged(global::Epsitec.Aider.PlacePersonRole oldValue, global::Epsitec.Aider.PlacePersonRole newValue);
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
	public partial class AiderLegalPersonEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Type</c> field.
		///	designer:fld/LVAR6/LVAS6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAS6]")]
		public global::Epsitec.Aider.LegalPersonType Type
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.LegalPersonType> ("[LVAS6]");
			}
			set
			{
				global::Epsitec.Aider.LegalPersonType oldValue = this.Type;
				if (oldValue != value || !this.IsFieldDefined("[LVAS6]"))
				{
					this.OnTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.LegalPersonType> ("[LVAS6]", oldValue, value);
					this.OnTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>RemovalReason</c> field.
		///	designer:fld/LVAR6/LVA17
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVA17]")]
		public global::Epsitec.Aider.eCH.RemovalReason RemovalReason
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.eCH.RemovalReason> ("[LVA17]");
			}
			set
			{
				global::Epsitec.Aider.eCH.RemovalReason oldValue = this.RemovalReason;
				if (oldValue != value || !this.IsFieldDefined("[LVA17]"))
				{
					this.OnRemovalReasonChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.eCH.RemovalReason> ("[LVA17]", oldValue, value);
					this.OnRemovalReasonChanged (oldValue, value);
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
		///	<summary>
		///	The <c>Contact1</c> field.
		///	designer:fld/LVAR6/LVAA7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAA7]")]
		public global::Epsitec.Aider.Entities.AiderContactEntity Contact1
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderContactEntity> ("[LVAA7]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderContactEntity oldValue = this.Contact1;
				if (oldValue != value || !this.IsFieldDefined("[LVAA7]"))
				{
					this.OnContact1Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderContactEntity> ("[LVAA7]", oldValue, value);
					this.OnContact1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Contact2</c> field.
		///	designer:fld/LVAR6/LVAC7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAC7]")]
		public global::Epsitec.Aider.Entities.AiderContactEntity Contact2
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Entities.AiderContactEntity> ("[LVAC7]");
			}
			set
			{
				global::Epsitec.Aider.Entities.AiderContactEntity oldValue = this.Contact2;
				if (oldValue != value || !this.IsFieldDefined("[LVAC7]"))
				{
					this.OnContact2Changing (oldValue, value);
					this.SetField<global::Epsitec.Aider.Entities.AiderContactEntity> ("[LVAC7]", oldValue, value);
					this.OnContact2Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Language</c> field.
		///	designer:fld/LVAR6/LVAN7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAN7]")]
		public global::Epsitec.Aider.Language Language
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.Language> ("[LVAN7]");
			}
			set
			{
				global::Epsitec.Aider.Language oldValue = this.Language;
				if (oldValue != value || !this.IsFieldDefined("[LVAN7]"))
				{
					this.OnLanguageChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.Language> ("[LVAN7]", oldValue, value);
					this.OnLanguageChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTypeChanging(global::Epsitec.Aider.LegalPersonType oldValue, global::Epsitec.Aider.LegalPersonType newValue);
		partial void OnTypeChanged(global::Epsitec.Aider.LegalPersonType oldValue, global::Epsitec.Aider.LegalPersonType newValue);
		partial void OnRemovalReasonChanging(global::Epsitec.Aider.eCH.RemovalReason oldValue, global::Epsitec.Aider.eCH.RemovalReason newValue);
		partial void OnRemovalReasonChanged(global::Epsitec.Aider.eCH.RemovalReason oldValue, global::Epsitec.Aider.eCH.RemovalReason newValue);
		partial void OnAddressChanging(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnAddressChanged(global::Epsitec.Aider.Entities.AiderAddressEntity oldValue, global::Epsitec.Aider.Entities.AiderAddressEntity newValue);
		partial void OnContact1Changing(global::Epsitec.Aider.Entities.AiderContactEntity oldValue, global::Epsitec.Aider.Entities.AiderContactEntity newValue);
		partial void OnContact1Changed(global::Epsitec.Aider.Entities.AiderContactEntity oldValue, global::Epsitec.Aider.Entities.AiderContactEntity newValue);
		partial void OnContact2Changing(global::Epsitec.Aider.Entities.AiderContactEntity oldValue, global::Epsitec.Aider.Entities.AiderContactEntity newValue);
		partial void OnContact2Changed(global::Epsitec.Aider.Entities.AiderContactEntity oldValue, global::Epsitec.Aider.Entities.AiderContactEntity newValue);
		partial void OnLanguageChanging(global::Epsitec.Aider.Language oldValue, global::Epsitec.Aider.Language newValue);
		partial void OnLanguageChanged(global::Epsitec.Aider.Language oldValue, global::Epsitec.Aider.Language newValue);
		
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
	public partial class AiderContactEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Role</c> field.
		///	designer:fld/LVAB7/LVAJ7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAJ7]")]
		public global::Epsitec.Aider.ContactRole Role
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.ContactRole> ("[LVAJ7]");
			}
			set
			{
				global::Epsitec.Aider.ContactRole oldValue = this.Role;
				if (oldValue != value || !this.IsFieldDefined("[LVAJ7]"))
				{
					this.OnRoleChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.ContactRole> ("[LVAJ7]", oldValue, value);
					this.OnRoleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MrMrs</c> field.
		///	designer:fld/LVAB7/LVAD7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[LVAD7]")]
		public global::Epsitec.Aider.eCH.PersonMrMrs MrMrs
		{
			get
			{
				return this.GetField<global::Epsitec.Aider.eCH.PersonMrMrs> ("[LVAD7]");
			}
			set
			{
				global::Epsitec.Aider.eCH.PersonMrMrs oldValue = this.MrMrs;
				if (oldValue != value || !this.IsFieldDefined("[LVAD7]"))
				{
					this.OnMrMrsChanging (oldValue, value);
					this.SetField<global::Epsitec.Aider.eCH.PersonMrMrs> ("[LVAD7]", oldValue, value);
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
		
		partial void OnRoleChanging(global::Epsitec.Aider.ContactRole oldValue, global::Epsitec.Aider.ContactRole newValue);
		partial void OnRoleChanged(global::Epsitec.Aider.ContactRole oldValue, global::Epsitec.Aider.ContactRole newValue);
		partial void OnMrMrsChanging(global::Epsitec.Aider.eCH.PersonMrMrs oldValue, global::Epsitec.Aider.eCH.PersonMrMrs newValue);
		partial void OnMrMrsChanged(global::Epsitec.Aider.eCH.PersonMrMrs oldValue, global::Epsitec.Aider.eCH.PersonMrMrs newValue);
		partial void OnTitleChanging(string oldValue, string newValue);
		partial void OnTitleChanged(string oldValue, string newValue);
		partial void OnFirstNameChanging(string oldValue, string newValue);
		partial void OnFirstNameChanged(string oldValue, string newValue);
		partial void OnLastNameChanging(string oldValue, string newValue);
		partial void OnLastNameChanged(string oldValue, string newValue);
		
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

