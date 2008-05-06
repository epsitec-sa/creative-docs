//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Reporting
{
	public struct DataNode
	{
		internal IDataItem DataItem
		{
			get;
			set;
		}

		public string Value
		{
			get
			{
				return this.DataItem.Value;
			}
		}

		public DataItemClass ItemClass
		{
			get
			{
				return this.DataItem.ItemClass;
			}
		}

		public DataItemType ItemType
		{
			get
			{
				return this.DataItem.ItemType;
			}
		}

		public DataNodeType NodeType
		{
			get;
			internal set;
		}
	}
}
