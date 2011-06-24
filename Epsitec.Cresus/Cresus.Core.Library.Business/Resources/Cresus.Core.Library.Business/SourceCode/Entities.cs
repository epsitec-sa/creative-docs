//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[GVA]", typeof (Epsitec.Cresus.Core.Entities.RelationEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVA1]", typeof (Epsitec.Cresus.Core.Entities.AffairEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVAM]", typeof (Epsitec.Cresus.Core.Entities.PeopleEntity))]
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
		///	The <c>DefaultAddress</c> field.
		///	designer:fld/GVA/GVAB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAB]")]
		public global::Epsitec.Cresus.Core.Entities.AddressEntity DefaultAddress
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.AddressEntity> ("[GVAB]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.AddressEntity oldValue = this.DefaultAddress;
				if (oldValue != value || !this.IsFieldDefined("[GVAB]"))
				{
					this.OnDefaultAddressChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.AddressEntity> ("[GVAB]", oldValue, value);
					this.OnDefaultAddressChanged (oldValue, value);
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
		partial void OnDefaultAddressChanging(global::Epsitec.Cresus.Core.Entities.AddressEntity oldValue, global::Epsitec.Cresus.Core.Entities.AddressEntity newValue);
		partial void OnDefaultAddressChanged(global::Epsitec.Cresus.Core.Entities.AddressEntity oldValue, global::Epsitec.Cresus.Core.Entities.AddressEntity newValue);
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
	public partial class AffairEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IReferenceNumber, global::Epsitec.Cresus.Core.Entities.IWorkflowHost, global::Epsitec.Cresus.Core.Entities.IBusinessLink, global::Epsitec.Cresus.Core.Entities.INameDescription, global::Epsitec.Cresus.Core.Entities.IComments
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
		///	The <c>DefaultDebtorBookAccount</c> field.
		///	designer:fld/GVA1/GVA6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA6]")]
		public string DefaultDebtorBookAccount
		{
			get
			{
				return this.GetField<string> ("[GVA6]");
			}
			set
			{
				string oldValue = this.DefaultDebtorBookAccount;
				if (oldValue != value || !this.IsFieldDefined("[GVA6]"))
				{
					this.OnDefaultDebtorBookAccountChanging (oldValue, value);
					this.SetField<string> ("[GVA6]", oldValue, value);
					this.OnDefaultDebtorBookAccountChanged (oldValue, value);
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
		///	designer:fld/GVA1/GVA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA8]")]
		public global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity ActiveAffairOwner
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity> ("[GVA8]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity oldValue = this.ActiveAffairOwner;
				if (oldValue != value || !this.IsFieldDefined("[GVA8]"))
				{
					this.OnActiveAffairOwnerChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity> ("[GVA8]", oldValue, value);
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
		
		partial void OnCustomerChanging(global::Epsitec.Cresus.Core.Entities.CustomerEntity oldValue, global::Epsitec.Cresus.Core.Entities.CustomerEntity newValue);
		partial void OnCustomerChanged(global::Epsitec.Cresus.Core.Entities.CustomerEntity oldValue, global::Epsitec.Cresus.Core.Entities.CustomerEntity newValue);
		partial void OnDefaultDebtorBookAccountChanging(string oldValue, string newValue);
		partial void OnDefaultDebtorBookAccountChanged(string oldValue, string newValue);
		partial void OnActiveSalesRepresentativeChanging(global::Epsitec.Cresus.Core.Entities.PeopleEntity oldValue, global::Epsitec.Cresus.Core.Entities.PeopleEntity newValue);
		partial void OnActiveSalesRepresentativeChanged(global::Epsitec.Cresus.Core.Entities.PeopleEntity oldValue, global::Epsitec.Cresus.Core.Entities.PeopleEntity newValue);
		partial void OnActiveAffairOwnerChanging(global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity newValue);
		partial void OnActiveAffairOwnerChanged(global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity newValue);
		
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

#region Epsitec.Cresus.Core.People Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>People</c> entity.
	///	designer:cap/GVAM
	///	</summary>
	public partial class PeopleEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IReferenceNumber
	{
		#region IReferenceNumber Members
		///	<summary>
		///	The <c>IdA</c> field.
		///	designer:fld/GVAM/8VA11
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
		///	designer:fld/GVAM/8VA3
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
		#region IReferenceNumber Members
		///	<summary>
		///	The <c>IdB</c> field.
		///	designer:fld/GVAM/8VA21
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
		///	designer:fld/GVAM/8VA31
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
		///	The <c>Person</c> field.
		///	designer:fld/GVAM/GVAN
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAN]")]
		public global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity Person
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity> ("[GVAN]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity oldValue = this.Person;
				if (oldValue != value || !this.IsFieldDefined("[GVAN]"))
				{
					this.OnPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity> ("[GVAN]", oldValue, value);
					this.OnPersonChanged (oldValue, value);
				}
			}
		}
		
		partial void OnPersonChanging(global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity newValue);
		partial void OnPersonChanged(global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.PeopleEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.PeopleEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 22);	// [GVAM]
		public static readonly string EntityStructuredTypeKey = "[GVAM]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<PeopleEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
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
		
		partial void OnCompanyChanging(global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue, global::Epsitec.Cresus.Core.Entities.RelationEntity newValue);
		partial void OnCompanyChanged(global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue, global::Epsitec.Cresus.Core.Entities.RelationEntity newValue);
		partial void OnFinanceChanging(global::Epsitec.Cresus.Core.Entities.FinanceSettingsEntity oldValue, global::Epsitec.Cresus.Core.Entities.FinanceSettingsEntity newValue);
		partial void OnFinanceChanged(global::Epsitec.Cresus.Core.Entities.FinanceSettingsEntity oldValue, global::Epsitec.Cresus.Core.Entities.FinanceSettingsEntity newValue);
		partial void OnTaxChanging(global::Epsitec.Cresus.Core.Entities.TaxSettingsEntity oldValue, global::Epsitec.Cresus.Core.Entities.TaxSettingsEntity newValue);
		partial void OnTaxChanged(global::Epsitec.Cresus.Core.Entities.TaxSettingsEntity oldValue, global::Epsitec.Cresus.Core.Entities.TaxSettingsEntity newValue);
		partial void OnCompanyLogoChanging(global::Epsitec.Cresus.Core.Entities.ImageEntity oldValue, global::Epsitec.Cresus.Core.Entities.ImageEntity newValue);
		partial void OnCompanyLogoChanged(global::Epsitec.Cresus.Core.Entities.ImageEntity oldValue, global::Epsitec.Cresus.Core.Entities.ImageEntity newValue);
		
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
		///	The <c>BillingDetails</c> field.
		///	designer:fld/GVAT/GVA51
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA51]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.BillingDetailEntity> BillingDetails
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.BillingDetailEntity> ("[GVA51]");
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
		public global::Epsitec.Common.Types.Date? PriceRefDate
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[GVA91]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.PriceRefDate;
				if (oldValue != value || !this.IsFieldDefined("[GVA91]"))
				{
					this.OnPriceRefDateChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[GVA91]", oldValue, value);
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
		partial void OnPriceRefDateChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnPriceRefDateChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnPriceGroupChanging(global::Epsitec.Cresus.Core.Entities.PriceGroupEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceGroupEntity newValue);
		partial void OnPriceGroupChanged(global::Epsitec.Cresus.Core.Entities.PriceGroupEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceGroupEntity newValue);
		partial void OnDebtorBookAccountChanging(string oldValue, string newValue);
		partial void OnDebtorBookAccountChanged(string oldValue, string newValue);
		
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
		///	The <c>Visibility</c> field.
		///	designer:fld/GVA31/GVAD1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAD1]")]
		public bool Visibility
		{
			get
			{
				return this.GetField<bool> ("[GVAD1]");
			}
			set
			{
				bool oldValue = this.Visibility;
				if (oldValue != value || !this.IsFieldDefined("[GVAD1]"))
				{
					this.OnVisibilityChanging (oldValue, value);
					this.SetField<bool> ("[GVAD1]", oldValue, value);
					this.OnVisibilityChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AutoGenerated</c> field.
		///	designer:fld/GVA31/GVAE1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAE1]")]
		public bool AutoGenerated
		{
			get
			{
				return this.GetField<bool> ("[GVAE1]");
			}
			set
			{
				bool oldValue = this.AutoGenerated;
				if (oldValue != value || !this.IsFieldDefined("[GVAE1]"))
				{
					this.OnAutoGeneratedChanging (oldValue, value);
					this.SetField<bool> ("[GVAE1]", oldValue, value);
					this.OnAutoGeneratedChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>GroupLevel</c> field.
		///	designer:fld/GVA31/GVAF1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAF1]")]
		public int GroupLevel
		{
			get
			{
				return this.GetField<int> ("[GVAF1]");
			}
			set
			{
				int oldValue = this.GroupLevel;
				if (oldValue != value || !this.IsFieldDefined("[GVAF1]"))
				{
					this.OnGroupLevelChanging (oldValue, value);
					this.SetField<int> ("[GVAF1]", oldValue, value);
					this.OnGroupLevelChanged (oldValue, value);
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
		
		partial void OnVisibilityChanging(bool oldValue, bool newValue);
		partial void OnVisibilityChanged(bool oldValue, bool newValue);
		partial void OnAutoGeneratedChanging(bool oldValue, bool newValue);
		partial void OnAutoGeneratedChanged(bool oldValue, bool newValue);
		partial void OnGroupLevelChanging(int oldValue, int newValue);
		partial void OnGroupLevelChanged(int oldValue, int newValue);
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
		///	The <c>VatCode</c> field.
		///	designer:fld/GVAC1/GVA02
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA02]")]
		public global::Epsitec.Cresus.Core.Business.Finance.VatCode VatCode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.VatCode> ("[GVA02]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.VatCode oldValue = this.VatCode;
				if (oldValue != value || !this.IsFieldDefined("[GVA02]"))
				{
					this.OnVatCodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.VatCode> ("[GVA02]", oldValue, value);
					this.OnVatCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PrimaryUnitPriceBeforeTax</c> field.
		///	designer:fld/GVAC1/GVA12
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA12]")]
		public global::System.Decimal PrimaryUnitPriceBeforeTax
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[GVA12]");
			}
			set
			{
				global::System.Decimal oldValue = this.PrimaryUnitPriceBeforeTax;
				if (oldValue != value || !this.IsFieldDefined("[GVA12]"))
				{
					this.OnPrimaryUnitPriceBeforeTaxChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[GVA12]", oldValue, value);
					this.OnPrimaryUnitPriceBeforeTaxChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PrimaryLinePriceBeforeTax</c> field.
		///	designer:fld/GVAC1/GVA22
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA22]")]
		public global::System.Decimal PrimaryLinePriceBeforeTax
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[GVA22]");
			}
			set
			{
				global::System.Decimal oldValue = this.PrimaryLinePriceBeforeTax;
				if (oldValue != value || !this.IsFieldDefined("[GVA22]"))
				{
					this.OnPrimaryLinePriceBeforeTaxChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[GVA22]", oldValue, value);
					this.OnPrimaryLinePriceBeforeTaxChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PrimaryLinePriceAfterTax</c> field.
		///	designer:fld/GVAC1/GVA32
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA32]")]
		public global::System.Decimal? PrimaryLinePriceAfterTax
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVA32]");
			}
			set
			{
				global::System.Decimal? oldValue = this.PrimaryLinePriceAfterTax;
				if (oldValue != value || !this.IsFieldDefined("[GVA32]"))
				{
					this.OnPrimaryLinePriceAfterTaxChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVA32]", oldValue, value);
					this.OnPrimaryLinePriceAfterTaxChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>NeverApplyDiscount</c> field.
		///	designer:fld/GVAC1/GVA42
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA42]")]
		public bool NeverApplyDiscount
		{
			get
			{
				return this.GetField<bool> ("[GVA42]");
			}
			set
			{
				bool oldValue = this.NeverApplyDiscount;
				if (oldValue != value || !this.IsFieldDefined("[GVA42]"))
				{
					this.OnNeverApplyDiscountChanging (oldValue, value);
					this.SetField<bool> ("[GVA42]", oldValue, value);
					this.OnNeverApplyDiscountChanged (oldValue, value);
				}
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
		///	The <c>TaxRate1</c> field.
		///	designer:fld/GVAC1/GVA62
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA62]")]
		public global::System.Decimal? TaxRate1
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVA62]");
			}
			set
			{
				global::System.Decimal? oldValue = this.TaxRate1;
				if (oldValue != value || !this.IsFieldDefined("[GVA62]"))
				{
					this.OnTaxRate1Changing (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVA62]", oldValue, value);
					this.OnTaxRate1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>TaxRate2</c> field.
		///	designer:fld/GVAC1/GVA72
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA72]")]
		public global::System.Decimal? TaxRate2
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVA72]");
			}
			set
			{
				global::System.Decimal? oldValue = this.TaxRate2;
				if (oldValue != value || !this.IsFieldDefined("[GVA72]"))
				{
					this.OnTaxRate2Changing (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVA72]", oldValue, value);
					this.OnTaxRate2Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>FixedLinePrice</c> field.
		///	designer:fld/GVAC1/GVA82
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA82]")]
		public global::System.Decimal? FixedLinePrice
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVA82]");
			}
			set
			{
				global::System.Decimal? oldValue = this.FixedLinePrice;
				if (oldValue != value || !this.IsFieldDefined("[GVA82]"))
				{
					this.OnFixedLinePriceChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVA82]", oldValue, value);
					this.OnFixedLinePriceChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>FixedLinePriceIncludesTaxes</c> field.
		///	designer:fld/GVAC1/GVA92
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA92]")]
		public bool FixedLinePriceIncludesTaxes
		{
			get
			{
				return this.GetField<bool> ("[GVA92]");
			}
			set
			{
				bool oldValue = this.FixedLinePriceIncludesTaxes;
				if (oldValue != value || !this.IsFieldDefined("[GVA92]"))
				{
					this.OnFixedLinePriceIncludesTaxesChanging (oldValue, value);
					this.SetField<bool> ("[GVA92]", oldValue, value);
					this.OnFixedLinePriceIncludesTaxesChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ResultingLinePriceBeforeTax</c> field.
		///	designer:fld/GVAC1/GVAA2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAA2]")]
		public global::System.Decimal? ResultingLinePriceBeforeTax
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAA2]");
			}
			set
			{
				global::System.Decimal? oldValue = this.ResultingLinePriceBeforeTax;
				if (oldValue != value || !this.IsFieldDefined("[GVAA2]"))
				{
					this.OnResultingLinePriceBeforeTaxChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAA2]", oldValue, value);
					this.OnResultingLinePriceBeforeTaxChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ResultingLineTax1</c> field.
		///	designer:fld/GVAC1/GVAB2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAB2]")]
		public global::System.Decimal? ResultingLineTax1
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAB2]");
			}
			set
			{
				global::System.Decimal? oldValue = this.ResultingLineTax1;
				if (oldValue != value || !this.IsFieldDefined("[GVAB2]"))
				{
					this.OnResultingLineTax1Changing (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAB2]", oldValue, value);
					this.OnResultingLineTax1Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ResultingLineTax2</c> field.
		///	designer:fld/GVAC1/GVAC2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAC2]")]
		public global::System.Decimal? ResultingLineTax2
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAC2]");
			}
			set
			{
				global::System.Decimal? oldValue = this.ResultingLineTax2;
				if (oldValue != value || !this.IsFieldDefined("[GVAC2]"))
				{
					this.OnResultingLineTax2Changing (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAC2]", oldValue, value);
					this.OnResultingLineTax2Changed (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>FinalLinePriceBeforeTax</c> field.
		///	designer:fld/GVAC1/GVAD2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAD2]")]
		public global::System.Decimal? FinalLinePriceBeforeTax
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAD2]");
			}
			set
			{
				global::System.Decimal? oldValue = this.FinalLinePriceBeforeTax;
				if (oldValue != value || !this.IsFieldDefined("[GVAD2]"))
				{
					this.OnFinalLinePriceBeforeTaxChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAD2]", oldValue, value);
					this.OnFinalLinePriceBeforeTaxChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ArticleShortDescriptionCache</c> field.
		///	designer:fld/GVAC1/GVAE2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAE2]")]
		public global::Epsitec.Common.Types.FormattedText ArticleShortDescriptionCache
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAE2]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.ArticleShortDescriptionCache;
				if (oldValue != value || !this.IsFieldDefined("[GVAE2]"))
				{
					this.OnArticleShortDescriptionCacheChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAE2]", oldValue, value);
					this.OnArticleShortDescriptionCacheChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ArticleLongDescriptionCache</c> field.
		///	designer:fld/GVAC1/GVAF2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAF2]")]
		public global::Epsitec.Common.Types.FormattedText ArticleLongDescriptionCache
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAF2]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.ArticleLongDescriptionCache;
				if (oldValue != value || !this.IsFieldDefined("[GVAF2]"))
				{
					this.OnArticleLongDescriptionCacheChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAF2]", oldValue, value);
					this.OnArticleLongDescriptionCacheChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ReplacementText</c> field.
		///	designer:fld/GVAC1/GVAG2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAG2]")]
		public global::Epsitec.Common.Types.FormattedText ReplacementText
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAG2]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.ReplacementText;
				if (oldValue != value || !this.IsFieldDefined("[GVAG2]"))
				{
					this.OnReplacementTextChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAG2]", oldValue, value);
					this.OnReplacementTextChanged (oldValue, value);
				}
			}
		}
		
		partial void OnVatCodeChanging(global::Epsitec.Cresus.Core.Business.Finance.VatCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.VatCode newValue);
		partial void OnVatCodeChanged(global::Epsitec.Cresus.Core.Business.Finance.VatCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.VatCode newValue);
		partial void OnPrimaryUnitPriceBeforeTaxChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnPrimaryUnitPriceBeforeTaxChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnPrimaryLinePriceBeforeTaxChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnPrimaryLinePriceBeforeTaxChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnPrimaryLinePriceAfterTaxChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnPrimaryLinePriceAfterTaxChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnNeverApplyDiscountChanging(bool oldValue, bool newValue);
		partial void OnNeverApplyDiscountChanged(bool oldValue, bool newValue);
		partial void OnTaxRate1Changing(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnTaxRate1Changed(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnTaxRate2Changing(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnTaxRate2Changed(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnFixedLinePriceChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnFixedLinePriceChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnFixedLinePriceIncludesTaxesChanging(bool oldValue, bool newValue);
		partial void OnFixedLinePriceIncludesTaxesChanged(bool oldValue, bool newValue);
		partial void OnResultingLinePriceBeforeTaxChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnResultingLinePriceBeforeTaxChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnResultingLineTax1Changing(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnResultingLineTax1Changed(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnResultingLineTax2Changing(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnResultingLineTax2Changed(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnFinalLinePriceBeforeTaxChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnFinalLinePriceBeforeTaxChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnArticleShortDescriptionCacheChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnArticleShortDescriptionCacheChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnArticleLongDescriptionCacheChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnArticleLongDescriptionCacheChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnReplacementTextChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnReplacementTextChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		
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
		///	The <c>InputVatCode</c> field.
		///	designer:fld/GVAK1/GVAI2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAI2]")]
		public global::Epsitec.Cresus.Core.Business.Finance.VatCode? InputVatCode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.VatCode?> ("[GVAI2]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.VatCode? oldValue = this.InputVatCode;
				if (oldValue != value || !this.IsFieldDefined("[GVAI2]"))
				{
					this.OnInputVatCodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.VatCode?> ("[GVAI2]", oldValue, value);
					this.OnInputVatCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>OutputVatCode</c> field.
		///	designer:fld/GVAK1/GVAJ2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAJ2]")]
		public global::Epsitec.Cresus.Core.Business.Finance.VatCode? OutputVatCode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.VatCode?> ("[GVAJ2]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.VatCode? oldValue = this.OutputVatCode;
				if (oldValue != value || !this.IsFieldDefined("[GVAJ2]"))
				{
					this.OnOutputVatCodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.VatCode?> ("[GVAJ2]", oldValue, value);
					this.OnOutputVatCodeChanged (oldValue, value);
				}
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
		///	The <c>Accounting</c> field.
		///	designer:fld/GVAK1/GVAN3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAN3]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.ArticleAccountingDefinitionEntity> Accounting
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.ArticleAccountingDefinitionEntity> ("[GVAN3]");
			}
		}
		///	<summary>
		///	The <c>BillingUnit</c> field.
		///	designer:fld/GVAK1/GVAO3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAO3]")]
		public global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity BillingUnit
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity> ("[GVAO3]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity oldValue = this.BillingUnit;
				if (oldValue != value || !this.IsFieldDefined("[GVAO3]"))
				{
					this.OnBillingUnitChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity> ("[GVAO3]", oldValue, value);
					this.OnBillingUnitChanged (oldValue, value);
				}
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
		
		partial void OnInputVatCodeChanging(global::Epsitec.Cresus.Core.Business.Finance.VatCode? oldValue, global::Epsitec.Cresus.Core.Business.Finance.VatCode? newValue);
		partial void OnInputVatCodeChanged(global::Epsitec.Cresus.Core.Business.Finance.VatCode? oldValue, global::Epsitec.Cresus.Core.Business.Finance.VatCode? newValue);
		partial void OnOutputVatCodeChanging(global::Epsitec.Cresus.Core.Business.Finance.VatCode? oldValue, global::Epsitec.Cresus.Core.Business.Finance.VatCode? newValue);
		partial void OnOutputVatCodeChanged(global::Epsitec.Cresus.Core.Business.Finance.VatCode? oldValue, global::Epsitec.Cresus.Core.Business.Finance.VatCode? newValue);
		partial void OnArticleCategoryChanging(global::Epsitec.Cresus.Core.Entities.ArticleCategoryEntity oldValue, global::Epsitec.Cresus.Core.Entities.ArticleCategoryEntity newValue);
		partial void OnArticleCategoryChanged(global::Epsitec.Cresus.Core.Entities.ArticleCategoryEntity oldValue, global::Epsitec.Cresus.Core.Entities.ArticleCategoryEntity newValue);
		partial void OnBillingUnitChanging(global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity oldValue, global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity newValue);
		partial void OnBillingUnitChanged(global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity oldValue, global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity newValue);
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
	public partial class ArticleQuantityEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Quantity</c> field.
		///	designer:fld/GVAP1/GVAQ1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAQ1]")]
		public global::System.Decimal Quantity
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[GVAQ1]");
			}
			set
			{
				global::System.Decimal oldValue = this.Quantity;
				if (oldValue != value || !this.IsFieldDefined("[GVAQ1]"))
				{
					this.OnQuantityChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[GVAQ1]", oldValue, value);
					this.OnQuantityChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>QuantityType</c> field.
		///	designer:fld/GVAP1/GVA75
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA75]")]
		public global::Epsitec.Cresus.Core.Business.ArticleQuantityType QuantityType
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.ArticleQuantityType> ("[GVA75]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.ArticleQuantityType oldValue = this.QuantityType;
				if (oldValue != value || !this.IsFieldDefined("[GVA75]"))
				{
					this.OnQuantityTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.ArticleQuantityType> ("[GVA75]", oldValue, value);
					this.OnQuantityTypeChanged (oldValue, value);
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
		///	The <c>ExpectedDate</c> field.
		///	designer:fld/GVAP1/GVAS1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAS1]")]
		public global::Epsitec.Common.Types.Date? ExpectedDate
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.Date?> ("[GVAS1]");
			}
			set
			{
				global::Epsitec.Common.Types.Date? oldValue = this.ExpectedDate;
				if (oldValue != value || !this.IsFieldDefined("[GVAS1]"))
				{
					this.OnExpectedDateChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.Date?> ("[GVAS1]", oldValue, value);
					this.OnExpectedDateChanged (oldValue, value);
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
		partial void OnQuantityTypeChanging(global::Epsitec.Cresus.Core.Business.ArticleQuantityType oldValue, global::Epsitec.Cresus.Core.Business.ArticleQuantityType newValue);
		partial void OnQuantityTypeChanged(global::Epsitec.Cresus.Core.Business.ArticleQuantityType oldValue, global::Epsitec.Cresus.Core.Business.ArticleQuantityType newValue);
		partial void OnUnitChanging(global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity oldValue, global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity newValue);
		partial void OnUnitChanged(global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity oldValue, global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity newValue);
		partial void OnExpectedDateChanging(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		partial void OnExpectedDateChanged(global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
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
		///	The <c>DefaultBillingUnit</c> field.
		///	designer:fld/GVAO2/GVAQ3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAQ3]")]
		public global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity DefaultBillingUnit
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity> ("[GVAQ3]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity oldValue = this.DefaultBillingUnit;
				if (oldValue != value || !this.IsFieldDefined("[GVAQ3]"))
				{
					this.OnDefaultBillingUnitChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity> ("[GVAQ3]", oldValue, value);
					this.OnDefaultBillingUnitChanged (oldValue, value);
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
		///	The <c>DefaultInputVatCode</c> field.
		///	designer:fld/GVAO2/GVAR2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAR2]")]
		public global::Epsitec.Cresus.Core.Business.Finance.VatCode? DefaultInputVatCode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.VatCode?> ("[GVAR2]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.VatCode? oldValue = this.DefaultInputVatCode;
				if (oldValue != value || !this.IsFieldDefined("[GVAR2]"))
				{
					this.OnDefaultInputVatCodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.VatCode?> ("[GVAR2]", oldValue, value);
					this.OnDefaultInputVatCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultOutputVatCode</c> field.
		///	designer:fld/GVAO2/GVAS2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAS2]")]
		public global::Epsitec.Cresus.Core.Business.Finance.VatCode? DefaultOutputVatCode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.VatCode?> ("[GVAS2]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.VatCode? oldValue = this.DefaultOutputVatCode;
				if (oldValue != value || !this.IsFieldDefined("[GVAS2]"))
				{
					this.OnDefaultOutputVatCodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.VatCode?> ("[GVAS2]", oldValue, value);
					this.OnDefaultOutputVatCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>VatNumber</c> field.
		///	designer:fld/GVAO2/GVAT2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAT2]")]
		public string VatNumber
		{
			get
			{
				return this.GetField<string> ("[GVAT2]");
			}
			set
			{
				string oldValue = this.VatNumber;
				if (oldValue != value || !this.IsFieldDefined("[GVAT2]"))
				{
					this.OnVatNumberChanging (oldValue, value);
					this.SetField<string> ("[GVAT2]", oldValue, value);
					this.OnVatNumberChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultAccounting</c> field.
		///	designer:fld/GVAO2/GVA23
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA23]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.ArticleAccountingDefinitionEntity> DefaultAccounting
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.ArticleAccountingDefinitionEntity> ("[GVA23]");
			}
		}
		///	<summary>
		///	The <c>DefaultRoundingMode</c> field.
		///	designer:fld/GVAO2/GVAU2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAU2]")]
		public global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity DefaultRoundingMode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity> ("[GVAU2]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity oldValue = this.DefaultRoundingMode;
				if (oldValue != value || !this.IsFieldDefined("[GVAU2]"))
				{
					this.OnDefaultRoundingModeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity> ("[GVAU2]", oldValue, value);
					this.OnDefaultRoundingModeChanged (oldValue, value);
				}
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
		
		partial void OnDefaultBillingUnitChanging(global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity oldValue, global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity newValue);
		partial void OnDefaultBillingUnitChanged(global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity oldValue, global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity newValue);
		partial void OnDefaultInputVatCodeChanging(global::Epsitec.Cresus.Core.Business.Finance.VatCode? oldValue, global::Epsitec.Cresus.Core.Business.Finance.VatCode? newValue);
		partial void OnDefaultInputVatCodeChanged(global::Epsitec.Cresus.Core.Business.Finance.VatCode? oldValue, global::Epsitec.Cresus.Core.Business.Finance.VatCode? newValue);
		partial void OnDefaultOutputVatCodeChanging(global::Epsitec.Cresus.Core.Business.Finance.VatCode? oldValue, global::Epsitec.Cresus.Core.Business.Finance.VatCode? newValue);
		partial void OnDefaultOutputVatCodeChanged(global::Epsitec.Cresus.Core.Business.Finance.VatCode? oldValue, global::Epsitec.Cresus.Core.Business.Finance.VatCode? newValue);
		partial void OnVatNumberChanging(string oldValue, string newValue);
		partial void OnVatNumberChanged(string oldValue, string newValue);
		partial void OnDefaultRoundingModeChanging(global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity newValue);
		partial void OnDefaultRoundingModeChanged(global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceRoundingModeEntity newValue);
		partial void OnNeverApplyDiscountChanging(bool oldValue, bool newValue);
		partial void OnNeverApplyDiscountChanged(bool oldValue, bool newValue);
		partial void OnArticleTypeChanging(global::Epsitec.Cresus.Core.Business.ArticleType oldValue, global::Epsitec.Cresus.Core.Business.ArticleType newValue);
		partial void OnArticleTypeChanged(global::Epsitec.Cresus.Core.Business.ArticleType oldValue, global::Epsitec.Cresus.Core.Business.ArticleType newValue);
		
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
	public partial class ArticleAccountingDefinitionEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.IDateRange, global::Epsitec.Cresus.Core.Entities.IBusinessLink
	{
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
		#region IBusinessLink Members
		///	<summary>
		///	The <c>BusinessCodeVector</c> field.
		///	designer:fld/GVA13/GVA5
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
		///	The <c>SellingBookAccount</c> field.
		///	designer:fld/GVA13/GVA33
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA33]")]
		public string SellingBookAccount
		{
			get
			{
				return this.GetField<string> ("[GVA33]");
			}
			set
			{
				string oldValue = this.SellingBookAccount;
				if (oldValue != value || !this.IsFieldDefined("[GVA33]"))
				{
					this.OnSellingBookAccountChanging (oldValue, value);
					this.SetField<string> ("[GVA33]", oldValue, value);
					this.OnSellingBookAccountChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SellingDiscountBookAccount</c> field.
		///	designer:fld/GVA13/GVA43
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA43]")]
		public string SellingDiscountBookAccount
		{
			get
			{
				return this.GetField<string> ("[GVA43]");
			}
			set
			{
				string oldValue = this.SellingDiscountBookAccount;
				if (oldValue != value || !this.IsFieldDefined("[GVA43]"))
				{
					this.OnSellingDiscountBookAccountChanging (oldValue, value);
					this.SetField<string> ("[GVA43]", oldValue, value);
					this.OnSellingDiscountBookAccountChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PurchaseBookAccount</c> field.
		///	designer:fld/GVA13/GVA53
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA53]")]
		public string PurchaseBookAccount
		{
			get
			{
				return this.GetField<string> ("[GVA53]");
			}
			set
			{
				string oldValue = this.PurchaseBookAccount;
				if (oldValue != value || !this.IsFieldDefined("[GVA53]"))
				{
					this.OnPurchaseBookAccountChanging (oldValue, value);
					this.SetField<string> ("[GVA53]", oldValue, value);
					this.OnPurchaseBookAccountChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PurchaseDiscountBookAccount</c> field.
		///	designer:fld/GVA13/GVA63
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA63]")]
		public string PurchaseDiscountBookAccount
		{
			get
			{
				return this.GetField<string> ("[GVA63]");
			}
			set
			{
				string oldValue = this.PurchaseDiscountBookAccount;
				if (oldValue != value || !this.IsFieldDefined("[GVA63]"))
				{
					this.OnPurchaseDiscountBookAccountChanging (oldValue, value);
					this.SetField<string> ("[GVA63]", oldValue, value);
					this.OnPurchaseDiscountBookAccountChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CurrencyCode</c> field.
		///	designer:fld/GVA13/GVA73
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA73]")]
		public global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode? CurrencyCode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode?> ("[GVA73]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode? oldValue = this.CurrencyCode;
				if (oldValue != value || !this.IsFieldDefined("[GVA73]"))
				{
					this.OnCurrencyCodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode?> ("[GVA73]", oldValue, value);
					this.OnCurrencyCodeChanged (oldValue, value);
				}
			}
		}
		
		partial void OnSellingBookAccountChanging(string oldValue, string newValue);
		partial void OnSellingBookAccountChanged(string oldValue, string newValue);
		partial void OnSellingDiscountBookAccountChanging(string oldValue, string newValue);
		partial void OnSellingDiscountBookAccountChanged(string oldValue, string newValue);
		partial void OnPurchaseBookAccountChanging(string oldValue, string newValue);
		partial void OnPurchaseBookAccountChanged(string oldValue, string newValue);
		partial void OnPurchaseDiscountBookAccountChanging(string oldValue, string newValue);
		partial void OnPurchaseDiscountBookAccountChanged(string oldValue, string newValue);
		partial void OnCurrencyCodeChanging(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode? oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode? newValue);
		partial void OnCurrencyCodeChanged(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode? oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode? newValue);
		
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
		public global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode? CurrencyCode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode?> ("[GVAF3]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode? oldValue = this.CurrencyCode;
				if (oldValue != value || !this.IsFieldDefined("[GVAF3]"))
				{
					this.OnCurrencyCodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode?> ("[GVAF3]", oldValue, value);
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
		///	The <c>ValueIncludesTaxes</c> field.
		///	designer:fld/GVAA3/GVAH3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAH3]")]
		public bool ValueIncludesTaxes
		{
			get
			{
				return this.GetField<bool> ("[GVAH3]");
			}
			set
			{
				bool oldValue = this.ValueIncludesTaxes;
				if (oldValue != value || !this.IsFieldDefined("[GVAH3]"))
				{
					this.OnValueIncludesTaxesChanging (oldValue, value);
					this.SetField<bool> ("[GVAH3]", oldValue, value);
					this.OnValueIncludesTaxesChanged (oldValue, value);
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
		
		partial void OnMinQuantityChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnMinQuantityChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnMaxQuantityChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnMaxQuantityChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnCurrencyCodeChanging(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode? oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode? newValue);
		partial void OnCurrencyCodeChanged(global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode? oldValue, global::Epsitec.Cresus.Core.Business.Finance.CurrencyCode? newValue);
		partial void OnValueChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnValueChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnValueIncludesTaxesChanging(bool oldValue, bool newValue);
		partial void OnValueIncludesTaxesChanged(bool oldValue, bool newValue);
		partial void OnValueOverridesPriceGroupChanging(bool oldValue, bool newValue);
		partial void OnValueOverridesPriceGroupChanged(bool oldValue, bool newValue);
		
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
		///	The <c>TextForFixedPrice</c> field.
		///	designer:fld/GVAF4/GVAJ4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAJ4]")]
		public global::Epsitec.Common.Types.FormattedText TextForFixedPrice
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[GVAJ4]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.TextForFixedPrice;
				if (oldValue != value || !this.IsFieldDefined("[GVAJ4]"))
				{
					this.OnTextForFixedPriceChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[GVAJ4]", oldValue, value);
					this.OnTextForFixedPriceChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PrimaryPriceBeforeTax</c> field.
		///	designer:fld/GVAF4/GVAK4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAK4]")]
		public global::System.Decimal? PrimaryPriceBeforeTax
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAK4]");
			}
			set
			{
				global::System.Decimal? oldValue = this.PrimaryPriceBeforeTax;
				if (oldValue != value || !this.IsFieldDefined("[GVAK4]"))
				{
					this.OnPrimaryPriceBeforeTaxChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAK4]", oldValue, value);
					this.OnPrimaryPriceBeforeTaxChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PrimaryTax</c> field.
		///	designer:fld/GVAF4/GVAL4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAL4]")]
		public global::System.Decimal? PrimaryTax
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAL4]");
			}
			set
			{
				global::System.Decimal? oldValue = this.PrimaryTax;
				if (oldValue != value || !this.IsFieldDefined("[GVAL4]"))
				{
					this.OnPrimaryTaxChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAL4]", oldValue, value);
					this.OnPrimaryTaxChanged (oldValue, value);
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
		///	The <c>FixedPrice</c> field.
		///	designer:fld/GVAF4/GVAN4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAN4]")]
		public global::System.Decimal? FixedPrice
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAN4]");
			}
			set
			{
				global::System.Decimal? oldValue = this.FixedPrice;
				if (oldValue != value || !this.IsFieldDefined("[GVAN4]"))
				{
					this.OnFixedPriceChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAN4]", oldValue, value);
					this.OnFixedPriceChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>FixedPriceIncludesTaxes</c> field.
		///	designer:fld/GVAF4/GVAO4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAO4]")]
		public bool FixedPriceIncludesTaxes
		{
			get
			{
				return this.GetField<bool> ("[GVAO4]");
			}
			set
			{
				bool oldValue = this.FixedPriceIncludesTaxes;
				if (oldValue != value || !this.IsFieldDefined("[GVAO4]"))
				{
					this.OnFixedPriceIncludesTaxesChanging (oldValue, value);
					this.SetField<bool> ("[GVAO4]", oldValue, value);
					this.OnFixedPriceIncludesTaxesChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ResultingTax</c> field.
		///	designer:fld/GVAF4/GVAU5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAU5]")]
		public global::System.Decimal? ResultingTax
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAU5]");
			}
			set
			{
				global::System.Decimal? oldValue = this.ResultingTax;
				if (oldValue != value || !this.IsFieldDefined("[GVAU5]"))
				{
					this.OnResultingTaxChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAU5]", oldValue, value);
					this.OnResultingTaxChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ResultingPriceBeforeTax</c> field.
		///	designer:fld/GVAF4/GVAP4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAP4]")]
		public global::System.Decimal? ResultingPriceBeforeTax
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAP4]");
			}
			set
			{
				global::System.Decimal? oldValue = this.ResultingPriceBeforeTax;
				if (oldValue != value || !this.IsFieldDefined("[GVAP4]"))
				{
					this.OnResultingPriceBeforeTaxChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAP4]", oldValue, value);
					this.OnResultingPriceBeforeTaxChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>FinalPriceBeforeTax</c> field.
		///	designer:fld/GVAF4/GVAQ4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAQ4]")]
		public global::System.Decimal? FinalPriceBeforeTax
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[GVAQ4]");
			}
			set
			{
				global::System.Decimal? oldValue = this.FinalPriceBeforeTax;
				if (oldValue != value || !this.IsFieldDefined("[GVAQ4]"))
				{
					this.OnFinalPriceBeforeTaxChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[GVAQ4]", oldValue, value);
					this.OnFinalPriceBeforeTaxChanged (oldValue, value);
				}
			}
		}
		
		partial void OnDisplayModesChanging(global::Epsitec.Cresus.Core.Business.Finance.PriceDisplayModes oldValue, global::Epsitec.Cresus.Core.Business.Finance.PriceDisplayModes newValue);
		partial void OnDisplayModesChanged(global::Epsitec.Cresus.Core.Business.Finance.PriceDisplayModes oldValue, global::Epsitec.Cresus.Core.Business.Finance.PriceDisplayModes newValue);
		partial void OnTextForPrimaryPriceChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextForPrimaryPriceChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextForResultingPriceChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextForResultingPriceChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextForFixedPriceChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextForFixedPriceChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnPrimaryPriceBeforeTaxChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnPrimaryPriceBeforeTaxChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnPrimaryTaxChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnPrimaryTaxChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnDiscountChanging(global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity newValue);
		partial void OnDiscountChanged(global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity newValue);
		partial void OnFixedPriceChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnFixedPriceChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnFixedPriceIncludesTaxesChanging(bool oldValue, bool newValue);
		partial void OnFixedPriceIncludesTaxesChanged(bool oldValue, bool newValue);
		partial void OnResultingTaxChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnResultingTaxChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnResultingPriceBeforeTaxChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnResultingPriceBeforeTaxChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnFinalPriceBeforeTaxChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnFinalPriceBeforeTaxChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		
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
		///	The <c>VatCode</c> field.
		///	designer:fld/GVAR4/GVAT4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAT4]")]
		public global::Epsitec.Cresus.Core.Business.Finance.VatCode VatCode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.VatCode> ("[GVAT4]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.VatCode oldValue = this.VatCode;
				if (oldValue != value || !this.IsFieldDefined("[GVAT4]"))
				{
					this.OnVatCodeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.VatCode> ("[GVAT4]", oldValue, value);
					this.OnVatCodeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>BaseAmount</c> field.
		///	designer:fld/GVAR4/GVAU4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAU4]")]
		public global::System.Decimal BaseAmount
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[GVAU4]");
			}
			set
			{
				global::System.Decimal oldValue = this.BaseAmount;
				if (oldValue != value || !this.IsFieldDefined("[GVAU4]"))
				{
					this.OnBaseAmountChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[GVAU4]", oldValue, value);
					this.OnBaseAmountChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Rate</c> field.
		///	designer:fld/GVAR4/GVAV4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAV4]")]
		public global::System.Decimal Rate
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[GVAV4]");
			}
			set
			{
				global::System.Decimal oldValue = this.Rate;
				if (oldValue != value || !this.IsFieldDefined("[GVAV4]"))
				{
					this.OnRateChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[GVAV4]", oldValue, value);
					this.OnRateChanged (oldValue, value);
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
		partial void OnVatCodeChanging(global::Epsitec.Cresus.Core.Business.Finance.VatCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.VatCode newValue);
		partial void OnVatCodeChanged(global::Epsitec.Cresus.Core.Business.Finance.VatCode oldValue, global::Epsitec.Cresus.Core.Business.Finance.VatCode newValue);
		partial void OnBaseAmountChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnBaseAmountChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnRateChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnRateChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
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
	public partial class CustomerEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IReferenceNumber, global::Epsitec.Cresus.Core.Entities.IBusinessLink, global::Epsitec.Cresus.Core.Entities.IWorkflowHost
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
		///	The <c>Relation</c> field.
		///	designer:fld/GVAV5/GVA06
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA06]")]
		public global::Epsitec.Cresus.Core.Entities.RelationEntity Relation
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.RelationEntity> ("[GVA06]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue = this.Relation;
				if (oldValue != value || !this.IsFieldDefined("[GVA06]"))
				{
					this.OnRelationChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.RelationEntity> ("[GVA06]", oldValue, value);
					this.OnRelationChanged (oldValue, value);
				}
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
		///	The <c>DefaultBillingMode</c> field.
		///	designer:fld/GVAV5/GVA46
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA46]")]
		public global::Epsitec.Cresus.Core.Business.Finance.BillingMode DefaultBillingMode
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.Finance.BillingMode> ("[GVA46]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.Finance.BillingMode oldValue = this.DefaultBillingMode;
				if (oldValue != value || !this.IsFieldDefined("[GVA46]"))
				{
					this.OnDefaultBillingModeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.Finance.BillingMode> ("[GVA46]", oldValue, value);
					this.OnDefaultBillingModeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultPriceGroup</c> field.
		///	designer:fld/GVAV5/GVA56
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA56]")]
		public global::Epsitec.Cresus.Core.Entities.PriceGroupEntity DefaultPriceGroup
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.PriceGroupEntity> ("[GVA56]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.PriceGroupEntity oldValue = this.DefaultPriceGroup;
				if (oldValue != value || !this.IsFieldDefined("[GVA56]"))
				{
					this.OnDefaultPriceGroupChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.PriceGroupEntity> ("[GVA56]", oldValue, value);
					this.OnDefaultPriceGroupChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DefaultDiscounts</c> field.
		///	designer:fld/GVAV5/GVA66
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA66]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity> DefaultDiscounts
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.PriceDiscountEntity> ("[GVA66]");
			}
		}
		
		partial void OnRelationChanging(global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue, global::Epsitec.Cresus.Core.Entities.RelationEntity newValue);
		partial void OnRelationChanged(global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue, global::Epsitec.Cresus.Core.Entities.RelationEntity newValue);
		partial void OnSalesRepresentativeChanging(global::Epsitec.Cresus.Core.Entities.PeopleEntity oldValue, global::Epsitec.Cresus.Core.Entities.PeopleEntity newValue);
		partial void OnSalesRepresentativeChanged(global::Epsitec.Cresus.Core.Entities.PeopleEntity oldValue, global::Epsitec.Cresus.Core.Entities.PeopleEntity newValue);
		partial void OnDefaultDebtorBookAccountChanging(string oldValue, string newValue);
		partial void OnDefaultDebtorBookAccountChanged(string oldValue, string newValue);
		partial void OnDefaultBillingModeChanging(global::Epsitec.Cresus.Core.Business.Finance.BillingMode oldValue, global::Epsitec.Cresus.Core.Business.Finance.BillingMode newValue);
		partial void OnDefaultBillingModeChanged(global::Epsitec.Cresus.Core.Business.Finance.BillingMode oldValue, global::Epsitec.Cresus.Core.Business.Finance.BillingMode newValue);
		partial void OnDefaultPriceGroupChanging(global::Epsitec.Cresus.Core.Entities.PriceGroupEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceGroupEntity newValue);
		partial void OnDefaultPriceGroupChanged(global::Epsitec.Cresus.Core.Entities.PriceGroupEntity oldValue, global::Epsitec.Cresus.Core.Entities.PriceGroupEntity newValue);
		
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
		///	The <c>PaymentModes</c> field.
		///	designer:fld/GVA76/GVAB6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVAB6]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.PaymentModeEntity> PaymentModes
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.PaymentModeEntity> ("[GVAB6]");
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

