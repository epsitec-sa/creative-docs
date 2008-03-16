//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>SearchEntity</c> class is a <see cref="GenericEntity"/> with
	/// additional support for the <see cref="IFieldPropertyStore"/> interface.
	/// </summary>
	internal sealed class SearchEntity : GenericEntity, IFieldPropertyStore
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchEntity"/> class.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		public SearchEntity(Druid entityId)
			: base (entityId)
		{
			this.store = new Dictionary<string, PropertyStore> ();
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
						base.Remove (property);
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
	}
}
