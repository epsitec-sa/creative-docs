//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting.DataItems
{
	class CollectionDataItem : IDataItem
	{
		public CollectionDataItem(System.Collections.IList collection)
		{
			this.collection = collection;
		}

		#region IDataItem Members

		public string Value
		{
			get
			{
				throw new System.NotImplementedException ();
			}
		}

		public int Count
		{
			get
			{
				throw new System.NotImplementedException ();
			}
		}

		public DataItemClass ItemClass
		{
			get
			{
				throw new System.NotImplementedException ();
			}
		}

		public DataItemType ItemType
		{
			get
			{
				throw new System.NotImplementedException ();
			}
		}

		public INamedType DataType
		{
			get
			{
				throw new System.NotImplementedException ();
			}
		}

		#endregion

		private readonly System.Collections.IList collection;
	}
}
