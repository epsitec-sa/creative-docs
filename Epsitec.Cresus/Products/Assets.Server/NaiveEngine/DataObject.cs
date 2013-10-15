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
			int i = 0;
			while (i < this.events.Count)
			{
				if (this.events[i].Timestamp > e.Timestamp)
				{
					i--;
					break;
				}

				i++;
			}

			this.events.Insert (i, e);
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
				var p = e.GetProperty (id);
				if (p != null)
				{
					p.State = PropertyState.Single;
					return p;
				}
			}

			return null;
		}

		//	Retourne la propriété définie à la date exacte ou antérieurement.
		public AbstractDataProperty GetSyntheticProperty(Timestamp timestamp, int id)
		{
			var p = this.GetSingleProperty (timestamp, id);

			if (p != null)
			{
				p.State = PropertyState.Single;
				return p;
			}

			// Implémentation totalement naïve qui cherche depuis la date donnée
			// en remontant dans le passé.
			var e = this.events
				.Where (x => x.Timestamp <= timestamp && x.GetProperty (id) != null)
				.LastOrDefault ();

			if (e != null)
			{
				p = e.GetProperty (id);
				if (p != null)
				{
					p.State = PropertyState.Synthetic;
					return p;
				}
			}

			return null;
		}


		private readonly List<DataEvent> events;
	}
}
