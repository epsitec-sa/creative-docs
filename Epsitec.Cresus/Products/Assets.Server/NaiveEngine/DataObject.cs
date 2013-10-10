//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.NaiveEngine
{
	public class DataObject
	{
		public DataObject(int id)
		{
			this.Guid = Guid.NewGuid ();
			this.Id   = id;

			this.events = new List<DataEvent> ();
		}

		public readonly Guid					Guid;
		public readonly int						Id;

		public List<DataEvent>					Events
		{
			get
			{
				return this.events;
			}
		}

		public void AddEvent(DataEvent e)
		{
			int index = 0;

			for (int i=0; i<this.events.Count; i++)
			{
				if (this.events[i].Timestamp < e.Timestamp)
				{
					index = i;
					break;
				}
			}

			this.events.Insert (index, e);
		}

		public DataEvent GetEvent(Timestamp timestamp)
		{
			return this.events.Where (x => x.Timestamp == timestamp).FirstOrDefault ();
		}


		//	Retourne la propriété définie à la date exacte.
		public AbstractDataProperty GetSingleProperty(Timestamp timestamp, int id)
		{
			var e = this.GetEvent (timestamp);

			if (e != null)
			{
				return e.GetProperty (id);
			}

			return null;
		}

		//	Retourne la propriété définie à la date exacte ou antérieurement.
		public AbstractDataProperty GetSyntheticProperty(Timestamp timestamp, int id)
		{
			var p = this.GetSingleProperty (timestamp, id);

			if (p != null)
			{
				return p;
			}

			// Implémentation totalement naïve qui cherche depuis la date donnée
			// en remontant dans le passé.
			foreach (var e in this.events)
			{
				if (timestamp >= e.Timestamp)
				{
					p = e.GetProperty (id);

					if (p != null)
					{
						return p;
					}
				}
			}

			return null;
		}


		private readonly List<DataEvent> events;
	}
}
