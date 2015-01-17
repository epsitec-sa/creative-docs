﻿//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[GVA]", typeof (Epsitec.Cresus.Core.Entities.RelationEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVA1]", typeof (Epsitec.Cresus.Core.Entities.AffairEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAO]", typeof (Epsitec.Cresus.Core.Entities.BusinessSettingsEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAT]", typeof (Epsitec.Cresus.Core.Entities.BusinessDocumentEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVA31]", typeof (Epsitec.Cresus.Core.Entities.AbstractDocumentItemEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAC1]", typeof (Epsitec.Cresus.Core.Entities.ArticleDocumentItemEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAK1]", typeof (Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAL1]", typeof (Epsitec.Cresus.Core.Entities.ArticleTraceabilityDetailEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAP1]", typeof (Epsitec.Cresus.Core.Entities.ArticleQuantityEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAK2]", typeof (Epsitec.Cresus.Core.Entities.AbstractArticleParameterDefinitionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAM2]", typeof (Epsitec.Cresus.Core.Entities.ArticleGroupEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAO2]", typeof (Epsitec.Cresus.Core.Entities.ArticleCategoryEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVA13]", typeof (Epsitec.Cresus.Core.Entities.ArticleAccountingDefinitionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVA83]", typeof (Epsitec.Cresus.Core.Entities.ArticleSupplyEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVA93]", typeof (Epsitec.Cresus.Core.Entities.ArticleStockLocationEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAA3]", typeof (Epsitec.Cresus.Core.Entities.ArticlePriceEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVA94]", typeof (Epsitec.Cresus.Core.Entities.EndTotalDocumentItemEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAF4]", typeof (Epsitec.Cresus.Core.Entities.SubTotalDocumentItemEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAR4]", typeof (Epsitec.Cresus.Core.Entities.TaxDocumentItemEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVA85]", typeof (Epsitec.Cresus.Core.Entities.EnumValueArticleParameterDefinitionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAE5]", typeof (Epsitec.Cresus.Core.Entities.NumericValueArticleParameterDefinitionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAK5]", typeof (Epsitec.Cresus.Core.Entities.OptionValueArticleParameterDefinitionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAM5]", typeof (Epsitec.Cresus.Core.Entities.OptionValueEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAP5]", typeof (Epsitec.Cresus.Core.Entities.FreeTextValueArticleParameterDefinitionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAS5]", typeof (Epsitec.Cresus.Core.Entities.TextDocumentItemEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAV5]", typeof (Epsitec.Cresus.Core.Entities.CustomerEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVA76]", typeof (Epsitec.Cresus.Core.Entities.FinanceSettingsEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAD6]", typeof (Epsitec.Cresus.Core.Entities.PaymentReminderDefinitionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAL6]", typeof (Epsitec.Cresus.Core.Entities.ArticleQuantityColumnEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAN8]", typeof (Epsitec.Cresus.Core.Entities.CustomerCategoryEntity))]
#region Epsitec.Cresus.Core.Relation Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>Relation</c> entity.
	///	designer:cap/GVA
	///	</summary>
	public partial class RelationEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IComments
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/GVA/8VA3
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
		#region IComments Members
		///	<summary>
		///	The <c>Comments</c> field.
		///	designer:fld/GVA/8VAT
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAT]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.CommentEntity> Comments
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.ICommentsInterfaceImplementation.GetComments (this);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Person</c> field.
		///	designer:fld/GVA/GVA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA3]")]
		public global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity Person
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity> ("[GVA3]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity oldValue = this.Person;
				if (oldValue != value || !this.IsFieldDefined("[GVA3]"))
				{
					this.OnPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity> ("[GVA3]", oldValue, value);
					this.OnPersonChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Contacts</c> field.
		///	designer:fld/GVA/GVAC9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAC9]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.ContactPersonEntity> Contacts
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.ContactPersonEntity> ("[GVAC9]");
			}
		}
		///	<summary>
		///	The <c>DefaultMailContact</c> field.
		///	designer:fld/GVA/GVAK7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAK7]")]
		public global::Epsitec.Cresus.Core.Entities.MailContactEntity DefaultMailContact
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.MailContactEntity> ("[GVAK7]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.MailContactEntity oldValue = this.DefaultMailContact;
				if (oldValue != value || !this.IsFieldDefined("[GVAK7]"))
				{
					this.OnDefaultMailContactChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.MailContactEntity> ("[GVAK7]", oldValue, value);
					this.OnDefaultMailContactChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>FirstContactDate</c> field.
		///	designer:fld/GVA/GVAD
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAD]")]
		public global::Epsitec.Common.Types.Date? FirstContactDate
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[GVAD]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.FirstContactDate;
				if (oldValue != value || !this.IsFieldDefined("[GVAD]"))
				{
					this.OnFirstContactDateChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[GVAD]", oldValue, value);
					this.OnFirstContactDateChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>VatNumber</c> field.
		///	designer:fld/GVA/GVAF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAF]")]
		public string VatNumber
		{
			get
			{
				return this.GetField<string> ("[GVAF]");
			}
			set
			{
				string oldValue = this.VatNumber;
				if (oldValue != value || !this.IsFieldDefined("[GVAF]"))
				{
					this.OnVatNumberChanging (oldValue, value);
					this.SetField<string> ("[GVAF]", oldValue, value);
					this.OnVatNumberChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TaxMode</c> field.
		///	designer:fld/GVA/GVAG
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAG]")]
		public global::Epsitec.Cresus.Core.Business.Finance.TaxMode TaxMode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.TaxMode> ("[GVAG]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.TaxMode oldValue = this.TaxMode;
				if (oldValue != value || !this.IsFieldDefined("[GVAG]"))
				{
					this.OnTaxModeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.TaxMode> ("[GVAG]", oldValue, value);
					this.OnTaxModeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultCurrencyCode</c> field.
		///	designer:fld/GVA/GVAL
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAL]")]
		public global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode DefaultCurrencyCode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode> ("[GVAL]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue = this.DefaultCurrencyCode;
				if (oldValue != value || !this.IsFieldDefined("[GVAL]"))
				{
					this.OnDefaultCurrencyCodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode> ("[GVAL]", oldValue, value);
					this.OnDefaultCurrencyCodeChanged (oldValue, value);
				}
			}
		}
		
		partial void OnPersonChanging(global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity newValue);
		partial void OnPersonChanged(global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity newValue);
		partial void OnDefaultMailContactChanging(global::Epsitec.Cresus.Core.Entities.MailContactEntity oldValue, global::Epsitec.Cresus.Core.Entities.MailContactEntity newValue);
		partial void OnDefaultMailContactChanged(global::Epsitec.Cresus.Core.Entities.MailContactEntity oldValue, global::Epsitec.Cresus.Core.Entities.MailContactEntity newValue);
		partial void OnFirstContactDateChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnFirstContactDateChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnVatNumberChanging(string oldValue, string newValue);
		partial void OnVatNumberChanged(string oldValue, string newValue);
		partial void OnTaxModeChanging(global::Epsitec.Cresus.Core.Business.Finance.TaxMode oldValue, global::Epsitec.Cresus.Core.Business.Finance.TaxMode newValue);
		partial void OnTaxModeChanged(global::Epsitec.Cresus.Core.Business.Finance.TaxMode oldValue, global::Epsitec.Cresus.Core.Business.Finance.TaxMode newValue);
		partial void OnDefaultCurrencyCodeChanging(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode newValue);
		partial void OnDefaultCurrencyCodeChanged(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.RelationEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.RelationEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 0);	// [GVA]
		public static readonly string EntityStructuredTypeKey = "[GVA]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<RelationEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.Affair Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>Affair</c> entity.
	///	designer:cap/GVA1
	///	</summary>
	public partial class AffairEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IReferenceNumber, global::Epsitec.Cresus.Core.Entities.IWorkflowHost, global::Epsitec.Cresus.Core.Entities.IBusinessLink, global::Epsitec.Cresus.Core.Entities.INameDescription, global::Epsitec.Cresus.Core.Entities.IComments, global::Epsitec.Cresus.Core.Entities.IItemCode
	{
		#region IReferenceNumber Members
		///	<summary>
		///	The <c>IdA</c> field.
		///	designer:fld/GVA1/8VA11
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA11]")]
		public string IdA
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.GetIdA (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.SetIdA (this, value);
			}
		}
		#endregion
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/GVA1/8VA3
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
		///	designer:fld/GVA1/8VA5
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
		///	designer:fld/GVA1/8VA7
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
		#region IComments Members
		///	<summary>
		///	The <c>Comments</c> field.
		///	designer:fld/GVA1/8VAT
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAT]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.CommentEntity> Comments
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.ICommentsInterfaceImplementation.GetComments (this);
			}
		}
		#endregion
		#region IWorkflowHost Members
		///	<summary>
		///	The <c>Workflow</c> field.
		///	designer:fld/GVA1/DVA31
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVA31]")]
		public global::Epsitec.Cresus.Core.Entities.WorkflowEntity Workflow
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IWorkflowHostInterfaceImplementation.GetWorkflow (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IWorkflowHostInterfaceImplementation.SetWorkflow (this, value);
			}
		}
		#endregion
		#region IBusinessLink Members
		///	<summary>
		///	The <c>BusinessCodeVector</c> field.
		///	designer:fld/GVA1/GVA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA5]")]
		public string BusinessCodeVector
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IBusinessLinkInterfaceImplementation.GetBusinessCodeVector (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IBusinessLinkInterfaceImplementation.SetBusinessCodeVector (this, value);
			}
		}
		#endregion
		#region IReferenceNumber Members
		///	<summary>
		///	The <c>IdB</c> field.
		///	designer:fld/GVA1/8VA21
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA21]")]
		public string IdB
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.GetIdB (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.SetIdB (this, value);
			}
		}
		#endregion
		#region INameDescription Members
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/GVA1/8VA8
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
		#region IReferenceNumber Members
		///	<summary>
		///	The <c>IdC</c> field.
		///	designer:fld/GVA1/8VA31
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA31]")]
		public string IdC
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.GetIdC (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.SetIdC (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Customer</c> field.
		///	designer:fld/GVA1/GVA2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA2]")]
		public global::Epsitec.Cresus.Core.Entities.CustomerEntity Customer
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.CustomerEntity> ("[GVA2]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.CustomerEntity oldValue = this.Customer;
				if (oldValue != value || !this.IsFieldDefined("[GVA2]"))
				{
					this.OnCustomerChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.CustomerEntity> ("[GVA2]", oldValue, value);
					this.OnCustomerChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AssociatedSite</c> field.
		///	designer:fld/GVA1/GVAD9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAD9]")]
		public global::Epsitec.Cresus.Core.Entities.ContactPersonEntity AssociatedSite
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.ContactPersonEntity> ("[GVAD9]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ContactPersonEntity oldValue = this.AssociatedSite;
				if (oldValue != value || !this.IsFieldDefined("[GVAD9]"))
				{
					this.OnAssociatedSiteChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.ContactPersonEntity> ("[GVAD9]", oldValue, value);
					this.OnAssociatedSiteChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AmountDue</c> field.
		///	designer:fld/GVA1/GVAR7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAR7]")]
		public global::System.Decimal? AmountDue
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAR7]");
			}
			set
			{
				global::System.Decimal? oldValue = this.AmountDue;
				if (oldValue != value || !this.IsFieldDefined("[GVAR7]"))
				{
					this.OnAmountDueChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAR7]", oldValue, value);
					this.OnAmountDueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	Mode de facturation (HT ou TTC)
		///	designer:fld/GVA1/GVAT7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAT7]")]
		public global::Epsitec.Cresus.Core.Business.Finance.BillingMode BillingMode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.BillingMode> ("[GVAT7]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.BillingMode oldValue = this.BillingMode;
				if (oldValue != value || !this.IsFieldDefined("[GVAT7]"))
				{
					this.OnBillingModeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.BillingMode> ("[GVAT7]", oldValue, value);
					this.OnBillingModeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CurrencyCode</c> field.
		///	designer:fld/GVA1/GVAP7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAP7]")]
		public global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode CurrencyCode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode> ("[GVAP7]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue = this.CurrencyCode;
				if (oldValue != value || !this.IsFieldDefined("[GVAP7]"))
				{
					this.OnCurrencyCodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode> ("[GVAP7]", oldValue, value);
					this.OnCurrencyCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DebtorBookAccount</c> field.
		///	designer:fld/GVA1/GVA6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA6]")]
		public string DebtorBookAccount
		{
			get
			{
				return this.GetField<string> ("[GVA6]");
			}
			set
			{
				string oldValue = this.DebtorBookAccount;
				if (oldValue != value || !this.IsFieldDefined("[GVA6]"))
				{
					this.OnDebtorBookAccountChanging (oldValue, value);
					this.SetField<string> ("[GVA6]", oldValue, value);
					this.OnDebtorBookAccountChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ActiveSalesRepresentative</c> field.
		///	designer:fld/GVA1/GVA7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA7]")]
		public global::Epsitec.Cresus.Core.Entities.PeopleEntity ActiveSalesRepresentative
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PeopleEntity> ("[GVA7]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PeopleEntity oldValue = this.ActiveSalesRepresentative;
				if (oldValue != value || !this.IsFieldDefined("[GVA7]"))
				{
					this.OnActiveSalesRepresentativeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PeopleEntity> ("[GVA7]", oldValue, value);
					this.OnActiveSalesRepresentativeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ActiveAffairOwner</c> field.
		///	designer:fld/GVA1/GVAM8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAM8]")]
		public global::Epsitec.Cresus.Core.Entities.PeopleEntity ActiveAffairOwner
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PeopleEntity> ("[GVAM8]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PeopleEntity oldValue = this.ActiveAffairOwner;
				if (oldValue != value || !this.IsFieldDefined("[GVAM8]"))
				{
					this.OnActiveAffairOwnerChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PeopleEntity> ("[GVAM8]", oldValue, value);
					this.OnActiveAffairOwnerChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SubAffairs</c> field.
		///	designer:fld/GVA1/GVA9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA9]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.AffairEntity> SubAffairs
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.AffairEntity> ("[GVA9]");
			}
		}
		///	<summary>
		///	The <c>Documents</c> field.
		///	designer:fld/GVA1/GVAA
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAA]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.DocumentMetadataEntity> Documents
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.DocumentMetadataEntity> ("[GVAA]");
			}
		}
		///	<summary>
		///	The <c>UnassignedPaymentTransactions</c> field.
		///	designer:fld/GVA1/GVAJ7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAJ7]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.PaymentTransactionEntity> UnassignedPaymentTransactions
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.PaymentTransactionEntity> ("[GVAJ7]");
			}
		}
		
		partial void OnCustomerChanging(global::Epsitec.Cresus.Core.Entities.CustomerEntity oldValue, global::Epsitec.Cresus.Core.Entities.CustomerEntity newValue);
		partial void OnCustomerChanged(global::Epsitec.Cresus.Core.Entities.CustomerEntity oldValue, global::Epsitec.Cresus.Core.Entities.CustomerEntity newValue);
		partial void OnAssociatedSiteChanging(global::Epsitec.Cresus.Core.Entities.ContactPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.ContactPersonEntity newValue);
		partial void OnAssociatedSiteChanged(global::Epsitec.Cresus.Core.Entities.ContactPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.ContactPersonEntity newValue);
		partial void OnAmountDueChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnAmountDueChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnBillingModeChanging(global::Epsitec.Cresus.Core.Business.Finance.BillingMode oldValue, global::Epsitec.Cresus.Core.Business.Finance.BillingMode newValue);
		partial void OnBillingModeChanged(global::Epsitec.Cresus.Core.Business.Finance.BillingMode oldValue, global::Epsitec.Cresus.Core.Business.Finance.BillingMode newValue);
		partial void OnCurrencyCodeChanging(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode newValue);
		partial void OnCurrencyCodeChanged(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode newValue);
		partial void OnDebtorBookAccountChanging(string oldValue, string newValue);
		partial void OnDebtorBookAccountChanged(string oldValue, string newValue);
		partial void OnActiveSalesRepresentativeChanging(global::Epsitec.Cresus.Core.Entities.PeopleEntity oldValue, global::Epsitec.Cresus.Core.Entities.PeopleEntity newValue);
		partial void OnActiveSalesRepresentativeChanged(global::Epsitec.Cresus.Core.Entities.PeopleEntity oldValue, global::Epsitec.Cresus.Core.Entities.PeopleEntity newValue);
		partial void OnActiveAffairOwnerChanging(global::Epsitec.Cresus.Core.Entities.PeopleEntity oldValue, global::Epsitec.Cresus.Core.Entities.PeopleEntity newValue);
		partial void OnActiveAffairOwnerChanged(global::Epsitec.Cresus.Core.Entities.PeopleEntity oldValue, global::Epsitec.Cresus.Core.Entities.PeopleEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.AffairEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.AffairEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 1);	// [GVA1]
		public static readonly string EntityStructuredTypeKey = "[GVA1]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AffairEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.IBusinessLink Interface
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>IBusinessLink</c> entity.
	///	designer:cap/GVA4
	///	</summary>
	public interface IBusinessLink
	{
		///	<summary>
		///	The <c>BusinessCodeVector</c> field.
		///	designer:fld/GVA4/GVA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA5]")]
		string BusinessCodeVector
		{
			get;
			set;
		}
	}
	public static partial class IBusinessLinkInterfaceImplementation
	{
		public static string GetBusinessCodeVector(global::Epsitec.Cresus.Core.Entities.IBusinessLink obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[GVA5]");
		}
		public static void SetBusinessCodeVector(global::Epsitec.Cresus.Core.Entities.IBusinessLink obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.BusinessCodeVector;
			if (oldValue != value || !entity.IsFieldDefined("[GVA5]"))
			{
				IBusinessLinkInterfaceImplementation.OnBusinessCodeVectorChanging (obj, oldValue, value);
				entity.SetField<string> ("[GVA5]", oldValue, value);
				IBusinessLinkInterfaceImplementation.OnBusinessCodeVectorChanged (obj, oldValue, value);
			}
		}
		static partial void OnBusinessCodeVectorChanged(global::Epsitec.Cresus.Core.Entities.IBusinessLink obj, string oldValue, string newValue);
		static partial void OnBusinessCodeVectorChanging(global::Epsitec.Cresus.Core.Entities.IBusinessLink obj, string oldValue, string newValue);
	}
}
#endregion

#region Epsitec.Cresus.Core.BusinessSettings Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>BusinessSettings</c> entity.
	///	designer:cap/GVAO
	///	</summary>
	public partial class BusinessSettingsEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.IItemCode
	{
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/GVAO/8VA5
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
		///	The <c>Company</c> field.
		///	designer:fld/GVAO/GVAP
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAP]")]
		public global::Epsitec.Cresus.Core.Entities.RelationEntity Company
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.RelationEntity> ("[GVAP]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue = this.Company;
				if (oldValue != value || !this.IsFieldDefined("[GVAP]"))
				{
					this.OnCompanyChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.RelationEntity> ("[GVAP]", oldValue, value);
					this.OnCompanyChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CompanyLogo</c> field.
		///	designer:fld/GVAO/GVAS
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAS]")]
		public global::Epsitec.Cresus.Core.Entities.ImageEntity CompanyLogo
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.ImageEntity> ("[GVAS]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ImageEntity oldValue = this.CompanyLogo;
				if (oldValue != value || !this.IsFieldDefined("[GVAS]"))
				{
					this.OnCompanyLogoChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.ImageEntity> ("[GVAS]", oldValue, value);
					this.OnCompanyLogoChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Finance</c> field.
		///	designer:fld/GVAO/GVA86
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA86]")]
		public global::Epsitec.Cresus.Core.Entities.FinanceSettingsEntity Finance
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.FinanceSettingsEntity> ("[GVA86]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.FinanceSettingsEntity oldValue = this.Finance;
				if (oldValue != value || !this.IsFieldDefined("[GVA86]"))
				{
					this.OnFinanceChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.FinanceSettingsEntity> ("[GVA86]", oldValue, value);
					this.OnFinanceChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>UnassignedPaymentTransactions</c> field.
		///	designer:fld/GVAO/GVAN7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAN7]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.PaymentTransactionEntity> UnassignedPaymentTransactions
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.PaymentTransactionEntity> ("[GVAN7]");
			}
		}
		///	<summary>
		///	The <c>Generators</c> field.
		///	designer:fld/GVAO/GVAI6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAI6]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.GeneratorDefinitionEntity> Generators
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.GeneratorDefinitionEntity> ("[GVAI6]");
			}
		}
		///	<summary>
		///	The <c>Tax</c> field.
		///	designer:fld/GVAO/GVAR
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAR]")]
		public global::Epsitec.Cresus.Core.Entities.TaxSettingsEntity Tax
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.TaxSettingsEntity> ("[GVAR]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.TaxSettingsEntity oldValue = this.Tax;
				if (oldValue != value || !this.IsFieldDefined("[GVAR]"))
				{
					this.OnTaxChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.TaxSettingsEntity> ("[GVAR]", oldValue, value);
					this.OnTaxChanged (oldValue, value);
				}
			}
		}
		
		partial void OnCompanyChanging(global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue, global::Epsitec.Cresus.Core.Entities.RelationEntity newValue);
		partial void OnCompanyChanged(global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue, global::Epsitec.Cresus.Core.Entities.RelationEntity newValue);
		partial void OnCompanyLogoChanging(global::Epsitec.Cresus.Core.Entities.ImageEntity oldValue, global::Epsitec.Cresus.Core.Entities.ImageEntity newValue);
		partial void OnCompanyLogoChanged(global::Epsitec.Cresus.Core.Entities.ImageEntity oldValue, global::Epsitec.Cresus.Core.Entities.ImageEntity newValue);
		partial void OnFinanceChanging(global::Epsitec.Cresus.Core.Entities.FinanceSettingsEntity oldValue, global::Epsitec.Cresus.Core.Entities.FinanceSettingsEntity newValue);
		partial void OnFinanceChanged(global::Epsitec.Cresus.Core.Entities.FinanceSettingsEntity oldValue, global::Epsitec.Cresus.Core.Entities.FinanceSettingsEntity newValue);
		partial void OnTaxChanging(global::Epsitec.Cresus.Core.Entities.TaxSettingsEntity oldValue, global::Epsitec.Cresus.Core.Entities.TaxSettingsEntity newValue);
		partial void OnTaxChanged(global::Epsitec.Cresus.Core.Entities.TaxSettingsEntity oldValue, global::Epsitec.Cresus.Core.Entities.TaxSettingsEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.BusinessSettingsEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.BusinessSettingsEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 24);	// [GVAO]
		public static readonly string EntityStructuredTypeKey = "[GVAO]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<BusinessSettingsEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.BusinessDocument Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>BusinessDocument</c> entity.
	///	designer:cap/GVAT
	///	</summary>
	public partial class BusinessDocumentEntity : global::Epsitec.Cresus.Core.Entities.AbstractDocumentEntity
	{
		///	<summary>
		///	The <c>BaseDocumentCode</c> field.
		///	designer:fld/GVAT/GVAK6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAK6]")]
		public string BaseDocumentCode
		{
			get
			{
				return this.GetField<string> ("[GVAK6]");
			}
			set
			{
				string oldValue = this.BaseDocumentCode;
				if (oldValue != value || !this.IsFieldDefined("[GVAK6]"))
				{
					this.OnBaseDocumentCodeChanging (oldValue, value);
					this.SetField<string> ("[GVAK6]", oldValue, value);
					this.OnBaseDocumentCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>BillToMailContact</c> field.
		///	designer:fld/GVAT/GVAU
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAU]")]
		public global::Epsitec.Cresus.Core.Entities.MailContactEntity BillToMailContact
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.MailContactEntity> ("[GVAU]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.MailContactEntity oldValue = this.BillToMailContact;
				if (oldValue != value || !this.IsFieldDefined("[GVAU]"))
				{
					this.OnBillToMailContactChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.MailContactEntity> ("[GVAU]", oldValue, value);
					this.OnBillToMailContactChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ShipToMailContact</c> field.
		///	designer:fld/GVAT/GVAV
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAV]")]
		public global::Epsitec.Cresus.Core.Entities.MailContactEntity ShipToMailContact
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.MailContactEntity> ("[GVAV]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.MailContactEntity oldValue = this.ShipToMailContact;
				if (oldValue != value || !this.IsFieldDefined("[GVAV]"))
				{
					this.OnShipToMailContactChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.MailContactEntity> ("[GVAV]", oldValue, value);
					this.OnShipToMailContactChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>OtherPartyRelation</c> field.
		///	designer:fld/GVAT/GVA01
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA01]")]
		public global::Epsitec.Cresus.Core.Entities.RelationEntity OtherPartyRelation
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.RelationEntity> ("[GVA01]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue = this.OtherPartyRelation;
				if (oldValue != value || !this.IsFieldDefined("[GVA01]"))
				{
					this.OnOtherPartyRelationChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.RelationEntity> ("[GVA01]", oldValue, value);
					this.OnOtherPartyRelationChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>OtherPartyBillingMode</c> field.
		///	designer:fld/GVAT/GVA11
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA11]")]
		public global::Epsitec.Cresus.Core.Business.Finance.BillingMode OtherPartyBillingMode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.BillingMode> ("[GVA11]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.BillingMode oldValue = this.OtherPartyBillingMode;
				if (oldValue != value || !this.IsFieldDefined("[GVA11]"))
				{
					this.OnOtherPartyBillingModeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.BillingMode> ("[GVA11]", oldValue, value);
					this.OnOtherPartyBillingModeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>OtherPartyTaxMode</c> field.
		///	designer:fld/GVAT/GVA21
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA21]")]
		public global::Epsitec.Cresus.Core.Business.Finance.TaxMode OtherPartyTaxMode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.TaxMode> ("[GVA21]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.TaxMode oldValue = this.OtherPartyTaxMode;
				if (oldValue != value || !this.IsFieldDefined("[GVA21]"))
				{
					this.OnOtherPartyTaxModeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.TaxMode> ("[GVA21]", oldValue, value);
					this.OnOtherPartyTaxModeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Lines</c> field.
		///	designer:fld/GVAT/GVA41
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA41]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.AbstractDocumentItemEntity> Lines
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.AbstractDocumentItemEntity> ("[GVA41]");
			}
		}
		///	<summary>
		///	The <c>PaymentTransactions</c> field.
		///	designer:fld/GVAT/GVA51
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA51]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.PaymentTransactionEntity> PaymentTransactions
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.PaymentTransactionEntity> ("[GVA51]");
			}
		}
		///	<summary>
		///	The <c>BillingStatus</c> field.
		///	designer:fld/GVAT/GVA61
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA61]")]
		public global::Epsitec.Cresus.Core.Business.Finance.BillingStatus BillingStatus
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.BillingStatus> ("[GVA61]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.BillingStatus oldValue = this.BillingStatus;
				if (oldValue != value || !this.IsFieldDefined("[GVA61]"))
				{
					this.OnBillingStatusChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.BillingStatus> ("[GVA61]", oldValue, value);
					this.OnBillingStatusChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>BillingDate</c> field.
		///	designer:fld/GVAT/GVA71
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA71]")]
		public global::Epsitec.Common.Types.Date? BillingDate
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[GVA71]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.BillingDate;
				if (oldValue != value || !this.IsFieldDefined("[GVA71]"))
				{
					this.OnBillingDateChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[GVA71]", oldValue, value);
					this.OnBillingDateChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CurrencyCode</c> field.
		///	designer:fld/GVAT/GVA81
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA81]")]
		public global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode CurrencyCode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode> ("[GVA81]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue = this.CurrencyCode;
				if (oldValue != value || !this.IsFieldDefined("[GVA81]"))
				{
					this.OnCurrencyCodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode> ("[GVA81]", oldValue, value);
					this.OnCurrencyCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PriceRefDate</c> field.
		///	designer:fld/GVAT/GVA91
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA91]")]
		public global::Epsitec.Common.Types.Date PriceRefDate
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date> ("[GVA91]");
			}
			set
			{
				global::Epsitec.Common.Types.Date oldValue = this.PriceRefDate;
				if (oldValue != value || !this.IsFieldDefined("[GVA91]"))
				{
					this.OnPriceRefDateChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date> ("[GVA91]", oldValue, value);
					this.OnPriceRefDateChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PriceGroup</c> field.
		///	designer:fld/GVAT/GVAA1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAA1]")]
		public global::Epsitec.Cresus.Core.Entities.PriceGroupEntity PriceGroup
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PriceGroupEntity> ("[GVAA1]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PriceGroupEntity oldValue = this.PriceGroup;
				if (oldValue != value || !this.IsFieldDefined("[GVAA1]"))
				{
					this.OnPriceGroupChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PriceGroupEntity> ("[GVAA1]", oldValue, value);
					this.OnPriceGroupChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DebtorBookAccount</c> field.
		///	designer:fld/GVAT/GVAB1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAB1]")]
		public string DebtorBookAccount
		{
			get
			{
				return this.GetField<string> ("[GVAB1]");
			}
			set
			{
				string oldValue = this.DebtorBookAccount;
				if (oldValue != value || !this.IsFieldDefined("[GVAB1]"))
				{
					this.OnDebtorBookAccountChanging (oldValue, value);
					this.SetField<string> ("[GVAB1]", oldValue, value);
					this.OnDebtorBookAccountChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>FooterText</c> field.
		///	designer:fld/GVAT/GVAB9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAB9]")]
		public global::Epsitec.Common.Types.FormattedText FooterText
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAB9]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.FooterText;
				if (oldValue != value || !this.IsFieldDefined("[GVAB9]"))
				{
					this.OnFooterTextChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAB9]", oldValue, value);
					this.OnFooterTextChanged (oldValue, value);
				}
			}
		}
		
		partial void OnBaseDocumentCodeChanging(string oldValue, string newValue);
		partial void OnBaseDocumentCodeChanged(string oldValue, string newValue);
		partial void OnBillToMailContactChanging(global::Epsitec.Cresus.Core.Entities.MailContactEntity oldValue, global::Epsitec.Cresus.Core.Entities.MailContactEntity newValue);
		partial void OnBillToMailContactChanged(global::Epsitec.Cresus.Core.Entities.MailContactEntity oldValue, global::Epsitec.Cresus.Core.Entities.MailContactEntity newValue);
		partial void OnShipToMailContactChanging(global::Epsitec.Cresus.Core.Entities.MailContactEntity oldValue, global::Epsitec.Cresus.Core.Entities.MailContactEntity newValue);
		partial void OnShipToMailContactChanged(global::Epsitec.Cresus.Core.Entities.MailContactEntity oldValue, global::Epsitec.Cresus.Core.Entities.MailContactEntity newValue);
		partial void OnOtherPartyRelationChanging(global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue, global::Epsitec.Cresus.Core.Entities.RelationEntity newValue);
		partial void OnOtherPartyRelationChanged(global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue, global::Epsitec.Cresus.Core.Entities.RelationEntity newValue);
		partial void OnOtherPartyBillingModeChanging(global::Epsitec.Cresus.Core.Business.Finance.BillingMode oldValue, global::Epsitec.Cresus.Core.Business.Finance.BillingMode newValue);
		partial void OnOtherPartyBillingModeChanged(global::Epsitec.Cresus.Core.Business.Finance.BillingMode oldValue, global::Epsitec.Cresus.Core.Business.Finance.BillingMode newValue);
		partial void OnOtherPartyTaxModeChanging(global::Epsitec.Cresus.Core.Business.Finance.TaxMode oldValue, global::Epsitec.Cresus.Core.Business.Finance.TaxMode newValue);
		partial void OnOtherPartyTaxModeChanged(global::Epsitec.Cresus.Core.Business.Finance.TaxMode oldValue, global::Epsitec.Cresus.Core.Business.Finance.TaxMode newValue);
		partial void OnBillingStatusChanging(global::Epsitec.Cresus.Core.Business.Finance.BillingStatus oldValue, global::Epsitec.Cresus.Core.Business.Finance.BillingStatus newValue);
		partial void OnBillingStatusChanged(global::Epsitec.Cresus.Core.Business.Finance.BillingStatus oldValue, global::Epsitec.Cresus.Core.Business.Finance.BillingStatus newValue);
		partial void OnBillingDateChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnBillingDateChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnCurrencyCodeChanging(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode newValue);
		partial void OnCurrencyCodeChanged(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode newValue);
		partial void OnPriceRefDateChanging(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnPriceRefDateChanged(global::Epsitec.Common.Types.Date oldValue, global::Epsitec.Common.Types.Date newValue);
		partial void OnPriceGroupChanging(global::Epsitec.Cresus.Core.Entities.PriceGroupEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceGroupEntity newValue);
		partial void OnPriceGroupChanged(global::Epsitec.Cresus.Core.Entities.PriceGroupEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceGroupEntity newValue);
		partial void OnDebtorBookAccountChanging(string oldValue, string newValue);
		partial void OnDebtorBookAccountChanged(string oldValue, string newValue);
		partial void OnFooterTextChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnFooterTextChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.BusinessDocumentEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.BusinessDocumentEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 29);	// [GVAT]
		public static readonly new string EntityStructuredTypeKey = "[GVAT]";
		
		#region Repository Class
		public new partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<BusinessDocumentEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.AbstractDocumentItem Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>AbstractDocumentItem</c> entity.
	///	designer:cap/GVA31
	///	</summary>
	public partial class AbstractDocumentItemEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Attributes</c> field.
		///	designer:fld/GVA31/GVA27
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA27]")]
		public global::Epsitec.Cresus.Core.Business.DocumentItemAttributes Attributes
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.DocumentItemAttributes> ("[GVA27]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.DocumentItemAttributes oldValue = this.Attributes;
				if (oldValue != value || !this.IsFieldDefined("[GVA27]"))
				{
					this.OnAttributesChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.DocumentItemAttributes> ("[GVA27]", oldValue, value);
					this.OnAttributesChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>GroupIndex</c> field.
		///	designer:fld/GVA31/GVAG1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAG1]")]
		public int GroupIndex
		{
			get
			{
				return this.GetField<int> ("[GVAG1]");
			}
			set
			{
				int oldValue = this.GroupIndex;
				if (oldValue != value || !this.IsFieldDefined("[GVAG1]"))
				{
					this.OnGroupIndexChanging (oldValue, value);
					this.SetField<int> ("[GVAG1]", oldValue, value);
					this.OnGroupIndexChanged (oldValue, value);
				}
			}
		}
		
		partial void OnAttributesChanging(global::Epsitec.Cresus.Core.Business.DocumentItemAttributes oldValue, global::Epsitec.Cresus.Core.Business.DocumentItemAttributes newValue);
		partial void OnAttributesChanged(global::Epsitec.Cresus.Core.Business.DocumentItemAttributes oldValue, global::Epsitec.Cresus.Core.Business.DocumentItemAttributes newValue);
		partial void OnGroupIndexChanging(int oldValue, int newValue);
		partial void OnGroupIndexChanged(int oldValue, int newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.AbstractDocumentItemEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.AbstractDocumentItemEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 35);	// [GVA31]
		public static readonly string EntityStructuredTypeKey = "[GVA31]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AbstractDocumentItemEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.ArticleDocumentItem Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ArticleDocumentItem</c> entity.
	///	designer:cap/GVAC1
	///	</summary>
	public partial class ArticleDocumentItemEntity : global::Epsitec.Cresus.Core.Entities.AbstractDocumentItemEntity, global::Epsitec.Cresus.Core.Entities.IDateRange, global::Epsitec.Cresus.Core.Entities.IArticleDefinitionParameters
	{
		#region IDateRange Members
		///	<summary>
		///	The <c>BeginDate</c> field.
		///	designer:fld/GVAC1/8VAO
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
		#region IArticleDefinitionParameters Members
		///	<summary>
		///	The <c>ArticleDefinition</c> field.
		///	designer:fld/GVAC1/GVAI1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAI1]")]
		public global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity ArticleDefinition
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IArticleDefinitionParametersInterfaceImplementation.GetArticleDefinition (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IArticleDefinitionParametersInterfaceImplementation.SetArticleDefinition (this, value);
			}
		}
		#endregion
		#region IDateRange Members
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/GVAC1/8VAP
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
		#region IArticleDefinitionParameters Members
		///	<summary>
		///	The <c>ArticleParameters</c> field.
		///	designer:fld/GVAC1/GVAJ1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAJ1]")]
		public string ArticleParameters
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IArticleDefinitionParametersInterfaceImplementation.GetArticleParameters (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IArticleDefinitionParametersInterfaceImplementation.SetArticleParameters (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>ArticleAttributes</c> field.
		///	designer:fld/GVAC1/GVAC7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAC7]")]
		public global::Epsitec.Cresus.Core.Business.ArticleDocumentItemAttributes ArticleAttributes
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.ArticleDocumentItemAttributes> ("[GVAC7]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.ArticleDocumentItemAttributes oldValue = this.ArticleAttributes;
				if (oldValue != value || !this.IsFieldDefined("[GVAC7]"))
				{
					this.OnArticleAttributesChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.ArticleDocumentItemAttributes> ("[GVAC7]", oldValue, value);
					this.OnArticleAttributesChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ArticleAccountingDefinition</c> field.
		///	designer:fld/GVAC1/GVA02
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA02]")]
		public global::Epsitec.Cresus.Core.Entities.ArticleAccountingDefinitionEntity ArticleAccountingDefinition
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.ArticleAccountingDefinitionEntity> ("[GVA02]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ArticleAccountingDefinitionEntity oldValue = this.ArticleAccountingDefinition;
				if (oldValue != value || !this.IsFieldDefined("[GVA02]"))
				{
					this.OnArticleAccountingDefinitionChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.ArticleAccountingDefinitionEntity> ("[GVA02]", oldValue, value);
					this.OnArticleAccountingDefinitionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ArticleTraceabilityDetails</c> field.
		///	designer:fld/GVAC1/GVAO1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAO1]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.ArticleTraceabilityDetailEntity> ArticleTraceabilityDetails
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.ArticleTraceabilityDetailEntity> ("[GVAO1]");
			}
		}
		///	<summary>
		///	The <c>ArticleQuantities</c> field.
		///	designer:fld/GVAC1/GVAV1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAV1]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.ArticleQuantityEntity> ArticleQuantities
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.ArticleQuantityEntity> ("[GVAV1]");
			}
		}
		///	<summary>
		///	The <c>Discounts</c> field.
		///	designer:fld/GVAC1/GVA52
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA52]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity> Discounts
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity> ("[GVA52]");
			}
		}
		///	<summary>
		///	The <c>VatRateA</c> field.
		///	designer:fld/GVAC1/GVA62
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA62]")]
		public global::System.Decimal VatRateA
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[GVA62]");
			}
			set
			{
				global::System.Decimal oldValue = this.VatRateA;
				if (oldValue != value || !this.IsFieldDefined("[GVA62]"))
				{
					this.OnVatRateAChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[GVA62]", oldValue, value);
					this.OnVatRateAChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>VatRateB</c> field.
		///	designer:fld/GVAC1/GVA72
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA72]")]
		public global::System.Decimal VatRateB
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[GVA72]");
			}
			set
			{
				global::System.Decimal oldValue = this.VatRateB;
				if (oldValue != value || !this.IsFieldDefined("[GVA72]"))
				{
					this.OnVatRateBChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[GVA72]", oldValue, value);
					this.OnVatRateBChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>VatRatio</c> field.
		///	designer:fld/GVAC1/GVAU7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAU7]")]
		public global::System.Decimal VatRatio
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[GVAU7]");
			}
			set
			{
				global::System.Decimal oldValue = this.VatRatio;
				if (oldValue != value || !this.IsFieldDefined("[GVAU7]"))
				{
					this.OnVatRatioChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[GVAU7]", oldValue, value);
					this.OnVatRatioChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>UnitPriceBeforeTax1</c> field.
		///	designer:fld/GVAC1/GVAS6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAS6]")]
		public global::System.Decimal? UnitPriceBeforeTax1
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAS6]");
			}
			set
			{
				global::System.Decimal? oldValue = this.UnitPriceBeforeTax1;
				if (oldValue != value || !this.IsFieldDefined("[GVAS6]"))
				{
					this.OnUnitPriceBeforeTax1Changing (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAS6]", oldValue, value);
					this.OnUnitPriceBeforeTax1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>UnitPriceBeforeTax2</c> field.
		///	designer:fld/GVAC1/GVAQ6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAQ6]")]
		public global::System.Decimal? UnitPriceBeforeTax2
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAQ6]");
			}
			set
			{
				global::System.Decimal? oldValue = this.UnitPriceBeforeTax2;
				if (oldValue != value || !this.IsFieldDefined("[GVAQ6]"))
				{
					this.OnUnitPriceBeforeTax2Changing (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAQ6]", oldValue, value);
					this.OnUnitPriceBeforeTax2Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>UnitPriceAfterTax1</c> field.
		///	designer:fld/GVAC1/GVAI7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAI7]")]
		public global::System.Decimal? UnitPriceAfterTax1
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAI7]");
			}
			set
			{
				global::System.Decimal? oldValue = this.UnitPriceAfterTax1;
				if (oldValue != value || !this.IsFieldDefined("[GVAI7]"))
				{
					this.OnUnitPriceAfterTax1Changing (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAI7]", oldValue, value);
					this.OnUnitPriceAfterTax1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>UnitPriceAfterTax2</c> field.
		///	designer:fld/GVAC1/GVAR6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAR6]")]
		public global::System.Decimal? UnitPriceAfterTax2
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAR6]");
			}
			set
			{
				global::System.Decimal? oldValue = this.UnitPriceAfterTax2;
				if (oldValue != value || !this.IsFieldDefined("[GVAR6]"))
				{
					this.OnUnitPriceAfterTax2Changing (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAR6]", oldValue, value);
					this.OnUnitPriceAfterTax2Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LinePriceBeforeTax1</c> field.
		///	designer:fld/GVAC1/GVA32
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA32]")]
		public global::System.Decimal? LinePriceBeforeTax1
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVA32]");
			}
			set
			{
				global::System.Decimal? oldValue = this.LinePriceBeforeTax1;
				if (oldValue != value || !this.IsFieldDefined("[GVA32]"))
				{
					this.OnLinePriceBeforeTax1Changing (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVA32]", oldValue, value);
					this.OnLinePriceBeforeTax1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LinePriceBeforeTax2</c> field.
		///	designer:fld/GVAC1/GVA82
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA82]")]
		public global::System.Decimal? LinePriceBeforeTax2
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVA82]");
			}
			set
			{
				global::System.Decimal? oldValue = this.LinePriceBeforeTax2;
				if (oldValue != value || !this.IsFieldDefined("[GVA82]"))
				{
					this.OnLinePriceBeforeTax2Changing (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVA82]", oldValue, value);
					this.OnLinePriceBeforeTax2Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LinePriceAfterTax1</c> field.
		///	designer:fld/GVAC1/GVAA2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAA2]")]
		public global::System.Decimal? LinePriceAfterTax1
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAA2]");
			}
			set
			{
				global::System.Decimal? oldValue = this.LinePriceAfterTax1;
				if (oldValue != value || !this.IsFieldDefined("[GVAA2]"))
				{
					this.OnLinePriceAfterTax1Changing (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAA2]", oldValue, value);
					this.OnLinePriceAfterTax1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LinePriceAfterTax2</c> field.
		///	designer:fld/GVAC1/GVAB2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAB2]")]
		public global::System.Decimal? LinePriceAfterTax2
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAB2]");
			}
			set
			{
				global::System.Decimal? oldValue = this.LinePriceAfterTax2;
				if (oldValue != value || !this.IsFieldDefined("[GVAB2]"))
				{
					this.OnLinePriceAfterTax2Changing (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAB2]", oldValue, value);
					this.OnLinePriceAfterTax2Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TotalRevenueAfterTax</c> field.
		///	designer:fld/GVAC1/GVAC2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAC2]")]
		public global::System.Decimal? TotalRevenueAfterTax
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAC2]");
			}
			set
			{
				global::System.Decimal? oldValue = this.TotalRevenueAfterTax;
				if (oldValue != value || !this.IsFieldDefined("[GVAC2]"))
				{
					this.OnTotalRevenueAfterTaxChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAC2]", oldValue, value);
					this.OnTotalRevenueAfterTaxChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TotalRevenueAccounted</c> field.
		///	designer:fld/GVAC1/GVA38
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA38]")]
		public global::System.Decimal? TotalRevenueAccounted
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVA38]");
			}
			set
			{
				global::System.Decimal? oldValue = this.TotalRevenueAccounted;
				if (oldValue != value || !this.IsFieldDefined("[GVA38]"))
				{
					this.OnTotalRevenueAccountedChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVA38]", oldValue, value);
					this.OnTotalRevenueAccountedChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ArticleNameCache</c> field.
		///	designer:fld/GVAC1/GVAE2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAE2]")]
		public global::Epsitec.Common.Types.FormattedText ArticleNameCache
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAE2]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.ArticleNameCache;
				if (oldValue != value || !this.IsFieldDefined("[GVAE2]"))
				{
					this.OnArticleNameCacheChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAE2]", oldValue, value);
					this.OnArticleNameCacheChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ArticleDescriptionCache</c> field.
		///	designer:fld/GVAC1/GVAF2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAF2]")]
		public global::Epsitec.Common.Types.FormattedText ArticleDescriptionCache
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAF2]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.ArticleDescriptionCache;
				if (oldValue != value || !this.IsFieldDefined("[GVAF2]"))
				{
					this.OnArticleDescriptionCacheChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAF2]", oldValue, value);
					this.OnArticleDescriptionCacheChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ReplacementName</c> field.
		///	designer:fld/GVAC1/GVKN
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVKN]")]
		public global::Epsitec.Common.Types.FormattedText ReplacementName
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVKN]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.ReplacementName;
				if (oldValue != value || !this.IsFieldDefined("[GVKN]"))
				{
					this.OnReplacementNameChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVKN]", oldValue, value);
					this.OnReplacementNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ReplacementDescription</c> field.
		///	designer:fld/GVAC1/GVAG2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAG2]")]
		public global::Epsitec.Common.Types.FormattedText ReplacementDescription
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAG2]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.ReplacementDescription;
				if (oldValue != value || !this.IsFieldDefined("[GVAG2]"))
				{
					this.OnReplacementDescriptionChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAG2]", oldValue, value);
					this.OnReplacementDescriptionChanged (oldValue, value);
				}
			}
		}
		
		partial void OnArticleAttributesChanging(global::Epsitec.Cresus.Core.Business.ArticleDocumentItemAttributes oldValue, global::Epsitec.Cresus.Core.Business.ArticleDocumentItemAttributes newValue);
		partial void OnArticleAttributesChanged(global::Epsitec.Cresus.Core.Business.ArticleDocumentItemAttributes oldValue, global::Epsitec.Cresus.Core.Business.ArticleDocumentItemAttributes newValue);
		partial void OnArticleAccountingDefinitionChanging(global::Epsitec.Cresus.Core.Entities.ArticleAccountingDefinitionEntity oldValue, global::Epsitec.Cresus.Core.Entities.ArticleAccountingDefinitionEntity newValue);
		partial void OnArticleAccountingDefinitionChanged(global::Epsitec.Cresus.Core.Entities.ArticleAccountingDefinitionEntity oldValue, global::Epsitec.Cresus.Core.Entities.ArticleAccountingDefinitionEntity newValue);
		partial void OnVatRateAChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnVatRateAChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnVatRateBChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnVatRateBChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnVatRatioChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnVatRatioChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnUnitPriceBeforeTax1Changing(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnUnitPriceBeforeTax1Changed(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnUnitPriceBeforeTax2Changing(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnUnitPriceBeforeTax2Changed(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnUnitPriceAfterTax1Changing(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnUnitPriceAfterTax1Changed(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnUnitPriceAfterTax2Changing(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnUnitPriceAfterTax2Changed(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnLinePriceBeforeTax1Changing(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnLinePriceBeforeTax1Changed(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnLinePriceBeforeTax2Changing(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnLinePriceBeforeTax2Changed(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnLinePriceAfterTax1Changing(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnLinePriceAfterTax1Changed(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnLinePriceAfterTax2Changing(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnLinePriceAfterTax2Changed(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnTotalRevenueAfterTaxChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnTotalRevenueAfterTaxChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnTotalRevenueAccountedChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnTotalRevenueAccountedChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnArticleNameCacheChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnArticleNameCacheChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnArticleDescriptionCacheChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnArticleDescriptionCacheChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnReplacementNameChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnReplacementNameChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnReplacementDescriptionChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnReplacementDescriptionChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleDocumentItemEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleDocumentItemEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 44);	// [GVAC1]
		public static readonly new string EntityStructuredTypeKey = "[GVAC1]";
		
		#region Repository Class
		public new partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ArticleDocumentItemEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.IArticleDefinitionParameters Interface
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>IArticleDefinitionParameters</c> entity.
	///	designer:cap/GVAH1
	///	</summary>
	public interface IArticleDefinitionParameters
	{
		///	<summary>
		///	The <c>ArticleDefinition</c> field.
		///	designer:fld/GVAH1/GVAI1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAI1]")]
		global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity ArticleDefinition
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>ArticleParameters</c> field.
		///	designer:fld/GVAH1/GVAJ1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAJ1]")]
		string ArticleParameters
		{
			get;
			set;
		}
	}
	public static partial class IArticleDefinitionParametersInterfaceImplementation
	{
		public static global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity GetArticleDefinition(global::Epsitec.Cresus.Core.Entities.IArticleDefinitionParameters obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity> ("[GVAI1]");
		}
		public static void SetArticleDefinition(global::Epsitec.Cresus.Core.Entities.IArticleDefinitionParameters obj, global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity oldValue = obj.ArticleDefinition;
			if (oldValue != value || !entity.IsFieldDefined("[GVAI1]"))
			{
				IArticleDefinitionParametersInterfaceImplementation.OnArticleDefinitionChanging (obj, oldValue, value);
				entity.SetField<global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity> ("[GVAI1]", oldValue, value);
				IArticleDefinitionParametersInterfaceImplementation.OnArticleDefinitionChanged (obj, oldValue, value);
			}
		}
		static partial void OnArticleDefinitionChanged(global::Epsitec.Cresus.Core.Entities.IArticleDefinitionParameters obj, global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity oldValue, global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity newValue);
		static partial void OnArticleDefinitionChanging(global::Epsitec.Cresus.Core.Entities.IArticleDefinitionParameters obj, global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity oldValue, global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity newValue);
		public static string GetArticleParameters(global::Epsitec.Cresus.Core.Entities.IArticleDefinitionParameters obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[GVAJ1]");
		}
		public static void SetArticleParameters(global::Epsitec.Cresus.Core.Entities.IArticleDefinitionParameters obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.ArticleParameters;
			if (oldValue != value || !entity.IsFieldDefined("[GVAJ1]"))
			{
				IArticleDefinitionParametersInterfaceImplementation.OnArticleParametersChanging (obj, oldValue, value);
				entity.SetField<string> ("[GVAJ1]", oldValue, value);
				IArticleDefinitionParametersInterfaceImplementation.OnArticleParametersChanged (obj, oldValue, value);
			}
		}
		static partial void OnArticleParametersChanged(global::Epsitec.Cresus.Core.Entities.IArticleDefinitionParameters obj, string oldValue, string newValue);
		static partial void OnArticleParametersChanging(global::Epsitec.Cresus.Core.Entities.IArticleDefinitionParameters obj, string oldValue, string newValue);
	}
}
#endregion

#region Epsitec.Cresus.Core.ArticleDefinition Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ArticleDefinition</c> entity.
	///	designer:cap/GVAK1
	///	</summary>
	public partial class ArticleDefinitionEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IReferenceNumber, global::Epsitec.Cresus.Core.Entities.IBusinessLink, global::Epsitec.Cresus.Core.Entities.INameDescription, global::Epsitec.Cresus.Core.Entities.IComments
	{
		#region IReferenceNumber Members
		///	<summary>
		///	The <c>IdA</c> field.
		///	designer:fld/GVAK1/8VA11
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA11]")]
		public string IdA
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.GetIdA (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.SetIdA (this, value);
			}
		}
		#endregion
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/GVAK1/8VA3
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
		///	designer:fld/GVAK1/8VA7
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
		#region IComments Members
		///	<summary>
		///	The <c>Comments</c> field.
		///	designer:fld/GVAK1/8VAT
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAT]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.CommentEntity> Comments
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.ICommentsInterfaceImplementation.GetComments (this);
			}
		}
		#endregion
		#region IBusinessLink Members
		///	<summary>
		///	The <c>BusinessCodeVector</c> field.
		///	designer:fld/GVAK1/GVA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA5]")]
		public string BusinessCodeVector
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IBusinessLinkInterfaceImplementation.GetBusinessCodeVector (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IBusinessLinkInterfaceImplementation.SetBusinessCodeVector (this, value);
			}
		}
		#endregion
		#region IReferenceNumber Members
		///	<summary>
		///	The <c>IdB</c> field.
		///	designer:fld/GVAK1/8VA21
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA21]")]
		public string IdB
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.GetIdB (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.SetIdB (this, value);
			}
		}
		#endregion
		#region INameDescription Members
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/GVAK1/8VA8
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
		#region IReferenceNumber Members
		///	<summary>
		///	The <c>IdC</c> field.
		///	designer:fld/GVAK1/8VA31
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA31]")]
		public string IdC
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.GetIdC (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.SetIdC (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Pictures</c> field.
		///	designer:fld/GVAK1/GVAH2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAH2]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.ImageEntity> Pictures
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.ImageEntity> ("[GVAH2]");
			}
		}
		///	<summary>
		///	The <c>ArticleParameterDefinitions</c> field.
		///	designer:fld/GVAK1/GVAL2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAL2]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.AbstractArticleParameterDefinitionEntity> ArticleParameterDefinitions
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.AbstractArticleParameterDefinitionEntity> ("[GVAL2]");
			}
		}
		///	<summary>
		///	The <c>ArticleGroups</c> field.
		///	designer:fld/GVAK1/GVAN2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAN2]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.ArticleGroupEntity> ArticleGroups
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.ArticleGroupEntity> ("[GVAN2]");
			}
		}
		///	<summary>
		///	The <c>ArticleCategory</c> field.
		///	designer:fld/GVAK1/GVAP2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAP2]")]
		public global::Epsitec.Cresus.Core.Entities.ArticleCategoryEntity ArticleCategory
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.ArticleCategoryEntity> ("[GVAP2]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ArticleCategoryEntity oldValue = this.ArticleCategory;
				if (oldValue != value || !this.IsFieldDefined("[GVAP2]"))
				{
					this.OnArticleCategoryChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.ArticleCategoryEntity> ("[GVAP2]", oldValue, value);
					this.OnArticleCategoryChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ArticleSupplies</c> field.
		///	designer:fld/GVAK1/GVAB3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAB3]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.ArticleSupplyEntity> ArticleSupplies
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.ArticleSupplyEntity> ("[GVAB3]");
			}
		}
		///	<summary>
		///	The <c>ArticlePrices</c> field.
		///	designer:fld/GVAK1/GVAC3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAC3]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.ArticlePriceEntity> ArticlePrices
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.ArticlePriceEntity> ("[GVAC3]");
			}
		}
		///	<summary>
		///	The <c>ArticleAvailabilities</c> field.
		///	designer:fld/GVAK1/GVAM3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAM3]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.DateRangeEntity> ArticleAvailabilities
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.DateRangeEntity> ("[GVAM3]");
			}
		}
		///	<summary>
		///	The <c>Units</c> field.
		///	designer:fld/GVAK1/GVAP3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAP3]")]
		public global::Epsitec.Cresus.Core.Entities.UnitOfMeasureGroupEntity Units
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.UnitOfMeasureGroupEntity> ("[GVAP3]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.UnitOfMeasureGroupEntity oldValue = this.Units;
				if (oldValue != value || !this.IsFieldDefined("[GVAP3]"))
				{
					this.OnUnitsChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.UnitOfMeasureGroupEntity> ("[GVAP3]", oldValue, value);
					this.OnUnitsChanged (oldValue, value);
				}
			}
		}
		
		partial void OnArticleCategoryChanging(global::Epsitec.Cresus.Core.Entities.ArticleCategoryEntity oldValue, global::Epsitec.Cresus.Core.Entities.ArticleCategoryEntity newValue);
		partial void OnArticleCategoryChanged(global::Epsitec.Cresus.Core.Entities.ArticleCategoryEntity oldValue, global::Epsitec.Cresus.Core.Entities.ArticleCategoryEntity newValue);
		partial void OnUnitsChanging(global::Epsitec.Cresus.Core.Entities.UnitOfMeasureGroupEntity oldValue, global::Epsitec.Cresus.Core.Entities.UnitOfMeasureGroupEntity newValue);
		partial void OnUnitsChanged(global::Epsitec.Cresus.Core.Entities.UnitOfMeasureGroupEntity oldValue, global::Epsitec.Cresus.Core.Entities.UnitOfMeasureGroupEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 52);	// [GVAK1]
		public static readonly string EntityStructuredTypeKey = "[GVAK1]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ArticleDefinitionEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.ArticleTraceabilityDetail Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ArticleTraceabilityDetail</c> entity.
	///	designer:cap/GVAL1
	///	</summary>
	public partial class ArticleTraceabilityDetailEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>SerialId</c> field.
		///	designer:fld/GVAL1/GVAM1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAM1]")]
		public string SerialId
		{
			get
			{
				return this.GetField<string> ("[GVAM1]");
			}
			set
			{
				string oldValue = this.SerialId;
				if (oldValue != value || !this.IsFieldDefined("[GVAM1]"))
				{
					this.OnSerialIdChanging (oldValue, value);
					this.SetField<string> ("[GVAM1]", oldValue, value);
					this.OnSerialIdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>BatchId</c> field.
		///	designer:fld/GVAL1/GVAN1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAN1]")]
		public string BatchId
		{
			get
			{
				return this.GetField<string> ("[GVAN1]");
			}
			set
			{
				string oldValue = this.BatchId;
				if (oldValue != value || !this.IsFieldDefined("[GVAN1]"))
				{
					this.OnBatchIdChanging (oldValue, value);
					this.SetField<string> ("[GVAN1]", oldValue, value);
					this.OnBatchIdChanged (oldValue, value);
				}
			}
		}
		
		partial void OnSerialIdChanging(string oldValue, string newValue);
		partial void OnSerialIdChanged(string oldValue, string newValue);
		partial void OnBatchIdChanging(string oldValue, string newValue);
		partial void OnBatchIdChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleTraceabilityDetailEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleTraceabilityDetailEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 53);	// [GVAL1]
		public static readonly string EntityStructuredTypeKey = "[GVAL1]";
	}
}
#endregion

#region Epsitec.Cresus.Core.ArticleQuantity Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ArticleQuantity</c> entity.
	///	designer:cap/GVAP1
	///	</summary>
	public partial class ArticleQuantityEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.IDateRange
	{
		#region IDateRange Members
		///	<summary>
		///	The <c>BeginDate</c> field.
		///	designer:fld/GVAP1/8VAO
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
		///	designer:fld/GVAP1/8VAP
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
		///	The <c>Quantity</c> field.
		///	designer:fld/GVAP1/GVA57
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA57]")]
		public global::System.Decimal Quantity
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[GVA57]");
			}
			set
			{
				global::System.Decimal oldValue = this.Quantity;
				if (oldValue != value || !this.IsFieldDefined("[GVA57]"))
				{
					this.OnQuantityChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[GVA57]", oldValue, value);
					this.OnQuantityChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>QuantityColumn</c> field.
		///	designer:fld/GVAP1/GVAN6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAN6]")]
		public global::Epsitec.Cresus.Core.Entities.ArticleQuantityColumnEntity QuantityColumn
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.ArticleQuantityColumnEntity> ("[GVAN6]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ArticleQuantityColumnEntity oldValue = this.QuantityColumn;
				if (oldValue != value || !this.IsFieldDefined("[GVAN6]"))
				{
					this.OnQuantityColumnChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.ArticleQuantityColumnEntity> ("[GVAN6]", oldValue, value);
					this.OnQuantityColumnChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Unit</c> field.
		///	designer:fld/GVAP1/GVAR1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAR1]")]
		public global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity Unit
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity> ("[GVAR1]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity oldValue = this.Unit;
				if (oldValue != value || !this.IsFieldDefined("[GVAR1]"))
				{
					this.OnUnitChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity> ("[GVAR1]", oldValue, value);
					this.OnUnitChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ExpectedDateFormat</c> field.
		///	designer:fld/GVAP1/GVAT1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAT1]")]
		public string ExpectedDateFormat
		{
			get
			{
				return this.GetField<string> ("[GVAT1]");
			}
			set
			{
				string oldValue = this.ExpectedDateFormat;
				if (oldValue != value || !this.IsFieldDefined("[GVAT1]"))
				{
					this.OnExpectedDateFormatChanging (oldValue, value);
					this.SetField<string> ("[GVAT1]", oldValue, value);
					this.OnExpectedDateFormatChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ColumnName</c> field.
		///	designer:fld/GVAP1/GVAU1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAU1]")]
		public global::Epsitec.Common.Types.FormattedText ColumnName
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAU1]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.ColumnName;
				if (oldValue != value || !this.IsFieldDefined("[GVAU1]"))
				{
					this.OnColumnNameChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAU1]", oldValue, value);
					this.OnColumnNameChanged (oldValue, value);
				}
			}
		}
		
		partial void OnQuantityChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnQuantityChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnQuantityColumnChanging(global::Epsitec.Cresus.Core.Entities.ArticleQuantityColumnEntity oldValue, global::Epsitec.Cresus.Core.Entities.ArticleQuantityColumnEntity newValue);
		partial void OnQuantityColumnChanged(global::Epsitec.Cresus.Core.Entities.ArticleQuantityColumnEntity oldValue, global::Epsitec.Cresus.Core.Entities.ArticleQuantityColumnEntity newValue);
		partial void OnUnitChanging(global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity oldValue, global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity newValue);
		partial void OnUnitChanged(global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity oldValue, global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity newValue);
		partial void OnExpectedDateFormatChanging(string oldValue, string newValue);
		partial void OnExpectedDateFormatChanged(string oldValue, string newValue);
		partial void OnColumnNameChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnColumnNameChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleQuantityEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleQuantityEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 57);	// [GVAP1]
		public static readonly string EntityStructuredTypeKey = "[GVAP1]";
	}
}
#endregion

#region Epsitec.Cresus.Core.AbstractArticleParameterDefinition Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>AbstractArticleParameterDefinition</c> entity.
	///	designer:cap/GVAK2
	///	</summary>
	public partial class AbstractArticleParameterDefinitionEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/GVAK2/8VA3
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
		///	designer:fld/GVAK2/8VA5
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
		///	designer:fld/GVAK2/8VA7
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
		///	designer:fld/GVAK2/8VA8
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
		
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.AbstractArticleParameterDefinitionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.AbstractArticleParameterDefinitionEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 84);	// [GVAK2]
		public static readonly string EntityStructuredTypeKey = "[GVAK2]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AbstractArticleParameterDefinitionEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Unknown)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.ArticleGroup Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ArticleGroup</c> entity.
	///	designer:cap/GVAM2
	///	</summary>
	public partial class ArticleGroupEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory, global::Epsitec.Cresus.Core.Entities.IItemRank
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/GVAM2/8VA1
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
		///	designer:fld/GVAM2/8VA3
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
		///	designer:fld/GVAM2/8VA5
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
		///	designer:fld/GVAM2/8VA7
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
		///	designer:fld/GVAM2/8VA8
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
		///	The <c>Type</c> field.
		///	designer:fld/GVAM2/GVA89
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA89]")]
		public global::Epsitec.Cresus.Core.Business.ArticleGroupType Type
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.ArticleGroupType> ("[GVA89]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.ArticleGroupType oldValue = this.Type;
				if (oldValue != value || !this.IsFieldDefined("[GVA89]"))
				{
					this.OnTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.ArticleGroupType> ("[GVA89]", oldValue, value);
					this.OnTypeChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTypeChanging(global::Epsitec.Cresus.Core.Business.ArticleGroupType oldValue, global::Epsitec.Cresus.Core.Business.ArticleGroupType newValue);
		partial void OnTypeChanged(global::Epsitec.Cresus.Core.Business.ArticleGroupType oldValue, global::Epsitec.Cresus.Core.Business.ArticleGroupType newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleGroupEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleGroupEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 86);	// [GVAM2]
		public static readonly string EntityStructuredTypeKey = "[GVAM2]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ArticleGroupEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.ArticleCategory Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ArticleCategory</c> entity.
	///	designer:cap/GVAO2
	///	</summary>
	public partial class ArticleCategoryEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/GVAO2/8VA3
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
		///	designer:fld/GVAO2/8VA5
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
		///	designer:fld/GVAO2/8VA7
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
		///	designer:fld/GVAO2/8VA8
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
		///	The <c>ArticleType</c> field.
		///	designer:fld/GVAO2/GVA03
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA03]")]
		public global::Epsitec.Cresus.Core.Business.ArticleType ArticleType
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.ArticleType> ("[GVA03]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.ArticleType oldValue = this.ArticleType;
				if (oldValue != value || !this.IsFieldDefined("[GVA03]"))
				{
					this.OnArticleTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.ArticleType> ("[GVA03]", oldValue, value);
					this.OnArticleTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>VatRateType</c> field.
		///	designer:fld/GVAO2/GVAJ8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAJ8]")]
		public global::Epsitec.Cresus.Core.Business.Finance.VatRateType VatRateType
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.VatRateType> ("[GVAJ8]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.VatRateType oldValue = this.VatRateType;
				if (oldValue != value || !this.IsFieldDefined("[GVAJ8]"))
				{
					this.OnVatRateTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.VatRateType> ("[GVAJ8]", oldValue, value);
					this.OnVatRateTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>UnitOfMeasureCategory</c> field.
		///	designer:fld/GVAO2/GVAD8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAD8]")]
		public global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory UnitOfMeasureCategory
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory> ("[GVAD8]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory oldValue = this.UnitOfMeasureCategory;
				if (oldValue != value || !this.IsFieldDefined("[GVAD8]"))
				{
					this.OnUnitOfMeasureCategoryChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory> ("[GVAD8]", oldValue, value);
					this.OnUnitOfMeasureCategoryChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultPictures</c> field.
		///	designer:fld/GVAO2/GVAQ2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAQ2]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.ImageEntity> DefaultPictures
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.ImageEntity> ("[GVAQ2]");
			}
		}
		///	<summary>
		///	The <c>Accounting</c> field.
		///	designer:fld/GVAO2/GVA23
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA23]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.ArticleAccountingDefinitionEntity> Accounting
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.ArticleAccountingDefinitionEntity> ("[GVA23]");
			}
		}
		///	<summary>
		///	The <c>NeverApplyDiscount</c> field.
		///	designer:fld/GVAO2/GVAV2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAV2]")]
		public bool NeverApplyDiscount
		{
			get
			{
				return this.GetField<bool> ("[GVAV2]");
			}
			set
			{
				bool oldValue = this.NeverApplyDiscount;
				if (oldValue != value || !this.IsFieldDefined("[GVAV2]"))
				{
					this.OnNeverApplyDiscountChanging (oldValue, value);
					this.SetField<bool> ("[GVAV2]", oldValue, value);
					this.OnNeverApplyDiscountChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>RoundingMode</c> field.
		///	designer:fld/GVAO2/GVAU2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAU2]")]
		public global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity RoundingMode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity> ("[GVAU2]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity oldValue = this.RoundingMode;
				if (oldValue != value || !this.IsFieldDefined("[GVAU2]"))
				{
					this.OnRoundingModeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity> ("[GVAU2]", oldValue, value);
					this.OnRoundingModeChanged (oldValue, value);
				}
			}
		}
		
		partial void OnArticleTypeChanging(global::Epsitec.Cresus.Core.Business.ArticleType oldValue, global::Epsitec.Cresus.Core.Business.ArticleType newValue);
		partial void OnArticleTypeChanged(global::Epsitec.Cresus.Core.Business.ArticleType oldValue, global::Epsitec.Cresus.Core.Business.ArticleType newValue);
		partial void OnVatRateTypeChanging(global::Epsitec.Cresus.Core.Business.Finance.VatRateType oldValue, global::Epsitec.Cresus.Core.Business.Finance.VatRateType newValue);
		partial void OnVatRateTypeChanged(global::Epsitec.Cresus.Core.Business.Finance.VatRateType oldValue, global::Epsitec.Cresus.Core.Business.Finance.VatRateType newValue);
		partial void OnUnitOfMeasureCategoryChanging(global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory oldValue, global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory newValue);
		partial void OnUnitOfMeasureCategoryChanged(global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory oldValue, global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory newValue);
		partial void OnNeverApplyDiscountChanging(bool oldValue, bool newValue);
		partial void OnNeverApplyDiscountChanged(bool oldValue, bool newValue);
		partial void OnRoundingModeChanging(global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity newValue);
		partial void OnRoundingModeChanged(global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleCategoryEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleCategoryEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 88);	// [GVAO2]
		public static readonly string EntityStructuredTypeKey = "[GVAO2]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ArticleCategoryEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.ArticleAccountingDefinition Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ArticleAccountingDefinition</c> entity.
	///	designer:cap/GVA13
	///	</summary>
	public partial class ArticleAccountingDefinitionEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory, global::Epsitec.Cresus.Core.Entities.IDateRange, global::Epsitec.Cresus.Core.Entities.IFreezable
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/GVA13/8VA3
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
		///	designer:fld/GVA13/8VA5
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
		///	designer:fld/GVA13/8VA7
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
		#region IFreezable Members
		///	<summary>
		///	The <c>IsFrozen</c> field.
		///	designer:fld/GVA13/8VAC2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAC2]")]
		public bool IsFrozen
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IFreezableInterfaceImplementation.GetIsFrozen (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IFreezableInterfaceImplementation.SetIsFrozen (this, value);
			}
		}
		#endregion
		#region IDateRange Members
		///	<summary>
		///	The <c>BeginDate</c> field.
		///	designer:fld/GVA13/8VAO
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
		///	designer:fld/GVA13/8VA8
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
		///	designer:fld/GVA13/8VAP
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
		///	The <c>AccountingOperation</c> field.
		///	designer:fld/GVA13/GVAK8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAK8]")]
		public global::Epsitec.Cresus.Core.Entities.AccountingOperationEntity AccountingOperation
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.AccountingOperationEntity> ("[GVAK8]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.AccountingOperationEntity oldValue = this.AccountingOperation;
				if (oldValue != value || !this.IsFieldDefined("[GVAK8]"))
				{
					this.OnAccountingOperationChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.AccountingOperationEntity> ("[GVAK8]", oldValue, value);
					this.OnAccountingOperationChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CurrencyCode</c> field.
		///	designer:fld/GVA13/GVA73
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA73]")]
		public global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode CurrencyCode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode> ("[GVA73]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue = this.CurrencyCode;
				if (oldValue != value || !this.IsFieldDefined("[GVA73]"))
				{
					this.OnCurrencyCodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode> ("[GVA73]", oldValue, value);
					this.OnCurrencyCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TransactionBookAccount</c> field.
		///	designer:fld/GVA13/GVA33
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA33]")]
		public string TransactionBookAccount
		{
			get
			{
				return this.GetField<string> ("[GVA33]");
			}
			set
			{
				string oldValue = this.TransactionBookAccount;
				if (oldValue != value || !this.IsFieldDefined("[GVA33]"))
				{
					this.OnTransactionBookAccountChanging (oldValue, value);
					this.SetField<string> ("[GVA33]", oldValue, value);
					this.OnTransactionBookAccountChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>VatBookAccount</c> field.
		///	designer:fld/GVA13/GVA48
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA48]")]
		public string VatBookAccount
		{
			get
			{
				return this.GetField<string> ("[GVA48]");
			}
			set
			{
				string oldValue = this.VatBookAccount;
				if (oldValue != value || !this.IsFieldDefined("[GVA48]"))
				{
					this.OnVatBookAccountChanging (oldValue, value);
					this.SetField<string> ("[GVA48]", oldValue, value);
					this.OnVatBookAccountChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DiscountBookAccount</c> field.
		///	designer:fld/GVA13/GVA43
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA43]")]
		public string DiscountBookAccount
		{
			get
			{
				return this.GetField<string> ("[GVA43]");
			}
			set
			{
				string oldValue = this.DiscountBookAccount;
				if (oldValue != value || !this.IsFieldDefined("[GVA43]"))
				{
					this.OnDiscountBookAccountChanging (oldValue, value);
					this.SetField<string> ("[GVA43]", oldValue, value);
					this.OnDiscountBookAccountChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>RoundingBookAccount</c> field.
		///	designer:fld/GVA13/GVA68
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA68]")]
		public string RoundingBookAccount
		{
			get
			{
				return this.GetField<string> ("[GVA68]");
			}
			set
			{
				string oldValue = this.RoundingBookAccount;
				if (oldValue != value || !this.IsFieldDefined("[GVA68]"))
				{
					this.OnRoundingBookAccountChanging (oldValue, value);
					this.SetField<string> ("[GVA68]", oldValue, value);
					this.OnRoundingBookAccountChanged (oldValue, value);
				}
			}
		}
		
		partial void OnAccountingOperationChanging(global::Epsitec.Cresus.Core.Entities.AccountingOperationEntity oldValue, global::Epsitec.Cresus.Core.Entities.AccountingOperationEntity newValue);
		partial void OnAccountingOperationChanged(global::Epsitec.Cresus.Core.Entities.AccountingOperationEntity oldValue, global::Epsitec.Cresus.Core.Entities.AccountingOperationEntity newValue);
		partial void OnCurrencyCodeChanging(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode newValue);
		partial void OnCurrencyCodeChanged(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode newValue);
		partial void OnTransactionBookAccountChanging(string oldValue, string newValue);
		partial void OnTransactionBookAccountChanged(string oldValue, string newValue);
		partial void OnVatBookAccountChanging(string oldValue, string newValue);
		partial void OnVatBookAccountChanged(string oldValue, string newValue);
		partial void OnDiscountBookAccountChanging(string oldValue, string newValue);
		partial void OnDiscountBookAccountChanged(string oldValue, string newValue);
		partial void OnRoundingBookAccountChanging(string oldValue, string newValue);
		partial void OnRoundingBookAccountChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleAccountingDefinitionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleAccountingDefinitionEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 97);	// [GVA13]
		public static readonly string EntityStructuredTypeKey = "[GVA13]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ArticleAccountingDefinitionEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.ArticleSupply Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ArticleSupply</c> entity.
	///	designer:cap/GVA83
	///	</summary>
	public partial class ArticleSupplyEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.INameDescription
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/GVA83/8VA3
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
		///	designer:fld/GVA83/8VA7
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
		///	designer:fld/GVA83/8VA8
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
		///	The <c>StockLocation</c> field.
		///	designer:fld/GVA83/GVAT3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAT3]")]
		public global::Epsitec.Cresus.Core.Entities.ArticleStockLocationEntity StockLocation
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.ArticleStockLocationEntity> ("[GVAT3]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ArticleStockLocationEntity oldValue = this.StockLocation;
				if (oldValue != value || !this.IsFieldDefined("[GVAT3]"))
				{
					this.OnStockLocationChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.ArticleStockLocationEntity> ("[GVAT3]", oldValue, value);
					this.OnStockLocationChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>RestockingDelay</c> field.
		///	designer:fld/GVA83/GVAU3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAU3]")]
		public int? RestockingDelay
		{
			get
			{
				return this.GetField<int?> ("[GVAU3]");
			}
			set
			{
				int? oldValue = this.RestockingDelay;
				if (oldValue != value || !this.IsFieldDefined("[GVAU3]"))
				{
					this.OnRestockingDelayChanging (oldValue, value);
					this.SetField<int?> ("[GVAU3]", oldValue, value);
					this.OnRestockingDelayChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>RestockingThreshold</c> field.
		///	designer:fld/GVA83/GVAV3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAV3]")]
		public global::System.Decimal? RestockingThreshold
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAV3]");
			}
			set
			{
				global::System.Decimal? oldValue = this.RestockingThreshold;
				if (oldValue != value || !this.IsFieldDefined("[GVAV3]"))
				{
					this.OnRestockingThresholdChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAV3]", oldValue, value);
					this.OnRestockingThresholdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SupplierRelation</c> field.
		///	designer:fld/GVA83/GVAS3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAS3]")]
		public global::Epsitec.Cresus.Core.Entities.RelationEntity SupplierRelation
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.RelationEntity> ("[GVAS3]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue = this.SupplierRelation;
				if (oldValue != value || !this.IsFieldDefined("[GVAS3]"))
				{
					this.OnSupplierRelationChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.RelationEntity> ("[GVAS3]", oldValue, value);
					this.OnSupplierRelationChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SupplierArticleId</c> field.
		///	designer:fld/GVA83/GVAR3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAR3]")]
		public string SupplierArticleId
		{
			get
			{
				return this.GetField<string> ("[GVAR3]");
			}
			set
			{
				string oldValue = this.SupplierArticleId;
				if (oldValue != value || !this.IsFieldDefined("[GVAR3]"))
				{
					this.OnSupplierArticleIdChanging (oldValue, value);
					this.SetField<string> ("[GVAR3]", oldValue, value);
					this.OnSupplierArticleIdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SupplierArticlePrices</c> field.
		///	designer:fld/GVA83/GVAL3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAL3]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.ArticlePriceEntity> SupplierArticlePrices
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.ArticlePriceEntity> ("[GVAL3]");
			}
		}
		
		partial void OnStockLocationChanging(global::Epsitec.Cresus.Core.Entities.ArticleStockLocationEntity oldValue, global::Epsitec.Cresus.Core.Entities.ArticleStockLocationEntity newValue);
		partial void OnStockLocationChanged(global::Epsitec.Cresus.Core.Entities.ArticleStockLocationEntity oldValue, global::Epsitec.Cresus.Core.Entities.ArticleStockLocationEntity newValue);
		partial void OnRestockingDelayChanging(int? oldValue, int? newValue);
		partial void OnRestockingDelayChanged(int? oldValue, int? newValue);
		partial void OnRestockingThresholdChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnRestockingThresholdChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnSupplierRelationChanging(global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue, global::Epsitec.Cresus.Core.Entities.RelationEntity newValue);
		partial void OnSupplierRelationChanged(global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue, global::Epsitec.Cresus.Core.Entities.RelationEntity newValue);
		partial void OnSupplierArticleIdChanging(string oldValue, string newValue);
		partial void OnSupplierArticleIdChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleSupplyEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleSupplyEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 104);	// [GVA83]
		public static readonly string EntityStructuredTypeKey = "[GVA83]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ArticleSupplyEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.ArticleStockLocation Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ArticleStockLocation</c> entity.
	///	designer:cap/GVA93
	///	</summary>
	public partial class ArticleStockLocationEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/GVA93/8VA3
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
		///	designer:fld/GVA93/8VA5
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
		///	designer:fld/GVA93/8VA7
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
		///	designer:fld/GVA93/8VA8
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
		
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleStockLocationEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleStockLocationEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 105);	// [GVA93]
		public static readonly string EntityStructuredTypeKey = "[GVA93]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ArticleStockLocationEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.ArticlePrice Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ArticlePrice</c> entity.
	///	designer:cap/GVAA3
	///	</summary>
	public partial class ArticlePriceEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.IDateTimeRange
	{
		#region IDateTimeRange Members
		///	<summary>
		///	The <c>BeginDate</c> field.
		///	designer:fld/GVAA3/8VAK
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
		///	designer:fld/GVAA3/8VAL
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
		///	The <c>MinQuantity</c> field.
		///	designer:fld/GVAA3/GVAD3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAD3]")]
		public global::System.Decimal? MinQuantity
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAD3]");
			}
			set
			{
				global::System.Decimal? oldValue = this.MinQuantity;
				if (oldValue != value || !this.IsFieldDefined("[GVAD3]"))
				{
					this.OnMinQuantityChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAD3]", oldValue, value);
					this.OnMinQuantityChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MaxQuantity</c> field.
		///	designer:fld/GVAA3/GVAE3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAE3]")]
		public global::System.Decimal? MaxQuantity
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAE3]");
			}
			set
			{
				global::System.Decimal? oldValue = this.MaxQuantity;
				if (oldValue != value || !this.IsFieldDefined("[GVAE3]"))
				{
					this.OnMaxQuantityChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAE3]", oldValue, value);
					this.OnMaxQuantityChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CurrencyCode</c> field.
		///	designer:fld/GVAA3/GVAF3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAF3]")]
		public global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode CurrencyCode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode> ("[GVAF3]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue = this.CurrencyCode;
				if (oldValue != value || !this.IsFieldDefined("[GVAF3]"))
				{
					this.OnCurrencyCodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode> ("[GVAF3]", oldValue, value);
					this.OnCurrencyCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Value</c> field.
		///	designer:fld/GVAA3/GVAG3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAG3]")]
		public global::System.Decimal Value
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[GVAG3]");
			}
			set
			{
				global::System.Decimal oldValue = this.Value;
				if (oldValue != value || !this.IsFieldDefined("[GVAG3]"))
				{
					this.OnValueChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[GVAG3]", oldValue, value);
					this.OnValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ValueOverridesPriceGroup</c> field.
		///	designer:fld/GVAA3/GVAI3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAI3]")]
		public bool ValueOverridesPriceGroup
		{
			get
			{
				return this.GetField<bool> ("[GVAI3]");
			}
			set
			{
				bool oldValue = this.ValueOverridesPriceGroup;
				if (oldValue != value || !this.IsFieldDefined("[GVAI3]"))
				{
					this.OnValueOverridesPriceGroupChanging (oldValue, value);
					this.SetField<bool> ("[GVAI3]", oldValue, value);
					this.OnValueOverridesPriceGroupChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>BillingUnit</c> field.
		///	designer:fld/GVAA3/GVAB8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAB8]")]
		public global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity BillingUnit
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity> ("[GVAB8]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity oldValue = this.BillingUnit;
				if (oldValue != value || !this.IsFieldDefined("[GVAB8]"))
				{
					this.OnBillingUnitChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity> ("[GVAB8]", oldValue, value);
					this.OnBillingUnitChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PriceCalculators</c> field.
		///	designer:fld/GVAA3/GVAK3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAK3]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.PriceCalculatorEntity> PriceCalculators
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.PriceCalculatorEntity> ("[GVAK3]");
			}
		}
		///	<summary>
		///	The <c>PriceGroups</c> field.
		///	designer:fld/GVAA3/GVAJ3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAJ3]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.PriceGroupEntity> PriceGroups
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.PriceGroupEntity> ("[GVAJ3]");
			}
		}
		
		partial void OnMinQuantityChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnMinQuantityChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnMaxQuantityChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnMaxQuantityChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnCurrencyCodeChanging(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode newValue);
		partial void OnCurrencyCodeChanged(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode newValue);
		partial void OnValueChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnValueChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnValueOverridesPriceGroupChanging(bool oldValue, bool newValue);
		partial void OnValueOverridesPriceGroupChanged(bool oldValue, bool newValue);
		partial void OnBillingUnitChanging(global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity oldValue, global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity newValue);
		partial void OnBillingUnitChanged(global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity oldValue, global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticlePriceEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticlePriceEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 106);	// [GVAA3]
		public static readonly string EntityStructuredTypeKey = "[GVAA3]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ArticlePriceEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.EndTotalDocumentItem Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>EndTotalDocumentItem</c> entity.
	///	designer:cap/GVA94
	///	</summary>
	public partial class EndTotalDocumentItemEntity : global::Epsitec.Cresus.Core.Entities.AbstractDocumentItemEntity
	{
		///	<summary>
		///	The <c>TextForPrice</c> field.
		///	designer:fld/GVA94/GVAA4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAA4]")]
		public global::Epsitec.Common.Types.FormattedText TextForPrice
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAA4]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.TextForPrice;
				if (oldValue != value || !this.IsFieldDefined("[GVAA4]"))
				{
					this.OnTextForPriceChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAA4]", oldValue, value);
					this.OnTextForPriceChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TextForFixedPrice</c> field.
		///	designer:fld/GVA94/GVAB4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAB4]")]
		public global::Epsitec.Common.Types.FormattedText TextForFixedPrice
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAB4]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.TextForFixedPrice;
				if (oldValue != value || !this.IsFieldDefined("[GVAB4]"))
				{
					this.OnTextForFixedPriceChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAB4]", oldValue, value);
					this.OnTextForFixedPriceChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PriceBeforeTax</c> field.
		///	designer:fld/GVA94/GVAC4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAC4]")]
		public global::System.Decimal? PriceBeforeTax
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAC4]");
			}
			set
			{
				global::System.Decimal? oldValue = this.PriceBeforeTax;
				if (oldValue != value || !this.IsFieldDefined("[GVAC4]"))
				{
					this.OnPriceBeforeTaxChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAC4]", oldValue, value);
					this.OnPriceBeforeTaxChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PriceAfterTax</c> field.
		///	designer:fld/GVA94/GVAD4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAD4]")]
		public global::System.Decimal? PriceAfterTax
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAD4]");
			}
			set
			{
				global::System.Decimal? oldValue = this.PriceAfterTax;
				if (oldValue != value || !this.IsFieldDefined("[GVAD4]"))
				{
					this.OnPriceAfterTaxChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAD4]", oldValue, value);
					this.OnPriceAfterTaxChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TotalRounding</c> field.
		///	designer:fld/GVA94/GVAH8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAH8]")]
		public global::System.Decimal? TotalRounding
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAH8]");
			}
			set
			{
				global::System.Decimal? oldValue = this.TotalRounding;
				if (oldValue != value || !this.IsFieldDefined("[GVAH8]"))
				{
					this.OnTotalRoundingChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAH8]", oldValue, value);
					this.OnTotalRoundingChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>FixedPriceAfterTax</c> field.
		///	designer:fld/GVA94/GVAE4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAE4]")]
		public global::System.Decimal? FixedPriceAfterTax
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAE4]");
			}
			set
			{
				global::System.Decimal? oldValue = this.FixedPriceAfterTax;
				if (oldValue != value || !this.IsFieldDefined("[GVAE4]"))
				{
					this.OnFixedPriceAfterTaxChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAE4]", oldValue, value);
					this.OnFixedPriceAfterTaxChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTextForPriceChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextForPriceChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextForFixedPriceChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextForFixedPriceChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnPriceBeforeTaxChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnPriceBeforeTaxChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnPriceAfterTaxChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnPriceAfterTaxChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnTotalRoundingChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnTotalRoundingChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnFixedPriceAfterTaxChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnFixedPriceAfterTaxChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.EndTotalDocumentItemEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.EndTotalDocumentItemEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 137);	// [GVA94]
		public static readonly new string EntityStructuredTypeKey = "[GVA94]";
	}
}
#endregion

#region Epsitec.Cresus.Core.SubTotalDocumentItem Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>SubTotalDocumentItem</c> entity.
	///	designer:cap/GVAF4
	///	</summary>
	public partial class SubTotalDocumentItemEntity : global::Epsitec.Cresus.Core.Entities.AbstractDocumentItemEntity
	{
		///	<summary>
		///	The <c>DisplayModes</c> field.
		///	designer:fld/GVAF4/GVAG4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAG4]")]
		public global::Epsitec.Cresus.Core.Business.Finance.PriceDisplayModes DisplayModes
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.PriceDisplayModes> ("[GVAG4]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.PriceDisplayModes oldValue = this.DisplayModes;
				if (oldValue != value || !this.IsFieldDefined("[GVAG4]"))
				{
					this.OnDisplayModesChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.PriceDisplayModes> ("[GVAG4]", oldValue, value);
					this.OnDisplayModesChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TextForPrimaryPrice</c> field.
		///	designer:fld/GVAF4/GVAH4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAH4]")]
		public global::Epsitec.Common.Types.FormattedText TextForPrimaryPrice
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAH4]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.TextForPrimaryPrice;
				if (oldValue != value || !this.IsFieldDefined("[GVAH4]"))
				{
					this.OnTextForPrimaryPriceChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAH4]", oldValue, value);
					this.OnTextForPrimaryPriceChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TextForResultingPrice</c> field.
		///	designer:fld/GVAF4/GVAI4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAI4]")]
		public global::Epsitec.Common.Types.FormattedText TextForResultingPrice
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAI4]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.TextForResultingPrice;
				if (oldValue != value || !this.IsFieldDefined("[GVAI4]"))
				{
					this.OnTextForResultingPriceChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAI4]", oldValue, value);
					this.OnTextForResultingPriceChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TextForDiscount</c> field.
		///	designer:fld/GVAF4/GVAO6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAO6]")]
		public global::Epsitec.Common.Types.FormattedText TextForDiscount
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAO6]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.TextForDiscount;
				if (oldValue != value || !this.IsFieldDefined("[GVAO6]"))
				{
					this.OnTextForDiscountChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAO6]", oldValue, value);
					this.OnTextForDiscountChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Discount</c> field.
		///	designer:fld/GVAF4/GVAM4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAM4]")]
		public global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity Discount
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity> ("[GVAM4]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity oldValue = this.Discount;
				if (oldValue != value || !this.IsFieldDefined("[GVAM4]"))
				{
					this.OnDiscountChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity> ("[GVAM4]", oldValue, value);
					this.OnDiscountChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PriceBeforeTax1</c> field.
		///	designer:fld/GVAF4/GVAK4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAK4]")]
		public global::System.Decimal? PriceBeforeTax1
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAK4]");
			}
			set
			{
				global::System.Decimal? oldValue = this.PriceBeforeTax1;
				if (oldValue != value || !this.IsFieldDefined("[GVAK4]"))
				{
					this.OnPriceBeforeTax1Changing (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAK4]", oldValue, value);
					this.OnPriceBeforeTax1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PriceBeforeTax2</c> field.
		///	designer:fld/GVAF4/GVAL4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAL4]")]
		public global::System.Decimal? PriceBeforeTax2
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAL4]");
			}
			set
			{
				global::System.Decimal? oldValue = this.PriceBeforeTax2;
				if (oldValue != value || !this.IsFieldDefined("[GVAL4]"))
				{
					this.OnPriceBeforeTax2Changing (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAL4]", oldValue, value);
					this.OnPriceBeforeTax2Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PriceAfterTax1</c> field.
		///	designer:fld/GVAF4/GVAU5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAU5]")]
		public global::System.Decimal? PriceAfterTax1
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAU5]");
			}
			set
			{
				global::System.Decimal? oldValue = this.PriceAfterTax1;
				if (oldValue != value || !this.IsFieldDefined("[GVAU5]"))
				{
					this.OnPriceAfterTax1Changing (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAU5]", oldValue, value);
					this.OnPriceAfterTax1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PriceAfterTax2</c> field.
		///	designer:fld/GVAF4/GVAP4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAP4]")]
		public global::System.Decimal? PriceAfterTax2
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAP4]");
			}
			set
			{
				global::System.Decimal? oldValue = this.PriceAfterTax2;
				if (oldValue != value || !this.IsFieldDefined("[GVAP4]"))
				{
					this.OnPriceAfterTax2Changing (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAP4]", oldValue, value);
					this.OnPriceAfterTax2Changed (oldValue, value);
				}
			}
		}
		
		partial void OnDisplayModesChanging(global::Epsitec.Cresus.Core.Business.Finance.PriceDisplayModes oldValue, global::Epsitec.Cresus.Core.Business.Finance.PriceDisplayModes newValue);
		partial void OnDisplayModesChanged(global::Epsitec.Cresus.Core.Business.Finance.PriceDisplayModes oldValue, global::Epsitec.Cresus.Core.Business.Finance.PriceDisplayModes newValue);
		partial void OnTextForPrimaryPriceChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextForPrimaryPriceChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextForResultingPriceChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextForResultingPriceChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextForDiscountChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextForDiscountChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnDiscountChanging(global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity newValue);
		partial void OnDiscountChanged(global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity newValue);
		partial void OnPriceBeforeTax1Changing(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnPriceBeforeTax1Changed(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnPriceBeforeTax2Changing(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnPriceBeforeTax2Changed(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnPriceAfterTax1Changing(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnPriceAfterTax1Changed(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnPriceAfterTax2Changing(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnPriceAfterTax2Changed(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.SubTotalDocumentItemEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.SubTotalDocumentItemEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 143);	// [GVAF4]
		public static readonly new string EntityStructuredTypeKey = "[GVAF4]";
	}
}
#endregion

#region Epsitec.Cresus.Core.TaxDocumentItem Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>TaxDocumentItem</c> entity.
	///	designer:cap/GVAR4
	///	</summary>
	public partial class TaxDocumentItemEntity : global::Epsitec.Cresus.Core.Entities.AbstractDocumentItemEntity
	{
		///	<summary>
		///	The <c>Text</c> field.
		///	designer:fld/GVAR4/GVAS4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAS4]")]
		public global::Epsitec.Common.Types.FormattedText Text
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAS4]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Text;
				if (oldValue != value || !this.IsFieldDefined("[GVAS4]"))
				{
					this.OnTextChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAS4]", oldValue, value);
					this.OnTextChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>VatRateType</c> field.
		///	designer:fld/GVAR4/GVAL8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAL8]")]
		public global::Epsitec.Cresus.Core.Business.Finance.VatRateType VatRateType
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.VatRateType> ("[GVAL8]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.VatRateType oldValue = this.VatRateType;
				if (oldValue != value || !this.IsFieldDefined("[GVAL8]"))
				{
					this.OnVatRateTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.VatRateType> ("[GVAL8]", oldValue, value);
					this.OnVatRateTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>VatRate</c> field.
		///	designer:fld/GVAR4/GVAV4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAV4]")]
		public global::System.Decimal VatRate
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[GVAV4]");
			}
			set
			{
				global::System.Decimal oldValue = this.VatRate;
				if (oldValue != value || !this.IsFieldDefined("[GVAV4]"))
				{
					this.OnVatRateChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[GVAV4]", oldValue, value);
					this.OnVatRateChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TotalRevenue</c> field.
		///	designer:fld/GVAR4/GVAU4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAU4]")]
		public global::System.Decimal TotalRevenue
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[GVAU4]");
			}
			set
			{
				global::System.Decimal oldValue = this.TotalRevenue;
				if (oldValue != value || !this.IsFieldDefined("[GVAU4]"))
				{
					this.OnTotalRevenueChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[GVAU4]", oldValue, value);
					this.OnTotalRevenueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ResultingTax</c> field.
		///	designer:fld/GVAR4/GVA05
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA05]")]
		public global::System.Decimal ResultingTax
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[GVA05]");
			}
			set
			{
				global::System.Decimal oldValue = this.ResultingTax;
				if (oldValue != value || !this.IsFieldDefined("[GVA05]"))
				{
					this.OnResultingTaxChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[GVA05]", oldValue, value);
					this.OnResultingTaxChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTextChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnVatRateTypeChanging(global::Epsitec.Cresus.Core.Business.Finance.VatRateType oldValue, global::Epsitec.Cresus.Core.Business.Finance.VatRateType newValue);
		partial void OnVatRateTypeChanged(global::Epsitec.Cresus.Core.Business.Finance.VatRateType oldValue, global::Epsitec.Cresus.Core.Business.Finance.VatRateType newValue);
		partial void OnVatRateChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnVatRateChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnTotalRevenueChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnTotalRevenueChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnResultingTaxChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnResultingTaxChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.TaxDocumentItemEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.TaxDocumentItemEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 155);	// [GVAR4]
		public static readonly new string EntityStructuredTypeKey = "[GVAR4]";
	}
}
#endregion

#region Epsitec.Cresus.Core.EnumValueArticleParameterDefinition Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>EnumValueArticleParameterDefinition</c> entity.
	///	designer:cap/GVA85
	///	</summary>
	public partial class EnumValueArticleParameterDefinitionEntity : global::Epsitec.Cresus.Core.Entities.AbstractArticleParameterDefinitionEntity
	{
		///	<summary>
		///	The <c>Cardinality</c> field.
		///	designer:fld/GVA85/GVA95
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA95]")]
		public global::Epsitec.Cresus.Core.Library.EnumValueCardinality Cardinality
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Library.EnumValueCardinality> ("[GVA95]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Library.EnumValueCardinality oldValue = this.Cardinality;
				if (oldValue != value || !this.IsFieldDefined("[GVA95]"))
				{
					this.OnCardinalityChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Library.EnumValueCardinality> ("[GVA95]", oldValue, value);
					this.OnCardinalityChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultValue</c> field.
		///	designer:fld/GVA85/GVAA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAA5]")]
		public string DefaultValue
		{
			get
			{
				return this.GetField<string> ("[GVAA5]");
			}
			set
			{
				string oldValue = this.DefaultValue;
				if (oldValue != value || !this.IsFieldDefined("[GVAA5]"))
				{
					this.OnDefaultValueChanging (oldValue, value);
					this.SetField<string> ("[GVAA5]", oldValue, value);
					this.OnDefaultValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Values</c> field.
		///	designer:fld/GVA85/GVAB5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAB5]")]
		public string Values
		{
			get
			{
				return this.GetField<string> ("[GVAB5]");
			}
			set
			{
				string oldValue = this.Values;
				if (oldValue != value || !this.IsFieldDefined("[GVAB5]"))
				{
					this.OnValuesChanging (oldValue, value);
					this.SetField<string> ("[GVAB5]", oldValue, value);
					this.OnValuesChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ShortDescriptions</c> field.
		///	designer:fld/GVA85/GVAC5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAC5]")]
		public global::Epsitec.Common.Types.FormattedText ShortDescriptions
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAC5]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.ShortDescriptions;
				if (oldValue != value || !this.IsFieldDefined("[GVAC5]"))
				{
					this.OnShortDescriptionsChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAC5]", oldValue, value);
					this.OnShortDescriptionsChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LongDescriptions</c> field.
		///	designer:fld/GVA85/GVAD5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAD5]")]
		public global::Epsitec.Common.Types.FormattedText LongDescriptions
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAD5]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.LongDescriptions;
				if (oldValue != value || !this.IsFieldDefined("[GVAD5]"))
				{
					this.OnLongDescriptionsChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAD5]", oldValue, value);
					this.OnLongDescriptionsChanged (oldValue, value);
				}
			}
		}
		
		partial void OnCardinalityChanging(global::Epsitec.Cresus.Core.Library.EnumValueCardinality oldValue, global::Epsitec.Cresus.Core.Library.EnumValueCardinality newValue);
		partial void OnCardinalityChanged(global::Epsitec.Cresus.Core.Library.EnumValueCardinality oldValue, global::Epsitec.Cresus.Core.Library.EnumValueCardinality newValue);
		partial void OnDefaultValueChanging(string oldValue, string newValue);
		partial void OnDefaultValueChanged(string oldValue, string newValue);
		partial void OnValuesChanging(string oldValue, string newValue);
		partial void OnValuesChanged(string oldValue, string newValue);
		partial void OnShortDescriptionsChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnShortDescriptionsChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnLongDescriptionsChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnLongDescriptionsChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.EnumValueArticleParameterDefinitionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.EnumValueArticleParameterDefinitionEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 168);	// [GVA85]
		public static readonly new string EntityStructuredTypeKey = "[GVA85]";
		
		#region Repository Class
		public new partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<EnumValueArticleParameterDefinitionEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.NumericValueArticleParameterDefinition Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>NumericValueArticleParameterDefinition</c> entity.
	///	designer:cap/GVAE5
	///	</summary>
	public partial class NumericValueArticleParameterDefinitionEntity : global::Epsitec.Cresus.Core.Entities.AbstractArticleParameterDefinitionEntity, global::Epsitec.Cresus.Core.Entities.IRoundingMode
	{
		#region IRoundingMode Members
		///	<summary>
		///	The <c>Modulo</c> field.
		///	designer:fld/GVAE5/CVAN3
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
		///	<summary>
		///	The <c>AddBeforeModulo</c> field.
		///	designer:fld/GVAE5/CVAO3
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
		///	The <c>MinValue</c> field.
		///	designer:fld/GVAE5/GVAF5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAF5]")]
		public global::System.Decimal? MinValue
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAF5]");
			}
			set
			{
				global::System.Decimal? oldValue = this.MinValue;
				if (oldValue != value || !this.IsFieldDefined("[GVAF5]"))
				{
					this.OnMinValueChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAF5]", oldValue, value);
					this.OnMinValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MaxValue</c> field.
		///	designer:fld/GVAE5/GVAG5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAG5]")]
		public global::System.Decimal? MaxValue
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAG5]");
			}
			set
			{
				global::System.Decimal? oldValue = this.MaxValue;
				if (oldValue != value || !this.IsFieldDefined("[GVAG5]"))
				{
					this.OnMaxValueChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAG5]", oldValue, value);
					this.OnMaxValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultValue</c> field.
		///	designer:fld/GVAE5/GVAH5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAH5]")]
		public global::System.Decimal? DefaultValue
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAH5]");
			}
			set
			{
				global::System.Decimal? oldValue = this.DefaultValue;
				if (oldValue != value || !this.IsFieldDefined("[GVAH5]"))
				{
					this.OnDefaultValueChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAH5]", oldValue, value);
					this.OnDefaultValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PreferredValues</c> field.
		///	designer:fld/GVAE5/GVAI5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAI5]")]
		public string PreferredValues
		{
			get
			{
				return this.GetField<string> ("[GVAI5]");
			}
			set
			{
				string oldValue = this.PreferredValues;
				if (oldValue != value || !this.IsFieldDefined("[GVAI5]"))
				{
					this.OnPreferredValuesChanging (oldValue, value);
					this.SetField<string> ("[GVAI5]", oldValue, value);
					this.OnPreferredValuesChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>UnitOfMeasure</c> field.
		///	designer:fld/GVAE5/GVAJ5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAJ5]")]
		public global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity UnitOfMeasure
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity> ("[GVAJ5]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity oldValue = this.UnitOfMeasure;
				if (oldValue != value || !this.IsFieldDefined("[GVAJ5]"))
				{
					this.OnUnitOfMeasureChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity> ("[GVAJ5]", oldValue, value);
					this.OnUnitOfMeasureChanged (oldValue, value);
				}
			}
		}
		
		partial void OnMinValueChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnMinValueChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnMaxValueChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnMaxValueChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnDefaultValueChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnDefaultValueChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnPreferredValuesChanging(string oldValue, string newValue);
		partial void OnPreferredValuesChanged(string oldValue, string newValue);
		partial void OnUnitOfMeasureChanging(global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity oldValue, global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity newValue);
		partial void OnUnitOfMeasureChanged(global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity oldValue, global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.NumericValueArticleParameterDefinitionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.NumericValueArticleParameterDefinitionEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 174);	// [GVAE5]
		public static readonly new string EntityStructuredTypeKey = "[GVAE5]";
		
		#region Repository Class
		public new partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<NumericValueArticleParameterDefinitionEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.OptionValueArticleParameterDefinition Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>OptionValueArticleParameterDefinition</c> entity.
	///	designer:cap/GVAK5
	///	</summary>
	public partial class OptionValueArticleParameterDefinitionEntity : global::Epsitec.Cresus.Core.Entities.AbstractArticleParameterDefinitionEntity
	{
		///	<summary>
		///	The <c>Cardinality</c> field.
		///	designer:fld/GVAK5/GVAL5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAL5]")]
		public global::Epsitec.Cresus.Core.Library.EnumValueCardinality Cardinality
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Library.EnumValueCardinality> ("[GVAL5]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Library.EnumValueCardinality oldValue = this.Cardinality;
				if (oldValue != value || !this.IsFieldDefined("[GVAL5]"))
				{
					this.OnCardinalityChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Library.EnumValueCardinality> ("[GVAL5]", oldValue, value);
					this.OnCardinalityChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Options</c> field.
		///	designer:fld/GVAK5/GVAO5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAO5]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.OptionValueEntity> Options
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.OptionValueEntity> ("[GVAO5]");
			}
		}
		
		partial void OnCardinalityChanging(global::Epsitec.Cresus.Core.Library.EnumValueCardinality oldValue, global::Epsitec.Cresus.Core.Library.EnumValueCardinality newValue);
		partial void OnCardinalityChanged(global::Epsitec.Cresus.Core.Library.EnumValueCardinality oldValue, global::Epsitec.Cresus.Core.Library.EnumValueCardinality newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.OptionValueArticleParameterDefinitionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.OptionValueArticleParameterDefinitionEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 180);	// [GVAK5]
		public static readonly new string EntityStructuredTypeKey = "[GVAK5]";
		
		#region Repository Class
		public new partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<OptionValueArticleParameterDefinitionEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.OptionValue Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>OptionValue</c> entity.
	///	designer:cap/GVAM5
	///	</summary>
	public partial class OptionValueEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory, global::Epsitec.Cresus.Core.Entities.IArticleDefinitionParameters
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/GVAM5/8VA3
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
		///	designer:fld/GVAM5/8VA5
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
		///	designer:fld/GVAM5/8VA7
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
		#region IArticleDefinitionParameters Members
		///	<summary>
		///	The <c>ArticleDefinition</c> field.
		///	designer:fld/GVAM5/GVAI1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAI1]")]
		public global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity ArticleDefinition
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IArticleDefinitionParametersInterfaceImplementation.GetArticleDefinition (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IArticleDefinitionParametersInterfaceImplementation.SetArticleDefinition (this, value);
			}
		}
		#endregion
		#region INameDescription Members
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/GVAM5/8VA8
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
		#region IArticleDefinitionParameters Members
		///	<summary>
		///	The <c>ArticleParameters</c> field.
		///	designer:fld/GVAM5/GVAJ1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAJ1]")]
		public string ArticleParameters
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IArticleDefinitionParametersInterfaceImplementation.GetArticleParameters (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IArticleDefinitionParametersInterfaceImplementation.SetArticleParameters (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Quantity</c> field.
		///	designer:fld/GVAM5/GVAN5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAN5]")]
		public global::System.Decimal Quantity
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[GVAN5]");
			}
			set
			{
				global::System.Decimal oldValue = this.Quantity;
				if (oldValue != value || !this.IsFieldDefined("[GVAN5]"))
				{
					this.OnQuantityChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[GVAN5]", oldValue, value);
					this.OnQuantityChanged (oldValue, value);
				}
			}
		}
		
		partial void OnQuantityChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnQuantityChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.OptionValueEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.OptionValueEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 182);	// [GVAM5]
		public static readonly string EntityStructuredTypeKey = "[GVAM5]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<OptionValueEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.FreeTextValueArticleParameterDefinition Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>FreeTextValueArticleParameterDefinition</c> entity.
	///	designer:cap/GVAP5
	///	</summary>
	public partial class FreeTextValueArticleParameterDefinitionEntity : global::Epsitec.Cresus.Core.Entities.AbstractArticleParameterDefinitionEntity
	{
		///	<summary>
		///	The <c>ShortText</c> field.
		///	designer:fld/GVAP5/GVAQ5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAQ5]")]
		public global::Epsitec.Common.Types.FormattedText ShortText
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAQ5]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.ShortText;
				if (oldValue != value || !this.IsFieldDefined("[GVAQ5]"))
				{
					this.OnShortTextChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAQ5]", oldValue, value);
					this.OnShortTextChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LongText</c> field.
		///	designer:fld/GVAP5/GVAR5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAR5]")]
		public global::Epsitec.Common.Types.FormattedText LongText
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAR5]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.LongText;
				if (oldValue != value || !this.IsFieldDefined("[GVAR5]"))
				{
					this.OnLongTextChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAR5]", oldValue, value);
					this.OnLongTextChanged (oldValue, value);
				}
			}
		}
		
		partial void OnShortTextChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnShortTextChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnLongTextChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnLongTextChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.FreeTextValueArticleParameterDefinitionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.FreeTextValueArticleParameterDefinitionEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 185);	// [GVAP5]
		public static readonly new string EntityStructuredTypeKey = "[GVAP5]";
		
		#region Repository Class
		public new partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<FreeTextValueArticleParameterDefinitionEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.TextDocumentItem Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>TextDocumentItem</c> entity.
	///	designer:cap/GVAS5
	///	</summary>
	public partial class TextDocumentItemEntity : global::Epsitec.Cresus.Core.Entities.AbstractDocumentItemEntity
	{
		///	<summary>
		///	The <c>Text</c> field.
		///	designer:fld/GVAS5/GVAT5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAT5]")]
		public global::Epsitec.Common.Types.FormattedText Text
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAT5]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Text;
				if (oldValue != value || !this.IsFieldDefined("[GVAT5]"))
				{
					this.OnTextChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAT5]", oldValue, value);
					this.OnTextChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTextChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.TextDocumentItemEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.TextDocumentItemEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 188);	// [GVAS5]
		public static readonly new string EntityStructuredTypeKey = "[GVAS5]";
	}
}
#endregion

#region Epsitec.Cresus.Core.Customer Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>Customer</c> entity.
	///	designer:cap/GVAV5
	///	</summary>
	public partial class CustomerEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IReferenceNumber, global::Epsitec.Cresus.Core.Entities.IBusinessLink, global::Epsitec.Cresus.Core.Entities.IWorkflowHost, global::Epsitec.Cresus.Core.Entities.IItemCode
	{
		#region IReferenceNumber Members
		///	<summary>
		///	The <c>IdA</c> field.
		///	designer:fld/GVAV5/8VA11
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA11]")]
		public string IdA
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.GetIdA (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.SetIdA (this, value);
			}
		}
		#endregion
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/GVAV5/8VA3
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
		///	designer:fld/GVAV5/8VA5
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
		#region IWorkflowHost Members
		///	<summary>
		///	The <c>Workflow</c> field.
		///	designer:fld/GVAV5/DVA31
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[DVA31]")]
		public global::Epsitec.Cresus.Core.Entities.WorkflowEntity Workflow
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IWorkflowHostInterfaceImplementation.GetWorkflow (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IWorkflowHostInterfaceImplementation.SetWorkflow (this, value);
			}
		}
		#endregion
		#region IBusinessLink Members
		///	<summary>
		///	The <c>BusinessCodeVector</c> field.
		///	designer:fld/GVAV5/GVA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA5]")]
		public string BusinessCodeVector
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IBusinessLinkInterfaceImplementation.GetBusinessCodeVector (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IBusinessLinkInterfaceImplementation.SetBusinessCodeVector (this, value);
			}
		}
		#endregion
		#region IReferenceNumber Members
		///	<summary>
		///	The <c>IdB</c> field.
		///	designer:fld/GVAV5/8VA21
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA21]")]
		public string IdB
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.GetIdB (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.SetIdB (this, value);
			}
		}
		///	<summary>
		///	The <c>IdC</c> field.
		///	designer:fld/GVAV5/8VA31
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA31]")]
		public string IdC
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.GetIdC (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IReferenceNumberInterfaceImplementation.SetIdC (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>CustomerCategory</c> field.
		///	designer:fld/GVAV5/GVAO8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAO8]")]
		public global::Epsitec.Cresus.Core.Entities.CustomerCategoryEntity CustomerCategory
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.CustomerCategoryEntity> ("[GVAO8]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.CustomerCategoryEntity oldValue = this.CustomerCategory;
				if (oldValue != value || !this.IsFieldDefined("[GVAO8]"))
				{
					this.OnCustomerCategoryChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.CustomerCategoryEntity> ("[GVAO8]", oldValue, value);
					this.OnCustomerCategoryChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MainRelation</c> field.
		///	designer:fld/GVAV5/GVA06
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA06]")]
		public global::Epsitec.Cresus.Core.Entities.RelationEntity MainRelation
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.RelationEntity> ("[GVA06]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue = this.MainRelation;
				if (oldValue != value || !this.IsFieldDefined("[GVA06]"))
				{
					this.OnMainRelationChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.RelationEntity> ("[GVA06]", oldValue, value);
					this.OnMainRelationChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Affairs</c> field.
		///	designer:fld/GVAV5/GVA26
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA26]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.AffairEntity> Affairs
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.AffairEntity> ("[GVA26]");
			}
		}
		///	<summary>
		///	The <c>SalesRepresentative</c> field.
		///	designer:fld/GVAV5/GVA16
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA16]")]
		public global::Epsitec.Cresus.Core.Entities.PeopleEntity SalesRepresentative
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PeopleEntity> ("[GVA16]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PeopleEntity oldValue = this.SalesRepresentative;
				if (oldValue != value || !this.IsFieldDefined("[GVA16]"))
				{
					this.OnSalesRepresentativeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PeopleEntity> ("[GVA16]", oldValue, value);
					this.OnSalesRepresentativeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultDebtorBookAccount</c> field.
		///	designer:fld/GVAV5/GVA36
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA36]")]
		public string DefaultDebtorBookAccount
		{
			get
			{
				return this.GetField<string> ("[GVA36]");
			}
			set
			{
				string oldValue = this.DefaultDebtorBookAccount;
				if (oldValue != value || !this.IsFieldDefined("[GVA36]"))
				{
					this.OnDefaultDebtorBookAccountChanging (oldValue, value);
					this.SetField<string> ("[GVA36]", oldValue, value);
					this.OnDefaultDebtorBookAccountChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>UnassignedPaymentTransactions</c> field.
		///	designer:fld/GVAV5/GVAM7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAM7]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.PaymentTransactionEntity> UnassignedPaymentTransactions
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.PaymentTransactionEntity> ("[GVAM7]");
			}
		}
		
		partial void OnCustomerCategoryChanging(global::Epsitec.Cresus.Core.Entities.CustomerCategoryEntity oldValue, global::Epsitec.Cresus.Core.Entities.CustomerCategoryEntity newValue);
		partial void OnCustomerCategoryChanged(global::Epsitec.Cresus.Core.Entities.CustomerCategoryEntity oldValue, global::Epsitec.Cresus.Core.Entities.CustomerCategoryEntity newValue);
		partial void OnMainRelationChanging(global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue, global::Epsitec.Cresus.Core.Entities.RelationEntity newValue);
		partial void OnMainRelationChanged(global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue, global::Epsitec.Cresus.Core.Entities.RelationEntity newValue);
		partial void OnSalesRepresentativeChanging(global::Epsitec.Cresus.Core.Entities.PeopleEntity oldValue, global::Epsitec.Cresus.Core.Entities.PeopleEntity newValue);
		partial void OnSalesRepresentativeChanged(global::Epsitec.Cresus.Core.Entities.PeopleEntity oldValue, global::Epsitec.Cresus.Core.Entities.PeopleEntity newValue);
		partial void OnDefaultDebtorBookAccountChanging(string oldValue, string newValue);
		partial void OnDefaultDebtorBookAccountChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.CustomerEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.CustomerEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 191);	// [GVAV5]
		public static readonly string EntityStructuredTypeKey = "[GVAV5]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<CustomerEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.FinanceSettings Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>FinanceSettings</c> entity.
	///	designer:cap/GVA76
	///	</summary>
	public partial class FinanceSettingsEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>IsrDefs</c> field.
		///	designer:fld/GVA76/GVA96
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA96]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.IsrDefinitionEntity> IsrDefs
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.IsrDefinitionEntity> ("[GVA96]");
			}
		}
		///	<summary>
		///	The <c>PaymentReminderDefs</c> field.
		///	designer:fld/GVA76/GVAA6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAA6]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.PaymentReminderDefinitionEntity> PaymentReminderDefs
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.PaymentReminderDefinitionEntity> ("[GVAA6]");
			}
		}
		///	<summary>
		///	The <c>PaymentCategories</c> field.
		///	designer:fld/GVA76/GVAB6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAB6]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity> PaymentCategories
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity> ("[GVAB6]");
			}
		}
		///	<summary>
		///	The <c>DefaultCurrencyCode</c> field.
		///	designer:fld/GVA76/GVAO7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAO7]")]
		public global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode? DefaultCurrencyCode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode?> ("[GVAO7]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode? oldValue = this.DefaultCurrencyCode;
				if (oldValue != value || !this.IsFieldDefined("[GVAO7]"))
				{
					this.OnDefaultCurrencyCodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode?> ("[GVAO7]", oldValue, value);
					this.OnDefaultCurrencyCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultPriceGroup</c> field.
		///	designer:fld/GVA76/GVAG8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAG8]")]
		public global::Epsitec.Cresus.Core.Entities.PriceGroupEntity DefaultPriceGroup
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PriceGroupEntity> ("[GVAG8]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PriceGroupEntity oldValue = this.DefaultPriceGroup;
				if (oldValue != value || !this.IsFieldDefined("[GVAG8]"))
				{
					this.OnDefaultPriceGroupChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PriceGroupEntity> ("[GVAG8]", oldValue, value);
					this.OnDefaultPriceGroupChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultDebtorBookAccount</c> field.
		///	designer:fld/GVA76/GVAS7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAS7]")]
		public string DefaultDebtorBookAccount
		{
			get
			{
				return this.GetField<string> ("[GVAS7]");
			}
			set
			{
				string oldValue = this.DefaultDebtorBookAccount;
				if (oldValue != value || !this.IsFieldDefined("[GVAS7]"))
				{
					this.OnDefaultDebtorBookAccountChanging (oldValue, value);
					this.SetField<string> ("[GVAS7]", oldValue, value);
					this.OnDefaultDebtorBookAccountChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SerializedChartsOfAccounts</c> field.
		///	designer:fld/GVA76/GVAC6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAC6]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.XmlBlobEntity> SerializedChartsOfAccounts
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.XmlBlobEntity> ("[GVAC6]");
			}
		}
		
		partial void OnDefaultCurrencyCodeChanging(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode? oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode? newValue);
		partial void OnDefaultCurrencyCodeChanged(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode? oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode? newValue);
		partial void OnDefaultPriceGroupChanging(global::Epsitec.Cresus.Core.Entities.PriceGroupEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceGroupEntity newValue);
		partial void OnDefaultPriceGroupChanged(global::Epsitec.Cresus.Core.Entities.PriceGroupEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceGroupEntity newValue);
		partial void OnDefaultDebtorBookAccountChanging(string oldValue, string newValue);
		partial void OnDefaultDebtorBookAccountChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.FinanceSettingsEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.FinanceSettingsEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 199);	// [GVA76]
		public static readonly string EntityStructuredTypeKey = "[GVA76]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<FinanceSettingsEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.PaymentReminderDefinition Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>PaymentReminderDefinition</c> entity.
	///	designer:cap/GVAD6
	///	</summary>
	public partial class PaymentReminderDefinitionEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/GVAD6/8VA3
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
		///	designer:fld/GVAD6/8VA5
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
		///	designer:fld/GVAD6/8VA7
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
		///	designer:fld/GVAD6/8VA8
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
		///	The <c>ExtraPaymentTerm</c> field.
		///	designer:fld/GVAD6/GVAE6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAE6]")]
		public int ExtraPaymentTerm
		{
			get
			{
				return this.GetField<int> ("[GVAE6]");
			}
			set
			{
				int oldValue = this.ExtraPaymentTerm;
				if (oldValue != value || !this.IsFieldDefined("[GVAE6]"))
				{
					this.OnExtraPaymentTermChanging (oldValue, value);
					this.SetField<int> ("[GVAE6]", oldValue, value);
					this.OnExtraPaymentTermChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AdministrativeTaxArticle</c> field.
		///	designer:fld/GVAD6/GVAF6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAF6]")]
		public global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity AdministrativeTaxArticle
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity> ("[GVAF6]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity oldValue = this.AdministrativeTaxArticle;
				if (oldValue != value || !this.IsFieldDefined("[GVAF6]"))
				{
					this.OnAdministrativeTaxArticleChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity> ("[GVAF6]", oldValue, value);
					this.OnAdministrativeTaxArticleChanged (oldValue, value);
				}
			}
		}
		
		partial void OnExtraPaymentTermChanging(int oldValue, int newValue);
		partial void OnExtraPaymentTermChanged(int oldValue, int newValue);
		partial void OnAdministrativeTaxArticleChanging(global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity oldValue, global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity newValue);
		partial void OnAdministrativeTaxArticleChanged(global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity oldValue, global::Epsitec.Cresus.Core.Entities.ArticleDefinitionEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.PaymentReminderDefinitionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.PaymentReminderDefinitionEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 205);	// [GVAD6]
		public static readonly string EntityStructuredTypeKey = "[GVAD6]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<PaymentReminderDefinitionEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.ArticleQuantityColumn Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ArticleQuantityColumn</c> entity.
	///	designer:cap/GVAL6
	///	</summary>
	public partial class ArticleQuantityColumnEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IItemCode, global::Epsitec.Cresus.Core.Entities.IItemRank, global::Epsitec.Cresus.Core.Entities.INameDescription
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/GVAL6/8VA1
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
		///	designer:fld/GVAL6/8VA3
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
		///	designer:fld/GVAL6/8VA5
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
		///	designer:fld/GVAL6/8VA7
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
		///	designer:fld/GVAL6/8VA8
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
		///	The <c>ShortName</c> field.
		///	designer:fld/GVAL6/GVAF9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAF9]")]
		public global::Epsitec.Common.Types.FormattedText ShortName
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAF9]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.ShortName;
				if (oldValue != value || !this.IsFieldDefined("[GVAF9]"))
				{
					this.OnShortNameChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAF9]", oldValue, value);
					this.OnShortNameChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>QuantityType</c> field.
		///	designer:fld/GVAL6/GVAM6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAM6]")]
		public global::Epsitec.Cresus.Core.Business.ArticleQuantityType QuantityType
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.ArticleQuantityType> ("[GVAM6]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.ArticleQuantityType oldValue = this.QuantityType;
				if (oldValue != value || !this.IsFieldDefined("[GVAM6]"))
				{
					this.OnQuantityTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.ArticleQuantityType> ("[GVAM6]", oldValue, value);
					this.OnQuantityTypeChanged (oldValue, value);
				}
			}
		}
		
		partial void OnShortNameChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnShortNameChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnQuantityTypeChanging(global::Epsitec.Cresus.Core.Business.ArticleQuantityType oldValue, global::Epsitec.Cresus.Core.Business.ArticleQuantityType newValue);
		partial void OnQuantityTypeChanged(global::Epsitec.Cresus.Core.Business.ArticleQuantityType oldValue, global::Epsitec.Cresus.Core.Business.ArticleQuantityType newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleQuantityColumnEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ArticleQuantityColumnEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 213);	// [GVAL6]
		public static readonly string EntityStructuredTypeKey = "[GVAL6]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ArticleQuantityColumnEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.CustomerCategory Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>CustomerCategory</c> entity.
	///	designer:cap/GVAN8
	///	</summary>
	public partial class CustomerCategoryEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/GVAN8/8VA3
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
		///	designer:fld/GVAN8/8VA5
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
		///	designer:fld/GVAN8/8VA7
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
		///	designer:fld/GVAN8/8VA8
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
		///	The <c>DefaultPaymentCategory</c> field.
		///	designer:fld/GVAN8/GVAR8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAR8]")]
		public global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity DefaultPaymentCategory
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity> ("[GVAR8]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity oldValue = this.DefaultPaymentCategory;
				if (oldValue != value || !this.IsFieldDefined("[GVAR8]"))
				{
					this.OnDefaultPaymentCategoryChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity> ("[GVAR8]", oldValue, value);
					this.OnDefaultPaymentCategoryChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Discounts</c> field.
		///	designer:fld/GVAN8/GVAQ8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAQ8]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity> Discounts
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity> ("[GVAQ8]");
			}
		}
		///	<summary>
		///	The <c>PriceGroup</c> field.
		///	designer:fld/GVAN8/GVAP8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAP8]")]
		public global::Epsitec.Cresus.Core.Entities.PriceGroupEntity PriceGroup
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PriceGroupEntity> ("[GVAP8]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PriceGroupEntity oldValue = this.PriceGroup;
				if (oldValue != value || !this.IsFieldDefined("[GVAP8]"))
				{
					this.OnPriceGroupChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PriceGroupEntity> ("[GVAP8]", oldValue, value);
					this.OnPriceGroupChanged (oldValue, value);
				}
			}
		}
		
		partial void OnDefaultPaymentCategoryChanging(global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity oldValue, global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity newValue);
		partial void OnDefaultPaymentCategoryChanged(global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity oldValue, global::Epsitec.Cresus.Core.Entities.PaymentCategoryEntity newValue);
		partial void OnPriceGroupChanging(global::Epsitec.Cresus.Core.Entities.PriceGroupEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceGroupEntity newValue);
		partial void OnPriceGroupChanged(global::Epsitec.Cresus.Core.Entities.PriceGroupEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceGroupEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.CustomerCategoryEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.CustomerCategoryEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 279);	// [GVAN8]
		public static readonly string EntityStructuredTypeKey = "[GVAN8]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<CustomerCategoryEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

