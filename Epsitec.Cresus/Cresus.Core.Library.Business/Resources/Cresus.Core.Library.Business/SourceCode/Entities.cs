//  --------------------------------------------------------------------------- 
//  ATTENTION !
//  Ce fichier a été généré automatiquement. Ne pas l'éditer manuellement, car 
//  toute modification sera perdue. 
//  --------------------------------------------------------------------------- 

[assembly: global::Epsitec.Common.Support.EntityClass ("[GVA]", typeof (Epsitec.Cresus.Core.Entities.RelationEntity))]
[assembly: global::Epsitec.Common.Support.EntityClass ("[GVA1]", typeof (Epsitec.Cresus.Core.Entities.AffairEntity))]
#region Epsitec.Cresus.Core.Relation Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>Relation</c> entity.
	///	designer:cap/GVA
	///	</summary>
	public partial class RelationEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Person</c> field.
		///	designer:fld/GVA/GVA3
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA3]")]
		public global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity Person
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity> ("[GVA3]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity oldValue = this.Person;
				if (oldValue != value || !this.IsFieldDefined("[GVA3]"))
				{
					this.OnPersonChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity> ("[GVA3]", oldValue, value);
					this.OnPersonChanged (oldValue, value);
				}
			}
		}
		
		partial void OnPersonChanging(global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity newValue);
		partial void OnPersonChanged(global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity oldValue, global::Epsitec.Cresus.Core.Entities.AbstractPersonEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.RelationEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.RelationEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 0);	// [GVA]
		public static readonly new string EntityStructuredTypeKey = "[GVA]";
	}
}
#endregion

#region Epsitec.Cresus.Core.Affair Entity
namespace Epsitec.Cresus.Core.Entities
{
	///	<summary>
	///	The <c>Affair</c> entity.
	///	designer:cap/GVA1
	///	</summary>
	public partial class AffairEntity : global::Epsitec.Common.Support.EntityEngine.AbstractEntity
	{
		///	<summary>
		///	The <c>Relation</c> field.
		///	designer:fld/GVA1/GVA2
		///	</summary>
		[global::Epsitec.Common.Support.EntityField ("[GVA2]")]
		public global::Epsitec.Cresus.Core.Entities.RelationEntity Relation
		{
			get
			{
				return this.GetField<global::Epsitec.Cresus.Core.Entities.RelationEntity> ("[GVA2]");
			}
			set
			{
				global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue = this.Relation;
				if (oldValue != value || !this.IsFieldDefined("[GVA2]"))
				{
					this.OnRelationChanging (oldValue, value);
					this.SetField<global::Epsitec.Cresus.Core.Entities.RelationEntity> ("[GVA2]", oldValue, value);
					this.OnRelationChanged (oldValue, value);
				}
			}
		}
		
		partial void OnRelationChanging(global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue, global::Epsitec.Cresus.Core.Entities.RelationEntity newValue);
		partial void OnRelationChanged(global::Epsitec.Cresus.Core.Entities.RelationEntity oldValue, global::Epsitec.Cresus.Core.Entities.RelationEntity newValue);
		
		public override global::Epsitec.Common.Support.Druid GetEntityStructuredTypeId()
		{
			return global::Epsitec.Cresus.Core.Entities.AffairEntity.EntityStructuredTypeId;
		}
		public override string GetEntityStructuredTypeKey()
		{
			return global::Epsitec.Cresus.Core.Entities.AffairEntity.EntityStructuredTypeKey;
		}
		public static readonly new global::Epsitec.Common.Support.Druid EntityStructuredTypeId = new global::Epsitec.Common.Support.Druid (1008, 10, 1);	// [GVA1]
		public static readonly new string EntityStructuredTypeKey = "[GVA1]";
	}
}
#endregion

