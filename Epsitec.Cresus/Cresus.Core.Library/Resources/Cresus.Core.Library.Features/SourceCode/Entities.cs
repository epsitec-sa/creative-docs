﻿//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[JVA]", typeof (Epsitec.Cresus.Core.Entities.ProductFeatureEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[JVA3]", typeof (Epsitec.Cresus.Core.Entities.ProductCustomizationEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[JVA5]", typeof (Epsitec.Cresus.Core.Entities.ProductSettingsEntity))]
#region Epsitec.Cresus.Core.ProductFeature Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ProductFeature</c> entity.
	///	designer:cap/JVA
	///	</summary>
	public partial class ProductFeatureEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IItemCode, global::Epsitec.Cresus.Core.Entities.INameDescription, global::Epsitec.Cresus.Core.Entities.IDataHash
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/JVA/8VA3
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
		///	designer:fld/JVA/8VA5
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
		///	designer:fld/JVA/8VA7
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
		#region IDataHash Members
		///	<summary>
		///	The <c>WeakHash</c> field.
		///	designer:fld/JVA/8VAC
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
		#region INameDescription Members
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/JVA/8VA8
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
		#region IDataHash Members
		///	<summary>
		///	The <c>StrongHash</c> field.
		///	designer:fld/JVA/8VAD
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
		///	<summary>
		///	The <c>EnabledSettings</c> field.
		///	designer:fld/JVA/JVA1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[JVA1]")]
		public global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity EnabledSettings
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity> ("[JVA1]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity oldValue = this.EnabledSettings;
				if (oldValue != value || !this.IsFieldDefined("[JVA1]"))
				{
					this.OnEnabledSettingsChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity> ("[JVA1]", oldValue, value);
					this.OnEnabledSettingsChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>DisabledSettings</c> field.
		///	designer:fld/JVA/JVA2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[JVA2]")]
		public global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity DisabledSettings
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity> ("[JVA2]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity oldValue = this.DisabledSettings;
				if (oldValue != value || !this.IsFieldDefined("[JVA2]"))
				{
					this.OnDisabledSettingsChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity> ("[JVA2]", oldValue, value);
					this.OnDisabledSettingsChanged (oldValue, value);
				}
			}
		}
		
		partial void OnEnabledSettingsChanging(global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity oldValue, global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity newValue);
		partial void OnEnabledSettingsChanged(global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity oldValue, global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity newValue);
		partial void OnDisabledSettingsChanging(global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity oldValue, global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity newValue);
		partial void OnDisabledSettingsChanged(global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity oldValue, global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ProductFeatureEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ProductFeatureEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1011, 10, 0);	// [JVA]
		public static readonly string EntityStructuredTypeKey = "[JVA]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ProductFeatureEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.ProductCustomization Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ProductCustomization</c> entity.
	///	designer:cap/JVA3
	///	</summary>
	public partial class ProductCustomizationEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.INameDescription, global::Epsitec.Cresus.Core.Entities.IDateTimeRange
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/JVA3/8VA3
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
		///	designer:fld/JVA3/8VA7
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
		#region IDateTimeRange Members
		///	<summary>
		///	The <c>BeginDate</c> field.
		///	designer:fld/JVA3/8VAK
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
		#region INameDescription Members
		///	<summary>
		///	The <c>Description</c> field.
		///	designer:fld/JVA3/8VA8
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
		#region IDateTimeRange Members
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/JVA3/8VAL
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
		///	The <c>Settings</c> field.
		///	designer:fld/JVA3/JVA4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[JVA4]")]
		public global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity Settings
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity> ("[JVA4]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity oldValue = this.Settings;
				if (oldValue != value || !this.IsFieldDefined("[JVA4]"))
				{
					this.OnSettingsChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity> ("[JVA4]", oldValue, value);
					this.OnSettingsChanged (oldValue, value);
				}
			}
		}
		
		partial void OnSettingsChanging(global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity oldValue, global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity newValue);
		partial void OnSettingsChanged(global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity oldValue, global::Epsitec.Cresus.Core.Entities.SoftwareUISettingsEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ProductCustomizationEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ProductCustomizationEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1011, 10, 3);	// [JVA3]
		public static readonly string EntityStructuredTypeKey = "[JVA3]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ProductCustomizationEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Stable)
			{
			}
		}
		#endregion
	}
}
#endregion

#region Epsitec.Cresus.Core.ProductSettings Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>ProductSettings</c> entity.
	///	designer:cap/JVA5
	///	</summary>
	public partial class ProductSettingsEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.IDateRange, global::Epsitec.Cresus.Core.Entities.IDataHash
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/JVA5/8VA3
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
		#region IDataHash Members
		///	<summary>
		///	The <c>WeakHash</c> field.
		///	designer:fld/JVA5/8VAC
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
		#region IDateRange Members
		///	<summary>
		///	The <c>BeginDate</c> field.
		///	designer:fld/JVA5/8VAO
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
		#region IDataHash Members
		///	<summary>
		///	The <c>StrongHash</c> field.
		///	designer:fld/JVA5/8VAD
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
		#region IDateRange Members
		///	<summary>
		///	The <c>EndDate</c> field.
		///	designer:fld/JVA5/8VAP
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
		///	The <c>LicensedFeatures</c> field.
		///	designer:fld/JVA5/JVA6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[JVA6]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.ProductFeatureEntity> LicensedFeatures
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.ProductFeatureEntity> ("[JVA6]");
			}
		}
		
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.ProductSettingsEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.ProductSettingsEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1011, 10, 5);	// [JVA5]
		public static readonly string EntityStructuredTypeKey = "[JVA5]";
		
		#region Repository Class
		public partial class Repository : global::Epsitec.Cresus.Core.Repositories.Repository<ProductSettingsEntity>
		{
			public Repository(global::Epsitec.Cresus.Core.CoreData data, global::Epsitec.Cresus.DataLayer.Context.DataContext dataContext) : base(data, dataContext, global::Epsitec.Common.Types.DataLifetimeExpectancy.Immutable)
			{
			}
		}
		#endregion
	}
}
#endregion

