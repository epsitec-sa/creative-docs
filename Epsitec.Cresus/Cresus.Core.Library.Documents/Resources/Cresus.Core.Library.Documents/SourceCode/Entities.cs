//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[HVA]", typeof (Epsitec.Cresus.Core.Entities.DocumentMetadataEntity))]
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

