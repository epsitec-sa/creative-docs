//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting.DataItems
{
	/// <summary>
	/// The <c>ValueDataItem</c> class represents items which map to simple
	/// values (these are leaf nodes in the data graph).
	/// </summary>
	class ValueDataItem : DataView.DataItem
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ValueDataItem"/> class.
		/// </summary>
		/// <param name="value">The value.</param>
		public ValueDataItem(object value)
		{
			this.value = value;
		}

		/// <summary>
		/// Gets the raw object value.
		/// </summary>
		/// <value>The raw object value.</value>
		public override object ObjectValue
		{
			get
			{
				return this.value;
			}
		}

		/// <summary>
		/// Gets the type of the item.
		/// </summary>
		/// <value>Always <c>DataItemType.Value</c>.</value>
		public override DataItemType ItemType
		{
			get
			{
				return DataItemType.Value;
			}
		}

		private readonly object value;
	}
}
