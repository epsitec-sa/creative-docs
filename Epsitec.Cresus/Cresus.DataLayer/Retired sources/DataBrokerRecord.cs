//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
#if false
	/// <summary>
	/// The <c>DataBrokerRecord</c> class implements a specialized structured
	/// data record (<see cref="StructuredData"/>) which links the data to a
	/// row in a data table through a <see cref="DataTableBroker"/>.
	/// </summary>
	public class DataBrokerRecord : StructuredData
	{
		public DataBrokerRecord(DataTableBroker broker)
			: base (broker)
		{
		}

		public DataTableBroker Broker
		{
			get
			{
				return (DataTableBroker) this.StructuredType;
			}
		}

		public long Id
		{
			get
			{
				return this.id;
			}
			internal set
			{
				this.id = value;
			}
		}


		public override StructuredData CreateEmptyCopy()
		{
			return new DataBrokerRecord (this.Broker);
		}

		public override void CopyContentsFrom(StructuredData data)
		{
			DataBrokerRecord record = data as DataBrokerRecord;

			if (record != null)
			{
				this.id = record.id;
			}
			
			base.CopyContentsFrom (data);
		}

		protected override void OnValueChanged(string id, object oldValue, object newValue)
		{
			//	Notify the broker that the record was edited and that the current
			//	instance should now be considered as writable, and written to, too.

			if (this.id > 0)
			{
				this.Broker.NotifyRecordChanged (this);
			}
			
			base.OnValueChanged (id, oldValue, newValue);
		}

		private long id;
	}
#endif
}
