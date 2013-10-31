//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.NaiveEngine
{
	public class DataEvent
	{
		public DataEvent(Timestamp timestamp, EventType type)
		{
			this.Guid      = Guid.NewGuid ();
			this.Timestamp = timestamp;
			this.Type      = type;

			this.properties = new List<AbstractDataProperty> ();
		}

		public readonly Guid					Guid;
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
			return this.properties.Where (x => x.Id == id).FirstOrDefault ();
		}


		private readonly List<AbstractDataProperty> properties;
	}
}
