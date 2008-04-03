//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityTable&lt;T&gt;</c> class represents a collection of
	/// entities which can be represented as a table.
	/// </summary>
	/// <typeparam name="T">The entity type, derived from <see cref="AbstractEntity"/>.</typeparam>
	public class EntityTable<T> : AbstractEntityTable, IEnumerable<T>, System.Collections.IEnumerable where T : AbstractEntity, new ()
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityTable&lt;T&gt;"/> class.
		/// </summary>
		public EntityTable()
		{
			this.values = new List<T> ();
		}

		/// <summary>
		/// Gets the number of items in the table.
		/// </summary>
		/// <value>The item count.</value>
		public override int Count
		{
			get
			{
				return this.values.Count;
			}
		}

		/// <summary>
		/// Gets the id of the <see cref="StructuredType"/> which describes
		/// the entities stored in this table.
		/// </summary>
		/// <returns>
		/// The id of the <see cref="StructuredType"/>.
		/// </returns>
		public override Druid GetEntityStructuredTypeId()
		{
			return EntityTable<T>.entityId;
		}


		/// <summary>
		/// Adds the specified item to the table.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Add(T item)
		{
			this.values.Add (item);
		}

		/// <summary>
		/// Adds the specified items to the table.
		/// </summary>
		/// <param name="collection">The item collection.</param>
		public void AddRange(IEnumerable<T> collection)
		{
			this.values.AddRange (collection);
		}

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return this.values.GetEnumerator ();
		}

		#endregion

		/// <summary>
		/// Returns an enumerator that iterates through the collection of
		/// entities
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be
		/// used to iterate through the collection of entities.
		/// </returns>
		protected override System.Collections.IEnumerator GetEnumerator()
		{
			System.Collections.IList list = this.values;
			return list.GetEnumerator ();
		}

		private static readonly Druid entityId = EntityClassResolver.FindEntityId (typeof (T));
		
		private readonly List<T> values;
	}
}
