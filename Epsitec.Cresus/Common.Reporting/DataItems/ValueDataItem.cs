//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting.DataItems
{
	class ValueDataItem : DataView.DataItem
	{
		public ValueDataItem(object value)
		{
			this.value = value;
		}

		public override object ObjectValue
		{
			get
			{
				return this.value;
			}
		}

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
