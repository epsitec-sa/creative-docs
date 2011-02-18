//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>GenericEntity</c> class provides a default wrapper for entities
	/// which cannot be resolved by the <see cref="EntityResolver"/> class.
	/// </summary>
	public class GenericEntity : AbstractEntity
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GenericEntity"/> class.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		public GenericEntity(Druid entityId)
		{
			this.entityId = entityId;
		}


		/// <summary>
		/// Gets the id of the <see cref="StructuredType"/> which describes
		/// this entity.
		/// </summary>
		/// <returns>
		/// The id of the <see cref="StructuredType"/>.
		/// </returns>
		public override Druid GetEntityStructuredTypeId()
		{
			return this.entityId;
		}

		/// <summary>
		/// Gets the key of the <see cref="StructuredType"/> which describes
		/// this entity. This is a textual representation of the underlying
		/// DRUID.
		/// </summary>
		/// <returns>
		/// The key of the <see cref="StructuredType"/>.
		/// </returns>
		public override string GetEntityStructuredTypeKey()
		{
			return this.entityId.ToString ();
		}


		/// <summary>
		/// Sets the value for the specified field, without any casting. Calls
		/// <c>InternalSetValue</c>, <c>UpdateDataGeneration</c> and <c>NotifyEventHandlers</c>.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected override void GenericSetValue(string id, object oldValue, object newValue)
		{
			if (this.entityId.IsValid)
			{
				base.GenericSetValue (id, oldValue, newValue);
			}
			else
			{
				//	If we have no associated schema information, then simply sets
				//	the value without any further checks.

				this.InternalSetValue (id, newValue);
				this.UpdateDataGeneration ();
				this.NotifyEventHandlers (id, oldValue, newValue);
			}
		}
		
		/// <summary>
		/// Gets the value for the specified field.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <returns>The value for the specified field.</returns>
		protected override object DynamicGetField(string id)
		{
			return this.GenericGetValue (id);
		}

		/// <summary>
		/// Set the value for the specified field.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <param name="newValue">The new value.</param>
		protected override void DynamicSetField(string id, object newValue)
		{
			if (UndefinedValue.IsUndefinedValue (newValue))
			{
				newValue = null;
			}

			this.GenericSetValue (id, this.InternalGetValue (id), newValue);
		}

		/// <summary>
		/// Asserts that the id identifies a simple field.
		/// </summary>
		/// <param name="id">The field id.</param>
		protected override void AssertSimpleField(string id)
		{
			if (this.entityId.IsValid)
			{
				base.AssertSimpleField (id);
			}
		}

		/// <summary>
		/// Asserts that the id identifies a collection field.
		/// </summary>
		/// <param name="id">The field id.</param>
		protected override void AssertCollectionField(string id)
		{
			if (this.entityId.IsValid)
			{
				base.AssertCollectionField (id);
			}
		}
		
		private readonly Druid entityId;
	}
}
