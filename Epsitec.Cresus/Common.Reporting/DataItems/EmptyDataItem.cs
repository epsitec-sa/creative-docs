//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting.DataItems
{
	/// <summary>
	/// The <c>EmptyDataItem</c> class represents an empty data item (i.e. an
	/// undefined, unknown or null value).
	/// </summary>
	class EmptyDataItem : DataView.DataItem
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EmptyDataItem"/> class.
		/// </summary>
		public EmptyDataItem()
		{
		}

		/// <summary>
		/// Gets the raw object value.
		/// </summary>
		/// <value>The raw object value, which is always <c>null</c>.</value>
		public override object ObjectValue
		{
			get
			{
				return null;
			}
		}


		public static readonly EmptyDataItem Value = new EmptyDataItem ();
	}
}
