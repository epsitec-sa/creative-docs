//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>SearchEntity</c> class is a façade to <see cref="AbstractEntity"/>
	/// with support for the <see cref="IFieldPropertyStore"/> interface.
	/// </summary>
	internal sealed class SearchEntity : AbstractEntity, IFieldPropertyStore
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchEntity"/> class.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		public SearchEntity(Druid entityId)
			: this (entityId, new GenericEntity (entityId))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchEntity"/> class.
		/// </summary>
		/// <param name="entity">The real entity.</param>
		public SearchEntity(AbstractEntity entity)
			: this (entity.GetEntityStructuredTypeId (), entity)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchEntity"/> class.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		/// <param name="entity">The real entity.</param>
		private SearchEntity(Druid entityId, AbstractEntity entity)
		{
			this.store    = new Dictionary<string, PropertyStore> ();
			this.entityId = entityId;
			this.target   = entity;
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

		
		protected override AbstractEntity Resolve()
		{
			return AbstractEntity.Resolve<AbstractEntity> (this.target);
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
			object oldValue = this.InternalGetValue (id);

#if false
			if (UndefinedValue.IsUndefinedValue (newValue))
			{
				newValue = null;
			}

			this.GenericSetValue (id, oldValue, newValue);
#else
			this.InternalSetValue (id, newValue);
			this.UpdateDataGeneration ();
			this.NotifyEventHandlers (id, oldValue, newValue);
#endif
		}

		#region IFieldPropertyStore Members

		/// <summary>
		/// Gets the value associated with the property of the specified field.
		/// </summary>
		/// <param name="fieldId">The field id.</param>
		/// <param name="property">The property.</param>
		/// <returns>
		/// The value or <c>UndefinedValue.Value</c> if it is not
		/// defined.
		/// </returns>
		public object GetValue(string fieldId, DependencyProperty property)
		{
			PropertyStore properties;

			if (this.store.TryGetValue (fieldId, out properties))
			{
				return properties[property];
			}
			else
			{
				return UndefinedValue.Value;
			}
		}

		/// <summary>
		/// Sets the value associated with the property of the specified field.
		/// </summary>
		/// <param name="fieldId">The field id.</param>
		/// <param name="property">The property.</param>
		/// <param name="value">The value to set.</param>
		public void SetValue(string fieldId, DependencyProperty property, object value)
		{
			PropertyStore properties;

			if (this.store.TryGetValue (fieldId, out properties))
			{
				properties[property] = value;
			}
			else if (UndefinedValue.IsUndefinedValue (value))
			{
				//	Undefining an undefined value does not do anything.
			}
			else
			{
				properties = new PropertyStore ();
				properties[property] = value;
				this.store[fieldId] = properties;
			}
		}

		/// <summary>
		/// Determines whether the specified field contains a value for the
		/// specified property.
		/// </summary>
		/// <param name="fieldId">The field id.</param>
		/// <param name="property">The property.</param>
		/// <returns>
		/// 	<c>true</c> if the specified field contains a value for the
		/// specified property; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsValue(string fieldId, DependencyProperty property)
		{
			PropertyStore properties;

			if (this.store.TryGetValue (fieldId, out properties))
			{
				return properties.ContainsKey (property);
			}
			else
			{
				return false;
			}
		}

		#endregion

		#region Private PropertyStore Class

		/// <summary>
		/// The <c>PropertyStore</c> class stores values based on properties.
		/// </summary>
		private sealed class PropertyStore : Dictionary<DependencyProperty, object>
		{
			public PropertyStore()
			{
			}

			public new object this[DependencyProperty property]
			{
				get
				{
					object value;

					if (this.TryGetValue (property, out value))
					{
						return value;
					}
					else
					{
						return UndefinedValue.Value;
					}
				}
				set
				{
					if (UndefinedValue.IsUndefinedValue (value))
					{
						this.Remove (property);
					}
					else
					{
						base[property] = value;
					}
				}
			}
		}

		#endregion


		private readonly Dictionary<string, PropertyStore> store;
		private readonly Druid					entityId;
		private readonly AbstractEntity			target;
	}
}
