//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[8VAQ]", typeof (Epsitec.Cresus.Core.Entities.CommentEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[8VAU]", typeof (Epsitec.Cresus.Core.Entities.LanguageEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[8VA41]", typeof (Epsitec.Cresus.Core.Entities.XmlBlobEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[8VA61]", typeof (Epsitec.Cresus.Core.Entities.DateRangeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[8VA91]", typeof (Epsitec.Cresus.Core.Entities.GeneratorDefinitionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[8VA02]", typeof (Epsitec.Cresus.Core.Entities.EntityAttributeEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[8VA12]", typeof (Epsitec.Cresus.Core.Entities.EntityAttributeCategoryEntity))]
#region Epsitec.Cresus.Core.IItemRank Interface
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>IItemRank</c> entity.
	///	designer:cap/8VA
	///	</summary>
	public interface IItemRank
	{
		///	<summary>
		///	The <c>Rank</c> field.
		///	designer:fld/8VA/8VA1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA1]")]
		int? Rank
		{
			get;
			set;
		}
	}
	public static partial class IItemRankInterfaceImplementation
	{
		public static int? GetRank(global::Epsitec.Cresus.Core.Entities.IItemRank obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<int?> ("[8VA1]");
		}
		public static void SetRank(global::Epsitec.Cresus.Core.Entities.IItemRank obj, int? value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			int? oldValue = obj.Rank;
			if (oldValue != value || !entity.IsFieldDefined("[8VA1]"))
			{
				IItemRankInterfaceImplementation.OnRankChanging (obj, oldValue, value);
				entity.SetField<int?> ("[8VA1]", oldValue, value);
				IItemRankInterfaceImplementation.OnRankChanged (obj, oldValue, value);
			}
		}
		static partial void OnRankChanged(global::Epsitec.Cresus.Core.Entities.IItemRank obj, int? oldValue, int? newValue);
		static partial void OnRankChanging(global::Epsitec.Cresus.Core.Entities.IItemRank obj, int? oldValue, int? newValue);
	}
}
#endregion

#region Epsitec.Cresus.Core.ILifetime Interface
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ILifetime</c> entity.
	///	designer:cap/8VA2
	///	</summary>
	public interface ILifetime
	{
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/8VA2/8VA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA3]")]
		bool IsArchive
		{
			get;
			set;
		}
	}
	public static partial class ILifetimeInterfaceImplementation
	{
		public static bool GetIsArchive(global::Epsitec.Cresus.Core.Entities.ILifetime obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<bool> ("[8VA3]");
		}
		public static void SetIsArchive(global::Epsitec.Cresus.Core.Entities.ILifetime obj, bool value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			bool oldValue = obj.IsArchive;
			if (oldValue != value || !entity.IsFieldDefined("[8VA3]"))
			{
				ILifetimeInterfaceImplementation.OnIsArchiveChanging (obj, oldValue, value);
				entity.SetField<bool> ("[8VA3]", oldValue, value);
				ILifetimeInterfaceImplementation.OnIsArchiveChanged (obj, oldValue, value);
			}
		}
		static partial void OnIsArchiveChanged(global::Epsitec.Cresus.Core.Entities.ILifetime obj, bool oldValue, bool newValue);
		static partial void OnIsArchiveChanging(global::Epsitec.Cresus.Core.Entities.ILifetime obj, bool oldValue, bool newValue);
	}
}
#endregion

#region Epsitec.Cresus.Core.IItemCode Interface
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>IItemCode</c> entity.
	///	designer:cap/8VA4
	///	</summary>
	public interface IItemCode
	{
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/8VA4/8VA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA5]")]
		string Code
		{
			get;
			set;
		}
	}
	public static partial class IItemCodeInterfaceImplementation
	{
		public static string GetCode(global::Epsitec.Cresus.Core.Entities.IItemCode obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[8VA5]");
		}
		public static void SetCode(global::Epsitec.Cresus.Core.Entities.IItemCode obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.Code;
			if (oldValue != value || !entity.IsFieldDefined("[8VA5]"))
			{
				IItemCodeInterfaceImplementation.OnCodeChanging (obj, oldValue, value);
				entity.SetField<string> ("[8VA5]", oldValue, value);
				IItemCodeInterfaceImplementation.OnCodeChanged (obj, oldValue, value);
			}
		}
		static partial void OnCodeChanged(global::Epsitec.Cresus.Core.Entities.IItemCode obj, string oldValue, string newValue);
		static partial void OnCodeChanging(global::Epsitec.Cresus.Core.Entities.IItemCode obj, string oldValue, string newValue);
	}
}
#endregion

#region Epsitec.Cresus.Core.INameDescription Interface
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>INameDescription</c> entity.
	///	designer:cap/8VA6
	///	</summary>
	public interface INameDescription
	{
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/8VA6/8VA7
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA7]")]
		global::Epsitec.Common.Types.FormattedText Name
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/8VA6/8VA8
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA8]")]
		global::Epsitec.Common.Types.FormattedText Description
		{
			get;
			set;
		}
	}
	public static partial class INameDescriptionInterfaceImplementation
	{
		public static global::Epsitec.Common.Types.FormattedText GetName(global::Epsitec.Cresus.Core.Entities.INameDescription obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::Epsitec.Common.Types.FormattedText> ("[8VA7]");
		}
		public static void SetName(global::Epsitec.Cresus.Core.Entities.INameDescription obj, global::Epsitec.Common.Types.FormattedText value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::Epsitec.Common.Types.FormattedText oldValue = obj.Name;
			if (oldValue != value || !entity.IsFieldDefined("[8VA7]"))
			{
				INameDescriptionInterfaceImplementation.OnNameChanging (obj, oldValue, value);
				entity.SetField<global::Epsitec.Common.Types.FormattedText> ("[8VA7]", oldValue, value);
				INameDescriptionInterfaceImplementation.OnNameChanged (obj, oldValue, value);
			}
		}
		static partial void OnNameChanged(global::Epsitec.Cresus.Core.Entities.INameDescription obj, global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		static partial void OnNameChanging(global::Epsitec.Cresus.Core.Entities.INameDescription obj, global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		public static global::Epsitec.Common.Types.FormattedText GetDescription(global::Epsitec.Cresus.Core.Entities.INameDescription obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::Epsitec.Common.Types.FormattedText> ("[8VA8]");
		}
		public static void SetDescription(global::Epsitec.Cresus.Core.Entities.INameDescription obj, global::Epsitec.Common.Types.FormattedText value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::Epsitec.Common.Types.FormattedText oldValue = obj.Description;
			if (oldValue != value || !entity.IsFieldDefined("[8VA8]"))
			{
				INameDescriptionInterfaceImplementation.OnDescriptionChanging (obj, oldValue, value);
				entity.SetField<global::Epsitec.Common.Types.FormattedText> ("[8VA8]", oldValue, value);
				INameDescriptionInterfaceImplementation.OnDescriptionChanged (obj, oldValue, value);
			}
		}
		static partial void OnDescriptionChanged(global::Epsitec.Cresus.Core.Entities.INameDescription obj, global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		static partial void OnDescriptionChanging(global::Epsitec.Cresus.Core.Entities.INameDescription obj, global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
	}
}
#endregion

#region Epsitec.Cresus.Core.IDateMetadata Interface
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>IDateMetadata</c> entity.
	///	designer:cap/8VA9
	///	</summary>
	public interface IDateMetadata
	{
		///	<summary>
		///	The <c>CreationDate</c> field.
		///	designer:fld/8VA9/8VAH
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAH]")]
		global::System.DateTime? CreationDate
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>LastModificationDate</c> field.
		///	designer:fld/8VA9/8VAI
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAI]")]
		global::System.DateTime? LastModificationDate
		{
			get;
			set;
		}
	}
	public static partial class IDateMetadataInterfaceImplementation
	{
		public static global::System.DateTime? GetCreationDate(global::Epsitec.Cresus.Core.Entities.IDateMetadata obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::System.DateTime?> ("[8VAH]");
		}
		public static void SetCreationDate(global::Epsitec.Cresus.Core.Entities.IDateMetadata obj, global::System.DateTime? value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::System.DateTime? oldValue = obj.CreationDate;
			if (oldValue != value || !entity.IsFieldDefined("[8VAH]"))
			{
				IDateMetadataInterfaceImplementation.OnCreationDateChanging (obj, oldValue, value);
				entity.SetField<global::System.DateTime?> ("[8VAH]", oldValue, value);
				IDateMetadataInterfaceImplementation.OnCreationDateChanged (obj, oldValue, value);
			}
		}
		static partial void OnCreationDateChanged(global::Epsitec.Cresus.Core.Entities.IDateMetadata obj, global::System.DateTime? oldValue, global::System.DateTime? newValue);
		static partial void OnCreationDateChanging(global::Epsitec.Cresus.Core.Entities.IDateMetadata obj, global::System.DateTime? oldValue, global::System.DateTime? newValue);
		public static global::System.DateTime? GetLastModificationDate(global::Epsitec.Cresus.Core.Entities.IDateMetadata obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::System.DateTime?> ("[8VAI]");
		}
		public static void SetLastModificationDate(global::Epsitec.Cresus.Core.Entities.IDateMetadata obj, global::System.DateTime? value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::System.DateTime? oldValue = obj.LastModificationDate;
			if (oldValue != value || !entity.IsFieldDefined("[8VAI]"))
			{
				IDateMetadataInterfaceImplementation.OnLastModificationDateChanging (obj, oldValue, value);
				entity.SetField<global::System.DateTime?> ("[8VAI]", oldValue, value);
				IDateMetadataInterfaceImplementation.OnLastModificationDateChanged (obj, oldValue, value);
			}
		}
		static partial void OnLastModificationDateChanged(global::Epsitec.Cresus.Core.Entities.IDateMetadata obj, global::System.DateTime? oldValue, global::System.DateTime? newValue);
		static partial void OnLastModificationDateChanging(global::Epsitec.Cresus.Core.Entities.IDateMetadata obj, global::System.DateTime? oldValue, global::System.DateTime? newValue);
	}
}
#endregion

#region Epsitec.Cresus.Core.IFileMetadata Interface
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>IFileMetadata</c> entity.
	///	designer:cap/8VAA
	///	</summary>
	public interface IFileMetadata : global::Epsitec.Cresus.Core.Entities.IDateMetadata
	{
		///	<summary>
		///	The <c>FileName</c> field.
		///	designer:fld/8VAA/8VAE
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAE]")]
		string FileName
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>FileUri</c> field.
		///	designer:fld/8VAA/8VAF
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAF]")]
		string FileUri
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>FileMimeType</c> field.
		///	designer:fld/8VAA/8VAG
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAG]")]
		string FileMimeType
		{
			get;
			set;
		}
	}
	public static partial class IFileMetadataInterfaceImplementation
	{
		public static string GetFileName(global::Epsitec.Cresus.Core.Entities.IFileMetadata obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[8VAE]");
		}
		public static void SetFileName(global::Epsitec.Cresus.Core.Entities.IFileMetadata obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.FileName;
			if (oldValue != value || !entity.IsFieldDefined("[8VAE]"))
			{
				IFileMetadataInterfaceImplementation.OnFileNameChanging (obj, oldValue, value);
				entity.SetField<string> ("[8VAE]", oldValue, value);
				IFileMetadataInterfaceImplementation.OnFileNameChanged (obj, oldValue, value);
			}
		}
		static partial void OnFileNameChanged(global::Epsitec.Cresus.Core.Entities.IFileMetadata obj, string oldValue, string newValue);
		static partial void OnFileNameChanging(global::Epsitec.Cresus.Core.Entities.IFileMetadata obj, string oldValue, string newValue);
		public static string GetFileUri(global::Epsitec.Cresus.Core.Entities.IFileMetadata obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[8VAF]");
		}
		public static void SetFileUri(global::Epsitec.Cresus.Core.Entities.IFileMetadata obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.FileUri;
			if (oldValue != value || !entity.IsFieldDefined("[8VAF]"))
			{
				IFileMetadataInterfaceImplementation.OnFileUriChanging (obj, oldValue, value);
				entity.SetField<string> ("[8VAF]", oldValue, value);
				IFileMetadataInterfaceImplementation.OnFileUriChanged (obj, oldValue, value);
			}
		}
		static partial void OnFileUriChanged(global::Epsitec.Cresus.Core.Entities.IFileMetadata obj, string oldValue, string newValue);
		static partial void OnFileUriChanging(global::Epsitec.Cresus.Core.Entities.IFileMetadata obj, string oldValue, string newValue);
		public static string GetFileMimeType(global::Epsitec.Cresus.Core.Entities.IFileMetadata obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[8VAG]");
		}
		public static void SetFileMimeType(global::Epsitec.Cresus.Core.Entities.IFileMetadata obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.FileMimeType;
			if (oldValue != value || !entity.IsFieldDefined("[8VAG]"))
			{
				IFileMetadataInterfaceImplementation.OnFileMimeTypeChanging (obj, oldValue, value);
				entity.SetField<string> ("[8VAG]", oldValue, value);
				IFileMetadataInterfaceImplementation.OnFileMimeTypeChanged (obj, oldValue, value);
			}
		}
		static partial void OnFileMimeTypeChanged(global::Epsitec.Cresus.Core.Entities.IFileMetadata obj, string oldValue, string newValue);
		static partial void OnFileMimeTypeChanging(global::Epsitec.Cresus.Core.Entities.IFileMetadata obj, string oldValue, string newValue);
	}
}
#endregion

#region Epsitec.Cresus.Core.IDataHash Interface
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>IDataHash</c> entity.
	///	designer:cap/8VAB
	///	</summary>
	public interface IDataHash
	{
		///	<summary>
		///	The <c>WeakHash</c> field.
		///	designer:fld/8VAB/8VAC
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAC]")]
		int? WeakHash
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>StrongHash</c> field.
		///	designer:fld/8VAB/8VAD
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAD]")]
		string StrongHash
		{
			get;
			set;
		}
	}
	public static partial class IDataHashInterfaceImplementation
	{
		public static int? GetWeakHash(global::Epsitec.Cresus.Core.Entities.IDataHash obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<int?> ("[8VAC]");
		}
		public static void SetWeakHash(global::Epsitec.Cresus.Core.Entities.IDataHash obj, int? value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			int? oldValue = obj.WeakHash;
			if (oldValue != value || !entity.IsFieldDefined("[8VAC]"))
			{
				IDataHashInterfaceImplementation.OnWeakHashChanging (obj, oldValue, value);
				entity.SetField<int?> ("[8VAC]", oldValue, value);
				IDataHashInterfaceImplementation.OnWeakHashChanged (obj, oldValue, value);
			}
		}
		static partial void OnWeakHashChanged(global::Epsitec.Cresus.Core.Entities.IDataHash obj, int? oldValue, int? newValue);
		static partial void OnWeakHashChanging(global::Epsitec.Cresus.Core.Entities.IDataHash obj, int? oldValue, int? newValue);
		public static string GetStrongHash(global::Epsitec.Cresus.Core.Entities.IDataHash obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[8VAD]");
		}
		public static void SetStrongHash(global::Epsitec.Cresus.Core.Entities.IDataHash obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.StrongHash;
			if (oldValue != value || !entity.IsFieldDefined("[8VAD]"))
			{
				IDataHashInterfaceImplementation.OnStrongHashChanging (obj, oldValue, value);
				entity.SetField<string> ("[8VAD]", oldValue, value);
				IDataHashInterfaceImplementation.OnStrongHashChanged (obj, oldValue, value);
			}
		}
		static partial void OnStrongHashChanged(global::Epsitec.Cresus.Core.Entities.IDataHash obj, string oldValue, string newValue);
		static partial void OnStrongHashChanging(global::Epsitec.Cresus.Core.Entities.IDataHash obj, string oldValue, string newValue);
	}
}
#endregion

#region Epsitec.Cresus.Core.IDateTimeRange Interface
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>IDateTimeRange</c> entity.
	///	designer:cap/8VAJ
	///	</summary>
	public interface IDateTimeRange
	{
		///	<summary>
		///	The <c>BeginDate</c> field.
		///	designer:fld/8VAJ/8VAK
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAK]")]
		global::System.DateTime? BeginDate
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/8VAJ/8VAL
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAL]")]
		global::System.DateTime? EndDate
		{
			get;
			set;
		}
	}
	public static partial class IDateTimeRangeInterfaceImplementation
	{
		public static global::System.DateTime? GetBeginDate(global::Epsitec.Cresus.Core.Entities.IDateTimeRange obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::System.DateTime?> ("[8VAK]");
		}
		public static void SetBeginDate(global::Epsitec.Cresus.Core.Entities.IDateTimeRange obj, global::System.DateTime? value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::System.DateTime? oldValue = obj.BeginDate;
			if (oldValue != value || !entity.IsFieldDefined("[8VAK]"))
			{
				IDateTimeRangeInterfaceImplementation.OnBeginDateChanging (obj, oldValue, value);
				entity.SetField<global::System.DateTime?> ("[8VAK]", oldValue, value);
				IDateTimeRangeInterfaceImplementation.OnBeginDateChanged (obj, oldValue, value);
			}
		}
		static partial void OnBeginDateChanged(global::Epsitec.Cresus.Core.Entities.IDateTimeRange obj, global::System.DateTime? oldValue, global::System.DateTime? newValue);
		static partial void OnBeginDateChanging(global::Epsitec.Cresus.Core.Entities.IDateTimeRange obj, global::System.DateTime? oldValue, global::System.DateTime? newValue);
		public static global::System.DateTime? GetEndDate(global::Epsitec.Cresus.Core.Entities.IDateTimeRange obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::System.DateTime?> ("[8VAL]");
		}
		public static void SetEndDate(global::Epsitec.Cresus.Core.Entities.IDateTimeRange obj, global::System.DateTime? value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::System.DateTime? oldValue = obj.EndDate;
			if (oldValue != value || !entity.IsFieldDefined("[8VAL]"))
			{
				IDateTimeRangeInterfaceImplementation.OnEndDateChanging (obj, oldValue, value);
				entity.SetField<global::System.DateTime?> ("[8VAL]", oldValue, value);
				IDateTimeRangeInterfaceImplementation.OnEndDateChanged (obj, oldValue, value);
			}
		}
		static partial void OnEndDateChanged(global::Epsitec.Cresus.Core.Entities.IDateTimeRange obj, global::System.DateTime? oldValue, global::System.DateTime? newValue);
		static partial void OnEndDateChanging(global::Epsitec.Cresus.Core.Entities.IDateTimeRange obj, global::System.DateTime? oldValue, global::System.DateTime? newValue);
	}
}
#endregion

#region Epsitec.Cresus.Core.ICategory Interface
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ICategory</c> entity.
	///	designer:cap/8VAM
	///	</summary>
	public interface ICategory : global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IItemCode, global::Epsitec.Cresus.Core.Entities.INameDescription
	{
	}
	public static partial class ICategoryInterfaceImplementation
	{
	}
}
#endregion

#region Epsitec.Cresus.Core.IDateRange Interface
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>IDateRange</c> entity.
	///	designer:cap/8VAN
	///	</summary>
	public interface IDateRange
	{
		///	<summary>
		///	The <c>BeginDate</c> field.
		///	designer:fld/8VAN/8VAO
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAO]")]
		global::Epsitec.Common.Types.Date? BeginDate
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/8VAN/8VAP
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAP]")]
		global::Epsitec.Common.Types.Date? EndDate
		{
			get;
			set;
		}
	}
	public static partial class IDateRangeInterfaceImplementation
	{
		public static global::Epsitec.Common.Types.Date? GetBeginDate(global::Epsitec.Cresus.Core.Entities.IDateRange obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::Epsitec.Common.Types.Date?> ("[8VAO]");
		}
		public static void SetBeginDate(global::Epsitec.Cresus.Core.Entities.IDateRange obj, global::Epsitec.Common.Types.Date? value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::Epsitec.Common.Types.Date? oldValue = obj.BeginDate;
			if (oldValue != value || !entity.IsFieldDefined("[8VAO]"))
			{
				IDateRangeInterfaceImplementation.OnBeginDateChanging (obj, oldValue, value);
				entity.SetField<global::Epsitec.Common.Types.Date?> ("[8VAO]", oldValue, value);
				IDateRangeInterfaceImplementation.OnBeginDateChanged (obj, oldValue, value);
			}
		}
		static partial void OnBeginDateChanged(global::Epsitec.Cresus.Core.Entities.IDateRange obj, global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		static partial void OnBeginDateChanging(global::Epsitec.Cresus.Core.Entities.IDateRange obj, global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		public static global::Epsitec.Common.Types.Date? GetEndDate(global::Epsitec.Cresus.Core.Entities.IDateRange obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::Epsitec.Common.Types.Date?> ("[8VAP]");
		}
		public static void SetEndDate(global::Epsitec.Cresus.Core.Entities.IDateRange obj, global::Epsitec.Common.Types.Date? value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::Epsitec.Common.Types.Date? oldValue = obj.EndDate;
			if (oldValue != value || !entity.IsFieldDefined("[8VAP]"))
			{
				IDateRangeInterfaceImplementation.OnEndDateChanging (obj, oldValue, value);
				entity.SetField<global::Epsitec.Common.Types.Date?> ("[8VAP]", oldValue, value);
				IDateRangeInterfaceImplementation.OnEndDateChanged (obj, oldValue, value);
			}
		}
		static partial void OnEndDateChanged(global::Epsitec.Cresus.Core.Entities.IDateRange obj, global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
		static partial void OnEndDateChanging(global::Epsitec.Cresus.Core.Entities.IDateRange obj, global::Epsitec.Common.Types.Date? oldValue, global::Epsitec.Common.Types.Date? newValue);
	}
}
#endregion

#region Epsitec.Cresus.Core.Comment Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>Comment</c> entity.
	///	designer:cap/8VAQ
	///	</summary>
	public partial class CommentEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/8VAQ/8VA3
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
		///	The <c>Text</c> field.
		///	designer:fld/8VAQ/8VAR
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAR]")]
		public global::Epsitec.Common.Types.FormattedText Text
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Types.FormattedText> ("[8VAR]");
			}
			set
			{
				global::Epsitec.Common.Types.FormattedText oldValue = this.Text;
				if (oldValue != value || !this.IsFieldDefined("[8VAR]"))
				{
					this.OnTextChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Types.FormattedText> ("[8VAR]", oldValue, value);
					this.OnTextChanged (oldValue, value);
				}
			}
		}
		
		partial void OnTextChanging(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		partial void OnTextChanged(global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.CommentEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.CommentEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1000, 10, 26);	// [8VAQ]
		public static readonly string EntityStructuredTypeKey = "[8VAQ]";
	}
}
#endregion

#region Epsitec.Cresus.Core.IComments Interface
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>IComments</c> entity.
	///	designer:cap/8VAS
	///	</summary>
	public interface IComments
	{
		///	<summary>
		///	The <c>Comments</c> field.
		///	designer:fld/8VAS/8VAT
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAT]")]
		global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.CommentEntity> Comments
		{
			get;
		}
	}
	public static partial class ICommentsInterfaceImplementation
	{
		public static global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.CommentEntity> GetComments(global::Epsitec.Cresus.Core.Entities.IComments obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.CommentEntity> ("[8VAT]");
		}
	}
}
#endregion

#region Epsitec.Cresus.Core.Language Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>Language</c> entity.
	///	designer:cap/8VAU
	///	</summary>
	public partial class LanguageEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.INameDescription
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/8VAU/8VA3
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
		///	designer:fld/8VAU/8VA7
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
		///	designer:fld/8VAU/8VA8
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
		///	The <c>IsoLanguageCode</c> field.
		///	designer:fld/8VAU/8VAV
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAV]")]
		public string IsoLanguageCode
		{
			get
			{
				return this.GetField<string> ("[8VAV]");
			}
			set
			{
				string oldValue = this.IsoLanguageCode;
				if (oldValue != value || !this.IsFieldDefined("[8VAV]"))
				{
					this.OnIsoLanguageCodeChanging (oldValue, value);
					this.SetField<string> ("[8VAV]", oldValue, value);
					this.OnIsoLanguageCodeChanged (oldValue, value);
				}
			}
		}
		
		partial void OnIsoLanguageCodeChanging(string oldValue, string newValue);
		partial void OnIsoLanguageCodeChanged(string oldValue, string newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.LanguageEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.LanguageEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1000, 10, 30);	// [8VAU]
		public static readonly string EntityStructuredTypeKey = "[8VAU]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<LanguageEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.IReferenceNumber Interface
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>IReferenceNumber</c> entity.
	///	designer:cap/8VA01
	///	</summary>
	public interface IReferenceNumber
	{
		///	<summary>
		///	The <c>IdA</c> field.
		///	designer:fld/8VA01/8VA11
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA11]")]
		string IdA
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>IdB</c> field.
		///	designer:fld/8VA01/8VA21
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA21]")]
		string IdB
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>IdC</c> field.
		///	designer:fld/8VA01/8VA31
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA31]")]
		string IdC
		{
			get;
			set;
		}
	}
	public static partial class IReferenceNumberInterfaceImplementation
	{
		public static string GetIdA(global::Epsitec.Cresus.Core.Entities.IReferenceNumber obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[8VA11]");
		}
		public static void SetIdA(global::Epsitec.Cresus.Core.Entities.IReferenceNumber obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.IdA;
			if (oldValue != value || !entity.IsFieldDefined("[8VA11]"))
			{
				IReferenceNumberInterfaceImplementation.OnIdAChanging (obj, oldValue, value);
				entity.SetField<string> ("[8VA11]", oldValue, value);
				IReferenceNumberInterfaceImplementation.OnIdAChanged (obj, oldValue, value);
			}
		}
		static partial void OnIdAChanged(global::Epsitec.Cresus.Core.Entities.IReferenceNumber obj, string oldValue, string newValue);
		static partial void OnIdAChanging(global::Epsitec.Cresus.Core.Entities.IReferenceNumber obj, string oldValue, string newValue);
		public static string GetIdB(global::Epsitec.Cresus.Core.Entities.IReferenceNumber obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[8VA21]");
		}
		public static void SetIdB(global::Epsitec.Cresus.Core.Entities.IReferenceNumber obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.IdB;
			if (oldValue != value || !entity.IsFieldDefined("[8VA21]"))
			{
				IReferenceNumberInterfaceImplementation.OnIdBChanging (obj, oldValue, value);
				entity.SetField<string> ("[8VA21]", oldValue, value);
				IReferenceNumberInterfaceImplementation.OnIdBChanged (obj, oldValue, value);
			}
		}
		static partial void OnIdBChanged(global::Epsitec.Cresus.Core.Entities.IReferenceNumber obj, string oldValue, string newValue);
		static partial void OnIdBChanging(global::Epsitec.Cresus.Core.Entities.IReferenceNumber obj, string oldValue, string newValue);
		public static string GetIdC(global::Epsitec.Cresus.Core.Entities.IReferenceNumber obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[8VA31]");
		}
		public static void SetIdC(global::Epsitec.Cresus.Core.Entities.IReferenceNumber obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.IdC;
			if (oldValue != value || !entity.IsFieldDefined("[8VA31]"))
			{
				IReferenceNumberInterfaceImplementation.OnIdCChanging (obj, oldValue, value);
				entity.SetField<string> ("[8VA31]", oldValue, value);
				IReferenceNumberInterfaceImplementation.OnIdCChanged (obj, oldValue, value);
			}
		}
		static partial void OnIdCChanged(global::Epsitec.Cresus.Core.Entities.IReferenceNumber obj, string oldValue, string newValue);
		static partial void OnIdCChanging(global::Epsitec.Cresus.Core.Entities.IReferenceNumber obj, string oldValue, string newValue);
	}
}
#endregion

#region Epsitec.Cresus.Core.XmlBlob Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>XmlBlob</c> entity.
	///	designer:cap/8VA41
	///	</summary>
	public partial class XmlBlobEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.IItemCode
	{
		#region IItemCode Members
		///	<summary>
		///	The <c>Code</c> field.
		///	designer:fld/8VA41/8VA5
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
		///	designer:fld/8VA41/8VA51
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA51]")]
		public global::System.Byte[] Data
		{
			get
			{
				return this.GetField<global::System.Byte[]> ("[8VA51]");
			}
			set
			{
				global::System.Byte[] oldValue = this.Data;
				if (oldValue != value || !this.IsFieldDefined("[8VA51]"))
				{
					this.OnDataChanging (oldValue, value);
					this.SetField<global::System.Byte[]> ("[8VA51]", oldValue, value);
					this.OnDataChanged (oldValue, value);
				}
			}
		}
		
		partial void OnDataChanging(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		partial void OnDataChanged(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.XmlBlobEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.XmlBlobEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1000, 10, 36);	// [8VA41]
		public static readonly string EntityStructuredTypeKey = "[8VA41]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<XmlBlobEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Volatile)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.DateRange Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>DateRange</c> entity.
	///	designer:cap/8VA61
	///	</summary>
	public partial class DateRangeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.IDateRange
	{
		#region IDateRange Members
		///	<summary>
		///	The <c>BeginDate</c> field.
		///	designer:fld/8VA61/8VAO
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
		///	designer:fld/8VA61/8VAP
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
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.DateRangeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.DateRangeEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1000, 10, 38);	// [8VA61]
		public static readonly string EntityStructuredTypeKey = "[8VA61]";
	}
}
#endregion

#region Epsitec.Cresus.Core.GeneratorDefinition Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>GeneratorDefinition</c> entity.
	///	designer:cap/8VA91
	///	</summary>
	public partial class GeneratorDefinitionEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.INameDescription
	{
		#region INameDescription Members
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/8VA91/8VA7
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
		///	designer:fld/8VA91/8VA8
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
		///	The <c>Entity</c> field.
		///	designer:fld/8VA91/8VAA1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAA1]")]
		public string Entity
		{
			get
			{
				return this.GetField<string> ("[8VAA1]");
			}
			set
			{
				string oldValue = this.Entity;
				if (oldValue != value || !this.IsFieldDefined("[8VAA1]"))
				{
					this.OnEntityChanging (oldValue, value);
					this.SetField<string> ("[8VAA1]", oldValue, value);
					this.OnEntityChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>IdField</c> field.
		///	designer:fld/8VA91/8VAB1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAB1]")]
		public string IdField
		{
			get
			{
				return this.GetField<string> ("[8VAB1]");
			}
			set
			{
				string oldValue = this.IdField;
				if (oldValue != value || !this.IsFieldDefined("[8VAB1]"))
				{
					this.OnIdFieldChanging (oldValue, value);
					this.SetField<string> ("[8VAB1]", oldValue, value);
					this.OnIdFieldChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Format</c> field.
		///	designer:fld/8VA91/8VAC1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAC1]")]
		public string Format
		{
			get
			{
				return this.GetField<string> ("[8VAC1]");
			}
			set
			{
				string oldValue = this.Format;
				if (oldValue != value || !this.IsFieldDefined("[8VAC1]"))
				{
					this.OnFormatChanging (oldValue, value);
					this.SetField<string> ("[8VAC1]", oldValue, value);
					this.OnFormatChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Key</c> field.
		///	designer:fld/8VA91/8VAD1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAD1]")]
		public string Key
		{
			get
			{
				return this.GetField<string> ("[8VAD1]");
			}
			set
			{
				string oldValue = this.Key;
				if (oldValue != value || !this.IsFieldDefined("[8VAD1]"))
				{
					this.OnKeyChanging (oldValue, value);
					this.SetField<string> ("[8VAD1]", oldValue, value);
					this.OnKeyChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>InitialValue</c> field.
		///	designer:fld/8VA91/8VAE1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAE1]")]
		public long InitialValue
		{
			get
			{
				return this.GetField<long> ("[8VAE1]");
			}
			set
			{
				long oldValue = this.InitialValue;
				if (oldValue != value || !this.IsFieldDefined("[8VAE1]"))
				{
					this.OnInitialValueChanging (oldValue, value);
					this.SetField<long> ("[8VAE1]", oldValue, value);
					this.OnInitialValueChanged (oldValue, value);
				}
			}
		}
		
		partial void OnEntityChanging(string oldValue, string newValue);
		partial void OnEntityChanged(string oldValue, string newValue);
		partial void OnIdFieldChanging(string oldValue, string newValue);
		partial void OnIdFieldChanged(string oldValue, string newValue);
		partial void OnFormatChanging(string oldValue, string newValue);
		partial void OnFormatChanged(string oldValue, string newValue);
		partial void OnKeyChanging(string oldValue, string newValue);
		partial void OnKeyChanged(string oldValue, string newValue);
		partial void OnInitialValueChanging(long oldValue, long newValue);
		partial void OnInitialValueChanged(long oldValue, long newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.GeneratorDefinitionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.GeneratorDefinitionEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1000, 10, 41);	// [8VA91]
		public static readonly string EntityStructuredTypeKey = "[8VA91]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<GeneratorDefinitionEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.IItemCodeUniversalReference Interface
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>IItemCodeUniversalReference</c> entity.
	///	designer:cap/8VAI1
	///	</summary>
	public interface IItemCodeUniversalReference
	{
		///	<summary>
		///	The <c>RefCode</c> field.
		///	designer:fld/8VAI1/8VAJ1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAJ1]")]
		string RefCode
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>RefEntityId</c> field.
		///	designer:fld/8VAI1/8VAK1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAK1]")]
		string RefEntityId
		{
			get;
			set;
		}
	}
	public static partial class IItemCodeUniversalReferenceInterfaceImplementation
	{
		public static string GetRefCode(global::Epsitec.Cresus.Core.Entities.IItemCodeUniversalReference obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[8VAJ1]");
		}
		public static void SetRefCode(global::Epsitec.Cresus.Core.Entities.IItemCodeUniversalReference obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.RefCode;
			if (oldValue != value || !entity.IsFieldDefined("[8VAJ1]"))
			{
				IItemCodeUniversalReferenceInterfaceImplementation.OnRefCodeChanging (obj, oldValue, value);
				entity.SetField<string> ("[8VAJ1]", oldValue, value);
				IItemCodeUniversalReferenceInterfaceImplementation.OnRefCodeChanged (obj, oldValue, value);
			}
		}
		static partial void OnRefCodeChanged(global::Epsitec.Cresus.Core.Entities.IItemCodeUniversalReference obj, string oldValue, string newValue);
		static partial void OnRefCodeChanging(global::Epsitec.Cresus.Core.Entities.IItemCodeUniversalReference obj, string oldValue, string newValue);
		public static string GetRefEntityId(global::Epsitec.Cresus.Core.Entities.IItemCodeUniversalReference obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[8VAK1]");
		}
		public static void SetRefEntityId(global::Epsitec.Cresus.Core.Entities.IItemCodeUniversalReference obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.RefEntityId;
			if (oldValue != value || !entity.IsFieldDefined("[8VAK1]"))
			{
				IItemCodeUniversalReferenceInterfaceImplementation.OnRefEntityIdChanging (obj, oldValue, value);
				entity.SetField<string> ("[8VAK1]", oldValue, value);
				IItemCodeUniversalReferenceInterfaceImplementation.OnRefEntityIdChanged (obj, oldValue, value);
			}
		}
		static partial void OnRefEntityIdChanged(global::Epsitec.Cresus.Core.Entities.IItemCodeUniversalReference obj, string oldValue, string newValue);
		static partial void OnRefEntityIdChanging(global::Epsitec.Cresus.Core.Entities.IItemCodeUniversalReference obj, string oldValue, string newValue);
	}
}
#endregion

#region Epsitec.Cresus.Core.IEntityAttribute Interface
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>IEntityAttribute</c> entity.
	///	designer:cap/8VAL1
	///	</summary>
	public interface IEntityAttribute : global::Epsitec.Cresus.Core.Entities.IDateTimeRange, global::Epsitec.Cresus.Core.Entities.IItemCodeUniversalReference
	{
		///	<summary>
		///	The <c>CreationUser</c> field.
		///	designer:fld/8VAL1/8VAM1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAM1]")]
		string CreationUser
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>CreationDate</c> field.
		///	designer:fld/8VAL1/8VAN1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAN1]")]
		global::System.DateTime? CreationDate
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>AttributeState</c> field.
		///	designer:fld/8VAL1/8VAT1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAT1]")]
		global::Epsitec.Cresus.Core.Data.AttributeState AttributeState
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>TargetUser</c> field.
		///	designer:fld/8VAL1/8VAU1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAU1]")]
		string TargetUser
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>TargetUserGroup</c> field.
		///	designer:fld/8VAL1/8VAV1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAV1]")]
		string TargetUserGroup
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>WorkflowThreadCode</c> field.
		///	designer:fld/8VAL1/8VA32
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA32]")]
		string WorkflowThreadCode
		{
			get;
			set;
		}
	}
	public static partial class IEntityAttributeInterfaceImplementation
	{
		public static string GetCreationUser(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[8VAM1]");
		}
		public static void SetCreationUser(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.CreationUser;
			if (oldValue != value || !entity.IsFieldDefined("[8VAM1]"))
			{
				IEntityAttributeInterfaceImplementation.OnCreationUserChanging (obj, oldValue, value);
				entity.SetField<string> ("[8VAM1]", oldValue, value);
				IEntityAttributeInterfaceImplementation.OnCreationUserChanged (obj, oldValue, value);
			}
		}
		static partial void OnCreationUserChanged(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj, string oldValue, string newValue);
		static partial void OnCreationUserChanging(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj, string oldValue, string newValue);
		public static global::System.DateTime? GetCreationDate(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::System.DateTime?> ("[8VAN1]");
		}
		public static void SetCreationDate(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj, global::System.DateTime? value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::System.DateTime? oldValue = obj.CreationDate;
			if (oldValue != value || !entity.IsFieldDefined("[8VAN1]"))
			{
				IEntityAttributeInterfaceImplementation.OnCreationDateChanging (obj, oldValue, value);
				entity.SetField<global::System.DateTime?> ("[8VAN1]", oldValue, value);
				IEntityAttributeInterfaceImplementation.OnCreationDateChanged (obj, oldValue, value);
			}
		}
		static partial void OnCreationDateChanged(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj, global::System.DateTime? oldValue, global::System.DateTime? newValue);
		static partial void OnCreationDateChanging(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj, global::System.DateTime? oldValue, global::System.DateTime? newValue);
		public static global::Epsitec.Cresus.Core.Data.AttributeState GetAttributeState(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::Epsitec.Cresus.Core.Data.AttributeState> ("[8VAT1]");
		}
		public static void SetAttributeState(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj, global::Epsitec.Cresus.Core.Data.AttributeState value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::Epsitec.Cresus.Core.Data.AttributeState oldValue = obj.AttributeState;
			if (oldValue != value || !entity.IsFieldDefined("[8VAT1]"))
			{
				IEntityAttributeInterfaceImplementation.OnAttributeStateChanging (obj, oldValue, value);
				entity.SetField<global::Epsitec.Cresus.Core.Data.AttributeState> ("[8VAT1]", oldValue, value);
				IEntityAttributeInterfaceImplementation.OnAttributeStateChanged (obj, oldValue, value);
			}
		}
		static partial void OnAttributeStateChanged(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj, global::Epsitec.Cresus.Core.Data.AttributeState oldValue, global::Epsitec.Cresus.Core.Data.AttributeState newValue);
		static partial void OnAttributeStateChanging(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj, global::Epsitec.Cresus.Core.Data.AttributeState oldValue, global::Epsitec.Cresus.Core.Data.AttributeState newValue);
		public static string GetTargetUser(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[8VAU1]");
		}
		public static void SetTargetUser(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.TargetUser;
			if (oldValue != value || !entity.IsFieldDefined("[8VAU1]"))
			{
				IEntityAttributeInterfaceImplementation.OnTargetUserChanging (obj, oldValue, value);
				entity.SetField<string> ("[8VAU1]", oldValue, value);
				IEntityAttributeInterfaceImplementation.OnTargetUserChanged (obj, oldValue, value);
			}
		}
		static partial void OnTargetUserChanged(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj, string oldValue, string newValue);
		static partial void OnTargetUserChanging(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj, string oldValue, string newValue);
		public static string GetTargetUserGroup(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[8VAV1]");
		}
		public static void SetTargetUserGroup(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.TargetUserGroup;
			if (oldValue != value || !entity.IsFieldDefined("[8VAV1]"))
			{
				IEntityAttributeInterfaceImplementation.OnTargetUserGroupChanging (obj, oldValue, value);
				entity.SetField<string> ("[8VAV1]", oldValue, value);
				IEntityAttributeInterfaceImplementation.OnTargetUserGroupChanged (obj, oldValue, value);
			}
		}
		static partial void OnTargetUserGroupChanged(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj, string oldValue, string newValue);
		static partial void OnTargetUserGroupChanging(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj, string oldValue, string newValue);
		public static string GetWorkflowThreadCode(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[8VA32]");
		}
		public static void SetWorkflowThreadCode(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.WorkflowThreadCode;
			if (oldValue != value || !entity.IsFieldDefined("[8VA32]"))
			{
				IEntityAttributeInterfaceImplementation.OnWorkflowThreadCodeChanging (obj, oldValue, value);
				entity.SetField<string> ("[8VA32]", oldValue, value);
				IEntityAttributeInterfaceImplementation.OnWorkflowThreadCodeChanged (obj, oldValue, value);
			}
		}
		static partial void OnWorkflowThreadCodeChanged(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj, string oldValue, string newValue);
		static partial void OnWorkflowThreadCodeChanging(global::Epsitec.Cresus.Core.Entities.IEntityAttribute obj, string oldValue, string newValue);
	}
}
#endregion

#region Epsitec.Cresus.Core.EntityAttribute Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>EntityAttribute</c> entity.
	///	designer:cap/8VA02
	///	</summary>
	public partial class EntityAttributeEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IEntityAttribute
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/8VA02/8VA3
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
		#region IItemCodeUniversalReference Members
		///	<summary>
		///	The <c>RefCode</c> field.
		///	designer:fld/8VA02/8VAJ1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAJ1]")]
		public string RefCode
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IItemCodeUniversalReferenceInterfaceImplementation.GetRefCode (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IItemCodeUniversalReferenceInterfaceImplementation.SetRefCode (this, value);
			}
		}
		#endregion
		#region IDateTimeRange Members
		///	<summary>
		///	The <c>BeginDate</c> field.
		///	designer:fld/8VA02/8VAK
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
		#endregion
		#region IItemCodeUniversalReference Members
		///	<summary>
		///	The <c>RefEntityId</c> field.
		///	designer:fld/8VA02/8VAK1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAK1]")]
		public string RefEntityId
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IItemCodeUniversalReferenceInterfaceImplementation.GetRefEntityId (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IItemCodeUniversalReferenceInterfaceImplementation.SetRefEntityId (this, value);
			}
		}
		#endregion
		#region IDateTimeRange Members
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/8VA02/8VAL
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
		#region IEntityAttribute Members
		///	<summary>
		///	The <c>CreationUser</c> field.
		///	designer:fld/8VA02/8VAM1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAM1]")]
		public string CreationUser
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IEntityAttributeInterfaceImplementation.GetCreationUser (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IEntityAttributeInterfaceImplementation.SetCreationUser (this, value);
			}
		}
		///	<summary>
		///	The <c>CreationDate</c> field.
		///	designer:fld/8VA02/8VAN1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAN1]")]
		public global::System.DateTime? CreationDate
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IEntityAttributeInterfaceImplementation.GetCreationDate (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IEntityAttributeInterfaceImplementation.SetCreationDate (this, value);
			}
		}
		///	<summary>
		///	The <c>AttributeState</c> field.
		///	designer:fld/8VA02/8VAT1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAT1]")]
		public global::Epsitec.Cresus.Core.Data.AttributeState AttributeState
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IEntityAttributeInterfaceImplementation.GetAttributeState (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IEntityAttributeInterfaceImplementation.SetAttributeState (this, value);
			}
		}
		///	<summary>
		///	The <c>TargetUser</c> field.
		///	designer:fld/8VA02/8VAU1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAU1]")]
		public string TargetUser
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IEntityAttributeInterfaceImplementation.GetTargetUser (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IEntityAttributeInterfaceImplementation.SetTargetUser (this, value);
			}
		}
		///	<summary>
		///	The <c>TargetUserGroup</c> field.
		///	designer:fld/8VA02/8VAV1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAV1]")]
		public string TargetUserGroup
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IEntityAttributeInterfaceImplementation.GetTargetUserGroup (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IEntityAttributeInterfaceImplementation.SetTargetUserGroup (this, value);
			}
		}
		///	<summary>
		///	The <c>WorkflowThreadCode</c> field.
		///	designer:fld/8VA02/8VA32
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA32]")]
		public string WorkflowThreadCode
		{
			get
			{
				return global::Epsitec.Cresus.Core.Entities.IEntityAttributeInterfaceImplementation.GetWorkflowThreadCode (this);
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.IEntityAttributeInterfaceImplementation.SetWorkflowThreadCode (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>AttributeCategory</c> field.
		///	designer:fld/8VA02/8VA22
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VA22]")]
		public global::Epsitec.Cresus.Core.Entities.EntityAttributeCategoryEntity AttributeCategory
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.EntityAttributeCategoryEntity> ("[8VA22]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.EntityAttributeCategoryEntity oldValue = this.AttributeCategory;
				if (oldValue != value || !this.IsFieldDefined("[8VA22]"))
				{
					this.OnAttributeCategoryChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.EntityAttributeCategoryEntity> ("[8VA22]", oldValue, value);
					this.OnAttributeCategoryChanged (oldValue, value);
				}
			}
		}
		
		partial void OnAttributeCategoryChanging(global::Epsitec.Cresus.Core.Entities.EntityAttributeCategoryEntity oldValue, global::Epsitec.Cresus.Core.Entities.EntityAttributeCategoryEntity newValue);
		partial void OnAttributeCategoryChanged(global::Epsitec.Cresus.Core.Entities.EntityAttributeCategoryEntity oldValue, global::Epsitec.Cresus.Core.Entities.EntityAttributeCategoryEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.EntityAttributeEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.EntityAttributeEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1000, 10, 64);	// [8VA02]
		public static readonly string EntityStructuredTypeKey = "[8VA02]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<EntityAttributeEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.EntityAttributeCategory Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>EntityAttributeCategory</c> entity.
	///	designer:cap/8VA12
	///	</summary>
	public partial class EntityAttributeCategoryEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/8VA12/8VA3
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
		///	designer:fld/8VA12/8VA5
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
		///	designer:fld/8VA12/8VA7
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
		///	designer:fld/8VA12/8VA8
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
		///	The <c>AttributeType</c> field.
		///	designer:fld/8VA12/8VAA2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[8VAA2]")]
		public global::Epsitec.Cresus.Core.Data.AttributeType AttributeType
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Data.AttributeType> ("[8VAA2]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Data.AttributeType oldValue = this.AttributeType;
				if (oldValue != value || !this.IsFieldDefined("[8VAA2]"))
				{
					this.OnAttributeTypeChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Data.AttributeType> ("[8VAA2]", oldValue, value);
					this.OnAttributeTypeChanged (oldValue, value);
				}
			}
		}
		
		partial void OnAttributeTypeChanging(global::Epsitec.Cresus.Core.Data.AttributeType oldValue, global::Epsitec.Cresus.Core.Data.AttributeType newValue);
		partial void OnAttributeTypeChanged(global::Epsitec.Cresus.Core.Data.AttributeType oldValue, global::Epsitec.Cresus.Core.Data.AttributeType newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.EntityAttributeCategoryEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.EntityAttributeCategoryEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1000, 10, 65);	// [8VA12]
		public static readonly string EntityStructuredTypeKey = "[8VA12]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<EntityAttributeCategoryEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

