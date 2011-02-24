//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

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

