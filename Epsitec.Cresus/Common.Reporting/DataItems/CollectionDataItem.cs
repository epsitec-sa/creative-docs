//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting.DataItems
{
	/// <summary>
	/// The <c>CollectionDataItem</c> class represents the base class for all
	/// items which map to a collection.
	/// </summary>
	abstract class CollectionDataItem : DataView.DataItem
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionDataItem"/> class.
		/// </summary>
		/// <param name="context">The data view context.</param>
		/// <param name="collection">The collection of items.</param>
		public CollectionDataItem(DataViewContext context, System.Collections.IList collection)
		{
			this.collection = collection;
			this.DataView = new DataView (context);
		}

		/// <summary>
		/// Gets a value indicating whether this instance is collection.
		/// </summary>
		/// <value>
		/// 	Always <c>true</c>.
		/// </value>
		public override bool IsCollection
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets the number of items in this collection.
		/// </summary>
		/// <value>The number of items in this collection.</value>
		public override int Count
		{
			get
			{
				return this.collection.Count;
			}
		}

		/// <summary>
		/// Gets the raw object value.
		/// </summary>
		/// <value>The raw object value.</value>
		public override object ObjectValue
		{
			get
			{
				return this.collection;
			}
		}


		/// <summary>
		/// Gets the value of the item at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The value for the specified index.</returns>
		public override object GetValue(int index)
		{
			return this.collection[index];
		}
		
		private readonly System.Collections.IList collection;
	}
}
