//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>AbstractEntity</c> class is the base class used to store the
	/// data represented by entity instances.
	/// </summary>
	public abstract class AbstractEntity
	{
		protected AbstractEntity()
		{
			this.context = EntityContext.Current;
			this.originalValues = this.context.CreateValueStore (this);
			this.modifiedValues = null;
		}

		/// <summary>
		/// Gets the id of the <see cref="StructuredType"/> which describes
		/// this entity.
		/// </summary>
		/// <returns>The id of the <see cref="StructuredType"/>.</returns>
		public abstract Druid GetStructuredTypeId();

		internal IValueStore OriginalValueStore
		{
			get
			{
				return this.originalValues;
			}
		}

		internal IValueStore ModifiedValueStore
		{
			get
			{
				return this.modifiedValues;
			}
		}

		public T GetField<T>(string id)
		{
			StructuredTypeField field = this.context.GetStructuredType (this).GetField (id);

			System.Diagnostics.Debug.Assert (field != null);
			System.Diagnostics.Debug.Assert (field.Relation != FieldRelation.Collection);

			object value;

			if (this.modifiedValues != null)
			{
				value = this.modifiedValues.GetValue (id);

				if (UndefinedValue.IsUndefinedValue (value))
				{
					value = this.originalValues.GetValue (id);
				}
			}
			else
			{
				value = this.originalValues.GetValue (id);
			}

			if (UnknownValue.IsUnknownValue (value))
			{
				throw new System.NotSupportedException (string.Format ("Field {0} not supported by value store", id));
			}

			if (UndefinedValue.IsUndefinedValue (value))
			{
				return default (T);
			}
			
			return (T) value;
		}

		public IList<T> GetFieldCollection<T>(string id) where T : AbstractEntity
		{
			object value;

			if (this.modifiedValues != null)
			{
				value = this.modifiedValues.GetValue (id);

				if (UndefinedValue.IsUndefinedValue (value))
				{
					value = this.originalValues.GetValue (id);
				}
			}
			else
			{
				value = this.originalValues.GetValue (id);
			}

			IList<T> list = value as IList<T>;

			if (list == null)
			{
				if (UndefinedValue.IsUndefinedValue (value))
				{
					//	The value store does not (yet) contain a collection for the
					//	specified items. We have to allocate one :

					list = new EntityCollection<T> (id, this);

					this.originalValues.SetValue (id, list);
				}
				else
				{
					throw new System.NotSupportedException (string.Format ("Field {0} uses incompatible collection type", id));
				}
			}

			return list;
		}

		internal EntityCollection<T> CopyFieldCollection<T>(string id, EntityCollection<T> collection) where T : AbstractEntity
		{
			EntityCollection<T> copy = new EntityCollection<T> (id, this);

			copy.AddRange (collection);

			this.InternalSetValue (id, copy);

			return copy;
		}

		public void SetField<T>(string id, T oldValue, T newValue)
		{
			StructuredTypeField field = this.context.GetStructuredType (this).GetField (id);

			System.Diagnostics.Debug.Assert (field != null);
			System.Diagnostics.Debug.Assert (field.Relation != FieldRelation.Collection);

			if ((field.IsNullable) &&
				(newValue == null))
			{
				//	The value is null and the field is nullable; this operation
				//	is valid and it will clear the field.

				this.InternalSetValue (id, UndefinedValue.Instance);
				return;
			}
			
			IDataConstraint constraint = field.Type as IDataConstraint;

			System.Diagnostics.Debug.Assert (constraint != null);

			if (constraint.IsValidValue (newValue))
			{
				object value;
				
				if (newValue == null)
				{
					value = UndefinedValue.Instance;
				}
				else
				{
					value = (object) newValue;
				}

				this.InternalSetValue (id, value);
			}
			else
			{
				throw new System.ArgumentException (string.Format ("Invalid value '{0}' specified for field {1}", newValue, id));
			}
		}

		private void InternalSetValue(string id, object value)
		{
			if (this.modifiedValues == null)
			{
				this.modifiedValues = this.context.CreateValueStore (this);
			}

			this.modifiedValues.SetValue (id, value);
		}

		public static TResult GetCalculation<T, TResult>(T entity, string id, System.Func<T, TResult> func)
		{
			return func (entity);
		}

		private EntityContext context;
		private IValueStore originalValues;
		private IValueStore modifiedValues;
	}
}
