//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[HVA]", typeof (Epsitec.Cresus.Core.Entities.DocumentMetadataEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[HVA1]", typeof (Epsitec.Cresus.Core.Entities.DocumentCategoryEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[HVA2]", typeof (Epsitec.Cresus.Core.Entities.DocumentOptionsEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[HVA3]", typeof (Epsitec.Cresus.Core.Entities.DocumentPrintingUnitsEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[HVA4]", typeof (Epsitec.Cresus.Core.Entities.AbstractDocumentEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[HVAC]", typeof (Epsitec.Cresus.Core.Entities.SerializedDocumentBlobEntity))]
#region Epsitec.Cresus.Core.DocumentMetadata Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>DocumentMetadata</c> entity.
	///	designer:cap/HVA
	///	</summary>
	public partial class DocumentMetadataEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IReferenceNumber, global::Epsitec.Cresus.Core.Entities.IFileMetadata, global::Epsitec.Cresus.Core.Entities.INameDescription, global::Epsitec.Cresus.Core.Entities.IComments, global::Epsitec.Cresus.Core.Entities.IWorkflowHost
	{
		#region IReferenceNumber Members
		///	<summary>
		///	The <c>IdA</c> field.
		///	designer:fld/HVA/8VA11
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
		///	designer:fld/HVA/8VA3
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
		///	designer:fld/HVA/8VA7
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
		#region IDateMetadata Members
		///	<summary>
		///	The <c>CreationDate</c> field.
		///	designer:fld/HVA/8VAH
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAH]")]
		public global::System.DateTime? CreationDate
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IDateMetadataInterfaceImplementation.GetCreationDate (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IDateMetadataInterfaceImplementation.SetCreationDate (this, value);
			}
		}
		#endregion
		#region IComments Members
		///	<summary>
		///	The <c>Comments</c> field.
		///	designer:fld/HVA/8VAT
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
		///	designer:fld/HVA/DVA31
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
		#region IReferenceNumber Members
		///	<summary>
		///	The <c>IdB</c> field.
		///	designer:fld/HVA/8VA21
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
		///	designer:fld/HVA/8VA8
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
		#region IDateMetadata Members
		///	<summary>
		///	The <c>LastModificationDate</c> field.
		///	designer:fld/HVA/8VAI
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAI]")]
		public global::System.DateTime? LastModificationDate
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IDateMetadataInterfaceImplementation.GetLastModificationDate (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IDateMetadataInterfaceImplementation.SetLastModificationDate (this, value);
			}
		}
		#endregion
		#region IReferenceNumber Members
		///	<summary>
		///	The <c>IdC</c> field.
		///	designer:fld/HVA/8VA31
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
		#region IFileMetadata Members
		///	<summary>
		///	The <c>FileName</c> field.
		///	designer:fld/HVA/8VAE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAE]")]
		public string FileName
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IFileMetadataInterfaceImplementation.GetFileName (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IFileMetadataInterfaceImplementation.SetFileName (this, value);
			}
		}
		///	<summary>
		///	The <c>FileUri</c> field.
		///	designer:fld/HVA/8VAF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAF]")]
		public string FileUri
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IFileMetadataInterfaceImplementation.GetFileUri (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IFileMetadataInterfaceImplementation.SetFileUri (this, value);
			}
		}
		///	<summary>
		///	The <c>FileMimeType</c> field.
		///	designer:fld/HVA/8VAG
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAG]")]
		public string FileMimeType
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IFileMetadataInterfaceImplementation.GetFileMimeType (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IFileMetadataInterfaceImplementation.SetFileMimeType (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>DocumentTitle</c> field.
		///	designer:fld/HVA/HVA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HVA5]")]
		public global::Epsitec.Common.Types.FormattedText DocumentTitle
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[HVA5]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.DocumentTitle;
				if (oldValue != value || !this.IsFieldDefined("[HVA5]"))
				{
					this.OnDocumentTitleChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[HVA5]", oldValue, value);
					this.OnDocumentTitleChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DocumentCategory</c> field.
		///	designer:fld/HVA/HVA6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HVA6]")]
		public global::Epsitec.Cresus.Core.Entities.DocumentCategoryEntity DocumentCategory
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.DocumentCategoryEntity> ("[HVA6]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.DocumentCategoryEntity oldValue = this.DocumentCategory;
				if (oldValue != value || !this.IsFieldDefined("[HVA6]"))
				{
					this.OnDocumentCategoryChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.DocumentCategoryEntity> ("[HVA6]", oldValue, value);
					this.OnDocumentCategoryChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SerializedDocumentVersions</c> field.
		///	designer:fld/HVA/HVAE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HVAE]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.SerializedDocumentBlobEntity> SerializedDocumentVersions
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.SerializedDocumentBlobEntity> ("[HVAE]");
			}
		}
		
		partial void OnDocumentTitleChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnDocumentTitleChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnDocumentCategoryChanging(global::Epsitec.Cresus.Core.Entities.DocumentCategoryEntity oldValue, global::Epsitec.Cresus.Core.Entities.DocumentCategoryEntity newValue);
		partial void OnDocumentCategoryChanged(global::Epsitec.Cresus.Core.Entities.DocumentCategoryEntity oldValue, global::Epsitec.Cresus.Core.Entities.DocumentCategoryEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.DocumentMetadataEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.DocumentMetadataEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1009, 10, 0);	// [HVA]
		public static readonly new string EntityStructuredTypeKey = "[HVA]";
	}
}
#endregion

#region Epsitec.Cresus.Core.DocumentCategory Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>DocumentCategory</c> entity.
	///	designer:cap/HVA1
	///	</summary>
	public partial class DocumentCategoryEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory, global::Epsitec.Cresus.Core.Entities.IItemRank
	{
		#region IItemRank Members
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/HVA1/8VA1
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
		///	designer:fld/HVA1/8VA3
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
		///	designer:fld/HVA1/8VA5
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
		///	designer:fld/HVA1/8VA7
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
		///	designer:fld/HVA1/8VA8
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
		///	The <c>DocumentType</c> field.
		///	designer:fld/HVA1/HVA7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HVA7]")]
		public global::Epsitec.Cresus.Core.Entities.DocumentCategoryEntity DocumentType
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.DocumentCategoryEntity> ("[HVA7]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.DocumentCategoryEntity oldValue = this.DocumentType;
				if (oldValue != value || !this.IsFieldDefined("[HVA7]"))
				{
					this.OnDocumentTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.DocumentCategoryEntity> ("[HVA7]", oldValue, value);
					this.OnDocumentTypeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DocumentOptions</c> field.
		///	designer:fld/HVA1/HVA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HVA8]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.DocumentOptionsEntity> DocumentOptions
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.DocumentOptionsEntity> ("[HVA8]");
			}
		}
		///	<summary>
		///	The <c>DocumentPrintingUnits</c> field.
		///	designer:fld/HVA1/HVA9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HVA9]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.DocumentPrintingUnitsEntity> DocumentPrintingUnits
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.DocumentPrintingUnitsEntity> ("[HVA9]");
			}
		}
		
		partial void OnDocumentTypeChanging(global::Epsitec.Cresus.Core.Entities.DocumentCategoryEntity oldValue, global::Epsitec.Cresus.Core.Entities.DocumentCategoryEntity newValue);
		partial void OnDocumentTypeChanged(global::Epsitec.Cresus.Core.Entities.DocumentCategoryEntity oldValue, global::Epsitec.Cresus.Core.Entities.DocumentCategoryEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.DocumentCategoryEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.DocumentCategoryEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1009, 10, 1);	// [HVA1]
		public static readonly new string EntityStructuredTypeKey = "[HVA1]";
	}
}
#endregion

#region Epsitec.Cresus.Core.DocumentOptions Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>DocumentOptions</c> entity.
	///	designer:cap/HVA2
	///	</summary>
	public partial class DocumentOptionsEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/HVA2/8VA3
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
		///	designer:fld/HVA2/8VA5
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
		///	designer:fld/HVA2/8VA7
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
		///	designer:fld/HVA2/8VA8
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
		///	designer:fld/HVA2/HVAA
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HVAA]")]
		public global::System.Byte[] SerializedData
		{
			get
			{
				return this.GetField<global::System.Byte[]> ("[HVAA]");
			}
			set
			{
				global::System.Byte[] oldValue = this.SerializedData;
				if (oldValue != value || !this.IsFieldDefined("[HVAA]"))
				{
					this.OnSerializedDataChanging (oldValue, value);
					this.SetField<global::System.Byte[]> ("[HVAA]", oldValue, value);
					this.OnSerializedDataChanged (oldValue, value);
				}
			}
		}
		
		partial void OnSerializedDataChanging(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		partial void OnSerializedDataChanged(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.DocumentOptionsEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.DocumentOptionsEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1009, 10, 2);	// [HVA2]
		public static readonly new string EntityStructuredTypeKey = "[HVA2]";
	}
}
#endregion

#region Epsitec.Cresus.Core.DocumentPrintingUnits Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>DocumentPrintingUnits</c> entity.
	///	designer:cap/HVA3
	///	</summary>
	public partial class DocumentPrintingUnitsEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/HVA3/8VA3
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
		///	designer:fld/HVA3/8VA5
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
		///	designer:fld/HVA3/8VA7
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
		///	designer:fld/HVA3/8VA8
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
		///	designer:fld/HVA3/HVAB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HVAB]")]
		public global::System.Byte[] SerializedData
		{
			get
			{
				return this.GetField<global::System.Byte[]> ("[HVAB]");
			}
			set
			{
				global::System.Byte[] oldValue = this.SerializedData;
				if (oldValue != value || !this.IsFieldDefined("[HVAB]"))
				{
					this.OnSerializedDataChanging (oldValue, value);
					this.SetField<global::System.Byte[]> ("[HVAB]", oldValue, value);
					this.OnSerializedDataChanged (oldValue, value);
				}
			}
		}
		
		partial void OnSerializedDataChanging(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		partial void OnSerializedDataChanged(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.DocumentPrintingUnitsEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.DocumentPrintingUnitsEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1009, 10, 3);	// [HVA3]
		public static readonly new string EntityStructuredTypeKey = "[HVA3]";
	}
}
#endregion

#region Epsitec.Cresus.Core.AbstractDocument Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>AbstractDocument</c> entity.
	///	designer:cap/HVA4
	///	</summary>
	public partial class AbstractDocumentEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.AbstractDocumentEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.AbstractDocumentEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1009, 10, 4);	// [HVA4]
		public static readonly new string EntityStructuredTypeKey = "[HVA4]";
	}
}
#endregion

#region Epsitec.Cresus.Core.SerializedDocumentBlob Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>SerializedDocumentBlob</c> entity.
	///	designer:cap/HVAC
	///	</summary>
	public partial class SerializedDocumentBlobEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.IItemCode
	{
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/HVAC/8VA5
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
		///	The <c>Data</c> field.
		///	designer:fld/HVAC/HVAD
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HVAD]")]
		public global::System.Byte[] Data
		{
			get
			{
				return this.GetField<global::System.Byte[]> ("[HVAD]");
			}
			set
			{
				global::System.Byte[] oldValue = this.Data;
				if (oldValue != value || !this.IsFieldDefined("[HVAD]"))
				{
					this.OnDataChanging (oldValue, value);
					this.SetField<global::System.Byte[]> ("[HVAD]", oldValue, value);
					this.OnDataChanged (oldValue, value);
				}
			}
		}
		
		partial void OnDataChanging(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		partial void OnDataChanged(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.SerializedDocumentBlobEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.SerializedDocumentBlobEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1009, 10, 12);	// [HVAC]
		public static readonly new string EntityStructuredTypeKey = "[HVAC]";
	}
}
#endregion

