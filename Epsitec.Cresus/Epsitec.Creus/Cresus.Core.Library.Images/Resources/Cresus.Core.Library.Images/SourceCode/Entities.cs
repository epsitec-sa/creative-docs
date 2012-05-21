//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[9VA]", typeof (Epsitec.Cresus.Core.Entities.ImageEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[9VA1]", typeof (Epsitec.Cresus.Core.Entities.ImageCategoryEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[9VA2]", typeof (Epsitec.Cresus.Core.Entities.ImageGroupEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[9VA4]", typeof (Epsitec.Cresus.Core.Entities.ImageBlobEntity))]
#region Epsitec.Cresus.Core.Image Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>Image</c> entity.
	///	designer:cap/9VA
	///	</summary>
	public partial class ImageEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.INameDescription
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/9VA/8VA3
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
		///	designer:fld/9VA/8VA7
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
		///	designer:fld/9VA/8VA8
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
		///	The <c>ImageBlob</c> field.
		///	designer:fld/9VA/9VAB
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VAB]")]
		public global::Epsitec.Cresus.Core.Entities.ImageBlobEntity ImageBlob
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.ImageBlobEntity> ("[9VAB]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ImageBlobEntity oldValue = this.ImageBlob;
				if (oldValue != value || !this.IsFieldDefined("[9VAB]"))
				{
					this.OnImageBlobChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.ImageBlobEntity> ("[9VAB]", oldValue, value);
					this.OnImageBlobChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ImageGroups</c> field.
		///	designer:fld/9VA/9VAC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VAC]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.ImageGroupEntity> ImageGroups
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.ImageGroupEntity> ("[9VAC]");
			}
		}
		///	<summary>
		///	The <c>ImageCategory</c> field.
		///	designer:fld/9VA/9VAD
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VAD]")]
		public global::Epsitec.Cresus.Core.Entities.ImageCategoryEntity ImageCategory
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.ImageCategoryEntity> ("[9VAD]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.ImageCategoryEntity oldValue = this.ImageCategory;
				if (oldValue != value || !this.IsFieldDefined("[9VAD]"))
				{
					this.OnImageCategoryChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.ImageCategoryEntity> ("[9VAD]", oldValue, value);
					this.OnImageCategoryChanged (oldValue, value);
				}
			}
		}
		
		partial void OnImageBlobChanging(global::Epsitec.Cresus.Core.Entities.ImageBlobEntity oldValue, global::Epsitec.Cresus.Core.Entities.ImageBlobEntity newValue);
		partial void OnImageBlobChanged(global::Epsitec.Cresus.Core.Entities.ImageBlobEntity oldValue, global::Epsitec.Cresus.Core.Entities.ImageBlobEntity newValue);
		partial void OnImageCategoryChanging(global::Epsitec.Cresus.Core.Entities.ImageCategoryEntity oldValue, global::Epsitec.Cresus.Core.Entities.ImageCategoryEntity newValue);
		partial void OnImageCategoryChanged(global::Epsitec.Cresus.Core.Entities.ImageCategoryEntity oldValue, global::Epsitec.Cresus.Core.Entities.ImageCategoryEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ImageEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ImageEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1001, 10, 0);	// [9VA]
		public static readonly string EntityStructuredTypeKey = "[9VA]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ImageEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.ImageCategory Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ImageCategory</c> entity.
	///	designer:cap/9VA1
	///	</summary>
	public partial class ImageCategoryEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/9VA1/8VA3
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
		///	designer:fld/9VA1/8VA5
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
		///	designer:fld/9VA1/8VA7
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
		///	designer:fld/9VA1/8VA8
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
		///	The <c>CompatibleGroups</c> field.
		///	designer:fld/9VA1/9VA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VA3]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.ImageGroupEntity> CompatibleGroups
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.ImageGroupEntity> ("[9VA3]");
			}
		}
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ImageCategoryEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ImageCategoryEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1001, 10, 1);	// [9VA1]
		public static readonly string EntityStructuredTypeKey = "[9VA1]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ImageCategoryEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.ImageGroup Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ImageGroup</c> entity.
	///	designer:cap/9VA2
	///	</summary>
	public partial class ImageGroupEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/9VA2/8VA3
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
		///	designer:fld/9VA2/8VA5
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
		///	designer:fld/9VA2/8VA7
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
		///	designer:fld/9VA2/8VA8
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
			return global::Epsitec.Cresus.Core.Entities.ImageGroupEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ImageGroupEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1001, 10, 2);	// [9VA2]
		public static readonly string EntityStructuredTypeKey = "[9VA2]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ImageGroupEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.ImageBlob Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ImageBlob</c> entity.
	///	designer:cap/9VA4
	///	</summary>
	public partial class ImageBlobEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IItemCode, global::Epsitec.Cresus.Core.Entities.IFileMetadata, global::Epsitec.Cresus.Core.Entities.IDataHash
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/9VA4/8VA3
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
		///	designer:fld/9VA4/8VA5
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
		#region IDataHash Members
		///	<summary>
		///	The <c>WeakHash</c> field.
		///	designer:fld/9VA4/8VAC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAC]")]
		public int? WeakHash
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IDataHashInterfaceImplementation.GetWeakHash (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IDataHashInterfaceImplementation.SetWeakHash (this, value);
			}
		}
		#endregion
		#region IDateMetadata Members
		///	<summary>
		///	The <c>CreationDate</c> field.
		///	designer:fld/9VA4/8VAH
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
		#region IDataHash Members
		///	<summary>
		///	The <c>StrongHash</c> field.
		///	designer:fld/9VA4/8VAD
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAD]")]
		public string StrongHash
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IDataHashInterfaceImplementation.GetStrongHash (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IDataHashInterfaceImplementation.SetStrongHash (this, value);
			}
		}
		#endregion
		#region IDateMetadata Members
		///	<summary>
		///	The <c>LastModificationDate</c> field.
		///	designer:fld/9VA4/8VAI
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
		#region IFileMetadata Members
		///	<summary>
		///	The <c>FileName</c> field.
		///	designer:fld/9VA4/8VAE
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
		///	designer:fld/9VA4/8VAF
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
		///	designer:fld/9VA4/8VAG
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
		///	The <c>Data</c> field.
		///	designer:fld/9VA4/9VA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VA5]")]
		public global::System.Byte[] Data
		{
			get
			{
				return this.GetField<global::System.Byte[]> ("[9VA5]");
			}
			set
			{
				global::System.Byte[] oldValue = this.Data;
				if (oldValue != value || !this.IsFieldDefined("[9VA5]"))
				{
					this.OnDataChanging (oldValue, value);
					this.SetField<global::System.Byte[]> ("[9VA5]", oldValue, value);
					this.OnDataChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PixelWidth</c> field.
		///	designer:fld/9VA4/9VA6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VA6]")]
		public int PixelWidth
		{
			get
			{
				return this.GetField<int> ("[9VA6]");
			}
			set
			{
				int oldValue = this.PixelWidth;
				if (oldValue != value || !this.IsFieldDefined("[9VA6]"))
				{
					this.OnPixelWidthChanging (oldValue, value);
					this.SetField<int> ("[9VA6]", oldValue, value);
					this.OnPixelWidthChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>PixelHeight</c> field.
		///	designer:fld/9VA4/9VA7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VA7]")]
		public int PixelHeight
		{
			get
			{
				return this.GetField<int> ("[9VA7]");
			}
			set
			{
				int oldValue = this.PixelHeight;
				if (oldValue != value || !this.IsFieldDefined("[9VA7]"))
				{
					this.OnPixelHeightChanging (oldValue, value);
					this.SetField<int> ("[9VA7]", oldValue, value);
					this.OnPixelHeightChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ThumbnailSize</c> field.
		///	designer:fld/9VA4/9VA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VA8]")]
		public int ThumbnailSize
		{
			get
			{
				return this.GetField<int> ("[9VA8]");
			}
			set
			{
				int oldValue = this.ThumbnailSize;
				if (oldValue != value || !this.IsFieldDefined("[9VA8]"))
				{
					this.OnThumbnailSizeChanging (oldValue, value);
					this.SetField<int> ("[9VA8]", oldValue, value);
					this.OnThumbnailSizeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Dpi</c> field.
		///	designer:fld/9VA4/9VA9
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VA9]")]
		public global::System.Decimal Dpi
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[9VA9]");
			}
			set
			{
				global::System.Decimal oldValue = this.Dpi;
				if (oldValue != value || !this.IsFieldDefined("[9VA9]"))
				{
					this.OnDpiChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[9VA9]", oldValue, value);
					this.OnDpiChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>BitsPerPixel</c> field.
		///	designer:fld/9VA4/9VAA
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[9VAA]")]
		public int BitsPerPixel
		{
			get
			{
				return this.GetField<int> ("[9VAA]");
			}
			set
			{
				int oldValue = this.BitsPerPixel;
				if (oldValue != value || !this.IsFieldDefined("[9VAA]"))
				{
					this.OnBitsPerPixelChanging (oldValue, value);
					this.SetField<int> ("[9VAA]", oldValue, value);
					this.OnBitsPerPixelChanged (oldValue, value);
				}
			}
		}
		
		partial void OnDataChanging(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		partial void OnDataChanged(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		partial void OnPixelWidthChanging(int oldValue, int newValue);
		partial void OnPixelWidthChanged(int oldValue, int newValue);
		partial void OnPixelHeightChanging(int oldValue, int newValue);
		partial void OnPixelHeightChanged(int oldValue, int newValue);
		partial void OnThumbnailSizeChanging(int oldValue, int newValue);
		partial void OnThumbnailSizeChanged(int oldValue, int newValue);
		partial void OnDpiChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnDpiChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnBitsPerPixelChanging(int oldValue, int newValue);
		partial void OnBitsPerPixelChanged(int oldValue, int newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ImageBlobEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ImageBlobEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1001, 10, 4);	// [9VA4]
		public static readonly string EntityStructuredTypeKey = "[9VA4]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ImageBlobEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

