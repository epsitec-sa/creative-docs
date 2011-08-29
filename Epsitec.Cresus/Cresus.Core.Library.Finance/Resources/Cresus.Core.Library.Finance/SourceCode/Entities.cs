//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[CVA9]", typeof (Epsitec.Cresus.Core.Entities.IsrDefinitionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[CVAL]", typeof (Epsitec.Cresus.Core.Entities.PaymentTransactionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[CVAT]", typeof (Epsitec.Cresus.Core.Entities.CurrencyEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[CVAU]", typeof (Epsitec.Cresus.Core.Entities.ExchangeRateSourceEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[CVAV]", typeof (Epsitec.Cresus.Core.Entities.PaymentCategoryEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[CVA21]", typeof (Epsitec.Cresus.Core.Entities.PaymentDetailEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[CVAR2]", typeof (Epsitec.Cresus.Core.Entities.VatDefinitionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[CVAP3]", typeof (Epsitec.Cresus.Core.Entities.TaxSettingsEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[CVA24]", typeof (Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[CVA44]", typeof (Epsitec.Cresus.Core.Entities.PriceDiscountEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[CVAA4]", typeof (Epsitec.Cresus.Core.Entities.PriceGroupEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[CVAF4]", typeof (Epsitec.Cresus.Core.Entities.PriceCalculatorEntity))]
#region Epsitec.Cresus.Core.IsrDefinition Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>IsrDefinition</c> entity.
	///	designer:cap/CVA9
	///	</summary>
	public partial class IsrDefinitionEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.INameDescription
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/CVA9/8VA3
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
		///	designer:fld/CVA9/8VA7
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
		///	designer:fld/CVA9/8VA8
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
		///	The <c>Currency</c> field.
		///	designer:fld/CVA9/CVAB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAB]")]
		public global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode Currency
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode> ("[CVAB]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue = this.Currency;
				if (oldValue != value || !this.IsFieldDefined("[CVAB]"))
				{
					this.OnCurrencyChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode> ("[CVAB]", oldValue, value);
					this.OnCurrencyChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SubscriberNumber</c> field.
		///	designer:fld/CVA9/CVAC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAC]")]
		public string SubscriberNumber
		{
			get
			{
				return this.GetField<string> ("[CVAC]");
			}
			set
			{
				string oldValue = this.SubscriberNumber;
				if (oldValue != value || !this.IsFieldDefined("[CVAC]"))
				{
					this.OnSubscriberNumberChanging (oldValue, value);
					this.SetField<string> ("[CVAC]", oldValue, value);
					this.OnSubscriberNumberChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SubscriberAddress</c> field.
		///	designer:fld/CVA9/CVAD
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAD]")]
		public global::Epsitec.Common.Types.FormattedText SubscriberAddress
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[CVAD]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.SubscriberAddress;
				if (oldValue != value || !this.IsFieldDefined("[CVAD]"))
				{
					this.OnSubscriberAddressChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[CVAD]", oldValue, value);
					this.OnSubscriberAddressChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>BankReferenceNumberPrefix</c> field.
		///	designer:fld/CVA9/CVAE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAE]")]
		public string BankReferenceNumberPrefix
		{
			get
			{
				return this.GetField<string> ("[CVAE]");
			}
			set
			{
				string oldValue = this.BankReferenceNumberPrefix;
				if (oldValue != value || !this.IsFieldDefined("[CVAE]"))
				{
					this.OnBankReferenceNumberPrefixChanging (oldValue, value);
					this.SetField<string> ("[CVAE]", oldValue, value);
					this.OnBankReferenceNumberPrefixChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>BankAddressLine1</c> field.
		///	designer:fld/CVA9/CVAG
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAG]")]
		public global::Epsitec.Common.Types.FormattedText BankAddressLine1
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[CVAG]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.BankAddressLine1;
				if (oldValue != value || !this.IsFieldDefined("[CVAG]"))
				{
					this.OnBankAddressLine1Changing (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[CVAG]", oldValue, value);
					this.OnBankAddressLine1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>BankAddressLine2</c> field.
		///	designer:fld/CVA9/CVAH
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAH]")]
		public global::Epsitec.Common.Types.FormattedText BankAddressLine2
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[CVAH]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.BankAddressLine2;
				if (oldValue != value || !this.IsFieldDefined("[CVAH]"))
				{
					this.OnBankAddressLine2Changing (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[CVAH]", oldValue, value);
					this.OnBankAddressLine2Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>BankAccount</c> field.
		///	designer:fld/CVA9/CVAI
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAI]")]
		public global::Epsitec.Common.Types.FormattedText BankAccount
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[CVAI]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.BankAccount;
				if (oldValue != value || !this.IsFieldDefined("[CVAI]"))
				{
					this.OnBankAccountChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[CVAI]", oldValue, value);
					this.OnBankAccountChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>IncomingBookAccount</c> field.
		///	designer:fld/CVA9/CVAJ
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAJ]")]
		public string IncomingBookAccount
		{
			get
			{
				return this.GetField<string> ("[CVAJ]");
			}
			set
			{
				string oldValue = this.IncomingBookAccount;
				if (oldValue != value || !this.IsFieldDefined("[CVAJ]"))
				{
					this.OnIncomingBookAccountChanging (oldValue, value);
					this.SetField<string> ("[CVAJ]", oldValue, value);
					this.OnIncomingBookAccountChanged (oldValue, value);
				}
			}
		}
		
		partial void OnCurrencyChanging(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode newValue);
		partial void OnCurrencyChanged(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode newValue);
		partial void OnSubscriberNumberChanging(string oldValue, string newValue);
		partial void OnSubscriberNumberChanged(string oldValue, string newValue);
		partial void OnSubscriberAddressChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnSubscriberAddressChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnBankReferenceNumberPrefixChanging(string oldValue, string newValue);
		partial void OnBankReferenceNumberPrefixChanged(string oldValue, string newValue);
		partial void OnBankAddressLine1Changing(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnBankAddressLine1Changed(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnBankAddressLine2Changing(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnBankAddressLine2Changed(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnBankAccountChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnBankAccountChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnIncomingBookAccountChanging(string oldValue, string newValue);
		partial void OnIncomingBookAccountChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.IsrDefinitionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.IsrDefinitionEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 9);	// [CVA9]
		public static readonly string EntityStructuredTypeKey = "[CVA9]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<IsrDefinitionEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.PaymentTransaction Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>PaymentTransaction</c> entity.
	///	designer:cap/CVAL
	///	</summary>
	public partial class PaymentTransactionEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.IItemCode
	{
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/CVAL/8VA5
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
		///	The <c>Text</c> field.
		///	designer:fld/CVAL/CVAM
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAM]")]
		public global::Epsitec.Common.Types.FormattedText Text
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[CVAM]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Text;
				if (oldValue != value || !this.IsFieldDefined("[CVAM]"))
				{
					this.OnTextChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[CVAM]", oldValue, value);
					this.OnTextChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PaymentDetail</c> field.
		///	designer:fld/CVAL/CVAN
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAN]")]
		public global::Epsitec.Cresus.Core.Entities.PaymentDetailEntity PaymentDetail
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PaymentDetailEntity> ("[CVAN]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PaymentDetailEntity oldValue = this.PaymentDetail;
				if (oldValue != value || !this.IsFieldDefined("[CVAN]"))
				{
					this.OnPaymentDetailChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PaymentDetailEntity> ("[CVAN]", oldValue, value);
					this.OnPaymentDetailChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TransactionId</c> field.
		///	designer:fld/CVAL/CVAO
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAO]")]
		public string TransactionId
		{
			get
			{
				return this.GetField<string> ("[CVAO]");
			}
			set
			{
				string oldValue = this.TransactionId;
				if (oldValue != value || !this.IsFieldDefined("[CVAO]"))
				{
					this.OnTransactionIdChanging (oldValue, value);
					this.SetField<string> ("[CVAO]", oldValue, value);
					this.OnTransactionIdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>IsrDefinition</c> field.
		///	designer:fld/CVAL/CVAP
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAP]")]
		public global::Epsitec.Cresus.Core.Entities.IsrDefinitionEntity IsrDefinition
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.IsrDefinitionEntity> ("[CVAP]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IsrDefinitionEntity oldValue = this.IsrDefinition;
				if (oldValue != value || !this.IsFieldDefined("[CVAP]"))
				{
					this.OnIsrDefinitionChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.IsrDefinitionEntity> ("[CVAP]", oldValue, value);
					this.OnIsrDefinitionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>IsrReferenceNumber</c> field.
		///	designer:fld/CVAL/CVAQ
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAQ]")]
		public string IsrReferenceNumber
		{
			get
			{
				return this.GetField<string> ("[CVAQ]");
			}
			set
			{
				string oldValue = this.IsrReferenceNumber;
				if (oldValue != value || !this.IsFieldDefined("[CVAQ]"))
				{
					this.OnIsrReferenceNumberChanging (oldValue, value);
					this.SetField<string> ("[CVAQ]", oldValue, value);
					this.OnIsrReferenceNumberChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>InstalmentRank</c> field.
		///	designer:fld/CVAL/CVAR
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAR]")]
		public int? InstalmentRank
		{
			get
			{
				return this.GetField<int?> ("[CVAR]");
			}
			set
			{
				int? oldValue = this.InstalmentRank;
				if (oldValue != value || !this.IsFieldDefined("[CVAR]"))
				{
					this.OnInstalmentRankChanging (oldValue, value);
					this.SetField<int?> ("[CVAR]", oldValue, value);
					this.OnInstalmentRankChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>InstalmentName</c> field.
		///	designer:fld/CVAL/CVAS
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAS]")]
		public global::Epsitec.Common.Types.FormattedText InstalmentName
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[CVAS]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.InstalmentName;
				if (oldValue != value || !this.IsFieldDefined("[CVAS]"))
				{
					this.OnInstalmentNameChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[CVAS]", oldValue, value);
					this.OnInstalmentNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTextChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnPaymentDetailChanging(global::Epsitec.Cresus.Core.Entities.PaymentDetailEntity oldValue, global::Epsitec.Cresus.Core.Entities.PaymentDetailEntity newValue);
		partial void OnPaymentDetailChanged(global::Epsitec.Cresus.Core.Entities.PaymentDetailEntity oldValue, global::Epsitec.Cresus.Core.Entities.PaymentDetailEntity newValue);
		partial void OnTransactionIdChanging(string oldValue, string newValue);
		partial void OnTransactionIdChanged(string oldValue, string newValue);
		partial void OnIsrDefinitionChanging(global::Epsitec.Cresus.Core.Entities.IsrDefinitionEntity oldValue, global::Epsitec.Cresus.Core.Entities.IsrDefinitionEntity newValue);
		partial void OnIsrDefinitionChanged(global::Epsitec.Cresus.Core.Entities.IsrDefinitionEntity oldValue, global::Epsitec.Cresus.Core.Entities.IsrDefinitionEntity newValue);
		partial void OnIsrReferenceNumberChanging(string oldValue, string newValue);
		partial void OnIsrReferenceNumberChanged(string oldValue, string newValue);
		partial void OnInstalmentRankChanging(int? oldValue, int? newValue);
		partial void OnInstalmentRankChanged(int? oldValue, int? newValue);
		partial void OnInstalmentNameChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnInstalmentNameChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.PaymentTransactionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.PaymentTransactionEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 21);	// [CVAL]
		public static readonly string EntityStructuredTypeKey = "[CVAL]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<PaymentTransactionEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.Currency Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>Currency</c> entity.
	///	designer:cap/CVAT
	///	</summary>
	public partial class CurrencyEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IDateRange
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/CVAT/8VA3
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
		#region IDateRange Members
		///	<summary>
		///	The <c>BeginDate</c> field.
		///	designer:fld/CVAT/8VAO
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
		///	designer:fld/CVAT/8VAP
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
		///	The <c>CurrencyCode</c> field.
		///	designer:fld/CVAT/CVA91
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA91]")]
		public global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode CurrencyCode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode> ("[CVA91]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue = this.CurrencyCode;
				if (oldValue != value || !this.IsFieldDefined("[CVA91]"))
				{
					this.OnCurrencyCodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode> ("[CVA91]", oldValue, value);
					this.OnCurrencyCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ExchangeRateBase</c> field.
		///	designer:fld/CVAT/CVAO4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAO4]")]
		public global::System.Decimal ExchangeRateBase
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[CVAO4]");
			}
			set
			{
				global::System.Decimal oldValue = this.ExchangeRateBase;
				if (oldValue != value || !this.IsFieldDefined("[CVAO4]"))
				{
					this.OnExchangeRateBaseChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[CVAO4]", oldValue, value);
					this.OnExchangeRateBaseChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ExchangeRate</c> field.
		///	designer:fld/CVAT/CVAN4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAN4]")]
		public global::System.Decimal ExchangeRate
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[CVAN4]");
			}
			set
			{
				global::System.Decimal oldValue = this.ExchangeRate;
				if (oldValue != value || !this.IsFieldDefined("[CVAN4]"))
				{
					this.OnExchangeRateChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[CVAN4]", oldValue, value);
					this.OnExchangeRateChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ExchangeRateSource</c> field.
		///	designer:fld/CVAT/CVAC1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAC1]")]
		public global::Epsitec.Cresus.Core.Entities.ExchangeRateSourceEntity ExchangeRateSource
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.ExchangeRateSourceEntity> ("[CVAC1]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ExchangeRateSourceEntity oldValue = this.ExchangeRateSource;
				if (oldValue != value || !this.IsFieldDefined("[CVAC1]"))
				{
					this.OnExchangeRateSourceChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.ExchangeRateSourceEntity> ("[CVAC1]", oldValue, value);
					this.OnExchangeRateSourceChanged (oldValue, value);
				}
			}
		}
		
		partial void OnCurrencyCodeChanging(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode newValue);
		partial void OnCurrencyCodeChanged(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode newValue);
		partial void OnExchangeRateBaseChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnExchangeRateBaseChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnExchangeRateChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnExchangeRateChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnExchangeRateSourceChanging(global::Epsitec.Cresus.Core.Entities.ExchangeRateSourceEntity oldValue, global::Epsitec.Cresus.Core.Entities.ExchangeRateSourceEntity newValue);
		partial void OnExchangeRateSourceChanged(global::Epsitec.Cresus.Core.Entities.ExchangeRateSourceEntity oldValue, global::Epsitec.Cresus.Core.Entities.ExchangeRateSourceEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.CurrencyEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.CurrencyEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 29);	// [CVAT]
		public static readonly string EntityStructuredTypeKey = "[CVAT]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<CurrencyEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.ExchangeRateSource Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ExchangeRateSource</c> entity.
	///	designer:cap/CVAU
	///	</summary>
	public partial class ExchangeRateSourceEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/CVAU/8VA3
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
		///	The <c>Type</c> field.
		///	designer:fld/CVAU/CVAD1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAD1]")]
		public global::Epsitec.Cresus.Core.Business.Finance.ExchangeRateSourceType Type
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.ExchangeRateSourceType> ("[CVAD1]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.ExchangeRateSourceType oldValue = this.Type;
				if (oldValue != value || !this.IsFieldDefined("[CVAD1]"))
				{
					this.OnTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.ExchangeRateSourceType> ("[CVAD1]", oldValue, value);
					this.OnTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Originator</c> field.
		///	designer:fld/CVAU/CVAE1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAE1]")]
		public string Originator
		{
			get
			{
				return this.GetField<string> ("[CVAE1]");
			}
			set
			{
				string oldValue = this.Originator;
				if (oldValue != value || !this.IsFieldDefined("[CVAE1]"))
				{
					this.OnOriginatorChanging (oldValue, value);
					this.SetField<string> ("[CVAE1]", oldValue, value);
					this.OnOriginatorChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTypeChanging(global::Epsitec.Cresus.Core.Business.Finance.ExchangeRateSourceType oldValue, global::Epsitec.Cresus.Core.Business.Finance.ExchangeRateSourceType newValue);
		partial void OnTypeChanged(global::Epsitec.Cresus.Core.Business.Finance.ExchangeRateSourceType oldValue, global::Epsitec.Cresus.Core.Business.Finance.ExchangeRateSourceType newValue);
		partial void OnOriginatorChanging(string oldValue, string newValue);
		partial void OnOriginatorChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ExchangeRateSourceEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ExchangeRateSourceEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 30);	// [CVAU]
		public static readonly string EntityStructuredTypeKey = "[CVAU]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ExchangeRateSourceEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.PaymentCategory Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>PaymentCategory</c> entity.
	///	designer:cap/CVAV
	///	</summary>
	public partial class PaymentCategoryEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory, global::Epsitec.Cresus.Core.Entities.IItemRank
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/CVAV/8VA1
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
		///	designer:fld/CVAV/8VA3
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
		///	designer:fld/CVAV/8VA5
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
		///	designer:fld/CVAV/8VA7
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
		///	designer:fld/CVAV/8VA8
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
		///	The <c>BookAccount</c> field.
		///	designer:fld/CVAV/CVA01
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA01]")]
		public string BookAccount
		{
			get
			{
				return this.GetField<string> ("[CVA01]");
			}
			set
			{
				string oldValue = this.BookAccount;
				if (oldValue != value || !this.IsFieldDefined("[CVA01]"))
				{
					this.OnBookAccountChanging (oldValue, value);
					this.SetField<string> ("[CVA01]", oldValue, value);
					this.OnBookAccountChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>StandardPaymentTerm</c> field.
		///	designer:fld/CVAV/CVA11
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA11]")]
		public int? StandardPaymentTerm
		{
			get
			{
				return this.GetField<int?> ("[CVA11]");
			}
			set
			{
				int? oldValue = this.StandardPaymentTerm;
				if (oldValue != value || !this.IsFieldDefined("[CVA11]"))
				{
					this.OnStandardPaymentTermChanging (oldValue, value);
					this.SetField<int?> ("[CVA11]", oldValue, value);
					this.OnStandardPaymentTermChanged (oldValue, value);
				}
			}
		}
		
		partial void OnBookAccountChanging(string oldValue, string newValue);
		partial void OnBookAccountChanged(string oldValue, string newValue);
		partial void OnStandardPaymentTermChanging(int? oldValue, int? newValue);
		partial void OnStandardPaymentTermChanged(int? oldValue, int? newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 31);	// [CVAV]
		public static readonly string EntityStructuredTypeKey = "[CVAV]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<PaymentCategoryEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.PaymentDetail Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>PaymentDetail</c> entity.
	///	designer:cap/CVA21
	///	</summary>
	public partial class PaymentDetailEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>PaymentType</c> field.
		///	designer:fld/CVA21/CVA31
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA31]")]
		public global::Epsitec.Cresus.Core.Business.Finance.PaymentDetailType PaymentType
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.PaymentDetailType> ("[CVA31]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.PaymentDetailType oldValue = this.PaymentType;
				if (oldValue != value || !this.IsFieldDefined("[CVA31]"))
				{
					this.OnPaymentTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.PaymentDetailType> ("[CVA31]", oldValue, value);
					this.OnPaymentTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PaymentCategory</c> field.
		///	designer:fld/CVA21/CVA41
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA41]")]
		public global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity PaymentCategory
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity> ("[CVA41]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity oldValue = this.PaymentCategory;
				if (oldValue != value || !this.IsFieldDefined("[CVA41]"))
				{
					this.OnPaymentCategoryChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity> ("[CVA41]", oldValue, value);
					this.OnPaymentCategoryChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PaymentData</c> field.
		///	designer:fld/CVA21/CVA51
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA51]")]
		public string PaymentData
		{
			get
			{
				return this.GetField<string> ("[CVA51]");
			}
			set
			{
				string oldValue = this.PaymentData;
				if (oldValue != value || !this.IsFieldDefined("[CVA51]"))
				{
					this.OnPaymentDataChanging (oldValue, value);
					this.SetField<string> ("[CVA51]", oldValue, value);
					this.OnPaymentDataChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Amount</c> field.
		///	designer:fld/CVA21/CVAP4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAP4]")]
		public global::System.Decimal Amount
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[CVAP4]");
			}
			set
			{
				global::System.Decimal oldValue = this.Amount;
				if (oldValue != value || !this.IsFieldDefined("[CVAP4]"))
				{
					this.OnAmountChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[CVAP4]", oldValue, value);
					this.OnAmountChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Currency</c> field.
		///	designer:fld/CVA21/CVA71
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA71]")]
		public global::Epsitec.Cresus.Core.Entities.CurrencyEntity Currency
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.CurrencyEntity> ("[CVA71]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.CurrencyEntity oldValue = this.Currency;
				if (oldValue != value || !this.IsFieldDefined("[CVA71]"))
				{
					this.OnCurrencyChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.CurrencyEntity> ("[CVA71]", oldValue, value);
					this.OnCurrencyChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Date</c> field.
		///	designer:fld/CVA21/CVA81
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA81]")]
		public global::Epsitec.Common.Types.Date? Date
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[CVA81]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.Date;
				if (oldValue != value || !this.IsFieldDefined("[CVA81]"))
				{
					this.OnDateChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[CVA81]", oldValue, value);
					this.OnDateChanged (oldValue, value);
				}
			}
		}
		
		partial void OnPaymentTypeChanging(global::Epsitec.Cresus.Core.Business.Finance.PaymentDetailType oldValue, global::Epsitec.Cresus.Core.Business.Finance.PaymentDetailType newValue);
		partial void OnPaymentTypeChanged(global::Epsitec.Cresus.Core.Business.Finance.PaymentDetailType oldValue, global::Epsitec.Cresus.Core.Business.Finance.PaymentDetailType newValue);
		partial void OnPaymentCategoryChanging(global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity oldValue, global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity newValue);
		partial void OnPaymentCategoryChanged(global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity oldValue, global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity newValue);
		partial void OnPaymentDataChanging(string oldValue, string newValue);
		partial void OnPaymentDataChanged(string oldValue, string newValue);
		partial void OnAmountChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnAmountChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnCurrencyChanging(global::Epsitec.Cresus.Core.Entities.CurrencyEntity oldValue, global::Epsitec.Cresus.Core.Entities.CurrencyEntity newValue);
		partial void OnCurrencyChanged(global::Epsitec.Cresus.Core.Entities.CurrencyEntity oldValue, global::Epsitec.Cresus.Core.Entities.CurrencyEntity newValue);
		partial void OnDateChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnDateChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.PaymentDetailEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.PaymentDetailEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 34);	// [CVA21]
		public static readonly string EntityStructuredTypeKey = "[CVA21]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<PaymentDetailEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.VatDefinition Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>VatDefinition</c> entity.
	///	designer:cap/CVAR2
	///	</summary>
	public partial class VatDefinitionEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IItemRank, global::Epsitec.Cresus.Core.Entities.IDateRange, global::Epsitec.Cresus.Core.Entities.INameDescription
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/CVAR2/8VA1
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
		///	designer:fld/CVAR2/8VA3
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
		///	designer:fld/CVAR2/8VA7
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
		#endregion
		#region IDateRange Members
		///	<summary>
		///	The <c>BeginDate</c> field.
		///	designer:fld/CVAR2/8VAO
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
		#region INameDescription Members
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/CVAR2/8VA8
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
		#region IDateRange Members
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/CVAR2/8VAP
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
		///	The <c>VatCode</c> field.
		///	designer:fld/CVAR2/CVAS2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAS2]")]
		public global::Epsitec.Cresus.Core.Business.Finance.VatCode VatCode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.VatCode> ("[CVAS2]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.VatCode oldValue = this.VatCode;
				if (oldValue != value || !this.IsFieldDefined("[CVAS2]"))
				{
					this.OnVatCodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.VatCode> ("[CVAS2]", oldValue, value);
					this.OnVatCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Rate</c> field.
		///	designer:fld/CVAR2/CVAS4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAS4]")]
		public global::System.Decimal Rate
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[CVAS4]");
			}
			set
			{
				global::System.Decimal oldValue = this.Rate;
				if (oldValue != value || !this.IsFieldDefined("[CVAS4]"))
				{
					this.OnRateChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[CVAS4]", oldValue, value);
					this.OnRateChanged (oldValue, value);
				}
			}
		}
		
		partial void OnVatCodeChanging(global::Epsitec.Cresus.Core.Business.Finance.VatCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.VatCode newValue);
		partial void OnVatCodeChanged(global::Epsitec.Cresus.Core.Business.Finance.VatCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.VatCode newValue);
		partial void OnRateChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnRateChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.VatDefinitionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.VatDefinitionEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 91);	// [CVAR2]
		public static readonly string EntityStructuredTypeKey = "[CVAR2]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<VatDefinitionEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.IRoundingMode Interface
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>IRoundingMode</c> entity.
	///	designer:cap/CVAM3
	///	</summary>
	public interface IRoundingMode
	{
		///	<summary>
		///	The <c>Modulo</c> field.
		///	designer:fld/CVAM3/CVAN3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAN3]")]
		global::System.Decimal Modulo
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>AddBeforeModulo</c> field.
		///	designer:fld/CVAM3/CVAO3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAO3]")]
		global::System.Decimal AddBeforeModulo
		{
			get;
			set;
		}
	}
	public static partial class IRoundingModeInterfaceImplementation
	{
		public static global::System.Decimal GetModulo(global::Epsitec.Cresus.Core.Entities.IRoundingMode obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::System.Decimal> ("[CVAN3]");
		}
		public static void SetModulo(global::Epsitec.Cresus.Core.Entities.IRoundingMode obj, global::System.Decimal value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::System.Decimal oldValue = obj.Modulo;
			if (oldValue != value || !entity.IsFieldDefined("[CVAN3]"))
			{
				IRoundingModeInterfaceImplementation.OnModuloChanging (obj, oldValue, value);
				entity.SetField<global::System.Decimal> ("[CVAN3]", oldValue, value);
				IRoundingModeInterfaceImplementation.OnModuloChanged (obj, oldValue, value);
			}
		}
		static partial void OnModuloChanged(global::Epsitec.Cresus.Core.Entities.IRoundingMode obj, global::System.Decimal oldValue, global::System.Decimal newValue);
		static partial void OnModuloChanging(global::Epsitec.Cresus.Core.Entities.IRoundingMode obj, global::System.Decimal oldValue, global::System.Decimal newValue);
		public static global::System.Decimal GetAddBeforeModulo(global::Epsitec.Cresus.Core.Entities.IRoundingMode obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::System.Decimal> ("[CVAO3]");
		}
		public static void SetAddBeforeModulo(global::Epsitec.Cresus.Core.Entities.IRoundingMode obj, global::System.Decimal value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::System.Decimal oldValue = obj.AddBeforeModulo;
			if (oldValue != value || !entity.IsFieldDefined("[CVAO3]"))
			{
				IRoundingModeInterfaceImplementation.OnAddBeforeModuloChanging (obj, oldValue, value);
				entity.SetField<global::System.Decimal> ("[CVAO3]", oldValue, value);
				IRoundingModeInterfaceImplementation.OnAddBeforeModuloChanged (obj, oldValue, value);
			}
		}
		static partial void OnAddBeforeModuloChanged(global::Epsitec.Cresus.Core.Entities.IRoundingMode obj, global::System.Decimal oldValue, global::System.Decimal newValue);
		static partial void OnAddBeforeModuloChanging(global::Epsitec.Cresus.Core.Entities.IRoundingMode obj, global::System.Decimal oldValue, global::System.Decimal newValue);
	}
}
#endregion

#region Epsitec.Cresus.Core.TaxSettings Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>TaxSettings</c> entity.
	///	designer:cap/CVAP3
	///	</summary>
	public partial class TaxSettingsEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>VatNumber</c> field.
		///	designer:fld/CVAP3/CVAQ3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAQ3]")]
		public string VatNumber
		{
			get
			{
				return this.GetField<string> ("[CVAQ3]");
			}
			set
			{
				string oldValue = this.VatNumber;
				if (oldValue != value || !this.IsFieldDefined("[CVAQ3]"))
				{
					this.OnVatNumberChanging (oldValue, value);
					this.SetField<string> ("[CVAQ3]", oldValue, value);
					this.OnVatNumberChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TaxMode</c> field.
		///	designer:fld/CVAP3/CVAR3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAR3]")]
		public global::Epsitec.Cresus.Core.Business.Finance.TaxMode TaxMode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.TaxMode> ("[CVAR3]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.TaxMode oldValue = this.TaxMode;
				if (oldValue != value || !this.IsFieldDefined("[CVAR3]"))
				{
					this.OnTaxModeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.TaxMode> ("[CVAR3]", oldValue, value);
					this.OnTaxModeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>VatDefinitions</c> field.
		///	designer:fld/CVAP3/CVAS3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAS3]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.VatDefinitionEntity> VatDefinitions
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.VatDefinitionEntity> ("[CVAS3]");
			}
		}
		
		partial void OnVatNumberChanging(string oldValue, string newValue);
		partial void OnVatNumberChanged(string oldValue, string newValue);
		partial void OnTaxModeChanging(global::Epsitec.Cresus.Core.Business.Finance.TaxMode oldValue, global::Epsitec.Cresus.Core.Business.Finance.TaxMode newValue);
		partial void OnTaxModeChanged(global::Epsitec.Cresus.Core.Business.Finance.TaxMode oldValue, global::Epsitec.Cresus.Core.Business.Finance.TaxMode newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.TaxSettingsEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.TaxSettingsEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 121);	// [CVAP3]
		public static readonly string EntityStructuredTypeKey = "[CVAP3]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<TaxSettingsEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.PriceRoundingMode Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>PriceRoundingMode</c> entity.
	///	designer:cap/CVA24
	///	</summary>
	public partial class PriceRoundingModeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.IRoundingMode, global::Epsitec.Cresus.Core.Entities.INameDescription
	{
		#region INameDescription Members
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/CVA24/8VA7
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
		#endregion
		#region IRoundingMode Members
		///	<summary>
		///	The <c>Modulo</c> field.
		///	designer:fld/CVA24/CVAN3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAN3]")]
		public global::System.Decimal Modulo
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IRoundingModeInterfaceImplementation.GetModulo (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IRoundingModeInterfaceImplementation.SetModulo (this, value);
			}
		}
		#endregion
		#region INameDescription Members
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/CVA24/8VA8
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
		#region IRoundingMode Members
		///	<summary>
		///	The <c>AddBeforeModulo</c> field.
		///	designer:fld/CVA24/CVAO3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAO3]")]
		public global::System.Decimal AddBeforeModulo
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IRoundingModeInterfaceImplementation.GetAddBeforeModulo (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IRoundingModeInterfaceImplementation.SetAddBeforeModulo (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>RoundingPolicy</c> field.
		///	designer:fld/CVA24/CVA34
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA34]")]
		public global::Epsitec.Cresus.Core.Business.Finance.RoundingPolicy RoundingPolicy
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.RoundingPolicy> ("[CVA34]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.RoundingPolicy oldValue = this.RoundingPolicy;
				if (oldValue != value || !this.IsFieldDefined("[CVA34]"))
				{
					this.OnRoundingPolicyChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.RoundingPolicy> ("[CVA34]", oldValue, value);
					this.OnRoundingPolicyChanged (oldValue, value);
				}
			}
		}
		
		partial void OnRoundingPolicyChanging(global::Epsitec.Cresus.Core.Business.Finance.RoundingPolicy oldValue, global::Epsitec.Cresus.Core.Business.Finance.RoundingPolicy newValue);
		partial void OnRoundingPolicyChanged(global::Epsitec.Cresus.Core.Business.Finance.RoundingPolicy oldValue, global::Epsitec.Cresus.Core.Business.Finance.RoundingPolicy newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 130);	// [CVA24]
		public static readonly string EntityStructuredTypeKey = "[CVA24]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<PriceRoundingModeEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.PriceDiscount Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>PriceDiscount</c> entity.
	///	designer:cap/CVA44
	///	</summary>
	public partial class PriceDiscountEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Text</c> field.
		///	designer:fld/CVA44/CVA54
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA54]")]
		public global::Epsitec.Common.Types.FormattedText Text
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[CVA54]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Text;
				if (oldValue != value || !this.IsFieldDefined("[CVA54]"))
				{
					this.OnTextChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[CVA54]", oldValue, value);
					this.OnTextChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DiscountRate</c> field.
		///	designer:fld/CVA44/CVAQ4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAQ4]")]
		public global::System.Decimal? DiscountRate
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[CVAQ4]");
			}
			set
			{
				global::System.Decimal? oldValue = this.DiscountRate;
				if (oldValue != value || !this.IsFieldDefined("[CVAQ4]"))
				{
					this.OnDiscountRateChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[CVAQ4]", oldValue, value);
					this.OnDiscountRateChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Value</c> field.
		///	designer:fld/CVA44/CVAR4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAR4]")]
		public global::System.Decimal? Value
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[CVAR4]");
			}
			set
			{
				global::System.Decimal? oldValue = this.Value;
				if (oldValue != value || !this.IsFieldDefined("[CVAR4]"))
				{
					this.OnValueChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[CVAR4]", oldValue, value);
					this.OnValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ValueIncludesTaxes</c> field.
		///	designer:fld/CVA44/CVA84
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA84]")]
		public bool ValueIncludesTaxes
		{
			get
			{
				return this.GetField<bool> ("[CVA84]");
			}
			set
			{
				bool oldValue = this.ValueIncludesTaxes;
				if (oldValue != value || !this.IsFieldDefined("[CVA84]"))
				{
					this.OnValueIncludesTaxesChanging (oldValue, value);
					this.SetField<bool> ("[CVA84]", oldValue, value);
					this.OnValueIncludesTaxesChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>RoundingMode</c> field.
		///	designer:fld/CVA44/CVA94
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA94]")]
		public global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity RoundingMode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity> ("[CVA94]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity oldValue = this.RoundingMode;
				if (oldValue != value || !this.IsFieldDefined("[CVA94]"))
				{
					this.OnRoundingModeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity> ("[CVA94]", oldValue, value);
					this.OnRoundingModeChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTextChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnDiscountRateChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnDiscountRateChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnValueChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnValueChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnValueIncludesTaxesChanging(bool oldValue, bool newValue);
		partial void OnValueIncludesTaxesChanged(bool oldValue, bool newValue);
		partial void OnRoundingModeChanging(global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity newValue);
		partial void OnRoundingModeChanged(global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 132);	// [CVA44]
		public static readonly string EntityStructuredTypeKey = "[CVA44]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<PriceDiscountEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.PriceGroup Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>PriceGroup</c> entity.
	///	designer:cap/CVAA4
	///	</summary>
	public partial class PriceGroupEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.INameDescription
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/CVAA4/8VA3
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
		///	designer:fld/CVAA4/8VA7
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
		///	designer:fld/CVAA4/8VA8
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
		///	The <c>NeverApplyDiscount</c> field.
		///	designer:fld/CVAA4/CVAB4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAB4]")]
		public bool NeverApplyDiscount
		{
			get
			{
				return this.GetField<bool> ("[CVAB4]");
			}
			set
			{
				bool oldValue = this.NeverApplyDiscount;
				if (oldValue != value || !this.IsFieldDefined("[CVAB4]"))
				{
					this.OnNeverApplyDiscountChanging (oldValue, value);
					this.SetField<bool> ("[CVAB4]", oldValue, value);
					this.OnNeverApplyDiscountChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultDivideRatio</c> field.
		///	designer:fld/CVAA4/CVAC4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAC4]")]
		public global::System.Decimal? DefaultDivideRatio
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[CVAC4]");
			}
			set
			{
				global::System.Decimal? oldValue = this.DefaultDivideRatio;
				if (oldValue != value || !this.IsFieldDefined("[CVAC4]"))
				{
					this.OnDefaultDivideRatioChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[CVAC4]", oldValue, value);
					this.OnDefaultDivideRatioChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultMultiplyRatio</c> field.
		///	designer:fld/CVAA4/CVAD4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAD4]")]
		public global::System.Decimal? DefaultMultiplyRatio
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[CVAD4]");
			}
			set
			{
				global::System.Decimal? oldValue = this.DefaultMultiplyRatio;
				if (oldValue != value || !this.IsFieldDefined("[CVAD4]"))
				{
					this.OnDefaultMultiplyRatioChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[CVAD4]", oldValue, value);
					this.OnDefaultMultiplyRatioChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultRoundingMode</c> field.
		///	designer:fld/CVAA4/CVAE4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAE4]")]
		public global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity DefaultRoundingMode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity> ("[CVAE4]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity oldValue = this.DefaultRoundingMode;
				if (oldValue != value || !this.IsFieldDefined("[CVAE4]"))
				{
					this.OnDefaultRoundingModeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity> ("[CVAE4]", oldValue, value);
					this.OnDefaultRoundingModeChanged (oldValue, value);
				}
			}
		}
		
		partial void OnNeverApplyDiscountChanging(bool oldValue, bool newValue);
		partial void OnNeverApplyDiscountChanged(bool oldValue, bool newValue);
		partial void OnDefaultDivideRatioChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnDefaultDivideRatioChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnDefaultMultiplyRatioChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnDefaultMultiplyRatioChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnDefaultRoundingModeChanging(global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity newValue);
		partial void OnDefaultRoundingModeChanged(global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.PriceGroupEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.PriceGroupEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 138);	// [CVAA4]
		public static readonly string EntityStructuredTypeKey = "[CVAA4]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<PriceGroupEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.PriceCalculator Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>PriceCalculator</c> entity.
	///	designer:cap/CVAF4
	///	</summary>
	public partial class PriceCalculatorEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/CVAF4/8VA3
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
		///	designer:fld/CVAF4/8VA5
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
		///	designer:fld/CVAF4/8VA7
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
		///	designer:fld/CVAF4/8VA8
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
		///	The <c>SerializedData</c> field.
		///	designer:fld/CVAF4/CVAG4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAG4]")]
		public global::System.Byte[] SerializedData
		{
			get
			{
				return this.GetField<global::System.Byte[]> ("[CVAG4]");
			}
			set
			{
				global::System.Byte[] oldValue = this.SerializedData;
				if (oldValue != value || !this.IsFieldDefined("[CVAG4]"))
				{
					this.OnSerializedDataChanging (oldValue, value);
					this.SetField<global::System.Byte[]> ("[CVAG4]", oldValue, value);
					this.OnSerializedDataChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Informations</c> field.
		///	designer:fld/CVAF4/CVAH4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAH4]")]
		public string Informations
		{
			get
			{
				return this.GetField<string> ("[CVAH4]");
			}
			set
			{
				string oldValue = this.Informations;
				if (oldValue != value || !this.IsFieldDefined("[CVAH4]"))
				{
					this.OnInformationsChanging (oldValue, value);
					this.SetField<string> ("[CVAH4]", oldValue, value);
					this.OnInformationsChanged (oldValue, value);
				}
			}
		}
		
		partial void OnSerializedDataChanging(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		partial void OnSerializedDataChanged(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		partial void OnInformationsChanging(string oldValue, string newValue);
		partial void OnInformationsChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.PriceCalculatorEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.PriceCalculatorEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 143);	// [CVAF4]
		public static readonly string EntityStructuredTypeKey = "[CVAF4]";
	}
}
#endregion

