//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[I1A]", typeof (Epsitec.Common.Tests.Vs.Entities.ValueDataEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[I1A3]", typeof (Epsitec.Common.Tests.Vs.Entities.ReferenceDataEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[I1A5]", typeof (Epsitec.Common.Tests.Vs.Entities.CollectionDataEntity))]
#region Epsitec.Common.Tests.Vs.ValueData Entity
namespace Epsitec.Common.Tests.Vs.Entities
{
	///	<summary>
	///	The <c>ValueData</c> entity.
	///	designer:cap/I1A
	///	</summary>
	public partial class ValueDataEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Value</c> field.
		///	designer:fld/I1A/I1A1
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[I1A1]")]
		public int Value
		{
			get
			{
				return this.GetField<int> ("[I1A1]");
			}
			set
			{
				int oldValue = this.Value;
				if (oldValue != value || !this.IsFieldDefined("[I1A1]"))
				{
					this.OnValueChanging (oldValue, value);
					this.SetField<int> ("[I1A1]", oldValue, value);
					this.OnValueChanged (oldValue, value);
				}
			}
		}
		///	<summary>
		///	The <c>NullableValue</c> field.
		///	designer:fld/I1A/I1A2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[I1A2]")]
		public int? NullableValue
		{
			get
			{
				return this.GetField<int?> ("[I1A2]");
			}
			set
			{
				int? oldValue = this.NullableValue;
				if (oldValue != value || !this.IsFieldDefined("[I1A2]"))
				{
					this.OnNullableValueChanging (oldValue, value);
					this.SetField<int?> ("[I1A2]", oldValue, value);
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
			return global::Epsitec.Common.Tests.Vs.Entities.ValueDataEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Tests.Vs.Entities.ValueDataEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (50, 10, 0);	// [I1A]
		public static readonly string EntityStructuredTypeKey = "[I1A]";
	}
}
#endregion

#region Epsitec.Common.Tests.Vs.ReferenceData Entity
namespace Epsitec.Common.Tests.Vs.Entities
{
	///	<summary>
	///	The <c>ReferenceData</c> entity.
	///	designer:cap/I1A3
	///	</summary>
	public partial class ReferenceDataEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Reference</c> field.
		///	designer:fld/I1A3/I1A4
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[I1A4]")]
		public global::Epsitec.Common.Tests.Vs.Entities.ValueDataEntity Reference
		{
			get
			{
				return this.GetField<global::Epsitec.Common.Tests.Vs.Entities.ValueDataEntity> ("[I1A4]");
			}
			set
			{
				global::Epsitec.Common.Tests.Vs.Entities.ValueDataEntity oldValue = this.Reference;
				if (oldValue != value || !this.IsFieldDefined("[I1A4]"))
				{
					this.OnReferenceChanging (oldValue, value);
					this.SetField<global::Epsitec.Common.Tests.Vs.Entities.ValueDataEntity> ("[I1A4]", oldValue, value);
					this.OnReferenceChanged (oldValue, value);
				}
			}
		}
		
		partial void OnReferenceChanging(global::Epsitec.Common.Tests.Vs.Entities.ValueDataEntity oldValue, global::Epsitec.Common.Tests.Vs.Entities.ValueDataEntity newValue);
		partial void OnReferenceChanged(global::Epsitec.Common.Tests.Vs.Entities.ValueDataEntity oldValue, global::Epsitec.Common.Tests.Vs.Entities.ValueDataEntity newValue);
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Tests.Vs.Entities.ReferenceDataEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Tests.Vs.Entities.ReferenceDataEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (50, 10, 3);	// [I1A3]
		public static readonly string EntityStructuredTypeKey = "[I1A3]";
	}
}
#endregion

#region Epsitec.Common.Tests.Vs.CollectionData Entity
namespace Epsitec.Common.Tests.Vs.Entities
{
	///	<summary>
	///	The <c>CollectionData</c> entity.
	///	designer:cap/I1A5
	///	</summary>
	public partial class CollectionDataEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Collection</c> field.
		///	designer:fld/I1A5/I1A6
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[I1A6]")]
		public global::System.Collections.Generic.IList<global::Epsitec.Common.Tests.Vs.Entities.ValueDataEntity> Collection
		{
			get
			{
				return this.GetFieldCollection<global::Epsitec.Common.Tests.Vs.Entities.ValueDataEntity> ("[I1A6]");
			}
		}
		
		
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Common.Tests.Vs.Entities.CollectionDataEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Common.Tests.Vs.Entities.CollectionDataEntity.EntityStructuredTypeKey;
		}
		public static readonly global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (50, 10, 5);	// [I1A5]
		public static readonly string EntityStructuredTypeKey = "[I1A5]";
	}
}
#endregion

