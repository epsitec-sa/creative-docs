//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.NaiveEngine
{
	public class DataEvent : IGuid
	{
		public DataEvent(Timestamp timestamp, EventType type)
		{
			this.guid      = Guid.NewGuid ();
			this.Timestamp = timestamp;
			this.Type      = type;

			this.properties = new List<AbstractDataProperty> ();
		}

		#region IGuid Members
		public Guid								Guid
		{
			get
			{
				return this.guid;
			}
		}
		#endregion

		public readonly Timestamp				Timestamp;
		public readonly EventType				Type;

		public List<AbstractDataProperty>		Properties
		{
			get
			{
				return this.properties;
			}
		}

		public AbstractDataProperty GetProperty(int id)
		{
			return this.properties.Where (x => x.FieldId == id).FirstOrDefault ();
		}


		private readonly Guid						guid;
		private readonly List<AbstractDataProperty>	properties;
	}
}
