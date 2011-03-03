//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[IVA]", typeof (Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[IVAF]", typeof (Epsitec.Cresus.Core.Entities.UnitOfMeasureGroupEntity))]
#region Epsitec.Cresus.Core.UnitOfMeasure Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>UnitOfMeasure</c> entity.
	///	designer:cap/IVA
	///	</summary>
	public partial class UnitOfMeasureEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ICategory
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/IVA/8VA3
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
		///	designer:fld/IVA/8VA5
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
		///	designer:fld/IVA/8VA7
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
		///	designer:fld/IVA/8VA8
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
		///	The <c>DivideRatio</c> field.
		///	designer:fld/IVA/IVA1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[IVA1]")]
		public global::System.Decimal DivideRatio
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[IVA1]");
			}
			set
			{
				global::System.Decimal oldValue = this.DivideRatio;
				if (oldValue != value || !this.IsFieldDefined("[IVA1]"))
				{
					this.OnDivideRatioChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[IVA1]", oldValue, value);
					this.OnDivideRatioChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>MultiplyRatio</c> field.
		///	designer:fld/IVA/IVA2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[IVA2]")]
		public global::System.Decimal MultiplyRatio
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[IVA2]");
			}
			set
			{
				global::System.Decimal oldValue = this.MultiplyRatio;
				if (oldValue != value || !this.IsFieldDefined("[IVA2]"))
				{
					this.OnMultiplyRatioChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[IVA2]", oldValue, value);
					this.OnMultiplyRatioChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>SmallestIncrement</c> field.
		///	designer:fld/IVA/IVA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[IVA3]")]
		public global::System.Decimal SmallestIncrement
		{
			get
			{
				return this.GetField<global::System.Decimal> ("[IVA3]");
			}
			set
			{
				global::System.Decimal oldValue = this.SmallestIncrement;
				if (oldValue != value || !this.IsFieldDefined("[IVA3]"))
				{
					this.OnSmallestIncrementChanging (oldValue, value);
					this.SetField<global::System.Decimal> ("[IVA3]", oldValue, value);
					this.OnSmallestIncrementChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Category</c> field.
		///	designer:fld/IVA/IVA4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[IVA4]")]
		public global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory Category
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory> ("[IVA4]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory oldValue = this.Category;
				if (oldValue != value || !this.IsFieldDefined("[IVA4]"))
				{
					this.OnCategoryChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory> ("[IVA4]", oldValue, value);
					this.OnCategoryChanged (oldValue, value);
				}
			}
		}
		
		partial void OnDivideRatioChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnDivideRatioChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnMultiplyRatioChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnMultiplyRatioChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnSmallestIncrementChanging(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnSmallestIncrementChanged(global::System.Decimal oldValue, global::System.Decimal newValue);
		partial void OnCategoryChanging(global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory oldValue, global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory newValue);
		partial void OnCategoryChanged(global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory oldValue, global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1010, 10, 0);	// [IVA]
		public static readonly new string EntityStructuredTypeKey = "[IVA]";
	}
}
#endregion

#region Epsitec.Cresus.Core.UnitOfMeasureGroup Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>UnitOfMeasureGroup</c> entity.
	///	designer:cap/IVAF
	///	</summary>
	public partial class UnitOfMeasureGroupEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity, global::Epsitec.Cresus.Core.Entities.ILifetime, global::Epsitec.Cresus.Core.Entities.INameDescription
	{
		#region ILifetime Members
		///	<summary>
		///	The <c>IsArchive</c> field.
		///	designer:fld/IVAF/8VA3
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
		///	designer:fld/IVAF/8VA7
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
		///	designer:fld/IVAF/8VA8
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
		///	The <c>Category</c> field.
		///	designer:fld/IVAF/IVAG
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[IVAG]")]
		public global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory Category
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory> ("[IVAG]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory oldValue = this.Category;
				if (oldValue != value || !this.IsFieldDefined("[IVAG]"))
				{
					this.OnCategoryChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory> ("[IVAG]", oldValue, value);
					this.OnCategoryChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>Units</c> field.
		///	designer:fld/IVAF/IVAH
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[IVAH]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity> Units
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Cresus.Core.Entities.UnitOfMeasureEntity> ("[IVAH]");
			}
		}
		
		partial void OnCategoryChanging(global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory oldValue, global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory newValue);
		partial void OnCategoryChanged(global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory oldValue, global::Epsitec.Cresus.Core.Business.UnitOfMeasureCategory newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.UnitOfMeasureGroupEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.UnitOfMeasureGroupEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1010, 10, 15);	// [IVAF]
		public static readonly new string EntityStructuredTypeKey = "[IVAF]";
	}
}
#endregion

