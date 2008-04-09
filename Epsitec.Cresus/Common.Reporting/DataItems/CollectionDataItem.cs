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
		public CollectionDataItem(DataViewContext context, System.Collections.IList collection)
		{
			this.collection = collection;
			this.DataView = new DataView (context);
		}

		public override bool IsCollection
		{
			get
			{
				return true;
			}
		}

		public override int Count
		{
			get
			{
				return this.collection.Count;
			}
		}

		public override object ObjectValue
		{
			get
			{
				return this.collection;
			}
		}


		public override object GetValue(int index)
		{
			return this.collection[index];
		}
		
		private readonly System.Collections.IList collection;
	}
}
