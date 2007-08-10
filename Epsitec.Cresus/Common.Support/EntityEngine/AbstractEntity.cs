//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	public abstract class AbstractEntity
	{
		protected AbstractEntity()
		{
			this.context = EntityContext.Current;
			this.values = this.context.CreateValueStore (this);
		}

		public abstract Druid GetStructuredTypeId();

		internal IValueStore ValueStore
		{
			get
			{
				return this.values;
			}
		}

		protected T GetField<T>(string id)
		{
			StructuredTypeField field = this.context.GetStructuredType (this).GetField (id);

			System.Diagnostics.Debug.Assert (field != null);
			System.Diagnostics.Debug.Assert (field.Relation != FieldRelation.Collection);
			
			object value = this.values.GetValue (id);

			if ((UndefinedValue.IsUndefinedValue (value)) ||
				(value == null))
			{
				return default (T);
			}
			
			if (UnknownValue.IsUnknownValue (value))
			{
				throw new System.NotSupportedException (string.Format ("Field {0} not supported by value store", id));
			}

			return (T) value;
		}

		protected IList<T> GetFieldCollection<T>(string id)
		{
			object value = this.values.GetValue (id);
			IList<T> list = value as IList<T>;

			if (list == null)
			{
				if ((value != null) &&
					(!UndefinedValue.IsUndefinedValue (value)))
				{
					throw new System.NotSupportedException (string.Format ("Field {0} uses incompatible collection type", id));
				}

				//	The value store does not (yet) contain a collection for the
				//	specified items. We have to allocate one :

				list = new ObservableList<T> ();

				this.values.SetValue (id, list);
			}

			return list;
		}

		protected void SetField<T>(string id, T oldValue, T newValue)
		{
			StructuredTypeField field = this.context.GetStructuredType (this).GetField (id);

			System.Diagnostics.Debug.Assert (field != null);
			System.Diagnostics.Debug.Assert (field.Relation != FieldRelation.Collection);

			if ((field.IsNullable) &&
				(newValue == null))
			{
				//	The value is null and the field is nullable; this is operation
				//	is valid and it will clear the field.

				this.values.SetValue (id, UndefinedValue.Instance);
				return;
			}
			
			IDataConstraint constraint = field.Type as IDataConstraint;

			System.Diagnostics.Debug.Assert (constraint != null);

			if (constraint.IsValidValue (newValue))
			{
				this.values.SetValue (id, newValue == null ? UndefinedValue.Instance : (object) newValue);
			}
			else
			{
				throw new System.ArgumentException (string.Format ("Invalid value '{0}' specified for field {1}", newValue, id));
			}
		}

		private EntityContext context;
		private IValueStore values;
	}
}
