//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[J1A4]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1A6]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.RegionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1A9]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.LocationEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1AE]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.PostBoxEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1AG]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.StreetEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1AJ]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.AddressEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1AN]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonTitleEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1AQ]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1AT]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.LanguageEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1AV]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.ContactRoleEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1A11]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonTypeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1A41]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.CommentEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1A61]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomTypeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1A81]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.UriSchemeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1AA1]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractContactEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1AB1]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractPersonEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1AE1]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1AJ1]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1AT1]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.MailContactEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1A02]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomContactEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1A42]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.UriContactEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[J1A72]", typeof (Epsitec.Cresus.DataLayer.UnitTests.Entities.ValueDataEntity))]
#region Epsitec.Cresus.DataLayer.UnitTests.IItemRank Interface
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>IItemRank</c> entity.
	///	designer:cap/J1A
	///	</summary>
	public interface IItemRank
	{
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/J1A/J1A1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A1]")]
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
			return entity.GetField<int?> ("[J1A1]");
		}
		public static void SetRank(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRank obj, int? value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			int? oldValue = obj.Rank;
			if (oldValue != value || !entity.IsFieldDefined("[J1A1]"))
			{
				IItemRankInterfaceImplementation.OnRankChanging (obj, oldValue, value);
				entity.SetField<int?> ("[J1A1]", oldValue, value);
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
	///	designer:cap/J1A2
	///	</summary>
	public interface IItemCode
	{
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/J1A2/J1A3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A3]")]
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
			return entity.GetField<string> ("[J1A3]");
		}
		public static void SetCode(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCode obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.Code;
			if (oldValue != value || !entity.IsFieldDefined("[J1A3]"))
			{
				IItemCodeInterfaceImplementation.OnCodeChanging (obj, oldValue, value);
				entity.SetField<string> ("[J1A3]", oldValue, value);
				IItemCodeInterfaceImplementation.OnCodeChanged (obj, oldValue, value);
			}
		}
		static partial void OnCodeChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCode obj, string oldValue, string newValue);
		static partial void OnCodeChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCode obj, string oldValue, string newValue);
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.Country Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>Country</c> entity.
	///	designer:cap/J1A4
	///	</summary>
	public partial class CountryEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCode
	{
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/J1A4/J1A3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A3]")]
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
		///	designer:fld/J1A4/J1A5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A5]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[J1A5]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[J1A5]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[J1A5]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 4);	// [J1A4]
		public static readonly new string EntityStructuredTypeKey = "[J1A4]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.Region Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>Region</c> entity.
	///	designer:cap/J1A6
	///	</summary>
	public partial class RegionEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCode
	{
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/J1A6/J1A3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A3]")]
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
		///	designer:fld/J1A6/J1A7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A7]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[J1A7]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[J1A7]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[J1A7]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Country</c> field.
		///	designer:fld/J1A6/J1A8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A8]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity Country
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity> ("[J1A8]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity oldValue = this.Country;
				if (oldValue != value || !this.IsFieldDefined("[J1A8]"))
				{
					this.OnCountryChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity> ("[J1A8]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 6);	// [J1A6]
		public static readonly new string EntityStructuredTypeKey = "[J1A6]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.Location Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>Location</c> entity.
	///	designer:cap/J1A9
	///	</summary>
	public partial class LocationEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>PostalCode</c> field.
		///	designer:fld/J1A9/J1AA
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AA]")]
		public string PostalCode
		{
			get
			{
				return this.GetField<string> ("[J1AA]");
			}
			set
			{
				string oldValue = this.PostalCode;
				if (oldValue != value || !this.IsFieldDefined("[J1AA]"))
				{
					this.OnPostalCodeChanging (oldValue, value);
					this.SetField<string> ("[J1AA]", oldValue, value);
					this.OnPostalCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/J1A9/J1AB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AB]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[J1AB]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[J1AB]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[J1AB]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Country</c> field.
		///	designer:fld/J1A9/J1AC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AC]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity Country
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity> ("[J1AC]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity oldValue = this.Country;
				if (oldValue != value || !this.IsFieldDefined("[J1AC]"))
				{
					this.OnCountryChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CountryEntity> ("[J1AC]", oldValue, value);
					this.OnCountryChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Region</c> field.
		///	designer:fld/J1A9/J1AD
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AD]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.RegionEntity Region
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.RegionEntity> ("[J1AD]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.RegionEntity oldValue = this.Region;
				if (oldValue != value || !this.IsFieldDefined("[J1AD]"))
				{
					this.OnRegionChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.RegionEntity> ("[J1AD]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 9);	// [J1A9]
		public static readonly new string EntityStructuredTypeKey = "[J1A9]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.PostBox Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>PostBox</c> entity.
	///	designer:cap/J1AE
	///	</summary>
	public partial class PostBoxEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Number</c> field.
		///	designer:fld/J1AE/J1AF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AF]")]
		public string Number
		{
			get
			{
				return this.GetField<string> ("[J1AF]");
			}
			set
			{
				string oldValue = this.Number;
				if (oldValue != value || !this.IsFieldDefined("[J1AF]"))
				{
					this.OnNumberChanging (oldValue, value);
					this.SetField<string> ("[J1AF]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 14);	// [J1AE]
		public static readonly new string EntityStructuredTypeKey = "[J1AE]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.Street Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>Street</c> entity.
	///	designer:cap/J1AG
	///	</summary>
	public partial class StreetEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Complement</c> field.
		///	designer:fld/J1AG/J1AH
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AH]")]
		public string Complement
		{
			get
			{
				return this.GetField<string> ("[J1AH]");
			}
			set
			{
				string oldValue = this.Complement;
				if (oldValue != value || !this.IsFieldDefined("[J1AH]"))
				{
					this.OnComplementChanging (oldValue, value);
					this.SetField<string> ("[J1AH]", oldValue, value);
					this.OnComplementChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>StreetName</c> field.
		///	designer:fld/J1AG/J1AI
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AI]")]
		public string StreetName
		{
			get
			{
				return this.GetField<string> ("[J1AI]");
			}
			set
			{
				string oldValue = this.StreetName;
				if (oldValue != value || !this.IsFieldDefined("[J1AI]"))
				{
					this.OnStreetNameChanging (oldValue, value);
					this.SetField<string> ("[J1AI]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 16);	// [J1AG]
		public static readonly new string EntityStructuredTypeKey = "[J1AG]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.Address Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>Address</c> entity.
	///	designer:cap/J1AJ
	///	</summary>
	public partial class AddressEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Street</c> field.
		///	designer:fld/J1AJ/J1AK
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AK]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.StreetEntity Street
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.StreetEntity> ("[J1AK]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.StreetEntity oldValue = this.Street;
				if (oldValue != value || !this.IsFieldDefined("[J1AK]"))
				{
					this.OnStreetChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.StreetEntity> ("[J1AK]", oldValue, value);
					this.OnStreetChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PostBox</c> field.
		///	designer:fld/J1AJ/J1AL
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AL]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PostBoxEntity PostBox
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PostBoxEntity> ("[J1AL]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PostBoxEntity oldValue = this.PostBox;
				if (oldValue != value || !this.IsFieldDefined("[J1AL]"))
				{
					this.OnPostBoxChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PostBoxEntity> ("[J1AL]", oldValue, value);
					this.OnPostBoxChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Location</c> field.
		///	designer:fld/J1AJ/J1AM
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AM]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LocationEntity Location
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LocationEntity> ("[J1AM]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LocationEntity oldValue = this.Location;
				if (oldValue != value || !this.IsFieldDefined("[J1AM]"))
				{
					this.OnLocationChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LocationEntity> ("[J1AM]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 19);	// [J1AJ]
		public static readonly new string EntityStructuredTypeKey = "[J1AJ]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.PersonTitle Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>PersonTitle</c> entity.
	///	designer:cap/J1AN
	///	</summary>
	public partial class PersonTitleEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRank
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/J1AN/J1A1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A1]")]
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
		///	designer:fld/J1AN/J1AO
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AO]")]
		public string ShortName
		{
			get
			{
				return this.GetField<string> ("[J1AO]");
			}
			set
			{
				string oldValue = this.ShortName;
				if (oldValue != value || !this.IsFieldDefined("[J1AO]"))
				{
					this.OnShortNameChanging (oldValue, value);
					this.SetField<string> ("[J1AO]", oldValue, value);
					this.OnShortNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/J1AN/J1AP
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AP]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[J1AP]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[J1AP]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[J1AP]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ComptatibleGenders</c> field.
		///	designer:fld/J1AN/J1AS
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AS]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity> ComptatibleGenders
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity> ("[J1AS]");
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 23);	// [J1AN]
		public static readonly new string EntityStructuredTypeKey = "[J1AN]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.PersonGender Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>PersonGender</c> entity.
	///	designer:cap/J1AQ
	///	</summary>
	public partial class PersonGenderEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRank, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCode
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/J1AQ/J1A1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A1]")]
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
		///	designer:fld/J1AQ/J1A3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A3]")]
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
		///	designer:fld/J1AQ/J1AR
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AR]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[J1AR]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[J1AR]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[J1AR]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 26);	// [J1AQ]
		public static readonly new string EntityStructuredTypeKey = "[J1AQ]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.Language Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>Language</c> entity.
	///	designer:cap/J1AT
	///	</summary>
	public partial class LanguageEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCode
	{
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/J1AT/J1A3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A3]")]
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
		///	designer:fld/J1AT/J1AU
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AU]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[J1AU]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[J1AU]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[J1AU]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 29);	// [J1AT]
		public static readonly new string EntityStructuredTypeKey = "[J1AT]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.ContactRole Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>ContactRole</c> entity.
	///	designer:cap/J1AV
	///	</summary>
	public partial class ContactRoleEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRank
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/J1AV/J1A1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A1]")]
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
		///	designer:fld/J1AV/J1A01
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A01]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[J1A01]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[J1A01]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[J1A01]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 31);	// [J1AV]
		public static readonly new string EntityStructuredTypeKey = "[J1AV]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.LegalPersonType Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>LegalPersonType</c> entity.
	///	designer:cap/J1A11
	///	</summary>
	public partial class LegalPersonTypeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRank
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/J1A11/J1A1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A1]")]
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
		///	designer:fld/J1A11/J1A21
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A21]")]
		public string ShortName
		{
			get
			{
				return this.GetField<string> ("[J1A21]");
			}
			set
			{
				string oldValue = this.ShortName;
				if (oldValue != value || !this.IsFieldDefined("[J1A21]"))
				{
					this.OnShortNameChanging (oldValue, value);
					this.SetField<string> ("[J1A21]", oldValue, value);
					this.OnShortNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/J1A11/J1A31
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A31]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[J1A31]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[J1A31]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[J1A31]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 33);	// [J1A11]
		public static readonly new string EntityStructuredTypeKey = "[J1A11]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.Comment Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>Comment</c> entity.
	///	designer:cap/J1A41
	///	</summary>
	public partial class CommentEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Text</c> field.
		///	designer:fld/J1A41/J1A51
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A51]")]
		public string Text
		{
			get
			{
				return this.GetField<string> ("[J1A51]");
			}
			set
			{
				string oldValue = this.Text;
				if (oldValue != value || !this.IsFieldDefined("[J1A51]"))
				{
					this.OnTextChanging (oldValue, value);
					this.SetField<string> ("[J1A51]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 36);	// [J1A41]
		public static readonly new string EntityStructuredTypeKey = "[J1A41]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.TelecomType Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>TelecomType</c> entity.
	///	designer:cap/J1A61
	///	</summary>
	public partial class TelecomTypeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRank, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCode
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/J1A61/J1A1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A1]")]
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
		///	designer:fld/J1A61/J1A3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A3]")]
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
		///	designer:fld/J1A61/J1A71
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A71]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[J1A71]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[J1A71]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[J1A71]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 38);	// [J1A61]
		public static readonly new string EntityStructuredTypeKey = "[J1A61]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.UriScheme Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>UriScheme</c> entity.
	///	designer:cap/J1A81
	///	</summary>
	public partial class UriSchemeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemCode, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.IItemRank
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/J1A81/J1A1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A1]")]
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
		///	designer:fld/J1A81/J1A3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A3]")]
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
		///	designer:fld/J1A81/J1A91
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A91]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[J1A91]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[J1A91]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[J1A91]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 40);	// [J1A81]
		public static readonly new string EntityStructuredTypeKey = "[J1A81]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.AbstractContact Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>AbstractContact</c> entity.
	///	designer:cap/J1AA1
	///	</summary>
	public partial class AbstractContactEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Roles</c> field.
		///	designer:fld/J1AA1/J1AP1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AP1]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.ContactRoleEntity> Roles
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.ContactRoleEntity> ("[J1AP1]");
			}
		}
		///	<summary>
		///	The <c>Comments</c> field.
		///	designer:fld/J1AA1/J1AQ1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AQ1]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CommentEntity> Comments
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.CommentEntity> ("[J1AQ1]");
			}
		}
		///	<summary>
		///	The <c>NaturalPerson</c> field.
		///	designer:fld/J1AA1/J1AR1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AR1]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity NaturalPerson
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity> ("[J1AR1]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity oldValue = this.NaturalPerson;
				if (oldValue != value || !this.IsFieldDefined("[J1AR1]"))
				{
					this.OnNaturalPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity> ("[J1AR1]", oldValue, value);
					this.OnNaturalPersonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LegalPerson</c> field.
		///	designer:fld/J1AA1/J1AS1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AS1]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity LegalPerson
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity> ("[J1AS1]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity oldValue = this.LegalPerson;
				if (oldValue != value || !this.IsFieldDefined("[J1AS1]"))
				{
					this.OnLegalPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity> ("[J1AS1]", oldValue, value);
					this.OnLegalPersonChanged (oldValue, value);
				}
			}
		}
		
		partial void OnNaturalPersonChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity newValue);
		partial void OnNaturalPersonChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.NaturalPersonEntity newValue);
		partial void OnLegalPersonChanging(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity newValue);
		partial void OnLegalPersonChanged(global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity oldValue, global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractContactEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractContactEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 42);	// [J1AA1]
		public static readonly new string EntityStructuredTypeKey = "[J1AA1]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.AbstractPerson Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>AbstractPerson</c> entity.
	///	designer:cap/J1AB1
	///	</summary>
	public partial class AbstractPersonEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Contacts</c> field.
		///	designer:fld/J1AB1/J1AC1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AC1]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractContactEntity> Contacts
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractContactEntity> ("[J1AC1]");
			}
		}
		///	<summary>
		///	The <c>PreferredLanguage</c> field.
		///	designer:fld/J1AB1/J1AD1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AD1]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LanguageEntity PreferredLanguage
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LanguageEntity> ("[J1AD1]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LanguageEntity oldValue = this.PreferredLanguage;
				if (oldValue != value || !this.IsFieldDefined("[J1AD1]"))
				{
					this.OnPreferredLanguageChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LanguageEntity> ("[J1AD1]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 43);	// [J1AB1]
		public static readonly new string EntityStructuredTypeKey = "[J1AB1]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.LegalPerson Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>LegalPerson</c> entity.
	///	designer:cap/J1AE1
	///	</summary>
	public partial class LegalPersonEntity : global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractPersonEntity
	{
		///	<summary>
		///	The <c>LegalPersonType</c> field.
		///	designer:fld/J1AE1/J1AF1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AF1]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonTypeEntity LegalPersonType
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonTypeEntity> ("[J1AF1]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonTypeEntity oldValue = this.LegalPersonType;
				if (oldValue != value || !this.IsFieldDefined("[J1AF1]"))
				{
					this.OnLegalPersonTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.LegalPersonTypeEntity> ("[J1AF1]", oldValue, value);
					this.OnLegalPersonTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/J1AE1/J1AG1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AG1]")]
		public string Name
		{
			get
			{
				return this.GetField<string> ("[J1AG1]");
			}
			set
			{
				string oldValue = this.Name;
				if (oldValue != value || !this.IsFieldDefined("[J1AG1]"))
				{
					this.OnNameChanging (oldValue, value);
					this.SetField<string> ("[J1AG1]", oldValue, value);
					this.OnNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ShortName</c> field.
		///	designer:fld/J1AE1/J1AH1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AH1]")]
		public string ShortName
		{
			get
			{
				return this.GetField<string> ("[J1AH1]");
			}
			set
			{
				string oldValue = this.ShortName;
				if (oldValue != value || !this.IsFieldDefined("[J1AH1]"))
				{
					this.OnShortNameChanging (oldValue, value);
					this.SetField<string> ("[J1AH1]", oldValue, value);
					this.OnShortNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Complement</c> field.
		///	designer:fld/J1AE1/J1AI1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AI1]")]
		public string Complement
		{
			get
			{
				return this.GetField<string> ("[J1AI1]");
			}
			set
			{
				string oldValue = this.Complement;
				if (oldValue != value || !this.IsFieldDefined("[J1AI1]"))
				{
					this.OnComplementChanging (oldValue, value);
					this.SetField<string> ("[J1AI1]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 46);	// [J1AE1]
		public static readonly new string EntityStructuredTypeKey = "[J1AE1]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.NaturalPerson Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>NaturalPerson</c> entity.
	///	designer:cap/J1AJ1
	///	</summary>
	public partial class NaturalPersonEntity : global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractPersonEntity
	{
		///	<summary>
		///	The <c>Title</c> field.
		///	designer:fld/J1AJ1/J1AK1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AK1]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonTitleEntity Title
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonTitleEntity> ("[J1AK1]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonTitleEntity oldValue = this.Title;
				if (oldValue != value || !this.IsFieldDefined("[J1AK1]"))
				{
					this.OnTitleChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonTitleEntity> ("[J1AK1]", oldValue, value);
					this.OnTitleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Firstname</c> field.
		///	designer:fld/J1AJ1/J1AL1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AL1]")]
		public string Firstname
		{
			get
			{
				return this.GetField<string> ("[J1AL1]");
			}
			set
			{
				string oldValue = this.Firstname;
				if (oldValue != value || !this.IsFieldDefined("[J1AL1]"))
				{
					this.OnFirstnameChanging (oldValue, value);
					this.SetField<string> ("[J1AL1]", oldValue, value);
					this.OnFirstnameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Lastname</c> field.
		///	designer:fld/J1AJ1/J1AM1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AM1]")]
		public string Lastname
		{
			get
			{
				return this.GetField<string> ("[J1AM1]");
			}
			set
			{
				string oldValue = this.Lastname;
				if (oldValue != value || !this.IsFieldDefined("[J1AM1]"))
				{
					this.OnLastnameChanging (oldValue, value);
					this.SetField<string> ("[J1AM1]", oldValue, value);
					this.OnLastnameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Gender</c> field.
		///	designer:fld/J1AJ1/J1AN1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AN1]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity Gender
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity> ("[J1AN1]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity oldValue = this.Gender;
				if (oldValue != value || !this.IsFieldDefined("[J1AN1]"))
				{
					this.OnGenderChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.PersonGenderEntity> ("[J1AN1]", oldValue, value);
					this.OnGenderChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>BirthDate</c> field.
		///	designer:fld/J1AJ1/J1AO1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AO1]")]
		public global::Epsitec.Common.Types.Date? BirthDate
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[J1AO1]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.BirthDate;
				if (oldValue != value || !this.IsFieldDefined("[J1AO1]"))
				{
					this.OnBirthDateChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[J1AO1]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 51);	// [J1AJ1]
		public static readonly new string EntityStructuredTypeKey = "[J1AJ1]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.MailContact Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>MailContact</c> entity.
	///	designer:cap/J1AT1
	///	</summary>
	public partial class MailContactEntity : global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractContactEntity
	{
		///	<summary>
		///	The <c>Complement</c> field.
		///	designer:fld/J1AT1/J1AU1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AU1]")]
		public string Complement
		{
			get
			{
				return this.GetField<string> ("[J1AU1]");
			}
			set
			{
				string oldValue = this.Complement;
				if (oldValue != value || !this.IsFieldDefined("[J1AU1]"))
				{
					this.OnComplementChanging (oldValue, value);
					this.SetField<string> ("[J1AU1]", oldValue, value);
					this.OnComplementChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Address</c> field.
		///	designer:fld/J1AT1/J1AV1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AV1]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AddressEntity Address
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AddressEntity> ("[J1AV1]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AddressEntity oldValue = this.Address;
				if (oldValue != value || !this.IsFieldDefined("[J1AV1]"))
				{
					this.OnAddressChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AddressEntity> ("[J1AV1]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 61);	// [J1AT1]
		public static readonly new string EntityStructuredTypeKey = "[J1AT1]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.TelecomContact Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>TelecomContact</c> entity.
	///	designer:cap/J1A02
	///	</summary>
	public partial class TelecomContactEntity : global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractContactEntity
	{
		///	<summary>
		///	The <c>TelecomType</c> field.
		///	designer:fld/J1A02/J1A12
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A12]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomTypeEntity TelecomType
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomTypeEntity> ("[J1A12]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomTypeEntity oldValue = this.TelecomType;
				if (oldValue != value || !this.IsFieldDefined("[J1A12]"))
				{
					this.OnTelecomTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.TelecomTypeEntity> ("[J1A12]", oldValue, value);
					this.OnTelecomTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Number</c> field.
		///	designer:fld/J1A02/J1A22
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A22]")]
		public string Number
		{
			get
			{
				return this.GetField<string> ("[J1A22]");
			}
			set
			{
				string oldValue = this.Number;
				if (oldValue != value || !this.IsFieldDefined("[J1A22]"))
				{
					this.OnNumberChanging (oldValue, value);
					this.SetField<string> ("[J1A22]", oldValue, value);
					this.OnNumberChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Extension</c> field.
		///	designer:fld/J1A02/J1A32
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A32]")]
		public string Extension
		{
			get
			{
				return this.GetField<string> ("[J1A32]");
			}
			set
			{
				string oldValue = this.Extension;
				if (oldValue != value || !this.IsFieldDefined("[J1A32]"))
				{
					this.OnExtensionChanging (oldValue, value);
					this.SetField<string> ("[J1A32]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 64);	// [J1A02]
		public static readonly new string EntityStructuredTypeKey = "[J1A02]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.UriContact Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>UriContact</c> entity.
	///	designer:cap/J1A42
	///	</summary>
	public partial class UriContactEntity : global::Epsitec.Cresus.DataLayer.UnitTests.Entities.AbstractContactEntity
	{
		///	<summary>
		///	The <c>UriScheme</c> field.
		///	designer:fld/J1A42/J1A52
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A52]")]
		public global::Epsitec.Cresus.DataLayer.UnitTests.Entities.UriSchemeEntity UriScheme
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.UriSchemeEntity> ("[J1A52]");
			}
			set
			{
				global::Epsitec.Cresus.DataLayer.UnitTests.Entities.UriSchemeEntity oldValue = this.UriScheme;
				if (oldValue != value || !this.IsFieldDefined("[J1A52]"))
				{
					this.OnUriSchemeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.DataLayer.UnitTests.Entities.UriSchemeEntity> ("[J1A52]", oldValue, value);
					this.OnUriSchemeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Uri</c> field.
		///	designer:fld/J1A42/J1A62
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A62]")]
		public string Uri
		{
			get
			{
				return this.GetField<string> ("[J1A62]");
			}
			set
			{
				string oldValue = this.Uri;
				if (oldValue != value || !this.IsFieldDefined("[J1A62]"))
				{
					this.OnUriChanging (oldValue, value);
					this.SetField<string> ("[J1A62]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 68);	// [J1A42]
		public static readonly new string EntityStructuredTypeKey = "[J1A42]";
	}
}
#endregion

#region Epsitec.Cresus.DataLayer.UnitTests.ValueData Entity
namespace Epsitec.Cresus.DataLayer.UnitTests.Entities
{
	///	<summary>
	///	The <c>ValueData</c> entity.
	///	designer:cap/J1A72
	///	</summary>
	public partial class ValueDataEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>BooleanValue</c> field.
		///	designer:fld/J1A72/J1A82
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A82]")]
		public bool BooleanValue
		{
			get
			{
				return this.GetField<bool> ("[J1A82]");
			}
			set
			{
				bool oldValue = this.BooleanValue;
				if (oldValue != value || !this.IsFieldDefined("[J1A82]"))
				{
					this.OnBooleanValueChanging (oldValue, value);
					this.SetField<bool> ("[J1A82]", oldValue, value);
					this.OnBooleanValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ByteArrayValue</c> field.
		///	designer:fld/J1A72/J1A92
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1A92]")]
		public global::System.Byte[] ByteArrayValue
		{
			get
			{
				return this.GetField<global::System.Byte[]> ("[J1A92]");
			}
			set
			{
				global::System.Byte[] oldValue = this.ByteArrayValue;
				if (oldValue != value || !this.IsFieldDefined("[J1A92]"))
				{
					this.OnByteArrayValueChanging (oldValue, value);
					this.SetField<global::System.Byte[]> ("[J1A92]", oldValue, value);
					this.OnByteArrayValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DateTimeValue</c> field.
		///	designer:fld/J1A72/J1AA2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AA2]")]
		public global::System.DateTime DateTimeValue
		{
			get
			{
				return this.GetField<global::System.DateTime> ("[J1AA2]");
			}
			set
			{
				global::System.DateTime oldValue = this.DateTimeValue;
				if (oldValue != value || !this.IsFieldDefined("[J1AA2]"))
				{
					this.OnDateTimeValueChanging (oldValue, value);
					this.SetField<global::System.DateTime> ("[J1AA2]", oldValue, value);
					this.OnDateTimeValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DateValue</c> field.
		///	designer:fld/J1A72/J1AB2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AB2]")]
		public global::Epsitec.Common.Types.Date DateValue
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date> ("[J1AB2]");
			}
			set
			{
				global::Epsitec.Common.Types.Date oldValue = this.DateValue;
				if (oldValue != value || !this.IsFieldDefined("[J1AB2]"))
				{
					this.OnDateValueChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date> ("[J1AB2]", oldValue, value);
					this.OnDateValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DecimalValue</c> field.
		///	designer:fld/J1A72/J1AC2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AC2]")]
		public global::System.Decimal DecimalValue
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[J1AC2]");
			}
			set
			{
				global::System.Decimal oldValue = this.DecimalValue;
				if (oldValue != value || !this.IsFieldDefined("[J1AC2]"))
				{
					this.OnDecimalValueChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[J1AC2]", oldValue, value);
					this.OnDecimalValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>IntegerValue</c> field.
		///	designer:fld/J1A72/J1AD2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AD2]")]
		public int IntegerValue
		{
			get
			{
				return this.GetField<int> ("[J1AD2]");
			}
			set
			{
				int oldValue = this.IntegerValue;
				if (oldValue != value || !this.IsFieldDefined("[J1AD2]"))
				{
					this.OnIntegerValueChanging (oldValue, value);
					this.SetField<int> ("[J1AD2]", oldValue, value);
					this.OnIntegerValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LongIntegerValue</c> field.
		///	designer:fld/J1A72/J1AE2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AE2]")]
		public long LongIntegerValue
		{
			get
			{
				return this.GetField<long> ("[J1AE2]");
			}
			set
			{
				long oldValue = this.LongIntegerValue;
				if (oldValue != value || !this.IsFieldDefined("[J1AE2]"))
				{
					this.OnLongIntegerValueChanging (oldValue, value);
					this.SetField<long> ("[J1AE2]", oldValue, value);
					this.OnLongIntegerValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>StringValue</c> field.
		///	designer:fld/J1A72/J1AF2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AF2]")]
		public string StringValue
		{
			get
			{
				return this.GetField<string> ("[J1AF2]");
			}
			set
			{
				string oldValue = this.StringValue;
				if (oldValue != value || !this.IsFieldDefined("[J1AF2]"))
				{
					this.OnStringValueChanging (oldValue, value);
					this.SetField<string> ("[J1AF2]", oldValue, value);
					this.OnStringValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TimeValue</c> field.
		///	designer:fld/J1A72/J1AG2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[J1AG2]")]
		public global::Epsitec.Common.Types.Time TimeValue
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Time> ("[J1AG2]");
			}
			set
			{
				global::Epsitec.Common.Types.Time oldValue = this.TimeValue;
				if (oldValue != value || !this.IsFieldDefined("[J1AG2]"))
				{
					this.OnTimeValueChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Time> ("[J1AG2]", oldValue, value);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (51, 10, 71);	// [J1A72]
		public static readonly new string EntityStructuredTypeKey = "[J1A72]";
	}
}
#endregion

