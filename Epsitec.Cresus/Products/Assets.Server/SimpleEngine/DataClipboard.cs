//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public class DataClipboard
	{
		public bool								HasEvent
		{
			get
			{
				return this.dataEvent != null;
			}
		}

		public Timestamp?						EventTimestamp
		{
			get
			{
				if (this.dataEvent == null)
				{
					return null;
				}
				else
				{
					return this.dataEvent.Timestamp;
				}
			}
		}

		public EventType						EventType
		{
			get
			{
				if (this.dataEvent == null)
				{
					return EventType.Unknown;
				}
				else
				{
					return this.dataEvent.Type;
				}
			}
		}

		public void CopyEvent(DataAccessor accessor, DataEvent e)
		{
			this.eventGuidMandat = accessor.Mandat.Guid;
			this.dataEvent = e;
		}

		public DataEvent PasteEvent(DataAccessor accessor, DataObject obj, System.DateTime date)
		{
			if (accessor.Mandat.Guid != this.eventGuidMandat)
			{
				return null;
			}

			var e = accessor.CreateAssetEvent (obj, date, this.dataEvent.Type);
			e.SetUndefinedProperties (this.dataEvent);

			Amortizations.UpdateAmounts (accessor, obj);

			return e;
		}


		private Guid							eventGuidMandat;
		private DataEvent						dataEvent;
	}
}
