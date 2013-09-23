//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[HUA0001]", typeof (Epsitec.Cresus.Assets.Data.Entities.AssetStateEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[HUA1001]", typeof (Epsitec.Cresus.Assets.Data.Entities.AssetEventEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[HUA9001]", typeof (Epsitec.Cresus.Assets.Data.Entities.AssetObjectPropertyEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[HUAD001]", typeof (Epsitec.Cresus.Assets.Data.Entities.AssetObjectValueEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[HUAH001]", typeof (Epsitec.Cresus.Assets.Data.Entities.AssetEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[HUAJ001]", typeof (Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[HUAK001]", typeof (Epsitec.Cresus.Assets.Data.Entities.CostCenterEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[HUAL001]", typeof (Epsitec.Cresus.Assets.Data.Entities.CostCenterAssignmentEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[HUAM001]", typeof (Epsitec.Cresus.Assets.Data.Entities.AssetObjectLinkEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[HUAQ001]", typeof (Epsitec.Cresus.Assets.Data.Entities.AssetTransactionEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[HUAT001]", typeof (Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity))]
#region Epsitec.Cresus.Assets.Data.AssetState Entity
namespace Epsitec.Cresus.Assets.Data.Entities
{
	///	<summary>
	///	The <c>AssetState</c> entity.
	///	designer:cap/HUA0001
	///	</summary>
	public partial class AssetStateEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Assets.Data.Entities.ITimestamp, global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectFields
	{
		#region ITimestamp Members
		///	<summary>
		///	The <c>DateTime</c> field.
		///	designer:fld/HUA0001/HUA3001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA3001]")]
		public global::System.DateTime DateTime
		{
			get
			{
				return global::Epsitec.Cresus.Assets.Data.Entities.ITimestampInterfaceImplementation.GetDateTime (this);
			}
			set
			{
				global::Epsitec.Cresus.Assets.Data.Entities.ITimestampInterfaceImplementation.SetDateTime (this, value);
			}
		}
		#endregion
		#region IAssetObjectFields Members
		///	<summary>
		///	The <c>Properties</c> field.
		///	designer:fld/HUA0001/HUA6001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA6001]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Assets.Data.Entities.AssetObjectPropertyEntity> Properties
		{
			get
			{
				return global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectFieldsInterfaceImplementation.GetProperties (this);
			}
		}
		#endregion
		#region ITimestamp Members
		///	<summary>
		///	The <c>Position</c> field.
		///	designer:fld/HUA0001/HUA4001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA4001]")]
		public int Position
		{
			get
			{
				return global::Epsitec.Cresus.Assets.Data.Entities.ITimestampInterfaceImplementation.GetPosition (this);
			}
			set
			{
				global::Epsitec.Cresus.Assets.Data.Entities.ITimestampInterfaceImplementation.SetPosition (this, value);
			}
		}
		#endregion
		#region IAssetObjectFields Members
		///	<summary>
		///	The <c>Values</c> field.
		///	designer:fld/HUA0001/HUAG001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUAG001]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Assets.Data.Entities.AssetObjectValueEntity> Values
		{
			get
			{
				return global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectFieldsInterfaceImplementation.GetValues (this);
			}
		}
		///	<summary>
		///	The <c>AssetClass</c> field.
		///	designer:fld/HUA0001/HUAP001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUAP001]")]
		public global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity AssetClass
		{
			get
			{
				return global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectFieldsInterfaceImplementation.GetAssetClass (this);
			}
			set
			{
				global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectFieldsInterfaceImplementation.SetAssetClass (this, value);
			}
		}
		///	<summary>
		///	The <c>CostCenterAssignments</c> field.
		///	designer:fld/HUA0001/HUA1101
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA1101]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Assets.Data.Entities.CostCenterAssignmentEntity> CostCenterAssignments
		{
			get
			{
				return global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectFieldsInterfaceImplementation.GetCostCenterAssignments (this);
			}
		}
		#endregion
		///	<summary>
		///	The <c>AssetChangeSet</c> field.
		///	designer:fld/HUA0001/HUAR001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUAR001]")]
		public global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity AssetChangeSet
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity> ("[HUAR001]");
			}
			set
			{
				global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity oldValue = this.AssetChangeSet;
				if (oldValue != value || !this.IsFieldDefined("[HUAR001]"))
				{
					this.OnAssetChangeSetChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity> ("[HUAR001]", oldValue, value);
					this.OnAssetChangeSetChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Xxx</c> field.
		///	designer:fld/HUA0001/HUAS001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUAS001]")]
		public string Xxx
		{
			get
			{
				return this.GetField<string> ("[HUAS001]");
			}
			set
			{
				string oldValue = this.Xxx;
				if (oldValue != value || !this.IsFieldDefined("[HUAS001]"))
				{
					this.OnXxxChanging (oldValue, value);
					this.SetField<string> ("[HUAS001]", oldValue, value);
					this.OnXxxChanged (oldValue, value);
				}
			}
		}
		
		partial void OnAssetChangeSetChanging(global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity oldValue, global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity newValue);
		partial void OnAssetChangeSetChanged(global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity oldValue, global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity newValue);
		partial void OnXxxChanging(string oldValue, string newValue);
		partial void OnXxxChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.AssetStateEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.AssetStateEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (2001, 10, 0);	// [HUA0001]
		public static readonly string EntityStructuredTypeKey = "[HUA0001]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AssetStateEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Assets.Data.AssetEvent Entity
namespace Epsitec.Cresus.Assets.Data.Entities
{
	///	<summary>
	///	The <c>AssetEvent</c> entity.
	///	designer:cap/HUA1001
	///	</summary>
	public partial class AssetEventEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.AssetEventEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.AssetEventEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (2001, 10, 1);	// [HUA1001]
		public static readonly string EntityStructuredTypeKey = "[HUA1001]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AssetEventEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Assets.Data.ITimestamp Interface
namespace Epsitec.Cresus.Assets.Data.Entities
{
	///	<summary>
	///	The <c>ITimestamp</c> entity.
	///	designer:cap/HUA2001
	///	</summary>
	public interface ITimestamp
	{
		///	<summary>
		///	The <c>DateTime</c> field.
		///	designer:fld/HUA2001/HUA3001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA3001]")]
		global::System.DateTime DateTime
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>Position</c> field.
		///	designer:fld/HUA2001/HUA4001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA4001]")]
		int Position
		{
			get;
			set;
		}
	}
	public static partial class ITimestampInterfaceImplementation
	{
		public static global::System.DateTime GetDateTime(global::Epsitec.Cresus.Assets.Data.Entities.ITimestamp obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::System.DateTime> ("[HUA3001]");
		}
		public static void SetDateTime(global::Epsitec.Cresus.Assets.Data.Entities.ITimestamp obj, global::System.DateTime value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::System.DateTime oldValue = obj.DateTime;
			if (oldValue != value || !entity.IsFieldDefined("[HUA3001]"))
			{
				ITimestampInterfaceImplementation.OnDateTimeChanging (obj, oldValue, value);
				entity.SetField<global::System.DateTime> ("[HUA3001]", oldValue, value);
				ITimestampInterfaceImplementation.OnDateTimeChanged (obj, oldValue, value);
			}
		}
		static partial void OnDateTimeChanged(global::Epsitec.Cresus.Assets.Data.Entities.ITimestamp obj, global::System.DateTime oldValue, global::System.DateTime newValue);
		static partial void OnDateTimeChanging(global::Epsitec.Cresus.Assets.Data.Entities.ITimestamp obj, global::System.DateTime oldValue, global::System.DateTime newValue);
		public static int GetPosition(global::Epsitec.Cresus.Assets.Data.Entities.ITimestamp obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<int> ("[HUA4001]");
		}
		public static void SetPosition(global::Epsitec.Cresus.Assets.Data.Entities.ITimestamp obj, int value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			int oldValue = obj.Position;
			if (oldValue != value || !entity.IsFieldDefined("[HUA4001]"))
			{
				ITimestampInterfaceImplementation.OnPositionChanging (obj, oldValue, value);
				entity.SetField<int> ("[HUA4001]", oldValue, value);
				ITimestampInterfaceImplementation.OnPositionChanged (obj, oldValue, value);
			}
		}
		static partial void OnPositionChanged(global::Epsitec.Cresus.Assets.Data.Entities.ITimestamp obj, int oldValue, int newValue);
		static partial void OnPositionChanging(global::Epsitec.Cresus.Assets.Data.Entities.ITimestamp obj, int oldValue, int newValue);
	}
}
#endregion

#region Epsitec.Cresus.Assets.Data.IAssetObjectFields Interface
namespace Epsitec.Cresus.Assets.Data.Entities
{
	///	<summary>
	///	The <c>IAssetObjectFields</c> entity.
	///	designer:cap/HUA5001
	///	</summary>
	public interface IAssetObjectFields
	{
		///	<summary>
		///	The <c>Properties</c> field.
		///	designer:fld/HUA5001/HUA6001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA6001]")]
		global::System.Collections.Generic.IList<global::Epsitec.Cresus.Assets.Data.Entities.AssetObjectPropertyEntity> Properties
		{
			get;
		}
		///	<summary>
		///	The <c>Values</c> field.
		///	designer:fld/HUA5001/HUAG001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUAG001]")]
		global::System.Collections.Generic.IList<global::Epsitec.Cresus.Assets.Data.Entities.AssetObjectValueEntity> Values
		{
			get;
		}
		///	<summary>
		///	The <c>AssetClass</c> field.
		///	designer:fld/HUA5001/HUAP001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUAP001]")]
		global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity AssetClass
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>CostCenterAssignments</c> field.
		///	designer:fld/HUA5001/HUA1101
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA1101]")]
		global::System.Collections.Generic.IList<global::Epsitec.Cresus.Assets.Data.Entities.CostCenterAssignmentEntity> CostCenterAssignments
		{
			get;
		}
	}
	public static partial class IAssetObjectFieldsInterfaceImplementation
	{
		public static global::System.Collections.Generic.IList<global::Epsitec.Cresus.Assets.Data.Entities.AssetObjectPropertyEntity> GetProperties(global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectFields obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetFieldCollection<global::Epsitec.Cresus.Assets.Data.Entities.AssetObjectPropertyEntity> ("[HUA6001]");
		}
		public static global::System.Collections.Generic.IList<global::Epsitec.Cresus.Assets.Data.Entities.AssetObjectValueEntity> GetValues(global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectFields obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetFieldCollection<global::Epsitec.Cresus.Assets.Data.Entities.AssetObjectValueEntity> ("[HUAG001]");
		}
		public static global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity GetAssetClass(global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectFields obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity> ("[HUAP001]");
		}
		public static void SetAssetClass(global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectFields obj, global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity oldValue = obj.AssetClass;
			if (oldValue != value || !entity.IsFieldDefined("[HUAP001]"))
			{
				IAssetObjectFieldsInterfaceImplementation.OnAssetClassChanging (obj, oldValue, value);
				entity.SetField<global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity> ("[HUAP001]", oldValue, value);
				IAssetObjectFieldsInterfaceImplementation.OnAssetClassChanged (obj, oldValue, value);
			}
		}
		static partial void OnAssetClassChanged(global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectFields obj, global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity oldValue, global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity newValue);
		static partial void OnAssetClassChanging(global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectFields obj, global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity oldValue, global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity newValue);
		public static global::System.Collections.Generic.IList<global::Epsitec.Cresus.Assets.Data.Entities.CostCenterAssignmentEntity> GetCostCenterAssignments(global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectFields obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetFieldCollection<global::Epsitec.Cresus.Assets.Data.Entities.CostCenterAssignmentEntity> ("[HUA1101]");
		}
	}
}
#endregion

#region Epsitec.Cresus.Assets.Data.IAssetObjectField Interface
namespace Epsitec.Cresus.Assets.Data.Entities
{
	///	<summary>
	///	The <c>IAssetObjectField</c> entity.
	///	designer:cap/HUA7001
	///	</summary>
	public interface IAssetObjectField
	{
		///	<summary>
		///	The <c>FieldId</c> field.
		///	designer:fld/HUA7001/HUA8001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA8001]")]
		string FieldId
		{
			get;
			set;
		}
	}
	public static partial class IAssetObjectFieldInterfaceImplementation
	{
		public static string GetFieldId(global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectField obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<string> ("[HUA8001]");
		}
		public static void SetFieldId(global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectField obj, string value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			string oldValue = obj.FieldId;
			if (oldValue != value || !entity.IsFieldDefined("[HUA8001]"))
			{
				IAssetObjectFieldInterfaceImplementation.OnFieldIdChanging (obj, oldValue, value);
				entity.SetField<string> ("[HUA8001]", oldValue, value);
				IAssetObjectFieldInterfaceImplementation.OnFieldIdChanged (obj, oldValue, value);
			}
		}
		static partial void OnFieldIdChanged(global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectField obj, string oldValue, string newValue);
		static partial void OnFieldIdChanging(global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectField obj, string oldValue, string newValue);
	}
}
#endregion

#region Epsitec.Cresus.Assets.Data.AssetObjectProperty Entity
namespace Epsitec.Cresus.Assets.Data.Entities
{
	///	<summary>
	///	The <c>AssetObjectProperty</c> entity.
	///	designer:cap/HUA9001
	///	</summary>
	public partial class AssetObjectPropertyEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectField
	{
		#region IAssetObjectField Members
		///	<summary>
		///	The <c>FieldId</c> field.
		///	designer:fld/HUA9001/HUA8001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA8001]")]
		public string FieldId
		{
			get
			{
				return global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectFieldInterfaceImplementation.GetFieldId (this);
			}
			set
			{
				global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectFieldInterfaceImplementation.SetFieldId (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>ShortText</c> field.
		///	designer:fld/HUA9001/HUAA001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUAA001]")]
		public string ShortText
		{
			get
			{
				return this.GetField<string> ("[HUAA001]");
			}
			set
			{
				string oldValue = this.ShortText;
				if (oldValue != value || !this.IsFieldDefined("[HUAA001]"))
				{
					this.OnShortTextChanging (oldValue, value);
					this.SetField<string> ("[HUAA001]", oldValue, value);
					this.OnShortTextChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>LongText</c> field.
		///	designer:fld/HUA9001/HUAB001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUAB001]")]
		public string LongText
		{
			get
			{
				return this.GetField<string> ("[HUAB001]");
			}
			set
			{
				string oldValue = this.LongText;
				if (oldValue != value || !this.IsFieldDefined("[HUAB001]"))
				{
					this.OnLongTextChanging (oldValue, value);
					this.SetField<string> ("[HUAB001]", oldValue, value);
					this.OnLongTextChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Blob</c> field.
		///	designer:fld/HUA9001/HUAC001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUAC001]")]
		public global::System.Byte[] Blob
		{
			get
			{
				return this.GetField<global::System.Byte[]> ("[HUAC001]");
			}
			set
			{
				global::System.Byte[] oldValue = this.Blob;
				if (oldValue != value || !this.IsFieldDefined("[HUAC001]"))
				{
					this.OnBlobChanging (oldValue, value);
					this.SetField<global::System.Byte[]> ("[HUAC001]", oldValue, value);
					this.OnBlobChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ChangeSet</c> field.
		///	designer:fld/HUA9001/HUA6101
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA6101]")]
		public global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity ChangeSet
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity> ("[HUA6101]");
			}
			set
			{
				global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity oldValue = this.ChangeSet;
				if (oldValue != value || !this.IsFieldDefined("[HUA6101]"))
				{
					this.OnChangeSetChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity> ("[HUA6101]", oldValue, value);
					this.OnChangeSetChanged (oldValue, value);
				}
			}
		}
		
		partial void OnShortTextChanging(string oldValue, string newValue);
		partial void OnShortTextChanged(string oldValue, string newValue);
		partial void OnLongTextChanging(string oldValue, string newValue);
		partial void OnLongTextChanged(string oldValue, string newValue);
		partial void OnBlobChanging(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		partial void OnBlobChanged(global::System.Byte[] oldValue, global::System.Byte[] newValue);
		partial void OnChangeSetChanging(global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity oldValue, global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity newValue);
		partial void OnChangeSetChanged(global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity oldValue, global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.AssetObjectPropertyEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.AssetObjectPropertyEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (2001, 10, 9);	// [HUA9001]
		public static readonly string EntityStructuredTypeKey = "[HUA9001]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AssetObjectPropertyEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Assets.Data.AssetObjectValue Entity
namespace Epsitec.Cresus.Assets.Data.Entities
{
	///	<summary>
	///	The <c>AssetObjectValue</c> entity.
	///	designer:cap/HUAD001
	///	</summary>
	public partial class AssetObjectValueEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectField
	{
		#region IAssetObjectField Members
		///	<summary>
		///	The <c>FieldId</c> field.
		///	designer:fld/HUAD001/HUA8001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA8001]")]
		public string FieldId
		{
			get
			{
				return global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectFieldInterfaceImplementation.GetFieldId (this);
			}
			set
			{
				global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectFieldInterfaceImplementation.SetFieldId (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>ValueBefore</c> field.
		///	designer:fld/HUAD001/HUAE001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUAE001]")]
		public global::System.Decimal? ValueBefore
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[HUAE001]");
			}
			set
			{
				global::System.Decimal? oldValue = this.ValueBefore;
				if (oldValue != value || !this.IsFieldDefined("[HUAE001]"))
				{
					this.OnValueBeforeChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[HUAE001]", oldValue, value);
					this.OnValueBeforeChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Value</c> field.
		///	designer:fld/HUAD001/HUAF001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUAF001]")]
		public global::System.Decimal? Value
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[HUAF001]");
			}
			set
			{
				global::System.Decimal? oldValue = this.Value;
				if (oldValue != value || !this.IsFieldDefined("[HUAF001]"))
				{
					this.OnValueChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[HUAF001]", oldValue, value);
					this.OnValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ChangeSet</c> field.
		///	designer:fld/HUAD001/HUA5101
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA5101]")]
		public global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity ChangeSet
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity> ("[HUA5101]");
			}
			set
			{
				global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity oldValue = this.ChangeSet;
				if (oldValue != value || !this.IsFieldDefined("[HUA5101]"))
				{
					this.OnChangeSetChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity> ("[HUA5101]", oldValue, value);
					this.OnChangeSetChanged (oldValue, value);
				}
			}
		}
		
		partial void OnValueBeforeChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnValueBeforeChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnValueChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnValueChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnChangeSetChanging(global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity oldValue, global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity newValue);
		partial void OnChangeSetChanged(global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity oldValue, global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.AssetObjectValueEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.AssetObjectValueEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (2001, 10, 13);	// [HUAD001]
		public static readonly string EntityStructuredTypeKey = "[HUAD001]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AssetObjectValueEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Assets.Data.Asset Entity
namespace Epsitec.Cresus.Assets.Data.Entities
{
	///	<summary>
	///	The <c>Asset</c> entity.
	///	designer:cap/HUAH001
	///	</summary>
	public partial class AssetEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>AssetId</c> field.
		///	designer:fld/HUAH001/HUA4101
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA4101]")]
		public string AssetId
		{
			get
			{
				return this.GetField<string> ("[HUA4101]");
			}
			set
			{
				string oldValue = this.AssetId;
				if (oldValue != value || !this.IsFieldDefined("[HUA4101]"))
				{
					this.OnAssetIdChanging (oldValue, value);
					this.SetField<string> ("[HUA4101]", oldValue, value);
					this.OnAssetIdChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>ChangeSets</c> field.
		///	designer:fld/HUAH001/HUAI001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUAI001]", IsVirtual=true)]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity> ChangeSets
		{
			get
			{
				global::System.Collections.Generic.IList<global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity> value = default (global::System.Collections.Generic.IList<global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity>);
				this.GetChangeSets (ref value);
				return value;
			}
		}
		
		partial void OnAssetIdChanging(string oldValue, string newValue);
		partial void OnAssetIdChanged(string oldValue, string newValue);
		
		partial void GetChangeSets(ref global::System.Collections.Generic.IList<global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity> value);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.AssetEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.AssetEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (2001, 10, 17);	// [HUAH001]
		public static readonly string EntityStructuredTypeKey = "[HUAH001]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AssetEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Assets.Data.AssetClass Entity
namespace Epsitec.Cresus.Assets.Data.Entities
{
	///	<summary>
	///	The <c>AssetClass</c> entity.
	///	designer:cap/HUAJ001
	///	</summary>
	public partial class AssetClassEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (2001, 10, 19);	// [HUAJ001]
		public static readonly string EntityStructuredTypeKey = "[HUAJ001]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AssetClassEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Assets.Data.CostCenter Entity
namespace Epsitec.Cresus.Assets.Data.Entities
{
	///	<summary>
	///	The <c>CostCenter</c> entity.
	///	designer:cap/HUAK001
	///	</summary>
	public partial class CostCenterEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.CostCenterEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.CostCenterEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (2001, 10, 20);	// [HUAK001]
		public static readonly string EntityStructuredTypeKey = "[HUAK001]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<CostCenterEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Assets.Data.CostCenterAssignment Entity
namespace Epsitec.Cresus.Assets.Data.Entities
{
	///	<summary>
	///	The <c>CostCenterAssignment</c> entity.
	///	designer:cap/HUAL001
	///	</summary>
	public partial class CostCenterAssignmentEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>CostCenter</c> field.
		///	designer:fld/HUAL001/HUA2101
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA2101]")]
		public global::Epsitec.Cresus.Assets.Data.Entities.CostCenterEntity CostCenter
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Assets.Data.Entities.CostCenterEntity> ("[HUA2101]");
			}
			set
			{
				global::Epsitec.Cresus.Assets.Data.Entities.CostCenterEntity oldValue = this.CostCenter;
				if (oldValue != value || !this.IsFieldDefined("[HUA2101]"))
				{
					this.OnCostCenterChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Assets.Data.Entities.CostCenterEntity> ("[HUA2101]", oldValue, value);
					this.OnCostCenterChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Rate</c> field.
		///	designer:fld/HUAL001/HUA3101
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA3101]")]
		public global::System.Decimal? Rate
		{
			get
			{
				return this.GetField<global::System.Decimal?> ("[HUA3101]");
			}
			set
			{
				global::System.Decimal? oldValue = this.Rate;
				if (oldValue != value || !this.IsFieldDefined("[HUA3101]"))
				{
					this.OnRateChanging (oldValue, value);
					this.SetField<global::System.Decimal?> ("[HUA3101]", oldValue, value);
					this.OnRateChanged (oldValue, value);
				}
			}
		}
		
		partial void OnCostCenterChanging(global::Epsitec.Cresus.Assets.Data.Entities.CostCenterEntity oldValue, global::Epsitec.Cresus.Assets.Data.Entities.CostCenterEntity newValue);
		partial void OnCostCenterChanged(global::Epsitec.Cresus.Assets.Data.Entities.CostCenterEntity oldValue, global::Epsitec.Cresus.Assets.Data.Entities.CostCenterEntity newValue);
		partial void OnRateChanging(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		partial void OnRateChanged(global::System.Decimal? oldValue, global::System.Decimal? newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.CostCenterAssignmentEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.CostCenterAssignmentEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (2001, 10, 21);	// [HUAL001]
		public static readonly string EntityStructuredTypeKey = "[HUAL001]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<CostCenterAssignmentEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Assets.Data.AssetObjectLink Entity
namespace Epsitec.Cresus.Assets.Data.Entities
{
	///	<summary>
	///	The <c>AssetObjectLink</c> entity.
	///	designer:cap/HUAM001
	///	</summary>
	public partial class AssetObjectLinkEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectField
	{
		#region IAssetObjectField Members
		///	<summary>
		///	The <c>FieldId</c> field.
		///	designer:fld/HUAM001/HUA8001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA8001]")]
		public string FieldId
		{
			get
			{
				return global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectFieldInterfaceImplementation.GetFieldId (this);
			}
			set
			{
				global::Epsitec.Cresus.Assets.Data.Entities.IAssetObjectFieldInterfaceImplementation.SetFieldId (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>AssetClass</c> field.
		///	designer:fld/HUAM001/HUAN001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUAN001]")]
		public global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity AssetClass
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity> ("[HUAN001]");
			}
			set
			{
				global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity oldValue = this.AssetClass;
				if (oldValue != value || !this.IsFieldDefined("[HUAN001]"))
				{
					this.OnAssetClassChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity> ("[HUAN001]", oldValue, value);
					this.OnAssetClassChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>CostCenterAssignment</c> field.
		///	designer:fld/HUAM001/HUAO001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUAO001]")]
		public global::Epsitec.Cresus.Assets.Data.Entities.CostCenterAssignmentEntity CostCenterAssignment
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Assets.Data.Entities.CostCenterAssignmentEntity> ("[HUAO001]");
			}
			set
			{
				global::Epsitec.Cresus.Assets.Data.Entities.CostCenterAssignmentEntity oldValue = this.CostCenterAssignment;
				if (oldValue != value || !this.IsFieldDefined("[HUAO001]"))
				{
					this.OnCostCenterAssignmentChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Assets.Data.Entities.CostCenterAssignmentEntity> ("[HUAO001]", oldValue, value);
					this.OnCostCenterAssignmentChanged (oldValue, value);
				}
			}
		}
		
		partial void OnAssetClassChanging(global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity oldValue, global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity newValue);
		partial void OnAssetClassChanged(global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity oldValue, global::Epsitec.Cresus.Assets.Data.Entities.AssetClassEntity newValue);
		partial void OnCostCenterAssignmentChanging(global::Epsitec.Cresus.Assets.Data.Entities.CostCenterAssignmentEntity oldValue, global::Epsitec.Cresus.Assets.Data.Entities.CostCenterAssignmentEntity newValue);
		partial void OnCostCenterAssignmentChanged(global::Epsitec.Cresus.Assets.Data.Entities.CostCenterAssignmentEntity oldValue, global::Epsitec.Cresus.Assets.Data.Entities.CostCenterAssignmentEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.AssetObjectLinkEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.AssetObjectLinkEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (2001, 10, 22);	// [HUAM001]
		public static readonly string EntityStructuredTypeKey = "[HUAM001]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AssetObjectLinkEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Assets.Data.AssetTransaction Entity
namespace Epsitec.Cresus.Assets.Data.Entities
{
	///	<summary>
	///	The <c>AssetTransaction</c> entity.
	///	designer:cap/HUAQ001
	///	</summary>
	public partial class AssetTransactionEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.AssetTransactionEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.AssetTransactionEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (2001, 10, 26);	// [HUAQ001]
		public static readonly string EntityStructuredTypeKey = "[HUAQ001]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AssetTransactionEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Assets.Data.AssetChangeSet Entity
namespace Epsitec.Cresus.Assets.Data.Entities
{
	///	<summary>
	///	The <c>AssetChangeSet</c> entity.
	///	designer:cap/HUAT001
	///	</summary>
	public partial class AssetChangeSetEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Assets.Data.Entities.ITimestamp
	{
		#region ITimestamp Members
		///	<summary>
		///	The <c>DateTime</c> field.
		///	designer:fld/HUAT001/HUA3001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA3001]")]
		public global::System.DateTime DateTime
		{
			get
			{
				return global::Epsitec.Cresus.Assets.Data.Entities.ITimestampInterfaceImplementation.GetDateTime (this);
			}
			set
			{
				global::Epsitec.Cresus.Assets.Data.Entities.ITimestampInterfaceImplementation.SetDateTime (this, value);
			}
		}
		///	<summary>
		///	The <c>Position</c> field.
		///	designer:fld/HUAT001/HUA4001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA4001]")]
		public int Position
		{
			get
			{
				return global::Epsitec.Cresus.Assets.Data.Entities.ITimestampInterfaceImplementation.GetPosition (this);
			}
			set
			{
				global::Epsitec.Cresus.Assets.Data.Entities.ITimestampInterfaceImplementation.SetPosition (this, value);
			}
		}
		#endregion
		///	<summary>
		///	The <c>Asset</c> field.
		///	designer:fld/HUAT001/HUAV001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUAV001]")]
		public global::Epsitec.Cresus.Assets.Data.Entities.AssetEntity Asset
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Assets.Data.Entities.AssetEntity> ("[HUAV001]");
			}
			set
			{
				global::Epsitec.Cresus.Assets.Data.Entities.AssetEntity oldValue = this.Asset;
				if (oldValue != value || !this.IsFieldDefined("[HUAV001]"))
				{
					this.OnAssetChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Assets.Data.Entities.AssetEntity> ("[HUAV001]", oldValue, value);
					this.OnAssetChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>AssetTransaction</c> field.
		///	designer:fld/HUAT001/HUAU001
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUAU001]")]
		public global::Epsitec.Cresus.Assets.Data.Entities.AssetTransactionEntity AssetTransaction
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Assets.Data.Entities.AssetTransactionEntity> ("[HUAU001]");
			}
			set
			{
				global::Epsitec.Cresus.Assets.Data.Entities.AssetTransactionEntity oldValue = this.AssetTransaction;
				if (oldValue != value || !this.IsFieldDefined("[HUAU001]"))
				{
					this.OnAssetTransactionChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Assets.Data.Entities.AssetTransactionEntity> ("[HUAU001]", oldValue, value);
					this.OnAssetTransactionChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Xxx</c> field.
		///	designer:fld/HUAT001/HUA0101
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[HUA0101]")]
		public string Xxx
		{
			get
			{
				return this.GetField<string> ("[HUA0101]");
			}
			set
			{
				string oldValue = this.Xxx;
				if (oldValue != value || !this.IsFieldDefined("[HUA0101]"))
				{
					this.OnXxxChanging (oldValue, value);
					this.SetField<string> ("[HUA0101]", oldValue, value);
					this.OnXxxChanged (oldValue, value);
				}
			}
		}
		
		partial void OnAssetChanging(global::Epsitec.Cresus.Assets.Data.Entities.AssetEntity oldValue, global::Epsitec.Cresus.Assets.Data.Entities.AssetEntity newValue);
		partial void OnAssetChanged(global::Epsitec.Cresus.Assets.Data.Entities.AssetEntity oldValue, global::Epsitec.Cresus.Assets.Data.Entities.AssetEntity newValue);
		partial void OnAssetTransactionChanging(global::Epsitec.Cresus.Assets.Data.Entities.AssetTransactionEntity oldValue, global::Epsitec.Cresus.Assets.Data.Entities.AssetTransactionEntity newValue);
		partial void OnAssetTransactionChanged(global::Epsitec.Cresus.Assets.Data.Entities.AssetTransactionEntity oldValue, global::Epsitec.Cresus.Assets.Data.Entities.AssetTransactionEntity newValue);
		partial void OnXxxChanging(string oldValue, string newValue);
		partial void OnXxxChanged(string oldValue, string newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Assets.Data.Entities.AssetChangeSetEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (2001, 10, 29);	// [HUAT001]
		public static readonly string EntityStructuredTypeKey = "[HUAT001]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<AssetChangeSetEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

