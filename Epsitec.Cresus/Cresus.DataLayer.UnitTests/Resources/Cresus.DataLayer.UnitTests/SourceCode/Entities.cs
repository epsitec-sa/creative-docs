//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[L0A1]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0A4]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.RegionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0A5]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.LocationEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AD]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.AddressEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AF]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.PostBoxEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AI]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.StreetEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AM]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractPersonEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AN]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AO]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AP]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractContactEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AQ]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.MailContactEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AT]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonTitleEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0A21]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.LanguageEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AA1]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AE1]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.ContactRoleEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AL1]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonTypeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AQ1]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.CommentEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AU1]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomContactEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AV1]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomTypeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0A52]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.UriContactEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0A62]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.UriSchemeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AI3]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.ValueDataEntity))]
#region Epsitec.Cresus.DataLayer.UnitTests.Country Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>Country</c> entity.
	///	designer:cap/L0A1
	///	</summary>
	public partial class CountryEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCode
	{
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/L0A1/L0AD3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AD3]")]
		public string Code
		{
			get
			{
				return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCodeInterfaceImplementation.GetCode (this);
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCodeInterfaceImplementation.SetCode (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/L0A1/L0A3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A3]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[L0A3]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[L0A3]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[L0A3]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 1);	// [L0A1]
		public static readonly new string EntityStructuredTypeKey = "[L0A1]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.Region Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>Region</c> entity.
	///	designer:cap/L0A4
	///	</summary>
	public partial class RegionEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCode
	{
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/L0A4/L0AD3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AD3]")]
		public string Code
		{
			get
			{
				return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCodeInterfaceImplementation.GetCode (this);
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCodeInterfaceImplementation.SetCode (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/L0A4/L0AC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AC]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[L0AC]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[L0AC]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[L0AC]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Country</c> field.
		///	designer:fld/L0A4/L0AA
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AA]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity Country
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity> ("[L0AA]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity oldValue = this.Country;
				if (oldValue != value || !this.IsFieldDefined("[L0AA]"))
				{
					this.OnCountryChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity> ("[L0AA]", oldValue, value);
					this.OnCountryChanged (oldValue, value);
				}
			}
		}
		
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		partial void OnCountryChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity newValue);
		partial void OnCountryChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.RegionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.RegionEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 4);	// [L0A4]
		public static readonly new string EntityStructuredTypeKey = "[L0A4]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.Location Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>Location</c> entity.
	///	designer:cap/L0A5
	///	</summary>
	public partial class LocationEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>PostalCode</c> field.
		///	designer:fld/L0A5/L0A6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A6]")]
		public string PostalCode
		{
			get
			{
				return this.GetField<string> ("[L0A6]");
			}
			set
			{
				string oldValue = this.PostalCode;
				if (oldValue != value || !this.IsFieldDefined("[L0A6]"))
				{
					this.OnPostalCodeChanging (oldValue, value);
					this.SetField<string> ("[L0A6]", oldValue, value);
					this.OnPostalCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/L0A5/L0A7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A7]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[L0A7]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[L0A7]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[L0A7]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Country</c> field.
		///	designer:fld/L0A5/L0A8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A8]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity Country
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity> ("[L0A8]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity oldValue = this.Country;
				if (oldValue != value || !this.IsFieldDefined("[L0A8]"))
				{
					this.OnCountryChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity> ("[L0A8]", oldValue, value);
					this.OnCountryChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Region</c> field.
		///	designer:fld/L0A5/L0A9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A9]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.RegionEntity Region
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.RegionEntity> ("[L0A9]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.RegionEntity oldValue = this.Region;
				if (oldValue != value || !this.IsFieldDefined("[L0A9]"))
				{
					this.OnRegionChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.RegionEntity> ("[L0A9]", oldValue, value);
					this.OnRegionChanged (oldValue, value);
				}
			}
		}
		
		partial void OnPostalCodeChanging(string oldValue, string newValue);
		partial void OnPostalCodeChanged(string oldValue, string newValue);
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		partial void OnCountryChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity newValue);
		partial void OnCountryChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity newValue);
		partial void OnRegionChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.RegionEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.RegionEntity newValue);
		partial void OnRegionChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.RegionEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.RegionEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LocationEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LocationEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 5);	// [L0A5]
		public static readonly new string EntityStructuredTypeKey = "[L0A5]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.Address Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>Address</c> entity.
	///	designer:cap/L0AD
	///	</summary>
	public partial class AddressEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Street</c> field.
		///	designer:fld/L0AD/L0AK
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AK]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.StreetEntity Street
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.StreetEntity> ("[L0AK]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.StreetEntity oldValue = this.Street;
				if (oldValue != value || !this.IsFieldDefined("[L0AK]"))
				{
					this.OnStreetChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.StreetEntity> ("[L0AK]", oldValue, value);
					this.OnStreetChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PostBox</c> field.
		///	designer:fld/L0AD/L0AH
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AH]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PostBoxEntity PostBox
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PostBoxEntity> ("[L0AH]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PostBoxEntity oldValue = this.PostBox;
				if (oldValue != value || !this.IsFieldDefined("[L0AH]"))
				{
					this.OnPostBoxChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PostBoxEntity> ("[L0AH]", oldValue, value);
					this.OnPostBoxChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Location</c> field.
		///	designer:fld/L0AD/L0AE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AE]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LocationEntity Location
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LocationEntity> ("[L0AE]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LocationEntity oldValue = this.Location;
				if (oldValue != value || !this.IsFieldDefined("[L0AE]"))
				{
					this.OnLocationChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LocationEntity> ("[L0AE]", oldValue, value);
					this.OnLocationChanged (oldValue, value);
				}
			}
		}
		
		partial void OnStreetChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.StreetEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.StreetEntity newValue);
		partial void OnStreetChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.StreetEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.StreetEntity newValue);
		partial void OnPostBoxChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PostBoxEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PostBoxEntity newValue);
		partial void OnPostBoxChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PostBoxEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PostBoxEntity newValue);
		partial void OnLocationChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LocationEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LocationEntity newValue);
		partial void OnLocationChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LocationEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LocationEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AddressEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AddressEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 13);	// [L0AD]
		public static readonly new string EntityStructuredTypeKey = "[L0AD]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.PostBox Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>PostBox</c> entity.
	///	designer:cap/L0AF
	///	</summary>
	public partial class PostBoxEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Number</c> field.
		///	designer:fld/L0AF/L0AG
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AG]")]
		public string Number
		{
			get
			{
				return this.GetField<string> ("[L0AG]");
			}
			set
			{
				string oldValue = this.Number;
				if (oldValue != value || !this.IsFieldDefined("[L0AG]"))
				{
					this.OnNumberChanging (oldValue, value);
					this.SetField<string> ("[L0AG]", oldValue, value);
					this.OnNumberChanged (oldValue, value);
				}
			}
		}
		
		partial void OnNumberChanging(string oldValue, string newValue);
		partial void OnNumberChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PostBoxEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PostBoxEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 15);	// [L0AF]
		public static readonly new string EntityStructuredTypeKey = "[L0AF]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.Street Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>Street</c> entity.
	///	designer:cap/L0AI
	///	</summary>
	public partial class StreetEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Complement</c> field.
		///	designer:fld/L0AI/L0AL
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AL]")]
		public string Complement
		{
			get
			{
				return this.GetField<string> ("[L0AL]");
			}
			set
			{
				string oldValue = this.Complement;
				if (oldValue != value || !this.IsFieldDefined("[L0AL]"))
				{
					this.OnComplementChanging (oldValue, value);
					this.SetField<string> ("[L0AL]", oldValue, value);
					this.OnComplementChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>StreetName</c> field.
		///	designer:fld/L0AI/L0AJ
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AJ]")]
		public string StreetName
		{
			get
			{
				return this.GetField<string> ("[L0AJ]");
			}
			set
			{
				string oldValue = this.StreetName;
				if (oldValue != value || !this.IsFieldDefined("[L0AJ]"))
				{
					this.OnStreetNameChanging (oldValue, value);
					this.SetField<string> ("[L0AJ]", oldValue, value);
					this.OnStreetNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnComplementChanging(string oldValue, string newValue);
		partial void OnComplementChanged(string oldValue, string newValue);
		partial void OnStreetNameChanging(string oldValue, string newValue);
		partial void OnStreetNameChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.StreetEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.StreetEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 18);	// [L0AI]
		public static readonly new string EntityStructuredTypeKey = "[L0AI]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.AbstractPerson Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>AbstractPerson</c> entity.
	///	designer:cap/L0AM
	///	</summary>
	public partial class AbstractPersonEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Contacts</c> field.
		///	designer:fld/L0AM/L0AS
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AS]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractContactEntity> Contacts
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractContactEntity> ("[L0AS]");
			}
		}
		///	<summary>
		///	The <c>PreferredLanguage</c> field.
		///	designer:fld/L0AM/L0AD1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AD1]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LanguageEntity PreferredLanguage
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LanguageEntity> ("[L0AD1]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LanguageEntity oldValue = this.PreferredLanguage;
				if (oldValue != value || !this.IsFieldDefined("[L0AD1]"))
				{
					this.OnPreferredLanguageChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LanguageEntity> ("[L0AD1]", oldValue, value);
					this.OnPreferredLanguageChanged (oldValue, value);
				}
			}
		}
		
		partial void OnPreferredLanguageChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LanguageEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LanguageEntity newValue);
		partial void OnPreferredLanguageChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LanguageEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LanguageEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractPersonEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractPersonEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 22);	// [L0AM]
		public static readonly new string EntityStructuredTypeKey = "[L0AM]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.NaturalPerson Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>NaturalPerson</c> entity.
	///	designer:cap/L0AN
	///	</summary>
	public partial class NaturalPersonEntity : global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractPersonEntity
	{
		///	<summary>
		///	The <c>Title</c> field.
		///	designer:fld/L0AN/L0AU
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AU]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonTitleEntity Title
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonTitleEntity> ("[L0AU]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonTitleEntity oldValue = this.Title;
				if (oldValue != value || !this.IsFieldDefined("[L0AU]"))
				{
					this.OnTitleChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonTitleEntity> ("[L0AU]", oldValue, value);
					this.OnTitleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Firstname</c> field.
		///	designer:fld/L0AN/L0AV
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AV]")]
		public string Firstname
		{
			get
			{
				return this.GetField<string> ("[L0AV]");
			}
			set
			{
				string oldValue = this.Firstname;
				if (oldValue != value || !this.IsFieldDefined("[L0AV]"))
				{
					this.OnFirstnameChanging (oldValue, value);
					this.SetField<string> ("[L0AV]", oldValue, value);
					this.OnFirstnameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Lastname</c> field.
		///	designer:fld/L0AN/L0A01
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A01]")]
		public string Lastname
		{
			get
			{
				return this.GetField<string> ("[L0A01]");
			}
			set
			{
				string oldValue = this.Lastname;
				if (oldValue != value || !this.IsFieldDefined("[L0A01]"))
				{
					this.OnLastnameChanging (oldValue, value);
					this.SetField<string> ("[L0A01]", oldValue, value);
					this.OnLastnameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Gender</c> field.
		///	designer:fld/L0AN/L0A11
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A11]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity Gender
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity> ("[L0A11]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity oldValue = this.Gender;
				if (oldValue != value || !this.IsFieldDefined("[L0A11]"))
				{
					this.OnGenderChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity> ("[L0A11]", oldValue, value);
					this.OnGenderChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>BirthDate</c> field.
		///	designer:fld/L0AN/L0A61
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A61]")]
		public global::Epsitec.Common.Types.Date? BirthDate
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[L0A61]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.BirthDate;
				if (oldValue != value || !this.IsFieldDefined("[L0A61]"))
				{
					this.OnBirthDateChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[L0A61]", oldValue, value);
					this.OnBirthDateChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTitleChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonTitleEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonTitleEntity newValue);
		partial void OnTitleChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonTitleEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonTitleEntity newValue);
		partial void OnFirstnameChanging(string oldValue, string newValue);
		partial void OnFirstnameChanged(string oldValue, string newValue);
		partial void OnLastnameChanging(string oldValue, string newValue);
		partial void OnLastnameChanged(string oldValue, string newValue);
		partial void OnGenderChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity newValue);
		partial void OnGenderChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity newValue);
		partial void OnBirthDateChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnBirthDateChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 23);	// [L0AN]
		public static readonly new string EntityStructuredTypeKey = "[L0AN]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.LegalPerson Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>LegalPerson</c> entity.
	///	designer:cap/L0AO
	///	</summary>
	public partial class LegalPersonEntity : global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractPersonEntity
	{
		///	<summary>
		///	The <c>LegalPersonType</c> field.
		///	designer:fld/L0AO/L0AO1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AO1]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonTypeEntity LegalPersonType
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonTypeEntity> ("[L0AO1]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonTypeEntity oldValue = this.LegalPersonType;
				if (oldValue != value || !this.IsFieldDefined("[L0AO1]"))
				{
					this.OnLegalPersonTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonTypeEntity> ("[L0AO1]", oldValue, value);
					this.OnLegalPersonTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/L0AO/L0AH1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AH1]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[L0AH1]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[L0AH1]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[L0AH1]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ShortName</c> field.
		///	designer:fld/L0AO/L0AI1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AI1]")]
		public string ShortName
		{
			get
			{
				return this.GetField<string> ("[L0AI1]");
			}
			set
			{
				string oldValue = this.ShortName;
				if (oldValue != value || !this.IsFieldDefined("[L0AI1]"))
				{
					this.OnShortNameChanging (oldValue, value);
					this.SetField<string> ("[L0AI1]", oldValue, value);
					this.OnShortNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Complement</c> field.
		///	designer:fld/L0AO/L0AJ1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AJ1]")]
		public string Complement
		{
			get
			{
				return this.GetField<string> ("[L0AJ1]");
			}
			set
			{
				string oldValue = this.Complement;
				if (oldValue != value || !this.IsFieldDefined("[L0AJ1]"))
				{
					this.OnComplementChanging (oldValue, value);
					this.SetField<string> ("[L0AJ1]", oldValue, value);
					this.OnComplementChanged (oldValue, value);
				}
			}
		}
		
		partial void OnLegalPersonTypeChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonTypeEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonTypeEntity newValue);
		partial void OnLegalPersonTypeChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonTypeEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonTypeEntity newValue);
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		partial void OnShortNameChanging(string oldValue, string newValue);
		partial void OnShortNameChanged(string oldValue, string newValue);
		partial void OnComplementChanging(string oldValue, string newValue);
		partial void OnComplementChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 24);	// [L0AO]
		public static readonly new string EntityStructuredTypeKey = "[L0AO]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.AbstractContact Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>AbstractContact</c> entity.
	///	designer:cap/L0AP
	///	</summary>
	public partial class AbstractContactEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Roles</c> field.
		///	designer:fld/L0AP/L0AG1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AG1]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.ContactRoleEntity> Roles
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.ContactRoleEntity> ("[L0AG1]");
			}
		}
		///	<summary>
		///	The <c>Comments</c> field.
		///	designer:fld/L0AP/L0AP1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AP1]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CommentEntity> Comments
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CommentEntity> ("[L0AP1]");
			}
		}
		///	<summary>
		///	The <c>LegalPerson</c> field.
		///	designer:fld/L0AP/L0A81
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A81]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity LegalPerson
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity> ("[L0A81]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity oldValue = this.LegalPerson;
				if (oldValue != value || !this.IsFieldDefined("[L0A81]"))
				{
					this.OnLegalPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity> ("[L0A81]", oldValue, value);
					this.OnLegalPersonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>NaturalPerson</c> field.
		///	designer:fld/L0AP/L0A71
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A71]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity NaturalPerson
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity> ("[L0A71]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity oldValue = this.NaturalPerson;
				if (oldValue != value || !this.IsFieldDefined("[L0A71]"))
				{
					this.OnNaturalPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity> ("[L0A71]", oldValue, value);
					this.OnNaturalPersonChanged (oldValue, value);
				}
			}
		}
		
		partial void OnLegalPersonChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity newValue);
		partial void OnLegalPersonChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity newValue);
		partial void OnNaturalPersonChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity newValue);
		partial void OnNaturalPersonChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractContactEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractContactEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 25);	// [L0AP]
		public static readonly new string EntityStructuredTypeKey = "[L0AP]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.MailContact Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>MailContact</c> entity.
	///	designer:cap/L0AQ
	///	</summary>
	public partial class MailContactEntity : global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractContactEntity
	{
		///	<summary>
		///	The <c>Complement</c> field.
		///	designer:fld/L0AQ/L0AK1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AK1]")]
		public string Complement
		{
			get
			{
				return this.GetField<string> ("[L0AK1]");
			}
			set
			{
				string oldValue = this.Complement;
				if (oldValue != value || !this.IsFieldDefined("[L0AK1]"))
				{
					this.OnComplementChanging (oldValue, value);
					this.SetField<string> ("[L0AK1]", oldValue, value);
					this.OnComplementChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Address</c> field.
		///	designer:fld/L0AQ/L0AR
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AR]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AddressEntity Address
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AddressEntity> ("[L0AR]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AddressEntity oldValue = this.Address;
				if (oldValue != value || !this.IsFieldDefined("[L0AR]"))
				{
					this.OnAddressChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AddressEntity> ("[L0AR]", oldValue, value);
					this.OnAddressChanged (oldValue, value);
				}
			}
		}
		
		partial void OnComplementChanging(string oldValue, string newValue);
		partial void OnComplementChanged(string oldValue, string newValue);
		partial void OnAddressChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AddressEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AddressEntity newValue);
		partial void OnAddressChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AddressEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AddressEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.MailContactEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.MailContactEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 26);	// [L0AQ]
		public static readonly new string EntityStructuredTypeKey = "[L0AQ]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.PersonTitle Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>PersonTitle</c> entity.
	///	designer:cap/L0AT
	///	</summary>
	public partial class PersonTitleEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRank
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/L0AT/L0A03
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A03]")]
		public int? Rank
		{
			get
			{
				return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRankInterfaceImplementation.GetRank (this);
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRankInterfaceImplementation.SetRank (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>ShortName</c> field.
		///	designer:fld/L0AT/L0AS1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AS1]")]
		public string ShortName
		{
			get
			{
				return this.GetField<string> ("[L0AS1]");
			}
			set
			{
				string oldValue = this.ShortName;
				if (oldValue != value || !this.IsFieldDefined("[L0AS1]"))
				{
					this.OnShortNameChanging (oldValue, value);
					this.SetField<string> ("[L0AS1]", oldValue, value);
					this.OnShortNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/L0AT/L0AT1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AT1]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[L0AT1]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[L0AT1]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[L0AT1]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CompatibleGenders</c> field.
		///	designer:fld/L0AT/L0AB3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AB3]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity> CompatibleGenders
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity> ("[L0AB3]");
			}
		}
		
		partial void OnShortNameChanging(string oldValue, string newValue);
		partial void OnShortNameChanged(string oldValue, string newValue);
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonTitleEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonTitleEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 29);	// [L0AT]
		public static readonly new string EntityStructuredTypeKey = "[L0AT]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.Language Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>Language</c> entity.
	///	designer:cap/L0A21
	///	</summary>
	public partial class LanguageEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCode
	{
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/L0A21/L0AD3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AD3]")]
		public string Code
		{
			get
			{
				return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCodeInterfaceImplementation.GetCode (this);
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCodeInterfaceImplementation.SetCode (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/L0A21/L0A41
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A41]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[L0A41]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[L0A41]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[L0A41]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LanguageEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LanguageEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 34);	// [L0A21]
		public static readonly new string EntityStructuredTypeKey = "[L0A21]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.PersonGender Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>PersonGender</c> entity.
	///	designer:cap/L0AA1
	///	</summary>
	public partial class PersonGenderEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRank, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCode
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/L0AA1/L0A03
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A03]")]
		public int? Rank
		{
			get
			{
				return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRankInterfaceImplementation.GetRank (this);
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRankInterfaceImplementation.SetRank (this, value);
			}
		}
		#endregion
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/L0AA1/L0AD3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AD3]")]
		public string Code
		{
			get
			{
				return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCodeInterfaceImplementation.GetCode (this);
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCodeInterfaceImplementation.SetCode (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/L0AA1/L0AC1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AC1]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[L0AC1]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[L0AC1]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[L0AC1]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 42);	// [L0AA1]
		public static readonly new string EntityStructuredTypeKey = "[L0AA1]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.ContactRole Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>ContactRole</c> entity.
	///	designer:cap/L0AE1
	///	</summary>
	public partial class ContactRoleEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRank
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/L0AE1/L0A03
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A03]")]
		public int? Rank
		{
			get
			{
				return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRankInterfaceImplementation.GetRank (this);
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRankInterfaceImplementation.SetRank (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/L0AE1/L0AF1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AF1]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[L0AF1]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[L0AF1]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[L0AF1]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.ContactRoleEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.ContactRoleEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 46);	// [L0AE1]
		public static readonly new string EntityStructuredTypeKey = "[L0AE1]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.LegalPersonType Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>LegalPersonType</c> entity.
	///	designer:cap/L0AL1
	///	</summary>
	public partial class LegalPersonTypeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRank
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/L0AL1/L0A03
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A03]")]
		public int? Rank
		{
			get
			{
				return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRankInterfaceImplementation.GetRank (this);
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRankInterfaceImplementation.SetRank (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>ShortName</c> field.
		///	designer:fld/L0AL1/L0AM1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AM1]")]
		public string ShortName
		{
			get
			{
				return this.GetField<string> ("[L0AM1]");
			}
			set
			{
				string oldValue = this.ShortName;
				if (oldValue != value || !this.IsFieldDefined("[L0AM1]"))
				{
					this.OnShortNameChanging (oldValue, value);
					this.SetField<string> ("[L0AM1]", oldValue, value);
					this.OnShortNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/L0AL1/L0AN1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AN1]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[L0AN1]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[L0AN1]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[L0AN1]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnShortNameChanging(string oldValue, string newValue);
		partial void OnShortNameChanged(string oldValue, string newValue);
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonTypeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonTypeEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 53);	// [L0AL1]
		public static readonly new string EntityStructuredTypeKey = "[L0AL1]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.Comment Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>Comment</c> entity.
	///	designer:cap/L0AQ1
	///	</summary>
	public partial class CommentEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Text</c> field.
		///	designer:fld/L0AQ1/L0AR1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AR1]")]
		public string Text
		{
			get
			{
				return this.GetField<string> ("[L0AR1]");
			}
			set
			{
				string oldValue = this.Text;
				if (oldValue != value || !this.IsFieldDefined("[L0AR1]"))
				{
					this.OnTextChanging (oldValue, value);
					this.SetField<string> ("[L0AR1]", oldValue, value);
					this.OnTextChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTextChanging(string oldValue, string newValue);
		partial void OnTextChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CommentEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CommentEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 58);	// [L0AQ1]
		public static readonly new string EntityStructuredTypeKey = "[L0AQ1]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.TelecomContact Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>TelecomContact</c> entity.
	///	designer:cap/L0AU1
	///	</summary>
	public partial class TelecomContactEntity : global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractContactEntity
	{
		///	<summary>
		///	The <c>TelecomType</c> field.
		///	designer:fld/L0AU1/L0A22
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A22]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomTypeEntity TelecomType
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomTypeEntity> ("[L0A22]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomTypeEntity oldValue = this.TelecomType;
				if (oldValue != value || !this.IsFieldDefined("[L0A22]"))
				{
					this.OnTelecomTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomTypeEntity> ("[L0A22]", oldValue, value);
					this.OnTelecomTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Number</c> field.
		///	designer:fld/L0AU1/L0A32
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A32]")]
		public string Number
		{
			get
			{
				return this.GetField<string> ("[L0A32]");
			}
			set
			{
				string oldValue = this.Number;
				if (oldValue != value || !this.IsFieldDefined("[L0A32]"))
				{
					this.OnNumberChanging (oldValue, value);
					this.SetField<string> ("[L0A32]", oldValue, value);
					this.OnNumberChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Extension</c> field.
		///	designer:fld/L0AU1/L0A42
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A42]")]
		public string Extension
		{
			get
			{
				return this.GetField<string> ("[L0A42]");
			}
			set
			{
				string oldValue = this.Extension;
				if (oldValue != value || !this.IsFieldDefined("[L0A42]"))
				{
					this.OnExtensionChanging (oldValue, value);
					this.SetField<string> ("[L0A42]", oldValue, value);
					this.OnExtensionChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTelecomTypeChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomTypeEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomTypeEntity newValue);
		partial void OnTelecomTypeChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomTypeEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomTypeEntity newValue);
		partial void OnNumberChanging(string oldValue, string newValue);
		partial void OnNumberChanged(string oldValue, string newValue);
		partial void OnExtensionChanging(string oldValue, string newValue);
		partial void OnExtensionChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomContactEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomContactEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 62);	// [L0AU1]
		public static readonly new string EntityStructuredTypeKey = "[L0AU1]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.TelecomType Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>TelecomType</c> entity.
	///	designer:cap/L0AV1
	///	</summary>
	public partial class TelecomTypeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRank, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCode
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/L0AV1/L0A03
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A03]")]
		public int? Rank
		{
			get
			{
				return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRankInterfaceImplementation.GetRank (this);
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRankInterfaceImplementation.SetRank (this, value);
			}
		}
		#endregion
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/L0AV1/L0AD3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AD3]")]
		public string Code
		{
			get
			{
				return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCodeInterfaceImplementation.GetCode (this);
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCodeInterfaceImplementation.SetCode (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/L0AV1/L0A02
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A02]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[L0A02]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[L0A02]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[L0A02]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomTypeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomTypeEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 63);	// [L0AV1]
		public static readonly new string EntityStructuredTypeKey = "[L0AV1]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.UriContact Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>UriContact</c> entity.
	///	designer:cap/L0A52
	///	</summary>
	public partial class UriContactEntity : global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractContactEntity
	{
		///	<summary>
		///	The <c>UriScheme</c> field.
		///	designer:fld/L0A52/L0A92
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A92]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.UriSchemeEntity UriScheme
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.UriSchemeEntity> ("[L0A92]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.UriSchemeEntity oldValue = this.UriScheme;
				if (oldValue != value || !this.IsFieldDefined("[L0A92]"))
				{
					this.OnUriSchemeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.UriSchemeEntity> ("[L0A92]", oldValue, value);
					this.OnUriSchemeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Uri</c> field.
		///	designer:fld/L0A52/L0AA2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AA2]")]
		public string Uri
		{
			get
			{
				return this.GetField<string> ("[L0AA2]");
			}
			set
			{
				string oldValue = this.Uri;
				if (oldValue != value || !this.IsFieldDefined("[L0AA2]"))
				{
					this.OnUriChanging (oldValue, value);
					this.SetField<string> ("[L0AA2]", oldValue, value);
					this.OnUriChanged (oldValue, value);
				}
			}
		}
		
		partial void OnUriSchemeChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.UriSchemeEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.UriSchemeEntity newValue);
		partial void OnUriSchemeChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.UriSchemeEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.UriSchemeEntity newValue);
		partial void OnUriChanging(string oldValue, string newValue);
		partial void OnUriChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.UriContactEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.UriContactEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 69);	// [L0A52]
		public static readonly new string EntityStructuredTypeKey = "[L0A52]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.UriScheme Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>UriScheme</c> entity.
	///	designer:cap/L0A62
	///	</summary>
	public partial class UriSchemeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRank, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCode
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/L0A62/L0A03
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A03]")]
		public int? Rank
		{
			get
			{
				return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRankInterfaceImplementation.GetRank (this);
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRankInterfaceImplementation.SetRank (this, value);
			}
		}
		#endregion
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/L0A62/L0AD3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AD3]")]
		public string Code
		{
			get
			{
				return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCodeInterfaceImplementation.GetCode (this);
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCodeInterfaceImplementation.SetCode (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/L0A62/L0A82
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A82]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[L0A82]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[L0A82]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[L0A82]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.UriSchemeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.UriSchemeEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 70);	// [L0A62]
		public static readonly new string EntityStructuredTypeKey = "[L0A62]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.IItemRank Interface
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>IItemRank</c> entity.
	///	designer:cap/L0AV2
	///	</summary>
	public interface IItemRank
	{
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/L0AV2/L0A03
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A03]")]
		int? Rank
		{
			get;
			set;
		}
	}
	public static partial class IItemRankInterfaceImplementation
	{
		public static int? GetRank(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRank obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<int?> ("[L0A03]");
		}
		public static void SetRank(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRank obj, int? value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			int? oldValue = obj.Rank;
			if (oldValue != value || !entity.IsFieldDefined("[L0A03]"))
			{
				IItemRankInterfaceImplementation.OnRankChanging (obj, oldValue, value);
				entity.SetField<int?> ("[L0A03]", oldValue, value);
				IItemRankInterfaceImplementation.OnRankChanged (obj, oldValue, value);
			}
		}
		static partial void OnRankChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRank obj, int? oldValue, int? newValue);
		static partial void OnRankChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRank obj, int? oldValue, int? newValue);
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.IItemCode Interface
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>IItemCode</c> entity.
	///	designer:cap/L0AC3
	///	</summary>
	public interface IItemCode
	{
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/L0AC3/L0AD3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AD3]")]
		string Code
		{
			get;
			set;
		}
	}
	public static partial class IItemCodeInterfaceImplementation
	{
		public static string GetCode(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCode obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[L0AD3]");
		}
		public static void SetCode(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCode obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.Code;
			if (oldValue != value || !entity.IsFieldDefined("[L0AD3]"))
			{
				IItemCodeInterfaceImplementation.OnCodeChanging (obj, oldValue, value);
				entity.SetField<string> ("[L0AD3]", oldValue, value);
				IItemCodeInterfaceImplementation.OnCodeChanged (obj, oldValue, value);
			}
		}
		static partial void OnCodeChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCode obj, string oldValue, string newValue);
		static partial void OnCodeChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCode obj, string oldValue, string newValue);
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.ValueData Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>ValueData</c> entity.
	///	designer:cap/L0AI3
	///	</summary>
	public partial class ValueDataEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>BooleanValue</c> field.
		///	designer:fld/L0AI3/L0AJ3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AJ3]")]
		public bool BooleanValue
		{
			get
			{
				return this.GetField<bool> ("[L0AJ3]");
			}
			set
			{
				bool oldValue = this.BooleanValue;
				if (oldValue != value || !this.IsFieldDefined("[L0AJ3]"))
				{
					this.OnBooleanValueChanging (oldValue, value);
					this.SetField<bool> ("[L0AJ3]", oldValue, value);
					this.OnBooleanValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ByteArrayValue</c> field.
		///	designer:fld/L0AI3/L0AK3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AK3]")]
		public global::System.Byte[] ByteArrayValue
		{
			get
			{
				return this.GetField<global::System.Byte[]> ("[L0AK3]");
			}
			set
			{
				global::System.Byte[] oldValue = this.ByteArrayValue;
				if (oldValue != value || !this.IsFieldDefined("[L0AK3]"))
				{
					this.OnByteArrayValueChanging (oldValue, value);
					this.SetField<global::System.Byte[]> ("[L0AK3]", oldValue, value);
					this.OnByteArrayValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DateTimeValue</c> field.
		///	designer:fld/L0AI3/L0AL3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AL3]")]
		public global::System.DateTime DateTimeValue
		{
			get
			{
				return this.GetField<global::System.DateTime> ("[L0AL3]");
			}
			set
			{
				global::System.DateTime oldValue = this.DateTimeValue;
				if (oldValue != value || !this.IsFieldDefined("[L0AL3]"))
				{
					this.OnDateTimeValueChanging (oldValue, value);
					this.SetField<global::System.DateTime> ("[L0AL3]", oldValue, value);
					this.OnDateTimeValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DateValue</c> field.
		///	designer:fld/L0AI3/L0AQ3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AQ3]")]
		public global::Epsitec.Common.Types.Date DateValue
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date> ("[L0AQ3]");
			}
			set
			{
				global::Epsitec.Common.Types.Date oldValue = this.DateValue;
				if (oldValue != value || !this.IsFieldDefined("[L0AQ3]"))
				{
					this.OnDateValueChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date> ("[L0AQ3]", oldValue, value);
					this.OnDateValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DecimalValue</c> field.
		///	designer:fld/L0AI3/L0AO3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AO3]")]
		public global::System.Decimal DecimalValue
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[L0AO3]");
			}
			set
			{
				global::System.Decimal oldValue = this.DecimalValue;
				if (oldValue != value || !this.IsFieldDefined("[L0AO3]"))
				{
					this.OnDecimalValueChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[L0AO3]", oldValue, value);
					this.OnDecimalValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>IntegerValue</c> field.
		///	designer:fld/L0AI3/L0AM3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AM3]")]
		public int IntegerValue
		{
			get
			{
				return this.GetField<int> ("[L0AM3]");
			}
			set
			{
				int oldValue = this.IntegerValue;
				if (oldValue != value || !this.IsFieldDefined("[L0AM3]"))
				{
					this.OnIntegerValueChanging (oldValue, value);
					this.SetField<int> ("[L0AM3]", oldValue, value);
					this.OnIntegerValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LongIntegerValue</c> field.
		///	designer:fld/L0AI3/L0AN3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AN3]")]
		public long LongIntegerValue
		{
			get
			{
				return this.GetField<long> ("[L0AN3]");
			}
			set
			{
				long oldValue = this.LongIntegerValue;
				if (oldValue != value || !this.IsFieldDefined("[L0AN3]"))
				{
					this.OnLongIntegerValueChanging (oldValue, value);
					this.SetField<long> ("[L0AN3]", oldValue, value);
					this.OnLongIntegerValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>StringValue</c> field.
		///	designer:fld/L0AI3/L0AS3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AS3]")]
		public string StringValue
		{
			get
			{
				return this.GetField<string> ("[L0AS3]");
			}
			set
			{
				string oldValue = this.StringValue;
				if (oldValue != value || !this.IsFieldDefined("[L0AS3]"))
				{
					this.OnStringValueChanging (oldValue, value);
					this.SetField<string> ("[L0AS3]", oldValue, value);
					this.OnStringValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TimeValue</c> field.
		///	designer:fld/L0AI3/L0AR3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AR3]")]
		public global::Epsitec.Common.Types.Time TimeValue
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Time> ("[L0AR3]");
			}
			set
			{
				global::Epsitec.Common.Types.Time oldValue = this.TimeValue;
				if (oldValue != value || !this.IsFieldDefined("[L0AR3]"))
				{
					this.OnTimeValueChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Time> ("[L0AR3]", oldValue, value);
					this.OnTimeValueChanged (oldValue, value);
				}
			}
		}
		
		partial void OnBooleanValueChanging(bool oldValue, bool newValue);
		partial void OnBooleanValueChanged(bool oldValue, bool newValue);
		partial void OnByteArrayValueChanging(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		partial void OnByteArrayValueChanged(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		partial void OnDateTimeValueChanging(global::System.DateTime oldValue, global::System.DateTime newValue);
		partial void OnDateTimeValueChanged(global::System.DateTime oldValue, global::System.DateTime newValue);
		partial void OnDateValueChanging(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDateValueChanged(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnDecimalValueChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnDecimalValueChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnIntegerValueChanging(int oldValue, int newValue);
		partial void OnIntegerValueChanged(int oldValue, int newValue);
		partial void OnLongIntegerValueChanging(long oldValue, long newValue);
		partial void OnLongIntegerValueChanged(long oldValue, long newValue);
		partial void OnStringValueChanging(string oldValue, string newValue);
		partial void OnStringValueChanged(string oldValue, string newValue);
		partial void OnTimeValueChanging(global::Epsitec.Common.Types.Time oldValue, global::Epsitec.Common.Types.Time newValue);
		partial void OnTimeValueChanged(global::Epsitec.Common.Types.Time oldValue, global::Epsitec.Common.Types.Time newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.ValueDataEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.ValueDataEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 114);	// [L0AI3]
		public static readonly new string EntityStructuredTypeKey = "[L0AI3]";
	}
}
#endregion

