//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// The <c>DataBrokerItem</c> structure stores a reference to a piece of
	/// structured data, which can either be read only (in which case it is
	/// volatile and subject to garbage collection) or writable (in which case
	/// it is persistent).
	/// </summary>
	public struct DataBrokerItem
	{
		public long								Id
		{
			get
			{
				return this.id;
			}
		}

		public DataBrokerRecord					Data
		{
			get
			{
				if (this.writableData != null)
				{
					return this.writableData;
				}
				else if (this.data != null)
				{
					return this.data.Target;
				}
				else
				{
					return null;
				}
			}
		}

		public bool								IsReadOnly
		{
			get
			{
				return this.writableData == null;
			}
		}

		public bool								IsWritable
		{
			get
			{
				return this.writableData != null;
			}
		}

		public bool								IsAlive
		{
			get
			{
				if (this.writableData != null)
				{
					return true;
				}

				if ((this.data != null) &&
					(this.data.IsAlive))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public static DataBrokerItem CreateReadOnlyItem(DataBrokerRecord data)
		{
			DataBrokerItem item = new DataBrokerItem ();

			item.id   = data.Id;
			item.data = new Weak<DataBrokerRecord> (data);

			return item;
		}

		public static DataBrokerItem CreateWritableItem(DataBrokerRecord data)
		{
			DataBrokerItem item = new DataBrokerItem ();

			item.id   = data.Id;
			item.data = null;
			
			item.writableData = data;

			return item;
		}

		private long id;
		private Weak<DataBrokerRecord> data;
		private DataBrokerRecord writableData;
	}
}
