//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>AbstractEntityTable</c> class is the base class used to
	/// implement the generic <c>EntityTable</c> class.
	/// </summary>
	public abstract class AbstractEntityTable : InternalEntityBase, System.Collections.IEnumerable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractEntityTable"/> class.
		/// </summary>
		protected AbstractEntityTable()
		{
		}

		/// <summary>
		/// Gets the number of items in the table.
		/// </summary>
		/// <value>The item count.</value>
		public abstract int Count
		{
			get;
		}

		/// <summary>
		/// Gets or sets the title. The title may contain formatting (this is
		/// a full fledged tagged text).
		/// </summary>
		/// <value>The title encoded as a tagged text.</value>
		public virtual string Title
		{
			get
			{
				return this.title;
			}
			set
			{
				this.title = value;
			}
		}

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
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
		protected abstract System.Collections.IEnumerator GetEnumerator();

		private string title;
	}
}
