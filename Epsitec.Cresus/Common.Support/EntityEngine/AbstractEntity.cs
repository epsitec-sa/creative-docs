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

		protected T GetField<T>(string id)
		{
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
			this.values.SetValue (id, newValue);
		}

		private EntityContext context;
		private IValueStore values;
	}
}
