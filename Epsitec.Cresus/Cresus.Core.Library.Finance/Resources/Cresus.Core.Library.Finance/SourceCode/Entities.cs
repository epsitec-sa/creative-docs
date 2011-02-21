//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[CVA9]", typeof (Epsitec.Cresus.Core.Entities.IsrDefinitionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[CVAL]", typeof (Epsitec.Cresus.Core.Entities.BillingDetailEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[CVAT]", typeof (Epsitec.Cresus.Core.Entities.CurrencyEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[CVAU]", typeof (Epsitec.Cresus.Core.Entities.ExchangeRateSourceEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[CVAV]", typeof (Epsitec.Cresus.Core.Entities.PaymentModeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[CVA21]", typeof (Epsitec.Cresus.Core.Entities.PaymentDetailEntity))]
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
		public global::Epsitec.Common.Types.UnresolvedEnum Currency
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.UnresolvedEnum> ("[CVAB]");
			}
			set
			{
				global::Epsitec.Common.Types.UnresolvedEnum oldValue = this.Currency;
				if (oldValue != value || !this.IsFieldDefined("[CVAB]"))
				{
					this.OnCurrencyChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.UnresolvedEnum> ("[CVAB]", oldValue, value);
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
		
		partial void OnCurrencyChanging(global::Epsitec.Common.Types.UnresolvedEnum oldValue, global::Epsitec.Common.Types.UnresolvedEnum newValue);
		partial void OnCurrencyChanged(global::Epsitec.Common.Types.UnresolvedEnum oldValue, global::Epsitec.Common.Types.UnresolvedEnum newValue);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 9);	// [CVA9]
		public static readonly new string EntityStructuredTypeKey = "[CVA9]";
	}
}
#endregion

#region Epsitec.Cresus.Core.BillingDetail Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>BillingDetail</c> entity.
	///	designer:cap/CVAL
	///	</summary>
	public partial class BillingDetailEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
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
		///	The <c>AmountDue</c> field.
		///	designer:fld/CVAL/CVAN
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAN]")]
		public global::Epsitec.Cresus.Core.Entities.PaymentDetailEntity AmountDue
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PaymentDetailEntity> ("[CVAN]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PaymentDetailEntity oldValue = this.AmountDue;
				if (oldValue != value || !this.IsFieldDefined("[CVAN]"))
				{
					this.OnAmountDueChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PaymentDetailEntity> ("[CVAN]", oldValue, value);
					this.OnAmountDueChanged (oldValue, value);
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
		partial void OnAmountDueChanging(global::Epsitec.Cresus.Core.Entities.PaymentDetailEntity oldValue, global::Epsitec.Cresus.Core.Entities.PaymentDetailEntity newValue);
		partial void OnAmountDueChanged(global::Epsitec.Cresus.Core.Entities.PaymentDetailEntity oldValue, global::Epsitec.Cresus.Core.Entities.PaymentDetailEntity newValue);
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
			return global::Epsitec.Cresus.Core.Entities.BillingDetailEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.BillingDetailEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 21);	// [CVAL]
		public static readonly new string EntityStructuredTypeKey = "[CVAL]";
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
	public partial class CurrencyEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IDateTimeRange
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
		#region IDateTimeRange Members
		///	<summary>
		///	The <c>BeginDate</c> field.
		///	designer:fld/CVAT/8VAK
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
		///	designer:fld/CVAT/8VAL
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
		///	The <c>CurrencyCode</c> field.
		///	designer:fld/CVAT/CVA91
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA91]")]
		public global::Epsitec.Common.Types.UnresolvedEnum CurrencyCode
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.UnresolvedEnum> ("[CVA91]");
			}
			set
			{
				global::Epsitec.Common.Types.UnresolvedEnum oldValue = this.CurrencyCode;
				if (oldValue != value || !this.IsFieldDefined("[CVA91]"))
				{
					this.OnCurrencyCodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.UnresolvedEnum> ("[CVA91]", oldValue, value);
					this.OnCurrencyCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ExchangeRateBase</c> field.
		///	designer:fld/CVAT/CVAA1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAA1]")]
		public global::System.Decimal ExchangeRateBase
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[CVAA1]");
			}
			set
			{
				global::System.Decimal oldValue = this.ExchangeRateBase;
				if (oldValue != value || !this.IsFieldDefined("[CVAA1]"))
				{
					this.OnExchangeRateBaseChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[CVAA1]", oldValue, value);
					this.OnExchangeRateBaseChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ExchangeRate</c> field.
		///	designer:fld/CVAT/CVAB1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVAB1]")]
		public global::System.Decimal ExchangeRate
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[CVAB1]");
			}
			set
			{
				global::System.Decimal oldValue = this.ExchangeRate;
				if (oldValue != value || !this.IsFieldDefined("[CVAB1]"))
				{
					this.OnExchangeRateChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[CVAB1]", oldValue, value);
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
		
		partial void OnCurrencyCodeChanging(global::Epsitec.Common.Types.UnresolvedEnum oldValue, global::Epsitec.Common.Types.UnresolvedEnum newValue);
		partial void OnCurrencyCodeChanged(global::Epsitec.Common.Types.UnresolvedEnum oldValue, global::Epsitec.Common.Types.UnresolvedEnum newValue);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 29);	// [CVAT]
		public static readonly new string EntityStructuredTypeKey = "[CVAT]";
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
		public global::Epsitec.Common.Types.UnresolvedEnum Type
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.UnresolvedEnum> ("[CVAD1]");
			}
			set
			{
				global::Epsitec.Common.Types.UnresolvedEnum oldValue = this.Type;
				if (oldValue != value || !this.IsFieldDefined("[CVAD1]"))
				{
					this.OnTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.UnresolvedEnum> ("[CVAD1]", oldValue, value);
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
		
		partial void OnTypeChanging(global::Epsitec.Common.Types.UnresolvedEnum oldValue, global::Epsitec.Common.Types.UnresolvedEnum newValue);
		partial void OnTypeChanged(global::Epsitec.Common.Types.UnresolvedEnum oldValue, global::Epsitec.Common.Types.UnresolvedEnum newValue);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 30);	// [CVAU]
		public static readonly new string EntityStructuredTypeKey = "[CVAU]";
	}
}
#endregion

#region Epsitec.Cresus.Core.PaymentMode Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>PaymentMode</c> entity.
	///	designer:cap/CVAV
	///	</summary>
	public partial class PaymentModeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory, global::Epsitec.Cresus.Core.Entities.IItemRank
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
			return global::Epsitec.Cresus.Core.Entities.PaymentModeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.PaymentModeEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 31);	// [CVAV]
		public static readonly new string EntityStructuredTypeKey = "[CVAV]";
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
		public global::Epsitec.Common.Types.UnresolvedEnum PaymentType
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.UnresolvedEnum> ("[CVA31]");
			}
			set
			{
				global::Epsitec.Common.Types.UnresolvedEnum oldValue = this.PaymentType;
				if (oldValue != value || !this.IsFieldDefined("[CVA31]"))
				{
					this.OnPaymentTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.UnresolvedEnum> ("[CVA31]", oldValue, value);
					this.OnPaymentTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PaymentMode</c> field.
		///	designer:fld/CVA21/CVA41
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA41]")]
		public global::Epsitec.Cresus.Core.Entities.PaymentModeEntity PaymentMode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PaymentModeEntity> ("[CVA41]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PaymentModeEntity oldValue = this.PaymentMode;
				if (oldValue != value || !this.IsFieldDefined("[CVA41]"))
				{
					this.OnPaymentModeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PaymentModeEntity> ("[CVA41]", oldValue, value);
					this.OnPaymentModeChanged (oldValue, value);
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
		///	designer:fld/CVA21/CVA61
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA61]")]
		public global::System.Decimal Amount
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[CVA61]");
			}
			set
			{
				global::System.Decimal oldValue = this.Amount;
				if (oldValue != value || !this.IsFieldDefined("[CVA61]"))
				{
					this.OnAmountChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[CVA61]", oldValue, value);
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
		
		partial void OnPaymentTypeChanging(global::Epsitec.Common.Types.UnresolvedEnum oldValue, global::Epsitec.Common.Types.UnresolvedEnum newValue);
		partial void OnPaymentTypeChanged(global::Epsitec.Common.Types.UnresolvedEnum oldValue, global::Epsitec.Common.Types.UnresolvedEnum newValue);
		partial void OnPaymentModeChanging(global::Epsitec.Cresus.Core.Entities.PaymentModeEntity oldValue, global::Epsitec.Cresus.Core.Entities.PaymentModeEntity newValue);
		partial void OnPaymentModeChanged(global::Epsitec.Cresus.Core.Entities.PaymentModeEntity oldValue, global::Epsitec.Cresus.Core.Entities.PaymentModeEntity newValue);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 34);	// [CVA21]
		public static readonly new string EntityStructuredTypeKey = "[CVA21]";
	}
}
#endregion

