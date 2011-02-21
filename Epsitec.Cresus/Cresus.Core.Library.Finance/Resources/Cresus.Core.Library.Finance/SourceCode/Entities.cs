//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[CVA9]", typeof (Epsitec.Cresus.Core.Entities.IsrDefinitionEntity))]
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

