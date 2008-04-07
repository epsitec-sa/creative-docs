//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting
{
	internal class DataItem : IDataItem
	{
		public DataItem()
		{
			this.entity = null;
		}

		public DataItem(AbstractEntity entity)
		{
			this.entity = entity;
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


		private readonly AbstractEntity entity;
	}
}
