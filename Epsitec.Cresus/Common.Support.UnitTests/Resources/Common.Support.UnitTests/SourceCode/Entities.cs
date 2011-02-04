//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AI3]", typeof (Epsitec.Common.Support.UnitTests.Entities.ValueDataEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AK3]", typeof (Epsitec.Common.Support.UnitTests.Entities.ReferenceDataEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[L0AM3]", typeof (Epsitec.Common.Support.UnitTests.Entities.CollectionDataEntity))]
#region Epsitec.Common.Support.UnitTests.ValueData Entity
namespace Epsitec.Common.Support.UnitTests.Entities
{
	///	<summary>
	///	The <c>ValueData</c> entity.
	///	designer:cap/L0AI3
	///	</summary>
	public partial class ValueDataEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Value</c> field.
		///	designer:fld/L0AI3/L0AJ3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AJ3]")]
		public int Value
		{
			get
			{
				return this.GetField<int> ("[L0AJ3]");
			}
			set
			{
				int oldValue = this.Value;
				if (oldValue != value)
				{
					this.OnValueChanging (oldValue, value);
					this.SetField<int> ("[L0AJ3]", oldValue, value);
					this.OnValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>NullableValue</c> field.
		///	designer:fld/L0AI3/L0AO3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AO3]")]
		public int? NullableValue
		{
			get
			{
				return this.GetField<int?> ("[L0AO3]");
			}
			set
			{
				int? oldValue = this.NullableValue;
				if (oldValue != value)
				{
					this.OnNullableValueChanging (oldValue, value);
					this.SetField<int?> ("[L0AO3]", oldValue, value);
					this.OnNullableValueChanged (oldValue, value);
				}
			}
		}
		
		partial void OnValueChanging(int oldValue, int newValue);
		partial void OnValueChanged(int oldValue, int newValue);
		partial void OnNullableValueChanging(int? oldValue, int? newValue);
		partial void OnNullableValueChanged(int? oldValue, int? newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.UnitTests.Entities.ValueDataEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.UnitTests.Entities.ValueDataEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 114);	// [L0AI3]
		public static readonly new string EntityStructuredTypeKey = "[L0AI3]";
	}
}
#endregion

#region Epsitec.Common.Support.UnitTests.ReferenceData Entity
namespace Epsitec.Common.Support.UnitTests.Entities
{
	///	<summary>
	///	The <c>ReferenceData</c> entity.
	///	designer:cap/L0AK3
	///	</summary>
	public partial class ReferenceDataEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Reference</c> field.
		///	designer:fld/L0AK3/L0AL3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AL3]")]
		public global::Epsitec.Common.Support.UnitTests.Entities.ValueDataEntity Reference
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Support.UnitTests.Entities.ValueDataEntity> ("[L0AL3]");
			}
			set
			{
				global::Epsitec.Common.Support.UnitTests.Entities.ValueDataEntity oldValue = this.Reference;
				if (oldValue != value)
				{
					this.OnReferenceChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Support.UnitTests.Entities.ValueDataEntity> ("[L0AL3]", oldValue, value);
					this.OnReferenceChanged (oldValue, value);
				}
			}
		}
		
		partial void OnReferenceChanging(global::Epsitec.Common.Support.UnitTests.Entities.ValueDataEntity oldValue, global::Epsitec.Common.Support.UnitTests.Entities.ValueDataEntity newValue);
		partial void OnReferenceChanged(global::Epsitec.Common.Support.UnitTests.Entities.ValueDataEntity oldValue, global::Epsitec.Common.Support.UnitTests.Entities.ValueDataEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.UnitTests.Entities.ReferenceDataEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.UnitTests.Entities.ReferenceDataEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 116);	// [L0AK3]
		public static readonly new string EntityStructuredTypeKey = "[L0AK3]";
	}
}
#endregion

#region Epsitec.Common.Support.UnitTests.CollectionData Entity
namespace Epsitec.Common.Support.UnitTests.Entities
{
	///	<summary>
	///	The <c>CollectionData</c> entity.
	///	designer:cap/L0AM3
	///	</summary>
	public partial class CollectionDataEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Collection</c> field.
		///	designer:fld/L0AM3/L0AN3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[L0AN3]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Common.Support.UnitTests.Entities.ValueDataEntity> Collection
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Common.Support.UnitTests.Entities.ValueDataEntity> ("[L0AN3]");
			}
		}
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Support.UnitTests.Entities.CollectionDataEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Support.UnitTests.Entities.CollectionDataEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (21, 10, 118);	// [L0AM3]
		public static readonly new string EntityStructuredTypeKey = "[L0AM3]";
	}
}
#endregion

