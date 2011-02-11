//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[CVA]", typeof (Epsitec.Cresus.Assets.Entities.AssetObjectEntity))]
#region Epsitec.Cresus.Assets.AssetObject Entity
namespace Epsitec.Cresus.Assets.Entities
{
	///	<summary>
	///	The <c>AssetObject</c> entity.
	///	designer:cap/CVA
	///	</summary>
	public partial class AssetObjectEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Assets.Entities.ILifetime, global::Epsitec.Cresus.Assets.Entities.INameDescription
	{
		#region INameDescription Members
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/CVA/CVA2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA2]")]
		public global::Epsitec.Common.Types.FormattedText Name
		{
			get
			{
				return global::Epsitec.Cresus.Assets.Entities.INameDescriptionInterfaceImplementation.GetName (this);
			}
			set
			{
				global::Epsitec.Cresus.Assets.Entities.INameDescriptionInterfaceImplementation.SetName (this, value);
			}
		}
		#endregion
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/CVA/CVA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA5]")]
		public bool IsArchive
		{
			get
			{
				return global::Epsitec.Cresus.Assets.Entities.ILifetimeInterfaceImplementation.GetIsArchive (this);
			}
			set
			{
				global::Epsitec.Cresus.Assets.Entities.ILifetimeInterfaceImplementation.SetIsArchive (this, value);
			}
		}
		#endregion
		#region INameDescription Members
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/CVA/CVA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA3]")]
		public global::Epsitec.Common.Types.FormattedText Description
		{
			get
			{
				return global::Epsitec.Cresus.Assets.Entities.INameDescriptionInterfaceImplementation.GetDescription (this);
			}
			set
			{
				global::Epsitec.Cresus.Assets.Entities.INameDescriptionInterfaceImplementation.SetDescription (this, value);
			}
		}
		#endregion
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Assets.Entities.AssetObjectEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Assets.Entities.AssetObjectEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1004, 10, 0);	// [CVA]
		public static readonly new string EntityStructuredTypeKey = "[CVA]";
	}
}
#endregion

#region Epsitec.Cresus.Assets.INameDescription Interface
namespace Epsitec.Cresus.Assets.Entities
{
	///	<summary>
	///	The <c>INameDescription</c> entity.
	///	designer:cap/CVA1
	///	</summary>
	public interface INameDescription
	{
		///	<summary>
		///	The <c>Name</c> field.
		///	designer:fld/CVA1/CVA2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA2]")]
		global::Epsitec.Common.Types.FormattedText Name
		{
			get;
			set;
		}
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/CVA1/CVA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA3]")]
		global::Epsitec.Common.Types.FormattedText Description
		{
			get;
			set;
		}
	}
	public static partial class INameDescriptionInterfaceImplementation
	{
		public static global::Epsitec.Common.Types.FormattedText GetName(global::Epsitec.Cresus.Assets.Entities.INameDescription obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::Epsitec.Common.Types.FormattedText> ("[CVA2]");
		}
		public static void SetName(global::Epsitec.Cresus.Assets.Entities.INameDescription obj, global::Epsitec.Common.Types.FormattedText value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::Epsitec.Common.Types.FormattedText oldValue = obj.Name;
			if (oldValue != value || !entity.IsFieldDefined("[CVA2]"))
			{
				INameDescriptionInterfaceImplementation.OnNameChanging (obj, oldValue, value);
				entity.SetField<global::Epsitec.Common.Types.FormattedText> ("[CVA2]", oldValue, value);
				INameDescriptionInterfaceImplementation.OnNameChanged (obj, oldValue, value);
			}
		}
		static partial void OnNameChanged(global::Epsitec.Cresus.Assets.Entities.INameDescription obj, global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		static partial void OnNameChanging(global::Epsitec.Cresus.Assets.Entities.INameDescription obj, global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		public static global::Epsitec.Common.Types.FormattedText GetDescription(global::Epsitec.Cresus.Assets.Entities.INameDescription obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<global::Epsitec.Common.Types.FormattedText> ("[CVA3]");
		}
		public static void SetDescription(global::Epsitec.Cresus.Assets.Entities.INameDescription obj, global::Epsitec.Common.Types.FormattedText value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			global::Epsitec.Common.Types.FormattedText oldValue = obj.Description;
			if (oldValue != value || !entity.IsFieldDefined("[CVA3]"))
			{
				INameDescriptionInterfaceImplementation.OnDescriptionChanging (obj, oldValue, value);
				entity.SetField<global::Epsitec.Common.Types.FormattedText> ("[CVA3]", oldValue, value);
				INameDescriptionInterfaceImplementation.OnDescriptionChanged (obj, oldValue, value);
			}
		}
		static partial void OnDescriptionChanged(global::Epsitec.Cresus.Assets.Entities.INameDescription obj, global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
		static partial void OnDescriptionChanging(global::Epsitec.Cresus.Assets.Entities.INameDescription obj, global::Epsitec.Common.Types.FormattedText oldValue, global::Epsitec.Common.Types.FormattedText newValue);
	}
}
#endregion

#region Epsitec.Cresus.Assets.ILifetime Interface
namespace Epsitec.Cresus.Assets.Entities
{
	///	<summary>
	///	The <c>ILifetime</c> entity.
	///	designer:cap/CVA4
	///	</summary>
	public interface ILifetime
	{
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/CVA4/CVA5
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[CVA5]")]
		bool IsArchive
		{
			get;
			set;
		}
	}
	public static partial class ILifetimeInterfaceImplementation
	{
		public static bool GetIsArchive(global::Epsitec.Cresus.Assets.Entities.ILifetime obj)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			return entity.GetField<bool> ("[CVA5]");
		}
		public static void SetIsArchive(global::Epsitec.Cresus.Assets.Entities.ILifetime obj, bool value)
		{
			global::Epsitec.Common.Support.EntityEngine.AbstractEntity entity = obj as global::Epsitec.Common.Support.EntityEngine.AbstractEntity;
			bool oldValue = obj.IsArchive;
			if (oldValue != value || !entity.IsFieldDefined("[CVA5]"))
			{
				ILifetimeInterfaceImplementation.OnIsArchiveChanging (obj, oldValue, value);
				entity.SetField<bool> ("[CVA5]", oldValue, value);
				ILifetimeInterfaceImplementation.OnIsArchiveChanged (obj, oldValue, value);
			}
		}
		static partial void OnIsArchiveChanged(global::Epsitec.Cresus.Assets.Entities.ILifetime obj, bool oldValue, bool newValue);
		static partial void OnIsArchiveChanging(global::Epsitec.Cresus.Assets.Entities.ILifetime obj, bool oldValue, bool newValue);
	}
}
#endregion

