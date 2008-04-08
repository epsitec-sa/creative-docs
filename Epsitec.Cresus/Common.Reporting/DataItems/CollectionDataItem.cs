//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting.DataItems
{
	class CollectionDataItem : DataView.DataItem
	{
		public CollectionDataItem(System.Collections.IList collection)
		{
			this.collection = collection;
		}

		public override object ObjectValue
		{
			get
			{
				return this.collection;
			}
		}
		
		private readonly System.Collections.IList collection;
	}
}
