//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[MVA]", typeof (Epsitec.Data.Platform.Entities.MatchStreetEntity))]
#region Epsitec.Data.Platform.MatchStreet Entity
namespace Epsitec.Data.Platform.Entities
{
	///	<summary>
	///	The <c>MatchStreet</c> entity.
	///	designer:cap/MVA
	///	</summary>
	public partial class MatchStreetEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.IItemCode
	{
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/MVA/8VA5
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
		///	The <c>BasicPostcode</c> field.
		///	designer:fld/MVA/MVA1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[MVA1]")]
		public int BasicPostcode
		{
			get
			{
				return this.GetField<int> ("[MVA1]");
			}
			set
			{
				int oldValue = this.BasicPostcode;
				if (oldValue != value || !this.IsFieldDefined("[MVA1]"))
				{
					this.OnBasicPostcodeChanging (oldValue, value);
					this.SetField<int> ("[MVA1]", oldValue, value);
					this.OnBasicPostcodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LanguageCode</c> field.
		///	designer:fld/MVA/MVAA
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[MVAA]")]
		public int LanguageCode
		{
			get
			{
				return this.GetField<int> ("[MVAA]");
			}
			set
			{
				int oldValue = this.LanguageCode;
				if (oldValue != value || !this.IsFieldDefined("[MVAA]"))
				{
					this.OnLanguageCodeChanging (oldValue, value);
					this.SetField<int> ("[MVAA]", oldValue, value);
					this.OnLanguageCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AddressPostcode</c> field.
		///	designer:fld/MVA/MVA2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[MVA2]")]
		public string AddressPostcode
		{
			get
			{
				return this.GetField<string> ("[MVA2]");
			}
			set
			{
				string oldValue = this.AddressPostcode;
				if (oldValue != value || !this.IsFieldDefined("[MVA2]"))
				{
					this.OnAddressPostcodeChanging (oldValue, value);
					this.SetField<string> ("[MVA2]", oldValue, value);
					this.OnAddressPostcodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AddressPostcodeSuffix</c> field.
		///	designer:fld/MVA/MVA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[MVA3]")]
		public string AddressPostcodeSuffix
		{
			get
			{
				return this.GetField<string> ("[MVA3]");
			}
			set
			{
				string oldValue = this.AddressPostcodeSuffix;
				if (oldValue != value || !this.IsFieldDefined("[MVA3]"))
				{
					this.OnAddressPostcodeSuffixChanging (oldValue, value);
					this.SetField<string> ("[MVA3]", oldValue, value);
					this.OnAddressPostcodeSuffixChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>HouseNumberFrom</c> field.
		///	designer:fld/MVA/MVA4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[MVA4]")]
		public int HouseNumberFrom
		{
			get
			{
				return this.GetField<int> ("[MVA4]");
			}
			set
			{
				int oldValue = this.HouseNumberFrom;
				if (oldValue != value || !this.IsFieldDefined("[MVA4]"))
				{
					this.OnHouseNumberFromChanging (oldValue, value);
					this.SetField<int> ("[MVA4]", oldValue, value);
					this.OnHouseNumberFromChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>HouseNumberFromSuffix</c> field.
		///	designer:fld/MVA/MVAB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[MVAB]")]
		public string HouseNumberFromSuffix
		{
			get
			{
				return this.GetField<string> ("[MVAB]");
			}
			set
			{
				string oldValue = this.HouseNumberFromSuffix;
				if (oldValue != value || !this.IsFieldDefined("[MVAB]"))
				{
					this.OnHouseNumberFromSuffixChanging (oldValue, value);
					this.SetField<string> ("[MVAB]", oldValue, value);
					this.OnHouseNumberFromSuffixChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>HouseNumberTo</c> field.
		///	designer:fld/MVA/MVA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[MVA5]")]
		public int HouseNumberTo
		{
			get
			{
				return this.GetField<int> ("[MVA5]");
			}
			set
			{
				int oldValue = this.HouseNumberTo;
				if (oldValue != value || !this.IsFieldDefined("[MVA5]"))
				{
					this.OnHouseNumberToChanging (oldValue, value);
					this.SetField<int> ("[MVA5]", oldValue, value);
					this.OnHouseNumberToChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>HouseNumberToSuffix</c> field.
		///	designer:fld/MVA/MVAC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[MVAC]")]
		public string HouseNumberToSuffix
		{
			get
			{
				return this.GetField<string> ("[MVAC]");
			}
			set
			{
				string oldValue = this.HouseNumberToSuffix;
				if (oldValue != value || !this.IsFieldDefined("[MVAC]"))
				{
					this.OnHouseNumberToSuffixChanging (oldValue, value);
					this.SetField<string> ("[MVAC]", oldValue, value);
					this.OnHouseNumberToSuffixChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>StreetName</c> field.
		///	designer:fld/MVA/MVA6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[MVA6]")]
		public string StreetName
		{
			get
			{
				return this.GetField<string> ("[MVA6]");
			}
			set
			{
				string oldValue = this.StreetName;
				if (oldValue != value || !this.IsFieldDefined("[MVA6]"))
				{
					this.OnStreetNameChanging (oldValue, value);
					this.SetField<string> ("[MVA6]", oldValue, value);
					this.OnStreetNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>StreetNameRoot</c> field.
		///	designer:fld/MVA/MVA7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[MVA7]")]
		public string StreetNameRoot
		{
			get
			{
				return this.GetField<string> ("[MVA7]");
			}
			set
			{
				string oldValue = this.StreetNameRoot;
				if (oldValue != value || !this.IsFieldDefined("[MVA7]"))
				{
					this.OnStreetNameRootChanging (oldValue, value);
					this.SetField<string> ("[MVA7]", oldValue, value);
					this.OnStreetNameRootChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>StreetTypeCode</c> field.
		///	designer:fld/MVA/MVA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[MVA8]")]
		public int StreetTypeCode
		{
			get
			{
				return this.GetField<int> ("[MVA8]");
			}
			set
			{
				int oldValue = this.StreetTypeCode;
				if (oldValue != value || !this.IsFieldDefined("[MVA8]"))
				{
					this.OnStreetTypeCodeChanging (oldValue, value);
					this.SetField<int> ("[MVA8]", oldValue, value);
					this.OnStreetTypeCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PrepositionCode</c> field.
		///	designer:fld/MVA/MVA9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[MVA9]")]
		public int PrepositionCode
		{
			get
			{
				return this.GetField<int> ("[MVA9]");
			}
			set
			{
				int oldValue = this.PrepositionCode;
				if (oldValue != value || !this.IsFieldDefined("[MVA9]"))
				{
					this.OnPrepositionCodeChanging (oldValue, value);
					this.SetField<int> ("[MVA9]", oldValue, value);
					this.OnPrepositionCodeChanged (oldValue, value);
				}
			}
		}
		
		partial void OnBasicPostcodeChanging(int oldValue, int newValue);
		partial void OnBasicPostcodeChanged(int oldValue, int newValue);
		partial void OnLanguageCodeChanging(int oldValue, int newValue);
		partial void OnLanguageCodeChanged(int oldValue, int newValue);
		partial void OnAddressPostcodeChanging(string oldValue, string newValue);
		partial void OnAddressPostcodeChanged(string oldValue, string newValue);
		partial void OnAddressPostcodeSuffixChanging(string oldValue, string newValue);
		partial void OnAddressPostcodeSuffixChanged(string oldValue, string newValue);
		partial void OnHouseNumberFromChanging(int oldValue, int newValue);
		partial void OnHouseNumberFromChanged(int oldValue, int newValue);
		partial void OnHouseNumberFromSuffixChanging(string oldValue, string newValue);
		partial void OnHouseNumberFromSuffixChanged(string oldValue, string newValue);
		partial void OnHouseNumberToChanging(int oldValue, int newValue);
		partial void OnHouseNumberToChanged(int oldValue, int newValue);
		partial void OnHouseNumberToSuffixChanging(string oldValue, string newValue);
		partial void OnHouseNumberToSuffixChanged(string oldValue, string newValue);
		partial void OnStreetNameChanging(string oldValue, string newValue);
		partial void OnStreetNameChanged(string oldValue, string newValue);
		partial void OnStreetNameRootChanging(string oldValue, string newValue);
		partial void OnStreetNameRootChanged(string oldValue, string newValue);
		partial void OnStreetTypeCodeChanging(int oldValue, int newValue);
		partial void OnStreetTypeCodeChanged(int oldValue, int newValue);
		partial void OnPrepositionCodeChanging(int oldValue, int newValue);
		partial void OnPrepositionCodeChanged(int oldValue, int newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Data.Platform.Entities.MatchStreetEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Data.Platform.Entities.MatchStreetEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1014, 10, 0);	// [MVA]
		public static readonly string EntityStructuredTypeKey = "[MVA]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<MatchStreetEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

