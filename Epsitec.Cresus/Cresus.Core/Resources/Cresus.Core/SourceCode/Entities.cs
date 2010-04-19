﻿//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[L0A1]", typeof (Epsitec.Cresus.Core.Entities.CountryEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0A4]", typeof (Epsitec.Cresus.Core.Entities.RegionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0A5]", typeof (Epsitec.Cresus.Core.Entities.LocationEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AD]", typeof (Epsitec.Cresus.Core.Entities.AddressEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AF]", typeof (Epsitec.Cresus.Core.Entities.PostBoxEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AI]", typeof (Epsitec.Cresus.Core.Entities.StreetEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AM]", typeof (Epsitec.Cresus.Core.Entities.AbstractPersonEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AN]", typeof (Epsitec.Cresus.Core.Entities.NaturalPersonEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AO]", typeof (Epsitec.Cresus.Core.Entities.LegalPersonEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AP]", typeof (Epsitec.Cresus.Core.Entities.AbstractContactEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AQ]", typeof (Epsitec.Cresus.Core.Entities.MailContactEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AT]", typeof (Epsitec.Cresus.Core.Entities.PersonTitleEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0A21]", typeof (Epsitec.Cresus.Core.Entities.LanguageEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AA1]", typeof (Epsitec.Cresus.Core.Entities.PersonGenderEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AE1]", typeof (Epsitec.Cresus.Core.Entities.ContactRoleEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AL1]", typeof (Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AQ1]", typeof (Epsitec.Cresus.Core.Entities.CommentEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AU1]", typeof (Epsitec.Cresus.Core.Entities.TelecomContactEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AV1]", typeof (Epsitec.Cresus.Core.Entities.TelecomTypeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0A52]", typeof (Epsitec.Cresus.Core.Entities.UriContactEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0A62]", typeof (Epsitec.Cresus.Core.Entities.UriSchemeEntity))]
#region Epsitec.Cresus.Core.Country Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>Country</c> entity.
	///	designer:cap/L0A1
	///	</summary>
	public partial class CountryEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/L0A1/L0A2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A2]")]
		public string Code
		{
			get
			{
				return this.GetField<string> ("[L0A2]");
			}
			set
			{
				string oldValue = this.Code;
				if (oldValue != value)
				{
					this.OnCodeChanging (oldValue, value);
					this.SetField<string> ("[L0A2]", oldValue, value);
					this.OnCodeChanged (oldValue, value);
				}
			}
		}
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
				if (oldValue != value)
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[L0A3]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnCodeChanging(string oldValue, string newValue);
		partial void OnCodeChanged(string oldValue, string newValue);
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.CountryEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.CountryEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 1);	// [L0A1]
		public static readonly new string EntityStructuredTypeKey = "[L0A1]";
	}
}
#endregion

#region Epsitec.Cresus.Core.Region Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>Region</c> entity.
	///	designer:cap/L0A4
	///	</summary>
	public partial class RegionEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/L0A4/L0AB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AB]")]
		public string Code
		{
			get
			{
				return this.GetField<string> ("[L0AB]");
			}
			set
			{
				string oldValue = this.Code;
				if (oldValue != value)
				{
					this.OnCodeChanging (oldValue, value);
					this.SetField<string> ("[L0AB]", oldValue, value);
					this.OnCodeChanged (oldValue, value);
				}
			}
		}
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
				if (oldValue != value)
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
		public global::Epsitec.Cresus.Core.Entities.CountryEntity Country
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.CountryEntity> ("[L0AA]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.CountryEntity oldValue = this.Country;
				if (oldValue != value)
				{
					this.OnCountryChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.CountryEntity> ("[L0AA]", oldValue, value);
					this.OnCountryChanged (oldValue, value);
				}
			}
		}
		
		partial void OnCodeChanging(string oldValue, string newValue);
		partial void OnCodeChanged(string oldValue, string newValue);
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		partial void OnCountryChanging(global::Epsitec.Cresus.Core.Entities.CountryEntity oldValue, global::Epsitec.Cresus.Core.Entities.CountryEntity newValue);
		partial void OnCountryChanged(global::Epsitec.Cresus.Core.Entities.CountryEntity oldValue, global::Epsitec.Cresus.Core.Entities.CountryEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.RegionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.RegionEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 4);	// [L0A4]
		public static readonly new string EntityStructuredTypeKey = "[L0A4]";
	}
}
#endregion

#region Epsitec.Cresus.Core.Location Entity
namespace Epsitec.Cresus.Core.Entities
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
				if (oldValue != value)
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
				if (oldValue != value)
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
		public global::Epsitec.Cresus.Core.Entities.CountryEntity Country
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.CountryEntity> ("[L0A8]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.CountryEntity oldValue = this.Country;
				if (oldValue != value)
				{
					this.OnCountryChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.CountryEntity> ("[L0A8]", oldValue, value);
					this.OnCountryChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Region</c> field.
		///	designer:fld/L0A5/L0A9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A9]")]
		public global::Epsitec.Cresus.Core.Entities.RegionEntity Region
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.RegionEntity> ("[L0A9]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.RegionEntity oldValue = this.Region;
				if (oldValue != value)
				{
					this.OnRegionChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.RegionEntity> ("[L0A9]", oldValue, value);
					this.OnRegionChanged (oldValue, value);
				}
			}
		}
		
		partial void OnPostalCodeChanging(string oldValue, string newValue);
		partial void OnPostalCodeChanged(string oldValue, string newValue);
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		partial void OnCountryChanging(global::Epsitec.Cresus.Core.Entities.CountryEntity oldValue, global::Epsitec.Cresus.Core.Entities.CountryEntity newValue);
		partial void OnCountryChanged(global::Epsitec.Cresus.Core.Entities.CountryEntity oldValue, global::Epsitec.Cresus.Core.Entities.CountryEntity newValue);
		partial void OnRegionChanging(global::Epsitec.Cresus.Core.Entities.RegionEntity oldValue, global::Epsitec.Cresus.Core.Entities.RegionEntity newValue);
		partial void OnRegionChanged(global::Epsitec.Cresus.Core.Entities.RegionEntity oldValue, global::Epsitec.Cresus.Core.Entities.RegionEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.LocationEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.LocationEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 5);	// [L0A5]
		public static readonly new string EntityStructuredTypeKey = "[L0A5]";
	}
}
#endregion

#region Epsitec.Cresus.Core.Address Entity
namespace Epsitec.Cresus.Core.Entities
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
		public global::Epsitec.Cresus.Core.Entities.StreetEntity Street
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.StreetEntity> ("[L0AK]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.StreetEntity oldValue = this.Street;
				if (oldValue != value)
				{
					this.OnStreetChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.StreetEntity> ("[L0AK]", oldValue, value);
					this.OnStreetChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PostBox</c> field.
		///	designer:fld/L0AD/L0AH
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AH]")]
		public global::Epsitec.Cresus.Core.Entities.PostBoxEntity PostBox
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PostBoxEntity> ("[L0AH]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PostBoxEntity oldValue = this.PostBox;
				if (oldValue != value)
				{
					this.OnPostBoxChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PostBoxEntity> ("[L0AH]", oldValue, value);
					this.OnPostBoxChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Location</c> field.
		///	designer:fld/L0AD/L0AE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AE]")]
		public global::Epsitec.Cresus.Core.Entities.LocationEntity Location
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.LocationEntity> ("[L0AE]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.LocationEntity oldValue = this.Location;
				if (oldValue != value)
				{
					this.OnLocationChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.LocationEntity> ("[L0AE]", oldValue, value);
					this.OnLocationChanged (oldValue, value);
				}
			}
		}
		
		partial void OnStreetChanging(global::Epsitec.Cresus.Core.Entities.StreetEntity oldValue, global::Epsitec.Cresus.Core.Entities.StreetEntity newValue);
		partial void OnStreetChanged(global::Epsitec.Cresus.Core.Entities.StreetEntity oldValue, global::Epsitec.Cresus.Core.Entities.StreetEntity newValue);
		partial void OnPostBoxChanging(global::Epsitec.Cresus.Core.Entities.PostBoxEntity oldValue, global::Epsitec.Cresus.Core.Entities.PostBoxEntity newValue);
		partial void OnPostBoxChanged(global::Epsitec.Cresus.Core.Entities.PostBoxEntity oldValue, global::Epsitec.Cresus.Core.Entities.PostBoxEntity newValue);
		partial void OnLocationChanging(global::Epsitec.Cresus.Core.Entities.LocationEntity oldValue, global::Epsitec.Cresus.Core.Entities.LocationEntity newValue);
		partial void OnLocationChanged(global::Epsitec.Cresus.Core.Entities.LocationEntity oldValue, global::Epsitec.Cresus.Core.Entities.LocationEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.AddressEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.AddressEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 13);	// [L0AD]
		public static readonly new string EntityStructuredTypeKey = "[L0AD]";
	}
}
#endregion

#region Epsitec.Cresus.Core.PostBox Entity
namespace Epsitec.Cresus.Core.Entities
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
				if (oldValue != value)
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
			return global::Epsitec.Cresus.Core.Entities.PostBoxEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.PostBoxEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 15);	// [L0AF]
		public static readonly new string EntityStructuredTypeKey = "[L0AF]";
	}
}
#endregion

#region Epsitec.Cresus.Core.Street Entity
namespace Epsitec.Cresus.Core.Entities
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
				if (oldValue != value)
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
				if (oldValue != value)
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
			return global::Epsitec.Cresus.Core.Entities.StreetEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.StreetEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 18);	// [L0AI]
		public static readonly new string EntityStructuredTypeKey = "[L0AI]";
	}
}
#endregion

#region Epsitec.Cresus.Core.AbstractPerson Entity
namespace Epsitec.Cresus.Core.Entities
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
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.AbstractContactEntity> Contacts
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.AbstractContactEntity> ("[L0AS]");
			}
		}
		///	<summary>
		///	The <c>PreferredLanguage</c> field.
		///	designer:fld/L0AM/L0AD1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AD1]")]
		public global::Epsitec.Cresus.Core.Entities.LanguageEntity PreferredLanguage
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.LanguageEntity> ("[L0AD1]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.LanguageEntity oldValue = this.PreferredLanguage;
				if (oldValue != value)
				{
					this.OnPreferredLanguageChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.LanguageEntity> ("[L0AD1]", oldValue, value);
					this.OnPreferredLanguageChanged (oldValue, value);
				}
			}
		}
		
		partial void OnPreferredLanguageChanging(global::Epsitec.Cresus.Core.Entities.LanguageEntity oldValue, global::Epsitec.Cresus.Core.Entities.LanguageEntity newValue);
		partial void OnPreferredLanguageChanged(global::Epsitec.Cresus.Core.Entities.LanguageEntity oldValue, global::Epsitec.Cresus.Core.Entities.LanguageEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 22);	// [L0AM]
		public static readonly new string EntityStructuredTypeKey = "[L0AM]";
	}
}
#endregion

#region Epsitec.Cresus.Core.NaturalPerson Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>NaturalPerson</c> entity.
	///	designer:cap/L0AN
	///	</summary>
	public partial class NaturalPersonEntity : global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity
	{
		///	<summary>
		///	The <c>Title</c> field.
		///	designer:fld/L0AN/L0AU
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AU]")]
		public global::Epsitec.Cresus.Core.Entities.PersonTitleEntity Title
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PersonTitleEntity> ("[L0AU]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PersonTitleEntity oldValue = this.Title;
				if (oldValue != value)
				{
					this.OnTitleChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PersonTitleEntity> ("[L0AU]", oldValue, value);
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
				if (oldValue != value)
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
				if (oldValue != value)
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
		public global::Epsitec.Cresus.Core.Entities.PersonGenderEntity Gender
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PersonGenderEntity> ("[L0A11]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PersonGenderEntity oldValue = this.Gender;
				if (oldValue != value)
				{
					this.OnGenderChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PersonGenderEntity> ("[L0A11]", oldValue, value);
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
				if (oldValue != value)
				{
					this.OnBirthDateChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[L0A61]", oldValue, value);
					this.OnBirthDateChanged (oldValue, value);
				}
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
		partial void OnBirthDateChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnBirthDateChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 23);	// [L0AN]
		public static readonly new string EntityStructuredTypeKey = "[L0AN]";
	}
}
#endregion

#region Epsitec.Cresus.Core.LegalPerson Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>LegalPerson</c> entity.
	///	designer:cap/L0AO
	///	</summary>
	public partial class LegalPersonEntity : global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity
	{
		///	<summary>
		///	The <c>Parent</c> field.
		///	designer:fld/L0AO/L0AB2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AB2]")]
		public global::Epsitec.Cresus.Core.Entities.LegalPersonEntity Parent
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.LegalPersonEntity> ("[L0AB2]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.LegalPersonEntity oldValue = this.Parent;
				if (oldValue != value)
				{
					this.OnParentChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.LegalPersonEntity> ("[L0AB2]", oldValue, value);
					this.OnParentChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LegalPersonType</c> field.
		///	designer:fld/L0AO/L0AO1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AO1]")]
		public global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity LegalPersonType
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity> ("[L0AO1]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity oldValue = this.LegalPersonType;
				if (oldValue != value)
				{
					this.OnLegalPersonTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity> ("[L0AO1]", oldValue, value);
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
				if (oldValue != value)
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
				if (oldValue != value)
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
				if (oldValue != value)
				{
					this.OnComplementChanging (oldValue, value);
					this.SetField<string> ("[L0AJ1]", oldValue, value);
					this.OnComplementChanged (oldValue, value);
				}
			}
		}
		
		partial void OnParentChanging(global::Epsitec.Cresus.Core.Entities.LegalPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.LegalPersonEntity newValue);
		partial void OnParentChanged(global::Epsitec.Cresus.Core.Entities.LegalPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.LegalPersonEntity newValue);
		partial void OnLegalPersonTypeChanging(global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity oldValue, global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity newValue);
		partial void OnLegalPersonTypeChanged(global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity oldValue, global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity newValue);
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		partial void OnShortNameChanging(string oldValue, string newValue);
		partial void OnShortNameChanged(string oldValue, string newValue);
		partial void OnComplementChanging(string oldValue, string newValue);
		partial void OnComplementChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.LegalPersonEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.LegalPersonEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 24);	// [L0AO]
		public static readonly new string EntityStructuredTypeKey = "[L0AO]";
	}
}
#endregion

#region Epsitec.Cresus.Core.AbstractContact Entity
namespace Epsitec.Cresus.Core.Entities
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
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.ContactRoleEntity> Roles
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.ContactRoleEntity> ("[L0AG1]");
			}
		}
		///	<summary>
		///	The <c>Comments</c> field.
		///	designer:fld/L0AP/L0AP1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AP1]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.CommentEntity> Comments
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.CommentEntity> ("[L0AP1]");
			}
		}
		///	<summary>
		///	The <c>LegalPerson</c> field.
		///	designer:fld/L0AP/L0A81
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A81]")]
		public global::Epsitec.Cresus.Core.Entities.LegalPersonEntity LegalPerson
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.LegalPersonEntity> ("[L0A81]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.LegalPersonEntity oldValue = this.LegalPerson;
				if (oldValue != value)
				{
					this.OnLegalPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.LegalPersonEntity> ("[L0A81]", oldValue, value);
					this.OnLegalPersonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>NaturalPerson</c> field.
		///	designer:fld/L0AP/L0A71
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A71]")]
		public global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity NaturalPerson
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity> ("[L0A71]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity oldValue = this.NaturalPerson;
				if (oldValue != value)
				{
					this.OnNaturalPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity> ("[L0A71]", oldValue, value);
					this.OnNaturalPersonChanged (oldValue, value);
				}
			}
		}
		
		partial void OnLegalPersonChanging(global::Epsitec.Cresus.Core.Entities.LegalPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.LegalPersonEntity newValue);
		partial void OnLegalPersonChanged(global::Epsitec.Cresus.Core.Entities.LegalPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.LegalPersonEntity newValue);
		partial void OnNaturalPersonChanging(global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity newValue);
		partial void OnNaturalPersonChanged(global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.AbstractContactEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.AbstractContactEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 25);	// [L0AP]
		public static readonly new string EntityStructuredTypeKey = "[L0AP]";
	}
}
#endregion

#region Epsitec.Cresus.Core.MailContact Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>MailContact</c> entity.
	///	designer:cap/L0AQ
	///	</summary>
	public partial class MailContactEntity : global::Epsitec.Cresus.Core.Entities.AbstractContactEntity
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
				if (oldValue != value)
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
		public global::Epsitec.Cresus.Core.Entities.AddressEntity Address
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.AddressEntity> ("[L0AR]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.AddressEntity oldValue = this.Address;
				if (oldValue != value)
				{
					this.OnAddressChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.AddressEntity> ("[L0AR]", oldValue, value);
					this.OnAddressChanged (oldValue, value);
				}
			}
		}
		
		partial void OnComplementChanging(string oldValue, string newValue);
		partial void OnComplementChanged(string oldValue, string newValue);
		partial void OnAddressChanging(global::Epsitec.Cresus.Core.Entities.AddressEntity oldValue, global::Epsitec.Cresus.Core.Entities.AddressEntity newValue);
		partial void OnAddressChanged(global::Epsitec.Cresus.Core.Entities.AddressEntity oldValue, global::Epsitec.Cresus.Core.Entities.AddressEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.MailContactEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.MailContactEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 26);	// [L0AQ]
		public static readonly new string EntityStructuredTypeKey = "[L0AQ]";
	}
}
#endregion

#region Epsitec.Cresus.Core.PersonTitle Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>PersonTitle</c> entity.
	///	designer:cap/L0AT
	///	</summary>
	public partial class PersonTitleEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
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
				if (oldValue != value)
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
				if (oldValue != value)
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[L0AT1]", oldValue, value);
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
			return global::Epsitec.Cresus.Core.Entities.PersonTitleEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.PersonTitleEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 29);	// [L0AT]
		public static readonly new string EntityStructuredTypeKey = "[L0AT]";
	}
}
#endregion

#region Epsitec.Cresus.Core.Language Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>Language</c> entity.
	///	designer:cap/L0A21
	///	</summary>
	public partial class LanguageEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/L0A21/L0A31
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A31]")]
		public string Code
		{
			get
			{
				return this.GetField<string> ("[L0A31]");
			}
			set
			{
				string oldValue = this.Code;
				if (oldValue != value)
				{
					this.OnCodeChanging (oldValue, value);
					this.SetField<string> ("[L0A31]", oldValue, value);
					this.OnCodeChanged (oldValue, value);
				}
			}
		}
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
				if (oldValue != value)
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[L0A41]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnCodeChanging(string oldValue, string newValue);
		partial void OnCodeChanged(string oldValue, string newValue);
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.LanguageEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.LanguageEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 34);	// [L0A21]
		public static readonly new string EntityStructuredTypeKey = "[L0A21]";
	}
}
#endregion

#region Epsitec.Cresus.Core.PersonGender Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>PersonGender</c> entity.
	///	designer:cap/L0AA1
	///	</summary>
	public partial class PersonGenderEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/L0AA1/L0AB1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AB1]")]
		public string Code
		{
			get
			{
				return this.GetField<string> ("[L0AB1]");
			}
			set
			{
				string oldValue = this.Code;
				if (oldValue != value)
				{
					this.OnCodeChanging (oldValue, value);
					this.SetField<string> ("[L0AB1]", oldValue, value);
					this.OnCodeChanged (oldValue, value);
				}
			}
		}
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
				if (oldValue != value)
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[L0AC1]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnCodeChanging(string oldValue, string newValue);
		partial void OnCodeChanged(string oldValue, string newValue);
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.PersonGenderEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.PersonGenderEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 42);	// [L0AA1]
		public static readonly new string EntityStructuredTypeKey = "[L0AA1]";
	}
}
#endregion

#region Epsitec.Cresus.Core.ContactRole Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ContactRole</c> entity.
	///	designer:cap/L0AE1
	///	</summary>
	public partial class ContactRoleEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
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
				if (oldValue != value)
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
			return global::Epsitec.Cresus.Core.Entities.ContactRoleEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ContactRoleEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 46);	// [L0AE1]
		public static readonly new string EntityStructuredTypeKey = "[L0AE1]";
	}
}
#endregion

#region Epsitec.Cresus.Core.LegalPersonType Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>LegalPersonType</c> entity.
	///	designer:cap/L0AL1
	///	</summary>
	public partial class LegalPersonTypeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
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
				if (oldValue != value)
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
				if (oldValue != value)
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
			return global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.LegalPersonTypeEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 53);	// [L0AL1]
		public static readonly new string EntityStructuredTypeKey = "[L0AL1]";
	}
}
#endregion

#region Epsitec.Cresus.Core.Comment Entity
namespace Epsitec.Cresus.Core.Entities
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
				if (oldValue != value)
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
			return global::Epsitec.Cresus.Core.Entities.CommentEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.CommentEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 58);	// [L0AQ1]
		public static readonly new string EntityStructuredTypeKey = "[L0AQ1]";
	}
}
#endregion

#region Epsitec.Cresus.Core.TelecomContact Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>TelecomContact</c> entity.
	///	designer:cap/L0AU1
	///	</summary>
	public partial class TelecomContactEntity : global::Epsitec.Cresus.Core.Entities.AbstractContactEntity
	{
		///	<summary>
		///	The <c>TelecomType</c> field.
		///	designer:fld/L0AU1/L0A22
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A22]")]
		public global::Epsitec.Cresus.Core.Entities.TelecomTypeEntity TelecomType
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.TelecomTypeEntity> ("[L0A22]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.TelecomTypeEntity oldValue = this.TelecomType;
				if (oldValue != value)
				{
					this.OnTelecomTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.TelecomTypeEntity> ("[L0A22]", oldValue, value);
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
				if (oldValue != value)
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
				if (oldValue != value)
				{
					this.OnExtensionChanging (oldValue, value);
					this.SetField<string> ("[L0A42]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 62);	// [L0AU1]
		public static readonly new string EntityStructuredTypeKey = "[L0AU1]";
	}
}
#endregion

#region Epsitec.Cresus.Core.TelecomType Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>TelecomType</c> entity.
	///	designer:cap/L0AV1
	///	</summary>
	public partial class TelecomTypeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/L0AV1/L0A12
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A12]")]
		public string Code
		{
			get
			{
				return this.GetField<string> ("[L0A12]");
			}
			set
			{
				string oldValue = this.Code;
				if (oldValue != value)
				{
					this.OnCodeChanging (oldValue, value);
					this.SetField<string> ("[L0A12]", oldValue, value);
					this.OnCodeChanged (oldValue, value);
				}
			}
		}
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
				if (oldValue != value)
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[L0A02]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnCodeChanging(string oldValue, string newValue);
		partial void OnCodeChanged(string oldValue, string newValue);
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.TelecomTypeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.TelecomTypeEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 63);	// [L0AV1]
		public static readonly new string EntityStructuredTypeKey = "[L0AV1]";
	}
}
#endregion

#region Epsitec.Cresus.Core.UriContact Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>UriContact</c> entity.
	///	designer:cap/L0A52
	///	</summary>
	public partial class UriContactEntity : global::Epsitec.Cresus.Core.Entities.AbstractContactEntity
	{
		///	<summary>
		///	The <c>UriScheme</c> field.
		///	designer:fld/L0A52/L0A92
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A92]")]
		public global::Epsitec.Cresus.Core.Entities.UriSchemeEntity UriScheme
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.UriSchemeEntity> ("[L0A92]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.UriSchemeEntity oldValue = this.UriScheme;
				if (oldValue != value)
				{
					this.OnUriSchemeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.UriSchemeEntity> ("[L0A92]", oldValue, value);
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
				if (oldValue != value)
				{
					this.OnUriChanging (oldValue, value);
					this.SetField<string> ("[L0AA2]", oldValue, value);
					this.OnUriChanged (oldValue, value);
				}
			}
		}
		
		partial void OnUriSchemeChanging(global::Epsitec.Cresus.Core.Entities.UriSchemeEntity oldValue, global::Epsitec.Cresus.Core.Entities.UriSchemeEntity newValue);
		partial void OnUriSchemeChanged(global::Epsitec.Cresus.Core.Entities.UriSchemeEntity oldValue, global::Epsitec.Cresus.Core.Entities.UriSchemeEntity newValue);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 69);	// [L0A52]
		public static readonly new string EntityStructuredTypeKey = "[L0A52]";
	}
}
#endregion

#region Epsitec.Cresus.Core.UriScheme Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>UriScheme</c> entity.
	///	designer:cap/L0A62
	///	</summary>
	public partial class UriSchemeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/L0A62/L0A72
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0A72]")]
		public string Code
		{
			get
			{
				return this.GetField<string> ("[L0A72]");
			}
			set
			{
				string oldValue = this.Code;
				if (oldValue != value)
				{
					this.OnCodeChanging (oldValue, value);
					this.SetField<string> ("[L0A72]", oldValue, value);
					this.OnCodeChanged (oldValue, value);
				}
			}
		}
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
				if (oldValue != value)
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[L0A82]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnCodeChanging(string oldValue, string newValue);
		partial void OnCodeChanged(string oldValue, string newValue);
		partial void OnNameChanging(string oldValue, string newValue);
		partial void OnNameChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.UriSchemeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.UriSchemeEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 70);	// [L0A62]
		public static readonly new string EntityStructuredTypeKey = "[L0A62]";
	}
}
#endregion

