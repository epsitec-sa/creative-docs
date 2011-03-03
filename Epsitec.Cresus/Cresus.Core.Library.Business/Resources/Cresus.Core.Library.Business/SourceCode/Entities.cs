//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[GVA]", typeof (Epsitec.Cresus.Core.Entities.RelationEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVA1]", typeof (Epsitec.Cresus.Core.Entities.AffairEntity))]
#region Epsitec.Cresus.Core.Relation Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>Relation</c> entity.
	///	designer:cap/GVA
	///	</summary>
	public partial class RelationEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
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
		
		partial void OnPersonChanging(global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity newValue);
		partial void OnPersonChanged(global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.RelationEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.RelationEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 0);	// [GVA]
		public static readonly new string EntityStructuredTypeKey = "[GVA]";
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
		///	The <c>Relation</c> field.
		///	designer:fld/GVA1/GVA2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA2]")]
		public global::Epsitec.Cresus.Core.Entities.RelationEntity Relation
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.RelationEntity> ("[GVA2]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue = this.Relation;
				if (oldValue != value || !this.IsFieldDefined("[GVA2]"))
				{
					this.OnRelationChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.RelationEntity> ("[GVA2]", oldValue, value);
					this.OnRelationChanged (oldValue, value);
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
		public global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity ActiveSalesRepresentative
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity> ("[GVA7]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity oldValue = this.ActiveSalesRepresentative;
				if (oldValue != value || !this.IsFieldDefined("[GVA7]"))
				{
					this.OnActiveSalesRepresentativeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity> ("[GVA7]", oldValue, value);
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
		
		partial void OnRelationChanging(global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue, global::Epsitec.Cresus.Core.Entities.RelationEntity newValue);
		partial void OnRelationChanged(global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue, global::Epsitec.Cresus.Core.Entities.RelationEntity newValue);
		partial void OnDefaultDebtorBookAccountChanging(string oldValue, string newValue);
		partial void OnDefaultDebtorBookAccountChanged(string oldValue, string newValue);
		partial void OnActiveSalesRepresentativeChanging(global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity newValue);
		partial void OnActiveSalesRepresentativeChanged(global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.NaturalPersonEntity newValue);
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
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 1);	// [GVA1]
		public static readonly new string EntityStructuredTypeKey = "[GVA1]";
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

